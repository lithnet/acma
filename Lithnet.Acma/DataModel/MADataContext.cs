// -----------------------------------------------------------------------
// <copyright file="MADataContext.cs" company="Lithnet">
// Copyright (c) 2013
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;
    using Lithnet.Logging;
    using Microsoft.MetadirectoryServices;
    using Reeb.SqlOM;
    using Lithnet.MetadirectoryServices;
    using Lithnet.Acma.DataModel;

    /// <summary>
    /// Provides access to the MAObjects table in the ACMA database
    /// </summary>
    public class MADataContext : IDisposable
    {
        /// <summary>
        /// The data context in use on this thread
        /// </summary>
        private DBConnection dbc;

        /// <summary>
        /// Indicates if the object has been disposed
        /// </summary>
        private bool disposed;

        private SqlConnection sharedHologramSqlConnection;

        /// <summary>
        /// Initializes a new instance of the MADataContext class
        /// </summary>
        /// <param name="connectionString">The SQL connection string</param>
        public MADataContext(string serverName, string databaseName)
        {
            this.dbc = new DBConnection(serverName, databaseName);
            this.sharedHologramSqlConnection = this.dbc.GetSqlConnection();
        }

        /// <summary>
        /// Initializes a new instance of the MADataContext class
        /// </summary>
        /// <param name="connectionString">The SQL connection string</param>
        public MADataContext(string connectionString)
        {
            this.dbc = new DBConnection(connectionString);
            this.sharedHologramSqlConnection = this.dbc.GetSqlConnection();
        }

        //public void ResetConnection()
        //{
        //    this.dbc.Connection.Close();
        //    this.dbc.Connection.Open();
        //}

        public SqlConnection GetSqlConnection()
        {
            return this.dbc.GetSqlConnection();
        }

        /// <summary>
        /// Finalizes an instance of the MADataContext class
        /// </summary>
        ~MADataContext()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets the SqlConnection object used for this data context
        /// </summary>
        internal SqlConnection SharedHologramSqlConnection
        {
            get
            {
                return this.sharedHologramSqlConnection;
            }
        }

        /// <summary>
        /// Gets an MAObject from the database
        /// </summary>
        /// <param name="objectId">The ID of the object to retrieve</param>
        /// <returns>The MAObject that matches the specified ID</returns>
        public MAObjectHologram GetMAObject(Guid objectId, AcmaSchemaObjectClass objectClass)
        {
            MAObjectHologram maObject = this.GetMAObjectOrDefault(objectId, objectClass);

            if (maObject == null)
            {
                throw new System.Data.ObjectNotFoundException(string.Format("The object with id '{0}' was not found", objectId.ToString()));
            }
            else
            {
                return maObject;
            }
        }

        /// <summary>
        /// Gets an MAObject from the database
        /// </summary>
        /// <param name="objectId">The ID of the object to retrieve</param>
        /// <returns>The MAObject that matches the specified ID, or null if the object wasn't found</returns>
        public MAObjectHologram GetMAObjectOrDefault(Guid objectId, AcmaSchemaObjectClass objectClass)
        {
            if (objectId == Guid.Empty)
            {
                return null;
            }

            //if (!this.hologramCache.ContainsKey(objectId))
            //{
            //using (SqlConnection connection = this.dbc.GetSqlConnection())
            ////{
            SqlCommand command = new SqlCommand();
            command.Connection = this.sharedHologramSqlConnection;
            command.CommandType = CommandType.Text;
            command.CommandText = string.Format("SELECT {0} FROM [dbo].[{1}] WHERE objectId=@id AND objectClass=@class", objectClass.ColumnListForSelectQuery, AcmaDatabase.ObjectTableName);
            command.Parameters.AddWithValue("@id", objectId);
            command.Parameters.AddWithValue("@class", objectClass.Name);

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            builder.ConflictOption = ConflictOption.OverwriteChanges;
            builder.QuotePrefix = "[";
            builder.QuoteSuffix = "]";

            DataSet dataset = new DataSet();
            adapter.AcceptChangesDuringUpdate = true;

            if (adapter.Fill(dataset) == 0)
            {
                return null;
            }
            else
            {
                return new MAObjectHologram(dataset.Tables[0].Rows[0], adapter, this);

                //this.hologramCache.Add(objectId, new MAObjectHologram(dataset.Tables[0].Rows[0], adapter, this));
            }
            //}

            //return this.hologramCache[objectId];
        }

        /// <summary>
        /// Gets an MAObject from the database
        /// </summary>
        /// <param name="objectId">The ID of the object to retrieve</param>
        /// <returns>The MAObject that matches the specified ID, or null if the object wasn't found</returns>
        public MAObjectHologram GetMAObjectOrDefault(Guid objectId)
        {
            if (objectId == Guid.Empty)
            {
                return null;
            }

            //if (!this.hologramCache.ContainsKey(objectId))
            //{
            //using (SqlConnection connection = this.dbc.GetSqlConnection())
            //{
            SqlCommand command = new SqlCommand();
            command.Connection = this.sharedHologramSqlConnection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = string.Format("[dbo].[spGetMAObject]");
            command.Parameters.AddWithValue("@id", objectId);

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            builder.ConflictOption = ConflictOption.OverwriteChanges;
            builder.QuotePrefix = "[";
            builder.QuoteSuffix = "]";

            DataSet dataset = new DataSet();
            adapter.AcceptChangesDuringUpdate = true;

            if (adapter.Fill(dataset) == 0)
            {
                return null;
            }
            else
            {
                return new MAObjectHologram(dataset.Tables[0].Rows[0], adapter, this);

                //this.hologramCache.Add(objectId, new MAObjectHologram(dataset.Tables[0].Rows[0], adapter, this));
            }
            //}


            //return this.hologramCache[objectId];
        }

        /// <summary>
        /// Gets an MAObject from the database
        /// </summary>
        /// <param name="objectId">The ID of the object to retrieve</param>
        /// <returns>The MAObject that matches the specified ID, or null if the object was not found</returns>
        public MAObjectHologram GetMAObjectOrDefault(string objectId, AcmaSchemaObjectClass objectClass)
        {
            return this.GetMAObjectOrDefault(new Guid(objectId), objectClass);
        }

        /// <summary>
        /// Gets an enumeration of MAObjects from the database
        /// </summary>
        /// <param name="objectIds">The IDs of the objects to get</param>
        /// <returns>An enumeration of MAObjects</returns>
        public IEnumerable<MAObjectHologram> GetMAObjects(IEnumerable<string> objectIds)
        {
            return this.GetMAObjects(objectIds.Select(t => new Guid(t)));
        }

        /// <summary>
        /// Gets an enumeration of MAObjects from the database
        /// </summary>
        /// <param name="objectIds">The IDs of the objects to get</param>
        /// <returns>An enumeration of MAObjects</returns>
        public IEnumerable<MAObjectHologram> GetMAObjects(IEnumerable<Guid> objectIds)
        {
            foreach (Guid objectId in objectIds)
            {
                MAObjectHologram maObject = this.GetMAObjectOrDefault(objectId);
                if (maObject != null)
                {
                    yield return maObject;
                }
            }
        }

        /// <summary>
        /// Gets the MAObjects that are not marked as deleted, up to the specified watermark 
        /// </summary>
        /// <param name="watermark">The timestamp value of the highest entry to retrieve</param>
        /// <returns>An enumeration of MAObjects</returns>
        public IEnumerable<MAObjectHologram> GetMAObjects(byte[] watermark)
        {
            return this.GetMAObjects(watermark, false);
        }

        /// <summary>
        /// Gets the delta objects from the database, up to the specified watermark
        /// </summary>
        /// <param name="watermark">The timestamp value of the highest entry to retrieve</param>
        /// <returns>An enumeration of MAObjects</returns>
        public IEnumerable<MAObjectHologram> GetDeltaMAObjects(byte[] watermark)
        {
            return this.GetMAObjectsDelta(watermark, true);
        }

        public IEnumerable<MAObjectHologram> GetDeltaMAObjects(long lastVersion)
        {
            return this.GetMAObjectsDelta(lastVersion);
        }

        /// <summary>
        /// Changes an MAObject's unique identifier
        /// </summary>
        /// <param name="oldId">The old object ID</param>
        /// <param name="newId">The new object Id</param>
        /// <param name="undelete">A value indicating if the object should be undeleted if it is in a deleted state</param>
        public void ChangeMAObjectId(Guid oldId, Guid newId, bool undelete)
        {
            using (SqlConnection connection = this.dbc.GetSqlConnection())
            {
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "[dbo].[spChangeMAObjectId]";
                command.Parameters.AddWithValue("@oldId", oldId);
                command.Parameters.AddWithValue("@newId", newId);
                command.Parameters.AddWithValue("@undelete", undelete);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                builder.ConflictOption = ConflictOption.OverwriteChanges;
                builder.QuotePrefix = "[";
                builder.QuoteSuffix = "]";

                if (command.ExecuteNonQuery() == 0)
                {
                    throw new System.Data.ObjectNotFoundException(string.Format("The object with ID {0} could not be renamed as it could not be found", oldId));
                }
            }
        }

        /// <summary>
        /// Searches the MAObjects table for a deleted object matching the specified resurrection criteria
        /// </summary>
        /// <param name="resurrectionParameters">The list of ResurrectionParameters used to find the deleted object</param>
        /// <param name="csentry">The incoming CSEntryChange object with an ObjectModificationType of 'Add'</param>
        /// <returns>An MAObject that matches the specified resurrection criteria, or null if no matching object was found</returns>
        public MAObjectHologram GetResurrectionObject(DBQueryGroup resurrectionParameters, CSEntryChange csentry)
        {
            if (csentry.ObjectModificationType != ObjectModificationType.Add)
            {
                throw new ArgumentException("Only CSEntryChange objects marked with an ObjectModificationType of 'Add' can be used to search for objects to resurrect");
            }

            if (string.IsNullOrWhiteSpace(csentry.ObjectType))
            {
                throw new InvalidOperationException("The CSEntryChange did not specify an object class");
            }

            DBQueryGroup parentQuery = new DBQueryGroup();
            parentQuery.Operator = GroupOperator.All;
            DBQueryByValue deletedQuery = new DBQueryByValue(ActiveConfig.DB.GetAttribute("deleted"), ValueOperator.GreaterThan, 0);
            parentQuery.DBQueries.Add(deletedQuery);
            parentQuery.DBQueries.Add(new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectClass"), ValueOperator.Equals, csentry.ObjectType));
            parentQuery.DBQueries.Add(resurrectionParameters);

            List<MAObjectHologram> results = this.GetMAObjectsFromDBQuery(parentQuery, csentry, 2).ToList();

            if (results.Count == 1)
            {
                MAObjectHologram hologram = results.First();
                hologram.SetObjectModificationType(TriggerEvents.Undelete);
                return hologram;
            }
            else if (results.Count == 0)
            {
                return null;
            }
            else
            {
                Logger.WriteLine("Object with DN '{0}' was matched against the following multiple objects {1}", csentry.DN, results.Select(t => t.DisplayText).ToCommaSeparatedString());
                Logger.WriteLine("Ensure that any attributes used for resurrection are unique");
                throw new MultipleMatchException("Multiple objects were returned in the search for a deleted object for resurrection. Ensure any attributes used for resurrection are unique");
            }
        }

        /// <summary>
        /// Gets an enumeration of MAObjectHolograms from a given dynamic database query
        /// </summary>
        /// <param name="attribute">The attribute in the database to query</param>
        /// <param name="valueOperator">The comparison operation to use</param>
        /// <param name="values">The values to compare</param>
        /// <returns>An enumeration of MAObjectHolograms matching the given search criteria</returns>
        public IEnumerable<MAObjectHologram> GetMAObjectsFromDBQuery(AcmaSchemaAttribute attribute, ValueOperator valueOperator, IList<ValueDeclaration> values)
        {
            DBQueryByValue query = new DBQueryByValue(attribute, valueOperator, values);
            DBQueryGroup group = new DBQueryGroup();
            group.Operator = GroupOperator.All;
            group.AddChildQueryObjects(query);

            DBQueryBuilder queryBuilder = new DBQueryBuilder(group, 0);
            return this.GetMAObjectsFromQueryBuilder(queryBuilder);
        }

        /// <summary>
        /// Gets an enumeration of MAObjectHolograms from a given dynamic database query
        /// </summary>
        /// <param name="attribute">The attribute in the database to query</param>
        /// <param name="valueOperator">The comparison operation to use</param>
        /// <param name="values">The values to compare</param>
        /// <returns>An enumeration of MAObjectHolograms matching the given search criteria</returns>
        public IEnumerable<MAObjectHologram> GetMAObjectsFromDBQuery(AcmaSchemaAttribute attribute, ValueOperator valueOperator, IList<object> values)
        {
            DBQueryByValue query = new DBQueryByValue(attribute, valueOperator, values);
            DBQueryGroup group = new DBQueryGroup();
            group.Operator = GroupOperator.All;
            group.AddChildQueryObjects(query);

            DBQueryBuilder queryBuilder = new DBQueryBuilder(group, 0);
            return this.GetMAObjectsFromQueryBuilder(queryBuilder);
        }

        /// <summary>
        /// Gets an enumeration of MAObjectHolograms from a given dynamic database query
        /// </summary>
        /// <param name="attribute">The attribute in the database to query</param>
        /// <param name="valueOperator">The comparison operation to use</param>
        /// <param name="value">The value to compare</param>
        /// <returns>An enumeration of MAObjectHolograms matching the given search criteria</returns>
        public IEnumerable<MAObjectHologram> GetMAObjectsFromDBQuery(AcmaSchemaAttribute attribute, ValueOperator valueOperator, object value)
        {
            DBQueryByValue query = new DBQueryByValue(attribute, valueOperator, value);
            DBQueryGroup group = new DBQueryGroup();
            group.Operator = GroupOperator.All;
            group.AddChildQueryObjects(query);

            DBQueryBuilder queryBuilder = new DBQueryBuilder(group, 0);
            return this.GetMAObjectsFromQueryBuilder(queryBuilder);
        }

        /// <summary>
        /// Gets an enumeration of MAObjectHolograms from a given dynamic database query
        /// </summary>
        /// <param name="queryGroup">The query to evaluate</param>
        /// <returns>An enumeration of MAObjectHolograms matching the given search criteria</returns>
        public IEnumerable<MAObjectHologram> GetMAObjectsFromDBQuery(DBQueryObject queryObject)
        {
            if (queryObject is DBQueryGroup)
            {
                return this.GetMAObjectsFromDBQuery((DBQueryGroup)queryObject);
            }
            else if (queryObject is DBQueryByValue)
            {
                DBQueryGroup group = new DBQueryGroup(GroupOperator.Any);
                group.DBQueries.Add(queryObject);
                return this.GetMAObjectsFromDBQuery(group);
            }
            else
            {
                throw new InvalidOperationException("The DBQueryObject type is unknown");
            }
        }

        /// <summary>
        /// Gets an enumeration of MAObjectHolograms from a given dynamic database query
        /// </summary>
        /// <param name="queryGroup">The query to evaluate</param>
        /// <returns>An enumeration of MAObjectHolograms matching the given search criteria</returns>
        public IEnumerable<MAObjectHologram> GetMAObjectsFromDBQuery(DBQueryGroup queryGroup, OrderByTermCollection orderByTerms)
        {
            DBQueryBuilder queryBuilder = new DBQueryBuilder(queryGroup, orderByTerms);
            return this.GetMAObjectsFromQueryBuilder(queryBuilder);
        }

        /// <summary>
        /// Gets an enumeration of MAObjectHolograms from a given dynamic database query
        /// </summary>
        /// <param name="queryGroup">The query to evaluate</param>
        /// <returns>An enumeration of MAObjectHolograms matching the given search criteria</returns>
        public IEnumerable<MAObjectHologram> GetMAObjectsFromDBQuery(DBQueryGroup queryGroup)
        {
            DBQueryBuilder queryBuilder = new DBQueryBuilder(queryGroup, 0);
            return this.GetMAObjectsFromQueryBuilder(queryBuilder);
        }

        /// <summary>
        /// Gets an enumeration of MAObjectHolograms from a given dynamic database query
        /// </summary>
        /// <param name="queryGroup">The query to evaluate</param>
        /// <returns>An enumeration of MAObjectHolograms matching the given search criteria</returns>
        public IEnumerable<MAObjectHologram> GetMAObjectsFromDBQuery(DBQueryGroup queryGroup, int maximumResults)
        {
            DBQueryBuilder queryBuilder = new DBQueryBuilder(queryGroup, maximumResults);
            return this.GetMAObjectsFromQueryBuilder(queryBuilder);
        }

        /// <summary>
        /// Gets an enumeration of MAObjectHolograms from a given dynamic database query
        /// </summary>
        /// <param name="queryGroup">The query to evaluate</param>
        /// <param name="csentry">The object containing the source values for the query</param>
        /// <returns>An enumeration of MAObjectHolograms matching the given search criteria</returns>
        public IEnumerable<MAObjectHologram> GetMAObjectsFromDBQuery(DBQueryGroup queryGroup, CSEntryChange csentry)
        {
            try
            {
                DBQueryBuilder queryBuilder = new DBQueryBuilder(queryGroup, 0, csentry);
                return this.GetMAObjectsFromQueryBuilder(queryBuilder);
            }
            catch (QueryValueNullException)
            {
                Logger.WriteLine("The query could not be built as one or more required values for the query was null", LogLevel.Debug);
                //Logger.WriteException(ex, LogLevel.Debug);
                return new List<MAObjectHologram>();
            }
        }

        /// <summary>
        /// Gets an enumeration of MAObjectHolograms from a given dynamic database query
        /// </summary>
        /// <param name="queryGroup">The query to evaluate</param>
        /// <param name="maObject">The object containing the source values for the query</param>
        /// <returns>An enumeration of MAObjectHolograms matching the given search criteria</returns>
        public IEnumerable<MAObjectHologram> GetMAObjectsFromDBQuery(DBQueryGroup queryGroup, MAObjectHologram maObject)
        {
            try
            {
                DBQueryBuilder queryBuilder = new DBQueryBuilder(queryGroup, 0, maObject);
                return this.GetMAObjectsFromQueryBuilder(queryBuilder);
            }
            catch (QueryValueNullException)
            {
                Logger.WriteLine("The query could not be built as one or more required values for the query was null", LogLevel.Debug);
                //Logger.WriteException(ex, LogLevel.Debug);
                return new List<MAObjectHologram>();
            }
        }

        /// <summary>
        /// Gets an enumeration of MAObjectHolograms from a given dynamic database query
        /// </summary>
        /// <param name="queryGroup">The query to evaluate</param>
        /// <param name="csentry">The object containing the source values for the query</param>
        /// <param name="maximumResults">The maximum number of results to return</param>
        /// <returns>An enumeration of MAObjectHolograms matching the given search criteria</returns>
        public IEnumerable<MAObjectHologram> GetMAObjectsFromDBQuery(DBQueryGroup queryGroup, CSEntryChange csentry, int maximumResults)
        {
            try
            {
                DBQueryBuilder queryBuilder = new DBQueryBuilder(queryGroup, maximumResults, csentry);
                return this.GetMAObjectsFromQueryBuilder(queryBuilder);
            }
            catch (QueryValueNullException)
            {
                Logger.WriteLine("The query could not be built as one or more required values for the query was null", LogLevel.Debug);
                //Logger.WriteException(ex, LogLevel.Debug);
                return new List<MAObjectHologram>();
            }
        }

        /// <summary>
        /// Gets an enumeration of MAObjectHolograms from a given dynamic database query
        /// </summary>
        /// <param name="queryGroup">The query to evaluate</param>
        /// <param name="maObject">The object containing the source values for the query</param>
        /// <param name="maximumResults">The maximum number of results to return</param>
        /// <returns>An enumeration of MAObjectHolograms matching the given search criteria</returns>
        public IEnumerable<MAObjectHologram> GetMAObjectsFromDBQuery(DBQueryGroup queryGroup, MAObjectHologram maObject, int maximumResults)
        {
            try
            {
                DBQueryBuilder queryBuilder = new DBQueryBuilder(queryGroup, maximumResults, maObject);
                return this.GetMAObjectsFromQueryBuilder(queryBuilder);
            }
            catch (QueryValueNullException)
            {
                Logger.WriteLine("The query could not be built as one or more required values for the query was null", LogLevel.Debug);
                /// Logger.WriteException(ex, LogLevel.Debug);
                return new List<MAObjectHologram>();
            }
        }

        /// <summary>
        /// Determines if an attribute and value pair exist in the database
        /// </summary>
        /// <param name="attribute">The MASchemaAttribute object representing the attribute to search for</param>
        /// <param name="attributeValue">The attribute value</param>
        /// <returns>A value indicating whether the attribute and value exists on the object</returns>
        public bool DoesAttributeValueExist(AcmaSchemaAttribute attribute, object attributeValue, Guid requestingObjectID)
        {
            using (SqlConnection connection = this.dbc.GetSqlConnection())
            {
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                bool useLike = false;

                string attributeValueString = attributeValue as string;
                if (attributeValueString != null)
                {
                    if (attributeValueString.Contains("%"))
                    {
                        useLike = true;
                    }
                }

                if (attribute.IsInAVPTable)
                {
                    command.CommandText = string.Format("SELECT TOP 1 1 FROM [dbo].[{0}] WHERE [AttributeName] = '{1}' AND [{2}] {3} @value", attribute.TableName, attribute.Name, attribute.ColumnName, useLike ? "LIKE" : "=");
                }
                else
                {
                    command.CommandText = string.Format("SELECT TOP 1 1 FROM [dbo].[{0}] WHERE [{1}] {2} @value", attribute.TableName, attribute.ColumnName, useLike ? "LIKE" : "=");
                }

                command.Parameters.AddWithValue("@value", attributeValue);

                if (requestingObjectID != Guid.Empty)
                {
                    command.CommandText += " AND [objectid] != @objectID";
                    command.Parameters.AddWithValue("@objectID", requestingObjectID);
                }

                if (command.ExecuteScalar() == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Determines if an attribute and value pair exist in the database
        /// </summary>
        /// <param name="attribute">The MASchemaAttribute object representing the attribute to search for</param>
        /// <param name="attributeValue">The attribute value</param>
        /// <returns>A value indicating whether the attribute and value exists on the object</returns>
        internal IEnumerable<string> GetAttributeValues(AcmaSchemaAttribute attribute, object wildcardValue, Guid requestingObjectID)
        {
            using (SqlConnection connection = this.dbc.GetSqlConnection())
            {
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.Text;

                if (attribute.IsInAVPTable)
                {
                    command.CommandText = string.Format("SELECT {2} FROM [dbo].[{0}] WHERE [AttributeName] = '{1}' AND [{2}] like @value", attribute.TableName, attribute.Name, attribute.ColumnName);
                }
                else
                {
                    command.CommandText = string.Format("SELECT {1} FROM [dbo].[{0}] WHERE [{1}] like @value", attribute.TableName, attribute.ColumnName);
                }

                command.Parameters.AddWithValue("@value", wildcardValue);

                if (requestingObjectID != Guid.Empty)
                {
                    command.CommandText += " AND [objectid] != @objectID";
                    command.Parameters.AddWithValue("@objectID", requestingObjectID);
                }

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    yield return ((IDataRecord)reader).GetString(0);
                }
            }
        }

        /// <summary>
        /// Clears all delta changes less than and equal to the specified watermark value
        /// </summary>
        /// <param name="watermark">The watermark value</param>
        public void ClearDeltas(byte[] watermark)
        {
            using (SqlConnection connection = this.dbc.GetSqlConnection())
            {
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "[dbo].[spClearMAObjectsDelta]";
                command.Parameters.AddWithValue("@watermark", watermark);

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Creates a new MAObject of the specified object class
        /// </summary>
        /// <param name="id">The ID of the new MAObject</param>
        /// <param name="objectClass">The object class to create</param>
        /// <returns>A newly created MAObject</returns>
        public MAObjectHologram CreateMAObject(Guid id, AcmaSchemaObjectClass objectClass)
        {
            return this.CreateMAObject(id, objectClass.Name, null, ObjectModificationType.Add);
        }


        /// <summary>
        /// Creates a new MAObject of the specified object class
        /// </summary>
        /// <param name="id">The ID of the new MAObject</param>
        /// <param name="objectClass">The object class to create</param>
        /// <returns>A newly created MAObject</returns>
        public MAObjectHologram CreateMAObject(Guid id, string objectClass)
        {
            return this.CreateMAObject(id, objectClass, null, ObjectModificationType.Add);
        }

        /// <summary>
        /// Creates a new MAObject of the specified object class
        /// </summary>
        /// <param name="id">The ID of the new MAObject</param>
        /// <param name="objectClass">The object class to create</param>
        /// <returns>A newly created MAObject</returns>
        public MAObjectHologram CreateMAObject(Guid id, string objectClass, ObjectModificationType modificationType)
        {
            return this.CreateMAObject(id, objectClass, null, modificationType);
        }

        /// <summary>
        /// Creates a new MAObject of the specified object class
        /// </summary>
        /// <param name="id">The ID of the new MAObject</param>
        /// <param name="objectClass">The object class to create</param>
        /// <param name="shadowParent">The shadow parent of this object</param>
        /// <returns>A newly created MAObject</returns>
        public MAObjectHologram CreateMAObject(Guid id, string objectClass, MAObject shadowParent, ObjectModificationType modificationType)
        {
            //using (SqlConnection connection = this.dbc.GetSqlConnection())
            //{
            SqlCommand command = new SqlCommand();
            command.Connection = this.sharedHologramSqlConnection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "[dbo].[spCreateMAObject]";
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@objectClass", objectClass);

            if (shadowParent != null)
            {
                command.Parameters.AddWithValue("@shadowParent", shadowParent.ObjectID);
            }

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            builder.ConflictOption = ConflictOption.OverwriteChanges;
            builder.QuotePrefix = "[";
            builder.QuoteSuffix = "]";

            DataSet dataset = new DataSet();
            adapter.AcceptChangesDuringUpdate = true;

            if (adapter.Fill(dataset) == 0)
            {
                throw new DataException("The record could not be added to the database");
            }
            else
            {
                MAObjectHologram hologram = new MAObjectHologram(dataset.Tables[0].Rows[0], adapter, this);
                hologram.SetObjectModificationType(modificationType, false);
                return hologram;
            }
            // }
        }

        /// <summary>
        /// Permanently deletes an object and all its attributes from the database
        /// </summary>
        /// <param name="id">The ID of the object to delete</param>
        public void DeleteMAObjectPermanent(Guid id)
        {
            MAObjectHologram hologram = this.GetMAObjectOrDefault(id);

            if (hologram != null)
            {
                foreach (Guid shadowID in hologram.GetShadowObjects())
                {
                    this.DeleteMAObjectPermanent(shadowID);
                    MAStatistics.AddShadowDelete();
                }
            }

            using (SqlConnection connection = this.dbc.GetSqlConnection())
            {
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "[dbo].[spDeleteMAObject]";
                command.Parameters.AddWithValue("@id", id);

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Gets the highest timestamp from the MAObjects table
        /// </summary>
        /// <returns>A timestamp byte array</returns>
        public byte[] GetHighWatermarkMAObjects()
        {
            using (SqlConnection connection = this.dbc.GetSqlConnection())
            {
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "[dbo].[spGetHighWatermarkMAObjects]";

                return (byte[])command.ExecuteScalar();
            }
        }

        /// <summary>
        /// Gets the highest timestamp from the MA_Objects_Delta table
        /// </summary>
        /// <returns>A timestamp byte array</returns>
        public byte[] GetHighWatermarkMAObjectsDelta()
        {
            using (SqlConnection connection = this.dbc.GetSqlConnection())
            {
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "[dbo].[spGetHighWatermarkMAObjectsDelta]";

                return (byte[])command.ExecuteScalar();
            }
        }

        /// <summary>
        /// Disposes the current object
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the current object
        /// </summary>
        /// <param name="disposing">A value indicating if the disposal is coming from a call to IDisposable.Dispose()</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.dbc.Dispose();
                }
            }

            this.disposed = true;
        }

        public Dictionary<Guid, IList<string>> GetReferencingObjects(MAObjectHologram hologram)
        {
            return this.GetReferencingObjects(hologram.ObjectID);
        }

        public Dictionary<Guid, IList<string>> GetReferencingObjects(Guid referencedObjectId)
        {
            using (SqlConnection connection = this.dbc.GetSqlConnection())
            {
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "[dbo].[spGetReferences]";
                command.Parameters.AddWithValue("@id", referencedObjectId);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                builder.ConflictOption = ConflictOption.OverwriteChanges;
                builder.QuotePrefix = "[";
                builder.QuoteSuffix = "]";

                DataSet dataset = new DataSet();
                adapter.AcceptChangesDuringUpdate = true;
                adapter.Fill(dataset);

                Dictionary<Guid, IList<string>> foundReferences = new Dictionary<Guid, IList<string>>();

                foreach (DataRow row in dataset.Tables[0].Rows)
                {
                    Guid referencingObjectId = (Guid)row["objectId"];
                    string referencingAttribute = (string)row["attributeName"];

                    if (!foundReferences.ContainsKey(referencingObjectId))
                    {
                        foundReferences.Add(referencingObjectId, new List<string>());
                    }

                    foundReferences[referencingObjectId].Add(referencingAttribute);
                }

                return foundReferences;
            }
        }

        /// <summary>
        /// Gets one or more MAObjects from the database using the specified parameters
        /// </summary>
        /// <param name="watermark">The value of the highest timestamp that should be returned</param>
        /// <param name="getDeleted">A value indicating if deleted objects should be returned in the result set</param>
        /// <returns>An enumeration of MAObjects</returns>
        private IEnumerable<MAObjectHologram> GetMAObjects(byte[] watermark = null, bool getDeleted = false)
        {
            //using (SqlConnection connection = this.dbc.GetSqlConnection())
            //{
            SqlCommand command = new SqlCommand();
            command.Connection = this.sharedHologramSqlConnection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "[dbo].[spGetMAObjects]";

            if (watermark != null)
            {
                command.Parameters.AddWithValue("@watermark", watermark);
            }

            if (getDeleted)
            {
                command.Parameters.AddWithValue("@deleted", true);
            }

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            builder.ConflictOption = ConflictOption.OverwriteChanges;
            builder.QuotePrefix = "[";
            builder.QuoteSuffix = "]";

            DataSet dataset = new DataSet();
            adapter.AcceptChangesDuringUpdate = true;
            adapter.Fill(dataset);

            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                yield return new MAObjectHologram(row, adapter, this);
            }
            // }
        }

        /// <summary>
        /// Gets one or more MAObjects from the database using the specified parameters
        /// </summary>
        /// <param name="watermark">The value of the highest timestamp that should be returned</param>
        /// <param name="getDeleted">A value indicating if deleted objects should be returned in the result set</param>
        /// <returns>An enumeration of MAObjects</returns>
        public ResultEnumerator EnumerateMAObjects(IList<string> objectTypes, byte[] lowWatermark, byte[] highWatermark)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = this.sharedHologramSqlConnection;
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM [dbo].[MA_Objects] WHERE ([deleted] = 0) AND ";

            if (lowWatermark != null)
            {
                command.CommandText += " ([rowversion] > @p0) AND ";
                command.Parameters.AddWithValue("@p0", lowWatermark);
            }

            if (highWatermark != null)
            {
                command.CommandText += " ([rowversion] <= @p1) AND ";
                command.Parameters.AddWithValue("@p1", highWatermark);
            }

            string paramPlaceholders = string.Empty;

            for (int i = 0; i < objectTypes.Count; i++)
            {
                string paramId = string.Format("@o{0}", i);
                if (i < objectTypes.Count - 1)
                {
                    paramPlaceholders += string.Format("{0},", paramId);
                }
                else
                {
                    paramPlaceholders += paramId;
                }

                command.Parameters.AddWithValue(paramId, objectTypes[i]);
            }

            command.CommandText += string.Format(" objectClass IN ({0})", paramPlaceholders);

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            builder.ConflictOption = ConflictOption.OverwriteChanges;
            builder.QuotePrefix = "[";
            builder.QuoteSuffix = "]";

            DataSet dataset = new DataSet();
            adapter.AcceptChangesDuringUpdate = true;
            adapter.Fill(dataset);

            return new ResultEnumerator(dataset.Tables[0].Rows, adapter, this);
        }

        /// <summary>
        /// Gets one or more MAObjects from the delta table of the database using the specified parameters
        /// </summary>
        /// <param name="watermark">The value of the highest timestamp that should be returned</param>
        /// <param name="getDeleted">A value indicating if deleted objects should be returned in the result set</param>
        /// <returns>An enumeration of MAObjects</returns>
        public ResultEnumerator EnumerateMAObjectsDelta(byte[] watermark = null)
        {
            //using (SqlConnection connection = this.dbc.GetSqlConnection())
            //{
            SqlCommand command = new SqlCommand();
            command.Connection = this.sharedHologramSqlConnection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "[dbo].[spGetMAObjectsDelta]";

            if (watermark != null)
            {
                command.Parameters.AddWithValue("@watermark", watermark);
            }

            command.Parameters.AddWithValue("@deleted", true);

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            builder.ConflictOption = ConflictOption.OverwriteChanges;
            builder.QuotePrefix = "[";
            builder.QuoteSuffix = "]";

            DataSet dataset = new DataSet();
            adapter.AcceptChangesDuringUpdate = true;
            adapter.Fill(dataset);

            return new ResultEnumerator(dataset.Tables[0].Rows, adapter, this);
        }


        /// <summary>
        /// Gets one or more MAObjects from the delta table of the database using the specified parameters
        /// </summary>
        /// <param name="highWatermark">The value of the highest timestamp that should be returned</param>
        /// <param name="getDeleted">A value indicating if deleted objects should be returned in the result set</param>
        /// <returns>An enumeration of MAObjects</returns>
        private IEnumerable<MAObjectHologram> GetMAObjectsDelta(byte[] highWatermark = null, bool getDeleted = false)
        {
            //using (SqlConnection connection = this.dbc.GetSqlConnection())
            //{
            SqlCommand command = new SqlCommand();
            command.Connection = this.sharedHologramSqlConnection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "[dbo].[spGetMAObjectsDelta]";

            if (highWatermark != null)
            {
                command.Parameters.AddWithValue("@watermark", highWatermark);
            }

            if (getDeleted)
            {
                command.Parameters.AddWithValue("@deleted", true);
            }

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            builder.ConflictOption = ConflictOption.OverwriteChanges;
            builder.QuotePrefix = "[";
            builder.QuoteSuffix = "]";

            DataSet dataset = new DataSet();
            adapter.AcceptChangesDuringUpdate = true;
            adapter.Fill(dataset);

            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                yield return new MAObjectHologram(row, adapter, this);
            }
            // }
        }

        /// <summary>
        /// Gets one or more MAObjects from the delta table of the database using the specified parameters
        /// </summary>
        /// <param name="highWatermark">The value of the highest timestamp that should be returned</param>
        /// <param name="getDeleted">A value indicating if deleted objects should be returned in the result set</param>
        /// <returns>An enumeration of MAObjects</returns>
        private IEnumerable<MAObjectHologram> GetMAObjectsDelta(long lastVersion)
        {
            //using (SqlConnection connection = this.dbc.GetSqlConnection())
            //{
            SqlCommand command = new SqlCommand();
            command.Connection = this.sharedHologramSqlConnection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "[dbo].[spGetDeltaChanges]";

            command.Parameters.AddWithValue("@changeVersion", lastVersion);

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            builder.ConflictOption = ConflictOption.OverwriteChanges;
            builder.QuotePrefix = "[";
            builder.QuoteSuffix = "]";

            DataSet dataset = new DataSet();
            adapter.AcceptChangesDuringUpdate = true;
            adapter.Fill(dataset);

            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                yield return new MAObjectHologram(row, adapter, this);
            }
            //}
        }

        /// <summary>
        /// Gets one or more MAObjects from the delta table of the database using the specified parameters
        /// </summary>
        /// <param name="highWatermark">The value of the highest timestamp that should be returned</param>
        /// <param name="getDeleted">A value indicating if deleted objects should be returned in the result set</param>
        /// <returns>An enumeration of MAObjects</returns>
        public long GetCurrentChangeVersion()
        {
            using (SqlConnection connection = this.dbc.GetSqlConnection())
            {
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "[dbo].[spGetChangeTrackingInfo]";

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                builder.ConflictOption = ConflictOption.OverwriteChanges;
                builder.QuotePrefix = "[";
                builder.QuoteSuffix = "]";

                DataSet dataset = new DataSet();
                adapter.AcceptChangesDuringUpdate = true;
                adapter.Fill(dataset);

                return (long)dataset.Tables[0].Rows[0]["CurrentVersion"];
            }
        }

        /// <summary>
        /// Gets one or more MAObjects from the delta table of the database using the specified parameters
        /// </summary>
        /// <param name="highWatermark">The value of the highest timestamp that should be returned</param>
        /// <param name="getDeleted">A value indicating if deleted objects should be returned in the result set</param>
        /// <returns>An enumeration of MAObjects</returns>
        public long GetMinimumChangeVersion()
        {
            using (SqlConnection connection = this.dbc.GetSqlConnection())
            {
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "[dbo].[spGetChangeTrackingInfo]";

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                builder.ConflictOption = ConflictOption.OverwriteChanges;
                builder.QuotePrefix = "[";
                builder.QuoteSuffix = "]";

                DataSet dataset = new DataSet();
                adapter.AcceptChangesDuringUpdate = true;
                adapter.Fill(dataset);

                return (long)dataset.Tables[0].Rows[0]["MinVersion"];
            }
        }

        /// <summary>
        /// Returns the MAObjectHolograms that match the specified query 
        /// </summary>
        /// <param name="queryBuilder">The query to use</param>
        /// <returns>The MAObjectHolograms that match the specified query </returns>
        private IEnumerable<MAObjectHologram> GetMAObjectsFromQueryBuilder(DBQueryBuilder queryBuilder)
        {
            if (string.IsNullOrWhiteSpace(queryBuilder.QueryString))
            {
                yield break;
            }

            //using (SqlConnection connection = this.dbc.GetSqlConnection())
            //{
            SqlCommand command = new SqlCommand();
            command.Connection = this.sharedHologramSqlConnection;
            command.CommandType = CommandType.Text;
            command.CommandText = queryBuilder.QueryString;
            command.Parameters.AddRange(queryBuilder.Parameters.ToArray());

            Logger.WriteLine("Running query: {0}", LogLevel.Debug, command.CommandText);
            Logger.WriteLine("Parameters: {0}", LogLevel.Debug, queryBuilder.Parameters.Select(t => t.ParameterName + ":" + t.Value.ToSmartStringOrNull()).ToCommaSeparatedString());

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            builder.ConflictOption = ConflictOption.OverwriteChanges;
            builder.QuotePrefix = "[";
            builder.QuoteSuffix = "]";

            DataSet dataset = new DataSet();
            adapter.AcceptChangesDuringUpdate = true;
            int count = adapter.Fill(dataset);

            Logger.WriteLine("Query returned {0} objects", LogLevel.Debug, count);

            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                yield return new MAObjectHologram(row, adapter, this);
            }
            //}
        }
    }
}
