using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Phase_v2._0
{
    static class PlaylistManager
    {
        static public Playlist defaultPlaylist = new Playlist();
        static public Playlist customPlaylist = new Playlist();

        static public string lastLoadedPlaylist;

        //TRUE - for default
        //FALSE - for custom
        static public bool activePlaylist = true;


        static public void ClearActivePlaylist()
        {
            if (activePlaylist == true)
            {
                defaultPlaylist.Tracklist.Clear();
                ((MainWindow)System.Windows.Application.Current.MainWindow).DefaultPlaylistBox.Items.Clear();
            }
            else
            {
                customPlaylist.Tracklist.Clear();
                ((MainWindow)System.Windows.Application.Current.MainWindow).CustomPlaylistBox.Items.Clear();
            }
        }

        //Only audio files are allowed
        static private List<string> ValidateTracks(string[] files)
        {
            List<string> validatedFiles = new List<string>();

            foreach (var file in files)
            {
                if (file.EndsWith(".mp3") || file.EndsWith("wav"))
                {
                    validatedFiles.Add(file);
                }
            }

            return validatedFiles;
        }

        static public void AddTracks(string[] files)
        {
            List<string> paths = ValidateTracks(files);

            if (((MainWindow)System.Windows.Application.Current.MainWindow).selectedTab == true)
            {
                foreach (var file in paths)
                {
                    Console.WriteLine("Added " + file + " to default");
                    Track temp = new Track(file);
                    defaultPlaylist.Tracklist.Add(temp);
                    ((MainWindow)System.Windows.Application.Current.MainWindow).DefaultPlaylistBox.Items.Add(temp);
                }
            }
            else
            {
                foreach (var file in paths)
                {
                    Console.WriteLine("Added " + file + "to custom");
                    Track temp = new Track(file);
                    customPlaylist.Tracklist.Add(temp);
                    ((MainWindow)System.Windows.Application.Current.MainWindow).CustomPlaylistBox.Items.Add(temp);
                }
            }
        }

        static public void RemoveTrack(bool selectedTab, Track track)
        {
            Track tempCurrentTrack = Player.CurrentTrack;

            if (selectedTab == activePlaylist)
            {
                if (Player.CurrentPlaylist.Tracklist.Count > 1)
                {
                    if (activePlaylist == true)
                    {
                        defaultPlaylist.Tracklist.Remove(track);                        
                        ((MainWindow)System.Windows.Application.Current.MainWindow).DefaultPlaylistBox.Items.Remove(((MainWindow)System.Windows.Application.Current.MainWindow).DefaultPlaylistBox.SelectedItem);

                    }
                    else
                    {
                        customPlaylist.Tracklist.Remove(track);
                        ((MainWindow)System.Windows.Application.Current.MainWindow).CustomPlaylistBox.Items.Remove(((MainWindow)System.Windows.Application.Current.MainWindow).CustomPlaylistBox.SelectedItem);
                    }


                    Player.CurrentPlaylist.Tracklist.Remove(track);
                    Player.Reload();
                }
                else
                {
                    if (activePlaylist == true)
                    {
                        defaultPlaylist.Tracklist.Clear();
                        ((MainWindow)System.Windows.Application.Current.MainWindow).DefaultPlaylistBox.Items.Clear();
                    }
                    else
                    {
                        customPlaylist.Tracklist.Clear();
                        ((MainWindow)System.Windows.Application.Current.MainWindow).CustomPlaylistBox.Items.Clear();
                    }

                    Player.CurrentPlaylist.Tracklist.Clear();
                    Player.Stop();
                }
            }
            else
            {
                if (selectedTab == true)
                {
                    defaultPlaylist.Tracklist.Remove(track);
                    ((MainWindow)System.Windows.Application.Current.MainWindow).DefaultPlaylistBox.Items.Remove(((MainWindow)System.Windows.Application.Current.MainWindow).DefaultPlaylistBox.SelectedItem);
                }
                else
                {
                    customPlaylist.Tracklist.Remove(track);
                    ((MainWindow)System.Windows.Application.Current.MainWindow).CustomPlaylistBox.Items.Remove(((MainWindow)System.Windows.Application.Current.MainWindow).CustomPlaylistBox.SelectedItem);
                }
            }

            if (track == tempCurrentTrack)
            {
                if (Player.isPlaying == true)
                {
                    Player.Current();
                }
                else
                {

                }
            }

            ((MainWindow)System.Windows.Application.Current.MainWindow).PreviousTrackLabel.Text = Player.PreviousTrack.TrackTitle;
            ((MainWindow)System.Windows.Application.Current.MainWindow).CurrentTrackLabel.Text = Player.CurrentTrack.TrackTitle;
            ((MainWindow)System.Windows.Application.Current.MainWindow).NextTrackLabel.Text = Player.NextTrack.TrackTitle;
        }

        static public Playlist GetActivePlaylist()
        {
            if (activePlaylist == true)
            {
                return defaultPlaylist;
            }
            else
            {
                return customPlaylist;
            }
        }

        static public void DrawSelection(int index, bool selectedTab)
        {
            if (selectedTab == activePlaylist)
            {
                if (activePlaylist == true)
                {
                    ((MainWindow)System.Windows.Application.Current.MainWindow).DefaultPlaylistBox.SelectedIndex = index;
                }
                else
                {
                    ((MainWindow)System.Windows.Application.Current.MainWindow).CustomPlaylistBox.SelectedIndex = index;
                }
            }
        }

        static public void SaveSelectedPlaylist(bool selectedTab)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = "pl";
            saveFileDialog.AddExtension = true;
            saveFileDialog.Title = "Save selected playlist as";
            saveFileDialog.InitialDirectory = @"d:\";
            saveFileDialog.Filter = "Playlist file (*.pl)|*.pl";

            if (selectedTab == true)
            {
                if (defaultPlaylist != null)
                {
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        if (saveFileDialog.FileName.EndsWith(".pl"))
                        {
                            defaultPlaylist.Save(saveFileDialog.FileName);
                        }
                    }
                }
            }
            else
            {
                if (customPlaylist != null)
                {
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        if (saveFileDialog.FileName.EndsWith(".pl"))
                        {
                            customPlaylist.Save(saveFileDialog.FileName);
                        }
                    } 
                }
            }
          
        }

        static public void OpenPlaylistDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = "pl";
            openFileDialog.AddExtension = true;
            openFileDialog.Title = "Open playlist";
            openFileDialog.InitialDirectory = @"d:\";
            openFileDialog.Filter = "Playlist file (*.pl)|*.pl";


            if (openFileDialog.ShowDialog() == true)
            {
                LoadPlaylist(openFileDialog.FileName);
            }
        }

        static public void LoadPlaylist(string filepath)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).DefaultPlaylistTab.Focus();

            lastLoadedPlaylist = filepath;
            Playlist openedPlaylist = Load(lastLoadedPlaylist);

            //Stop and clear
            Player.Stop();
            defaultPlaylist.Tracklist.Clear();
            ((MainWindow)System.Windows.Application.Current.MainWindow).DefaultPlaylistBox.Items.Clear();

            //Add to the player and playlist tab
            activePlaylist = true;
            AddTracks(openedPlaylist.ToStringArray());
            defaultPlaylist = openedPlaylist;
            Player.Load(defaultPlaylist);
        }

        static private Playlist Load(string path)
        {
            Playlist tempPlaylist = new Playlist();

            XDocument doc = XDocument.Load(path);

            var playlist = doc.Elements();

            var tracklist = playlist.Elements();

            foreach (var track in tracklist.Elements())
            {
                string trackTitle = track.Element("title").Value;
                Uri trackUri = new Uri(track.Element("uri").Value);

                Track tempTrack = new Track();

                tempTrack.TrackTitle = trackTitle;
                tempTrack.TrackUri = trackUri;

                tempPlaylist.Tracklist.Add(tempTrack);
            }

            return tempPlaylist;
        }

        static public void Clear(bool selectedTab)
        {
            Player.Stop();

            if (selectedTab == true)
            {
                defaultPlaylist.Tracklist.Clear();
                ((MainWindow)System.Windows.Application.Current.MainWindow).DefaultPlaylistBox.Items.Clear();
            }
            else
            {
                customPlaylist.Tracklist.Clear();
                ((MainWindow)System.Windows.Application.Current.MainWindow).CustomPlaylistBox.Items.Clear();
            }
        }
    }
}
