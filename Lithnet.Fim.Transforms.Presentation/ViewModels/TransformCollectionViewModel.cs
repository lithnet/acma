using System;
using System.Linq;
using System.Windows;
using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System.Collections;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Fim.Transforms.Presentation
{
    public class TransformCollectionViewModel : ListViewModel<TransformViewModel, Transform>
    {
        private TransformKeyedCollection model;

        public TransformCollectionViewModel(TransformKeyedCollection model)
            : base()
        {
            this.model = model;
            this.SetCollectionViewModel((IList)this.model, t => this.ViewModelResolver(t));

            this.Commands.AddItem("AddTransform", t => this.AddTransform());

            this.IgnorePropertyHasChanged.Add("DisplayName");
            this.DisplayIcon = new BitmapImage(new Uri("pack://application:,,,/Lithnet.Fim.Transforms.Presentation;component/Resources/transforms.png", UriKind.Absolute)); ;
            this.OnModelRemoved += TransformCollectionViewModel_OnModelRemoved;
            
            foreach(var type in typeof(Transform).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Transform)) && !t.IsAbstract))
            {
                this.PasteableTypes.Add(type);
            }
        }

        private void TransformCollectionViewModel_OnModelRemoved(ListViewModelChangedEventArgs args)
        {
            Transform transform = args.Model as Transform;

            if (transform != null)
            {
                UniqueIDCache.RemoveItem(transform, Transform.CacheGroupName);
            }
        }

        public string DisplayName
        {
            get
            {
                return string.Format("Transforms");
            }
        }

        public void AddTransform()
        {
            NewItemWindow window = new NewItemWindow();
            NewTransformViewModel vm = new NewTransformViewModel(this, window);
            window.DataContext = vm;
            window.ShowDialog();
        }

        private TransformViewModel ViewModelResolver(Transform model)
        {
            TransformViewModel viewModel;

            Type t = GetViewModelType(model);
            viewModel = (TransformViewModel)Activator.CreateInstance(t, new object[] { model });
            return viewModel;
        }

        private static Type GetViewModelType(object model)
        {
            return GetViewModelType(model, model.GetType());
        }

        private static Type GetViewModelType(object model, Type modelType)
        {
            string modelTypeName = modelType.Name;

            Type viewModelType = Type.GetType("Lithnet.Fim.Transforms.Presentation." + modelTypeName + "ViewModel");

            if (viewModelType == null)
            {
                if (modelType.BaseType != typeof(object))
                {
                    return GetViewModelType(modelType, modelType.BaseType);
                }
                else
                {
                    throw new ArgumentException("A view model could not be found for the specified type " + model.GetType().Name);
                }
            }
            else
            {
                return Type.GetType("Lithnet.Fim.Transforms.Presentation." + modelTypeName + "ViewModel");
            }
        }
    }
}