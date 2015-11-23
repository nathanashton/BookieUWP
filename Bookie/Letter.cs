using Bookie.Mvvm;

namespace Bookie
{
    public class Letter : NotifyBase
    {
        private string _name;

        private bool _selected;
        private double _width;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        public double LWidth
        {
            get { return _width; }
            set
            {
                _width = value;
                NotifyPropertyChanged("LWidth");
            }
        }

        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                NotifyPropertyChanged("Selected");
            }
        }
    }
}