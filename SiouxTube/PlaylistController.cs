using System;
using System.Collections.Generic;
using Retlang.Fibers;
using Retlang.Channels;

namespace SiouxTube
{
    /// <summary>
    /// Class responsible for managing the playlist
    /// 
    /// New videos to play and "playing complete" messages come
    /// in, while "play now" messages go out.
    /// </summary>
    public class PlaylistController : IDisposable
    {
        private readonly IPublisher<PlayerCommand> playerCommands;
        private readonly IDisposable newClipSubscription;
        private readonly IDisposable finishedSubscription;
        
        private readonly LinkedList<RichYouTubeClip> playQueue = new LinkedList<RichYouTubeClip>();
        private bool currentlyPlaying = false;
        
        public PlaylistController(IFiber fiber,
            ISubscriber<RichYouTubeClip> newClips,
            ISubscriber<RichYouTubeClip> finishedPlaying,
            IPublisher<PlayerCommand> playerCommands)
        {
            this.playerCommands = playerCommands;
            this.newClipSubscription  = newClips.Subscribe(fiber, NewClip);
            this.finishedSubscription = finishedPlaying.Subscribe(fiber, Finished);
        }
        
        private void NewClip(RichYouTubeClip clip)
        {
            playQueue.AddLast(clip);
            PlayNext();
        }
        
        private void Finished(RichYouTubeClip clip)
        {
            PlayNext();
        }
        
        private void PlayNext()
        {
            if (playQueue.Count > 0)
            {
                if (!currentlyPlaying) playerCommands.Publish(new PlaylistBegins());
                currentlyPlaying = true;
                playerCommands.Publish(new PlayClip(playQueue.First.Value));
                playQueue.RemoveFirst();
            }
            else
            {
                if (currentlyPlaying) playerCommands.Publish(new PlaylistFinished());
                currentlyPlaying = false;
            }
        }
        
        public virtual void Dispose()
        {
            newClipSubscription.Dispose();
            finishedSubscription.Dispose();
        }
    }
}

