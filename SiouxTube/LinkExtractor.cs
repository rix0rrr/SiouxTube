using System;
using System.Linq;
using System.Text.RegularExpressions;
using Retlang.Channels;
using Retlang.Fibers;
using AE.Net.Mail;

namespace SiouxTube
{
    /// <summary>
    /// Class that extracts YouTube links from e-mail Messages
    /// </summary>
    public class LinkExtractor : IDisposable
    {
        private static Regex YouTubeLinkRegex = new Regex(
            @"http://(?:www\.)?youtube\.com/watch\?(.*&)?v=(?<videoid>[a-zA-Z0-9_-]+)");

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
            var match = YouTubeLinkRegex.Match(MessageAsText(m));
            if (match.Success)
            {
                var vid = match.Groups["videoid"].Value;
                var clip = new SimpleYouTubeClip(vid, m.From != null ? m.From.DisplayName : "Unknown Sender", DateTime.Now);
                clipChannel.Publish(clip);
            }
        }

        /// <summary>
        /// Return a plain text version of the message
        /// </summary>
        private string MessageAsText(MailMessage m)
        {
            return m.Subject + Environment.NewLine + m.Body;
        }

        public void Dispose()
        {
            subscription.Dispose();
        }
    }
}

