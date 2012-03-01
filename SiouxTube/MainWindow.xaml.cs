﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Retlang.Channels;
using Retlang.Fibers;
using System.Threading;

namespace SiouxTube
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Timer finishedTimer;
        private System.Timers.Timer progressTimer;
        private TimeSpan progress;
        private TimeSpan duration;
        private RichYouTubeClip currentClip;
        private IPublisher<RichYouTubeClip> finishedChannel;
        private SiouxTV tv = new SiouxTV();

        public MainWindow(ISubscriber<PlayerCommand> commandChannel, IPublisher<RichYouTubeClip> finishedChannel)
        {
            InitializeComponent();
            this.finishedChannel = finishedChannel;

            finishedTimer = new Timer(TimerExpired);
            
            progressTimer = new System.Timers.Timer(1000);
            progressTimer.AutoReset = true;
            progressTimer.Elapsed += new System.Timers.ElapsedEventHandler(progressTimer_Elapsed);

            var fiber     = new DispatcherFiber(Dispatcher);
            commandChannel.Subscribe(fiber, OnPlayerCommand);
            fiber.Start();

            WindowStyle = WindowStyle.None;
            WindowState = WindowState.Maximized;
            ShowInTaskbar = false;
            
            Cursor = Cursors.None;
            try
            {
                tv.Initialize();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error initialize remote control", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnPlayerCommand(PlayerCommand command)
        {
            if (command is PlaylistBegins) OnPlaylistBegins();
            if (command is PlaylistFinished) OnPlaylistFinished();
            if (command is PlayClip) OnPlayClip((PlayClip)command);
        }

        private void HideWindow()
        {
            Hide();
        }

        private void ShowWindow()
        {
            Show();
        }

        private void OnPlaylistFinished()
        {
            tv.SwitchToExt1();
            HideWindow();
        }

        private void OnPlaylistBegins()
        {
            tv.SwitchToPCInput();
            MoveMouseToSide();
            ShowWindow();
        }

        private void OnPlayClip(PlayClip playClip)
        {
            SenderName.Text = playClip.Clip.Submitter;
            ClipTitle.Text = playClip.Clip.Title;
            Browser.Navigate(playClip.Clip.EmbeddedURL);
            currentClip = playClip.Clip;
            duration = playClip.Clip.Duration;

            progress = new TimeSpan(0);
            ElapsedTime.Text = "00:00 / " + duration.ToString(@"mm\:ss");
            progressTimer.Start();

            // This is the bastard way to signal a finished clip -- take the duration and add a couple of seconds...
            finishedTimer.Change((playClip.Clip as RichYouTubeClip).Duration + new TimeSpan(0, 0, 5), new TimeSpan(0, 0, 0, 0, -1));
        }

        void progressTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if(progress < duration)
                progress += new TimeSpan(0, 0, 0, 1);

            // We can call on the current thread
            this.Dispatcher.Invoke((Action)delegate { ElapsedTime.Text = progress.ToString(@"mm\:ss") + " / " + duration.ToString(@"mm\:ss"); });
        }


        private void TimerExpired(object sender)
        {
            finishedChannel.Publish(currentClip);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }

        /// <summary>
        /// Move the mouse to the right side of the screen
        /// </summary>
        private void MoveMouseToSide()
        {
            var r = System.Windows.Forms.SystemInformation.VirtualScreen;
            Mouse.Location = new Point(r.Width, r.Height / 2);
        }
    }
}