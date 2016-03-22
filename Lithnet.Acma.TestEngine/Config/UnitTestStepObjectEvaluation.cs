using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lithnet.Acma;
using System.Xml;
using System.Runtime.Serialization;
using Lithnet.Acma.DataModel;
using Lithnet.Logging;

namespace Lithnet.Acma.TestEngine
{
    [DataContract(Name = "object-evaluation", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    public class UnitTestStepObjectEvaluation : UnitTestStep, IObjectClassScopeProvider
    {
        private StringBuilder failureReasons;

        private RuleGroup successCriteria;

        private UnitTestStepObjectCreation objectCreationStep;

        private string objectCreationStepName;

        [DataMember(Name = "object-creation-step-name")]
        public string ObjectCreationStepName
        {
            get
            {
                if (this.objectCreationStep == null)
                {
                    return this.objectCreationStepName;
                }
                else
                {
                    return this.objectCreationStep.Name;
                }
            }
            private set
            {
                this.objectCreationStepName = value;
            }
        }

        public UnitTestStepObjectCreation ObjectCreationStep
        {
            get
            {
                if (this.objectCreationStep == null)
                {
                    if (this.ObjectCreationStepName != null && this.ParentTest != null)
                    {
                        this.objectCreationStep = this.ParentTest.GetObjectCreationStepsBeforeItem(this)
                            .FirstOrDefault(t => t.Name == this.ObjectCreationStepName);
                    }
                    else
                    {
                        return null;
                    }
                }

                return this.objectCreationStep;
            }
            set
            {
                this.objectCreationStep = value;
            }
        }

        [DataMember(Name = "success-criteria")]
        public RuleGroup SuccessCriteria
        {
            get
            {
                return this.successCriteria;
            }
            set
            {
                if (this.successCriteria != null)
                {
                    this.successCriteria.ObjectClassScopeProvider = null;
                }

                this.successCriteria = value;
                this.successCriteria.BlockEventRules = true;
                this.successCriteria.ObjectClassScopeProvider = this;
            }
        }

        [DataMember(Name = "test-name")]
        public override string Name { get; set; }

        public string FailureReason
        {
            get
            {
                return this.failureReasons.ToString();
            }
        }

        public UnitTestStepObjectEvaluation()
        {
            this.Initialize();
        }

        public override void Execute()
        {
            MAObjectHologram sourceObject = this.GetObjectFromAlias();
            Rule.RuleFailedEvent += RuleBase_RuleFailedEvent;
            RuleGroup.RuleGroupFailedEvent += RuleGroup_RuleGroupFailedEvent;
            try
            {
                if (!this.SuccessCriteria.Evaluate(sourceObject))
                {
                    Logger.WriteLine("The unit test evaluation {0} failed", this.Name);
                    Logger.WriteSeparatorLine('-');
                    Logger.WriteLine("MAObject drop");
                    Logger.WriteRaw(sourceObject.AttributeDataToString());
                    Logger.WriteSeparatorLine('-');

                    throw new EvaluationFailedException();
                }
            }
            finally
            {
                Rule.RuleFailedEvent -= RuleBase_RuleFailedEvent;
                RuleGroup.RuleGroupFailedEvent -= RuleGroup_RuleGroupFailedEvent;
            }
        }

        private MAObjectHologram GetObjectFromAlias()
        {
            return ActiveConfig.DB.GetMAObject(this.ObjectCreationStep.ObjectId, this.ObjectCreationStep.ObjectClass);
        }

        private void RuleGroup_RuleGroupFailedEvent(RuleGroup sender, string failureReason)
        {
            failureReasons.AppendLine(string.Format("Rule group evaluation failed: {0}", failureReason));
        }

        private void RuleBase_RuleFailedEvent(Rule sender, string failureReason)
        {
            failureReasons.AppendLine(string.Format("Rule evaluation failed: {0}", failureReason));
        }

        public override void Cleanup()
        {
        }

        protected override void ValidatePropertyChange(string propertyName)
        {
            base.ValidatePropertyChange(propertyName);

            if (propertyName == "ObjectCreationStep" || propertyName == "ParentTest")
            {
                if (this.ParentTest != null)
                {
                    if (this.ObjectCreationStep == null)
                    {
                        this.AddError("ObjectCreationStep", "An object creation step must be specified");
                    }
                    else
                    {
                        this.RemoveError("ObjectCreationStep");
                    }
                }
            }
        }

        private void Initialize()
        {
            this.failureReasons = new StringBuilder();
            this.SuccessCriteria = new RuleGroup();
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.Initialize();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
        }

        public AcmaSchemaObjectClass ObjectClass
        {
            get
            {
                if (this.ObjectCreationStep == null)
                {
                    return null;
                }
                else
                {
                    return this.ObjectCreationStep.ObjectClass;
                }
            }
        }
    }
}
