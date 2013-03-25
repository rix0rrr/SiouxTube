using System;
using System.Linq;
using System.Net;
using System.IO;
using System.Xml;
using Retlang.Channels;
using Retlang.Fibers;
using System.Diagnostics;

namespace SiouxTube
{
    /// <summary>
    /// This class downloads info about YouTube clips based on simple clip
    /// info and outputs enriched info.
    /// </summary>
    public class YouTubeInfoDownloader : IDisposable
    {
        private readonly IPublisher<RichYouTubeClip> richChannel;
        private readonly IDisposable subscription;

        public YouTubeInfoDownloader(IFiber fiber, ISubscriber<SimpleYouTubeClip> simpleChannel, IPublisher<RichYouTubeClip> richChannel)
        {
            this.richChannel = richChannel;
            this.subscription = simpleChannel.Subscribe(fiber, EnrichYouTubeClip);
        }

        private void EnrichYouTubeClip(SimpleYouTubeClip simple)
        {
            var dataUrl = string.Format("http://gdata.youtube.com/feeds/api/videos/{0}", simple.ID);

            Debug.WriteLine("Downloading " + dataUrl);

            using (var wc = new WebClient())
            {
                string xml = wc.DownloadString(dataUrl);
                StringReader reader = new StringReader(xml);
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);

                // Get properties from feed
                var seconds  = int.Parse(doc.GetElementsByTagName("yt:duration")[0].Attributes["seconds"].Value);
                var duration = new TimeSpan(0, 0, seconds);
                var title    = doc.GetElementsByTagName("title")[0].FirstChild.Value;
                var thumbnailURL = new Uri(doc.GetElementsByTagName("media:thumbnail")[0].Attributes["url"].Value);

                var rich = RichYouTubeClip.FromSimple(simple, duration, title, thumbnailURL);
                Debug.WriteLine("YouTube clip enriched. Publishing.");
                richChannel.Publish(rich);
            }
        }

        public virtual void Dispose()
        {
            subscription.Dispose();
        }
        
#if NEVER
        // We could use this piece of code to also download the thumb
        // if we had WPF. But we don't.
        private void LoadThumbnail(Uri url)
        {
            try
            {
                    WebClient wc = new WebClient();
                    byte[] bytes = wc.DownloadData(url);

                    MemoryStream stream = new MemoryStream(bytes);
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = stream;
                    image.EndInit();
                    //set thumbnail
                    Thumbnail = image;
            }
            catch
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri("youtube_logo.png", UriKind.Relative);
                image.EndInit();
                Thumbnail = image;
            }
        }
#endif
    }
}

