// -----------------------------------------------------------------------
// <copyright file="MAObject.cs" company="Lithnet">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Xml;
    using Lithnet.Logging;
    using Microsoft.MetadirectoryServices;
    using Lithnet.MetadirectoryServices;
    using Lithnet.Acma.DataModel;
    using Lithnet.Acma.ServiceModel;

    /// <summary>
    /// Represents an object from the ACMA database
    /// </summary>
    public abstract class MAObject
    {
        /// <summary>
        /// The data row for this object
        /// </summary>
        private DataRow dataRow;

        /// <summary>
        /// The data adapter for this object
        /// </summary>
        private SqlDataAdapter adapter;

        /// <summary>
        /// The schema entry for this object type
        /// </summary>
        private AcmaSchemaObjectClass schemaObject;

        /// <summary>
        /// Contains a cached collection of multivalued attributes
        /// </summary>
        private Dictionary<string, DBAttributeValues> attributeValuesCache;

        /// <summary>
        /// Initializes a new instance of the MAObject class
        /// </summary>
        /// <param name="row">The data row for the MAObject</param>
        /// <param name="adapter">The data adapter for the object</param>
        /// <param name="dc">The data context in use on this thread</param>
        protected MAObject(DataRow row, SqlDataAdapter adapter)
        {
            this.attributeValuesCache = new Dictionary<string, DBAttributeValues>();
            this.adapter = adapter;
            this.dataRow = row;
        }

        /// <summary>
        /// Gets the object ID
        /// </summary>
        public Guid ObjectID
        {
            get
            {
                if (this.dataRow["objectId"] != DBNull.Value)
                {
                    return (Guid)this.dataRow["objectId"];
                }
                else
                {
                    if (this.dataRow["deltaObjectId"] != DBNull.Value)
                    {
                        return (Guid)this.dataRow["deltaObjectId"];
                    }
                    else
                    {
                        throw new InvalidOperationException("The specified object does not have an object id");
                    }
                }
            }
        }

        /// <summary>
        /// Gets the object attributeChange type
        /// </summary>
        public string DeltaChangeType
        {
            get
            {
                if (this.dataRow.Table.Columns.Contains("operation"))
                {
                    return (string)this.dataRow["operation"];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the object attributeChange type
        /// </summary>
        public string DeltaObjectClassName
        {
            get
            {
                if (this.dataRow.Table.Columns.Contains("deltaObjectClass"))
                {
                    return this.dataRow["deltaObjectClass"] == DBNull.Value ? null : (string)this.dataRow["deltaObjectClass"];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the timestamp of when the object was deleted
        /// </summary>
        public long DeletedTimestamp
        {
            get
            {
                return (long)this.dataRow["deleted"];
            }

            set
            {
                this.dataRow["deleted"] = value;
                this.Commit();
            }
        }

        public string ShadowLinkName
        {
            get
            {
                object value = this.dataRow["shadowLink"];

                if (value == DBNull.Value)
                {
                    return null;
                }
                else
                {
                    return (string)value;
                }
            }
            set
            {
                if (value == null)
                {
                    this.dataRow["shadowLink"] = DBNull.Value;
                }
                else
                {
                    this.dataRow["shadowLink"] = value;
                }

                this.Commit();
            }
        }

        public AcmaSchemaShadowObjectLink ShadowLink
        {
            get
            {
                if (this.ShadowLinkName == null)
                {
                    return null;
                }
                else
                {
                    return ActiveConfig.DB.GetShadowLink(this.ShadowLinkName);
                }
            }
            set
            {
                if (value == null)
                {
                    this.ShadowLinkName = null;
                }
                else
                {
                    this.ShadowLinkName = value.Name;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this object has had an inherited value that has been updated. This will trigger the automatic creation of a delta record
        /// if one does not already exist, and the flag will be automatically cleared when the row is committed.
        /// </summary>
        public bool InheritedUpdate
        {
            get
            {
                return (bool)this.dataRow["inheritedUpdate"];
            }

            set
            {
                this.dataRow["inheritedUpdate"] = value;
            }
        }

        public byte[] RowVersion
        {
            get
            {
                return (byte[])this.dataRow["rowversion"];
            }
        }

        public long ChangeVersion
        {
            get
            {
                if (dataRow.Table.Columns.Contains("version"))
                {
                    return (long)this.dataRow["version"]; ;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets or sets the shadow parent of this object
        /// </summary>
        public Guid ShadowParentID
        {
            get
            {
                object value = this.dataRow["shadowParent"];

                if (value == DBNull.Value)
                {
                    return Guid.Empty;
                }
                else
                {
                    return (Guid)value;
                }
            }

            set
            {
                if (value == Guid.Empty)
                {
                    this.dataRow["shadowParent"] = null;
                }
                else
                {
                    this.dataRow["shadowParent"] = value;
                }
            }
        }

        /// <summary>
        /// Gets the schema entry for this object type
        /// </summary>
        public AcmaSchemaObjectClass ObjectClass
        {
            get
            {
                if (this.schemaObject == null)
                {
                    string name = this.dataRow["objectClass"] as string;
                    if (name == null)
                    {
                        return null;
                    }
                    else
                    {
                        this.schemaObject = ActiveConfig.DB.GetObjectClass(name);
                    }
                }

                return this.schemaObject;
            }
        }

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns>A string that represents the current object</returns>
        public override string ToString()
        {
            return this.ObjectID.ToString();
        }

        internal void PreLoadAVPs()
        {
            this.PreLoadAVPs(this.ObjectClass.Attributes.Where(t => t.IsInAVPTable));
        }

        internal void PreLoadAVPs(IEnumerable<AcmaSchemaAttribute> attributes)
        {
            IList<DBAttributeValues> valueSets = DBAttributeValues.GetAttributeValues(attributes, this.ObjectID);

            foreach (var valueSet in valueSets)
            {
                if (!this.attributeValuesCache.ContainsKey(valueSet.Attribute.Name))
                {
                    this.attributeValuesCache.Add(valueSet.Attribute.Name, valueSet);
                }
            }
        }

        /// <summary>
        /// Determines if the specified attribute has one or more values
        /// </summary>
        /// <param name="attribute">The attribute to look up</param>
        /// <returns>A value indicating if the attribute has one or more values</returns>
        protected bool DBHasAttribute(AcmaSchemaAttribute attribute)
        {
            return !this.DBGetAttributeValues(attribute).IsEmptyOrNull;
        }

        /// <summary>
        /// Determines if the specified attribute has a specified value
        /// </summary>
        /// <param name="attribute">The attribute to look up</param>
        /// <param name="value">The value to search for</param>
        /// <returns>A value indicating if the attribute has the value specified</returns>
        protected bool DBHasAttributeValue(AcmaSchemaAttribute attribute, object value)
        {
            return this.DBGetAttributeValues(attribute).HasValue(value);
        }

        /// <summary>
        /// Rolls back changes made to the object
        /// </summary>
        protected void Rollback()
        {
            this.dataRow.RejectChanges();

            foreach (KeyValuePair<string, DBAttributeValues> pair in this.attributeValuesCache)
            {
                pair.Value.Rollback();
            }

            this.attributeValuesCache.Clear();
        }

        private SqlConnection workingConnection;

        /// <summary>
        /// Commits changes made to the object
        /// </summary>
        protected void Commit()
        {
            this.adapter.RowUpdating += adapter_RowUpdating;
            if (this.dataRow.RowState != DataRowState.Unchanged)
            {
                using (SqlConnection connection = ActiveConfig.DB.GetNewConnection())
                {
                    this.workingConnection = connection;
                    this.adapter.SelectCommand.Connection = connection;
                    this.adapter.Update(this.dataRow.Table.DataSet);
                }
                this.workingConnection = null;
            }
            
            foreach (KeyValuePair<string, DBAttributeValues> pair in this.attributeValuesCache)
            {
                pair.Value.Commit();
            }

            this.attributeValuesCache.Clear();
        }

        /*
         The following is a workaround for a bug that is appearing 
         */

        private void adapter_RowUpdating(object sender, SqlRowUpdatingEventArgs e)
        {
            if (e.Command != null)
            {
                if (e.Command.Connection == null || string.IsNullOrWhiteSpace(e.Command.Connection.ConnectionString))
                {
                    if (this.workingConnection != null)
                    {
                        e.Command.Connection = this.workingConnection;
                    }
                }
            }
        }

        protected AttributeValue DBGetSVAttributeValue(AcmaSchemaAttribute attribute)
        {
            if (attribute.IsMultivalued)
            {
                throw new ArgumentException("The specified attribute is a multivalued attribute");
            }

            AttributeValues values = this.DBGetAttributeValues(attribute);

            return values.FirstOrDefault() ?? new AttributeValue(attribute);
        }

        protected AttributeValues DBGetMVAttributeValues(AcmaSchemaAttribute attribute)
        {
            if (!attribute.IsMultivalued)
            {
                throw new ArgumentException("The specified attribute is a single-valued attribute");
            }

            return this.DBGetAttributeValues(attribute);
        }

        protected AttributeValues DBGetAttributeValues(AcmaSchemaAttribute attribute)
        {
            if (attribute.Operation == AcmaAttributeOperation.AcmaInternalTemp)
            {
                return null;
            }

            if (this.IsAttributeInherited(attribute))
            {
                throw new InvalidOperationException("An inherited attribute was requested from the underlying MAObject");
            }

            if (attribute.IsInAVPTable)
            {
                return this.DBGetAVPAttributeValues(attribute);
            }
            else
            {
                return new DBAttributeValuesSVWrapper(this.DBGetOTAttributeValue(attribute));
            }
        }

        /// <summary>
        /// Gets the value of the specified single-valued attribute
        /// </summary>
        /// <param name="attribute">The attribute to obtain the value for</param>
        /// <returns>A DBAttributeValue object containing the value of the specified attribute</returns>
        private DBAttributeValue DBGetOTAttributeValue(AcmaSchemaAttribute attribute)
        {
            return new DBAttributeValue(attribute, this.dataRow);
        }

        /// <summary>
        /// Gets the values of the specified multivalued attribute
        /// </summary>
        /// <param name="attribute">The attribute to obtain the value for</param>
        /// <returns>A DBAttributeValues object containing the values of the specified attribute</returns>
        private DBAttributeValues DBGetAVPAttributeValues(AcmaSchemaAttribute attribute)
        {
            if (!this.attributeValuesCache.ContainsKey(attribute.Name))
            {
                this.attributeValuesCache.Add(attribute.Name, DBAttributeValues.GetAttributeValues(attribute, this.ObjectID));
            }

            return this.attributeValuesCache[attribute.Name];
        }

        /// <summary>
        /// Deletes all the values of the specified attribute
        /// </summary>
        /// <param name="attribute">The attribute to delete the values from</param>
        protected void DBDeleteAttribute(AcmaSchemaAttribute attribute)
        {
            this.ThrowOnInheritedValueModification(attribute);

            if (attribute.Operation == AcmaAttributeOperation.AcmaInternalTemp)
            {
                return;
            }

            AttributeValues values = this.DBGetAttributeValues(attribute);
            values.ClearValues();
        }

        /// <summary>
        /// Updates the values of the specified attribute according to the supplied ValueChange list
        /// </summary>
        /// <param name="attribute">The attribute to update</param>
        /// <param name="valueChanges">The value changes to apply</param>
        protected void DBUpdateAttribute(AcmaSchemaAttribute attribute, IList<ValueChange> valueChanges)
        {
            this.ThrowOnInheritedValueModification(attribute);

            if (attribute.Operation == AcmaAttributeOperation.AcmaInternalTemp)
            {
                return;
            }

            AttributeValues values = this.DBGetAttributeValues(attribute);
            values.ApplyValueChanges(valueChanges);
        }

        /// <summary>
        /// Throws an exception if the specified attribute is inherited, and therefor read-only
        /// </summary>
        /// <param name="attribute">The attribute to examine</param>
        protected void ThrowOnInheritedValueModification(AcmaSchemaAttribute attribute)
        {
            if (this.IsAttributeInherited(attribute))
            {
                throw new InheritedValueModificationException(attribute.Name);
            }
        }

        private bool IsAttributeInherited(AcmaSchemaAttribute attribute)
        {
            return attribute.IsInheritedInClass(this.ObjectClass);
        }
    }
}