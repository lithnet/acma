using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Lithnet.Fim.Core;
using Microsoft.MetadirectoryServices;
using Lithnet.Fim.Transforms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;

namespace Lithnet.Fim.UniversalMARE
{
    /// <summary>
    /// Summary description for MAExtensionObject.
    /// </summary>
    public class MAExtensionObject : IMASynchronization
    {
        public XmlConfigFile config;

        public MAExtensionObject()
        {
            TransformGlobal.HostProcessSupportsLoopbackTransforms = true;
            this.config = new XmlConfigFile();
        }

        void IMASynchronization.Initialize()
        {
            string path = ConfigManager.AppConfigPath;

            if (!System.IO.File.Exists(path))
            {
                throw new FileNotFoundException("Unable to open configuration file: " + path);
            }

            this.config = ConfigManager.LoadXml(path);
        }

        void IMASynchronization.Terminate()
        {
            //
            // TODO: write termination code
            //
        }

        bool IMASynchronization.ShouldProjectToMV(CSEntry csentry, out string MVObjectType)
        {
            throw new EntryPointNotImplementedException();
        }

        DeprovisionAction IMASynchronization.Deprovision(CSEntry csentry)
        {
            throw new EntryPointNotImplementedException();
        }

        bool IMASynchronization.FilterForDisconnection(CSEntry csentry)
        {
            throw new EntryPointNotImplementedException();
        }

        void IMASynchronization.MapAttributesForJoin(string FlowRuleName, CSEntry csentry, ref ValueCollection values)
        {
            FlowRuleParameters parameters = new FlowRuleParameters(this.config, FlowRuleName, FlowRuleType.Join);

            AttributeType type;
            IList<object> sourceValues = this.GetSourceValuesForImport(parameters, csentry, out type);
            IList<object> returnValues = Transform.ExecuteTransformChain(parameters.Transforms, sourceValues);

            foreach (object value in returnValues)
            {
                switch (type)
                {
                    case AttributeType.Binary:
                        values.Add(TypeConverter.ConvertData<byte[]>(value));
                        break;

                    case AttributeType.Integer:
                        values.Add(TypeConverter.ConvertData<long>(value));
                        break;

                    case AttributeType.String:
                        values.Add(TypeConverter.ConvertData<string>(value));
                        break;

                    case AttributeType.Reference:
                    case AttributeType.Boolean:
                    case AttributeType.Undefined:
                        throw new UnknownOrUnsupportedDataTypeException();
                }
            }
        }

        bool IMASynchronization.ResolveJoinSearch(string joinCriteriaName, CSEntry csentry, MVEntry[] rgmventry, out int imventry, ref string MVObjectType)
        {
            throw new EntryPointNotImplementedException();
        }

        void IMASynchronization.MapAttributesForImport(string FlowRuleName, CSEntry csentry, MVEntry mventry)
        {
            FlowRuleParameters parameters = new FlowRuleParameters(this.config, FlowRuleName, FlowRuleType.Import);
            AttributeType type;
            
            IList<object> sourceValues = this.GetSourceValuesForImport(parameters, csentry, out type);
            IList<object> returnValues;

            if (parameters.Transforms.Any(t => t.ImplementsLoopbackProcessing))
            {
                object existingTargetValue = this.GetExistingTargetValueForImportLoopback(parameters, mventry);
                returnValues = Transform.ExecuteTransformChainWithLoopback(parameters.Transforms, sourceValues, existingTargetValue);
            }
            else
            {
                returnValues = Transform.ExecuteTransformChain(parameters.Transforms, sourceValues);
            }

            this.SetDestinationAttributeValueForImport(parameters, mventry, returnValues);
        }

        void IMASynchronization.MapAttributesForExport(string FlowRuleName, MVEntry mventry, CSEntry csentry)
        {
            FlowRuleParameters parameters = new FlowRuleParameters(this.config, FlowRuleName, FlowRuleType.Export);

            IList<object> sourceValues = this.GetSourceValuesForExport(parameters, mventry);

            IList<object> returnValues;

            if (parameters.Transforms.Any(t => t.ImplementsLoopbackProcessing))
            {
                object existingTargetValue = this.GetExistingTargetValueForExportLoopback(parameters, csentry);
                returnValues = Transform.ExecuteTransformChainWithLoopback(parameters.Transforms, sourceValues, existingTargetValue);
            }
            else
            {
                returnValues = Transform.ExecuteTransformChain(parameters.Transforms, sourceValues);
            }

            this.SetDestinationAttributeValueForExport(parameters, csentry, returnValues);
        }

        private IList<object> GetSourceValuesForImport(FlowRuleParameters parameters, CSEntry csentry, out AttributeType attributeType)
        {
            List<object> values = new List<object>();
            attributeType = AttributeType.Undefined;

            foreach (string attributeName in parameters.SourceAttributeNames)
            {
                Attrib attribute = csentry[attributeName];

                if (attributeType == AttributeType.Undefined)
                {
                    attributeType = attribute.DataType;
                }
                else if (attributeType != attribute.DataType)
                {
                    attributeType = AttributeType.String;
                }

                if (attribute.IsMultivalued)
                {
                    values.AddRange(this.GetMVAttributeValue(attribute));
                }
                else
                {
                    values.Add(this.GetSVAttributeValue(attribute));
                }
            }

            return values;
        }

        private IList<object> GetSourceValuesForExport(FlowRuleParameters parameters, MVEntry mventry)
        {
            List<object> values = new List<object>();

            foreach (string attributeName in parameters.SourceAttributeNames)
            {
                Attrib attribute = mventry[attributeName];

                if (attribute.IsMultivalued)
                {
                    values.AddRange(this.GetMVAttributeValue(attribute));
                }
                else
                {
                    values.Add(this.GetSVAttributeValue(attribute));
                }
            }

            return values;
        }

        private object GetExistingTargetValueForImportLoopback(FlowRuleParameters parameters, MVEntry mventry)
        {
            List<object> values = new List<object>();

            Attrib attribute = mventry[parameters.TargetAttributeName];

            if (attribute.IsMultivalued)
            {
                throw new TooManyValuesException();
            }
            else
            {
                return this.GetSVAttributeValue(attribute);
            }
        }

        private object GetExistingTargetValueForExportLoopback(FlowRuleParameters parameters, CSEntry csentry)
        {
            List<object> values = new List<object>();

            Attrib attribute = csentry[parameters.TargetAttributeName];

            if (attribute.IsMultivalued)
            {
                throw new TooManyValuesException();
            }
            else
            {
                return this.GetSVAttributeValue(attribute);
            }
        }

        private void SetDestinationAttributeValueForImport(FlowRuleParameters parameters, MVEntry mventry, IEnumerable<object> values)
        {
            Attrib attribute = mventry[parameters.TargetAttributeName];
            this.SetDestinationAttributeValue(parameters, values, attribute);
        }

        private void SetDestinationAttributeValueForExport(FlowRuleParameters parameters, CSEntry csentry, IEnumerable<object> values)
        {
            Attrib attribute = csentry[parameters.TargetAttributeName];
            this.SetDestinationAttributeValue(parameters, values, attribute);
        }

        private void SetDestinationAttributeValue(FlowRuleParameters parameters, IEnumerable<object> values, Attrib attribute)
        {
            if (attribute.IsMultivalued)
            {
                if (values.Count() == 0)
                {
                    attribute.Delete();
                }
                else
                {
                    this.SetAttributeValues(values, attribute);
                }
            }
            else
            {
                if (values.Count() > 1)
                {
                    throw new TooManyValuesException(parameters.TargetAttributeName);
                }
                else if (values.Count() == 0)
                {
                    attribute.Delete();
                }
                else
                {
                    this.SetAttributeValue(values.First(), attribute);
                }
            }
        }

        private void SetAttributeValue(object attributeValue, Attrib attribute)
        {
            switch (attribute.DataType)
            {
                case AttributeType.Binary:
                    attribute.BinaryValue = TypeConverter.ConvertData<byte[]>(attributeValue);
                    break;

                case AttributeType.Boolean:
                    attribute.BooleanValue = TypeConverter.ConvertData<bool>(attributeValue);
                    break;

                case AttributeType.Integer:
                    attribute.IntegerValue = TypeConverter.ConvertData<long>(attributeValue);
                    break;

                case AttributeType.String:
                    attribute.StringValue = TypeConverter.ConvertData<string>(attributeValue);
                    break;

                case AttributeType.Reference:
                case AttributeType.Undefined:
                default:
                    throw new UnknownOrUnsupportedDataTypeException();
            }
        }

        private void SetAttributeValues(IEnumerable<object> attributeValues, Attrib attribute)
        {
            if (attribute.Values.Count > 0)
            {
                attribute.Values.Clear();
            }

            if (!attribute.IsMultivalued && attributeValues.Count() > 1)
            {
                throw new TooManyValuesException(attribute.Name);
            }
            
            foreach (object value in attributeValues)
            {
                if (value == null)
                {
                    continue;
                }

                switch (attribute.DataType)
                {
                    case AttributeType.Binary:
                        attribute.Values.Add(TypeConverter.ConvertData<byte[]>(value));
                        break;

                    case AttributeType.Integer:
                        attribute.Values.Add(TypeConverter.ConvertData<long>(value));
                        break;

                    case AttributeType.String:
                        attribute.Values.Add(TypeConverter.ConvertData<string>(value));
                        break;

                    case AttributeType.Boolean:
                    case AttributeType.Reference:
                    case AttributeType.Undefined:
                    default:
                        throw new UnknownOrUnsupportedDataTypeException();
                }
            }
        }

        private object GetSVAttributeValue(Attrib attribute)
        {
            object csEntryAttributeValue;

            if (!attribute.IsPresent)
            {
                if (attribute.DataType == AttributeType.Boolean)
                {
                    return false;
                }
                else
                {
                    return null;
                }
            }

            switch (attribute.DataType)
            {
                case AttributeType.Binary:
                    csEntryAttributeValue = attribute.BinaryValue;
                    break;

                case AttributeType.Boolean:
                    csEntryAttributeValue = attribute.BooleanValue;
                    break;

                case AttributeType.Integer:
                    csEntryAttributeValue = attribute.IntegerValue;
                    break;

                case AttributeType.String:
                    csEntryAttributeValue = attribute.StringValue;
                    break;

                case AttributeType.Reference:
                case AttributeType.Undefined:
                default:
                    throw new UnknownOrUnsupportedDataTypeException();
            }
            return csEntryAttributeValue;
        }

        private IEnumerable<object> GetMVAttributeValue(Attrib attribute)
        {
            List<object> attributeValues = new List<object>();

            foreach (Value item in attribute.Values)
            {
                object value;

                switch (item.DataType)
                {
                    case AttributeType.Binary:
                        value = item.ToBinary();
                        break;

                    case AttributeType.Integer:
                        value = item.ToInteger();
                        break;

                    case AttributeType.String:
                        value = item.ToString();
                        break;

                    case AttributeType.Boolean:
                    case AttributeType.Reference:
                    case AttributeType.Undefined:
                    default:
                        throw new UnknownOrUnsupportedDataTypeException();
                }

                attributeValues.Add(value);
            }

            return attributeValues;
        }
    }
}