using System;
using System.Linq;
using NUnit.Framework;
using Retlang.Channels;
using Retlang.Fibers;

namespace SiouxTube
{
	[TestFixture]
    public class YouTubeInfoDownloaderTest
    {
        private StubFiber fiber;
        private YouTubeInfoDownloader downloader;
        private Channel<SimpleYouTubeClip> simpleChannel;
        private SinkChannel<RichYouTubeClip> sink;

        [SetUp]
        public void SetUp()
        {
            fiber = new StubFiber() { ExecutePendingImmediately = true };
            simpleChannel = new Channel<SimpleYouTubeClip>();
            sink = new SinkChannel<RichYouTubeClip>();

            downloader = new  YouTubeInfoDownloader(fiber, simpleChannel, sink);
        }

        [TearDown]
        public void TearDown()
        {
            downloader.Dispose();
        }

        [Test]
        public void TestRetrieval()
        {
            var simple = new SimpleYouTubeClip("dC1FJBw8uaU", "test@test.nl", DateTime.Now);
            simpleChannel.Publish(simple);

            Assert.AreEqual(1, sink.Messages.Count);
            var clip = sink.Messages[0];

            Assert.AreEqual(new TimeSpan(0, 0, 12), clip.Duration);
            Assert.AreEqual("foreman.qcif", clip.Title);
        }
    }
}

