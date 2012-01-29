using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Retlang.Fibers;
using Retlang.Channels;

namespace SiouxTube
{
    /// <summary>
    /// Clip filter -- will only allow the clips through if it's not lunchtime
    /// (12:00-13:00)
    /// </summary>
    class LunchFilter : IDisposable
    {
        private readonly IDisposable subscription;
        private readonly IPublisher<SimpleYouTubeClip> output;

        public LunchFilter(IFiber fiber,
            ISubscriber<SimpleYouTubeClip> input,
            IPublisher<SimpleYouTubeClip> output)
        {
            this.output = output;
            this.subscription  = input.Subscribe(fiber, NewClip);
        }

        private void NewClip(SimpleYouTubeClip clip)
        {
            if (DateTime.Now.Hour != 12 || !Enabled) output.Publish(clip);
        }

        public void Dispose()
        {
            subscription.Dispose();
        }

        public bool Enabled { get; set; }
    }
}
