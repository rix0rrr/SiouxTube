using System;
using System.Collections.Generic;
using Retlang.Channels;

namespace SiouxTube
{
    public class SinkChannel<T> : IPublisher<T>
    {
        public SinkChannel()
        {
            Messages = new List<T>();
        }

        public bool Publish(T msg)
        {
            Messages.Add(msg);
            return true;
        }

        public List<T> Messages { get; private set; }
    }
}
