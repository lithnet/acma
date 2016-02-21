// -----------------------------------------------------------------------
// <copyright file="ValueDeclaration.cs" company="Lithnet">
// Copyright (c) 2013
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Text.RegularExpressions;
    using Lithnet.Common.ObjectModel;
    using Lithnet.Transforms;
    using Microsoft.MetadirectoryServices;
    using Lithnet.Acma.DataModel;
    using Lithnet.MetadirectoryServices;
    using System.Linq;

    /// <summary>
    /// A string containing an attribute value, which may include zero or more attribute declarations
    /// </summary>
    [DataContract(Name = "value-declaration", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    public class ValueDeclaration : UINotifyPropertyChanges, IExtensibleDataObject
    {
        /// <summary>
        /// Matches a simple declaration
        /// </summary>
        private const string SimpleDeclarationRegex = @"^[\{\%][^\{\%]*?[\}\%]\r?$";

        /// <summary>
        /// The regular expression used to parse variable declarations from a string
        /// </summary>
        internal const string VariableDeclarationRegex = @"(?<!\\)\%.*?[^\\]\%";

        /// <summary>
        /// The internal object responsible for expanding the declaration
        /// </summary>
        private IValueDeclaration internalDeclaration;

        /// <summary>
        /// Initializes a new instance of the ValueDeclaration class
        /// </summary>
        public ValueDeclaration()
        {
            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the ValueDeclaration class
        /// </summary>
        /// <param name="declaration">The declaration string</param>
        /// <param name="transforms">The transforms to apply to this declaration</param>
        public ValueDeclaration(string declaration, string transforms)
            : this()
        {
            this.Declaration = declaration;
            this.TransformsString = transforms;
        }

        /// <summary>
        /// Initializes a new instance of the ValueDeclaration class
        /// </summary>
        /// <param name="declaration">The declaration string</param>
        public ValueDeclaration(string declaration)
            : this()
        {
            this.Declaration = declaration;
        }

        /// <summary>
        /// Gets or sets the raw declaration text
        /// </summary>
        [DataMember(Name = "value")]
        public string Declaration { get; set; }

        /// <summary>
        /// Gets the underlying type of the value declaration
        /// </summary>
        public Type InternalType
        {
            get
            {
                if (this.internalDeclaration == null)
                {
                    return null;
                }
                else
                {
                    return this.internalDeclaration.GetType();
                }
            }
        }

        /// <summary>
        /// Gets or sets the transformation string to apply
        /// </summary>
        [DataMember(Name = "transform-string")]
        public string TransformsString { get; set; }

        /// <summary>
        /// Gets or sets the serialization extension data object
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }

        public SchemaAttributeUsage GetAttributeUsage(string parentPath, AcmaSchemaAttribute attribute)
        {
            if (this.internalDeclaration == null)
            {
                return null;
            }
            else
            {
                return this.internalDeclaration.GetAttributeUsage(parentPath, attribute);
            }
        }

        /// <summary>
        /// Compares whether two ValueDeclarations are equal to each other
        /// </summary>
        /// <param name="a">The first object to compare</param>
        /// <param name="b">The second object to compare</param>
        /// <returns>Returns a value indicating if the two objects are equal</returns>
        public static bool operator ==(ValueDeclaration a, ValueDeclaration b)
        {
            // If both are null, or both are same instance, return true.
            if (object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            return a.Declaration == b.Declaration && a.TransformsString == b.TransformsString;
        }

        /// <summary>
        /// Compares whether two ValueDeclarations are not equal to each other
        /// </summary>
        /// <param name="a">The first object to compare</param>
        /// <param name="b">The second object to compare</param>
        /// <returns>Returns a value indicating if the two objects are not equal</returns>
        public static bool operator !=(ValueDeclaration a, ValueDeclaration b)
        {
            return !(a == b);
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
        /// Expands the value declaration
        /// </summary>
        /// <returns>Returns the expanded values</returns>
        public IList<object> Expand()
        {
            try
            {
                this.ParseDeclaration();
                IList<Transform> transforms = ValueDeclaration.ExtractTransforms(this.TransformsString);
                return Transform.ExecuteTransformChain(transforms, this.internalDeclaration.ExpandDeclaration());
            }
            catch (Exception ex)
            {
                throw new DeclarationExpansionException(this.Declaration, string.Empty, ex);
            }
        }

        /// <summary>
        /// Expands the value declaration
        /// </summary>
        /// <param name="hologram">The object to obtain the values used in the expansion</param>
        /// <returns>Returns the expanded values</returns>
        public IList<object> Expand(MAObjectHologram hologram)
        {
            try
            {
                this.ParseDeclaration();
                IList<Transform> transforms = ValueDeclaration.ExtractTransforms(this.TransformsString);

                return Transform.ExecuteTransformChain(transforms, this.internalDeclaration.ExpandDeclaration(hologram));
            }
            catch (Exception ex)
            {
                throw new DeclarationExpansionException(this.Declaration, hologram.Id.ToString(), ex);
            }
        }

        /// <summary>
        /// Expands the value declaration
        /// </summary>
        /// <param name="csentry">The object to obtain the values used in the expansion</param>
        /// <returns>Returns the expanded values</returns>
        public IList<object> Expand(CSEntryChange csentry)
        {
            try
            {
                this.ParseDeclaration();
                IList<Transform> transforms = ValueDeclaration.ExtractTransforms(this.TransformsString);

                return Transform.ExecuteTransformChain(transforms, this.internalDeclaration.ExpandDeclaration(csentry));
            }
            catch (Exception ex)
            {
                throw new DeclarationExpansionException(this.Declaration, csentry.DN, ex);
            }
        }

        /// <summary>
        /// Determines whether an object is equal to this object 
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>Returns a value indicating if the two objects are equal</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is ValueDeclaration)
            {
                ValueDeclaration other = (ValueDeclaration)obj;

                if (this.Declaration == other.Declaration && this.TransformsString == other.TransformsString)
                {
                    return true;
                }
            }

            return base.Equals(obj);
        }

        /// <summary>
        /// Gets the hash code for this object
        /// </summary>
        /// <returns>The computed has code for this object</returns>
        public override int GetHashCode()
        {
            // The hash code for the object cannot change over the life of the object
            // however, as the declaration can be updated, that value can change
            // resulting in incorrect equality evaluations.
            // A side effect of returning the same hash code is that when declarations are added to a collection
            // they will all end up in the same bucket, causing a performance overhead. However, given the relatively
            // small number of these objects that end up in lists, the impact of this is minimal
            return 0;
        }

        /// <summary>
        /// Extracts the list of transforms from the declaration text
        /// </summary>
        /// <param name="source">The declaration text</param>
        /// <returns>Returns a list of transforms extracted from the source string</returns>
        internal static IList<Transform> ExtractTransforms(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return new List<Transform>();
            }

            TransformParser t = new TransformParser(source);

            if (t.HasErrors)
            {
                throw new InvalidDeclarationStringException(
                    string.Format(
                        "The transform declaration contained one or more errors: {0}\n{1}",
                        source,
                        t.Errors.Select(u => u.ToString()).ToNewLineSeparatedString()));
            }

            return t.Transforms;
        }

        /// <summary>
        /// Validates the internal declaration
        /// </summary>
        private void ParseDeclaration()
        {
            if (this.internalDeclaration == null)
            {
                if (string.IsNullOrEmpty(this.Declaration))
                {
                    this.internalDeclaration = new ComplexValueDeclaration(string.Empty);
                }
                else
                {
                    if (Regex.IsMatch(this.Declaration, ValueDeclaration.SimpleDeclarationRegex))
                    {
                        this.internalDeclaration = new SimpleValueDeclaration(this.Declaration);
                    }
                    else
                    {
                        this.internalDeclaration = new ComplexValueDeclaration(this.Declaration);
                    }
                }
            }
        }

        /// <summary>
        /// Initializes the object
        /// </summary>
        private void Initialize()
        {
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
        }
    }
}