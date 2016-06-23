using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Lithnet.Acma
{
    public class ResultEnumerator : IEnumerator<MAObjectHologram>, IEnumerable<MAObjectHologram>
    {
        private DataRowCollection rows;

        private SqlDataAdapter adapter;

        public int CurrentIndex { get; set; }

        public int LastIndex { get; private set; }

        public int TotalCount
        {
            get
            {
                return this.rows.Count;
            }
        }

        public ResultEnumerator(DataRowCollection rows, SqlDataAdapter adapter)
        {
            this.CurrentIndex = -1;
            this.rows = rows;
            this.adapter = adapter;
        }

        public IEnumerator<MAObjectHologram> GetEnumerator()
        {
            return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }

        public MAObjectHologram Current
        {
            get
            {
                return new MAObjectHologram(this.rows[this.CurrentIndex], this.adapter);
            }
        }

        public void Dispose()
        {
        }

        object System.Collections.IEnumerator.Current
        {
            get
            {
                return new MAObjectHologram(this.rows[this.CurrentIndex], this.adapter);
            }
        }

        public bool MoveNext()
        {
            this.CurrentIndex++;
            this.LastIndex++;
            return this.CurrentIndex < this.TotalCount;
        }

        public void Reset()
        {
            //this.CurrentIndex = -1;
        }
    }
}
