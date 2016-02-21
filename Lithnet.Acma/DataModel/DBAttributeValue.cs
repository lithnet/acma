// -----------------------------------------------------------------------
// <copyright file="DBAttributeValue.cs" company="Ryan Newington">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using Lithnet.Logging;
    using Microsoft.MetadirectoryServices;
    using Lithnet.MetadirectoryServices;
    using Lithnet.Acma.DataModel;
    using System.Data.SqlClient;

    /// <summary>
    /// Contains the value of an attribute stored in the database and methods for accessing its strongly-typed value
    /// </summary>
    public class DBAttributeValue : AttributeValue
    {
        /// <summary>
        /// The data row containing this attribute value
        /// </summary>
        private DataRow dataRow;

        /// <summary>
        /// Initializes a new instance of the DBAttributeValue class
        /// </summary>
        /// <param name="attribute">The schema attribute associated with the value stored in this object</param>
        /// <param name="row">The data row containing the attribute value</param>
        public DBAttributeValue(AcmaSchemaAttribute attribute, DataRow row)
            : base(attribute)
        {
            this.dataRow = row;
        }

        /// <summary>
        /// Gets the data row associated with this attribute value
        /// </summary>
        private DataRow DataRow
        {
            get
            {
                return this.dataRow;
            }
        }

        /// <summary>
        /// Gets the underlying object value
        /// </summary>
        public override object Value
        {
            get
            {
                if (this.Attribute.Type == ExtendedAttributeType.Boolean)
                {
                    if (this.dataRow.RowState == DataRowState.Deleted || this.dataRow.RowState == DataRowState.Detached)
                    {
                        return false;
                    }

                    return this.dataRow[this.Attribute.ColumnName] == System.DBNull.Value ? false : this.dataRow[this.Attribute.ColumnName];
                }
                else
                {
                    if (this.dataRow.RowState == DataRowState.Deleted || this.dataRow.RowState == DataRowState.Detached)
                    {
                        return null;
                    }

                    return this.dataRow[this.Attribute.ColumnName] == System.DBNull.Value ? null : this.dataRow[this.Attribute.ColumnName];

                }
            }
        }
      
        /// <summary>
        /// Gets a value indicating if the underlying attribute value is null
        /// </summary>
        public override bool IsNull
        {
            get
            {
                if (this.dataRow.RowState == DataRowState.Deleted || this.dataRow.RowState == DataRowState.Detached)
                {
                    return true;
                }
                else
                {
                    return this.dataRow[this.Attribute.ColumnName] == System.DBNull.Value;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating if the underlying attribute value has changed since the last commit
        /// </summary>
        public override bool HasChanged
        {
            get
            {
                return this.dataRow.RowState != DataRowState.Unchanged;
            }
        }

        /// <summary>
        /// Sets the underlying object value
        /// </summary>
        /// <param name="value">The new value to set</param>
        internal void SetValue(object value)
        {
            string dataRowName = this.Attribute.ColumnName;

            if (this.Attribute.IsInAVPTable && value == null)
            {
                this.dataRow.Delete();
            }
            else
            {
                this.dataRow[dataRowName] = value == null ? System.DBNull.Value : TypeConverter.ConvertData(value, this.Attribute.Type);
            }

            string proposedValue;
            string originalValue;

            switch (this.dataRow.RowState)
            {
                case DataRowState.Added:
                    proposedValue = this.dataRow[dataRowName, DataRowVersion.Current] == System.DBNull.Value ? "null" : this.dataRow[dataRowName, DataRowVersion.Current].ToSmartString();
                    Logger.WriteLine("Added value for {{{0}}}: {1}", this.Attribute.Name, proposedValue);
                    break;

                case DataRowState.Deleted:
                    originalValue = this.dataRow[dataRowName, DataRowVersion.Original] == System.DBNull.Value ? "null" : this.dataRow[dataRowName, DataRowVersion.Original].ToSmartString();
                    Logger.WriteLine("Deleted value from {{{0}}}: {1}", this.Attribute.Name, originalValue);
                    break;

                case DataRowState.Modified:
                    proposedValue = this.dataRow[dataRowName, DataRowVersion.Current] == System.DBNull.Value ? "null" : this.dataRow[dataRowName, DataRowVersion.Current].ToSmartString();
                    originalValue = this.dataRow[dataRowName, DataRowVersion.Original] == System.DBNull.Value ? "null" : this.dataRow[dataRowName, DataRowVersion.Original].ToSmartString();

                    if (proposedValue != originalValue)
                    {
                        if (proposedValue == "null")
                        {
                            Logger.WriteLine("Deleted value from {{{0}}}: {1}", this.Attribute.Name, originalValue);
                        }
                        else if (originalValue == "null")
                        {
                            Logger.WriteLine("Added value for {{{0}}}: {1}", this.Attribute.Name, proposedValue);
                        }
                        else
                        {
                            Logger.WriteLine("Changed value for {{{0}}}: {1} -> {2}", this.Attribute.Name, originalValue, proposedValue);
                        }
                    }

                    break;

                case DataRowState.Unchanged:
                    Logger.WriteLine("Attribute value unchanged {{{0}}}", LogLevel.Debug, this.Attribute.Name);
                    break;

                case DataRowState.Detached:
                default:
                    break;
            }
        }

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns>A string that represents the current object</returns>
        public override string ToString()
        {
            return this.Value == null ? null : this.Value.ToString();
        }
    }
}