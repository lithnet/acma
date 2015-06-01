using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lithnet.Fim.Core;
using System.Text.RegularExpressions;

namespace Lithnet.Acma.DataModel
{
    public partial class SafetyRule
    {
        private Regex patternRegex;

        private Regex PatternRegex
        {
            get
            {
                if (this.patternRegex == null)
                {
                    this.patternRegex = new Regex(this.Pattern);
                }

                return this.patternRegex;
            }
        }

        /// <summary>
        /// Validates the rule against the specified value
        /// </summary>
        /// <param name="value">The value to validate</param>
        public void Validate(AttributeValue value)
        {
            if (this.AcmaSchemaMapping.Attribute != value.Attribute)
            {
                return;
            }

            if (value.IsNull)
            {
                if (this.NullAllowed)
                {
                    return;
                }
                else
                {
                    throw new SafetyRuleViolationException(this.Name, "null");
                }
            }

            if (!this.PatternRegex.IsMatch(value.ToSmartString()))
            {
                throw new SafetyRuleViolationException(this.Name, value.ToSmartString());
            }
        }

        /// <summary>
        /// Validates the rule against the specified values
        /// </summary>
        /// <param name="values">The values to evaluate</param>
        public void Validate(AttributeValues values)
        {
            if (this.AcmaSchemaMapping.Attribute != values.Attribute)
            {
                return;
            }

            if (values.IsEmptyOrNull)
            {
                if (this.NullAllowed)
                {
                    return;
                }
                else
                {
                    throw new SafetyRuleViolationException(this.Name, "null");
                }
            }

            foreach (AttributeValue value in values)
            {
                if (!this.PatternRegex.IsMatch(value.ToSmartString()))
                {
                    throw new SafetyRuleViolationException(this.Name, value.ToSmartString());
                }
            }
        }

        
    }
}
