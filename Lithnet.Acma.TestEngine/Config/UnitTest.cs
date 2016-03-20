using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Lithnet.Acma;
using Lithnet.Logging;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using Microsoft.MetadirectoryServices;
using System.Transactions;
using Lithnet.MetadirectoryServices;

namespace Lithnet.Acma.TestEngine
{
    [DataContract(Name = "unit-test", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    public class UnitTest : UnitTestObject
    {
        private ObservableCollection<UnitTestStep> steps;

        public delegate void UnitTestStartEventHandler(object sender);

        public delegate void UnitTestCompleteEventHandler(object sender, UnitTestOutcome outcome);

        public static event UnitTestStartEventHandler UnitTestStartingEvent;

        public static event UnitTestCompleteEventHandler UnitTestCompletedEvent;

        [DataMember(Name = "steps")]
        public ObservableCollection<UnitTestStep> Steps
        {
            get
            {
                return this.steps;
            }
            set
            {
                if (this.steps != null)
                {
                    this.Steps.CollectionChanged -= Steps_CollectionChanged;

                    foreach (var item in this.Steps)
                    {
                        item.ParentTest = null;
                    }
                }

                this.steps = value;

                if (this.steps != null)
                {
                    this.Steps.CollectionChanged += Steps_CollectionChanged;

                    foreach (var item in this.Steps)
                    {
                        item.ParentTest = this;
                    }
                }
            }
        }

        [DataMember(Name = "expected-constructors")]
        public ObservableCollection<string> ExpectedConstructors { get; set; }

        private ObservableCollection<string> ExecutedConstructors { get; set; }

        public UnitTest()
        {
            this.Initialize();
        }

        public IEnumerable<UnitTestStepObjectCreation> GetObjectCreationSteps()
        {
            return this.Steps.OfType<UnitTestStepObjectCreation>();
        }

        public IEnumerable<UnitTestStepObjectCreation> GetObjectCreationStepsBeforeItem(UnitTestStep step)
        {
            int index = this.Steps.IndexOf(step);

            for (int i = 0; i < index; i++)
            {
                if (this.Steps[i] is UnitTestStepObjectCreation)
                {
                    yield return this.Steps[i] as UnitTestStepObjectCreation;
                }
            }
        }

        public UnitTestStepObjectCreation GetObjectCreationStep(string testName)
        {
            return this.Steps.OfType<UnitTestStepObjectCreation>().FirstOrDefault(t => t.Name == testName);
        }

        public UnitTestOutcome Execute()
        {
            this.RaiseStartEvent();

            Logger.WriteLine("Executing unit test: " + this.ID);

            int stepCount = 0;
            UnitTestOutcome outcome = new UnitTestOutcome();
            outcome.Test = this;
            this.ExecutedConstructors = new ObservableCollection<string>();
            AttributeConstructor.AttributeConstructorCompletedEvent += AttributeConstructor_AttributeConstructorCompletedEvent;
            List<UnitTestStep> executedSteps = new List<UnitTestStep>();

            try
            {

                Logger.IncreaseIndent();
               // using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, TransactionManager.MaximumTimeout))
                //{
                    for (int i = 0; i < this.Steps.Count; i++)
                    {
                        UnitTestStep step = this.Steps[i];

                        stepCount++;

                        try
                        {
                            Logger.IncreaseIndent();
                            Logger.WriteLine("Executing step {0}/{1}: {2} ", stepCount, this.steps.Count, step.Name);

                            executedSteps.Add(step);
                            step.Execute();

                        }
                        catch (EvaluationFailedException)
                        {
                            outcome.FailureStepNumber = stepCount;
                            outcome.Result = UnitTestResult.Failed;
                            outcome.FailureStepName = step.Name;
                            outcome.AdditionalFailureInformation = ((UnitTestStepObjectEvaluation)step).FailureReason;
                            this.RaiseCompleteEvent(outcome);

                            Logger.WriteLine(outcome.Result.ToString());
                            Logger.WriteLine(outcome.Description);
                            Logger.WriteLine(outcome.AdditionalFailureInformation);

                            if (UnitTestFile.BreakOnTestFailure)
                            {
                                throw new OperationCanceledException();
                            }

                            return outcome;
                        }
                        catch (Exception ex)
                        {
                            Logger.WriteException(ex);
                            outcome.FailureStepNumber = stepCount;
                            outcome.FailureStepName = step.Name;
                            outcome.Result = UnitTestResult.Error;
                            outcome.Exception = ex;
                            this.RaiseCompleteEvent(outcome);

                            if (UnitTestFile.BreakOnTestFailure)
                            {
                                throw new OperationCanceledException();
                            }

                            return outcome;
                        }
                        finally
                        {
                            Logger.DecreaseIndent();
                        }
                    }
                //}
            }
            finally
            {
                Logger.DecreaseIndent();
                AttributeConstructor.AttributeConstructorCompletedEvent -= AttributeConstructor_AttributeConstructorCompletedEvent;

                foreach (UnitTestStep step in executedSteps)
                {
                    try
                    {
                        step.Cleanup();
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLine("Warning: An exception occurred during cleanup", LogLevel.Debug);
                        Logger.WriteException(ex, LogLevel.Debug);
                    }
                }
            }

            if (this.ExpectedConstructors.Count > 0)
            {
                outcome.MissingConstructors = this.ExpectedConstructors.Except(this.ExecutedConstructors).Where(t => !string.IsNullOrWhiteSpace(t)).ToList();

                if (outcome.MissingConstructors.Count > 0 && outcome.Result == UnitTestResult.Passed)
                {
                    outcome.Result = UnitTestResult.Inconclusive;
                    outcome.AdditionalFailureInformation =
                        string.Format("The following constructors did not execute: {0}", outcome.MissingConstructors.ToCommaSeparatedString());
                }
                else
                {
                    outcome.Result = UnitTestResult.Passed;
                }
            }
            else
            {
                outcome.Result = UnitTestResult.Passed;
            }

            if (outcome.Result == UnitTestResult.Passed && !this.Steps.Any(t => t is UnitTestStepObjectEvaluation))
            {
                outcome.Result = UnitTestResult.Inconclusive;
                outcome.AdditionalFailureInformation = "No object evaluations were present in the test";
            }

            this.RaiseCompleteEvent(outcome);
            return outcome;
        }

        private void AttributeConstructor_AttributeConstructorCompletedEvent(AttributeConstructor sender)
        {
            this.ExecutedConstructors.Add(sender.ID);
        }

        private void RaiseStartEvent()
        {
            if (UnitTestStartingEvent != null)
            {
                UnitTestStartingEvent(this);
            }
        }

        private void RaiseCompleteEvent(UnitTestOutcome outcome)
        {
            if (UnitTestCompletedEvent != null)
            {
                UnitTestCompletedEvent(this, outcome);
            }
        }

        private void Initialize()
        {
            this.Steps = new ObservableCollection<UnitTestStep>();
            this.ExpectedConstructors = new ObservableCollection<string>();
            this.ExecutedConstructors = new ObservableCollection<string>();
        }

        private void Steps_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems.OfType<UnitTestStep>())
                    {
                        item.ParentTest = this;
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems.OfType<UnitTestStep>())
                    {
                        if (item.ParentTest == this)
                        {
                            item.ParentTest = null;
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    foreach (var item in e.OldItems.OfType<UnitTestStep>())
                    {
                        if (item.ParentTest == this)
                        {
                            item.ParentTest = null;
                        }
                    }

                    foreach (var item in e.NewItems.OfType<UnitTestStep>())
                    {
                        item.ParentTest = this;
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    foreach (var item in this.Steps)
                    {
                        item.ParentTest = this;
                    }

                    break;
                default:
                    break;
            }
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.Initialize();
        }
    }
}
