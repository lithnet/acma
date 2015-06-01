using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.Acma
{
    public class SchemaAttributeUsage
    {
        public SchemaAttributeUsage(string objectType, string path, string context)
        {
            this.ObjectType = objectType;
            this.Path = path;
            this.Context = context;
        }

        public string ObjectType { get; set; }
        public string Path { get; set; }
        public string Context { get; set; }
    }
}
