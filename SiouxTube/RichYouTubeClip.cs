using System;

namespace SiouxTube
{
    public class RichYouTubeClip : SimpleYouTubeClip
    {
        public RichYouTubeClip(string id, string submitter, DateTime submittedTime, TimeSpan duration, string title, Uri thumbnailURL)
            : base(id, submitter, submittedTime)
        {
            this.Duration = duration;
            this.Title = title;
            this.ThumbnailURL = thumbnailURL;
        }

        public TimeSpan Duration { get; private set; }

        public string Title { get; private set; }

        public Uri ThumbnailURL { get; private set; }

        public static RichYouTubeClip FromSimple(SimpleYouTubeClip simple, TimeSpan duration, string title, Uri thumbnailURL)
        {
            return new RichYouTubeClip(
                simple.ID, simple.Submitter, simple.SubmittedTime,
                duration, title, thumbnailURL);
        }

        public override string ToString()
        {
            return base.ToString() + ": " + Title + " (" + Duration + ")";
        }
    }
}

