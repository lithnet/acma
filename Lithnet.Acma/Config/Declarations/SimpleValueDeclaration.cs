// -----------------------------------------------------------------------
// <copyright file="SimpleValueDeclaration.cs" company="Lithnet">
// Copyright (c) 2013
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Lithnet.MetadirectoryServices;
    using Microsoft.MetadirectoryServices;
    using System.Linq;
    using Lithnet.Acma.DataModel;

    /// <summary>
    /// Defines a value declaration that contains only a single attribute or variable
    /// </summary>
    public class SimpleValueDeclaration : IValueDeclaration
    {
        private bool parsedDeclaration;

        /// <summary>
        /// Initializes a new instance of the SimpleValueDeclaration class
        /// </summary>
        /// <param name="declaration">The raw declaration string</param>
        public SimpleValueDeclaration(string declaration)
        {
            this.Declaration = declaration;
        }

        /// <summary>
        /// Gets or sets the raw declaration text
        /// </summary>
        public string Declaration { get; set; }

        /// <summary>
        /// Gets or sets the declaration component used in this value declaration
        /// </summary>
        public DeclarationComponent DeclarationComponent { get; set; }

        /// <summary>
        /// Expands the declaration
        /// </summary>
        /// <returns>A list of expanded values</returns>
        public IList<object> ExpandDeclaration()
        {
            return this.ExpandSimpleDeclaration();
        }

        /// <summary>
        /// Expands the declaration
        /// </summary>
        /// <param name="hologram">The object containing the values to use in the expansion</param>
        /// <returns>A list of expanded values</returns>
        public IList<object> ExpandDeclaration(MAObjectHologram hologram)
        {
            return this.ExpandSimpleDeclaration(hologram);
        }

        /// <summary>
        /// Expands the declaration
        /// </summary>
        /// <param name="csentry">The object containing the values to use in the expansion</param>
        /// <returns>A list of expanded values</returns>
        public IList<object> ExpandDeclaration(CSEntryChange csentry)
        {
            return this.ExpandSimpleDeclaration(csentry);
        }

        public SchemaAttributeUsage GetAttributeUsage(string parentPath, AcmaSchemaAttribute attribute)
        {
            AttributeDeclaration attributeDeclaration = this.DeclarationComponent as AttributeDeclaration;

            if (attributeDeclaration != null)
            {
                if (attributeDeclaration.Attribute == attribute)
                {
                    return new SchemaAttributeUsage(this, "Value declaration", null, parentPath, this.Declaration);
                }
            }

            return null;
        }

        /// <summary>
        /// Occurs when the declaration string is updated
        /// </summary>
        private void ParseDeclaration()
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

                IList<AttributeDeclaration> attributeDeclarations = p.AttributeDeclarations.ToList();

                IList<VariableDeclaration> variableDeclarations = p.VariableDeclarations.ToList();

                int totalDeclarations = attributeDeclarations.Count + variableDeclarations.Count ;
                
                if (totalDeclarations > 1)
                {
                    throw new InvalidDeclarationStringException("The declaration string contained too many values for simple declaration");
                }
                else if (totalDeclarations == 0)
                {
                    throw new InvalidDeclarationStringException("The declaration string did not contain any attribute or variable declarations");
                }

                if (attributeDeclarations.Count == 1)
                {
                    this.DeclarationComponent = attributeDeclarations.First();
                }
                else if (variableDeclarations.Count == 1)
                {
                    VariableDeclaration variableDeclaration = variableDeclarations.First();

                    if (variableDeclaration.IsUniqueAllocationVariable)
                    {
                        throw new InvalidDeclarationStringException("A declaration containing only a unique allocator is not supported");
                    }

                    this.DeclarationComponent = variableDeclaration;
                }
                else
                {
                    throw new InvalidOperationException();
                }

                this.parsedDeclaration = true;
            }
        }

        /// <summary>
        /// Expands the attribute or variable referenced in a simple declaration
        /// </summary>
        /// <param name="hologram">The object containing the values to use in the expansion</param>
        /// <returns>The target attribute value with any references expanded out to their values</returns>
        private IList<object> ExpandSimpleDeclaration(MAObjectHologram hologram)
        {
            this.ParseDeclaration();

            if (this.DeclarationComponent is AttributeDeclaration)
            {
                if (hologram == null)
                {
                    throw new ArgumentNullException("hologram");
                }

                return ((AttributeDeclaration)this.DeclarationComponent).Expand(hologram);
            }
            else
            {
                return ((VariableDeclaration)this.DeclarationComponent).Expand();
            }
        }

        /// <summary>
        /// Expands the attribute or variable referenced in a simple declaration
        /// </summary>
        /// <param name="csentry">The object containing the values to use in the expansion</param>
        /// <returns>The target attribute value with any references expanded out to their values</returns>
        private IList<object> ExpandSimpleDeclaration(CSEntryChange csentry)
        {
            this.ParseDeclaration();

            if (this.DeclarationComponent is AttributeDeclaration)
            {
                if (csentry == null)
                {
                    throw new ArgumentNullException("csentry");
                }

                return ((AttributeDeclaration)this.DeclarationComponent).Expand(csentry);
            }
            else
            {
                return ((VariableDeclaration)this.DeclarationComponent).Expand();
            }
        }

        /// <summary>
        /// Expands the attribute or variable referenced in a simple declaration
        /// </summary>
        /// <returns>The target attribute value with any references expanded out to their values</returns>
        private IList<object> ExpandSimpleDeclaration()
        {
            this.ParseDeclaration();

            if (this.DeclarationComponent is AttributeDeclaration)
            {
                throw new NotSupportedException("declaration cannot be expanded without providing a hologram or CSEntryChange");
            }
            else
            {
                return ((VariableDeclaration)this.DeclarationComponent).Expand();
            }
        }
    }
}
