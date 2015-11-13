using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bookie.Mvvm;

namespace Bookie
{
    public class Letter : NotifyBase
    {
        private string _name;
        private double _width;

        public string Name
        {
            get { return _name; }
            set { _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        public double LWidth
        {
            get { return _width; }
            set { _width = value;
                NotifyPropertyChanged("LWidth");
            }
        }

        private bool _selected;

        public bool Selected
        {
            get { return _selected; }
            set { _selected = value;
                NotifyPropertyChanged("Selected");
            }
        }

    }
}
