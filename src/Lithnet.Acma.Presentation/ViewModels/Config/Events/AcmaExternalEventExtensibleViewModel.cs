using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using Lithnet.Acma;
using Lithnet.Acma.DataModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;

namespace Lithnet.Acma.Presentation
{
    public class AcmaExternalExitEventExtensibleViewModel : AcmaEventViewModel
    {
        private AcmaExternalExitEventExtensible typedModel;

        private RuleGroupViewModel ruleGroup;

        public AcmaExternalExitEventExtensibleViewModel(AcmaExternalExitEventExtensible model)
            : base(model)
        {
            this.typedModel = model;

            if (this.typedModel.RuleGroup == null)
            {
                this.typedModel.RuleGroup = new RuleGroup();
            }

            this.RuleGroup = new RuleGroupViewModel(this.typedModel.RuleGroup, false, "Execution rules");
        }

        public override IEnumerable<ViewModelBase> ChildNodes
        {
            get
            {
                yield return this.RuleGroup;
            }
        }

        public RuleGroupViewModel RuleGroup
        {
            get
            {
                return this.ruleGroup;
            }
            set
            {
                if (this.ruleGroup != null)
                {
                    this.UnregisterChildViewModel(this.ruleGroup);
                }

                this.ruleGroup = value;

                if (this.ruleGroup != null)
                {
                    this.RegisterChildViewModel(this.ruleGroup);
                }
            }
        }

        public string ClassName
        {
            get
            {
                return this.typedModel.ClassName;
            }

            set
            {
                this.typedModel.ClassName = value;
            }
        }

        public string ExtensionPath
        {
            get
            {
                return this.typedModel.ExtensionPath;
            }

            set
            {
                this.typedModel.ExtensionPath = value;
            }
        }

        public bool RunAsync
        {
            get
            {
                return this.typedModel.RunAsync;
            }
            set
            {
                this.typedModel.RunAsync = value;
                if (value)
                {
                    this.ErrorHandlingMode = AcmaEventErrorHandlingMode.Log;
                }
            }
        }

        public bool CanSetErrorHandlingMode
        {
            get
            {
                return !this.RunAsync;
            }
        }

        public AcmaEventErrorHandlingMode ErrorHandlingMode
        {
            get
            {
                return this.typedModel.ErrorHandlingMode;
            }
            set
            {
                this.typedModel.ErrorHandlingMode = value;
            }
        }
    }
}
