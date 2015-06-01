using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.Acma
{
    public interface IAcmaExternalEventExtensible
    {
        void Execute(RaisedEvent raisedEvent);
        void OnEventRaised(RaisedEvent raisedEvent, MAObjectHologram sender);
    }
}
