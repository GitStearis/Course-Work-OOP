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
        private bool isDraggingPosition;

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

            Player.SetVolumeLevel(1);

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

                sliderTimer.Start();
                TrackProgressSlider.Value = 0;
            }
        }
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (Player.CurrentPlaylist != null)
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
        }
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (Player.CurrentPlaylist.Count() > 0)
            {
                Player.isPlaying = true;

                Player.Next();

                sliderTimer.Start();
                TrackProgressSlider.Value = 0;
            }
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

        //TOP MENU
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

            PlaylistManager.activePlaylist = false;
            selectedTab = false;
            CustomPlaylistTab.Focus();
        }
        private void OpenPlaylist_Click(object sender, RoutedEventArgs e)
        {
            PlaylistManager.OpenPlaylist();
        }


        private void PlaylistButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DefaultPlaylistTab_GotFocus(object sender, RoutedEventArgs e)
        {
            selectedTab = true;

            if (Player.CurrentPlaylist != null)
            {
                if (Player.CurrentPlaylist.Count() > 0)
                {

                    int index = PlaylistManager.GetActivePlaylist().Tracklist.IndexOf(Player.CurrentTrack);
                    if (index >= 0)
                    {
                        PlaylistManager.DrawSelection(index, selectedTab);
                    }
                }
            }
        }

        private void CustomPlaylistTab_GotFocus(object sender, RoutedEventArgs e)
        {
            selectedTab = false;

            if (Player.CurrentPlaylist != null)
            {
                if (Player.CurrentPlaylist.Count() > 0)
                {
                    int index = PlaylistManager.GetActivePlaylist().Tracklist.IndexOf(Player.CurrentTrack);
                    if (index >= 0)
                    {
                        PlaylistManager.DrawSelection(index, selectedTab);
                    }
                }
            }
        }

        private void DefaultPlaylist_TrackDoubleClick(Object sender, MouseButtonEventArgs e)
        {
            selectedTab = true;
            PlaylistManager.activePlaylist = true;

            Player.Load(PlaylistManager.defaultPlaylist);

            DependencyObject obj = (DependencyObject)e.OriginalSource;

            if (Player.CurrentPlaylist != null)
            {
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
            }

            sliderTimer.Start();
            TrackProgressSlider.Value = 0;
        }
        private void CustomPlaylist_TrackDoubleClick(Object sender, MouseButtonEventArgs e)
        {
            selectedTab = false;
            PlaylistManager.activePlaylist = false;

            Player.Load(PlaylistManager.customPlaylist);

            DependencyObject obj = (DependencyObject)e.OriginalSource;

            if (Player.CurrentPlaylist != null)
            {
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
            if (selectedTab == true)
            {
                if (DefaultPlaylistBox.SelectedItem != null)
                {
                    //Works not well with loading playlist. Fix it

                    Track selectedTrack = PlaylistManager.defaultPlaylist.Tracklist[DefaultPlaylistBox.Items.IndexOf(DefaultPlaylistBox.SelectedItem)];
                    PlaylistManager.RemoveTrack(selectedTab, selectedTrack);
                }
            }
            else
            {
                if (CustomPlaylistBox.SelectedItem != null)
                {

                    Track selectedTrack = PlaylistManager.customPlaylist.Tracklist[CustomPlaylistBox.Items.IndexOf(CustomPlaylistBox.SelectedItem)];
                    PlaylistManager.RemoveTrack(selectedTab, selectedTrack);
                }
            }
        }

        private void SaveCurrentPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            PlaylistManager.SaveSelectedPlaylist(selectedTab);
        }

        private void ChangeSliderPosition(object sender, EventArgs e)
        {
            if (!isDraggingPosition)
            {
                if (Player.player.NaturalDuration.HasTimeSpan == true)
                {
                    TrackProgressSlider.Value = Player.player.Position.TotalMilliseconds * 1000 / Player.player.NaturalDuration.TimeSpan.TotalMilliseconds;
                }
            }
        }

        private void TrackProgressSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            isDraggingPosition = true;
        }

        private void TrackProgressSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            isDraggingPosition = false;

            if (Player.CurrentPlaylist.Count() > 0)
            {
                Player.pauseTime = Player.totalTime * TrackProgressSlider.Value / 1000;
                Player.StartPlaying();
            }
        }

        private void MuteButton_Click(object sender, RoutedEventArgs e)
        {
            if (Player.isMuted == false)
            {
                Player.MuteOn();
                MuteButton.Background = Brushes.White;
            }
            else
            {
                Player.MuteOff();
                MuteButton.Background = new SolidColorBrush(Color.FromArgb(153, 255, 255, 255));
            }
        }

        private void VolumeLevelSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            Player.MuteOff();
            MuteButton.Background = new SolidColorBrush(Color.FromArgb(153, 255, 255, 255));
            Player.SetVolumeLevel(VolumeLevelSlider.Value / 100);
        }

    }
}
