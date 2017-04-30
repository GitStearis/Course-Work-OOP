using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phase_v2._0
{
    static class PlaylistManager
    {
        static public Playlist defaultPlaylist = new Playlist();
        static public Playlist customPlaylist = new Playlist();

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

            if (activePlaylist == true)
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
    }
}
