using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Phase_v1._0
{
    class Track
    {
        private string title;
        public string TrackTitle
        {
            get
            {
                return title;
            }
            set => title = value;
        }

        private Uri path;
        public Uri TrackUri
        {
            get
            {
                return path;
            }
            set => path = value;
        }

        public Track(string path)
        {
            TrackTitle = Path.GetFileName(path);
            TrackTitle = TrackTitle.Substring(0, TrackTitle.Length - 4);
            TrackUri = new Uri(@path);
        }

        public override string ToString()
        {
            return TrackTitle;
        }
    }
}
