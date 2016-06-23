// -----------------------------------------------------------------------
// <copyright file="ComplexValueDeclaration.cs" company="Ryan Newington">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Lithnet.MetadirectoryServices;
    using Microsoft.MetadirectoryServices;
    using Lithnet.Acma.DataModel;

    /// <summary>
    /// A value declaration that combines multiple attributes declarations or text
    /// </summary>
    public class ComplexValueDeclaration : IValueDeclaration
    {
        private bool parsedDeclaration;

        /// <summary>
        /// Initializes a new instance of the ComplexValueDeclaration class
        /// </summary>
        /// <param name="declaration">The raw declaration string</param>
        public ComplexValueDeclaration(string declaration) 
        {
            this.AttributeDeclarations = new List<AttributeDeclaration>();
            this.VariableDeclarations = new List<VariableDeclaration>();
            this.Declaration = declaration;
        }

        /// <summary>
        /// Gets or sets the raw declaration string
        /// </summary>
        public string Declaration { get; set; }

        /// <summary>
        /// Gets the data type that will be returned by the declaration
        /// </summary>
        public ExtendedAttributeType DataType
        {
            get
            {
                return ExtendedAttributeType.String;
            }
        }

        /// <summary>
        /// Gets or sets the list of attributes referenced in this object
        /// </summary>
        private IList<AttributeDeclaration> AttributeDeclarations { get; set; }

        /// <summary>
        /// Gets or sets the list of variables referenced in this object
        /// </summary>
        private IList<VariableDeclaration> VariableDeclarations { get; set; }

        /// <summary>
        /// Expands the declaration
        /// </summary>
        /// <param name="hologram">The hologram containing the values to use in the expansion operation</param>
        /// <returns>A list of expanded attribute values</returns>
        public IList<object> ExpandDeclaration(MAObjectHologram hologram)
        {
            return new List<object>() { this.ExpandComplexDeclaration(hologram) };
        }

        /// <summary>
        /// Expands the declaration
        /// </summary>
        /// <param name="csentry">The CSEntryChange containing the values to use in the expansion operation</param>
        /// <returns>A list of expanded attribute values</returns>
        public IList<object> ExpandDeclaration(CSEntryChange csentry)
        {
            return new List<object>() { this.ExpandComplexDeclaration(csentry) };
        }

        /// <summary>
        /// Expands the declaration
        /// </summary>
        /// <returns>A list of expanded attribute values</returns>
        public IList<object> ExpandDeclaration()
        {
            return new List<object>() { this.ExpandComplexDeclaration() };
        }


        /// <summary>
        /// Expands the attributes referenced in the declaration text
        /// </summary>
        /// <param name="hologram">The object containing the values to use in the expansion</param>
        /// <returns>The target attribute value with any references expanded out to their values</returns>
        protected string ExpandComplexDeclaration(MAObjectHologram hologram)
        {
            this.ParseDeclaration();

            string constructedString = this.RemoveEscapedCharacters(this.Declaration);

            foreach (AttributeDeclaration declaration in this.AttributeDeclarations)
            {
                if (hologram == null)
                {
                    throw new ArgumentNullException("hologram");
                }

                IList<object> values = declaration.Expand(hologram);

                if (values.Count == 0)
                {
                    constructedString = constructedString.Replace(declaration.Declaration, string.Empty);
                }
                else if (values.Count > 1)
                {
                    throw new TooManyValuesException("The declaration string returned more than one attribute value which is unsupported for a complex value declaration", null);
                }
                else
                {
                    constructedString = constructedString.Replace(declaration.Declaration, values[0].ToSmartStringOrNull());
                }
            }

            constructedString = this.ExpandVariables(constructedString as string);

            return constructedString;
        }

        /// <summary>
        /// Expands the attributes referenced in the declaration text
        /// </summary>
        /// <param name="csentry">The object containing the values to use in the expansion</param>
        /// <returns>The target attribute value with any references expanded out to their values</returns>
        protected string ExpandComplexDeclaration(CSEntryChange csentry)
        {
            this.ParseDeclaration();

            string constructedString = this.RemoveEscapedCharacters(this.Declaration);

            foreach (AttributeDeclaration declaration in this.AttributeDeclarations)
            {
                if (csentry == null)
                {
                    throw new ArgumentNullException("csentry");
                }

                IList<object> values = declaration.Expand(csentry);

                if (values.Count == 0)
                {
                    constructedString = constructedString.Replace(declaration.Declaration, string.Empty);
                }
                else if (values.Count > 1)
                {
                    throw new TooManyValuesException("The declaration string returned more than one attribute value which is unsupported for a complex value declaration", null);
                }
                else
                {
                    constructedString = constructedString.Replace(declaration.Declaration, values[0].ToSmartStringOrNull());
                }
            }

            constructedString = this.ExpandVariables(constructedString as string);

            return constructedString;
        }

        /// <summary>
        /// Expands the attributes referenced in the declaration text
        /// </summary>
        /// <returns>The target attribute value with any references expanded out to their values</returns>
        protected string ExpandComplexDeclaration()
        {
            this.ParseDeclaration();

            string constructedString = this.RemoveEscapedCharacters(this.Declaration);

            if (this.AttributeDeclarations.Any())
            {
                throw new NotSupportedException("The declaration contained a reference to an attribute, which requires an object to provide the value for. The declaration was processed in a context where no object was present");
            }

            constructedString = this.ExpandVariables(constructedString as string);

            return constructedString;
        }

        /// <summary>
        /// Updates the internal state when the declaration string changes
        /// </summary>
        internal void ParseDeclaration()
        {
            if (!this.parsedDeclaration)
            {
                ValueDeclarationParser p = new ValueDeclarationParser(this.Declaration);

                if (p.HasErrors)
                {
                    throw new InvalidDeclarationStringException(
                        string.Format(
                            "The declaration contained one or more errors: {0}\n{1}",
                            this.Declaration,
                            p.Errors.Select(u => u.ToString()).ToNewLineSeparatedString()));
                }

                this.AttributeDeclarations = p.AttributeDeclarations.ToList();

                if (this.AttributeDeclarations.Any(t => t.HasReferenceAttribute))
                {
                    throw new InvalidDeclarationStringException("A complex declaration cannot contain reference attributes");
                }

                this.VariableDeclarations = p.VariableDeclarations.ToList();

                if (this.VariableDeclarations.Any(t => t.IsUniqueAllocationVariable))
                {
                    throw new InvalidDeclarationStringException("The declaration string contains a unique allocation variable");
                }

                this.parsedDeclaration = true;
            }
        }

        /// <summary>
        /// Expands the variables referenced in the declaration text
        /// </summary>
        /// <param name="existingExpansion">The current string value under expansion</param>
        /// <returns>The target attribute value with any references expanded out to their values</returns>
        private string ExpandVariables(string existingExpansion)
        {
            string constructedString = existingExpansion;

            foreach (VariableDeclaration declaration in this.VariableDeclarations.Where(t => !t.IsUniqueAllocationVariable))
            {
                IList<object> values = declaration.Expand();

                if (values.Count == 0)
                {
                    constructedString = constructedString.Replace(declaration.Declaration, string.Empty);
                }
                else if (values.Count > 1)
                {
                    throw new TooManyValuesException("The declaration string returned more than one variable value which is unsupported for a complex value declaration", null);
                }
                else
                {
                    constructedString = constructedString.Replace(declaration.Declaration, values[0].ToSmartStringOrNull());
                }
            }

            return constructedString;
        }

        private string RemoveEscapedCharacters(string declaration)
        {
            return Regex.Replace(declaration, @"\\(.)", "$1");
        }

        public SchemaAttributeUsage GetAttributeUsage(string parentPath, AcmaSchemaAttribute attribute)
        {
            this.ParseDeclaration();

            if (this.AttributeDeclarations.Any(t => t.Attribute == attribute))
            {
                return new SchemaAttributeUsage(this, "Value declaration", null, parentPath, this.Declaration);
            }
            else
            {
                return null;
            }
        }
    }
}
