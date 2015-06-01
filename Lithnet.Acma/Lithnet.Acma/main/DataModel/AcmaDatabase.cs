using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.Linq;
using Lithnet.Acma.DataModel;
using Microsoft.MetadirectoryServices;
using Lithnet.Fim.Core;
using System.ComponentModel;
using System.Transactions;
using System.Text.RegularExpressions;
using Lithnet.Logging;
using System.Data.Common;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Acma
{
    public class AcmaDatabase
    {
        public delegate void AttributeRenameEventDelegate(AcmaSchemaAttribute attribute, string oldName);

        public event AttributeRenameEventDelegate OnAttributeRenamed;

        public delegate void AttributeDeleteEventDelegate(string name);

        public event AttributeDeleteEventDelegate OnAttributeDeleted;

        public delegate void AttributeBindingEventDelegate(string objectClassName, string attributeName);

        public event AttributeBindingEventDelegate OnBindingDeleted;

        public delegate void ObjectClassDeleteEventDelegate(string objectClassName);

        public event ObjectClassDeleteEventDelegate OnObjectClassDeleted;

        public delegate void ObjectClassRenameEventDeledate(AcmaSchemaObjectClass objectClass, string oldName);

        public event ObjectClassRenameEventDeledate OnObjectClassRenamed;

        public const string ObjectTableName = "MA_Objects";

        private MADataContext objectDataContext;

        [ThreadStatic]
        private DBSchemaDataContext dc;

        private DBSchemaDataContext DataContext
        {
            get
            {
                if (this.dc == null)
                {
                   // this.dc = new DBSchemaDataContext(this.connString);
                    throw new NotConnectedException("The connection to the database has not yet been established. Call ConnectToDatabase before using methods in this class");
                }

                return this.dc;
            }
        }

        private List<AcmaSchemaAttribute> attributes;

        private List<AcmaSchemaObjectClass> objectClasses;

        private List<AcmaSchemaMapping> mappings;

        private List<AcmaSchemaShadowObjectLink> shadowObjectLinks;

        private List<AcmaSequence> sequences;

        private List<AcmaSchemaReferenceLink> referenceLinks;

        private List<SafetyRule> safetyRules;

        private List<AcmaConstant> constants;

        public IBindingList AttributesBindingList { get; private set; }

        public IBindingList ObjectClassesBindingList { get; private set; }

        public IBindingList MappingsBindingList { get; private set; }

        public IBindingList ShadowObjectLinksBindingList { get; private set; }

        public IBindingList SequencesBindingList { get; private set; }

        public IBindingList ReferenceLinksBindingList { get; private set; }

        public IBindingList SafetyRulesBindingList { get; private set; }

        public IBindingList ConstantsBindingList { get; private set; }

        public ProgressInformation ProgressInfo { get; private set; }

        public IEnumerable<AcmaSchemaAttribute> Attributes
        {
            get
            {
                if (this.CanCache)
                {
                    if (this.attributes == null)
                    {
                        this.attributes = this.DataContext.AcmaSchemaAttributes.OrderBy(t => t.Name).ToList();
                    }

                    return this.attributes;
                }
                else
                {
                    return this.DataContext.AcmaSchemaAttributes.OrderBy(t => t.Name);
                }
            }
        }

        public IEnumerable<AcmaSchemaObjectClass> ObjectClasses
        {
            get
            {
                if (this.CanCache)
                {
                    if (this.objectClasses == null)
                    {
                        this.objectClasses = this.DataContext.AcmaSchemaObjectClasses.ToList();
                    }

                    return this.objectClasses;
                }
                else
                {
                    return this.DataContext.AcmaSchemaObjectClasses;
                }
            }
        }

        public IEnumerable<AcmaSchemaMapping> Mappings
        {
            get
            {
                if (this.CanCache)
                {
                    if (this.mappings == null)
                    {
                        this.mappings = this.DataContext.AcmaSchemaMappings.ToList();
                    }

                    return this.mappings;
                }
                else
                {
                    return this.DataContext.AcmaSchemaMappings;
                }
            }
        }

        public IEnumerable<AcmaSchemaShadowObjectLink> ShadowObjectLinks
        {
            get
            {
                if (this.CanCache)
                {
                    if (this.shadowObjectLinks == null)
                    {
                        this.shadowObjectLinks = this.DataContext.AcmaSchemaShadowObjectLinks.ToList();
                    }

                    return this.shadowObjectLinks;
                }
                else
                {
                    return this.DataContext.AcmaSchemaShadowObjectLinks;
                }
            }
        }

        public IEnumerable<AcmaSequence> Sequences
        {
            get
            {
                if (this.CanCache)
                {
                    if (this.sequences == null)
                    {
                        this.sequences = this.DataContext.AcmaSequences.ToList();
                    }

                    return this.sequences;
                }
                else
                {
                    return this.DataContext.AcmaSequences;
                }
            }
        }

        public IEnumerable<AcmaSchemaReferenceLink> ReferenceLinks
        {
            get
            {
                if (this.CanCache)
                {
                    if (this.referenceLinks == null)
                    {
                        this.referenceLinks = this.DataContext.AcmaSchemaReferenceLinks.ToList();
                    }

                    return this.referenceLinks;
                }
                else
                {
                    return this.DataContext.AcmaSchemaReferenceLinks;
                }
            }
        }

        public IEnumerable<SafetyRule> SafetyRules
        {
            get
            {
                if (this.CanCache)
                {
                    if (this.safetyRules == null)
                    {
                        this.safetyRules = this.DataContext.SafetyRules.ToList();
                    }

                    return this.safetyRules;
                }
                else
                {
                    return this.DataContext.SafetyRules;
                }
            }
        }

        public IEnumerable<AcmaConstant> Constants
        {
            get
            {
                if (this.CanCache)
                {
                    if (this.constants == null)
                    {
                        this.constants = this.DataContext.AcmaConstants.ToList();
                    }

                    return this.constants;
                }
                else
                {
                    return this.DataContext.AcmaConstants;
                }
            }
        }

        public string ServerName
        {
            get
            {
                if (this.DataContext != null)
                {
                    return this.DataContext.Connection.DataSource + "\\" + this.DataContext.Connection.Database;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public MADataContext MADataConext
        {
            get
            {
                if (this.objectDataContext == null)
                {
                    this.objectDataContext = new MADataContext(this.DataContext.Connection.ConnectionString);
                }

                return this.objectDataContext;
            }
        }

        public bool IsConnected
        {
            get
            {
                return this.dc != null;
            }
        }

        public bool CanCache { get; set; }

        public AcmaDatabase(string connectionString)
        {
            this.ProgressInfo = new ProgressInformation();
            this.CanCache = true;
            this.ConnectToDatabase(connectionString);
        }

        public AcmaDatabase(string serverName, string database)
        {
            this.ProgressInfo = new ProgressInformation();
            this.CanCache = true;
            this.ConnectToDatabase(serverName, database);
        }

        public void ClearCache()
        {
            this.attributes = null;
            this.objectClasses = null;
            this.mappings = null;
            this.shadowObjectLinks = null;
            this.sequences = null;
            this.referenceLinks = null;
            this.safetyRules = null;
            this.constants = null;
        }

        private string connString;

        public void ConnectToDatabase(string server, string database)
        {
            this.ConnectToDatabase(string.Format("Data Source={0};Initial Catalog={1};Integrated Security=True", server, database));
        }

        public void ConnectToDatabase(string connectionString)
        {
            this.ThrowOnNullArgument("connectionString", connectionString);

            this.connString = connectionString;
            this.dc = new DBSchemaDataContext(connectionString);
            this.ValidateVersion();

            this.ObjectClassesBindingList = this.DataContext.AcmaSchemaObjectClasses.GetNewBindingList();
            this.AttributesBindingList = this.DataContext.AcmaSchemaAttributes.GetNewBindingList();
            this.MappingsBindingList = this.DataContext.AcmaSchemaMappings.GetNewBindingList();
            this.ShadowObjectLinksBindingList = this.DataContext.AcmaSchemaShadowObjectLinks.GetNewBindingList();
            this.SequencesBindingList = this.DataContext.AcmaSequences.GetNewBindingList();
            this.ReferenceLinksBindingList = this.DataContext.AcmaSchemaReferenceLinks.GetNewBindingList();
            this.SafetyRulesBindingList = this.DataContext.SafetyRules.GetNewBindingList();
            this.ConstantsBindingList = this.DataContext.AcmaConstants.GetNewBindingList();
        }

        private void ValidateVersion()
        {
            try
            {
                AcmaDBVersion version = this.dc.AcmaDBVersions.OrderByDescending(t => t.MajorReleaseNumber).ThenByDescending(t => t.MinorReleaseNumber).ThenByDescending(t => t.PointReleaseNumber).FirstOrDefault();

                if (version == null)
                {
                    throw new DBVersionException(strings.DBUpdateRequired);
                }

                DBInstallUpgrader.ValidateVersion(version.MajorReleaseNumber, version.MinorReleaseNumber, version.PointReleaseNumber);
            }
            catch (DBVersionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.WriteException(ex);
                throw new DBVersionException("An unexpected error occurred while reading the database version. The database may need to be upgraded to work with this version of the application", ex);
            }
        }

        public void RefreshEntity(object entity)
        {
            this.DataContext.Refresh(RefreshMode.OverwriteCurrentValues, entity);
        }

        public void Commit()
        {
            this.DataContext.SubmitChanges();
        }

        public bool HasConstant(string name)
        {
            return this.Constants.Any(t => t.Name == name);
        }

        public string GetConstantValue(string name)
        {
            return this.GetConstant(name).Value;
        }

        public void SetConstant(string name, string value)
        {
            this.GetConstant(name).Value = value;
            this.DataContext.SubmitChanges();
        }

        public void DeleteConstant(string name)
        {
            AcmaConstant constant = this.GetConstant(name);
            this.ConstantsBindingList.Remove(constant);
            this.DataContext.AcmaConstants.DeleteOnSubmit(constant);
            this.DataContext.SubmitChanges();
        }

        public AcmaConstant AddConstant(string name, string value)
        {
            AcmaConstant constant = new AcmaConstant();
            constant.Name = name;
            constant.Value = value;

            this.ConstantsBindingList.Add(constant);
            this.DataContext.SubmitChanges();

            return constant;
        }

        public AcmaConstant GetConstant(string name)
        {
            AcmaConstant constant = this.Constants.FirstOrDefault(t => t.Name == name);

            if (constant == null)
            {
                throw new NotFoundException("The specified constant does not exist");
            }

            return constant;
        }

        public SafetyRule CreateSafetyRule(AcmaSchemaMapping mapping, string name, string pattern, bool nullAllowed)
        {
            return this.CreateSafetyRule(mapping, name, pattern, nullAllowed, SafetyRulesBindingList);
        }

        public SafetyRule CreateSafetyRule(AcmaSchemaMapping mapping, string name, string pattern, bool nullAllowed, IBindingList bindingList)
        {
            this.ThrowOnNullArgument("mapping", mapping);
            this.ThrowOnNullArgument("name", name);
            this.ThrowOnNullArgument("pattern", pattern);
            this.ThrowOnNullArgument("bindingList", bindingList);

            this.ValidateCanCreateSafetyRule(name, pattern);

            SafetyRule rule = (SafetyRule)bindingList.AddNew();
            rule.Name = name;
            rule.AcmaSchemaMapping = mapping;
            rule.Pattern = pattern;
            rule.NullAllowed = nullAllowed;

            this.DataContext.SubmitChanges();

            return rule;
        }

        private void ValidateCanCreateSafetyRule(string name, string pattern)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidOperationException("The safety rule name cannot be null");
            }

            if (this.HasSafetyRule(name))
            {
                throw new InvalidOperationException("The safety rule name is already in use");
            }

            try
            {
                Regex regex = new Regex(pattern);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("The regular expression syntax is invalid", ex);
            }
        }

        public bool HasSafetyRule(string name)
        {
            return this.SafetyRules.Any(t => t.Name == name);
        }

        public void DeleteSafetyRule(SafetyRule rule)
        {
            this.ThrowOnNullArgument("rule", rule);

            rule.AcmaSchemaMapping.SafetyRuleBindingList.Remove(rule);
            this.SafetyRulesBindingList.Remove(rule);
            this.DataContext.SubmitChanges();
        }

        public AcmaSchemaReferenceLink GetReferenceLink(AcmaSchemaObjectClass forwardLinkObjectClass, AcmaSchemaAttribute forwardLink, AcmaSchemaObjectClass backLinkObjectClass, AcmaSchemaAttribute backLink)
        {
            AcmaSchemaReferenceLink link = this.ReferenceLinks.FirstOrDefault(t => t.ForwardLinkObjectClass == forwardLinkObjectClass &&
                t.ForwardLinkAttribute == forwardLink &&
                t.BackLinkObjectClass == backLinkObjectClass &&
                t.BackLinkAttribute == backLink);

            if (link == null)
            {
                throw new NotFoundException("The reference back link was not found");
            }

            return link;
        }

        public AcmaSchemaReferenceLink CreateReferenceLink(AcmaSchemaObjectClass forwardLinkObjectClass, AcmaSchemaAttribute forwardLink, AcmaSchemaObjectClass backLinkObjectClass, AcmaSchemaAttribute backLink)
        {
            return this.CreateReferenceLink(forwardLinkObjectClass, forwardLink, backLinkObjectClass, backLink, this.ReferenceLinksBindingList);
        }

        public AcmaSchemaReferenceLink CreateReferenceLink(AcmaSchemaObjectClass forwardLinkObjectClass, AcmaSchemaAttribute forwardLink, AcmaSchemaObjectClass backLinkObjectClass, AcmaSchemaAttribute backLink, IBindingList bindingList)
        {
            this.ThrowOnNullArgument("forwardLinkobjectClass", forwardLinkObjectClass);
            this.ThrowOnNullArgument("forwardLink", forwardLink);
            this.ThrowOnNullArgument("backLinkObjectClass", backLinkObjectClass);
            this.ThrowOnNullArgument("backLink", backLink);
            this.ThrowOnNullArgument("bindingList", bindingList);

            this.ValidateCanCreateReferenceLink(forwardLinkObjectClass, forwardLink, backLinkObjectClass, backLink);

            AcmaSchemaReferenceLink link = (AcmaSchemaReferenceLink)bindingList.AddNew();
            link.ForwardLinkObjectClass = forwardLinkObjectClass;
            link.ForwardLinkAttribute = forwardLink;
            link.BackLinkAttribute = backLink;
            link.BackLinkObjectClass = backLinkObjectClass;
            this.DataContext.SubmitChanges();

            return link;
        }

        private void ValidateCanCreateReferenceLink(AcmaSchemaObjectClass forwardLinkObjectClass, AcmaSchemaAttribute forwardLink, AcmaSchemaObjectClass backLinkObjectClass, AcmaSchemaAttribute backLink)
        {
            if (!forwardLinkObjectClass.Attributes.Any(t => t == forwardLink))
            {
                throw new InvalidOperationException("The specified forward link attribute does not exist on the object class");
            }

            if (!backLinkObjectClass.Attributes.Any(t => t == backLink))
            {
                throw new InvalidOperationException("The specified back link attribute does not exist on the object class");
            }

            if (backLinkObjectClass.BackLinks.Any(t => t.BackLinkAttribute == backLink))
            {
                throw new InvalidOperationException("The specified attribute is already used in a back link on this object class");
            }

            if (backLink == forwardLink && forwardLinkObjectClass == backLinkObjectClass)
            {
                throw new InvalidOperationException("The forward and back link attributes cannot be the same if they belong to the same object class");
            }

            if (backLink.Type != ExtendedAttributeType.Reference)
            {
                throw new InvalidOperationException("The back link attribute must of of a 'reference' attribute type");
            }

            if (forwardLink.Type != ExtendedAttributeType.Reference)
            {
                throw new InvalidOperationException("The forward link attribute must of of a 'reference' attribute type");
            }

            if (forwardLink.Operation == AcmaAttributeOperation.AcmaInternalTemp)
            {
                throw new InvalidOperationException("The forward link attribute cannot be a temporary attribute");
            }

            if (backLink.Operation == AcmaAttributeOperation.AcmaInternalTemp)
            {
                throw new InvalidOperationException("The back link attribute cannot be a temporary attribute");
            }

            if (backLink.IsBuiltIn)
            {
                throw new InvalidOperationException("The back link attribute cannot be a built-in attribute");
            }

            if (backLink.IsInheritedInClass(backLinkObjectClass))
            {
                throw new InvalidOperationException("The back link attribute cannot be an inherited attribute");
            }
        }

        public void DeleteReferenceLink(AcmaSchemaReferenceLink link)
        {
            this.DeleteReferenceLink(link, this.ReferenceLinksBindingList);
        }

        public void DeleteReferenceLink(AcmaSchemaReferenceLink link, IBindingList bindingList)
        {
            this.ThrowOnNullArgument("link", link);
            this.ThrowOnNullArgument("bindingList", bindingList);

            // We need to remove from the foreign key mapping, as well as the base table.
            bindingList.Remove(link);
            this.DataContext.AcmaSchemaReferenceLinks.DeleteOnSubmit(link);

            this.ReferenceLinksBindingList.Remove(link);
            this.DataContext.SubmitChanges();
        }

        public bool HasSequence(string name)
        {
            return this.Sequences.Any(t => t.Name == name);
        }

        public AcmaSequence GetSequence(string name)
        {
            return this.Sequences.FirstOrDefault(t => t.Name == name);
        }

        public long GetNextSequenceValue(string name)
        {
            return this.DataContext.spSequenceGetNextValue(name);
        }

        public AcmaSequence CreateSequence(string name, long startValue, long incrementBy, long? minValue, long? maxValue, bool cycle)
        {
            this.ValidateCanCreateSequence(name, startValue, incrementBy, minValue, maxValue, cycle);

            AcmaSequence sequence = new AcmaSequence();

            sequence.Name = name;
            sequence.StartValue = startValue;
            sequence.Increment = incrementBy;
            sequence.MinValue = minValue;
            sequence.MaxValue = maxValue;
            sequence.IsCycleEnabled = cycle;

            this.SequencesBindingList.Add(sequence);
            this.DataContext.SubmitChanges();

            return sequence;
        }

        private void ValidateCanCreateSequence(string name, long startValue, long incrementBy, long? minValue, long? maxValue, bool cycle)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidOperationException("A name must be provided");
            }

            if (this.Sequences.Any(t => t.Name == name))
            {
                throw new InvalidOperationException("The specified sequence name is already in use");
            }

            if (minValue.HasValue && maxValue.HasValue)
            {
                if (minValue.Value >= maxValue.Value)
                {
                    throw new InvalidOperationException("The maximum value must be greater than the minimum value");
                }
            }

            if (minValue.HasValue ^ maxValue.HasValue)
            {
                throw new InvalidOperationException("A minimum and maximum value are required if either is specified");
            }
        }

        public void UpdateSafetyRule(SafetyRule safetyRule)
        {
            this.DataContext.SubmitChanges();
        }

        public void UpdateSequence(AcmaSequence sequence)
        {
            this.DataContext.SubmitChanges();
        }

        public void DeleteSequence(string name)
        {
            AcmaSequence sequence = this.GetSequence(name);
            this.DeleteSequence(sequence);
        }

        public void DeleteSequence(AcmaSequence sequence)
        {
            this.SequencesBindingList.Remove(sequence);
            this.DataContext.SubmitChanges();
        }

        public bool HasObjectClass(string name)
        {
            this.ThrowOnNullArgument("name", name);

            return this.ObjectClasses.Any(t => t.Name == name);
        }

        public AcmaSchemaObjectClass GetObjectClass(string name)
        {
            this.ThrowOnNullArgument("name", name);
            AcmaSchemaObjectClass objectClass = this.ObjectClasses.FirstOrDefault(t => t.Name == name);

            if (objectClass == null)
            {
                throw new NoSuchClassException(name);
            }

            return objectClass;
        }

        public void CreateObjectClass(string name)
        {
            this.ThrowOnNullArgument("name", name);
            this.CreateObjectClass(name, false, null);
        }

        public AcmaSchemaObjectClass CreateObjectClass(string name, bool canResurrect)
        {
            this.ThrowOnNullArgument("name", name);
            return this.CreateObjectClass(name, canResurrect, null);
        }

        public AcmaSchemaObjectClass CreateObjectClass(string name, AcmaSchemaObjectClass shadowFromClass)
        {
            this.ThrowOnNullArgument("name", name);
            return this.CreateObjectClass(name, false, shadowFromClass);
        }

        public AcmaSchemaObjectClass CreateObjectClass(string name, bool canResurrect, AcmaSchemaObjectClass shadowFromClass)
        {
            this.ThrowOnNullArgument("name", name);
            this.ValidateNewObjectClassParameters(name, shadowFromClass);
            AcmaSchemaObjectClass objectClass;

            using (var trans = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }, EnterpriseServicesInteropOption.Automatic))
            {
                objectClass = new AcmaSchemaObjectClass();
                objectClass.Name = name;
                objectClass.ShadowFromObjectClass = shadowFromClass;
                objectClass.AllowResurrection = canResurrect;

                this.ObjectClassesBindingList.Add(objectClass);

                this.CreateBuiltInMappings(objectClass);

                if (shadowFromClass != null)
                {
                    this.CreateBuiltInMappingsForShadowObject(objectClass);
                }

                this.DataContext.SubmitChanges();

                trans.Complete();
            }

            return objectClass;
        }

        public void RenameObjectClass(string currentName, string newName)
        {
            this.ThrowOnNullArgument("currentName", currentName);
            this.ThrowOnNullArgument("newName", newName);
            AcmaSchemaObjectClass objectClass = this.GetObjectClass(currentName);

            this.DataContext.spSchemaRenameObjectClass(currentName, newName);

            this.RaiseObjectClassRenamedEvent(objectClass, currentName);
        }

        public void DeleteObjectClass(AcmaSchemaObjectClass objectClass)
        {
            this.ThrowOnNullArgument("objectClass", objectClass);

            this.ValidateCanDeleteObjectClass(objectClass);

            string name = objectClass.Name;

            this.DeleteObjects(objectClass);

            this.ObjectClassesBindingList.Remove(objectClass);
            this.DataContext.AcmaSchemaObjectClasses.DeleteOnSubmit(objectClass);

            this.DataContext.SubmitChanges();

            this.RaiseObjectClassDeletedEvent(name);
        }

        private void DeleteObjects(AcmaSchemaObjectClass objectClass)
        {
            DBQueryByValue query1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectClass"), Fim.Core.ValueOperator.Equals, new ValueDeclaration(objectClass.Name));

            this.ProgressInfo.OperationDescription = string.Format("Deleting {0} objects", objectClass.Name);
            this.ProgressInfo.MinValue = 0;
            this.ProgressInfo.MaxValue = 100;
            this.ProgressInfo.CurrentValue = 0;
            this.ProgressInfo.ProgressDescription = "Searching for objects";
            this.ProgressInfo.CanCancel = true;
            this.ProgressInfo.Canceled = false;

            try
            {
                using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionManager.MaximumTimeout))
                {
                    ActiveConfig.DB.MADataConext.SqlConnection.EnlistTransaction(Transaction.Current);

                    List<MAObjectHologram> holograms = ActiveConfig.DB.MADataConext.GetMAObjectsFromDBQuery(query1).ToList();
                    this.ProgressInfo.MaxValue = holograms.Count;

                    foreach (MAObjectHologram hologram in holograms)
                    {
                        this.ProgressInfo.ProgressDescription = "Deleting " + hologram.DisplayText;

                        if (this.ProgressInfo.Canceled)
                        {
                            throw new OperationCanceledException();
                        }

                        hologram.Delete(true);
                        this.ProgressInfo.CurrentValue++;
                    }

                    transaction.Complete();
                }
            }
            catch
            {
                ActiveConfig.DB.MADataConext.ResetConnection();
                throw;
            }
        }


        public bool HasAttribute(string name)
        {
            this.ThrowOnNullArgument("name", name);
            return this.Attributes.Any(t => t.Name == name);
        }

        public AcmaSchemaAttribute GetAttribute(string name)
        {
            this.ThrowOnNullArgument("name", name);
            AcmaSchemaAttribute attribute = this.Attributes.FirstOrDefault(t => t.Name == name);

            if (attribute == null)
            {
                throw new NoSuchAttributeException(name);
            }
            else
            {
                return attribute;
            }
        }

        public AcmaSchemaAttribute GetAttribute(string name, AcmaSchemaObjectClass objectClass)
        {
            this.ThrowOnNullArgument("name", name);
            this.ThrowOnNullArgument("objectClass", objectClass);

            AcmaSchemaAttribute attribute = this.Attributes.FirstOrDefault(t => t.Name == name);

            if (attribute == null)
            {
                throw new NoSuchAttributeException(name);
            }
            else
            {
                if (!attribute.Mappings.Any(t => t.ObjectClass == objectClass))
                {
                    throw new NoSuchAttributeInObjectTypeException(name);
                }
                else
                {
                    return attribute;
                }
            }
        }

        public AcmaSchemaAttribute GetAttribute(string name, string objectClassName)
        {
            this.ThrowOnNullArgument("name", name);
            this.ThrowOnNullArgument("objectClass", objectClassName);
            AcmaSchemaObjectClass objectClass = this.ObjectClasses.FirstOrDefault(t => t.Name == objectClassName);

            if (objectClass == null)
            {
                throw new NoSuchObjectTypeException(objectClassName);
            }

            return this.GetAttribute(name, objectClass);
        }

        public AcmaSchemaAttribute CreateAttribute(string name, ExtendedAttributeType type, bool isMultiValued, AcmaAttributeOperation operation, bool indexable, bool indexed)
        {
            this.ThrowOnNullArgument("name", name);
            return this.CreateAttribute(name, type, isMultiValued, operation, indexable, indexed, false);
        }

        internal AcmaSchemaAttribute CreateAttribute(string name, ExtendedAttributeType type, bool isMultiValued, AcmaAttributeOperation operation, bool isIndexable, bool isIndexed, bool isBuiltIn)
        {
            this.ThrowOnNullArgument("name", name);

            this.ValidateAttributeParameters(name, type, isMultiValued, operation, isIndexable, isIndexed);

            if (type == ExtendedAttributeType.Reference)
            {
                isIndexable = true;
                isIndexed = true;
            }

            if (type == ExtendedAttributeType.Integer)
            {
                isIndexable = true;
            }

            AcmaSchemaAttribute newAttribute;

            using (var trans = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, EnterpriseServicesInteropOption.Automatic))
            {
                newAttribute = new AcmaSchemaAttribute();
                newAttribute.Name = name;
                newAttribute.Type = type;
                newAttribute.Operation = operation;
                newAttribute.IsIndexable = isIndexable;
                newAttribute.IsIndexed = isIndexed;
                newAttribute.IsMultivalued = isMultiValued;
                this.AttributesBindingList.Add(newAttribute);
                this.DataContext.SubmitChanges();

                this.DataContext.spSchemaSetupNewAttribute(newAttribute.ID);

                this.DataContext.Refresh(RefreshMode.OverwriteCurrentValues, newAttribute);
                trans.Complete();
            }

            return newAttribute;
        }


        public void RenameAttribute(string currentName, string newName)
        {
            this.ThrowOnNullArgument("currentName", currentName);
            this.ThrowOnNullArgument("newName", newName);

            AcmaSchemaAttribute attribute = this.GetAttribute(currentName);

            this.DataContext.spSchemaRenameAttribute(currentName, newName);
            this.DataContext.Refresh(RefreshMode.OverwriteCurrentValues, attribute);

            this.RaiseAttributeRenamedEvent(attribute, currentName);
        }

        public void DeleteAttribute(AcmaSchemaAttribute attribute)
        {
            this.ThrowOnNullArgument("attribute", attribute);
            this.DeleteAttribute(attribute, this.AttributesBindingList);
        }

        public void DeleteAttribute(AcmaSchemaAttribute attribute, IBindingList bindingList)
        {
            string name = attribute.Name;
            this.ThrowOnNullArgument("attribute", attribute);
            this.ThrowOnNullArgument("bindingList", bindingList);

            this.ValidateCanDeleteAttribute(attribute);

            bindingList.Remove(attribute);
            this.DataContext.AcmaSchemaAttributes.DeleteOnSubmit(attribute);

            this.DataContext.SubmitChanges();

            this.RaiseAttributeDeletedEvent(name);
        }


        public AcmaSchemaShadowObjectLink GetShadowLink(int id)
        {
            AcmaSchemaShadowObjectLink link = this.ShadowObjectLinks.FirstOrDefault(t => t.ID == id);

            if (link == null)
            {
                throw new NotFoundException(string.Format("The shadow link with ID {0} was not found", id));
            }

            return link;
        }

        public AcmaSchemaShadowObjectLink GetShadowLink(string name)
        {
            AcmaSchemaShadowObjectLink link = this.ShadowObjectLinks.FirstOrDefault(t => t.Name == name);

            if (link == null)
            {
                throw new NotFoundException(string.Format("The shadow link with name '{0}' was not found", name));
            }

            return link;
        }

        public AcmaSchemaShadowObjectLink CreateShadowLink(AcmaSchemaObjectClass shadowClass, AcmaSchemaAttribute provisioningAttribute, AcmaSchemaAttribute referenceAtttribute, string name)
        {
            return this.CreateShadowLink(shadowClass, provisioningAttribute, referenceAtttribute, name, ShadowObjectLinksBindingList);
        }

        public AcmaSchemaShadowObjectLink CreateShadowLink(AcmaSchemaObjectClass shadowClass, AcmaSchemaAttribute provisioningAttribute, AcmaSchemaAttribute referenceAttribute, string name, IBindingList bindingList)
        {
            this.ThrowOnNullArgument("shadowClass", shadowClass);
            this.ThrowOnNullArgument("provisioningAttribute", provisioningAttribute);
            this.ThrowOnNullArgument("referenceAtttribute", referenceAttribute);
            this.ThrowOnNullArgument("bindingList", bindingList);
            this.ThrowOnNullArgument("name", bindingList);

            this.ValidateCanCreateShadowLink(shadowClass, provisioningAttribute, referenceAttribute, name);

            AcmaSchemaShadowObjectLink link = (AcmaSchemaShadowObjectLink)bindingList.AddNew();
            link.ShadowObjectClass = shadowClass;
            link.ParentObjectClass = shadowClass.ShadowFromObjectClass;
            link.ReferenceAttribute = referenceAttribute;
            link.ProvisioningAttribute = provisioningAttribute;
            link.Name = name;
            this.DataContext.SubmitChanges();

            return link;
        }

        public void DeleteShadowLink(AcmaSchemaShadowObjectLink link)
        {
            this.DeleteShadowLink(link, ShadowObjectLinksBindingList);
        }

        public void DeleteShadowLink(AcmaSchemaShadowObjectLink link, IBindingList bindingList)
        {
            this.ThrowOnNullArgument("link", link);
            this.ThrowOnNullArgument("bindingList", bindingList);

            this.ValidateCanDeleteShadowLink(link);

            this.DeleteShadowObjectsByLink(link);

            // We need to remove from the foreign key mapping, as well as the base table.
            bindingList.Remove(link);
            this.ShadowObjectLinksBindingList.Remove(link);
            this.DataContext.AcmaSchemaShadowObjectLinks.DeleteOnSubmit(link);

            this.DataContext.SubmitChanges();
        }

        private void DeleteShadowObjectsByLink(AcmaSchemaShadowObjectLink link)
        {
            DBQueryByValue query1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectClass"), Fim.Core.ValueOperator.Equals, new ValueDeclaration(link.ShadowObjectClass.Name));
            DBQueryByValue query2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("shadowLink"), Fim.Core.ValueOperator.Equals, new ValueDeclaration(link.Name));
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(query1, query2);

            this.ProgressInfo.OperationDescription = string.Format("Deleting shadow objects from link {0}", link.Name);
            this.ProgressInfo.MinValue = 0;
            this.ProgressInfo.MaxValue = 100;
            this.ProgressInfo.CurrentValue = 0;
            this.ProgressInfo.ProgressDescription = "Searching for objects";
            this.ProgressInfo.CanCancel = true;
            this.ProgressInfo.Canceled = false;

            try
            {
                using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionManager.MaximumTimeout))
                {
                    ActiveConfig.DB.MADataConext.SqlConnection.EnlistTransaction(Transaction.Current);

                    List<MAObjectHologram> holograms = ActiveConfig.DB.MADataConext.GetMAObjectsFromDBQuery(group).ToList();
                    this.ProgressInfo.MaxValue = holograms.Count;

                    foreach (MAObjectHologram hologram in holograms)
                    {
                        this.ProgressInfo.ProgressDescription = "Deleting " + hologram.DisplayText;

                        if (this.ProgressInfo.Canceled)
                        {
                            throw new OperationCanceledException();
                        }

                        hologram.Delete(true);
                        this.ProgressInfo.CurrentValue++;
                    }

                    transaction.Complete();
                }
            }
            catch
            {
                ActiveConfig.DB.MADataConext.ResetConnection();
                throw;
            }
        }

        public AcmaSchemaMapping GetMapping(string attributeName, string objectClass)
        {
            this.ThrowOnNullArgument("attributeName", attributeName);
            this.ThrowOnNullArgument("objectClass", objectClass);

            AcmaSchemaObjectClass schemaObject = this.ObjectClasses.FirstOrDefault(t => t.Name == objectClass);
            if (schemaObject == null)
            {
                throw new NoSuchObjectTypeException(objectClass);
            }

            AcmaSchemaMapping mapping = schemaObject.Mappings.FirstOrDefault(t => t.Attribute.Name == attributeName);
            if (mapping == null)
            {
                throw new NoSuchAttributeInObjectTypeException(attributeName);
            }

            return mapping;
        }

        public AcmaSchemaMapping CreateMapping(AcmaSchemaObjectClass objectClass, AcmaSchemaAttribute attribute)
        {
            this.ThrowOnNullArgument("objectClass", objectClass);
            this.ThrowOnNullArgument("attributeName", attribute);

            return this.CreateMapping(objectClass, attribute, null, null, null, objectClass.MappingsBindingList, false);
        }

        public AcmaSchemaMapping CreateMapping(AcmaSchemaObjectClass objectClass, AcmaSchemaAttribute attribute, AcmaSchemaAttribute inheritanceSourceAttribute, AcmaSchemaObjectClass inheritanceSourceObjectClass, AcmaSchemaAttribute inheritedAttribute)
        {
            this.ThrowOnNullArgument("objectClass", objectClass);
            this.ThrowOnNullArgument("attribute", attribute);
            this.ThrowOnNullArgument("inheritanceSourceAttribute", inheritanceSourceAttribute);
            this.ThrowOnNullArgument("inheritedAttribute", inheritedAttribute);
            this.ThrowOnNullArgument("inheritanceSourceObjectClass", inheritanceSourceObjectClass);

            return CreateMapping(objectClass, attribute, inheritanceSourceAttribute, inheritanceSourceObjectClass, inheritedAttribute, objectClass.MappingsBindingList, false);
        }

        public AcmaSchemaMapping CreateMapping(AcmaSchemaObjectClass objectClass, AcmaSchemaAttribute attribute, IBindingList mappingsBindingList)
        {
            this.ThrowOnNullArgument("objectClass", objectClass);
            this.ThrowOnNullArgument("attributeName", attribute);

            return this.CreateMapping(objectClass, attribute, null, null, null, mappingsBindingList, false);
        }

        public AcmaSchemaMapping CreateMapping(AcmaSchemaObjectClass objectClass, AcmaSchemaAttribute attribute, AcmaSchemaAttribute inheritanceSourceAttribute, AcmaSchemaObjectClass inheritanceSourceObjectClass, AcmaSchemaAttribute inheritedAttribute, IBindingList mappingsBindingList)
        {
            this.ThrowOnNullArgument("objectClass", objectClass);
            this.ThrowOnNullArgument("attribute", attribute);
            this.ThrowOnNullArgument("inheritanceSourceAttribute", inheritanceSourceAttribute);
            this.ThrowOnNullArgument("inheritedAttribute", inheritedAttribute);
            this.ThrowOnNullArgument("inheritanceSourceObjectClass", inheritanceSourceObjectClass);

            return this.CreateMapping(objectClass, attribute, inheritanceSourceAttribute, inheritanceSourceObjectClass, inheritedAttribute, mappingsBindingList, false);
        }

        internal AcmaSchemaMapping CreateMapping(AcmaSchemaObjectClass objectClass, AcmaSchemaAttribute attribute, AcmaSchemaAttribute inheritanceSourceAttribute, AcmaSchemaObjectClass inheritanceSourceObjectClass, AcmaSchemaAttribute inheritedAttribute, IBindingList mappingsBindingList, bool isBuiltIn)
        {
            this.ThrowOnNullArgument("objectClass", objectClass);
            this.ThrowOnNullArgument("attribute", attribute);

            if (inheritedAttribute != null && inheritanceSourceAttribute == null)
            {
                throw new ArgumentNullException("inheritanceSourceAttribute");
            }

            if (inheritanceSourceAttribute != null && inheritedAttribute == null)
            {
                throw new ArgumentNullException("inheritedAttribute");
            }

            if (inheritanceSourceAttribute != null && inheritanceSourceObjectClass == null)
            {
                throw new ArgumentNullException("inheritanceSourceObjectClass");
            }

            this.ValidateCanCreateMapping(objectClass, attribute, inheritanceSourceAttribute, inheritanceSourceObjectClass, inheritedAttribute, isBuiltIn);

            AcmaSchemaMapping mapping = (AcmaSchemaMapping)mappingsBindingList.AddNew();
            mapping.Attribute = attribute;
            mapping.ObjectClass = objectClass;
            mapping.InheritanceSourceAttribute = inheritanceSourceAttribute;
            mapping.InheritanceSourceObjectClass = inheritanceSourceObjectClass;
            mapping.InheritedAttribute = inheritedAttribute;
            mapping.IsBuiltIn = isBuiltIn;
            
            //this.DataContext.AcmaSchemaMappings.InsertOnSubmit(mapping);
            this.DataContext.SubmitChanges();
            
            
            //mappingsBindingList.Add(mapping);

            return mapping;
        }

        public void DeleteMapping(AcmaSchemaMapping mapping)
        {
            this.ThrowOnNullArgument("mapping", mapping);

            this.DeleteMapping(mapping, this.MappingsBindingList);
        }

        public void DeleteMapping(AcmaSchemaMapping mapping, IBindingList bindingList)
        {
            this.ThrowOnNullArgument("mapping", mapping);
            this.ThrowOnNullArgument("mappingsBindingList", bindingList);

            this.ValidateCanDeleteMapping(mapping);

            this.DeleteMappingFromObjects(mapping);

            string attributeName = mapping.Attribute.Name;
            string objectClassName = mapping.ObjectClass.Name;

            bindingList.Remove(mapping);
            this.DataContext.AcmaSchemaMappings.DeleteOnSubmit(mapping);
            //mapping.ObjectClass = null;
            //mapping.InheritanceSourceAttribute = null;
            //mapping.InheritanceSourceObjectClass = null;
            //mapping.InheritedAttribute = null;
            //mapping.Attribute = null;

            this.DataContext.SubmitChanges();

            this.RaiseBindingDeletedEvent(objectClassName, attributeName);
        }

        private void DeleteMappingFromObjects(AcmaSchemaMapping mapping)
        {
            DBQueryByValue query1 = new DBQueryByValue(mapping.Attribute, Fim.Core.ValueOperator.IsPresent);
            DBQueryByValue query2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectClass"), Fim.Core.ValueOperator.Equals, new ValueDeclaration(mapping.ObjectClass.Name));
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.DBQueries.Add(query1);
            group.DBQueries.Add(query2);

            this.ProgressInfo.OperationDescription = string.Format("Removing binding {{{1}}} from '{0}'", mapping.ObjectClass.Name, mapping.Attribute.Name);
            this.ProgressInfo.MinValue = 0;
            this.ProgressInfo.MaxValue = 100;
            this.ProgressInfo.CurrentValue = 0;
            this.ProgressInfo.ProgressDescription = "Searching for objects";
            this.ProgressInfo.CanCancel = true;
            this.ProgressInfo.Canceled = false;

            try
            {
                using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionManager.MaximumTimeout))
                {
                    ActiveConfig.DB.MADataConext.SqlConnection.EnlistTransaction(Transaction.Current);

                    List<MAObjectHologram> holograms = ActiveConfig.DB.MADataConext.GetMAObjectsFromDBQuery(group).ToList();
                    this.ProgressInfo.MaxValue = holograms.Count;

                    foreach (MAObjectHologram hologram in holograms)
                    {
                        this.ProgressInfo.ProgressDescription = "Processing " + hologram.DisplayText;

                        if (this.ProgressInfo.Canceled)
                        {
                            throw new OperationCanceledException();
                        }

                        hologram.SetObjectModificationType(ObjectModificationType.Update, false);

                        if (mapping.Attribute.IsInheritedInClass(mapping.ObjectClass))
                        {
                            AcmaInheritedAttributeChange change = new AcmaInheritedAttributeChange(mapping.Attribute, AttributeModificationType.Delete, null);
                            hologram.InternalAttributeChanges.Add(change);
                        }
                        else
                        {
                            hologram.DeleteAttribute(mapping.Attribute);
                        }

                        hologram.CommitCSEntryChange();
                        this.ProgressInfo.CurrentValue++;
                    }

                    transaction.Complete();
                }
            }
            catch
            {
                ActiveConfig.DB.MADataConext.ResetConnection();
                throw;
            }
        }

        private void CreateBuiltInMappings(AcmaSchemaObjectClass objectClass)
        {
            this.CreateMapping(objectClass, this.GetAttribute("objectId"), null, null, null, objectClass.MappingsBindingList, true);
            this.CreateMapping(objectClass, this.GetAttribute("deleted"), null, null, null, objectClass.MappingsBindingList, true);
            this.CreateMapping(objectClass, this.GetAttribute("objectClass"), null, null, null, objectClass.MappingsBindingList, true);
        }

        private void CreateBuiltInMappingsForShadowObject(AcmaSchemaObjectClass objectClass)
        {
            this.CreateMapping(objectClass, this.GetAttribute("shadowParent"), null, null, null, objectClass.MappingsBindingList, true);
            this.CreateMapping(objectClass, this.GetAttribute("shadowLink"), null, null, null, objectClass.MappingsBindingList, true);
        }


        public void CreateIndex(AcmaSchemaAttribute attribute)
        {
            this.ThrowOnNullArgument("attribute", attribute);
            this.ValidateCanCreateIndex(attribute);
            this.DataContext.spSchemaCreateIndex(attribute.Name);
        }

        public void DeleteIndex(AcmaSchemaAttribute attribute)
        {
            this.ThrowOnNullArgument("attribute", attribute);
            this.ValidateCanDeleteIndex(attribute);
            this.DataContext.spSchemaDeleteIndex(attribute.Name);
        }



        private void ValidateCanCreateIndex(AcmaSchemaAttribute attribute)
        {
            this.ThrowOnNullArgument("attribute", attribute);

            if (attribute.Type == ExtendedAttributeType.Boolean)
            {
                throw new InvalidOperationException("An index cannot be created on a boolean attribute");
            }

            if (attribute.IsMultivalued)
            {
                throw new InvalidOperationException("Multivalued attributes cannot be explicitly indexed");
            }

            if (!attribute.IsIndexable)
            {
                throw new InvalidOperationException("The attribute data type is not indexable");
            }

            if (attribute.IsIndexed)
            {
                throw new InvalidOperationException("The attribute is already indexed");
            }
        }

        private void ValidateCanDeleteIndex(AcmaSchemaAttribute attribute)
        {
            this.ThrowOnNullArgument("attribute", attribute);

            if (attribute.IsMultivalued)
            {
                throw new InvalidOperationException("Cannot delete the index on a multivalued attribute");
            }

            if (!attribute.IsIndexed)
            {
                throw new InvalidOperationException("The attribute is not currently indexed");
            }
        }

        private void ValidateCanDeleteObjectClass(AcmaSchemaObjectClass objectClass)
        {
            this.ThrowOnNullArgument("objectClass", objectClass);

            if (objectClass.ShadowChildren.Any())
            {
                throw new InvalidOperationException("The object class is inherited by one or more shadow classes. Delete the shadow classes before attempting to delete this object");
            }

            if (ActiveConfig.XmlConfig != null)
            {
                if (ActiveConfig.XmlConfig.ClassConstructors.Contains(objectClass.Name))
                {
                    throw new InvalidOperationException("The object class has a class constructor which must be deleted first");
                }
            }
        }

        private void ValidateCanCreateMapping(AcmaSchemaObjectClass objectClass, AcmaSchemaAttribute attribute, AcmaSchemaAttribute inheritanceSourceAttribute, AcmaSchemaObjectClass inheritanceSourceObjectClass, AcmaSchemaAttribute inheritedAttribute, bool isBuiltIn)
        {
            if (inheritedAttribute == null || inheritanceSourceObjectClass == null || inheritanceSourceAttribute == null)
            {
                if (inheritedAttribute != null || inheritanceSourceObjectClass != null || inheritanceSourceAttribute != null)
                {
                    throw new InvalidOperationException("The inherited attribute, inheritance source, and source object class must all be provided");
                }
                else
                {
                    if (this.Mappings.Any(t => t.Attribute == attribute && t.ObjectClass == objectClass))
                    {
                        throw new InvalidOperationException("The specified attribute mapping already exists");
                    }
                    else
                    {
                        return;
                    }
                }
            }

            if (!inheritanceSourceObjectClass.Attributes.Any(t => t == inheritedAttribute))
            {
                throw new InvalidOperationException(string.Format("The attribute {0} cannot be inherited as it does not exist on the source object type {1}", inheritedAttribute.Name, inheritanceSourceObjectClass.Name));
            }

            if (inheritanceSourceAttribute.Type != ExtendedAttributeType.Reference)
            {
                throw new UnknownOrUnsupportedDataTypeException("An inheritance source must be a reference data type");
            }

            if (inheritanceSourceAttribute.IsMultivalued)
            {
                throw new InvalidOperationException("Cannot inherit from a multivalued reference");
            }

            if (inheritedAttribute.Type != attribute.Type)
            {
                throw new UnknownOrUnsupportedDataTypeException(string.Format(
                    "An attribute can only inherit from an attribute with a matching data type. {0} has a data type of {1}, and {2} has a data type of {3}",
                    attribute.Name,
                    attribute.Type.ToString(),
                    inheritedAttribute.Name,
                    inheritedAttribute.Type.ToString()
                    ));
            }

            if (inheritedAttribute.IsMultivalued != attribute.IsMultivalued)
            {
                throw new InvalidOperationException("An inherited attribute must match the multivalued characteristics of its source attribute");
            }
        }

        private void ValidateNewObjectClassParameters(string name, AcmaSchemaObjectClass shadowFromClass)
        {
            this.ThrowOnNullArgument("name", name);

            if (name.Length > 50)
            {
                throw new ArgumentOutOfRangeException("The object class name must be a maximum of 50 characters");
            }

            if (this.ObjectClasses.Any(t => t.Name == name))
            {
                throw new DuplicateObjectException(string.Format("The object class {0} could not be created because another object with the same name already exists", name));
            }
        }

        private void ValidateAttributeParameters(string name, ExtendedAttributeType type, bool isMultiValued, AcmaAttributeOperation operation, bool indexable, bool indexed)
        {
            this.ThrowOnNullArgument("name", name);

            if (this.Attributes.Any(t => t.Name == name))
            {
                throw new DuplicateObjectException(string.Format("The attribute {0} could not be created because another object with the same name already exists", name));
            }

            if (name.Length > 50)
            {
                throw new ArgumentOutOfRangeException("The attribute name must be a maximum of 50 characters");
            }

            if (type == ExtendedAttributeType.Boolean)
            {
                if (isMultiValued)
                {
                    throw new InvalidOperationException("An attribute of type Boolean cannot be multivalued");
                }

                if (indexed || indexable)
                {
                    throw new InvalidOperationException("An attribute of type Boolean cannot be indexed");
                }
            }
            else if (type == ExtendedAttributeType.Undefined)
            {
                throw new UnknownOrUnsupportedDataTypeException("The specified data type is unsupported");
            }
        }

        public bool CanDeleteAttribute(AcmaSchemaAttribute attribute)
        {
            return !(
                attribute.IsBuiltIn ||
                attribute.Mappings.Any() ||
                attribute.BackLinks.Any() ||
                attribute.ForwardLinks.Any() ||
                attribute.InheritanceMappingSources.Any() ||
                attribute.InheritanceMappingValues.Any() ||
                attribute.ShadowObjectProvisioningLinks.Any() ||
                attribute.ShadowObjectReferenceLinks.Any());
        }

        private void ValidateCanDeleteAttribute(AcmaSchemaAttribute attribute)
        {
            if (attribute.Mappings.Any())
            {
                throw new InvalidOperationException("The attribute cannot be deleted as it used by one or more object classes");
            }

            if (attribute.BackLinks.Any())
            {
                throw new InvalidOperationException("The attribute cannot be deleted as it is the target of one or more back-links");
            }

            if (attribute.ForwardLinks.Any())
            {
                throw new InvalidOperationException("The attribute cannot be deleted as it is the source of one or more back-links");
            }

            if (attribute.InheritanceMappingSources.Any())
            {
                throw new InvalidOperationException("The attribute cannot be deleted as it is the source of one or more inherited attributes");
            }

            if (attribute.InheritanceMappingValues.Any())
            {
                throw new InvalidOperationException("The attribute cannot be deleted as it is inherited by one or more attributes");
            }

            if (attribute.ShadowObjectProvisioningLinks.Any())
            {
                throw new InvalidOperationException("The attribute cannot be deleted as it is used in one or more shadow object provisioning links");
            }

            if (attribute.ShadowObjectReferenceLinks.Any())
            {
                throw new InvalidOperationException("The attribute cannot be deleted as it is used in one or more shadow object reference links");
            }
        }

        private void ValidateCanDeleteMapping(AcmaSchemaMapping mapping)
        {
            if (mapping.IsBuiltIn)
            {
                throw new InvalidOperationException("Cannot delete a built-in attribute binding");
            }

            if (mapping.Attribute.InheritanceMappingSources.Any(t => t != mapping && t.ObjectClass == mapping.ObjectClass))
            {
                throw new InvalidOperationException("The attribute cannot be deleted because one or more attributes inherit from this attribute. Delete the inherited mapping and retry the operation");
            }

            if (mapping.Attribute.InheritanceMappingValues.Any(t => t != mapping && t.InheritanceSourceObjectClass == mapping.ObjectClass))
            {
                throw new InvalidOperationException("The attribute cannot be deleted because one or more attributes inherit this attribute's value. Delete the inherited mapping and retry the operation");
            }

            if (ActiveConfig.XmlConfig != null)
            {
                IEnumerable<SchemaAttributeUsage> usage = (ActiveConfig.XmlConfig.GetAttributeUsage(mapping.Attribute, mapping.ObjectClass));

                if (usage != null && usage.Count() > 0)
                {
                    throw new InvalidOperationException(string.Format("The attribute is in use in the config file\n{0}",
                        usage.Select(t => string.Format("{0}, {1}, {2}", t.ObjectType, t.Path, t.Context)).ToCommaSeparatedString()));
                }
            }
        }

        private void ValidateCanDeleteShadowLink(AcmaSchemaShadowObjectLink link)
        {
            //TODO: Validate that no MA_Objects are using this link before it is deleted.
        }

        private void ValidateCanCreateShadowLink(AcmaSchemaObjectClass shadowClass, AcmaSchemaAttribute provisioningAttribute, AcmaSchemaAttribute referenceAtttribute, string name)
        {
            if (!shadowClass.IsShadowObject)
            {
                throw new InvalidOperationException("The specified class is not a shadow class");
            }

            if (provisioningAttribute.Type != ExtendedAttributeType.Boolean)
            {
                throw new InvalidOperationException("The provisioning attribute must be a boolean data type");
            }

            if (provisioningAttribute.Operation == AcmaAttributeOperation.AcmaInternalTemp)
            {
                throw new InvalidOperationException("The provisioning attribute cannot be a temporary attribute");
            }

            if (referenceAtttribute.Type != ExtendedAttributeType.Reference)
            {
                throw new InvalidOperationException("The reference attribute must be a reference data type");
            }

            if (referenceAtttribute.IsMultivalued)
            {
                throw new InvalidOperationException("The reference attribute cannot be multivalued");
            }

            if (referenceAtttribute.Operation == AcmaAttributeOperation.AcmaInternalTemp)
            {
                throw new InvalidOperationException("The reference attribute cannot be a temporary attribute");
            }

            if (this.ShadowObjectLinks.Any(t => t.Name == name))
            {
                throw new InvalidOperationException("The specified mapping name is already in use");
            }
        }

        public void ThrowOnMissingAttribute(string name)
        {
            if (!this.Attributes.Any(t => t.Name == name))
            {
                throw new NoSuchAttributeException(name);
            }
        }

        private void ThrowOnNullArgument(string argumentName, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        private void RaiseAttributeRenamedEvent(AcmaSchemaAttribute attribute, string oldName)
        {
            if (this.OnAttributeRenamed != null)
            {
                this.OnAttributeRenamed(attribute, oldName);
            }
        }

        private void RaiseAttributeDeletedEvent(string name)
        {
            if (this.OnAttributeDeleted != null)
            {
                this.OnAttributeDeleted(name);
            }
        }

        private void RaiseBindingDeletedEvent(string objectClassName, string attributeName)
        {
            if (this.OnBindingDeleted != null)
            {
                this.OnBindingDeleted(objectClassName, attributeName);
            }
        }

        private void RaiseObjectClassDeletedEvent(string objectClassName)
        {
            if (this.OnObjectClassDeleted != null)
            {
                this.OnObjectClassDeleted(objectClassName);
            }
        }

        private void RaiseObjectClassRenamedEvent(AcmaSchemaObjectClass objectClass, string oldName)
        {
            if (this.OnObjectClassRenamed != null)
            {
                this.OnObjectClassRenamed(objectClass, oldName);
            }
        }
    }
}
