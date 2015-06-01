using Lithnet.Common.Presentation;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Lithnet.Fim.Core;
using Microsoft.MetadirectoryServices;
using Lithnet.Acma;
using Lithnet.Acma.DataModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;

namespace Lithnet.Acma.Presentation
{
    public class AcmaExternalExitEventCmdViewModel : AcmaEventViewModel
    {
        private AcmaExternalExitEventCmd typedModel;

        private RuleGroupViewModel ruleGroup;

        private ValueDeclarationViewModel arguments;

        public AcmaExternalExitEventCmdViewModel(AcmaExternalExitEventCmd model)
            : base(model)
        {
            this.typedModel = model;
            
            if (this.typedModel.RuleGroup == null)
            {
                this.typedModel.RuleGroup = new RuleGroup();
            }

            this.RuleGroup = new RuleGroupViewModel(this.typedModel.RuleGroup, false, "Execution rules");

            if (model.Arguments == null)
            {
                model.Arguments = new ValueDeclaration();
            }

            this.Arguments = new ValueDeclarationViewModel(model.Arguments, model.ObjectClass);
        }

        public IEnumerable ConfigItems
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

        public string CommandLine
        {
            get
            {
                return this.typedModel.CommandLine;
            }

            set
            {
                this.typedModel.CommandLine = value;
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

        public ValueDeclarationViewModel Arguments
        {
            get
            {
                return this.arguments;
            }
            set
            {
                if (this.arguments != null)
                {
                    this.UnregisterChildViewModel(this.arguments);
                }

                this.arguments = value;

                if (this.arguments != null)
                {
                    this.RegisterChildViewModel(this.arguments);
                }
            }
        }

        public string Declaration
        {
            get
            {
                return this.Arguments.Declaration;
            }

            set
            {
                try
                {
                    this.Arguments.Declaration = value;
                    this.RemoveError("Declaration");
                }
                catch (Exception ex)
                {
                    this.AddError("Declaration", ex.Message);
                }
            }
        }
    }
}
