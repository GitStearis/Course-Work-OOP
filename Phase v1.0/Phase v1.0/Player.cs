using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace Phase_v1._0
{

    class Playlist
    {
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set => name = value;
        }

        public List<Track> tracklist = new List<Track>();

        public override string ToString()
        {
            string result = "\n";
            foreach (var track in tracklist)
            {
                result += tracklist.IndexOf(track) + track.TrackTitle + "\n";
            }
            return result;
        }
    }


    //RETURN TO PLAYER
    static class Memorizer
    {
        private static Playlist primaryPlaylist = new Playlist();

        public static void Remember(Playlist data)
        {
            primaryPlaylist = data;
        }

        public static Playlist Show()
        {
            return primaryPlaylist;
        }
    }

    static class Player
    {
        public static MediaPlayer player = new MediaPlayer();

        private static Playlist currentPlaylist = new Playlist();
        internal static Playlist CurrentPlaylist { get => currentPlaylist; set => currentPlaylist = value; }
        private static Playlist shuffledPlaylist = new Playlist();
        internal static Playlist ShuffledPlaylist { get => shuffledPlaylist; set => shuffledPlaylist = value; }

        private static Track previousTrack;
        public static Track PreviousTrack { get => previousTrack; set => previousTrack = value; }
        private static Track currentTrack;
        public static Track CurrentTrack { get => currentTrack; set => currentTrack = value; }
        private static Track nextTrack;
        public static Track NextTrack { get => nextTrack; set => nextTrack = value; }

        public static double currentTime;
        public static double totalTime = 0;

        public static bool isLooped = false;
        public static bool isShuffled = false;

        static public void Load(Playlist value)
        {
            CurrentPlaylist = value;
            if (CurrentPlaylist.tracklist.Count > 1)
            {
                CurrentTrack = CurrentPlaylist.tracklist.First();
                NextTrack = CurrentPlaylist.tracklist[1];
                PreviousTrack = CurrentPlaylist.tracklist.Last();
            }
            else
            {
                CurrentTrack = CurrentPlaylist.tracklist.First();
                NextTrack = CurrentPlaylist.tracklist.First();
                PreviousTrack = CurrentPlaylist.tracklist.First();
            }
        }

        static public void Play()
        {
            player.MediaOpened += new EventHandler(player_MediaOpened);

            player.Open(CurrentTrack.TrackUri);
            
            player.Position = new TimeSpan(0, 0, 0, 0, (int)currentTime);
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
                currentTime = 0;
                Play();
            }
            else
            {
                Next();
            }
        }

        static public void Play(Track track)
        {
            CurrentTrack = track;
            currentTime = 0;

            Console.WriteLine(CurrentTrack.TrackTitle);

            if (CurrentPlaylist.tracklist.Count > 1)
            {
                if (CurrentTrack == CurrentPlaylist.tracklist.Last())
                {
                    NextTrack = CurrentPlaylist.tracklist.First();
                    PreviousTrack = CurrentPlaylist.tracklist[CurrentPlaylist.tracklist.IndexOf(CurrentTrack) - 1];
                }

                else
                {
                    if (CurrentTrack == CurrentPlaylist.tracklist.First())
                    {
                        PreviousTrack = CurrentPlaylist.tracklist.Last();
                        NextTrack = CurrentPlaylist.tracklist[CurrentPlaylist.tracklist.IndexOf(track) + 1];
                    }
                    else
                    {
                        PreviousTrack = CurrentPlaylist.tracklist[CurrentPlaylist.tracklist.IndexOf(track) - 1];
                        NextTrack = CurrentPlaylist.tracklist[CurrentPlaylist.tracklist.IndexOf(track) + 1];
                    }
                }
            }
            Play();
        }

        static public void Pause()
        {
            try
            {
                currentTime = player.Position.TotalMilliseconds;
                player.Pause();
            }
            catch (System.Exception error)
            {
                Console.WriteLine(error.Message);
            }
        }

        static public void Next()
        {
            PreviousTrack = CurrentTrack;
            currentTime = 0;

            if (CurrentPlaylist.tracklist.Count > 1)
            {
                if (CurrentTrack == CurrentPlaylist.tracklist.Last())
                {
                    CurrentTrack = CurrentPlaylist.tracklist.First();
                    NextTrack = CurrentPlaylist.tracklist[CurrentPlaylist.tracklist.IndexOf(CurrentTrack) + 1];
                }
                else
                {
                    if (CurrentTrack == CurrentPlaylist.tracklist[CurrentPlaylist.tracklist.Count - 2])
                    {
                        CurrentTrack = CurrentPlaylist.tracklist.Last();
                        NextTrack = CurrentPlaylist.tracklist.First();
                    }
                    else
                    {
                        CurrentTrack = NextTrack;
                        NextTrack = CurrentPlaylist.tracklist[CurrentPlaylist.tracklist.IndexOf(CurrentTrack) + 1];
                    }
                }
            }

            Play();
        }

        static public void Previous()
        {
            NextTrack = CurrentTrack;
            currentTime = 0;

            if (CurrentPlaylist.tracklist.Count > 1)
            {
                if (CurrentTrack == CurrentPlaylist.tracklist.First())
                {
                    CurrentTrack = CurrentPlaylist.tracklist.Last();
                    PreviousTrack = CurrentPlaylist.tracklist[CurrentPlaylist.tracklist.IndexOf(CurrentTrack) - 1];
                }
                else
                {
                    if (CurrentTrack == CurrentPlaylist.tracklist[1])
                    {
                        CurrentTrack = CurrentPlaylist.tracklist.First();
                        PreviousTrack = CurrentPlaylist.tracklist.Last();
                    }
                    else
                    {
                        CurrentTrack = PreviousTrack;
                        PreviousTrack = CurrentPlaylist.tracklist[CurrentPlaylist.tracklist.IndexOf(CurrentTrack) - 1];
                    }
                }
            }
            Play();
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
            Memorizer.Remember(CurrentPlaylist);

            //Reshuffle current playlist
            Random rand = new Random();
            ShuffledPlaylist.tracklist = CurrentPlaylist.tracklist.OrderBy(c => rand.Next()).Select(c => c).ToList();

            //Copy shuffled to current
            CurrentPlaylist = ShuffledPlaylist;

            Play(CurrentTrack);
        }
        static public void ShuffleOff()
        {
            isShuffled = false;

            //Restore old playlist;
            CurrentPlaylist = Memorizer.Show();
        }

    }
}
