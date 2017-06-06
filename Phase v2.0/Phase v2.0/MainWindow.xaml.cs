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

        //4 - Wave visualization
        public int mode = 4;

        //TRUE - for default
        //FALSE - for custom
        public bool selectedTab = true;

        public MainWindow()
        {
            try
            {
                InitializeComponent();

                Player.SetVolumeLevel(1);

                Restorer.LoadLastSession();

                sliderTimer.Interval = TimeSpan.FromMilliseconds(50);
                sliderTimer.Tick += new EventHandler(ChangeSliderPosition);

                labelTimer.Interval = TimeSpan.FromMilliseconds(500);
                labelTimer.Tick += new EventHandler(RedrawLabel);
                labelTimer.Start();

                DefaultPlaylistBox.MouseDoubleClick += DefaultPlaylist_TrackDoubleClick;
                CustomPlaylistBox.MouseDoubleClick += CustomPlaylist_TrackDoubleClick;

                VolumeLevelSlider.ValueChanged += VolumeLevelSlider_Click;

                Player.Load(PlaylistManager.GetActivePlaylist());

                CubesContainer.InitializeField(Pad);
                CubesContainer.SetRandomly();
            }
            catch
            {

            }
        }

        private void RedrawLabel(object sender, EventArgs e)
        {
            try
            {
                string result = null;
                string hour = Player.player.Position.Hours.ToString();
                string minute = Player.player.Position.Minutes.ToString();
                string second = Player.player.Position.Seconds.ToString();

                result = hour + ":" + minute + ":" + second;

                CurrentTimeLabel.Content = result;
            }
            catch
            {

            }
        }

        //WINDOW CONTROL BUTTONS
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            Main.WindowState = WindowState.Minimized;
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Restorer.SaveCurrentSession();
                Close();
            }
            catch
            {

            }
        }


        //PLAYER BUTTONS
        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Player.CurrentPlaylist.Tracklist.Count > 0)
                {
                    Player.isPlaying = true;

                    Player.Previous();

                    sliderTimer.Start();
                    TrackProgressSlider.Value = 0;
                }
            }
            catch
            {

            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Player.CurrentPlaylist != null)
                {
                    if (Player.CurrentPlaylist.Tracklist.Count > 0)
                    {
                        if (Player.isPlaying == false)
                        {
                            Player.Continue();
                        }
                        else
                        {
                            Player.Pause();
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Player.CurrentPlaylist.Tracklist.Count > 0)
                {
                    Player.isPlaying = true;

                    Player.Next();

                    sliderTimer.Start();
                    TrackProgressSlider.Value = 0;
                }
            }
            catch
            {

            }
        }

        private void LoopButton_Click(object sender, RoutedEventArgs e)
        {
            try
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
            catch
            {

            }
        }

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            try
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
            catch
            {

            }
        }

        //VISUALIZATION
        private void VisualizationButton_Click(object sender, RoutedEventArgs e)
        {
            try
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
            catch
            {

            }
        }

        //TOP MENU
        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            try
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
            catch
            {

            }
        }

        private void OpenTrack_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = true;
                openFileDialog.Title = "Open file";
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
            catch
            {

            }
        }

        private void OpenPlaylist_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PlaylistManager.OpenPlaylistDialog();

                //Close menu
                if ((sender as Button) != null)
                {
                    (sender as Button).ContextMenu.IsOpen = false;
                }
                isTopMenuOpened = false;
            }
            catch
            {

            }
        }

        private void SavePlaylistAs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PlaylistManager.SaveSelectedPlaylist(selectedTab);
            }
            catch
            {

            }
        }

        //PLAYLIST SECTION
        private void DefaultPlaylistTab_GotFocus(object sender, RoutedEventArgs e)
        {
            try
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
            catch
            {

            }
        }

        private void CustomPlaylistTab_GotFocus(object sender, RoutedEventArgs e)
        {
            try
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
            catch
            {

            }
        }

        private void DefaultPlaylist_TrackDoubleClick(Object sender, MouseButtonEventArgs e)
        {
            try
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
            catch
            {

            }
        }

        private void CustomPlaylist_TrackDoubleClick(Object sender, MouseButtonEventArgs e)
        {
            try
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
            catch
            {

            }
        }

        private void DefaultPlaylistDrop(object sender, DragEventArgs e)
        {
            try
            {
                PlaylistManager.activePlaylist = true;

                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] chosenFiles = (string[])e.Data.GetData(DataFormats.FileDrop);

                    PlaylistManager.AddTracks(chosenFiles);
                }

                Player.Load(PlaylistManager.GetActivePlaylist());
            }
            catch
            {

            }
        }

        private void CustomPlaylistDrop(object sender, DragEventArgs e)
        {
            try
            {
                PlaylistManager.activePlaylist = false;

                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] chosenFiles = (string[])e.Data.GetData(DataFormats.FileDrop);

                    PlaylistManager.AddTracks(chosenFiles);
                }

                Player.Load(PlaylistManager.GetActivePlaylist());
            }
            catch
            {

            }
        }


        //BOTTOM BUTTONS
        private void AddTrackButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = true;
                openFileDialog.Title = "Open file";
                openFileDialog.Filter = "MP3 files (*.mp3)|*.mp3|WAV files (*.wav)|*.wav";
                openFileDialog.InitialDirectory = @"d:\";

                if (openFileDialog.ShowDialog() == true)
                {
                    string[] files = openFileDialog.FileNames;
                    PlaylistManager.AddTracks(files);
                }

                Player.Load(PlaylistManager.GetActivePlaylist());
            }
            catch
            {

            }
        }

        private void RemoveTrackButton_Click(object sender, RoutedEventArgs e)
        {
            try
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
            catch
            {

            }
        }

        private void ClearCurrentPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PlaylistManager.Clear(selectedTab);
            }
            catch
            {

            }
        }

        private void SaveCurrentPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
                PlaylistManager.SaveSelectedPlaylist(selectedTab);
            }
            catch
            {

            }
        }

        private void LoadPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
                PlaylistManager.OpenPlaylistDialog();
            }
            catch
            {

            }
        }


        //POSITION SLIDER
        private void ChangeSliderPosition(object sender, EventArgs e)
        {
            try
            {
                if (!isDraggingPosition && !isClickedPosition)
                {
                    if (Player.player.NaturalDuration.HasTimeSpan == true)
                    {
                        TrackProgressSlider.Value = Player.player.Position.TotalMilliseconds * 1000 / Player.player.NaturalDuration.TimeSpan.TotalMilliseconds;
                    }
                }
            }
            catch
            {

            }
        }

        private void TrackProgressSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            try
            { 
                isDraggingPosition = true;
                Visualizer.PauseCurrentVisualization();
            }
            catch
            {

            }
        }

        private void TrackProgressSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            try
            {
                isDraggingPosition = false;

                if (Player.CurrentPlaylist.Tracklist.Count > 0)
                {
                    Player.pauseTime = Player.totalTime * TrackProgressSlider.Value / 1000;
                    Player.StartPlaying();
                }
            }
            catch
            {

            }
        }


        //VOLUME
        private void MuteButton_Click(object sender, RoutedEventArgs e)
        {
            try
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
            catch
            {

            }
        }


        //VOLUME SLIDER
        private void VolumeLevelSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            try
            {
                Player.MuteOff();
                MuteButton.Background = new SolidColorBrush(Color.FromArgb(153, 255, 255, 255));
                Player.SetVolumeLevel(VolumeLevelSlider.Value / 100);
            }
            catch
            {

            }
        }

        private void VolumeLevelSlider_Click(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                Player.MuteOff();
                MuteButton.Background = new SolidColorBrush(Color.FromArgb(153, 255, 255, 255));
                Player.SetVolumeLevel(VolumeLevelSlider.Value / 100);
            }
            catch
            {

            }
        }

        private void ModeNone_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Visualizer.StopVisualizaton();
                mode = 0;
                Visualizer.ChoseVisualizationMode(mode);

                isVisualizationMenuOpened = false;
                (sender as Button).ContextMenu.IsOpen = false;
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }
        }

        private void ModeHorizontal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Visualizer.StopVisualizaton();
                mode = 1;
                Visualizer.ChoseVisualizationMode(mode);

                isVisualizationMenuOpened = false;
                (sender as Button).ContextMenu.IsOpen = false;

            }
            catch
            {

            }
        }

        private void ModeVertical_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Visualizer.StopVisualizaton();
                mode = 2;
                Visualizer.ChoseVisualizationMode(mode);

                isVisualizationMenuOpened = false;
                (sender as Button).ContextMenu.IsOpen = false;
            }
            catch
            {

            }
        }

        private void ModeMesh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Visualizer.StopVisualizaton();
                mode = 3;
                Visualizer.ChoseVisualizationMode(mode);

                isVisualizationMenuOpened = false;
                (sender as Button).ContextMenu.IsOpen = false;
            }
            catch
            {

            }
        }

        private void ModeWave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Visualizer.StopVisualizaton();
                mode = 4;
                Visualizer.ChoseVisualizationMode(mode);

                isVisualizationMenuOpened = false;
                (sender as Button).ContextMenu.IsOpen = false;
            }
            catch
            {

            }
        }

        private void ShowAboutMessageBox(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show("PHAZE 2.1 RELEASE;\nCourse work by George Puisha;\nFebruary-June 2017.", "Credits");
            }
            catch
            {

            }
        }
    }
}
