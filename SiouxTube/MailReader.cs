using System;
using System.Collections.Generic;
using System.Linq;
using Retlang.Channels;
using Retlang.Fibers;
using AE.Net.Mail;
using System.Net.Sockets;
using System.Windows;
using System.Diagnostics;

namespace SiouxTube
{
    /// <summary>
    /// Reads a mailbox and sends any newly encountered
    /// message over a Channel.
    /// </summary>
    public class MailReader : IDisposable
    {
        private readonly IPublisher<MailMessage> messageChannel;
        private readonly ServerAddress popServerAddress;
        private readonly bool deleteReadMessages;
        private readonly Dictionary<string, bool> seenUuids = new Dictionary<string, bool>();
        private readonly IDisposable subscription;

        public MailReader (ServerAddress popServerAddress, long checkIntervalMs, bool deleteReadMessages, IFiber fiber, IPublisher<MailMessage> messageChannel)
        {
            this.popServerAddress = popServerAddress;
            this.messageChannel   = messageChannel;
            this.deleteReadMessages = deleteReadMessages;
            
            this.subscription = fiber.ScheduleOnInterval(CheckNow, 0, checkIntervalMs);
        }
        
        /// <summary>
        /// Check the mailbox for new unread e-mails and send them out over
        /// the publisher channel
        /// </summary>
        private void CheckNow()
        {
            try
            {
                Debug.WriteLine("Checking for messages.");
                using (Pop3Client client = new Pop3Client())
                {
                    client.Connect(popServerAddress.Host, popServerAddress.Port, popServerAddress.Ssl);
                    client.Login(popServerAddress.Username, popServerAddress.Password);
                    
                    foreach (var msg in UnreadMessages(client))
                    {
                        messageChannel.Publish(msg.Item2);
                        HandledMessage(client, msg);
                    }

                    client.Disconnect(); // Necessary otherwise new mails won't be detected on next connect... (???)
                }
                // Disconnect will  trigger cleanup of deleted messages
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occurred checking the mailbox: {0}", ex);
            }
        }
        
        /// <summary>
        /// Returns all unread messages in tuples along with their position in the mailbox
        /// </summary>
        private IEnumerable<Tuple<int, MailMessage>> UnreadMessages(Pop3Client pop3)
        {
            var count = pop3.GetMessageCount();
            Debug.WriteLine("Found {0} messages", count);
            for (var i = 0; i < count; i++)
            {
                var msgHeads = pop3.GetMessage(i, true);
                if (seenUuids.ContainsKey(msgHeads.Uid)) continue; // Old message

                yield return new Tuple<int, MailMessage>(i, pop3.GetMessage(i));
            }
        }
        
        /// <summary>
        /// Mark the message with the given UUID as handled. Either
        /// delete the message or add it to the set of seen UUIDs.
        /// </summary>
        private void HandledMessage(Pop3Client pop3, Tuple<int, MailMessage> message)
        {
            if (deleteReadMessages)
                pop3.DeleteMessage(message.Item1);
            else
                seenUuids[message.Item2.Uid] = true;
        }

        public virtual void Dispose()
        {
            subscription.Dispose();
        }
    }
}

