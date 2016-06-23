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
            AcmaExternalExitEvent.EventQueue = new BlockingCollection<RaisedEvent>();

            Thread thread = new Thread(() =>
            {
                Logger.StartThreadLog();
                foreach (RaisedEvent e in AcmaExternalExitEvent.EventQueue.GetConsumingEnumerable())
                {
                    /* Parallel foreach is causing a thread-leak in this configuration. Overnight it ran up 32k threads and 1.9GB of memory
                     * WinDbg shows the same call stack for each thread. Thread count will continue to grow from the moment the service
                     * is started.
                     *
                        00000000`23e5e058 000007fe`fd821420 ntdll!NtWaitForMultipleObjects+0xa
                        00000000`23e5e060 00000000`77821753 KERNELBASE!WaitForMultipleObjectsEx+0xe8
                        00000000`23e5e160 000007fe`f31d8976 kernel32!WaitForMultipleObjectsExImplementation+0xb3
                        00000000`23e5e1f0 000007fe`f31d877a clr!WaitForMultipleObjectsEx_SO_TOLERANT+0x62
                        00000000`23e5e250 000007fe`f31d8591 clr!Thread::DoAppropriateWaitWorker+0x1d0
                        00000000`23e5e350 000007fe`f31d883d clr!Thread::DoAppropriateWait+0x7d
                        00000000`23e5e3d0 000007fe`f31f3ed6 clr!CLREventBase::WaitEx+0xc0
                        00000000`23e5e460 000007fe`f31f3dea clr!AwareLock::EnterEpilogHelper+0xc6
                        00000000`23e5e520 000007fe`f31f4521 clr!AwareLock::EnterEpilog+0x62
                        00000000`23e5e580 000007fe`f31f4293 clr!AwareLock::Contention+0x1e3
                        00000000`23e5e640 000007fe`f27dd095 clr!JITutil_MonContention+0xaf
                        00000000`23e5e7d0 000007fe`f27dc601 mscorlib_ni+0xdbd095
                        00000000`23e5e870 000007fe`f27bae42 mscorlib_ni+0xdbc601
                        00000000`23e5e8b0 000007fe`f279b672 mscorlib_ni+0xd9ae42
                        00000000`23e5e980 000007fe`f2952e01 mscorlib_ni+0xd7b672
                        00000000`23e5e9b0 000007fe`f1f3611e mscorlib_ni+0xf32e01
                        00000000`23e5ea50 000007fe`f1ec39a5 mscorlib_ni+0x51611e
                        00000000`23e5eac0 000007fe`f1ec3719 mscorlib_ni+0x4a39a5
                        00000000`23e5ec20 000007fe`f1f363f5 mscorlib_ni+0x4a3719
                        00000000`23e5ec50 000007fe`f1f35a95 mscorlib_ni+0x5163f5
                        00000000`23e5ed30 000007fe`f1ef136a mscorlib_ni+0x515a95
                        00000000`23e5ed70 000007fe`f30ba7f3 mscorlib_ni+0x4d136a
                        00000000`23e5ee30 000007fe`f30ba6de clr!CallDescrWorkerInternal+0x83
                        00000000`23e5ee70 000007fe`f30bae76 clr!CallDescrWorkerWithHandler+0x4a
                        00000000`23e5eeb0 000007fe`f3140221 clr!MethodDescCallSite::CallTargetWorker+0x251
                        00000000`23e5f060 000007fe`f30bc121 clr!QueueUserWorkItemManagedCallback+0x2a
                        00000000`23e5f150 000007fe`f30bc0a8 clr!ManagedThreadBase_DispatchInner+0x2d
                        00000000`23e5f190 000007fe`f30bc019 clr!ManagedThreadBase_DispatchMiddle+0x6c
                        00000000`23e5f290 000007fe`f30bc15f clr!ManagedThreadBase_DispatchOuter+0x75
                        00000000`23e5f320 000007fe`f31401ae clr!ManagedThreadBase_FullTransitionWithAD+0x2f
                        00000000`23e5f380 000007fe`f313f046 clr!ManagedPerAppDomainTPCount::DispatchWorkItem+0xa2
                        00000000`23e5f510 000007fe`f313ef3a clr!ThreadpoolMgr::ExecuteWorkRequest+0x46
                        00000000`23e5f540 000007fe`f31ffcb6 clr!ThreadpoolMgr::WorkerThreadStart+0xf4
                        00000000`23e5f5d0 00000000`778159ed clr!Thread::intermediateThreadProc+0x7d
                        00000000`23e5f910 00000000`7794b371 kernel32!BaseThreadInitThunk+0xd
                        00000000`23e5f940 00000000`00000000 ntdll!RtlUserThreadStart+0x1d
                    */

                    //Parallel.ForEach(AcmaExternalExitEvent.EventQueue.GetConsumingEnumerable(), e =>
                    //{
                    try
                    {
                        Logger.StartThreadLog();
                        Logger.WriteLine("Executing event: " + e.EventID);

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
                    //});
                }

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
