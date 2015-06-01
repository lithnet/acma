// -----------------------------------------------------------------------
// <copyright file="UINotifyPropertyChanges.cs" company="Lithnet">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Common.ObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Reflection;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Text;


    /// <summary>
    /// Provides an abstract implementation of the INotifyPropertyChanged and IDataErrorInfo interfaces
    /// </summary>
    //[System.Diagnostics.DebuggerStepThrough()]
    //[DataContract(IsReference = true)]
    [DataContract]
    public abstract class UINotifyPropertyChanges : INotifyPropertyChanged, IDataErrorInfo
    {
        private static int ignoreAllChangesCount = 0;

        private static bool ignoreAllChanges;

        private bool ignoreChanges;

        private List<UINotifyPropertyChanges> inheritErrorsFrom;

        private List<UINotifyPropertyChanges> inheritHasChangedFrom;

        private Dictionary<string, List<string>> dependentProperties = new Dictionary<string, List<string>>();

        /// <summary>
        /// Occurs when a property value changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangedEventHandler OnErrorChanged;

        public UINotifyPropertyChanges()
        {
            this.Initialize();
        }

        [PropertyChanged.DoNotNotify]
        public List<string> IgnorePropertyHasChanged { get; set; }

        /// <summary>
        /// Gets an error message indicating what is wrong with this object
        /// </summary>
        public string Error
        {
            get
            {
                if (this.ErrorCount == 0)
                {
                    return null;
                }

                StringBuilder builder = new StringBuilder();
                foreach (var item in this.ErrorList)
                {
                    builder.AppendFormat("{0}\n", item.Value);
                }

                if (this.inheritErrorsFrom != null)
                {
                    foreach (UINotifyPropertyChanges errorProvider in this.inheritErrorsFrom)
                    {
                        foreach (var item in errorProvider.ErrorList)
                        {
                            builder.AppendFormat("{0}\n", item.Value);
                        }
                    }
                }

                return builder.ToString();
            }
        }

        [PropertyChanged.DoNotNotify]
        public int ErrorCount
        {
            get
            {
                int count = 0;

                if (this.inheritErrorsFrom != null)
                {
                    foreach (UINotifyPropertyChanges errorProvider in this.inheritErrorsFrom)
                    {
                        count += errorProvider.ErrorCount;
                    }
                }

                count += this.ErrorList.Count;

                return count;
            }
        }

        [PropertyChanged.DoNotNotify]
        public virtual bool HasErrors
        {
            get
            {
                return this.ErrorCount > 0;
            }
        }

        [PropertyChanged.DoNotNotify]
        public Dictionary<string, string> ErrorList { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether any of the query properties has changed
        /// </summary>
        [PropertyChanged.DoNotNotify]
        public bool HasOwnChanges { get; protected set; }

        [PropertyChanged.DoNotNotify]
        public bool SetHasChangedOnPropertyChange { get; set; }

        [PropertyChanged.DoNotNotify]
        public bool IgnoreOwnPropertyChanges { get; set; }

        public bool HasInheritedChanges
        {
            get
            {
                return this.inheritHasChangedFrom.Any(t => t.HasAnyChanges);
            }
        }

        public void BeginIgnoreChanges()
        {
            this.ignoreChanges = true;
        }

        public void EndIgnoreChanges()
        {
            this.ignoreChanges = false;
        }

        public static void BeginIgnoreAllChanges()
        {
            UINotifyPropertyChanges.ignoreAllChangesCount++;
            UINotifyPropertyChanges.ignoreAllChanges = true;
        }

        public static void EndIgnoreAllChanges()
        {
            if (UINotifyPropertyChanges.ignoreAllChangesCount <= 1)
            {
                UINotifyPropertyChanges.ignoreAllChangesCount = 0;
                UINotifyPropertyChanges.ignoreAllChanges = false;
            }
            else
            {
                UINotifyPropertyChanges.ignoreAllChangesCount--;
            }
        }

        public bool ThrowExceptionOnValidationError { get; set; }

        public virtual string this[string propertyName]
        {
            get
            {
                if (this.inheritErrorsFrom.Count == 0)
                {
                    return this.GetLocalError(propertyName);
                }
                else
                {
                    return this.GetLocalError(propertyName) ?? this.GetInheritedError(propertyName);
                }
            }
        }

        public void AddDependentPropertyNotification(string propertyName, string dependentProperty)
        {
            if (!this.dependentProperties.ContainsKey(propertyName))
            {
                this.dependentProperties.Add(propertyName, new List<string>());
            }

            if (this.dependentProperties[propertyName] == null)
            {
                this.dependentProperties[propertyName] = new List<string>();
            }

            this.dependentProperties[propertyName].Add(dependentProperty);
        }

        public virtual void ResetChangeState()
        {
            this.ignoreChanges = true;

            foreach (var item in this.inheritHasChangedFrom)
            {
                item.ResetChangeState();
            }

            this.ignoreChanges = false;

            this.HasOwnChanges = false;
            // Debug.WriteLine("Reset change state event: {0}. HasOwnChanges: {1}, HasInheritedChanges {2}", this.GetType().Name, this.HasOwnChanges, this.HasInheritedChanges);

            this.RaisePropertyChanged("HasAnyChanges", false, this);
            this.RaisePropertyChanged("HasOwnChanges", false, this);
        }

        protected virtual void ValidatePropertyChange(string propertyName)
        {
        }

        /// <summary>
        /// Adds a property error to the internal error list
        /// </summary>
        /// <param name="propertyName">The name of the property whose error message to set</param>
        /// <param name="error">The error message to set against the specified property</param>
        protected void AddError(string propertyName, string error)
        {
            if (!this.ErrorList.ContainsKey(propertyName))
            {
                this.ErrorList.Add(propertyName, error);
            }
            else
            {
                this.ErrorList[propertyName] = error;
            }

            if (this.ThrowExceptionOnValidationError)
            {
                throw new ValidationException(propertyName, error);
            }

            // Debug.WriteLine("Added error {0}.{1} - {2}", this.GetType().Name, propertyName, error);
            this.RaiseErrorChanged(propertyName);
        }

        /// <summary>
        /// Removes a property error from the internal error list
        /// </summary>
        /// <param name="propertyName">The name of the property whose error message to clear</param>
        protected void RemoveError(string propertyName)
        {
            if (this.ErrorList.ContainsKey(propertyName))
            {
                this.ErrorList.Remove(propertyName);
                // Debug.WriteLine("Removed error {0}.{1}", this.GetType().Name, propertyName);
                this.RaiseErrorChanged(propertyName);
            }
        }

        private bool IgnoreHasChanged(string propertyName)
        {
            return this.IgnorePropertyHasChanged.Any(t => t.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
        }

        public void InheritPropertyChanges(INotifyPropertyChanged inheritFrom)
        {
            inheritFrom.PropertyChanged += this.propertyChangeObject_PropertyChanged;
        }

        public void UninheritPropertyChanges(INotifyPropertyChanged inheritFrom)
        {
            inheritFrom.PropertyChanged -= this.propertyChangeObject_PropertyChanged;
        }

        public void InheritHasChanged(UINotifyPropertyChanges inheritFrom)
        {
            if (inheritFrom == null)
            {
                throw new ArgumentNullException("inheritFrom");
            }

            if (!this.inheritHasChangedFrom.Contains(inheritFrom))
            {
                this.inheritHasChangedFrom.Add(inheritFrom);
                inheritFrom.PropertyChanged += inheritedHasChanged_PropertyChanged;
            }
        }

        private void inheritedHasChanged_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is UINotifyPropertyChanges && ((UINotifyPropertyChanges)sender).HasAnyChanges)
            {
                this.RaisePropertyChanged("HasAnyChanges", false, sender);
            }
        }

        public bool HasAnyChanges
        {
            get
            {
                return this.HasOwnChanges || this.HasInheritedChanges;
            }
        }

        public void UnsubscribeFromHasChanged(UINotifyPropertyChanges subscription)
        {
            if (subscription == null)
            {
                throw new ArgumentNullException("subscription");
            }

            if (this.inheritHasChangedFrom.Contains(subscription))
            {
                this.inheritHasChangedFrom.Remove(subscription);
            }
        }

        public void SubscribeToErrors(UINotifyPropertyChanges subscription)
        {
            if (subscription == null)
            {
                throw new ArgumentNullException("subscription");
            }

            if (!this.inheritErrorsFrom.Contains(subscription))
            {
                this.inheritErrorsFrom.Add(subscription);
                subscription.OnErrorChanged += errorInheritFrom_ErrorChanged;
            }
        }

        public void UnsubscribeFromErrors(UINotifyPropertyChanges subscription)
        {
            if (subscription == null)
            {
                throw new ArgumentNullException("subscription");
            }

            if (this.inheritErrorsFrom.Contains(subscription))
            {
                this.inheritErrorsFrom.Remove(subscription);
                subscription.OnErrorChanged -= errorInheritFrom_ErrorChanged;
            }
        }

        private void propertyChangeObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(e.PropertyName, false, sender);
        }

        private void errorInheritFrom_ErrorChanged(object sender, PropertyChangedEventArgs e)
        {
#if DEBUG
            // Debug.WriteLine("Inherited HasErrors event: {0}.{1} from {2}", new object[] { this.GetType().Name, e.PropertyName, sender.GetType().Name });
#endif
            this.RaiseErrorChanged(e.PropertyName);
        }

        private void SetHasChanged(string propertyName)
        {
            if (!this.SetHasChangedOnPropertyChange || this.ignoreChanges || UINotifyPropertyChanges.ignoreAllChanges)
            {
                // Debug.WriteLine("Discarding HasChanged for: {0}.{1}", this.GetType().Name, propertyName);
                return;
            }

            if (!this.IgnoreHasChanged(propertyName))
            {
                // Debug.WriteLine("Setting HasChanged for: {0}.{1}", this.GetType().Name, propertyName);
                this.HasOwnChanges = true;
                //this.RaisePropertyChanged("HasAnyChanges", false, this);
            }
        }

        /// <summary>
        /// Raises a PropertyChanged event for the specified property
        /// </summary>
        /// <param name="propertyName">The name of the property to raise a PropertyChanged event against</param>
        protected void RaiseErrorChanged(string propertyName)
        {
            //Debug.WriteLine("HasError change event: {0}.{1}", this.GetType().Name, propertyName);
            this.RaisePropertyChanged("HasErrors", false, this);
            this.RaisePropertyChanged("Error", false, this);

            if (this.OnErrorChanged != null)
            {
                this.OnErrorChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            this.RaisePropertyChanged(propertyName, true, this);
        }

        private void RaisePropertyChanged(string propertyName, bool setHasChanged, object sender)
        {
            this.ValidatePropertyChange(propertyName);

            if (this.ignoreChanges || UINotifyPropertyChanges.ignoreAllChanges)
            {
                //Debug.WriteLine("Discarding Property Change for: {0}->{1}.{2}", sender == this ? "this" : sender.GetType().Name, this.GetType().Name, propertyName);
                return;
            }

            if (sender == this && this.IgnoreOwnPropertyChanges)
            {
                //Debug.WriteLine("Discarding Property Change for: {0}->{1}.{2}", sender == this ? "this" : sender.GetType().Name, this.GetType().Name, propertyName);
                return;
            }

            // Debug.WriteLine("Property change event: {0}->{1}.{2}", sender == this ? "this" : sender.GetType().Name, this.GetType().Name, propertyName);

            PropertyChangedEventHandler temp = this.PropertyChanged;

            if (temp != null)
            {
                try
                {
                    temp(this, new PropertyChangedEventArgs(propertyName));
                }
                catch (NullReferenceException ex)
                {
                    // TODO: Related to the ExpressionDark theme
                    if (ex.TargetSite.Name != "EvaluateOldNewStates")
                    {
                        throw;
                    }
                }
            }

            if (setHasChanged)
            {
                this.SetHasChanged(propertyName);
            }

            if (this.dependentProperties.ContainsKey(propertyName))
            {
                foreach (string dependentProperty in this.dependentProperties[propertyName])
                {
                    // Debug.WriteLine("Dependent property change event: {0}->{1}.{2}->{3}", sender == this ? "this" : sender.GetType().Name, this.GetType().Name, propertyName, dependentProperty);
                    this.RaisePropertyChanged(dependentProperty, setHasChanged, sender);
                }
            }
        }

        private string GetLocalError(string propertyName)
        {
            if (!this.ErrorList.ContainsKey(propertyName))
            {
                return null;
            }
            else
            {
                return this.ErrorList[propertyName];
            }
        }

        private string GetInheritedError(string propertyName)
        {
            foreach (UINotifyPropertyChanges errorProvider in this.inheritErrorsFrom)
            {
                if (!errorProvider.ErrorList.ContainsKey(propertyName))
                {
                    continue;
                }
                else
                {
                    return errorProvider.ErrorList[propertyName];
                }
            }

            return null;
        }

        private void Initialize()
        {
            this.IgnorePropertyHasChanged = new List<string>() { 
                "HasOwnChanges", 
                "HasAnyChanges", 
                "HasErrors",
                "Error",
                "HasInheritedChanges", 
                "SetHasChangedOnPropertyChange",
                "ThrowExceptionOnValidationError",
                "IgnorePropertyHasChanged",
                "IgnoreOwnChanges",
                "IgnoreOwnPropertyChanges"};

            this.SetHasChangedOnPropertyChange = false;
            this.ThrowExceptionOnValidationError = false;
            this.dependentProperties = new Dictionary<string, List<string>>();
            this.inheritErrorsFrom = new List<UINotifyPropertyChanges>();
            this.inheritHasChangedFrom = new List<UINotifyPropertyChanges>();
            this.ErrorList = new Dictionary<string, string>();
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.Initialize();
        }
    }
}
