using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IguanaRC5;
using System.Threading;

namespace TestIR
{
    class Program
    {
        static void Main(string[] args)
        {
            var tr = new RC5Transmitter();

            while (true)
            {
                tr.TransmitAll(
                    TVCodes.SourcesMenu,
                    TVCodes.ArrowDown,
                    TVCodes.ArrowRight);

                Thread.Sleep(500);
                tr.TransmitAll(TVCodes.Ch1);

                Thread.Sleep(500);
                tr.TransmitAll(TVCodes.Ch2);

                Thread.Sleep(500);
            }
        }
    }
}
