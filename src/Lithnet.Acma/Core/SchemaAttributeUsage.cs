using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.Acma
{
    public class SchemaAttributeUsage
    {
        public SchemaAttributeUsage(object usedBy, string objectType, string objectID, string path, string context)
        {
            this.ObjectType = objectType;
            this.Path = path;
            this.Context = context;
            this.ObjectID = objectID;
            this.UsedBy = usedBy;
        }

        public object UsedBy { get; set; }
        public string ObjectType { get; set; }
        public string Path { get; set; }
        public string Context { get; set; }
        public string ObjectID { get; set; }
    }
}
