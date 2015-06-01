using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;

namespace Lithnet.Fim.UniversalMARE.UnitTests
{
    public class TestCSEntry : CSEntry
    {
        Dictionary<string, Attrib> attributes;

        public TestCSEntry()
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
       
        public override AttributeNameEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public override string ObjectType
        {
            get { throw new NotImplementedException(); }
        }

        public override void CommitNewConnector()
        {
            throw new NotImplementedException();
        }

        public override DateTime ConnectionChangeTime
        {
            get { throw new NotImplementedException(); }
        }

        public override RuleType ConnectionRule
        {
            get { throw new NotImplementedException(); }
        }

        public override ConnectionState ConnectionState
        {
            get { throw new NotImplementedException(); }
        }

        public override void Deprovision()
        {
            throw new NotImplementedException();
        }

        public override ReferenceValue DN
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override ManagementAgent MA
        {
            get { throw new NotImplementedException(); }
        }

        public override ValueCollection ObjectClass
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override string RDN
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override string ToString()
        {
            return "TestCSEntry";
        }
    }
}
