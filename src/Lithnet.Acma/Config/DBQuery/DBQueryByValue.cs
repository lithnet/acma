// -----------------------------------------------------------------------
// <copyright file="DBQueryByValue.cs" company="Lithnet">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using Lithnet.Acma.DataModel;
    using Lithnet.MetadirectoryServices;
    using Microsoft.MetadirectoryServices;
    using Reeb.SqlOM;

    /// <summary>
    /// Represents the definition of an attribute mapping to use during the object resurrection or join process
    /// </summary>
    [DataContract(Name = "dbquery-byvalue", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    public class DBQueryByValue : DBQuery
    {
        /// <summary>
        /// Initializes a new instance of the DBQueryByValue class
        /// </summary>
        public DBQueryByValue()
            : base()
        {
            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the DBQueryByValue class
        /// </summary>
        /// <param name="searchAttribute">The attribute to search on</param>
        /// <param name="valueOperator">The comparison operator to use when searching values</param>
        public DBQueryByValue(AcmaSchemaAttribute searchAttribute, ValueOperator valueOperator)
            : this()
        {
            this.Initialize();
            this.Operator = valueOperator;
            this.SearchAttribute = searchAttribute;
        }

        /// <summary>
        /// Initializes a new instance of the DBQueryByValue class
        /// </summary>
        /// <param name="searchAttribute">The attribute to search for in the database</param>
        /// <param name="valueOperator">The operator to apply to values in the search</param>
        /// <param name="values">The values to search for</param>
        public DBQueryByValue(AcmaSchemaAttribute searchAttribute, ValueOperator valueOperator, IList<object> values)
            : this(searchAttribute, valueOperator)
        {
            if (values != null)
            {
                foreach (var item in values)
                {
                    this.ValueDeclarations.Add(new ValueDeclaration(item.ToSmartStringOrNull()));
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the DBQueryByValue class
        /// </summary>
        /// <param name="searchAttribute">The attribute to search for in the database</param>
        /// <param name="valueOperator">The operator to apply to values in the search</param>
        /// <param name="value">The value to search for</param>
        public DBQueryByValue(AcmaSchemaAttribute searchAttribute, ValueOperator valueOperator, object value)
            : this(searchAttribute, valueOperator)
        {
            this.ValueDeclarations.Add(new ValueDeclaration(value.ToSmartStringOrNull()));
        }

        /// <summary>
        /// Initializes a new instance of the DBQueryByValue class
        /// </summary>
        /// <param name="searchAttribute">The attribute to search for in the database</param>
        /// <param name="valueOperator">The operator to apply to values in the search</param>
        /// <param name="attribute">The attribute value to use to search against the searchAttribute parameter</param>
        public DBQueryByValue(AcmaSchemaAttribute searchAttribute, ValueOperator valueOperator, AcmaSchemaAttribute attribute)
            : this(searchAttribute, valueOperator)
        {
            this.ValueDeclarations.Add(new ValueDeclaration(string.Format("{{{0}}}", attribute.Name)));
        }

        /// <summary>
        /// Initializes a new instance of the DBQueryByValue class
        /// </summary>
        /// <param name="searchAttribute">The attribute to search for in the database</param>
        /// <param name="valueOperator">The operator to apply to values in the search</param>
        /// <param name="declaration">The value to search for</param>
        public DBQueryByValue(AcmaSchemaAttribute searchAttribute, ValueOperator valueOperator, ValueDeclaration declaration)
            : this(searchAttribute, valueOperator)
        {
            this.ValueDeclarations.Add(declaration);
        }

        /// <summary>
        /// Initializes a new instance of the DBQueryByValue class
        /// </summary>
        /// <param name="searchAttribute">The attribute to search for in the database</param>
        /// <param name="valueOperator">The operator to apply to values in the search</param>
        /// <param name="declarations">The value declarations to search for</param>
        public DBQueryByValue(AcmaSchemaAttribute searchAttribute, ValueOperator valueOperator, IList<ValueDeclaration> declarations)
            : this(searchAttribute, valueOperator)
        {
            foreach (var declaration in declarations)
            {
                this.ValueDeclarations.Add(declaration);
            }
        }

        /// <summary>
        /// Gets or sets the values to search for
        /// </summary>
        [DataMember(Name = "value-declarations")]
        public IList<ValueDeclaration> ValueDeclarations { get; set; }

        /// <summary>
        /// Creates the WHERE clause for this query
        /// </summary>
        /// <param name="builder">The builder for this query</param>
        /// <param name="hologram">The hologram object to obtain the search values from</param>
        /// <returns>A WhereClause object representing the terms of this query</returns>
        public WhereClause CreateWhereClause(DBQueryBuilder builder, MAObjectHologram hologram)
        {
            if (this.Operator == ValueOperator.IsPresent || this.Operator == ValueOperator.NotPresent)
            {
                return this.CreateWhereClause(builder, new List<string>());
            }
            else
            {
                if (this.ValueDeclarations == null || this.ValueDeclarations.Count == 0 || this.ValueDeclarations.Any(t => t == null))
                {
                    throw new QueryValueNullException();
                }

                IList<object> values = this.GetValues(hologram);

                if (values == null || values.Count == 0)
                {
                    throw new QueryValueNullException();
                }

                IList<string> parameterNames = this.CreateParameters(builder, values);
                return this.CreateWhereClause(builder, parameterNames);
            }
        }

        /// <summary>
        /// Creates the WHERE clause for this query
        /// </summary>
        /// <param name="builder">The builder for this query</param>
        /// <param name="csentry">The CSEntryChange to obtain the search values from</param>
        /// <returns>A WhereClause object representing the terms of this query</returns>
        public WhereClause CreateWhereClause(DBQueryBuilder builder, CSEntryChange csentry)
        {
            if (this.Operator == ValueOperator.IsPresent || this.Operator == ValueOperator.NotPresent)
            {
                return this.CreateWhereClause(builder, new List<string>());
            }
            else
            {
                if (this.ValueDeclarations == null || this.ValueDeclarations.Count == 0 || this.ValueDeclarations.Any(t => t == null))
                {
                    throw new QueryValueNullException();
                }

                IList<object> values = this.GetValues(csentry);

                if (values == null || values.Count == 0)
                {
                    throw new QueryValueNullException();
                }

                IList<string> parameterNames = this.CreateParameters(builder, values);
                return this.CreateWhereClause(builder, parameterNames);
            }
        }

        /// <summary>
        /// Creates the WHERE clause for this query
        /// </summary>
        /// <param name="builder">The builder for this query</param>
        /// <returns>A WhereClause object representing the terms of this query</returns>
        public WhereClause CreateWhereClause(DBQueryBuilder builder)
        {
            if (this.Operator == ValueOperator.IsPresent || this.Operator == ValueOperator.NotPresent)
            {
                return this.CreateWhereClause(builder, new List<string>());
            }
            else
            {
                if (this.ValueDeclarations == null || this.ValueDeclarations.Count == 0 || this.ValueDeclarations.Any(t => t == null))
                {
                    throw new QueryValueNullException();
                }

                IList<object> values = this.GetValuesFromDelarations();

                if (values == null || values.Count == 0)
                {
                    throw new QueryValueNullException();
                }

                IList<string> parameterNames = this.CreateParameters(builder, values);
                return this.CreateWhereClause(builder, parameterNames);
            }
        }

        /// <summary>
        /// Validates a change to a property
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed</param>
        protected override void ValidatePropertyChange(string propertyName)
        {
            base.ValidatePropertyChange(propertyName);

            switch (propertyName)
            {
                case "SearchAttribute":
                    if (this.SearchAttribute == null)
                    {
                        this.AddError("SearchAttribute", "A search attribute must be provided");
                    }
                    else
                    {
                        this.RemoveError("SearchAttribute");

                        if (!ComparisonEngine.IsAllowedOperator(this.Operator, this.SearchAttribute.Type))
                        {
                            this.AddError("Operator", "The specified search operator is not valid for this data type");
                        }
                        else
                        {
                            this.RemoveError("Operator");
                        }
                    }

                    break;

                case "Operator":
                    if (this.SearchAttribute != null)
                    {
                        if (!ComparisonEngine.IsAllowedOperator(this.Operator, this.SearchAttribute.Type) && (!ComparisonEngine.IsAllowedPresenceOperator(this.Operator)))
                        {
                            this.AddError("Operator", "The specified search operator is not valid for this data type");
                        }
                        else
                        {
                            this.RemoveError("Operator");
                        }
                    }

                    break;

                default:
                    break;
            }
        }

        public override IEnumerable<SchemaAttributeUsage> GetAttributeUsage(string parentPath, AcmaSchemaAttribute attribute)
        {
            foreach (ValueDeclaration value in this.ValueDeclarations)
            {
                SchemaAttributeUsage usage = value.GetAttributeUsage(parentPath, attribute);

                if (usage != null)
                {
                    yield return usage;
                }
            }
        }

        /// <summary>
        /// Gets a list of values from the specified hologram
        /// </summary>
        /// <param name="hologram">The object from which to obtain the values</param>
        /// <returns>A list of attribute values obtained from the hologram</returns>
        private IList<object> GetValues(MAObjectHologram hologram)
        {
            List<object> values = new List<object>();

            foreach (var declaration in this.ValueDeclarations)
            {
                if (string.IsNullOrEmpty(declaration.Declaration))
                {
                    throw new QueryValueNullException();
                }

                foreach (var value in declaration.Expand(hologram))
                {
                    values.Add(TypeConverter.ConvertData(value, this.SearchAttribute.Type));
                }
            }

            return values;
        }

        /// <summary>
        /// Gets a list of values from the specified CSEntryChange
        /// </summary>
        /// <param name="csentry">The object from which to obtain the values</param>
        /// <returns>A list of values obtained from the object</returns>
        private IList<object> GetValues(CSEntryChange csentry)
        {
            List<object> values = new List<object>();

            foreach (var declaration in this.ValueDeclarations)
            {
                if (string.IsNullOrEmpty(declaration.Declaration))
                {
                    throw new QueryValueNullException();
                }

                foreach (var value in declaration.Expand(csentry))
                {
                    values.Add(TypeConverter.ConvertData(value, this.SearchAttribute.Type));
                }
            }

            return values;
        }

        /// <summary>
        /// Expands the value declarations and returns the values specified
        /// </summary>
        /// <returns>The values expanded from the list of value declarations associated with the query object</returns>
        private IList<object> GetValuesFromDelarations()
        {
            List<object> values = new List<object>();

            foreach (var declaration in this.ValueDeclarations)
            {
                if (string.IsNullOrEmpty(declaration.Declaration))
                {
                    throw new QueryValueNullException();
                }

                foreach (var value in declaration.Expand())
                {
                    values.Add(TypeConverter.ConvertData(value, this.SearchAttribute.Type));
                }
            }

            return values;
        }

        /// <summary>
        /// Initializes the object
        /// </summary>
        private void Initialize()
        {
            this.ValueDeclarations = new List<ValueDeclaration>();
            this.Operator = ValueOperator.Equals;
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

        /// <summary>
        /// Occurs after the object has been deserialized
        /// </summary>
        /// <param name="context">The serialization context</param>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            this.ValidatePropertyChange("SearchAttribute");
            this.ValidatePropertyChange("Operator");
        }
    }
}
