using System;
using IguanaRC5;
using System.Threading;
using System.ServiceProcess;

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
            try
            {
                remoteControl.Initialize();
            }
            catch (Exception ex)
            {
                // If there is an error about the daemon, try restarting it once
                if (ex.Message.Contains("daemon"))
                {
                    RestartService();
                    remoteControl.Initialize();
                }
            }
        }

        private void RestartService()
        {
            var timeout = TimeSpan.FromSeconds(10);

            var svc = new ServiceController("igdaemon");
            if (svc.Status == ServiceControllerStatus.Running)
            {
                svc.Stop();
                svc.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                Thread.Sleep(1000);
            }

            svc.Start();
            svc.WaitForStatus(ServiceControllerStatus.Running, timeout);
            Thread.Sleep(2000); // Give service some time to get started properly (because it responds too quickly to the SVC queries)
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

                // The TV takes a while to switch, so add some sleep for it to catch up
                Thread.Sleep(8000);
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
