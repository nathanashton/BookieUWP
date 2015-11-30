using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookie.Common.Model
{
    public class TagResult : NotifyBase
    {
        public string Word { get; set; }
        public int Count { get; set; }
        public override string ToString()
        {
            return Word + " (" + Count + ")";
        }

        private bool _checked;

        public bool Checked
        {
            get { return _checked; }
            set { _checked = value;
                NotifyPropertyChanged("Checked");
            }
        }
    }
}
