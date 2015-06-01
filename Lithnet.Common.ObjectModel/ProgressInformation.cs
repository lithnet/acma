using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.Common.ObjectModel
{
    [PropertyChanged.ImplementPropertyChanged]
    public class ProgressInformation
    {
        private bool hasCanceled;
        private bool canCancel;

        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public int CurrentValue { get; set; }
        public string OperationDescription { get; set; }
        public string ProgressDescription { get; set; }
        public bool Canceled
        {
            get
            {
                return this.hasCanceled;
            }
            set
            {
                this.hasCanceled = value;
            }
        }

        public bool CanCancel
        {
            get
            {
                if (this.hasCanceled)
                {
                    return false;
                }
                else
                {
                    return this.canCancel;
                }
            }
            set
            {
                this.canCancel = value;
            }
        }
    }
}
