using System;
using System.Text;

namespace SiouxTube
{
    /// <summary>
    /// Representation of a YouTube clip
    /// </summary>
    /// <remarks>
    /// FIXME: Not supported yet: time indexes.
    /// </remarks>
    public class SimpleYouTubeClip
    {
        public SimpleYouTubeClip(string id, string submitter, DateTime submittedTime)
        {
            this.ID = id;
            this.Submitter = submitter;
            this.SubmittedTime = submittedTime;
        }

        public string ID { get; private set; }

        public string Submitter { get; private set; }

        public DateTime SubmittedTime { get; private set; }
        
        /// <summary>
        /// Return the URL to play this clip and nothing else
        /// </summary>
        public string EmbeddedURL
        {
            get
            {
                StringBuilder sb = new StringBuilder("http://www.youtube.com/v/");
                sb.Append(ID);
                
                sb.Append("?version=3");        // Use embedded AS3 player
                sb.Append("&controls=0");       // Don't show player controls
                sb.Append("&autoplay=1");       // Automatically play the video after loading
                sb.Append("&disablekb=1");      // Disable keyboard input
                sb.Append("&rel=0");            // Do not load relevant videos
                sb.Append("&showsearch=0");     // Turn off search
                sb.Append("&iv_load_policy=3"); // Turn off annotations
                sb.Append("&hd=1");             // Force HD
    
                return sb.ToString();
            }
        }
    }
}

