using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using Lithnet.Acma.DataModel;
using Lithnet.Common.ObjectModel;
using System.Collections.Generic;
using Lithnet.Logging;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Lithnet.Acma
{
    [DataContract(Name = "exit-event-external", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    [KnownType(typeof(AcmaExternalExitEventCmd))]
    [KnownType(typeof(AcmaExternalExitEventExtensible))]
    public abstract class AcmaExternalExitEvent : AcmaEvent
    {
        public static BlockingCollection<RaisedEvent> EventQueue = new BlockingCollection<RaisedEvent>();

        private static ManualResetEvent waitHandle;

        /// <summary>
        /// The rule group used for determining if the event should be raised
        /// </summary>
        private RuleGroup ruleGroup;

        public AcmaExternalExitEvent()
        {
            this.Initialize();
        }

        public override AcmaEventType EventType
        {
            get
            {
                return AcmaEventType.ExternalExitEvent;
            }
        }

        [DataMember(Name = "error-handling-mode")]
        public AcmaEventErrorHandlingMode ErrorHandlingMode { get; set; }

        [DataMember(Name = "run-async")]
        public bool RunAsync { get; set; }

        /// <summary>
        /// Gets or sets the rule group set that determines if this event should be raised
        /// </summary>
        [DataMember(Name = "rule-group")]
        public RuleGroup RuleGroup
        {
            get
            {
                return this.ruleGroup;
            }

            set
            {
                if (this.ruleGroup != null)
                {
                    this.ruleGroup.ObjectClassScopeProvider = null;
                }

                if (value != null)
                {
                    value.ObjectClassScopeProvider = this;
                }

                this.ruleGroup = value;
            }
        }

        public void Execute(RaisedEvent raisedEvent)
        {
            if (this.IsDisabled)
            {
                Logger.WriteLine("Skipping disabled event: {0}", this.ID);
                return;
            }

            if (this.RunAsync)
            {
                AcmaExternalExitEvent.EventQueue.Add(raisedEvent);
            }
            else
            {
                this.ExecuteInternal(raisedEvent);
            }
        }

        protected abstract void ExecuteInternal(RaisedEvent raisedEvent);

        protected virtual void OnBeforeObjectCommit(MAObjectHologram hologram)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();
            this.RuleGroup = new RuleGroup();
        }

        public static void StartEventQueue()
        {
            AcmaExternalExitEvent.waitHandle = new ManualResetEvent(false);
            Logger.WriteLine("Starting loop thread for external event processing", LogLevel.Debug);

            Thread thread = new Thread(() =>
            {
                Logger.StartThreadLog();
                Parallel.ForEach(AcmaExternalExitEvent.EventQueue.GetConsumingEnumerable(), e =>
                    {
                        try
                        {
                            Logger.StartThreadLog();
                            Logger.WriteLine("Executing async event: " + e.EventID);

                            ((AcmaExternalExitEvent)e.Event).ExecuteInternal(e);
                        }
                        catch (Exception ex)
                        {
                            Logger.WriteLine("The exit event {0} threw an exception", e.EventID);
                            Logger.WriteException(ex);
                        }
                        finally
                        {
                            Logger.EndThreadLog();
                        }
                    });

                Logger.EndThreadLog();
                Logger.WriteLine("Event queue completed. Signaling wait handle", LogLevel.Debug);
                AcmaExternalExitEvent.waitHandle.Set();
            });

            thread.Start();
        }

        public static void CompleteEventQueue()
        {
            Logger.WriteLine("Marking event queue as complete", LogLevel.Debug);
            AcmaExternalExitEvent.EventQueue.CompleteAdding();
        }

        public static void WaitForComplete()
        {
            Logger.WriteLine("Waiting for event queue to finalize", LogLevel.Debug);

            if (AcmaExternalExitEvent.waitHandle != null)
            {
                AcmaExternalExitEvent.waitHandle.WaitOne();
            }
        }
    }
}
