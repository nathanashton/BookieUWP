using System.IO;
using Windows.UI.Xaml;

namespace Bookie.Common
{
    public class Theme
    {
        public ResourceDictionary Resource { get; set; }

        public string Name
        {
            get
            {
                if (Resource != null)
                {
                    return Path.GetFileNameWithoutExtension(Resource.Source.ToString());
                }
                return "";
            }
        }
    }
}
