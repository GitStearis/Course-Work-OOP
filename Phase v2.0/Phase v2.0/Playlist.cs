﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Phase_v2._0
{
    public class Playlist
    {
        public List<Track> Tracklist = new List<Track>();

        public Track this[int index]
        {
            get { return Tracklist[index]; }
            set => Tracklist[index] = value;
        }

        public Playlist()
        {

        }

        public int Count()
        {
            return Tracklist.Count;
        }

        public int IndexOf(Track track)
        {
            return Tracklist.IndexOf(track);
        }

        public Track First()
        {
            return Tracklist.First();
        }
        public Track Last()
        {
            return Tracklist.Last();
        }

        public override string ToString()
        {
            string result = "\n";
            foreach (var track in Tracklist)
            {
                result += Tracklist.IndexOf(track) + track.TrackTitle + "\n";
            }
            return result;
        }

        //MAKE PLAYLIST SAVING
        public void Save(string path)
        {
            XDocument doc = new XDocument();

            XElement playlist = new XElement("playlist");

            XElement tracklist = new XElement("tracklist");
            foreach (var track in Tracklist)
            {
                XElement trackElement = new XElement("track");
                trackElement.Add(new XElement("title", track.TrackTitle),
                                new XElement("uri", track.TrackUri));
                tracklist.Add(trackElement);
            }

            playlist.Add(tracklist);
            doc.Add(playlist);

            doc.Save(path);
        }
    }
}