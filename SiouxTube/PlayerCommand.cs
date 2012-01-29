using System;

namespace SiouxTube
{
    /// <summary>
    /// Class that encapsulates different player commands
    /// </summary>
    abstract public class PlayerCommand
    {
    }
    
    /// <summary>
    /// Indicate that a new sequence of playback begins
    /// </summary>
    public class PlaylistBegins : PlayerCommand
    {
    }
    
    /// <summary>
    /// Indicate that we don't have anything to play right now
    /// </summary>
    public class PlaylistFinished : PlayerCommand
    {
    }
    
    /// <summary>
    /// Indicate that the given clip must be played now
    /// </summary>
    public class PlayClip : PlayerCommand
    {
        public PlayClip(RichYouTubeClip clip)
        {
            this.Clip = clip;
        }
        
        public RichYouTubeClip Clip { get; private set; }
    }
}

