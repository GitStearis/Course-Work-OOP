using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Phase_v2._0
{
    static class Restorer
    {
        static string datFilePath = "previous_session.dat";

        static public void SaveCurrentSession()
        {
            FileStream filestream = new FileStream(datFilePath, FileMode.Append);
            filestream.Close();

            XDocument doc = new XDocument();
            XElement information = new XElement("information");

            string defaultPlaylistPath = PlaylistManager.lastLoadedPlaylist;
            int playedTrack = -1;
            if (PlaylistManager.activePlaylist == true)
            {
                if (Player.CurrentTrack != null)
                {
                    playedTrack = Player.CurrentPlaylist.IndexOf(Player.CurrentTrack);
                }
            }
            else
            {
                playedTrack = 0;
            }

            string volumeLevel = Player.player.Volume.ToString();
            string playerPosition = Player.player.Position.TotalMilliseconds.ToString();


            XElement lastDefaultPlaylist = new XElement("DefaultPlaylist", defaultPlaylistPath);
            XElement lastPlayedTrack = new XElement("PlayedTrack", playedTrack);
            XElement lastVolumeLevel = new XElement("VolumeLevel", volumeLevel);
            XElement lastPlayerPosition = new XElement("PlayerPosition", playerPosition);

            information.Add(lastDefaultPlaylist);
            information.Add(lastPlayedTrack);
            information.Add(lastVolumeLevel);
            information.Add(lastPlayerPosition);

            doc.Add(information);

            doc.Save(datFilePath);


        }
        static public void LoadLastSession()
        {
            try
            {
                XDocument doc = XDocument.Load(datFilePath);
                XElement info = doc.Element("information");         

                string defaultPlaylistPath = info.Element("DefaultPlaylist").Value;
                int playedTrack = Int32.Parse(info.Element("PlayedTrack").Value);
                double volumeLevel = Double.Parse(info.Element("VolumeLevel").Value);
                double playerPosition = Double.Parse(info.Element("PlayerPosition").Value);

                Console.WriteLine(defaultPlaylistPath);
                Console.WriteLine(playedTrack);
                Console.WriteLine(volumeLevel);
                Console.WriteLine(playerPosition);

                if (defaultPlaylistPath != null)
                {
                    PlaylistManager.LoadPlaylist(defaultPlaylistPath);
                }

                if (playedTrack != -1)
                {
                    Player.CurrentTrack = Player.CurrentPlaylist[playedTrack];
                    //TODO: RESTORE LAST PLAYED TRACK
                }

            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }
        }
    }
}
