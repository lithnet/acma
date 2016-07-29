using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Lithnet.Acma.ServiceModel.Client.Model
{
    [Serializable]
    
    public class AcmaAttribute 
    {
        public int ID { get; set; }

        public string Name { get; set; }
        
        public int Type { get; set; }

        public bool IsMultivalued { get; set; }

        public int Operation { get; set; }

        public bool IsIndexable { get; set; }

        public bool IsIndexed { get; set; }

        public bool IsBuiltIn { get; set; }

        internal string TableName { get; set; }

        internal string ColumnName { get; set; }
    }
}
