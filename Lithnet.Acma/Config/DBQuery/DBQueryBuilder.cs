// -----------------------------------------------------------------------
// <copyright file="DBQueryBuilder.cs" company="Lithnet">
// Copyright (c) 2014
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.SqlClient;
    using System.Linq;
    using Lithnet.Acma.DataModel;
    using Microsoft.MetadirectoryServices;
    using Reeb.SqlOM;
    using Reeb.SqlOM.Render;

    /// <summary> 
    /// A classed used to construct dynamic database queries
    /// </summary>
    public class DBQueryBuilder
    {
        /// <summary>
        /// The parameters for this query
        /// </summary>
        private List<SqlParameter> parameters;

        /// <summary>
        /// The list of MA_Attribute tables used in the query
        /// </summary>
        private List<FromTerm> attributeTables;

        /// <summary>
        /// The current count of MA_Attribute table references
        /// </summary>
        private int currentAttributeTableNumber;

        /// <summary>
        /// The current count of parameters
        /// </summary>
        private int currentParameterNumber;

        /// <summary>
        /// Initializes a new instance of the DBQueryBuilder class
        /// </summary>
        /// <param name="queryGroup">The query group to build the query from</param>
        /// <param name="maximumResults">The maximum number of results to return</param>
        public DBQueryBuilder(DBQueryGroup queryGroup, OrderByTermCollection orderByTerms)
            : this()
        {
            WhereClause whereClause = queryGroup.CreateWhereClause(this);
            this.OrderByTerms = orderByTerms;
            this.QueryString = this.BuildQuery(whereClause, 0);
        }

        /// <summary>
        /// Initializes a new instance of the DBQueryBuilder class
        /// </summary>
        /// <param name="queryGroup">The query group to build the query from</param>
        /// <param name="maximumResults">The maximum number of results to return</param>
        public DBQueryBuilder(DBQueryGroup queryGroup, int maximumResults)
            : this()
        {
            WhereClause whereClause = queryGroup.CreateWhereClause(this);
            this.QueryString = this.BuildQuery(whereClause, maximumResults);
        }

        /// <summary>
        /// Initializes a new instance of the DBQueryBuilder class
        /// </summary>
        /// <param name="queryGroup">The query group to build the query from</param>
        /// <param name="maximumResults">The maximum number of results to return</param>
        /// <param name="csentry">The object used as the source of the query values</param>
        public DBQueryBuilder(DBQueryGroup queryGroup, int maximumResults, CSEntryChange csentry)
            : this()
        {
            this.SourceObjectId = new Guid(csentry.DN);
            WhereClause whereClause = queryGroup.CreateWhereClause(this, csentry);
            this.QueryString = this.BuildQuery(whereClause, maximumResults);
        }

        /// <summary>
        /// Initializes a new instance of the DBQueryBuilder class
        /// </summary>
        /// <param name="queryGroup">The query group to build the query from</param>
        /// <param name="maximumResults">The maximum number of results to return</param>
        /// <param name="hologram">The object used as the source of the query values</param>
        public DBQueryBuilder(DBQueryGroup queryGroup, int maximumResults, MAObjectHologram hologram)
            : this()
        {
            this.SourceObjectId = hologram.Id;
            WhereClause whereClause = queryGroup.CreateWhereClause(this, hologram);
            this.QueryString = this.BuildQuery(whereClause, maximumResults);
        }

        /// <summary>
        /// Prevents a default instance of the DBQueryBuilder class from being created
        /// </summary>
        private DBQueryBuilder()
        {
            this.parameters = new List<SqlParameter>();
            this.attributeTables = new List<FromTerm>();
        }

        /// <summary>
        /// Gets a collection of SqlParameters used in this query
        /// </summary>
        public ReadOnlyCollection<SqlParameter> Parameters
        {
            get
            {
                return this.parameters.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets the query string built by this object
        /// </summary>
        public string QueryString { get; private set; }

        /// <summary>
        /// Gets or sets the ID of the object generating this query
        /// </summary>
        private Guid SourceObjectId { get; set; }
        
        /// <summary>
        /// Adds a new query parameter
        /// </summary>
        /// <param name="value">The value of the parameter</param>
        /// <returns>The name allocated to the parameter</returns>
        public string AddParameter(object value)
        {
            string name = this.GetNextParameter();
            this.parameters.Add(new SqlParameter(name, value));
            return name;
        }

        /// <summary>
        /// Generates a unique reference to an attribute-value pair table
        /// </summary>
        /// <param name="tableName">The name of the attribute-value pair table</param>
        /// <returns>A new instance of the AVP table FromTerm</returns>
        public FromTerm GetNextAVPTableReference(string tableName)
        {
            FromTerm term = FromTerm.Table(tableName, "tb" + ++this.currentAttributeTableNumber, "[dbo]");
            this.attributeTables.Add(term);
            return term;
        }

        /// <summary>
        /// Gets the next parameter name in the sequence
        /// </summary>
        /// <returns>The name of the next parameter</returns>
        private string GetNextParameter()
        {
            return "@p" + ++this.currentParameterNumber;
        }

        public OrderByTermCollection OrderByTerms { get; private set; }

        /// <summary>
        /// Builds the query
        /// </summary>
        /// <param name="whereClause">The WHERE conditions</param>
        /// <param name="maxResults">The number of results to return</param>
        /// <returns>The constructed SQL query</returns>
        private string BuildQuery(WhereClause whereClause, int maxResults)
        {
            SelectQuery query = new SelectQuery();
            AcmaSchemaAttribute objectIdAttribute = ActiveConfig.DB.GetAttribute("objectId");
            FromTerm objectBaseTable = objectIdAttribute.DBTable;

            query.Columns.Add(new SelectColumn("*", objectBaseTable));
            query.Distinct = true;
            query.FromClause.BaseTable = objectBaseTable;

            foreach (FromTerm term in this.attributeTables)
            {
                query.FromClause.Join(JoinType.Left, objectBaseTable, term, new JoinCondition("objectId", "objectId"));
            }

            if (maxResults > 0)
            {
                query.Top = maxResults;
            }

            if (this.parameters.Count() == 0)
            {
                return null;
            }

            // Prevents returning the querying object as a search result
            if (Guid.Empty != this.SourceObjectId)
            {
                string param = this.AddParameter(this.SourceObjectId);
                query.WherePhrase.Terms.Add(WhereTerm.CreateCompare(SqlExpression.Field(objectIdAttribute.Name, objectIdAttribute.DBTable), SqlExpression.Parameter(param), CompareOperator.NotEqual));
            }

            if (this.OrderByTerms != null)
            {
                query.OrderByTerms.AddRange(this.OrderByTerms);
            }

            query.WherePhrase.SubClauses.Add(whereClause);

            SqlServerRenderer renderer = new SqlServerRenderer();
            return renderer.RenderSelect(query);
        }
    }
}
