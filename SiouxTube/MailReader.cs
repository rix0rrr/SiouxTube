using System;
using System.Collections.Generic;
using System.Linq;
using Retlang.Channels;
using Retlang.Fibers;
using AE.Net.Mail;
using System.Net.Sockets;
using System.Windows;
using System.Diagnostics;
using System.Threading;
using AE.Net.Mail.Imap;

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
        private readonly ImapClient imap;

        public MailReader (ServerAddress popServerAddress, long checkIntervalMs, bool deleteReadMessages, IFiber fiber, IPublisher<MailMessage> messageChannel)
        {
            this.popServerAddress = popServerAddress;
            this.messageChannel   = messageChannel;
            this.deleteReadMessages = deleteReadMessages;

            this.imap = new ImapClient(popServerAddress.Host, popServerAddress.Username, popServerAddress.Password, ImapClient.AuthMethods.Login, popServerAddress.Port, popServerAddress.Ssl);
            imap.SelectMailbox("INBOX");

            imap.NewMessage += HandleNewMessage;
        }

        /// <summary>
        /// Scan all messages currently in the mailbox
        /// </summary>
        private void ScanAllMessages() 
        {
            var uids = imap.Search(SearchCondition.Unseen());
            if (uids.Count() == 0) return;

            Debug.WriteLine("Found {0} new messages", uids.Count());
            foreach (var uid in uids)
            {
                var msg = imap.GetMessage(uid);
                HandleMessage(msg);
            }        
        }

        private void HandleNewMessage(object sender, MessageEventArgs args)
        {
            var msg = imap.GetMessage(args.MessageCount - 1);
            if (msg == null) return;
            Debug.WriteLine("Received a message: " + msg.Subject);
            HandleMessage(msg);
        }

        private void HandleMessage(MailMessage message)
        {
            messageChannel.Publish(message);
            if (deleteReadMessages)
                imap.DeleteMessage(message);
        }

        public virtual void Dispose()
        {
            imap.NewMessage -= HandleNewMessage;
            imap.Disconnect();
        }
    }
}

