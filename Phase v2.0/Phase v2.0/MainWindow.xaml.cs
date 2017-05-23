using Microsoft.Win32;
using System;
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
        public DispatcherTimer sliderTimer = new DispatcherTimer();
        public DispatcherTimer labelTimer = new DispatcherTimer();

        private bool isTopMenuOpened = false;
        private bool isVisualizationMenuOpened = false;
        private bool isDraggingPosition;
        private bool isClickedPosition = false;

        //TRUE - for default
        //FALSE - for custom
        public bool selectedTab = true;

        public MainWindow()
        {
            InitializeComponent();

            sliderTimer.Interval = TimeSpan.FromMilliseconds(50);
            sliderTimer.Tick += new EventHandler(ChangeSliderPosition);

            labelTimer.Interval = TimeSpan.FromMilliseconds(50);
            labelTimer.Tick += new EventHandler(RedrawLabel);
            labelTimer.Start();

            DefaultPlaylistBox.MouseDoubleClick += DefaultPlaylist_TrackDoubleClick;
            CustomPlaylistBox.MouseDoubleClick += CustomPlaylist_TrackDoubleClick;

            VolumeLevelSlider.ValueChanged += VolumeLevelSlider_Click;
            Player.SetVolumeLevel(1);

            Player.Load(PlaylistManager.GetActivePlaylist());

            CubesContainer.InitializeField(Pad);
            CubesContainer.SetRandomly();
        }

        private void RedrawLabel(object sender, EventArgs e)
        {
            string result = null;
            string hour = Player.player.Position.Hours.ToString();
            string minute = Player.player.Position.Minutes.ToString();
            string second = Player.player.Position.Seconds.ToString();
            string msecond = (Player.player.Position.Milliseconds % 10).ToString();

            result = hour + ":" + minute + ":" + second + ":" + msecond;

            CurrentTimeLabel.Content = result;
        }

        //WINDOW CONTROL BUTTONS
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            Main.WindowState = WindowState.Minimized;
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        //PLAYER BUTTONS
        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (Player.CurrentPlaylist.Tracklist.Count > 0)
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
                if (Player.CurrentPlaylist.Tracklist.Count > 0)
                {
                    if (Player.isPlaying == false)
                    {
                        Player.isPlaying = true;

                        Player.StartPlaying();

                        PlaylistManager.DrawSelection(PlaylistManager.GetActivePlaylist().Tracklist.IndexOf(Player.CurrentTrack), selectedTab);

                        PreviousTrackLabel.Text = Player.PreviousTrack.TrackTitle;
                        CurrentTrackLabel.Text = Player.CurrentTrack.TrackTitle;
                        NextTrackLabel.Text = Player.NextTrack.TrackTitle;

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
            if (Player.CurrentPlaylist.Tracklist.Count > 0)
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

        //VISUALIZATION
        private void VisualizationButton_Click(object sender, RoutedEventArgs e)
        {
            if (isVisualizationMenuOpened == false)
            {
                (sender as Button).ContextMenu.IsEnabled = true;
                (sender as Button).ContextMenu.PlacementTarget = (sender as Button);
                (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                (sender as Button).ContextMenu.IsOpen = true;
                isVisualizationMenuOpened = true;
            }
            else
            {
                (sender as Button).ContextMenu.IsOpen = false;
                isVisualizationMenuOpened = false;
            }
        }

        //TOP MENU
        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (isTopMenuOpened == false)
            {
                (sender as Button).ContextMenu.IsEnabled = true;
                (sender as Button).ContextMenu.PlacementTarget = (sender as Button);
                (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                (sender as Button).ContextMenu.IsOpen = true;
                isTopMenuOpened = true;
            }
            else
            {
                (sender as Button).ContextMenu.IsOpen = false;
                isTopMenuOpened = false;
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

            //Firstly focus the custom tab
            CustomPlaylistTab.Focus();

            //Then add files
            if (openFileDialog.ShowDialog() == true)
            {
                string[] files = openFileDialog.FileNames;
                PlaylistManager.AddTracks(files);
            }

            Player.Load(PlaylistManager.GetActivePlaylist());

            PlaylistManager.activePlaylist = false;
            selectedTab = false;

            //Close menu
            if ((sender as Button) != null)
            {
                (sender as Button).ContextMenu.IsOpen = false;
            }
            isTopMenuOpened = false;
        }

        private void OpenPlaylist_Click(object sender, RoutedEventArgs e)
        {
            PlaylistManager.OpenPlaylist();

            //Close menu
            if ((sender as Button) != null)
            {
                (sender as Button).ContextMenu.IsOpen = false;
            }
            isTopMenuOpened = false;
        }

        private void SavePlaylistAs_Click(object sender, RoutedEventArgs e)
        {
            PlaylistManager.SaveSelectedPlaylist(selectedTab);
        }

        //PLAYLIST SECTION
        private void DefaultPlaylistTab_GotFocus(object sender, RoutedEventArgs e)
        {
            selectedTab = true;

            if (Player.CurrentPlaylist != null)
            {
                if (Player.CurrentPlaylist.Tracklist.Count > 0)
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
                if (Player.CurrentPlaylist.Tracklist.Count > 0)
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

                        PreviousTrackLabel.Text = Player.PreviousTrack.TrackTitle;
                        CurrentTrackLabel.Text = Player.CurrentTrack.TrackTitle;
                        NextTrackLabel.Text = Player.NextTrack.TrackTitle;

                        PlayIcon.Source = new BitmapImage(new Uri(@"D:/Work/C#/Курсовой проект/Icons/pause.png", UriKind.RelativeOrAbsolute));

                        Player.PlayTrack(Player.CurrentPlaylist[DefaultPlaylistBox.Items.IndexOf(DefaultPlaylistBox.SelectedItem)]);

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

                        PreviousTrackLabel.Text = Player.PreviousTrack.TrackTitle;
                        CurrentTrackLabel.Text = Player.CurrentTrack.TrackTitle;
                        NextTrackLabel.Text = Player.NextTrack.TrackTitle;

                        PlayIcon.Source = new BitmapImage(new Uri(@"D:/Work/C#/Курсовой проект/Icons/pause.png", UriKind.RelativeOrAbsolute));

                        Player.PlayTrack(Player.CurrentPlaylist[CustomPlaylistBox.Items.IndexOf(CustomPlaylistBox.SelectedItem)]);

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


        //BOTTOM BUTTONS
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

        private void ClearCurrentPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            PlaylistManager.Clear(selectedTab);
        }

        private void SaveCurrentPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            PlaylistManager.SaveSelectedPlaylist(selectedTab);
        }

        private void LoadPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            PlaylistManager.OpenPlaylist();
        }


        //POSITION SLIDER
        private void ChangeSliderPosition(object sender, EventArgs e)
        {
            if (!isDraggingPosition && !isClickedPosition)
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
            Visualizer.PauseCurrentVisualization();
        }

        private void TrackProgressSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            isDraggingPosition = false;

            if (Player.CurrentPlaylist.Tracklist.Count > 0)
            {
                Player.pauseTime = Player.totalTime * TrackProgressSlider.Value / 1000;
                Player.StartPlaying();
            }
        }


        //VOLUME
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


        //VOLUME SLIDER
        private void VolumeLevelSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            Player.MuteOff();
            MuteButton.Background = new SolidColorBrush(Color.FromArgb(153, 255, 255, 255));
            Player.SetVolumeLevel(VolumeLevelSlider.Value / 100);
        }

        private void VolumeLevelSlider_Click(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Player.MuteOff();
            MuteButton.Background = new SolidColorBrush(Color.FromArgb(153, 255, 255, 255));
            Player.SetVolumeLevel(VolumeLevelSlider.Value / 100);
        }
    }
}
