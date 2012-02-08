using System;
using System.Text;
using NUnit.Framework;
using Retlang.Channels;
using Retlang.Fibers;
using AE.Net.Mail;
using System.Diagnostics;

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

        [Test]
        public void RecognizeHttpsLink()
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

Check this: https://www.youtube.com/watch?feature=embedded_player&v=8HhncO6w4Pc

--_----------=_132282841491260--");
            input.Publish (msg);
            
            Assert.AreEqual(1, sink.Messages.Count);
            var clip = sink.Messages[0];
            Assert.AreEqual("8HhncO6w4Pc", clip.ID);
            Assert.AreEqual("Sender", clip.Submitter);
        }

        [Test]
        public void RecognizeBase64EncodedMail()
        {
            var msg = new MailMessage();
            msg.Load(@"Delivered-To: siouxtube@gmail.com
Received: by 10.112.29.134 with SMTP id k6cs32313lbh;
        Wed, 8 Feb 2012 03:31:35 -0800 (PST)
Received: by 10.213.13.218 with SMTP id d26mr2697709eba.56.1328700694648;
        Wed, 08 Feb 2012 03:31:34 -0800 (PST)
Return-Path: <bart.sanders@sioux.eu>
Received: from smtpout01.onlinespamfilter.nl (smtpout01.onlinespamfilter.nl. [217.21.240.157])
        by mx.google.com with ESMTPS id b46si798117eei.102.2012.02.08.03.31.34
        (version=TLSv1/SSLv3 cipher=OTHER);
        Wed, 08 Feb 2012 03:31:34 -0800 (PST)
Received-SPF: neutral (google.com: 217.21.240.157 is neither permitted nor denied by best guess record for domain of bart.sanders@sioux.eu) client-ip=217.21.240.157;
Authentication-Results: mx.google.com; spf=neutral (google.com: 217.21.240.157 is neither permitted nor denied by best guess record for domain of bart.sanders@sioux.eu) smtp.mail=bart.sanders@sioux.eu
Received: from smtp.onlinespamfilter.nl (localhost [127.0.0.1])
	by smtp.onlinespamfilter.nl (Postfix) with ESMTP id F29C025088
	for <siouxtube@gmail.com>; Wed,  8 Feb 2012 12:31:32 +0100 (CET)
Received: from [77.62.58.8] (unknown [77.62.58.8])
	(using TLSv1 with cipher RC4-MD5 (128/128 bits))
	(No client certificate requested)
	by smtp.onlinespamfilter.nl (Postfix) with ESMTP
	for <siouxtube@gmail.com>; Wed,  8 Feb 2012 12:31:32 +0100 (CET)
To: siouxtube@gmail.com
From: #=?utf-8?B?QmFydCBTYW5kZXJz?=# <bart.sanders@sioux.eu>
Subject: 
Date: Wed, 08 Feb 2012 12:31:22 +0100
MIME-Version: 1.0
Content-Type: multipart/alternative;
	boundary=#----=_Part_0_1328700682140#
Message-ID: <mailbox-29881-1328700693-386288@smtpin01.onlinespamfilter.nl>
X-OSF-Virus: CLEAN
X-OSF-Outgoing: Innocent
X-OSF-SUM: 97ab0211c2938cdf9afc5a79ac55dbc4
X-OSF-Info: Checked for spam and viruses

------=_Part_0_1328700682140
Content-Type: text/plain;
	charset=utf-8
Content-Transfer-Encoding: base64
Content-Disposition: inline

aHR0cDovL3d3dy55b3V0dWJlLmNvbS93YXRjaD92PU56ZXVTNkJpV0M4CgoKCg==


------=_Part_0_1328700682140
Content-Type: text/html;
	charset=utf-8
Content-Transfer-Encoding: base64
Content-Disposition: inline

PGEgaHJlZj0iaHR0cDovL3d3dy55b3V0dWJlLmNvbS93YXRjaD92PU56ZXVTNkJpV0M4Ij5odHRw
Oi8vd3d3LnlvdXR1YmUuY29tL3dhdGNoP3Y9TnpldVM2QmlXQzg8L2E+PGJyPjxicj48YnI+PGJy
Pg==


------=_Part_0_1328700682140--".Replace("#", "\""));
            input.Publish(msg);
            
            Assert.AreEqual(1, sink.Messages.Count);
            var clip = sink.Messages[0];
            Assert.AreEqual("NzeuS6BiWC8", clip.ID);
        }
    }
}

