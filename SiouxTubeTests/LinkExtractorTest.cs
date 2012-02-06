using System;
using System.Text;
using NUnit.Framework;
using Retlang.Channels;
using Retlang.Fibers;
using AE.Net.Mail;

namespace SiouxTube
{
    [TestFixture]
    public class LinkExtractorTest
    {
        private StubFiber fiber;
        private Channel<MailMessage> input;
        private SinkChannel<SimpleYouTubeClip> sink;
        private LinkExtractor extractor;
        
        [SetUp]
        public void SetUp()
        {
            fiber = new StubFiber() { ExecutePendingImmediately = true };
            input = new Channel<MailMessage>();
            sink  = new SinkChannel<SimpleYouTubeClip>();

            extractor = new LinkExtractor(fiber, input, sink);
        }
        
        [TearDown]
        public void TearDown()
        {
            extractor.Dispose();
        }
        
        [Test]
        public void RecognizeLinkInSubject()
        {
            var msg = new MailMessage();
            
            msg.Load(@"Delivered-To: rix0rrr@gmail.com
Received: by 10.229.51.77 with SMTP id c13cs138895qcg;
        Fri, 2 Dec 2011 07:07:47 -0800 (PST)
Date: Fri, 02 Dec 2011 16:07:44 +0100
From: Sender <sender@example.nl>
To: bla@bla.nl
Message-ID: <20111202132014.03454797@hezemans.nl>
X-Mailer: Dada Mail 4.6.0 Stable - 08/07/11 
Content-type: multipart/alternative; boundary="+"\"_----------=_132282841491260\"" + @"; charset=UTF-8
Content-Transfer-Encoding: binary
MIME-Version: 1.0
Subject: Check this: http://www.youtube.com/watch?v=8HhncO6w4Pc&feature=g-all

This is a multi-part message in MIME format.

--_----------=_132282841491260
MIME-Version: 1.0
Content-Transfer-Encoding: quoted-printable
Content-Type: text/plain; charset=UTF-8
Date: Fri, 2 Dec 2011 13:20:14 +0100

Yaay

--_----------=_132282841491260--");
            input.Publish (msg);
            
            Assert.AreEqual(1, sink.Messages.Count);
            var clip = sink.Messages[0];
            Assert.AreEqual("8HhncO6w4Pc", clip.ID);
            Assert.AreEqual("Sender", clip.Submitter);
        }
        
        [Test]
        public void RecognizeLinkInBody()
        {
            var msg = new MailMessage();
            msg.Load(@"Delivered-To: rix0rrr@gmail.com
Received: by 10.229.51.77 with SMTP id c13cs138895qcg;
        Fri, 2 Dec 2011 07:07:47 -0800 (PST)
Date: Fri, 02 Dec 2011 16:07:44 +0100
From: Sender <sender@example.nl>
To: bla@bla.nl
Message-ID: <20111202132014.03454797@hezemans.nl>
X-Mailer: Dada Mail 4.6.0 Stable - 08/07/11 
Content-type: multipart/alternative; boundary="+"\"_----------=_132282841491260\"" + @"; charset=UTF-8
Content-Transfer-Encoding: binary
MIME-Version: 1.0
Subject: Nice video for you

This is a multi-part message in MIME format.

--_----------=_132282841491260
MIME-Version: 1.0
Content-Transfer-Encoding: quoted-printable
Content-Type: text/plain; charset=UTF-8
Date: Fri, 2 Dec 2011 13:20:14 +0100

Check this: http://www.youtube.com/watch?v=8HhncO6w4Pc&feature=g-all

--_----------=_132282841491260--");
            input.Publish (msg);
            
            Assert.AreEqual(1, sink.Messages.Count);
            var clip = sink.Messages[0];
            Assert.AreEqual("8HhncO6w4Pc", clip.ID);
            Assert.AreEqual("Sender", clip.Submitter);
        }

        [Test]
        public void RecognizeLinkWithVideoIdLater()
        {
            var msg = new MailMessage();
            msg.Load(@"Delivered-To: rix0rrr@gmail.com
Received: by 10.229.51.77 with SMTP id c13cs138895qcg;
        Fri, 2 Dec 2011 07:07:47 -0800 (PST)
Date: Fri, 02 Dec 2011 16:07:44 +0100
From: Sender <sender@example.nl>
To: bla@bla.nl
Message-ID: <20111202132014.03454797@hezemans.nl>
X-Mailer: Dada Mail 4.6.0 Stable - 08/07/11 
Content-type: multipart/alternative; boundary="+"\"_----------=_132282841491260\"" + @"; charset=UTF-8
Content-Transfer-Encoding: binary
MIME-Version: 1.0
Subject: Nice video for you

This is a multi-part message in MIME format.

--_----------=_132282841491260
MIME-Version: 1.0
Content-Transfer-Encoding: quoted-printable
Content-Type: text/plain; charset=UTF-8
Date: Fri, 2 Dec 2011 13:20:14 +0100

Check this: http://www.youtube.com/watch?feature=embedded_player&v=8HhncO6w4Pc

--_----------=_132282841491260--");
            input.Publish (msg);
            
            Assert.AreEqual(1, sink.Messages.Count);
            var clip = sink.Messages[0];
            Assert.AreEqual("8HhncO6w4Pc", clip.ID);
            Assert.AreEqual("Sender", clip.Submitter);
        }
    }
}

