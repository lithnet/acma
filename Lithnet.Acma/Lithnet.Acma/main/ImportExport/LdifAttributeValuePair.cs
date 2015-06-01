using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.Acma
{
    public class LdifAttributeValuePair
    {
        public string Name { get; set; }

        public object Value { get; set; }

        public IEnumerable<string> Options { get; set; }

        public LdifAttributeValuePair(string line)
        {
            bool base64 = false;
            int sepIndex = line.IndexOf(":");
            this.ExtractNameAndOptions(line, sepIndex);

            if (sepIndex + 1 >= line.Length)
            {
                return;
            }

            if (line[sepIndex + 1] == ':')
            {
                base64 = true;
                sepIndex++;
            }

            if (line[sepIndex + 1] == '<')
            {
                throw new FormatException("This parser does not support attribute input from a URL");
            }

            int valuelength = line.Length - sepIndex - 1;
            string value = line.Substring(sepIndex + 1, valuelength);

            value = value.TrimStart(' ');

            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            if (base64)
            {
                this.Value = Convert.FromBase64String(value);
            }
            else
            {
                this.Value = value;
            }
        }

        private void ExtractNameAndOptions(string line, int sepIndex)
        {
            string attributeDescription = line.Substring(0, sepIndex);

            if (!attributeDescription.Contains(';'))
            {
                this.Name = attributeDescription;
                return;
            }
            else
            {
                int firstOptionIndex = attributeDescription.IndexOf(';');
                this.Name = attributeDescription.Substring(0, firstOptionIndex);
                string options = attributeDescription.Remove(0, this.Name.Length + 1);
                this.Options = options.Split(';').Select(t => t.Trim());
            }
        }
    }
}
