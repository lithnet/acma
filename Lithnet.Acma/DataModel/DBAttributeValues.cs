// -----------------------------------------------------------------------
// <copyright file="DBAttributeValues.cs" company="Ryan Newington">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;
    using Lithnet.Logging;
    using Microsoft.MetadirectoryServices;
    using Lithnet.MetadirectoryServices;
    using Lithnet.Acma.DataModel;
    using System.Transactions;

    /// <summary>
    /// Represents a collection of DBAttributeValue objects
    /// </summary>
    public class DBAttributeValues : AttributeValues
    {
        /// <summary>
        /// The collection of rows containing the attribute values
        /// </summary>
        private DataTable dataTable;

        /// <summary>
        /// The data adapter used on this thread
        /// </summary>
        private SqlDataAdapter dataAdapter;

        /// <summary>
        /// The ID of the MAObject that contains these attribute values
        /// </summary>
        private Guid objectId;

        /// <summary>
        /// Initializes a new instance of the DBAttributeValues class
        /// </summary>
        /// <param name="attribute">The schema attribute associated with the values stored in this object</param>
        /// <param name="dataTable">The DataTable object containing associated values for this collection</param>
        /// <param name="adapter">The data adapter used on this thread</param>
        /// <param name="objectId">The ID of the MAObject that contains the values stored in this collection</param>
        public DBAttributeValues(AcmaSchemaAttribute attribute, DataTable dataTable, SqlDataAdapter adapter, Guid objectId)
            : base(attribute)
        {
            if (!attribute.IsInAVPTable)
            {
                throw new ArgumentException("An DBAttributeValueCollection class can only be initialized for an attribute stored in an AVP table");
            }

            this.InternalValues = new List<AttributeValue>();
            this.dataTable = dataTable;
            this.dataAdapter = adapter;
            this.objectId = objectId;
            this.PopulateValuesFromDataTable();
        }

        private DBAttributeValues(AcmaSchemaAttribute attribute, DataTable dataTable, SqlDataAdapter adapter, Guid objectID, List<AttributeValue> values)
            : base(attribute)
        {
            if (!attribute.IsInAVPTable)
            {
                throw new ArgumentException("An DBAttributeValueCollection class can only be initialized for an attribute stored in an AVP table");
            }

            this.InternalValues = values;
            this.dataAdapter = adapter;
            this.dataTable = dataTable;
            this.objectId = objectID;
        }

        /// <summary>
        /// Gets a value indicating whether any values in the collection have changed
        /// </summary>
        public bool HasChanged
        {
            get
            {
                return this.dataTable.DataSet.HasChanges() || this.InternalValues.Any(t => t.HasChanged);
            }
        }

        /// <summary>
        /// Gets a read-only list of values contained in this collection
        /// </summary>
        public override IList<AttributeValue> Values
        {
            get
            {
                return this.InternalValues.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets a DBAttributeValues for the specified attribute of an MAObject
        /// </summary>
        /// <param name="attribute">The schema attribute to obtain</param>
        /// <param name="objectId">The ID of the MAObject to obtain the attributes from</param>
        /// <param name="dataContext">The current data context object used on this thread</param>
        /// <returns>A collection of DBAttributeValues</returns>
        public static DBAttributeValues GetAttributeValues(AcmaSchemaAttribute attribute, Guid objectId, MADataContext dataContext)
        {
            SqlDataAdapter adapter;
            return new DBAttributeValues(attribute, GetAttributeValuesDataTable(attribute, objectId, dataContext, out adapter), adapter, objectId);
        }

        /// <summary>
        /// Gets a DBAttributeValues for the specified attribute of an MAObject
        /// </summary>
        /// <param name="attribute">The schema attribute to obtain</param>
        /// <param name="objectId">The ID of the MAObject to obtain the attributes from</param>
        /// <param name="dataContext">The current data context object used on this thread</param>
        /// <returns>A collection of DBAttributeValues</returns>
        public static IList<DBAttributeValues> GetAttributeValues(IEnumerable<AcmaSchemaAttribute> attributes, Guid objectId, MADataContext dataContext)
        {
            List<DBAttributeValues> values = new List<DBAttributeValues>();

            SqlDataAdapter adapter;
            foreach (string tableName in attributes.Where(t => t.IsInAVPTable).Select(t => t.TableName).Distinct())
            {
                DataTable table = GetAttributeValuesDataTable(objectId, tableName, dataContext, out adapter);

                foreach (var attribute in attributes.Where(t => t.IsInAVPTable && t.TableName == tableName))
                {
                    values.Add(new DBAttributeValues(attribute, table, adapter, objectId));
                }
            }

            return values;
        }

        /// <summary>
        /// Checks the collection of values for the presence of a particular value
        /// </summary>
        /// <param name="obj">The value to test for</param>
        /// <returns>A value indicating if the specified value is contained in the collection of values</returns>
        public override bool HasValue(object obj)
        {
            return this.InternalValues.Any(t => t.ValueEquals(obj));
        }

        /// <summary>
        /// Adds a new value to the collection
        /// </summary>
        /// <param name="obj">The value to add</param>
        protected void AddValue(object obj)
        {
            if (!this.Attribute.IsMultivalued && this.InternalValues.Count >= 1)
            {
                throw new TooManyValuesException(this.Attribute.Name);
            }

            if (!this.HasValue(obj))
            {
                DataRow newRow = this.dataTable.NewRow();
                this.dataTable.Rows.Add(newRow);
                newRow["objectId"] = this.objectId;
                newRow["attributeName"] = this.Attribute.Name;
                DBAttributeValue newValue = new DBAttributeValue(this.Attribute, newRow);
                newValue.SetValue(obj);
                this.InternalValues.Add(newValue);
            }
        }

        /// <summary>
        /// Removes a value from the collection
        /// </summary>
        /// <param name="obj">The value to remove</param>
        protected void RemoveValue(object obj)
        {
            foreach (AttributeValue existingValue in this.InternalValues.ToList())
            {
                if (existingValue.ValueEquals(obj))
                {
                    this.InternalValues.Remove(existingValue);

                    if (existingValue is DBAttributeValue)
                    {
                        ((DBAttributeValue)existingValue).SetValue(null);
                    }

                }
            }
        }

        /// <summary>
        /// Removes all values from the collection
        /// </summary>
        public override void ClearValues()
        {
            foreach (AttributeValue existingValue in this.InternalValues.ToList())
            {
                this.InternalValues.Remove(existingValue);

                if (existingValue is DBAttributeValue)
                {
                    ((DBAttributeValue)existingValue).SetValue(null);
                }
            }
        }

        /// <summary>
        /// Applies a set of ValueChange objects to the collection of values
        /// </summary>
        /// <param name="valueChanges">The list of value changes to make</param>
        public override void ApplyValueChanges(IList<ValueChange> valueChanges)
        {
            if (!this.Attribute.IsMultivalued && valueChanges.Count(t => t.ModificationType == ValueModificationType.Add) > 1)
            {
                throw new TooManyValuesException(this.Attribute.Name);
            }

            if (!this.Attribute.IsMultivalued)
            {
                ValueChange valueAdd = valueChanges.FirstOrDefault(t => t.ModificationType == ValueModificationType.Add);

                if (valueAdd != null)
                {
                    this.ClearValues();
                    this.AddValue(valueAdd.Value);
                }

                foreach (ValueChange valueChange in valueChanges.Where(y => y.ModificationType == ValueModificationType.Delete))
                {
                    this.RemoveValue(valueChange.Value);
                }
            }
            else
            {
                foreach (ValueChange valueChange in valueChanges.OrderBy(y => y.ModificationType == ValueModificationType.Delete))
                {
                    if (valueChange.ModificationType == ValueModificationType.Add)
                    {
                        this.AddValue(valueChange.Value);
                    }
                    else if (valueChange.ModificationType == ValueModificationType.Delete)
                    {
                        this.RemoveValue(valueChange.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Commits all changes to the database
        /// </summary>
        internal void Commit()
        {
            using (this.dataAdapter.SelectCommand.Connection = ActiveConfig.DB.MADataConext.GetSqlConnection())
            {
                this.dataAdapter.Update(this.dataTable);
            }
        }

        /// <summary>
        /// Reverts any uncommitted changes to the database
        /// </summary>
        internal void Rollback()
        {
            this.dataTable.Reset();
            this.PopulateValuesFromDataTable();
        }

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns>A string that represents the current object</returns>
        public override string ToString()
        {
            return this.Values.Select(t => t.ToString()).ToCommaSeparatedString();
        }

        /// <summary>
        /// Gets a DBAttributeValues for the specified attribute of an MAObject
        /// </summary>
        /// <param name="attribute">The schema attribute to obtain</param>
        /// <param name="objectId">The ID of the MAObject to obtain the attributes from</param>
        /// <param name="context">The current data context used on this thread</param>
        /// <param name="adapter">The DataAdapter created for the DataTable returned by the method</param>
        /// <returns>A data table containing the result set</returns>
        private static DataTable GetAttributeValuesDataTable(AcmaSchemaAttribute attribute, Guid objectId, MADataContext context, out SqlDataAdapter adapter)
        {
            using (SqlConnection connection  = context.GetSqlConnection())
            {
                SqlCommand command = new SqlCommand();
                //command.Connection = context.SqlConnection;
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = string.Format("SELECT t1.[ID], t1.[objectId], t1.[attributeName], t1.[{1}] FROM [dbo].[{0}] t1 WHERE t1.[objectId]=@objectId AND t1.[attributeName]=@attributeName", attribute.TableName, attribute.ColumnName);
                command.Parameters.AddWithValue("@objectid", objectId);
                command.Parameters.AddWithValue("@attributeName", attribute.Name);

                adapter = new SqlDataAdapter(command);
                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                builder.ConflictOption = ConflictOption.OverwriteChanges;
                builder.QuotePrefix = "[";
                builder.QuoteSuffix = "]";
                DataSet dataset = new DataSet();
                dataset.Locale = System.Globalization.CultureInfo.CurrentCulture;
                adapter.AcceptChangesDuringUpdate = true;
                adapter.Fill(dataset);

                if (dataset.Tables.Count == 0)
                {
                    throw new DataException("The dataset contained no results");
                }
                else
                {
                    return dataset.Tables[0];
                }
            }
        }

        /// <summary>
        /// Gets a DBAttributeValues for the specified attribute of an MAObject
        /// </summary>
        /// <param name="attribute">The schema attribute to obtain</param>
        /// <param name="objectId">The ID of the MAObject to obtain the attributes from</param>
        /// <param name="context">The current data context used on this thread</param>
        /// <param name="adapter">The DataAdapter created for the DataTable returned by the method</param>
        /// <returns>A data table containing the result set</returns>
        private static DataTable GetAttributeValuesDataTable(Guid objectId, string tableName, MADataContext context, out SqlDataAdapter adapter)
        {
            using (SqlConnection connection = context.GetSqlConnection())
            {
                SqlCommand command = new SqlCommand();
                //command.Connection = context.SqlConnection;
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = string.Format("SELECT t1.* FROM [dbo].[{0}] t1 WHERE t1.[objectId]=@objectId ORDER BY [attributeName]", tableName);
                command.Parameters.AddWithValue("@objectid", objectId);

                adapter = new SqlDataAdapter(command);
                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                builder.ConflictOption = ConflictOption.OverwriteChanges;
                builder.QuotePrefix = "[";
                builder.QuoteSuffix = "]";
                DataSet dataset = new DataSet();
                dataset.Locale = System.Globalization.CultureInfo.CurrentCulture;
                adapter.AcceptChangesDuringUpdate = true;
                adapter.Fill(dataset);

                if (dataset.Tables.Count == 0)
                {
                    throw new DataException("The dataset contained no results");
                }
                else
                {
                    return dataset.Tables[0];
                }
            }
        }

        internal static IList<DBAttributeValues> GetAVPAttributesForObject(Guid objectId, MADataContext context, AcmaSchemaObjectClass objectClass)
        {
            List<DBAttributeValues> values = new List<DBAttributeValues>();

            foreach (string tableName in objectClass.AvpAttributes.Select(t => t.TableName).Distinct())
            {
                SqlDataAdapter adapter;
                DataTable table = DBAttributeValues.GetAttributeValuesDataTable(objectId, tableName, context, out adapter);
                values.AddRange(DBAttributeValues.GetDBAttributeValuesFromDataTable(table, adapter, objectId));
            }

            return values;
        }

        private static IEnumerable<DBAttributeValues> GetDBAttributeValuesFromDataTable(DataTable table, SqlDataAdapter adapter, Guid objectID)
        {
            Dictionary<AcmaSchemaAttribute, List<AttributeValue>> cache = new Dictionary<AcmaSchemaAttribute, List<AttributeValue>>();

            foreach (DataRow row in table.Rows)
            {
                AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute(row["attributeName"] as string);

                if (!cache.ContainsKey(attribute))
                {
                    cache.Add(attribute, new List<AttributeValue>());
                }

                cache[attribute].Add(new DBAttributeValue(attribute, row));
            }

            foreach (KeyValuePair<AcmaSchemaAttribute, List<AttributeValue>> value in cache)
            {
                yield return new DBAttributeValues(value.Key, table, adapter, objectID, value.Value);
            }

        }

        /// <summary>
        /// Populates the collection from the values in a data table
        /// </summary>
        private void PopulateValuesFromDataTable()
        {
            this.InternalValues.Clear();

            foreach (DataRow row in this.dataTable.Rows.OfType<DataRow>().Where(t => (string)t["attributeName"] == this.Attribute.Name))
            {
                this.InternalValues.Add(new DBAttributeValue(this.Attribute, row));
            }
        }
    }
}
