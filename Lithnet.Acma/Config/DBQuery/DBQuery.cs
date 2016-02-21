// -----------------------------------------------------------------------
// <copyright file="DBQuery.cs" company="Ryan Newington">
// Copyright (c) 2014 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Lithnet.Acma.DataModel;
    using Lithnet.MetadirectoryServices;
    using Microsoft.MetadirectoryServices;
    using Reeb.SqlOM;

    /// <summary> 
    /// Represents the base class for a dynamic database query
    /// </summary>
    [KnownType(typeof(DBQueryByValue))]
    [DataContract(Name = "dbquery-attribute", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    public abstract class DBQuery : DBQueryObject
    {
        /// <summary>
        /// Initializes a new instance of the DBQuery class
        /// </summary>
        protected DBQuery()
        {
        }

        /// <summary>
        /// Initializes a new instance of the DBQuery class
        /// </summary>
        /// <param name="searchAttribute">The attribute to search against</param>
        /// <param name="valueOperator">The value operator to apply to this query</param>
        protected DBQuery(AcmaSchemaAttribute searchAttribute, ValueOperator valueOperator)
        {
            this.SearchAttribute = searchAttribute;
            this.Operator = valueOperator;
            this.Validate();
        }

        /// <summary>
        /// Gets or sets the attribute in the database to search against
        /// </summary>
        [DataMember(Name = "search-attribute")]
        public AcmaSchemaAttribute SearchAttribute { get; set; }

        /// <summary>
        /// Gets or sets the operator to apply to this query
        /// </summary>
        [DataMember(Name = "operator")]
        public ValueOperator Operator { get; set; }

        /// <summary>
        /// Gets the CompareOperator to use in the WHERE clause
        /// </summary>
        private CompareOperator CompareOperator
        {
            get
            {
                switch (this.Operator)
                {
                    case ValueOperator.Equals:
                        return CompareOperator.Equal;

                    case ValueOperator.NotEquals:
                        return CompareOperator.NotEqual;

                    case ValueOperator.GreaterThan:
                        return CompareOperator.Greater;

                    case ValueOperator.LessThan:
                        return CompareOperator.Less;

                    case ValueOperator.GreaterThanOrEq:
                        return CompareOperator.GreaterOrEqual;

                    case ValueOperator.LessThanOrEq:
                        return CompareOperator.LessOrEqual;

                    case ValueOperator.EndsWith:
                    case ValueOperator.StartsWith:
                    case ValueOperator.Contains:
                        return CompareOperator.Like;

                    case ValueOperator.And:
                        return CompareOperator.BitwiseAnd;

                    default:
                    case ValueOperator.IsPresent:
                    case ValueOperator.NotPresent:
                    case ValueOperator.NotContains:
                    case ValueOperator.Or:
                    case ValueOperator.Smallest:
                    case ValueOperator.Largest:
                    case ValueOperator.None:
                        throw new ArgumentException("Unable to convert to CompareOperator: " + this.Operator.ToSmartString());
                }
            }
        }

        /// <summary>
        /// Creates parameters for each of the values used in the query
        /// </summary>
        /// <param name="builder">The DBQueryBuilder used by this query</param>
        /// <param name="values">The values to search for</param>
        /// <returns>A list of parameter names that were added to the query</returns>
        protected IList<string> CreateParameters(DBQueryBuilder builder, IList<object> values)
        {
            List<string> parameterNames = new List<string>();

            if (!(this.Operator == ValueOperator.IsPresent || this.Operator == ValueOperator.NotPresent))
            {
                foreach (object value in values)
                {
                    object parameterValue = value;

                    if (this.Operator == ValueOperator.Contains && parameterValue is string)
                    {
                        parameterValue = "%" + (string)parameterValue + "%";
                    }

                    if (this.Operator == ValueOperator.StartsWith && parameterValue is string)
                    {
                        parameterValue = (string)parameterValue + "%";
                    }

                    if (this.Operator == ValueOperator.EndsWith && parameterValue is string)
                    {
                        parameterValue = "%" + (string)parameterValue;
                    }

                    parameterNames.Add(builder.AddParameter(parameterValue));
                }
            }

            return parameterNames;
        }

        /// <summary>
        /// Creates a WHERE clause for this query
        /// </summary>
        /// <param name="builder">The builder for this query</param>
        /// <param name="parameterNames">The names of the parameters added to the query</param>
        /// <returns>The WhereClause object for this query</returns>
        protected WhereClause CreateWhereClause(DBQueryBuilder builder, IList<string> parameterNames)
        {
            if (this.SearchAttribute.IsInAVPTable)
            {
                return this.CreateWhereClauseforAVPTarget(builder, parameterNames);
            }
            else
            {
                return this.CreateWhereClauseforSVTarget(parameterNames);
            }
        }

        /// <summary>
        /// Creates a WHERE clause for a single-valued attribute
        /// </summary>
        /// <param name="parameterNames">The parameters to evaluate against</param>
        /// <returns>The WhereClause object for this query</returns>
        private WhereClause CreateWhereClauseforSVTarget(IList<string> parameterNames)
        {
            FromTerm fromTable;
            fromTable = this.SearchAttribute.DBTable;

            WhereClause clause = new WhereClause(WhereClauseRelationship.And);

            if (this.Operator == ValueOperator.IsPresent)
            {
                clause.Terms.Add(WhereTerm.CreateIsNotNull(SqlExpression.Field(this.SearchAttribute.ColumnName, fromTable)));
            }
            else if (this.Operator == ValueOperator.NotPresent)
            {
                clause.Terms.Add(WhereTerm.CreateIsNull(SqlExpression.Field(this.SearchAttribute.ColumnName, fromTable)));
            }
            else if (parameterNames.Count == 0)
            {
                throw new ArgumentNullException("parameterNames");
            }
            else if (parameterNames.Count > 1)
            {
                WhereClause subClause = new WhereClause(WhereClauseRelationship.Or);

                switch (this.Operator)
                {
                    case ValueOperator.And:
                    case ValueOperator.Contains:
                    case ValueOperator.StartsWith:
                    case ValueOperator.EndsWith:
                    case ValueOperator.GreaterThan:
                    case ValueOperator.GreaterThanOrEq:
                    case ValueOperator.LessThan:
                    case ValueOperator.LessThanOrEq:
                        foreach (string parameterName in parameterNames)
                        {
                            subClause.Terms.Add(WhereTerm.CreateCompare(SqlExpression.Field(this.SearchAttribute.ColumnName, fromTable), SqlExpression.Parameter(parameterName), this.CompareOperator));
                        }

                        break;

                    case ValueOperator.Equals:
                        subClause.Terms.Add(WhereTerm.CreateIn(SqlExpression.Field(this.SearchAttribute.ColumnName, fromTable), parameterNames.ToCommaSeparatedString()));
                        break;

                    case ValueOperator.NotEquals:
                        subClause.Terms.Add(WhereTerm.CreateNotIn(SqlExpression.Field(this.SearchAttribute.ColumnName, fromTable), parameterNames.ToCommaSeparatedString()));
                        break;

                    default:
                        throw new InvalidOperationException();
                }

                clause.SubClauses.Add(subClause);
            }
            else
            {
                clause.Terms.Add(WhereTerm.CreateCompare(SqlExpression.Field(this.SearchAttribute.ColumnName, fromTable), SqlExpression.Parameter(parameterNames[0]), this.CompareOperator));
            }

            return clause;
        }

        /// <summary>
        /// Creates a WHERE clause for a multivalued attribute
        /// </summary>
        /// <param name="builder">The builder for this query</param>
        /// <param name="parameterNames">The parameters to evaluate against</param>
        /// <returns>The WhereClause object for this query</returns>
        private WhereClause CreateWhereClauseforAVPTarget(DBQueryBuilder builder, IList<string> parameterNames)
        {
            FromTerm fromTable;
            fromTable = builder.GetNextAVPTableReference(this.SearchAttribute.TableName);

            WhereClause clause = new WhereClause(WhereClauseRelationship.And);

            if (this.Operator != ValueOperator.NotPresent)
            {
                string paramName = builder.AddParameter(this.SearchAttribute.Name);
                WhereTerm attributeNameTerm = WhereTerm.CreateCompare(SqlExpression.Field("attributeName", fromTable), SqlExpression.Parameter(paramName), CompareOperator.Equal);
                clause.Terms.Add(attributeNameTerm);
            }

            if (this.Operator == ValueOperator.IsPresent)
            {
                clause.Terms.Add(WhereTerm.CreateIsNotNull(SqlExpression.Field(this.SearchAttribute.ColumnName, fromTable)));
            }
            else if (this.Operator == ValueOperator.NotPresent)
            {
                string sql = this.CreateSubSelectStatementForMVNotExists(builder);
                clause.Terms.Add(WhereTerm.CreateNotExists(sql));
            }
            else
            {
                WhereClause subClause = new WhereClause(WhereClauseRelationship.Or);

                switch (this.Operator)
                {
                    case ValueOperator.And:
                    case ValueOperator.Contains:
                    case ValueOperator.GreaterThan:
                    case ValueOperator.GreaterThanOrEq:
                    case ValueOperator.LessThan:
                    case ValueOperator.StartsWith:
                    case ValueOperator.EndsWith:
                    case ValueOperator.LessThanOrEq:
                        foreach (string parameterName in parameterNames)
                        {
                            subClause.Terms.Add(WhereTerm.CreateCompare(SqlExpression.Field(this.SearchAttribute.ColumnName, fromTable), SqlExpression.Parameter(parameterName), this.CompareOperator));
                        }

                        break;

                    case ValueOperator.Equals:
                        subClause.Terms.Add(WhereTerm.CreateIn(SqlExpression.Field(this.SearchAttribute.ColumnName, fromTable), parameterNames.ToCommaSeparatedString()));
                        break;

                    case ValueOperator.NotEquals:
                        string sql = this.CreateSubSelectStatementForMVNotEquals(builder, parameterNames);
                        clause.Terms.Add(WhereTerm.CreateNotExists(sql));
                        break;

                    default:
                        throw new InvalidOperationException();
                }

                clause.SubClauses.Add(subClause);
            }

            return clause;
        }

        /// <summary>
        /// Create a sub-select statement for a query condition where the operator specifies that the specified single-valued attribute value must not exist
        /// </summary>
        /// <param name="builder">The builder for this query</param>
        /// <returns>An SQL SELECT statement</returns>
        private string CreateSubSelectStatementForMVNotExists(DBQueryBuilder builder)
        {
            string paramName = builder.AddParameter(this.SearchAttribute.Name);
            WhereClause existWhere = new WhereClause(WhereClauseRelationship.And);
            AcmaSchemaAttribute objectIdAttribute = ActiveConfig.DB.GetAttribute("objectId");

            existWhere.Terms.Add(WhereTerm.CreateCompare(SqlExpression.Field("objectId", this.SearchAttribute.DBTable), SqlExpression.Field(objectIdAttribute.Name, objectIdAttribute.DBTable), Reeb.SqlOM.CompareOperator.Equal));
            existWhere.Terms.Add(WhereTerm.CreateCompare(SqlExpression.Field("attributeName", this.SearchAttribute.DBTable), SqlExpression.Parameter(paramName), Reeb.SqlOM.CompareOperator.Equal));
            SelectQuery subQuery = new SelectQuery();
            subQuery.Columns.Add(new SelectColumn("objectId"));
            subQuery.Top = 1;
            subQuery.FromClause.BaseTable = this.SearchAttribute.DBTable;
            subQuery.WherePhrase.SubClauses.Add(existWhere);
            Reeb.SqlOM.Render.SqlServerRenderer render = new Reeb.SqlOM.Render.SqlServerRenderer();
            string sql = render.RenderSelect(subQuery);
            return sql;
        }

        /// <summary>
        /// Create a sub-select statement for a query condition where the operator specifies that the specified multivalued attribute value must not exist
        /// </summary>
        /// <param name="builder">The builder for this query</param>
        /// <param name="parameterNames">The parameters to evaluate against</param>
        /// <returns>An SQL SELECT statement</returns>
        private string CreateSubSelectStatementForMVNotEquals(DBQueryBuilder builder, IList<string> parameterNames)
        {
            string paramNameAttribute = builder.AddParameter(this.SearchAttribute.Name);
            WhereClause subClause = new WhereClause(WhereClauseRelationship.Or);

            WhereClause existWhere = new WhereClause(WhereClauseRelationship.And);
            AcmaSchemaAttribute objectIdAttribute = ActiveConfig.DB.GetAttribute("objectId");

            existWhere.Terms.Add(WhereTerm.CreateCompare(SqlExpression.Field("objectId", this.SearchAttribute.DBTable), SqlExpression.Field(objectIdAttribute.Name, objectIdAttribute.DBTable), Reeb.SqlOM.CompareOperator.Equal));
            existWhere.Terms.Add(WhereTerm.CreateCompare(SqlExpression.Field("attributeName", this.SearchAttribute.DBTable), SqlExpression.Parameter(paramNameAttribute), Reeb.SqlOM.CompareOperator.Equal));
            foreach (string paramNameValue in parameterNames)
            {
                subClause.Terms.Add(WhereTerm.CreateCompare(SqlExpression.Field(this.SearchAttribute.ColumnName, this.SearchAttribute.DBTable), SqlExpression.Parameter(paramNameValue), Reeb.SqlOM.CompareOperator.Equal));
            }

            existWhere.SubClauses.Add(subClause);

            SelectQuery subQuery = new SelectQuery();
            subQuery.Columns.Add(new SelectColumn("objectId"));
            subQuery.Top = 1;
            subQuery.FromClause.BaseTable = this.SearchAttribute.DBTable;
            subQuery.WherePhrase.SubClauses.Add(existWhere);
            Reeb.SqlOM.Render.SqlServerRenderer render = new Reeb.SqlOM.Render.SqlServerRenderer();
            string sql = render.RenderSelect(subQuery);
            return sql;
        }

        /// <summary>
        /// Validates the parameters of this object
        /// </summary>
        private void Validate()
        {
            switch (this.Operator)
            {
                case ValueOperator.IsPresent:
                case ValueOperator.NotPresent:
                case ValueOperator.Equals:
                case ValueOperator.NotEquals:

                    break;

                case ValueOperator.GreaterThanOrEq:
                case ValueOperator.LessThanOrEq:
                case ValueOperator.LessThan:
                case ValueOperator.And:
                case ValueOperator.GreaterThan:
                    if (this.SearchAttribute.Type != ExtendedAttributeType.Integer && this.SearchAttribute.Type != ExtendedAttributeType.DateTime)
                    {
                        throw new NotSupportedException(string.Format("An attribute type of {0} cannot be used in a query with the query type of {1}", this.SearchAttribute.Type.ToSmartString(), this.Operator.ToSmartString()));
                    }

                    break;

                case ValueOperator.Contains:
                case ValueOperator.StartsWith:
                case ValueOperator.EndsWith:
                    if (this.SearchAttribute.Type != ExtendedAttributeType.String)
                    {
                        throw new NotSupportedException(string.Format("An attribute type of {0} cannot be used in a query with the query type of {1}", this.SearchAttribute.Type.ToSmartString(), this.Operator.ToSmartString()));
                    }

                    break;

                default:
                case ValueOperator.None:
                case ValueOperator.Largest:
                case ValueOperator.Smallest:
                case ValueOperator.Or:
                case ValueOperator.NotContains:
                    throw new ArgumentException("The value operator specified is invalid: " + this.Operator.ToSmartString());
            }
        }
    }
}
