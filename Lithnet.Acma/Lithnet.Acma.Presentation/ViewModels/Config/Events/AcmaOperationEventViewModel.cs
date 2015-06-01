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
    public class AcmaOperationEventViewModel : AcmaEventViewModel
    {
        private AcmaOperationEvent typedModel;

        private DBQueryObjectsViewModel queryGroups;

        public AcmaOperationEventViewModel(AcmaOperationEvent model)
            : base(model)
        {
            this.typedModel = model;
            this.QueryGroups = new DBQueryObjectsViewModel(this.typedModel.RecipientQueries, "Recipient queries");
            this.QueryGroups.CollectionChanged += QueryGroups_CollectionChanged;
            this.ValidateQueryCount();
        }

        private void QueryGroups_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.ValidateQueryCount();
        }

        [PropertyChanged.AlsoNotifyFor("OperationTypeFullImport", "OperationTypeDeltaImport", "OperationTypeExport")]
        public AcmaEventOperationType OperationTypes
        {
            get
            {
                return this.typedModel.OperationTypes;
            }
            set
            {
                if (value == 0)
                {
                    this.typedModel.OperationTypes = AcmaEventOperationType.FullImport;
                }
                else
                {
                    this.typedModel.OperationTypes = value;
                }
            }
        }

        [PropertyChanged.AlsoNotifyFor("OperationTypes")]
        public bool OperationTypeFullImport
        {
            get
            {
                return this.OperationTypes.HasFlag(AcmaEventOperationType.FullImport);
            }
            set
            {
                if (value == true)
                {
                    this.OperationTypes = this.OperationTypes | AcmaEventOperationType.FullImport;
                }
                else
                {
                    this.OperationTypes = this.OperationTypes & ~AcmaEventOperationType.FullImport;
                }
            }
        }

        [PropertyChanged.AlsoNotifyFor("OperationTypes")]
        public bool OperationTypeDeltaImport
        {
            get
            {
                return this.OperationTypes.HasFlag(AcmaEventOperationType.DeltaImport);
            }
            set
            {
                if (value == true)
                {
                    this.OperationTypes = this.OperationTypes | AcmaEventOperationType.DeltaImport;
                }
                else
                {
                    this.OperationTypes = this.OperationTypes & ~AcmaEventOperationType.DeltaImport;
                }
            }
        }

        [PropertyChanged.AlsoNotifyFor("OperationTypes")]
        public bool OperationTypeExport
        {
            get
            {
                return this.OperationTypes.HasFlag(AcmaEventOperationType.Export);
            }
            set
            {
                if (value == true)
                {
                    this.OperationTypes = this.OperationTypes | AcmaEventOperationType.Export;
                }
                else
                {
                    this.OperationTypes = this.OperationTypes & ~AcmaEventOperationType.Export;
                }
            }
        }

        public IEnumerable ConfigItems
        {
            get
            {
                yield return this.QueryGroups;
            }
        }

        public DBQueryObjectsViewModel QueryGroups
        {
            get
            {
                return this.queryGroups;
            }
            set
            {
                if (this.queryGroups != null)
                {
                    this.UnregisterChildViewModel(this.queryGroups);
                }

                this.queryGroups = value;

                if (this.queryGroups != null)
                {
                    this.RegisterChildViewModel(this.queryGroups);
                }
            }
        }

        private void ValidateQueryCount()
        {
            if (this.QueryGroups.Count == 0)
            {
                this.AddError("QueryGroups", "At least one query must be present");
            }
            else
            {
                this.RemoveError("QueryGroups");
            }
        }
    }
}
