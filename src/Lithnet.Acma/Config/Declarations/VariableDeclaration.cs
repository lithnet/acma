// -----------------------------------------------------------------------
// <copyright file="VariableDeclaration.cs" company="Lithnet">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Lithnet.MetadirectoryServices;
    using Lithnet.Transforms;

    /// <summary>
    /// Contains the declaration of a variable within a DeclarationString
    /// </summary>
    public class VariableDeclaration : DeclarationComponent
    {
        protected internal const string VariableNameRandomLowerCaseString = "randstringalphalcase";
        protected internal const string VariableNameRandomAlphaNumericString = "randstring";
        protected internal const string VariableNameRandomNumber = "randnum";
        protected internal const string VariableNameNewGuid = "newguid";
        protected internal const string VariableNameDateUtc = "utcdate";
        protected internal const string VariableNameDateLocal = "date";
        protected internal const string VariableNameUniquePlaceholderNumeric = "n";
        protected internal const string VariableNameUniquePlaceholderNumericOptional = "o";

        internal VariableDeclaration(string variableName, string variableParameters, IList<Transform> transforms, string declaration)
        {
            this.StartingValue = 0;
            this.VariableName = variableName;
            this.VariableParameters = variableParameters;
            this.Transforms = transforms;
            this.Declaration = declaration;

            if (this.VariableName == VariableDeclaration.VariableNameUniquePlaceholderNumericOptional ||
                this.VariableName == VariableDeclaration.VariableNameUniquePlaceholderNumeric)
            {
                this.IsUniqueAllocationVariable = true;
            }
        }

        /// <summary>
        /// Gets the raw attribute declaration 
        /// </summary>
        public string Declaration { get; private set; }

        /// <summary>
        /// Gets the name of the variable
        /// </summary>
        public string VariableName { get; private set; }

        /// <summary>
        /// Gets the parameters that apply to the variable
        /// </summary>
        public string VariableParameters { get; private set; }

        /// <summary>
        /// Gets a list of transforms to be applied to this variable after expansion
        /// </summary>
        public IList<Transform> Transforms { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this variable can be used to unique allocation
        /// </summary>
        public bool IsUniqueAllocationVariable { get; private set; }

        /// <summary>
        /// Gets or sets the starting value of the unique allocation counter
        /// </summary>
        protected int StartingValue { get; set; }

        /// <summary>
        /// Expands the variable
        /// </summary>
        /// <returns>The expanded value of the variable, after any transforms have been applied</returns>
        public IList<object> Expand()
        {
            return this.ApplyTransforms(this.GetVariableValue());
        }

        /// <summary>
        /// Resets the internal counter used for unique allocation
        /// </summary>
        public void ResetCounter()
        {
            this.StartingValue = 0;
        }

        /// <summary>
        /// Gets the variable value
        /// </summary>
        /// <returns>The raw expanded value of the variable</returns>
        private object GetVariableValue()
        {
            switch (this.VariableName)
            {
                case VariableDeclaration.VariableNameDateLocal:
                    return DateTime.Now.Truncate(TimeSpan.TicksPerSecond);

                case VariableDeclaration.VariableNameDateUtc:
                    return DateTime.Now.ToUniversalTime().Truncate(TimeSpan.TicksPerSecond);

                case VariableDeclaration.VariableNameUniquePlaceholderNumericOptional:
                    int current = this.StartingValue++;

                    if (current > ActiveConfig.UniqueAllocationDepth)
                    {
                        throw new MaximumAllocationAttemptsExceededException("The maximum number of unique allocation attempts has been exceeded");
                    }

                    return current == 0 ? null : (object)current;

                case VariableDeclaration.VariableNameUniquePlaceholderNumeric:
                    if (++this.StartingValue > ActiveConfig.UniqueAllocationDepth)
                    {
                        throw new MaximumAllocationAttemptsExceededException("The maximum number of unique allocation attempts has been exceeded");
                    }

                    return this.StartingValue;

                case VariableDeclaration.VariableNameNewGuid:
                    return Guid.NewGuid();

                case VariableDeclaration.VariableNameRandomAlphaNumericString:
                    return this.GetRandomAlphaNumericString();

                case VariableDeclaration.VariableNameRandomLowerCaseString:
                    return this.GetRandomLowerCaseString();

                case VariableDeclaration.VariableNameRandomNumber:
                    return this.GetRandomNumber();

                default:
                    if (ActiveConfig.DB.HasConstant(this.VariableName))
                    {
                        return ActiveConfig.DB.Constants.First(t => t.Name == this.VariableName).Value;
                    }
                    else
                    {
                        throw new NotFoundException("The specified variable or constant is unknown or not defined - " + this.VariableName);
                    }
            }
        }

        /// <summary>
        /// Applies any transforms to the expanded value
        /// </summary>
        /// <param name="input">The expanded variable value</param>
        /// <returns>The variable value after transforms have been applied</returns>
        private IList<object> ApplyTransforms(object input)
        {
            return Transform.ExecuteTransformChain(this.Transforms, new List<object>() { input });
        }

        /// <summary>
        /// Gets a random string value
        /// </summary>
        /// <returns>A random string of the length specified in the declaration</returns>
        private string GetRandomAlphaNumericString()
        {
            if (!int.TryParse(this.VariableParameters, out int length))
            {
                throw new InvalidOperationException($"The length of the random string could not be extracted from the variable parameter. Variable should be in the format of %{VariableNameRandomAlphaNumericString}:X%, where X is the length of the random string to generate");
            }

            return RandomValueGenerator.GenerateRandomString(length, RandomValueGenerator.AlphaNumericCharacterSet);
        }

        /// <summary>
        /// Gets a random string value
        /// </summary>
        /// <returns>A random string of the length specified in the declaration</returns>
        private string GetRandomLowerCaseString()
        {
            if (!int.TryParse(this.VariableParameters, out int length))
            {
                throw new InvalidOperationException($"The length of the random string could not be extracted from the variable parameter. Variable should be in the format of %{VariableNameRandomLowerCaseString}:X%, where X is the length of the random string to generate");
            }

            return RandomValueGenerator.GenerateRandomString(length, RandomValueGenerator.LowercaseAlphaCharacterSet);
        }

        /// <summary>
        /// Gets a random number value
        /// </summary>
        /// <returns>A random number of the length specified in the declaration</returns>
        private long GetRandomNumber()
        {
            int length;

            if (string.IsNullOrWhiteSpace(this.VariableParameters))
            {
                return RandomValueGenerator.GenerateRandomNumber();
            }
            else if (!int.TryParse(this.VariableParameters, out length))
            {
                throw new InvalidOperationException($"The length of the random string could not be extracted from the variable parameter. Variable should be in the format of %{VariableNameRandomNumber}:X%, where X is the length of the random number to generate");
            }

            return RandomValueGenerator.GenerateRandomNumber(length);
        }

        public static bool DoesVariableSupportParameters(string name)
        {
            switch (name)
            {
                case VariableDeclaration.VariableNameRandomAlphaNumericString:
                case VariableDeclaration.VariableNameRandomLowerCaseString:
                case VariableDeclaration.VariableNameRandomNumber:
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsValidParameter(string name, string parameter)
        {
            if (!VariableDeclaration.DoesVariableSupportParameters(name))
            {
                return false;
            }

            if (name == VariableDeclaration.VariableNameRandomAlphaNumericString ||
                name == VariableDeclaration.VariableNameRandomNumber ||
                name == VariableDeclaration.VariableNameRandomLowerCaseString)
            {
                return int.TryParse(parameter, out int _);
            }

            return false;
        }

        public static IEnumerable<string> GetBuiltInVariableNames()
        {
            yield return VariableDeclaration.VariableNameUniquePlaceholderNumericOptional;
            yield return VariableDeclaration.VariableNameUniquePlaceholderNumeric;
            yield return VariableDeclaration.VariableNameDateLocal;
            yield return VariableDeclaration.VariableNameDateUtc;
            yield return VariableDeclaration.VariableNameNewGuid;
            yield return VariableDeclaration.VariableNameRandomAlphaNumericString;
            yield return VariableDeclaration.VariableNameRandomLowerCaseString;
            yield return VariableDeclaration.VariableNameRandomNumber;
        }

        public static IEnumerable<string> GetUserDefinedVariableNames()
        {
            return ActiveConfig.DB.Constants.Select(t => t.Name);
        }

        public static IEnumerable<string> GetVariableNames()
        {
            foreach (var variableName in VariableDeclaration.GetBuiltInVariableNames())
            {
                yield return variableName;
            }

            foreach (var variableName in VariableDeclaration.GetUserDefinedVariableNames())
            {
                yield return variableName;
            }
        }

        /// <summary>
        /// Gets the data type of the declared variable
        /// </summary>
        /// <returns>An AttributeType of the declared variable</returns>
        private ExtendedAttributeType GetVariableDataType()
        {
            switch (this.VariableName)
            {
                case VariableDeclaration.VariableNameUniquePlaceholderNumericOptional:
                case VariableDeclaration.VariableNameUniquePlaceholderNumeric:
                case VariableDeclaration.VariableNameRandomNumber:
                    return ExtendedAttributeType.Integer;

                case VariableDeclaration.VariableNameDateLocal:
                case VariableDeclaration.VariableNameDateUtc:
                    return ExtendedAttributeType.DateTime;

                case VariableDeclaration.VariableNameNewGuid:
                case VariableDeclaration.VariableNameRandomAlphaNumericString:
                case VariableDeclaration.VariableNameRandomLowerCaseString:
                    return ExtendedAttributeType.String;

                default:

                    if (ActiveConfig.DB.HasConstant(this.VariableName))
                    {
                        return ExtendedAttributeType.String;
                    }
                    else
                    {
                        throw new ArgumentException("The specified variable name is unknown: " + this.VariableName);
                    }
            }
        }
    }
}