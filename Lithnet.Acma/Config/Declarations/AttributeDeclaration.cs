// -----------------------------------------------------------------------
// <copyright file="AttributeDeclaration.cs" company="Ryan Newington">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Lithnet.Acma.DataModel;
    using Lithnet.MetadirectoryServices;
    using Lithnet.Transforms;
    using Microsoft.MetadirectoryServices;

    /// <summary>
    /// Represents an attribute in a declaration string
    /// </summary>
    public class AttributeDeclaration : DeclarationComponent
    {
        /// <summary>
        /// The regular expression used to parse attribute declarations from a string
        /// </summary>
        //internal const string AttributeDeclarationRegex = @"(?<!\\)\[(?<preText>.*?)(?<!\\)(?<attribute>\{.*?[^\\]\})(?<postText>.*?)(?<!\\)\]|(?<!(?<!\\)\\)(?<attribute>\{.*?[^\\]\})";

        internal AttributeDeclaration(
            AcmaSchemaAttribute attribute,
            HologramView view,
            IList<Transform> transforms,
            string preText,
            string postText,
            IList<AcmaSchemaAttribute> evaluationPath,
            string declaration)
        {
            this.Attribute = attribute;
            this.View = view;
            this.Transforms = transforms;
            this.PreText = preText;
            this.PostText = postText;
            this.ObjectEvaluationPath = evaluationPath;
            this.Declaration = declaration;
        }

        /// <summary>
        /// Gets the raw attribute declaration 
        /// </summary>
        public string Declaration { get; private set; }

        /// <summary>
        /// Gets the attribute defined in the declaration
        /// </summary>
        public AcmaSchemaAttribute Attribute { get; private set; }

        /// <summary>
        /// Gets the hologram view specified in the declaration
        /// </summary>
        public HologramView View { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is a multivalued attribute at any stage in the evaluation chain
        /// </summary>
        public bool HasMultivaluedAttributeInEvaluationChain
        {
            get
            {
                return this.Attribute.IsMultivalued || this.ObjectEvaluationPath.Any(t => t.IsMultivalued);
            }
        }

        public bool HasReferenceAttribute
        {
            get
            {
                return this.Attribute.Type == ExtendedAttributeType.Reference;
            }
        }

        /// <summary>
        /// Gets a list of transforms to apply to this attribute declaration
        /// </summary>
        public IList<Transform> Transforms { get; private set; }

        /// <summary>
        /// Gets or sets the text that appears before the declaration enclosed in an optional declaration
        /// </summary>
        private string PreText { get; set; }

        /// <summary>
        /// Gets or sets the text that appears after the declaration enclosed in an optional declaration
        /// </summary>
        private string PostText { get; set; }

        /// <summary>
        /// Gets or sets the path of reference attributes to follow to the specified attribute
        /// </summary>
        private IList<AcmaSchemaAttribute> ObjectEvaluationPath { get; set; }

        /// <summary>
        /// Expands the attribute declaration to its value
        /// </summary>
        /// <param name="hologram">The MAObject containing the attribute to expand</param>
        /// <returns>The expanded value of the attribute, after any transforms have been applied</returns>
        public IList<object> Expand(MAObjectHologram hologram)
        {
            return this.ExpandAttribute(hologram);
        }

        /// <summary>
        /// Expands the attribute declaration to its value
        /// </summary>
        /// <param name="csentry">The CSEntryChange containing the attribute to expand</param>
        /// <returns>The expanded value of the attribute, after any transforms have been applied</returns>
        public IList<object> Expand(CSEntryChange csentry)
        {
            return this.ExpandAttribute(csentry);
        }

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns>A string that represents the current object</returns>
        public override string ToString()
        {
            return this.Declaration;
        }

        /// <summary>
        /// Expands the attributes referenced in the constructedValue
        /// </summary>
        /// <param name="hologram">The object to construct the value for</param>
        /// <returns>The constructedValue with any attributes expanded out to their values</returns>
        private IList<object> ExpandAttribute(MAObjectHologram hologram)
        {
            IEnumerable<MAObjectHologram> attributeSourceObjects = this.GetSourceObjects(hologram);

            if (attributeSourceObjects == null)
            {
                throw new ReferencedObjectNotPresentException();
            }

            List<object> values = new List<object>();

            foreach (MAObjectHologram sourceObject in attributeSourceObjects)
            {
                IList<object> sourceValues = this.GetAttributeValues(sourceObject);

                if (sourceValues.Count == 0)
                {
                    continue;
                }
                else
                {
                    sourceValues = Transform.ExecuteTransformChain(this.Transforms, sourceValues);

                    if (this.PreText != null || this.PostText != null)
                    {
                        IList<object> transformedValuesWithOptionalText = new List<object>();

                        foreach (object transformedValue in sourceValues)
                        {
                            if (transformedValue != null)
                            {
                                transformedValuesWithOptionalText.Add(this.PreText + transformedValue.ToSmartStringOrNull() + this.PostText);
                            }
                        }

                        sourceValues = transformedValuesWithOptionalText;
                    }

                    values.AddRange(sourceValues);
                }
            }

            return values;
        }

        /// <summary>
        /// Expands the attributes referenced in the constructedValue
        /// </summary>
        /// <param name="csentry">The object to construct the value for</param>
        /// <returns>The constructedValue with any attributes expanded out to their values</returns>
        private IList<object> ExpandAttribute(CSEntryChange csentry)
        {
            if (this.ObjectEvaluationPath.Count > 0)
            {
                throw new NotSupportedException("Cannot chase references from a CSEntryChange object");
            }

            List<object> values = new List<object>();

            AttributeChange attributechange = csentry.AttributeChanges.FirstOrDefault(t => t.Name == this.Attribute.Name);

            if (attributechange == null)
            {
                return values;
            }

            if (attributechange.ModificationType == AttributeModificationType.Delete)
            {
                throw new UnknownOrUnsupportedModificationTypeException("An AttributeChange with an unsupported modification was presented to the attribute declaration");
            }

            IList<object> attributeValues = attributechange.ValueChanges.Where(t => t.ModificationType == ValueModificationType.Add).Select(t => t.Value).ToList();

            if (attributeValues.Count == 0)
            {
                return values;
            }

            attributeValues = Transform.ExecuteTransformChain(this.Transforms, attributeValues);

            if (this.PreText != null || this.PostText != null)
            {
                IList<object> transformedValuesWithOptionalText = new List<object>();

                foreach (object transformedValue in attributeValues)
                {
                    if (transformedValue != null)
                    {
                        transformedValuesWithOptionalText.Add(this.PreText + transformedValue.ToSmartStringOrNull() + this.PostText);
                    }
                }

                attributeValues = transformedValuesWithOptionalText;
            }

            values.AddRange(attributeValues);

            return values;
        }

        /// <summary>
        /// Gets the value of the declared attribute from the specified hologram
        /// </summary>
        /// <param name="hologram">The hologram to extract the values from</param>
        /// <returns>The values expanded from the hologram</returns>
        private IList<object> GetAttributeValues(MAObjectHologram hologram)
        {
            List<object> values = new List<object>();

            AttributeValues attributeValues = hologram.GetAttributeValues(this.Attribute, this.View);

            //if (this.Attribute.Name == "mail" && this.View == HologramView.Current)
            //{
            //    System.Diagnostics.Debugger.Launch();
            //}

            if (!attributeValues.IsEmptyOrNull)
            {
                foreach (AttributeValue attributeValue in attributeValues)
                {
                    values.Add(attributeValue.Value);
                }
            }

            if (values.Count == 0 && this.Attribute.Type == ExtendedAttributeType.Boolean)
            {
                values.Add(false);
            }

            return values;
        }

        /// <summary>
        /// Gets the object containing the attribute value to use
        /// </summary>
        /// <param name="hologram">The object that is under construction</param>
        /// <returns>If the declaration contains a reference to another object, then that object is returned, otherwise the original object is returned</returns>
        private IEnumerable<MAObjectHologram> GetSourceObjects(MAObjectHologram hologram)
        {
            if (this.ObjectEvaluationPath.Count == 0)
            {
                return new List<MAObjectHologram>() { hologram };
            }
            else
            {
                IEnumerable<MAObjectHologram> foundObjects = new List<MAObjectHologram>() { hologram };

                foreach (AcmaSchemaAttribute pathItem in this.ObjectEvaluationPath)
                {
                    List<MAObjectHologram> childObjects = new List<MAObjectHologram>();

                    foreach (var foundObject in foundObjects)
                    {
                        childObjects.AddRange(foundObject.GetReferencedObjects(pathItem));
                    }

                    foundObjects = childObjects;
                }

                return foundObjects;
            }
        }

    }
}