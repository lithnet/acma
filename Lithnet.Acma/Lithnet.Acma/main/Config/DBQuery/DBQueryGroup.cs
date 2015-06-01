// -----------------------------------------------------------------------
// <copyright file="DBQueryGroup.cs" company="Ryan Newington">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    using Microsoft.MetadirectoryServices;
    using Reeb.SqlOM;
    using Lithnet.Acma.DataModel;

    /// <summary>
    /// Represents a group of attribute matches
    /// </summary>
    [DataContract(Name = "dbquery-group", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    public class DBQueryGroup : DBQueryObject
    {
        /// <summary>
        /// Initializes a new instance of the DBQueryGroup class
        /// </summary>
        public DBQueryGroup()
        {
            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the DBQueryGroup class
        /// </summary>
        /// <param name="groupOperator">The operator to apply to queries within this group</param>
        public DBQueryGroup(GroupOperator groupOperator)
            : this()
        {
            this.Operator = groupOperator;
        }

        /// <summary>
        /// Gets or sets the logical operator used to apply to this group
        /// </summary>
        [DataMember(Name = "operator")]
        public GroupOperator Operator { get; set; }

        /// <summary>
        /// Gets or sets the user-provided display name for this group
        /// </summary>
        [DataMember(Name = "display-name")]
        public string UserDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the list of query objects that are within this group
        /// </summary>
        [DataMember(Name = "dbqueries")]
        public ObservableCollection<DBQueryObject> DBQueries { get; set; }

        /// <summary>
        /// Adds a range of query objects to this group
        /// </summary>
        /// <param name="queries">The list of child objects to add to the group</param>
        public void AddChildQueryObjects(params DBQueryObject[] queries)
        {
            foreach (DBQueryObject match in queries)
            {
                this.DBQueries.Add(match);
            }
        }
       
        public override IEnumerable<SchemaAttributeUsage> GetAttributeUsage(string parentPath, AcmaSchemaAttribute attribute)
        {
            string path = string.Format("{0}\\{1}", parentPath, string.Format("query group ({0})", this.Operator.ToString().ToLower()));
                       
            foreach(DBQueryObject query in this.DBQueries)
            {
                foreach (SchemaAttributeUsage usage in query.GetAttributeUsage(path, attribute))
                {
                    yield return usage;
                }
            }
        }

        /// <summary>
        /// Creates the WHERE clause for this query
        /// </summary>
        /// <param name="builder">The builder used by this query</param>
        /// <param name="hologram">The hologram used as the source of query values</param>
        /// <returns>A WhereClause object containing the terms of the query</returns>
        public WhereClause CreateWhereClause(DBQueryBuilder builder, MAObjectHologram hologram)
        {
            if (this.DBQueries.Count == 0)
            {
                throw new InvalidOperationException("The query group cannot generate a query when no child queries are specified");
            }

            WhereClause parentClause;
            WhereClause querySubClause;

            if (this.Operator == GroupOperator.None)
            {
                // Workaround to stop SqlOm from dropping the operator when there is only a single term in the where clause
                parentClause = new WhereClause(WhereClauseRelationship.Not);
                parentClause.Terms.Add(WhereTerm.CreateCompare(SqlExpression.Number(1), SqlExpression.Number(1), CompareOperator.Equal));
                querySubClause = new WhereClause(WhereClauseRelationship.Or);
                parentClause.SubClauses.Add(querySubClause);
            }
            else
            {
                parentClause = new WhereClause(this.GetWhereClauseRelationship());
                querySubClause = parentClause;
            }

            IList<DBQueryObject> queryObjects = this.ProcessChildDBQueryObjects(querySubClause, builder, hologram);

            if (queryObjects.Count == 0)
            {
                throw new QueryValueNullException("The query group contained no valid queries to execute");
            }
            else
            {
                return parentClause;
            }
        }

        /// <summary>
        /// Creates the WHERE clause for this query
        /// </summary>
        /// <param name="builder">The builder used by this query</param>
        /// <param name="csentry">The CSEntryChange used as the source of query values</param>
        /// <returns>A WhereClause object containing the terms of the query</returns>
        public WhereClause CreateWhereClause(DBQueryBuilder builder, CSEntryChange csentry)
        {
            if (this.DBQueries.Count == 0)
            {
                throw new InvalidOperationException("The query group cannot generate a query when no child queries are specified");
            }

            WhereClause parentClause;
            WhereClause querySubClause;

            if (this.Operator == GroupOperator.None)
            {
                // Workaround to stop SqlOm from dropping the operator when there is only a single term in the where clause
                parentClause = new WhereClause(WhereClauseRelationship.Not);
                parentClause.Terms.Add(WhereTerm.CreateCompare(SqlExpression.Number(1), SqlExpression.Number(1), CompareOperator.Equal));
                querySubClause = new WhereClause(WhereClauseRelationship.Or);
                parentClause.SubClauses.Add(querySubClause);
            }
            else
            {
                parentClause = new WhereClause(this.GetWhereClauseRelationship());
                querySubClause = parentClause;
            }

            IList<DBQueryObject> queryObjects = this.ProcessChildDBQueryObjects(querySubClause, builder, csentry);

            if (queryObjects.Count == 0)
            {
                throw new QueryValueNullException("The query group contained no valid queries to execute");
            }
            else
            {
                return parentClause;
            }
        }

        /// <summary>
        /// Creates the WHERE clause for this query
        /// </summary>
        /// <param name="builder">The builder used by this query</param>
        /// <returns>A WhereClause object containing the terms of the query</returns>
        public WhereClause CreateWhereClause(DBQueryBuilder builder)
        {
            if (this.DBQueries.Count == 0)
            {
                throw new InvalidOperationException("The query group cannot generate a query when no child queries are specified");
            }

            WhereClause parentClause;
            WhereClause querySubClause;

            if (this.Operator == GroupOperator.None)
            {
                // Workaround to stop SqlOm from dropping the operator when there is only a single term in the where clause
                parentClause = new WhereClause(WhereClauseRelationship.Not);
                parentClause.Terms.Add(WhereTerm.CreateCompare(SqlExpression.Number(1), SqlExpression.Number(1), CompareOperator.Equal));
                querySubClause = new WhereClause(WhereClauseRelationship.Or);
                parentClause.SubClauses.Add(querySubClause);
            }
            else
            {
                parentClause = new WhereClause(this.GetWhereClauseRelationship());
                querySubClause = parentClause;
            }

            IList<DBQueryObject> queryObjects = this.ProcessChildDBQueryObjects(querySubClause, builder);

            if (queryObjects.Count == 0)
            {
                throw new QueryValueNullException("The query group contained no valid queries to execute");
            }
            else
            {
                return parentClause;
            }
        }

        /// <summary>
        /// Process the child query objects in this group
        /// </summary>
        /// <param name="parentClause">The parent clause for this query</param>
        /// <param name="builder">The builder for this query</param>
        /// <param name="csentry">The CSEntryChange used as the source of query values</param>
        /// <returns>A list of successfully processed query objects</returns>
        private IList<DBQueryObject> ProcessChildDBQueryObjects(WhereClause parentClause, DBQueryBuilder builder, CSEntryChange csentry)
        {
            IList<DBQueryObject> objectsToProcess = this.DBQueries;
            IList<DBQueryObject> processedObjects = new List<DBQueryObject>();

            foreach (DBQueryObject match in objectsToProcess)
            {
                WhereClause clause = null;

                try
                {
                    clause = this.GetWhereClauseFromDBQueryObject(builder, match, csentry);
                }
                catch (QueryValueNullException)
                {
                    if (this.Operator != GroupOperator.Any)
                    {
                        throw;
                    }
                }

                if (clause == null)
                {
                    continue;
                }

                processedObjects.Add(match);
                parentClause.SubClauses.Add(clause);
            }

            return processedObjects;
        }

        /// <summary>
        /// Process the child query objects in this group
        /// </summary>
        /// <param name="parentClause">The parent clause for this query</param>
        /// <param name="builder">The builder for this query</param>
        /// <param name="hologram">The MAObjectHologram used as the source of query values</param>
        /// <returns>A list of successfully processed query objects</returns>
        private IList<DBQueryObject> ProcessChildDBQueryObjects(WhereClause parentClause, DBQueryBuilder builder, MAObjectHologram hologram)
        {
            IList<DBQueryObject> objectsToProcess = this.DBQueries;
            IList<DBQueryObject> processedObjects = new List<DBQueryObject>();

            foreach (DBQueryObject obj in objectsToProcess)
            {
                WhereClause clause = null;

                try
                {
                    clause = this.GetWhereClauseFromDBQueryObject(builder, obj, hologram);
                }
                catch (QueryValueNullException)
                {
                    if (this.Operator != GroupOperator.Any)
                    {
                        throw;
                    }
                }

                if (clause == null)
                {
                    continue;
                }

                processedObjects.Add(obj);
                parentClause.SubClauses.Add(clause);
            }

            return processedObjects;
        }

        /// <summary>
        /// Process the child query objects in this group
        /// </summary>
        /// <param name="parentClause">The parent clause for this query</param>
        /// <param name="builder">The builder for this query</param>
        /// <returns>A list of successfully processed query objects</returns>
        private IList<DBQueryObject> ProcessChildDBQueryObjects(WhereClause parentClause, DBQueryBuilder builder)
        {
            IList<DBQueryObject> objectsToProcess = this.DBQueries;
            IList<DBQueryObject> processedObjects = new List<DBQueryObject>();

            foreach (DBQueryObject obj in objectsToProcess)
            {
                WhereClause clause = null;

                try
                {
                    clause = this.GetWhereClauseFromDBQueryObject(builder, obj);
                }
                catch (QueryValueNullException)
                {
                    if (this.Operator != GroupOperator.Any)
                    {
                        throw;
                    }
                }

                if (clause == null)
                {
                    continue;
                }

                processedObjects.Add(obj);
                parentClause.SubClauses.Add(clause);
            }

            return processedObjects;
        }
        
        /// <summary>
        /// Gets a WHERE clause from a child query
        /// </summary>
        /// <param name="builder">The builder for this query</param>
        /// <param name="queryObject">The query object to get the WHERE clause from</param>
        /// <param name="hologram">The MAObjectHologram used as the source of query values</param>
        /// <returns>A WhereClause object containing the terms of the child query</returns>
        private WhereClause GetWhereClauseFromDBQueryObject(DBQueryBuilder builder, DBQueryObject queryObject, MAObjectHologram hologram)
        {
            if (queryObject is DBQueryByValue)
            {
                return ((DBQueryByValue)queryObject).CreateWhereClause(builder, hologram);
            }
            else if (queryObject is DBQueryGroup)
            {
                return ((DBQueryGroup)queryObject).CreateWhereClause(builder, hologram);
            }
            else
            {
                throw new InvalidOperationException("The DBQueryObject type is unknown");
            }
        }

        /// <summary>
        /// Gets a WHERE clause from a child query
        /// </summary>
        /// <param name="builder">The builder for this query</param>
        /// <param name="queryObject">The query object to get the WHERE clause from</param>
        /// <param name="csentry">The CSEntryChange used as the source of query values</param>
        /// <returns>A WhereClause object containing the terms of the child query</returns>
        private WhereClause GetWhereClauseFromDBQueryObject(DBQueryBuilder builder, DBQueryObject queryObject, CSEntryChange csentry)
        {
            if (queryObject is DBQueryByValue)
            {
                return ((DBQueryByValue)queryObject).CreateWhereClause(builder, csentry);
            }
            else if (queryObject is DBQueryGroup)
            {
                return ((DBQueryGroup)queryObject).CreateWhereClause(builder, csentry);
            }
            else
            {
                throw new InvalidOperationException("The DBQueryObject type is unknown");
            }
        }

        /// <summary>
        /// Gets a WHERE clause from a child query
        /// </summary>
        /// <param name="builder">The builder for this query</param>
        /// <param name="queryObject">The query object to get the WHERE clause from</param>
        /// <returns>A WhereClause object containing the terms of the child query</returns>
        private WhereClause GetWhereClauseFromDBQueryObject(DBQueryBuilder builder, DBQueryObject queryObject)
        {
            if (queryObject is DBQueryByValue)
            {
                return ((DBQueryByValue)queryObject).CreateWhereClause(builder);
            }
            else if (queryObject is DBQueryGroup)
            {
                return ((DBQueryGroup)queryObject).CreateWhereClause(builder);
            }
            else
            {
                throw new InvalidOperationException("The DBQueryObject type is unknown");
            }
        }

        /// <summary>
        /// Gets the WhereClauseRelationship from the group operator
        /// </summary>
        /// <returns>A WhereClauseRelationship value</returns>
        private WhereClauseRelationship GetWhereClauseRelationship()
        {
            switch (this.Operator)
            {
                case GroupOperator.All:
                    return WhereClauseRelationship.And;

                case GroupOperator.Any:
                    return WhereClauseRelationship.Or;

                case GroupOperator.None:
                    return WhereClauseRelationship.Or;

                case GroupOperator.One:
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Initializes the object
        /// </summary>
        private void Initialize()
        {
            this.Operator = GroupOperator.Any;
            this.DBQueries = new ObservableCollection<DBQueryObject>();
        }

        /// <summary>
        /// Occurs just prior to the object being deserialized
        /// </summary>
        /// <param name="context">The serialization context</param>
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.Initialize();
        }
    }
}