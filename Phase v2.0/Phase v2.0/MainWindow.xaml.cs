using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Phase_v2._0
{
    public partial class MainWindow : Window
    {
        DispatcherTimer sliderTimer = new DispatcherTimer();

        private bool isMenuOpened = false;
        private bool isDragging;

        //TRUE - for default
        //FALSE - for custom
        public bool selectedTab = true;

        public MainWindow()
        {
            InitializeComponent();

            sliderTimer.Interval = TimeSpan.FromMilliseconds(50);
            sliderTimer.Tick += new EventHandler(ChangeSliderPosition);

            DefaultPlaylistBox.MouseDoubleClick += DefaultPlaylist_TrackDoubleClick;
            CustomPlaylistBox.MouseDoubleClick += CustomPlaylist_TrackDoubleClick;

            Player.Load(PlaylistManager.GetActivePlaylist());
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            Main.WindowState = WindowState.Minimized;
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (Player.CurrentPlaylist.Count() > 0)
            {
                Player.isPlaying = true;

                Player.Previous();

                PlaylistManager.DrawSelection(PlaylistManager.GetActivePlaylist().Tracklist.IndexOf(Player.CurrentTrack), selectedTab);

                sliderTimer.Start();
                TrackProgressSlider.Value = 0;

                CurrentTrackLabel.Content = Player.CurrentTrack.TrackTitle;
                PlayIcon.Source = new BitmapImage(new Uri(@"D:/Work/C#/Курсовой проект/Icons/pause.png", UriKind.RelativeOrAbsolute));
            }
        }
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (Player.CurrentPlaylist.Count() > 0)
            {
                if (Player.isPlaying == false)
                {
                    Player.isPlaying = true;

                    Player.StartPlaying();

                    PlaylistManager.DrawSelection(PlaylistManager.GetActivePlaylist().Tracklist.IndexOf(Player.CurrentTrack), selectedTab);

                    CurrentTrackLabel.Content = Player.CurrentTrack.TrackTitle;
                    PlayIcon.Source = new BitmapImage(new Uri(@"D:/Work/C#/Курсовой проект/Icons/pause.png", UriKind.RelativeOrAbsolute));

                    sliderTimer.Start();
                }
                else
                {
                    Player.isPlaying = false;

                    Player.Pause();

                    PlayIcon.Source = new BitmapImage(new Uri(@"D:/Work/C#/Курсовой проект/Icons/play.png", UriKind.RelativeOrAbsolute));

                    sliderTimer.Stop();
                }
            }
        }
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (Player.CurrentPlaylist.Count() > 0)
            {
                Player.isPlaying = true;

                Player.Next();

                PlaylistManager.DrawSelection(PlaylistManager.GetActivePlaylist().Tracklist.IndexOf(Player.CurrentTrack), selectedTab);

                sliderTimer.Start();
                TrackProgressSlider.Value = 0;

                CurrentTrackLabel.Content = Player.CurrentTrack.TrackTitle;
                PlayIcon.Source = new BitmapImage(new Uri(@"D:/Work/C#/Курсовой проект/Icons/pause.png", UriKind.RelativeOrAbsolute));
            }
        }

        private void LoopButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (isMenuOpened == false)
            {
                (sender as Button).ContextMenu.IsEnabled = true;
                (sender as Button).ContextMenu.PlacementTarget = (sender as Button);
                (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                (sender as Button).ContextMenu.IsOpen = true;
                isMenuOpened = true;
            }
            else
            {
                (sender as Button).ContextMenu.IsOpen = false;
                isMenuOpened = false;
            }
        }
        private void OpenTrack_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "MP3 files (*.mp3)|*.mp3|WAV files (*.wav)|*.wav";
            openFileDialog.InitialDirectory = @"d:\";

            //To custom playlist
            PlaylistManager.activePlaylist = false;
            //Firstly clear playlist
            PlaylistManager.ClearActivePlaylist();

            if (openFileDialog.ShowDialog() == true)
            {
                string[] files = openFileDialog.FileNames;
                PlaylistManager.AddTracks(files);
            }

            Player.Load(PlaylistManager.GetActivePlaylist());
        }

        private void PlaylistButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DefaultPlaylistTab_GotFocus(object sender, RoutedEventArgs e)
        {
            PlaylistManager.activePlaylist = true;
            selectedTab = true;

            if (Player.CurrentPlaylist.Count() > 0)
            {
                DefaultPlaylistBox.UnselectAll();
                int index = PlaylistManager.GetActivePlaylist().Tracklist.IndexOf(Player.CurrentTrack);
                if (index >= 0)
                {
                    PlaylistManager.DrawSelection(index, selectedTab);
                }
            }
        }

        private void CustomPlaylistTab_GotFocus(object sender, RoutedEventArgs e)
        {
            PlaylistManager.activePlaylist = false;
            selectedTab = false;

            if (Player.CurrentPlaylist.Count() > 0)
            {
                DefaultPlaylistBox.UnselectAll();
                int index = PlaylistManager.GetActivePlaylist().Tracklist.IndexOf(Player.CurrentTrack);
                if (index >= 0)
                {
                    PlaylistManager.DrawSelection(index, selectedTab);
                }
            }
        }

        private void DefaultPlaylist_TrackDoubleClick(Object sender, MouseButtonEventArgs e)
        {
            Player.Load(PlaylistManager.GetActivePlaylist());

            DependencyObject obj = (DependencyObject)e.OriginalSource;

            while (obj != null && obj != DefaultPlaylistBox)
            {
                if (obj.GetType() == typeof(ListBoxItem))
                {
                    Player.isPlaying = true;
                    Player.PlayTrack(Player.CurrentPlaylist[DefaultPlaylistBox.Items.IndexOf(DefaultPlaylistBox.SelectedItem)]);

                    CurrentTrackLabel.Content = Player.CurrentTrack.TrackTitle;
                    PlayIcon.Source = new BitmapImage(new Uri(@"D:/Work/C#/Курсовой проект/Icons/pause.png", UriKind.RelativeOrAbsolute));

                    break;
                }
                obj = VisualTreeHelper.GetParent(obj);
            }

            sliderTimer.Start();
            TrackProgressSlider.Value = 0;
        }
        private void CustomPlaylist_TrackDoubleClick(Object sender, MouseButtonEventArgs e)
        {
            Player.Load(PlaylistManager.GetActivePlaylist());

            DependencyObject obj = (DependencyObject)e.OriginalSource;

            while (obj != null && obj != CustomPlaylistBox)
            {
                if (obj.GetType() == typeof(ListBoxItem))
                {
                    Player.isPlaying = true;
                    Player.PlayTrack(Player.CurrentPlaylist[CustomPlaylistBox.Items.IndexOf(CustomPlaylistBox.SelectedItem)]);

                    CurrentTrackLabel.Content = Player.CurrentTrack.TrackTitle;
                    PlayIcon.Source = new BitmapImage(new Uri(@"D:/Work/C#/Курсовой проект/Icons/pause.png", UriKind.RelativeOrAbsolute));

                    break;
                }
                obj = VisualTreeHelper.GetParent(obj);
            }

            sliderTimer.Start();
            TrackProgressSlider.Value = 0;
        }

        private void DefaultPlaylistDrop(object sender, DragEventArgs e)
        {
            PlaylistManager.activePlaylist = true;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] chosenFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
                
                PlaylistManager.AddTracks(chosenFiles);
            }

            Player.Load(PlaylistManager.GetActivePlaylist());
        }
        private void CustomPlaylistDrop(object sender, DragEventArgs e)
        {
            PlaylistManager.activePlaylist = false;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] chosenFiles = (string[])e.Data.GetData(DataFormats.FileDrop);

                PlaylistManager.AddTracks(chosenFiles);
            }

            Player.Load(PlaylistManager.GetActivePlaylist());
        }

        private void RefreshBox()
        {
            
        }
        private void AddTrackButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "MP3 files (*.mp3)|*.mp3|WAV files (*.wav)|*.wav";
            openFileDialog.InitialDirectory = @"d:\";

            if (openFileDialog.ShowDialog() == true)
            {
                string[] files = openFileDialog.FileNames;
                PlaylistManager.AddTracks(files);
            }

            Player.Load(PlaylistManager.GetActivePlaylist());
        }

        private void RemoveTrackButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void SaveCurrentPlaylistButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ChangeSliderPosition(object sender, EventArgs e)
        {
            if (!isDragging)
            {
                if (Player.player.NaturalDuration.HasTimeSpan == true)
                {
                    TrackProgressSlider.Value = Player.player.Position.TotalMilliseconds * 1000 / Player.player.NaturalDuration.TimeSpan.TotalMilliseconds;
                }
            }
        }

        private void TrackProgressSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            isDragging = true;
        }

        private void TrackProgressSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            isDragging = false;

            if (Player.CurrentPlaylist.Count() > 0)
            {
                Player.pauseTime = Player.totalTime * TrackProgressSlider.Value / 1000;
                Player.StartPlaying();
            }
        }

    }
}
