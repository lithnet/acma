using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lithnet.Common.Presentation;

namespace Lithnet.Acma.Presentation
{
    public class MRUItem
    {
        public MRUItem(string name, DelegateCommand command)
        {
            this.Name = name;
            this.Command = command;
        }

        public string Name { get; set; }
        public DelegateCommand Command { get; set; }
    }
}
