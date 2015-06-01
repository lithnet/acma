using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;

namespace Lithnet.Fim.UniversalMARE.UnitTests
{
    public class TestMVEntry : MVEntry
    {
        Dictionary<string, Attrib> attributes;

        public TestMVEntry()
        {
            this.attributes = new Dictionary<string, Attrib>();
        }

        public override Attrib this[string attributeName]
        {
            get
            {
                return attributes[attributeName];
            }
        }

        public void Add(TestAttrib attribute)
        {
            this.attributes.Add(attribute.Name, attribute);
        }

        public void Remove(string name)
        {
            this.attributes.Remove(name);
        }

        public override ConnectedMACollection ConnectedMAs
        {
            get { throw new NotImplementedException(); }
        }

        public override Guid ObjectID
        {
            get { throw new NotImplementedException(); }
        }

        public override string ObjectType
        {
            get { throw new NotImplementedException(); }
        }

        public override AttributeNameEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "TestMVEntry";
        }
    }
}
