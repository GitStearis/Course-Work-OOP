using System;
using System.IO;

namespace Phase_v2._0
{
    public class Track
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

        public Track()
        {
            TrackTitle = null;
            TrackUri = null;
        }

        public override string ToString()
        {
            return TrackTitle;
        }
    }
}
