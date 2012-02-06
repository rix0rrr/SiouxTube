using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SiouxTube;
using Retlang.Channels;
using Retlang.Fibers;

namespace SiouxTubeTests
{
    class PlaylistControllerTest
    {
        private StubFiber fiber;
        private PlaylistController controller;
        private Channel<RichYouTubeClip> newChannel;
        private Channel<RichYouTubeClip> finishedChannel;
        private SinkChannel<PlayerCommand> commandChannel;
        private RichYouTubeClip clip1 = new RichYouTubeClip("1", "1", DateTime.Now, TimeSpan.FromSeconds(1), "1", new Uri("http://www.example.com/"));
        private RichYouTubeClip clip2 = new RichYouTubeClip("2", "2", DateTime.Now, TimeSpan.FromSeconds(1), "2", new Uri("http://www.example.com/"));

        [SetUp]
        public void SetUp()
        {
            fiber = new StubFiber() { ExecutePendingImmediately = true };
            newChannel = new Channel<RichYouTubeClip>();
            finishedChannel = new Channel<RichYouTubeClip>();
            commandChannel  = new SinkChannel<PlayerCommand>();

            controller = new PlaylistController(fiber, newChannel, finishedChannel, commandChannel);
        }

        /// <summary>
        /// Don't start a new item when the previous one hasn't completed yet
        /// </summary>
        [Test]
        public void NoStartWhileRunning()
        {
            newChannel.Publish(clip1);
            newChannel.Publish(clip2);

            Assert.AreEqual(1, commandChannel.Messages.OfType<PlayClip>().Count());
        }

        /// <summary>
        /// Do start a new item after the previous one finished
        /// </summary>
        [Test]
        public void PlayNextOnFinished()
        {
            newChannel.Publish(clip1);
            newChannel.Publish(clip2);

            finishedChannel.Publish(clip1);
            Assert.AreEqual(2, commandChannel.Messages.OfType<PlayClip>().Count());
        }
    }
}
