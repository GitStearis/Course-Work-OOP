using System;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Phase_v2._0
{
    static class Player
    {
        public static MediaPlayer player = new MediaPlayer();

        private static Playlist currentPlaylist = new Playlist();
        internal static Playlist CurrentPlaylist { get => currentPlaylist; set => currentPlaylist = value; }
        private static Playlist shuffledPlaylist = new Playlist();
        internal static Playlist ShuffledPlaylist { get => shuffledPlaylist; set => shuffledPlaylist = value; }
        private static Playlist primaryPlaylist = new Playlist();
        internal static Playlist PrimaryPlaylist { get => primaryPlaylist; set => primaryPlaylist = value; }

        private static Track previousTrack;
        public static Track PreviousTrack { get => previousTrack; set => previousTrack = value; }
        private static Track currentTrack;
        public static Track CurrentTrack { get => currentTrack; set => currentTrack = value; }
        private static Track nextTrack;
        public static Track NextTrack { get => nextTrack; set => nextTrack = value; }


        public static double pauseTime;
        public static double totalTime = 0;
        public static double currentVolume;

        public static bool isPlaying = false;
        public static bool isLooped = false;
        public static bool isShuffled = false;
        public static bool isMuted = false;

        static public void Load(Playlist value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("Loading playlist to the Player - playlist is null.");
            }
            else
            {
                CurrentPlaylist = value;
                pauseTime = 0;

                if (value.Tracklist.Count == 1)
                {
                    CurrentTrack = CurrentPlaylist.First();
                    NextTrack = CurrentPlaylist.First();
                    PreviousTrack = CurrentPlaylist.First();
                }
                if (value.Tracklist.Count > 1)
                {
                    CurrentTrack = CurrentPlaylist.First();
                    NextTrack = CurrentPlaylist[1];
                    PreviousTrack = CurrentPlaylist.Last();
                }
            }
        }
        static public void Reload()
        {
            if (CurrentPlaylist.Tracklist.Count > 1)
            {
                CurrentTrack = CurrentPlaylist.First();
                NextTrack = CurrentPlaylist[1];
                PreviousTrack = CurrentPlaylist.Last();
            }
            if (CurrentPlaylist.Tracklist.Count == 1)
            {
                CurrentTrack = CurrentPlaylist.First();
                NextTrack = CurrentPlaylist.First();
                PreviousTrack = CurrentPlaylist.First();
            }
            if (CurrentPlaylist.Tracklist.Count == 0)
            {
                CurrentTrack = null;
                NextTrack = null;
                PreviousTrack = null;
            }
        }

        static public void StartPlaying()
        {
            if (CurrentPlaylist.Tracklist.Count > 0)
            {
                Console.WriteLine(PreviousTrack.TrackTitle);
                Console.WriteLine(CurrentTrack.TrackTitle);
                Console.WriteLine(NextTrack.TrackTitle + "\n");

                player.MediaOpened += new EventHandler(player_MediaOpened);

                player.Open(CurrentTrack.TrackUri);

                player.Position = new TimeSpan(0, 0, 0, 0, (int)pauseTime);
                player.Play();

                //Console.WriteLine();
                Analyzer.GetInfo(CurrentTrack.TrackUri.LocalPath);

                //After ending next song will be playing
                player.MediaEnded += Player_MediaEnded;

                Visualizer.StartVisualization(CurrentTrack.TrackUri.LocalPath);
            }
        }

        private static void player_MediaOpened(object sender, EventArgs e)
        {
            if (player.NaturalDuration.HasTimeSpan == true)
            {
                totalTime = player.NaturalDuration.TimeSpan.TotalMilliseconds;
            }
        }

        private static void Player_MediaEnded(object sender, EventArgs e)
        {
            if (isLooped == true)
            {
                pauseTime = 0;
                StartPlaying();
            }
            else
            {
                Next();
            }
        }

        static public void PlayTrack(Track track)
        {
            if (CurrentPlaylist != null)
            {
                CurrentTrack = track;
                pauseTime = 0;

                if (CurrentPlaylist.Tracklist.Count > 1)
                {
                    if (CurrentTrack == CurrentPlaylist.Last())
                    {
                        PreviousTrack = CurrentPlaylist[CurrentPlaylist.IndexOf(CurrentTrack) - 1];
                        NextTrack = CurrentPlaylist.First();
                    }

                    else
                    {
                        if (CurrentTrack == CurrentPlaylist.First())
                        {
                            PreviousTrack = CurrentPlaylist.Last();
                            NextTrack = CurrentPlaylist[CurrentPlaylist.IndexOf(CurrentTrack) + 1];
                        }
                        else
                        {
                            PreviousTrack = CurrentPlaylist[CurrentPlaylist.IndexOf(CurrentTrack) - 1];
                            NextTrack = CurrentPlaylist[CurrentPlaylist.IndexOf(CurrentTrack) + 1];
                        }
                    }
                }

                Console.WriteLine(PreviousTrack.TrackTitle);
                Console.WriteLine(CurrentTrack.TrackTitle);
                Console.WriteLine(NextTrack.TrackTitle + "\n");

                StartPlaying();
            }
        }

        static public void Pause()
        {
            pauseTime = player.Position.TotalMilliseconds;
            player.Pause();

            Visualizer.PauseCurrentVisualization();
        }

        static public void Stop()
        {
            Pause();

            CurrentPlaylist = new Playlist();
            pauseTime = 0;

            PreviousTrack = new Track();
            CurrentTrack = new Track();
            NextTrack = new Track();

            ((MainWindow)System.Windows.Application.Current.MainWindow).sliderTimer.Stop();
            ((MainWindow)System.Windows.Application.Current.MainWindow).TrackProgressSlider.Value = 0;
            ((MainWindow)System.Windows.Application.Current.MainWindow).CurrentTrackLabel.Content = null;
            ((MainWindow)System.Windows.Application.Current.MainWindow).PlayIcon.Source = new BitmapImage(new Uri(@"D:/Work/C#/Курсовой проект/Icons/play.png", UriKind.RelativeOrAbsolute));

            Visualizer.StopVisualizaton();
        }

        static public void Next()
        {  
            PlayTrack(NextTrack);

            //Selection in playlist
            int index = PlaylistManager.GetActivePlaylist().Tracklist.IndexOf(CurrentTrack);
            PlaylistManager.DrawSelection(index, ((MainWindow)System.Windows.Application.Current.MainWindow).selectedTab);

            //Title
            ((MainWindow)System.Windows.Application.Current.MainWindow).CurrentTrackLabel.Content = CurrentTrack.TrackTitle;
            //Icon
            ((MainWindow)System.Windows.Application.Current.MainWindow).PlayIcon.Source = new BitmapImage(new Uri(@"D:/Work/C#/Курсовой проект/Icons/pause.png", UriKind.RelativeOrAbsolute));
        }

        static public void Previous()
        {
            PlayTrack(PreviousTrack);

            //Selection in playlist
            int index = PlaylistManager.GetActivePlaylist().Tracklist.IndexOf(CurrentTrack);
            PlaylistManager.DrawSelection(index, ((MainWindow)System.Windows.Application.Current.MainWindow).selectedTab);

            //Title
            ((MainWindow)System.Windows.Application.Current.MainWindow).CurrentTrackLabel.Content = CurrentTrack.TrackTitle;
            //Icon
            ((MainWindow)System.Windows.Application.Current.MainWindow).PlayIcon.Source = new BitmapImage(new Uri(@"D:/Work/C#/Курсовой проект/Icons/pause.png", UriKind.RelativeOrAbsolute));
        }

        static public void Current()
        {
            PlayTrack(CurrentTrack);

            //Selection in playlist
            int index = PlaylistManager.GetActivePlaylist().Tracklist.IndexOf(CurrentTrack);
            PlaylistManager.DrawSelection(index, ((MainWindow)System.Windows.Application.Current.MainWindow).selectedTab);

            //Title
            ((MainWindow)System.Windows.Application.Current.MainWindow).CurrentTrackLabel.Content = CurrentTrack.TrackTitle;
            //Icon
            ((MainWindow)System.Windows.Application.Current.MainWindow).PlayIcon.Source = new BitmapImage(new Uri(@"D:/Work/C#/Курсовой проект/Icons/pause.png", UriKind.RelativeOrAbsolute));
        }

        static public void LoopOn()
        {
            isLooped = true;
        }
        static public void LoopOff()
        {
            isLooped = false;
        }

        static public void ShuffleOn()
        {
            isShuffled = true;

            //Saving old playlist
            PrimaryPlaylist = CurrentPlaylist;

            //Reshuffle current playlist
            Random rand = new Random();
            ShuffledPlaylist.Tracklist = CurrentPlaylist.Tracklist.OrderBy(c => rand.Next()).Select(c => c).ToList();

            Pause();

                //Copy shuffled to current
                CurrentPlaylist = ShuffledPlaylist;

            StartPlaying();
        }
        static public void ShuffleOff()
        {
            isShuffled = false;

            //Restore old playlist;
            CurrentPlaylist = PrimaryPlaylist;
        }

        static public void MuteOn()
        {
            isMuted = true;

            Console.WriteLine(player.Volume);

            currentVolume = player.Volume;
            player.Volume = 0;
        }

        static public void MuteOff()
        {
            isMuted = false;

            player.Volume = currentVolume;
        }

        static public void SetVolumeLevel(double level)
        {
            player.Volume = level;
        }
    }
}
