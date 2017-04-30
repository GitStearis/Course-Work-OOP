using System;
using System.Linq;
using System.Windows.Media;

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

        public static bool isPlaying = false;
        public static bool isLooped = false;
        public static bool isShuffled = false;

        static public void Load(Playlist value)
        {
            if (value.Count() > 0)
            {
                CurrentPlaylist = value;
                pauseTime = 0;

                if (value.Count() == 1)
                {

                    CurrentTrack = CurrentPlaylist.First();
                    NextTrack = CurrentPlaylist.First();
                    PreviousTrack = CurrentPlaylist.First();
                }
                if (value.Count() > 1)
                {
                    CurrentTrack = CurrentPlaylist.First();
                    NextTrack = CurrentPlaylist[1];
                    PreviousTrack = CurrentPlaylist.Last();
                }
            }
        }
        static public void Reload()
        {
            if (CurrentPlaylist.Count() > 1)
            {
                CurrentTrack = CurrentPlaylist.First();
                NextTrack = CurrentPlaylist[1];
                PreviousTrack = CurrentPlaylist.Last();
            }
            else
            {
                CurrentTrack = CurrentPlaylist.First();
                NextTrack = CurrentPlaylist.First();
                PreviousTrack = CurrentPlaylist.First();
            }
        }

        static public void StartPlaying()
        {

            Console.WriteLine(PreviousTrack.TrackTitle);
            Console.WriteLine(CurrentTrack.TrackTitle);
            Console.WriteLine(NextTrack.TrackTitle + "\n");

            player.MediaOpened += new EventHandler(player_MediaOpened);

            player.Open(CurrentTrack.TrackUri);
            
            player.Position = new TimeSpan(0, 0, 0, 0, (int)pauseTime);
            player.Play();

            //After ending next song will be playing
            player.MediaEnded += Player_MediaEnded; 
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
            CurrentTrack = track;
            pauseTime = 0;

            if (CurrentPlaylist.Count() > 1)
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

        static public void Pause()
        {
            pauseTime = player.Position.TotalMilliseconds;
            player.Pause();
        }

        static public void Next()
        {  
            PlayTrack(NextTrack);

            int index = PlaylistManager.GetActivePlaylist().Tracklist.IndexOf(CurrentTrack);
            PlaylistManager.DrawSelection(index, ((MainWindow)System.Windows.Application.Current.MainWindow).selectedTab);
        }

        static public void Previous()
        {
            PlayTrack(PreviousTrack);
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

    }
}
