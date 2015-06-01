// -----------------------------------------------------------------------
// <copyright file="XmlHelper.cs" company="Lithnet">
// Copyright (c) 2013
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Fim.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;
    using System.IO;
    using System.Collections;
    using System.Linq.Expressions;

    /// <summary>
    /// Provides helper function to validate XML file structure
    /// </summary>
    public static class XmlHelper
    {
        public static T Deserialize<T>(Stream stream, XmlSchemaSet schemas)
        {
            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.Schemas = schemas;
            readerSettings.ValidationType = ValidationType.Schema;
            readerSettings.ValidationFlags =
                XmlSchemaValidationFlags.ProcessIdentityConstraints | 
                XmlSchemaValidationFlags.ProcessInlineSchema |
                XmlSchemaValidationFlags.ReportValidationWarnings;

            readerSettings.ValidationEventHandler += readerSettings_ValidationEventHandler;
            readerSettings.ConformanceLevel = ConformanceLevel.Document;

            XmlReader reader = XmlReader.Create(stream, readerSettings);
            reader.ReadStartElement();
            T deserializedObject = XmlHelper.Deserialize<T>(reader);
            reader.Close();
            stream.Close();

            return deserializedObject;
        }

        public static T Deserialize<T>(XmlReader reader)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
                       
            //reader.ReadStartElement();
            return (T)serializer.Deserialize(reader);
        }

        public static XmlReader CreateReader(string filename, XmlSchemaSet schemas)
        {
            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.Schemas = schemas;
            readerSettings.ValidationType = ValidationType.None;
            readerSettings.ValidationFlags =
                XmlSchemaValidationFlags.ProcessIdentityConstraints |
                XmlSchemaValidationFlags.ProcessInlineSchema |
                XmlSchemaValidationFlags.ReportValidationWarnings;

            readerSettings.ValidationEventHandler += readerSettings_ValidationEventHandler;
            readerSettings.ConformanceLevel = ConformanceLevel.Document;

            StreamReader streamReader = new StreamReader(filename);

            return XmlReader.Create(streamReader, readerSettings);
        }

        private static void readerSettings_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            throw e.Exception;
        }

        /// <summary>
        /// Gets the inner text of an element inside an XmlNode and converts it to an integer
        /// </summary>
        /// <param name="elementName">The name of the child element</param>
        /// <param name="node">The node containing the element to retrieve the value from</param>
        /// <returns>An integer representation of the inner text of the specified element of the XmlNode</returns>
        public static int GetNodeInnerTextAsInteger(string elementName, XmlNode node)
        {
            string value = XmlHelper.GetNodeInnerText(elementName, node, true);

            try
            {
                return int.Parse(value);
            }
            catch
            {
                throw new ArgumentException(string.Format("The node <{0}> in node <{1}> contains the following value which could not be parsed into a valid integer value: {2}", elementName, node.Name, value));
            }
        }

        /// <summary>
        /// Gets the inner text of an element inside an XmlNode and converts it to a boolean value
        /// </summary>
        /// <param name="elementName">The name of the child element</param>
        /// <param name="node">The node containing the element to retrieve the value from</param>
        /// <returns>An boolean representation of the inner text of the specified element of the XmlNode</returns>
        public static bool GetNodeInnerTextAsBoolean(string elementName, XmlNode node)
        {
            string value = XmlHelper.GetNodeInnerText(elementName, node, true);

            try
            {
                return bool.Parse(value);
            }
            catch
            {
                throw new ArgumentException(string.Format("The attribute '{0}' in node <{1}> contains the following value which could not be parsed into a valid boolean value: {2}", elementName, node.Name, value));
            }
        }

        /// <summary>
        /// Gets the inner text of an element inside an XmlNode and converts it to a Guid value
        /// </summary>
        /// <param name="elementName">The name of the child element</param>
        /// <param name="node">The node containing the element to retrieve the value from</param>
        /// <returns>An Guid representation of the inner text of the specified element of the XmlNode</returns>
        public static Guid GetNodeInnerTextAsGuid(string elementName, XmlNode node)
        {
            string value = XmlHelper.GetNodeInnerText(elementName, node, true);

            try
            {
                return Guid.Parse(value);
            }
            catch
            {
                throw new ArgumentException(string.Format("The attribute '{0}' in node <{1}> contains the following value which could not be parsed into a valid Guid value: {2}", elementName, node.Name, value));
            }
        }

        /// <summary>
        /// Gets the value of an attribute inside an XmlNode and converts it to an integer
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="node">The node containing the attribute to retrieve the value from</param>
        /// <returns>An integer representation of the value of the specified attribute of the XmlNode</returns>
        public static int GetAttributeValueAsInteger(string attributeName, XmlNode node)
        {
            string value = XmlHelper.GetAttributeValue(attributeName, node, true);
            
            try
            {
                return int.Parse(value);
            }
            catch
            {
                throw new ArgumentException(string.Format("The attribute '{0}' in node <{1}> contains the following value which could not be parsed into a valid integer value: {2}", attributeName, node.Name, value));
            }
        }

        /// <summary>
        /// Gets the value of an attribute inside an XmlNode and converts it to a boolean value
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="node">The node containing the attribute to retrieve the value from</param>
        /// <returns>An boolean representation of the value of the specified attribute of the XmlNode</returns>
        public static bool GetAttributeValueAsBoolean(string attributeName, XmlNode node)
        {
            string value = XmlHelper.GetAttributeValue(attributeName, node, true);

            try
            {
                return bool.Parse(value);
            }
            catch
            {
                throw new ArgumentException(string.Format("The attribute '{0}' in node <{1}> contains the following value which could not be parsed into a valid boolean value: {2}", attributeName, node.Name, value));
            }
        }

        /// <summary>
        /// Gets the value of an attribute inside an XmlNode and converts it to a guid value
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="node">The node containing the attribute to retrieve the value from</param>
        /// <returns>An GUID representation of the value of the specified attribute of the XmlNode</returns>
        public static Guid GetAttributeValueAsGuid(string attributeName, XmlNode node)
        {
            string value = XmlHelper.GetAttributeValue(attributeName, node, true);

            try
            {
                return Guid.Parse(value);
            }
            catch
            {
                throw new ArgumentException(string.Format("The attribute '{0}' in node <{1}> contains the following value which could not be parsed into a valid GUID value: {2}", attributeName, node.Name, value));
            }
        }

        public static bool HasAttribute(string attributeName, XmlNode node)
        {
            return !string.IsNullOrEmpty(XmlHelper.GetAttributeValueOrDefault(attributeName, node));
        }

        /// <summary>
        /// Gets the value of an attribute inside an XmlNode and converts it to a specified enumeration type
        /// </summary>
        /// <typeparam name="T">The type to return the value as</typeparam>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="node">The node containing the attribute to retrieve the value from</param>
        /// <returns>The representation of the value of the specified attribute of the XmlNode cast to the specified enumeration</returns>
        public static T GetAttributeValueOrDefaultAsEnum<T>(string attributeName, XmlNode node)
        {
            string value = XmlHelper.GetAttributeValueOrDefault(attributeName, node);

            if (string.IsNullOrEmpty(value))
            {
                return default(T);
            }

            Type enumType = typeof(T);

            if (!enumType.IsEnum)
            {
                throw new InvalidOperationException("The specified type is not an enum");
            }

            try
            {
                return (T)Enum.Parse(enumType, value, true);
            }
            catch
            {
                throw new ArgumentException(string.Format("The attribute '{0}' in node <{1}> contains the following value which could not be parsed into a valid value: {2}", attributeName, node.Name, value));
            }
        }

        /// <summary>
        /// Gets the value of an attribute inside an XmlNode and converts it to a specified enumeration type
        /// </summary>
        /// <typeparam name="T">The type to return the value as</typeparam>
        /// <param name="elementName">The name of the attribute</param>
        /// <param name="node">The node containing the attribute to retrieve the value from</param>
        /// <returns>The representation of the value of the specified attribute of the XmlNode cast to the specified enumeration</returns>
        public static T GetNodeInnerTextOrDefaultAsEnum<T>(string elementName, XmlNode node)
        {
            string value = XmlHelper.GetNodeInnerTextOrDefault(elementName, node);

            if (string.IsNullOrEmpty(value))
            {
                return default(T);
            }

            Type enumType = typeof(T);

            if (!enumType.IsEnum)
            {
                throw new InvalidOperationException("The specified type is not an enum");
            }

            try
            {
                return (T)Enum.Parse(enumType, value, true);
            }
            catch
            {
                throw new ArgumentException(string.Format("The attribute '{0}' in node <{1}> contains the following value which could not be parsed into a valid value: {2}", elementName, node.Name, value));
            }
        }

        /// <summary>
        /// Gets the value of an attribute inside an XmlNode and converts it to a specified enumeration type
        /// </summary>
        /// <typeparam name="T">The type to return the value as</typeparam>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="node">The node containing the attribute to retrieve the value from</param>
        /// <returns>The representation of the value of the specified attribute of the XmlNode cast to the specified enumeration</returns>
        public static T GetAttributeValueAsEnum<T>(string attributeName, XmlNode node)
        {
            string value = XmlHelper.GetAttributeValue(attributeName, node, true);
           
            Type enumType = typeof(T);
            
            if (!enumType.IsEnum)
            { 
                throw new InvalidOperationException("The specified type is not an enum");
            }   
            
            try
            {
                value = value.Replace(" ", ",");
                return (T)Enum.Parse(enumType, value, true); 
            }
            catch 
            {
                throw new ArgumentException(string.Format("The attribute '{0}' in node <{1}> contains the following value which could not be parsed into a valid value: {2}", attributeName, node.Name, value));
            }
        }

        /// <summary>
        /// Gets the inner text of an element of an XmlNode and converts it to a specified enumeration type
        /// </summary>
        /// <typeparam name="T">The type to return the value as</typeparam>
        /// <param name="elementName">The name of the element</param>
        /// <param name="node">The node containing the element to retrieve the value from</param>
        /// <returns>The representation of the inner text of the specified element of the XmlNode cast to the specified enumeration</returns>
        public static T GetNodeInnerTextAsEnum<T>(string elementName, XmlNode node)
        {
            string value = XmlHelper.GetNodeInnerText(elementName, node, true);
           
            Type enumType = typeof(T);
            
            if (!enumType.IsEnum)
            { 
                throw new InvalidOperationException("The specified type is not an enum");
            }   
            
            try
            {
                return (T)Enum.Parse(enumType, value, true); 
            }
            catch 
            {
                throw new ArgumentException(string.Format("The xml node <{0}> in node <{1}> contains the following value which could not be parsed into a valid value: {2}", elementName, node.Name, value));
            }
        }

        /// <summary>
        /// Gets the value of an attribute inside an XmlNode
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="node">The node containing the attribute to retrieve the value from</param>
        /// <param name="throwOnEmptyValue">Indicates if an exception should be thrown if the value of the attribute is empty</param>
        /// <returns>The value of the specified attribute of the XmlNode</returns>
        /// <exception cref="MissingValueException">Throw when the attribute was not present on the node</exception>
        public static string GetAttributeValue(string attributeName, XmlNode node, bool throwOnEmptyValue)
        {
            XmlAttribute attribute = node.Attributes[attributeName];
            XmlHelper.ThrowOnMissingOrEmptyAttribute(attributeName, node, throwOnEmptyValue);

            return attribute.Value;
        }

        /// <summary>
        /// Gets the value of an attribute inside an XmlNode
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="node">The node containing the attribute to retrieve the value from</param>
        /// <returns>The value of the specified attribute of the XmlNode, or null if the attribute is not present</returns>
        public static string GetAttributeValueOrDefault(string attributeName, XmlNode node)
        {
            XmlAttribute attribute = node.Attributes[attributeName];

            if (attribute == null)
            {
                return null;
            }
            else
            {
                return attribute.Value;
            }
        }

        /// <summary>
        /// Gets the value of an attribute inside an XmlNode
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="node">The node containing the attribute to retrieve the value from</param>
        /// <typeparam name="T">The type to convert the attribute value to</typeparam>
        /// <returns>The value of the specified attribute of the XmlNode, or null if the attribute is not present</returns>
        public static T GetAttributeValueOrDefault<T>(string attributeName, XmlNode node)
        {
            XmlAttribute attribute = node.Attributes[attributeName];

            if (attribute == null)
            {
                return default(T);
            }
            else
            {
                return TypeConverter.ConvertData<T>(attribute.Value);
            }
        }

        /// <summary>
        /// Gets the inner text of an element inside an XmlNode
        /// </summary>
        /// <param name="elementName">The name of the child element</param>
        /// <param name="node">The node containing the element to retrieve the value from</param>
        /// <param name="throwOnEmptyInnerText">Indicates if an exception should be thrown if the inner text of the element is empty</param>
        /// <returns>The inner text of the specified element of the XmlNode</returns>
        /// <exception cref="MissingValueException">Thrown when the element is missing or has a null value</exception>
        public static string GetNodeInnerText(string elementName, XmlNode node, bool throwOnEmptyInnerText)
        {
            if (!string.IsNullOrWhiteSpace(elementName))
            {
                XmlHelper.ThrowOnMissingElement(node, throwOnEmptyInnerText, elementName);
                XmlNode innerNode = node.ChildNodes.OfType<XmlElement>().First(t => t.LocalName == elementName);
                return innerNode.InnerText;
            }
            else
            {
                if (throwOnEmptyInnerText && string.IsNullOrWhiteSpace(node.InnerText))
                {
                    throw new MissingValueException(string.Format("The xml node <{0}> must have a value", node.Name));
                }

                return node.InnerText;
            }
        }

        /// <summary>
        /// Gets the inner text of an element inside an XmlNode
        /// </summary>
        /// <param name="elementName">The name of the child element</param>
        /// <param name="node">The node containing the element to retrieve the value from</param>
        /// <returns>The inner text of the specified element of the XmlNode, or null, if the element doesn't exist</returns>
        public static string GetNodeInnerTextOrDefault(string elementName, XmlNode node)
        {
            XmlNode innerNode = node.SelectSingleNode(elementName);

            if (innerNode == null)
            {
                return null;
            }
            else
            {
                return innerNode.InnerText;
            }
        }

        /// <summary>
        /// Throws an exception if the specified attribute is missing from the XmlNode
        /// </summary>
        /// <param name="attributeName">The attribute to find</param>
        /// <param name="node">The XmlNode to evaluate</param>
        public static void ThrowOnMissingAttribute(string attributeName, XmlNode node)
        {
            XmlHelper.ThrowOnMissingOrEmptyAttribute(attributeName, node, false);
        }

        /// <summary>
        /// Throws an exception if the specified attribute is missing from the XmlNode, or if its value is an empty string
        /// </summary>
        /// <param name="attributeName">The attribute to find</param>
        /// <param name="node">The XmlNode to evaluate</param>
        public static void ThrowOnMissingOrEmptyAttribute(string attributeName, XmlNode node)
        {
            XmlHelper.ThrowOnMissingOrEmptyAttribute(attributeName, node, true);
        }

        /// <summary>
        /// Throws an error is any of the listed elements are not present in the XML node
        /// </summary>
        /// <param name="node">The node to evaluation</param>
        /// <param name="elementNames">The names of the mandatory elements</param>
        public static void ThrowOnMissingElement(XmlNode node, params string[] elementNames)
        {
            XmlHelper.ThrowOnMissingElement(node, false, elementNames);
        }

        /// <summary>
        /// Throws an error is any of the listed elements are not present in the XML node, or if they are present but have a null or empty value
        /// </summary>
        /// <param name="node">The node to evaluation</param>
        /// <param name="elementNames">The names of the mandatory elements</param>
        public static void ThrowOnMissingOrEmptyElement(XmlNode node, params string[] elementNames)
        {
            XmlHelper.ThrowOnMissingElement(node, true, elementNames);
        }

        /// <summary>
        /// Throws an exception if the specified attribute is missing from the XmlNode, and optionally if its value is an empty string
        /// </summary>
        /// <param name="attributeName">The attribute to find</param>
        /// <param name="node">The XmlNode to evaluate</param>
        /// <param name="throwOnEmptyValue">Indicates if an exception should be thrown if the attribute value is an empty string</param>
        private static void ThrowOnMissingOrEmptyAttribute(string attributeName, XmlNode node, bool throwOnEmptyValue)
        {
            XmlAttribute attribute = node.Attributes[attributeName];

            if (attribute == null)
            {
                throw new MissingValueException(string.Format("The xml attribute '{0}' is missing from the <{1}> parent node", attributeName, node.Name));
            }
            else if (throwOnEmptyValue && string.IsNullOrWhiteSpace(attribute.Value))
            {
                throw new MissingValueException(string.Format("The xml attribute '{0}' in <{1}> parent node must have a value", attributeName, node.Name));
            }
        }

        /// <summary>
        /// Throws an error is any of the listed elements are not present in the XML node, and optionally, if they are present but have a null or empty value
        /// </summary>
        /// <param name="node">The node to evaluation</param>
        /// <param name="throwOnEmptyInnerText">A value indicating if an element with an empty inner text value should cause a validation failure</param>
        /// <param name="elementNames">The names of the mandatory elements</param>
        private static void ThrowOnMissingElement(XmlNode node, bool throwOnEmptyInnerText, params string[] elementNames)
        {
            foreach (string elementName in elementNames)
            {
                XmlNode innerNode = node.ChildNodes.OfType<XmlElement>().FirstOrDefault(t => t.LocalName == elementName);
                if (innerNode == null)
                {
                    throw new MissingValueException(string.Format("The xml node <{0}> is missing from the <{1}> parent node", elementName, node.Name));
                }
                else if (throwOnEmptyInnerText && string.IsNullOrWhiteSpace(innerNode.InnerText))
                {
                    throw new MissingValueException(string.Format("The xml node <{0}> in parent node <{1}> must have a value", elementName, node.Name));
                }
            }
        }
    }
}
