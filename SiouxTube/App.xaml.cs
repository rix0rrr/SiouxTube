using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Retlang.Fibers;
using Retlang.Channels;
using AE.Net.Mail;
using SiouxTube.Properties;

namespace SiouxTube
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var settings = Settings.Default;

            var poolFiber = new PoolFiber();
            poolFiber.Start();

            var mailChannel      = new Channel<MailMessage>();
            var foundClipChannel = new Channel<SimpleYouTubeClip>();
            var filteredChannel  = new Channel<SimpleYouTubeClip>();
            var richChannel      = new Channel<RichYouTubeClip>();

            var finishedChannel  = new Channel<RichYouTubeClip>();
            var commandChannel   = new Channel<PlayerCommand>();

            var extractor   = new LinkExtractor(poolFiber, mailChannel, foundClipChannel);
            var lunchFilter = new LunchFilter(poolFiber, foundClipChannel, filteredChannel);
            var downloader  = new YouTubeInfoDownloader(poolFiber, filteredChannel, richChannel);
            var playlist    = new PlaylistController(poolFiber, richChannel, finishedChannel, commandChannel);

            var main = new MainWindow(commandChannel, finishedChannel);

            lunchFilter.Enabled = settings.UseLunchFilter;

            // ----- FAKE INPUT ---------------------
            if (settings.DisplayFakeWindow)
            {
                var fake = new FakeMailWindow(mailChannel);
                fake.Show();
            }

            // ----- MAIL INPUT ---------------------
            if (settings.MailHost != "")
            {
                var server = new ServerAddress(settings.MailHost, settings.MailUseSsl ? 995 : 110, settings.MailUseSsl, settings.MailUser, settings.MailPassword);
                var mail = new MailReader(server, settings.MailCheckIntervalSecs * 1000, settings.DeleteHandledMail, poolFiber, mailChannel);
            }

            base.OnStartup(e);
        }
    }
}
