using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IguanaRC5;

namespace SiouxTube
{
    /// <summary>
    /// Static routines for controlling the Sioux TV
    /// </summary>
    public class SiouxTV
    {
        private RC5Transmitter remoteControl = new RC5Transmitter();

        public void Initialize()
        {
            remoteControl.Initialize();
        }

        public void SwitchToPCInput()
        {
            try
            {
                remoteControl.TransmitAll(
                    TVCodes.SourcesMenu,
                    TVCodes.ArrowDown,
                    TVCodes.ArrowDown,
                    TVCodes.ArrowDown,
                    TVCodes.ArrowDown,
                    TVCodes.ArrowRight);
            } catch (Exception) { }
        }

        public void SwitchToExt1()
        {
            try
            {
                remoteControl.TransmitAll(
                    TVCodes.SourcesMenu,
                    TVCodes.ArrowDown,
                    TVCodes.ArrowRight);
            } catch (Exception) { }
        }
    }
}
