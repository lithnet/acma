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
    using System.Text.RegularExpressions;
    using Lithnet.Fim.Core;
    using Lithnet.Fim.Transforms;
    using Microsoft.MetadirectoryServices;

    /// <summary>
    /// Contains the declaration of a variable within a DeclarationString
    /// </summary>
    public class VariableDeclaration : DeclarationComponent
    {
        internal VariableDeclaration(string variableName, string variableParameters, IList<Transform> transforms, string declaration)
        {
            this.StartingValue = 1;
            this.VariableName = variableName;
            this.VariableParameters = variableParameters;
            this.Transforms = transforms;
            this.Declaration = declaration;

            if (this.VariableName == "o" || this.VariableName == "n")
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
            this.StartingValue = 1;
        }

        /// <summary>
        /// Gets the variable value
        /// </summary>
        /// <returns>The raw expanded value of the variable</returns>
        private object GetVariableValue()
        {
            switch (this.VariableName)
            {
                case "date":
                    return DateTime.Now.Truncate(TimeSpan.TicksPerSecond);

                case "utcdate":
                    return DateTime.Now.ToUniversalTime().Truncate(TimeSpan.TicksPerSecond);

                case "o":
                case "n":
                    if (this.StartingValue < ActiveConfig.UniqueAllocationDepth)
                    {
                        return this.StartingValue++;
                    }
                    else
                    {
                        throw new MaximumAllocationAttemptsExceededException("The maximum number of unique allocation attempts has been exceeded");
                    }

                case "newguid":
                    return Guid.NewGuid();

                case "randstring":
                    return this.GetRandomString();

                case "randnum":
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
        private string GetRandomString()
        {
            int length;

            if (!int.TryParse(this.VariableParameters, out length))
            {
                throw new InvalidOperationException("The length of the random string could not be extracted from the variable parameter. Variable should be in the format of %randstring:X%, where X is the length of the random string to generate");
            }

            return RandomValueGenerator.GenerateRandomString(length);
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
                throw new InvalidOperationException("The length of the random string could not be extracted from the variable parameter. Variable should be in the format of %randnum:X%, where X is the length of the random number to generate");
            }

            return RandomValueGenerator.GenerateRandomNumber(length);
        }

        public static bool DoesVariableSupportParameters(string name)
        {
            switch (name)
            {
                case "randstring":
                case "randnum":
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

            if (name == "randstring" || name=="randnum")
            {
                int length;
                return int.TryParse(parameter, out length);
            }

            return false;
        }

        public static IEnumerable<string> GetBuiltInVariableNames()
        {
            yield return "o";
            yield return "n";
            yield return "date";
            yield return "utcdate";
            yield return "newguid";
            yield return "randstring";
            yield return "randnum";

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
                case "o":
                case "n":
                case "randnum":
                    return ExtendedAttributeType.Integer;

                case "date":
                case "utcdate":
                    return ExtendedAttributeType.DateTime;

                case "newguid":
                case "randstring":
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