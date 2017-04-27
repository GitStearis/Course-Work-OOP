using System.Windows.Threading;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.ComponentModel;
using System.Threading;
using Microsoft.Win32;

namespace Phase_v1._0
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private Playlist defaultPlaylist = new Playlist();
        private Playlist customPlaylist = new Playlist();
        private bool isPlaying;

        private void Drawer(object sender, EventArgs e)
        {
            var window = sender as MainWindow;

            Random rnd = new Random();
            Random rnd1 = new Random();

            Color currentColor = RandomColor.Generate(60);

            CubesForms.Cross(rnd.Next(CubesContainer.maxCubesX), rnd.Next(CubesContainer.maxCubesY), currentColor);
            CubesForms.Cross(rnd.Next(CubesContainer.maxCubesX), rnd.Next(CubesContainer.maxCubesY), currentColor);

            CubesForms.Circle(rnd.Next(CubesContainer.maxCubesX), rnd.Next(CubesContainer.maxCubesY), currentColor);

        }
        private void StartDraw()
        {
            if (isPlaying == true)
            {
                timer = new DispatcherTimer();
                timer.Tick += new EventHandler(Drawer);
                timer.Tick += new EventHandler(Drawer);
                timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
                timer.Start();
            }
        }
        private void StopDraw()
        {
            if (isPlaying == false)
            {
                timer.Tick -= new EventHandler(Drawer);
                timer.Tick -= new EventHandler(Drawer);
                timer.Stop();
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            CubesContainer.InitializeField(Pad);
            CubesContainer.SetRandomly();

            Track test = new Track("D:/Music/BritIndyAlternative/The XX - Crystalised.mp3");
            Track test2 = new Track("D:/Music/BritIndyAlternative/Jake Bugg - Lightning Bolt.mp3");
            Track test3 = new Track("D:/Music/BritIndyAlternative/Kasabian - Club Foot.mp3");
            Track test4 = new Track("D:/Music/BritIndyAlternative/White Stripes - Seven Nation Army.mp3");
            defaultPlaylist.tracklist.Add(test);
            defaultPlaylist.tracklist.Add(test2);
            defaultPlaylist.tracklist.Add(test3);
            defaultPlaylist.tracklist.Add(test4);

            foreach(var track in defaultPlaylist.tracklist)
            {
                CurrentPlaylistBox.Items.Add(track);
            }

            Player.Load(defaultPlaylist);

            isPlaying = false;

            CurrentPlaylistBox.MouseDoubleClick += Playlist_TrackDoubleClick;
            TrackProgressSlider.ValueChanged += TrackProgressSlider_ValueChanged;
            //TODO: Slider animation
            //TODO: Slider style
        }

            private void MouseClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point currentPoint = new Point();

            if (e.ButtonState == MouseButtonState.Pressed)
                currentPoint = e.GetPosition(this);

            if ((int)currentPoint.X >= 0 && (int)currentPoint.Y >= 0)
            {
                int xIndex = (int)currentPoint.X / 20;
                int yIndex = (int)currentPoint.Y / 20;

                Random rand = new Random();

                Color col = RandomColor.Generate(80);

                CubesForms.HorizontalLine(yIndex, col);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            Main.WindowState = WindowState.Minimized;
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            Player.Previous();
            PlayIcon.Source = new BitmapImage(new Uri(@"D:/Work/C#/Курсовой проект/Icons/pause.png", UriKind.RelativeOrAbsolute));
            isPlaying = true;
            TrackProgressSlider.Value = 0;

            Console.WriteLine(CurrentPlaylistBox.Items.IndexOf(Player.CurrentTrack));
            CurrentPlaylistBox.SelectedIndex = CurrentPlaylistBox.Items.IndexOf(Player.CurrentTrack);
            CurrentTrackLabel.Content = Player.CurrentTrack.TrackTitle;

            StartDraw();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(CurrentPlaylistBox.Items.IndexOf(Player.CurrentTrack));
            CurrentPlaylistBox.SelectedIndex = CurrentPlaylistBox.Items.IndexOf(Player.CurrentTrack);
            CurrentTrackLabel.Content = Player.CurrentTrack.TrackTitle;

            if (isPlaying == false)
            {
                Player.Play();
                PlayIcon.Source = new BitmapImage(new Uri(@"D:/Work/C#/Курсовой проект/Icons/pause.png", UriKind.RelativeOrAbsolute));
                isPlaying = true;
                TrackProgressSlider.Value = 0;

                StartDraw();
            }
            else
            {
                Player.Pause();
                PlayIcon.Source = new BitmapImage(new Uri(@"D:/Work/C#/Курсовой проект/Icons/play.png", UriKind.RelativeOrAbsolute));
                isPlaying = false;

                StopDraw();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            Player.Next();
            PlayIcon.Source = new BitmapImage(new Uri(@"D:/Work/C#/Курсовой проект/Icons/pause.png", UriKind.RelativeOrAbsolute));
            isPlaying = true;
            TrackProgressSlider.Value = 0;

            CurrentPlaylistBox.SelectedIndex = CurrentPlaylistBox.Items.IndexOf(Player.CurrentTrack);
            CurrentTrackLabel.Content = Player.CurrentTrack.TrackTitle;

            StartDraw();
        }

        private void LoopButton_Click(object sender, RoutedEventArgs e)
        {
            if (Player.isLooped == false)
            {
                Player.LoopOn();
                LoopButton.Background = Brushes.White;
            }
            else
            {
                Player.LoopOff();
                LoopButton.Background = new SolidColorBrush(Color.FromArgb(153, 255, 255, 255));
            }
        }

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            if (Player.isShuffled == false)
            {
                Player.ShuffleOn();
                ShuffleButton.Background = Brushes.White;
            }
            else
            {
                Player.ShuffleOff();
                ShuffleButton.Background = new SolidColorBrush(Color.FromArgb(153, 255, 255, 255));
            }
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.IsEnabled = true;
            (sender as Button).ContextMenu.PlacementTarget = (sender as Button);
            (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            (sender as Button).ContextMenu.IsOpen = true;
        }

        private void PlaylistButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TrackProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Player.currentTime = Player.totalTime * TrackProgressSlider.Value / 100;
            Player.Play();
        }

        private void Playlist_TrackDoubleClick(Object sender, MouseButtonEventArgs e)
        {
            DependencyObject obj = (DependencyObject)e.OriginalSource;

            while (obj != null && obj != CurrentPlaylistBox)
            {
                if (obj.GetType() == typeof(ListBoxItem))
                {
                    //Plays selected track
                    Player.Play(Player.CurrentPlaylist.tracklist[CurrentPlaylistBox.Items.IndexOf(CurrentPlaylistBox.SelectedItem)]);
                    PlayIcon.Source = new BitmapImage(new Uri(@"D:/Work/C#/Курсовой проект/Icons/pause.png", UriKind.RelativeOrAbsolute));
                    isPlaying = true;

                    StartDraw();
                    break;
                }
                obj = VisualTreeHelper.GetParent(obj);
            }
        }

        private void AddToPlaylistBox(string[] files)
        {
            foreach (var file in files)
            {
                Console.WriteLine("Added " + file);
                //Create track
                Track temp = new Track(file);
                //Adding to default playlist
                defaultPlaylist.tracklist.Add(temp);
                //Drawing on the playlist box
                CurrentPlaylistBox.Items.Add(temp);
                //Loading playlist to player
            }
            Player.Load(defaultPlaylist);
        }

        private void DefaultPlaylistDrop(object sender, DragEventArgs e)
        {
            //NOTE!
            //Files could be not only music types!
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                AddToPlaylistBox(files);
            }
        }

        private void RefreshBox()
        {
            CurrentPlaylistBox.Items.Clear();

            foreach (var track in defaultPlaylist.tracklist)
            {
                CurrentPlaylistBox.Items.Add(track);
            }
        }
        private void AddTrackButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "MP3 files (*.mp3)|*.mp3|WAV files (*.wav)|*.wav";
            openFileDialog.InitialDirectory = @"d:\";

            if(openFileDialog.ShowDialog() == true)
            {
                string[] files = openFileDialog.FileNames;

                AddToPlaylistBox(files);
            }
        }

        private void RemoveTrackButton_Click(object sender, RoutedEventArgs e)
        {
            Track selected = Player.CurrentPlaylist.tracklist[CurrentPlaylistBox.Items.IndexOf(CurrentPlaylistBox.SelectedItem)];

            Console.WriteLine(selected);
            CurrentPlaylistBox.Items.RemoveAt(defaultPlaylist.tracklist.IndexOf(selected));
            Player.CurrentPlaylist.tracklist.Remove(selected);

            RefreshBox();


            //SEPARATE THIS FROM BUTTON
            Player.Next();
            PlayIcon.Source = new BitmapImage(new Uri(@"D:/Work/C#/Курсовой проект/Icons/pause.png", UriKind.RelativeOrAbsolute));
            isPlaying = true;
            TrackProgressSlider.Value = 0;

            StartDraw();
        }

        private void SaveCurrentPlaylistButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
