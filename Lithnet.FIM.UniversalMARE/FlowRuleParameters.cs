using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lithnet.Fim.Transforms;
using System.Text.RegularExpressions;
using Lithnet.Fim.Core;
using Microsoft.MetadirectoryServices;

namespace Lithnet.Fim.UniversalMARE
{
    public class FlowRuleParameters
    {
        private string flowRuleDescription;

        private XmlConfigFile config;

        public FlowRuleParameters(XmlConfigFile config, string flowRuleName, FlowRuleType type)
        {
            this.config = config;
            this.SourceAttributeNames = new List<string>();
            this.Transforms = new List<Transform>();

            string parametersFromAlias = this.GetParametersFromAlias(flowRuleName);

            if (parametersFromAlias == null)
            {
                this.flowRuleDescription = flowRuleName;
            }
            else
            {
                this.flowRuleDescription = parametersFromAlias;
            }

            this.FlowRuleType = type;
            this.ValidateFlowRuleName();

            this.GetSourceAttributes();
            this.GetTargetAttribute();
            this.GetTransforms();
        }

        public List<Transform> Transforms { get; set; }

        public List<string> SourceAttributeNames { get; set; }

        public string TargetAttributeName { get; set; }

        public FlowRuleType FlowRuleType { get; set; }

        private void ValidateFlowRuleName()
        {
            switch (this.FlowRuleType)
            {
                case FlowRuleType.Import:
                    this.ValidateFlowRuleNameImport();
                    break;

                case FlowRuleType.Export:
                    this.ValidateFlowRuleNameExport();
                    break;

                case FlowRuleType.Join:
                    this.ValidateFlowRuleNameJoin();
                    break;

                default:
                    throw new ArgumentException();
            }
        }

        private void ValidateFlowRuleNameImport()
        {
            if (this.flowRuleDescription.Contains(">>"))
            {
                if (this.flowRuleDescription.Contains("<<"))
                {
                    throw new InvalidFlowRuleNameException();
                }
            }
            else
            {
                throw new InvalidFlowRuleNameException();
            }
        }

        private void ValidateFlowRuleNameJoin()
        {
            if (this.flowRuleDescription.Contains(">>"))
            {
                if (this.flowRuleDescription.Contains("<<"))
                {
                    throw new InvalidFlowRuleNameException();
                }
            }
            else
            {
                throw new InvalidFlowRuleNameException();
            }
        }

        private void ValidateFlowRuleNameExport()
        {
            if (this.flowRuleDescription.Contains("<<"))
            {
                if (this.flowRuleDescription.Contains(">>"))
                {
                    throw new InvalidFlowRuleNameException();
                }
            }
            else
            {
                throw new InvalidFlowRuleNameException();
            }
        }

        private void GetSourceAttributes()
        {
            if (this.FlowRuleType == FlowRuleType.Export)
            {
                this.GetSourceAttributesForExport();
            }
            else if (this.FlowRuleType == FlowRuleType.Import || this.FlowRuleType == FlowRuleType.Join)
            {
                this.GetSourceAttributesForImport();
            }
        }

        private void GetSourceAttributesForImport()
        {
            string attributeNames;
            Match match = Regex.Match(this.flowRuleDescription, @"(?<attribute>.+?)>>.+");
            if (match.Success)
            {

                attributeNames = match.Groups["attribute"].Captures[0].Value;
            }
            else
            {
                throw new InvalidFlowRuleNameException();
            }

            string[] attributes = Regex.Split(attributeNames, @"\+");


            if (attributes.Length == 0)
            {
                throw new InvalidFlowRuleNameException();
            }

            this.SourceAttributeNames.AddRange(attributes);
        }

        private void GetSourceAttributesForExport()
        {
            string attributeNames;

            Match match = Regex.Match(this.flowRuleDescription, @".+<<(?<attribute>.+?)", RegexOptions.RightToLeft);
            if (match.Success)
            {

                attributeNames = match.Groups["attribute"].Captures[0].Value;
            }
            else
            {
                throw new ArgumentException("The flow rule name was not in a supported format");
            }

            IEnumerable<string> attributes = Regex.Split(attributeNames, @"\+").Reverse();

            if (attributes.Count() == 0)
            {
                throw new InvalidFlowRuleNameException();
            }

            this.SourceAttributeNames.AddRange(attributes);
        }



        private void GetTargetAttribute()
        {
            if (this.FlowRuleType == FlowRuleType.Import)
            {
                this.GetTargetAttributeForImport();
            }
            else if (this.FlowRuleType == FlowRuleType.Export)
            {
                this.GetTargetAttributeForExport();
            }
            else if (this.FlowRuleType == UniversalMARE.FlowRuleType.Join)
            {
                this.TargetAttributeName = null;
            }
        }

        private void GetTargetAttributeForImport()
        {
            Match match = Regex.Match(this.flowRuleDescription, @">>(?<attribute>.+?)$", RegexOptions.RightToLeft);
            if (match.Success)
            {
                this.TargetAttributeName = match.Groups["attribute"].Captures[0].Value;
            }
            else
            {
                throw new InvalidFlowRuleNameException();
            }
        }

        private void GetTargetAttributeForExport()
        {
            Match match = Regex.Match(this.flowRuleDescription, @"(?<attribute>.+?)<<.+");
            if (match.Success)
            {
                this.TargetAttributeName = match.Groups["attribute"].Captures[0].Value;
            }
            else
            {
                throw new InvalidFlowRuleNameException();
            }
        }


        private void GetTransforms()
        {
            if (this.FlowRuleType == FlowRuleType.Import)
            {
                this.GetTransformsForImport();
            }
            else if (this.FlowRuleType == FlowRuleType.Export)
            {
                this.GetTransformsForExport();
            }
            else if (this.FlowRuleType == UniversalMARE.FlowRuleType.Join)
            {
                this.GetTransformsForJoin();
            }

            this.ValidateLoopbackTransformPosition();
        }

        private void GetTransformsForImport()
        {
            Match match = Regex.Match(this.flowRuleDescription, @".*?>>(?<transform>.+)>>.+");

            if (match.Success)
            {
                string transformNames = match.Groups["transform"].Captures[0].Value;
                string[] transformNamesSplit = Regex.Split(transformNames, ">>");

                this.PopulateTransformsFromNames(transformNamesSplit);
            }
            else
            {
                throw new ArgumentException("The flow rule name was not in a supported format");
            }
        }

        private void PopulateTransformsFromNames(IEnumerable<string> transformNamesSplit)
        {
            foreach (string transformName in transformNamesSplit)
            {
                if (this.config.Transforms.Contains(transformName))
                {
                    this.Transforms.Add(this.config.Transforms[transformName]);
                }
                else
                {
                    throw new ConfigurationException("The specified transform was not found: " + transformName);
                }
            }
        }

        private void ValidateLoopbackTransformPosition()
        {
            int loopbackCount = this.Transforms.Count(t => t.ImplementsLoopbackProcessing);

            if (this.FlowRuleType == FlowRuleType.Join && loopbackCount > 0)
            {
                throw new InvalidFlowRuleNameException("Loopback transforms are not supported on join flow rules");
            }

            if (loopbackCount > 1)
            {
                throw new InvalidFlowRuleNameException("There can only be a single loopback transform in a transform chain");
            }
            else if (loopbackCount == 1)
            {
                Transform loopback = this.Transforms.First(t => t.ImplementsLoopbackProcessing);
                if (this.Transforms.Last() != loopback)
                {
                    throw new InvalidFlowRuleNameException("A loopback transform must appear last in the chain");
                }
            }
        }

        private void GetTransformsForJoin()
        {
            Match match = Regex.Match(this.flowRuleDescription, @".*?>>(?<transform>.+)$");

            if (match.Success)
            {
                string transformNames = match.Groups["transform"].Captures[0].Value;
                string[] transformNamesSplit = Regex.Split(transformNames, ">>");

                this.PopulateTransformsFromNames(transformNamesSplit);

            }
            else
            {
                throw new ArgumentException("The flow rule name was not in a supported format");
            }

            this.ValidateLoopbackTransformPosition();
        }

        private void GetTransformsForExport()
        {
            Match match = Regex.Match(this.flowRuleDescription, @".*?<<(?<transform>.+)<<.+");
            if (match.Success)
            {
                string transformNames = match.Groups["transform"].Captures[0].Value;
                IEnumerable<string> transformNamesSplit = Regex.Split(transformNames, "<<").Reverse();

                this.PopulateTransformsFromNames(transformNamesSplit);
            }
            else
            {
                throw new ArgumentException("The flow rule name was not in a supported format");
            }

            this.ValidateLoopbackTransformPosition();
        }

        public string GetParametersFromAlias(string flowRuleName)
        {
            FlowRuleAlias alias = this.config.FlowRuleAliases.FirstOrDefault(t => t.Alias.Equals(flowRuleName));
            if (alias == null)
            {
                return null;
            }
            else
            {
                return alias.FlowRuleDefinition;
            }
        }
    }
}
