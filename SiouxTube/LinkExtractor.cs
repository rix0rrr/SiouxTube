using System;
using System.Linq;
using System.Text.RegularExpressions;
using Retlang.Channels;
using Retlang.Fibers;
using AE.Net.Mail;
using System.Diagnostics;

namespace SiouxTube
{
    /// <summary>
    /// Class that extracts YouTube links from e-mail Messages
    /// </summary>
    public class LinkExtractor : IDisposable
    {
        private static Regex YouTubeLinkRegex1 = new Regex(
            @"https?://(?:www\.)?youtube\.com/watch\?(.*&)?v=(?<videoid>[a-zA-Z0-9_-]+)");
        private static Regex YouTubeLinkRegex2 = new Regex(
            @"https?://(?:www\.)?youtu\.be/(?<videoid>[a-zA-Z0-9_-]+)");

        private readonly IPublisher<SimpleYouTubeClip> clipChannel;
        private readonly IDisposable subscription;

        public LinkExtractor(IFiber fiber, ISubscriber<MailMessage> messageChannel, IPublisher<SimpleYouTubeClip> clipChannel)
        {
            this.clipChannel  = clipChannel;
            this.subscription = messageChannel.Subscribe(fiber, HandleMessage);
        }

        /// <summary>
        /// Parse one message for YouTube links and publish them on the clip channel
        /// if found.
        /// </summary>
        /// <remarks>
        /// Currently only supports one link per mail.
        /// </remarks>
        private void HandleMessage(MailMessage m)
        {
            var match = YouTubeLinkRegex1.Match(MessageAsText(m));
            if (!match.Success) match = YouTubeLinkRegex2.Match(MessageAsText(m));

            if (match.Success)
            {
                var vid = match.Groups["videoid"].Value;
                var clip = new SimpleYouTubeClip(vid, m.From != null ? m.From.DisplayName : "Unknown Sender", DateTime.Now);
                Debug.WriteLine("In message '{0}' found: {1}", m.Subject, clip);
                clipChannel.Publish(clip);
            }
            else
                Debug.WriteLine("Did not find a YouTube link in '" + m.Subject + "'");
        }

        /// <summary>
        /// Return a plain text version of the message
        /// </summary>
        private string MessageAsText(MailMessage m)
        {
            return m.Subject + Environment.NewLine + m.Body + 
                string.Join(Environment.NewLine, m.Attachments.Where(_ => _.ContentType == "text/plain").Select(_ => _.Body));
        }

        public void Dispose()
        {
            subscription.Dispose();
        }
    }
}

