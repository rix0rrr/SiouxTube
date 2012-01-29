using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Retlang.Channels;
using AE.Net.Mail;

namespace SiouxTube
{
    /// <summary>
    /// Interaction logic for FakeMailWindow.xaml
    /// </summary>
    public partial class FakeMailWindow : Window
    {
        private readonly IPublisher<MailMessage> messageChannel;

        public FakeMailWindow(IPublisher<MailMessage> messageChannel)
        {
            InitializeComponent();

            this.messageChannel = messageChannel;
            ResetMail();
        }

        private void ReceiveButton_Click(object sender, RoutedEventArgs e)
        {
            FakeReceiveMail(MailBox.Text);
            ResetMail();
        }

        private void ResetMail()
        {
            MailBox.Text = @"From: Someone <someone@example.com>
To: You <you@you.com>
Content-type: text/plain; charset=UTF-8
MIME-Version: 1.0
Subject: Check this vid out!

http://www.youtube.com/watch?v=8HhncO6w4Pc&feature=g-all";
        }

        private void FakeReceiveMail(string mailText)
        {
            var msg = new MailMessage();
            msg.Load(mailText);
            messageChannel.Publish(msg);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}
