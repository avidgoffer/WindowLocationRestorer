using System;
using Microsoft.Win32;

namespace WindowLocationRestorer
{
    public class CheckForWorkstationLocking : IDisposable
    {
        private SessionSwitchEventHandler _sseh;

        private void SysEventsCheck(object sender, SessionSwitchEventArgs e)
        {
            switch(e.Reason)
            {
                case SessionSwitchReason.SessionLock:
                    Console.WriteLine(@"Lock Encountered");
                    break;
                case SessionSwitchReason.SessionUnlock:
                    Console.WriteLine(@"UnLock Encountered");
                    break;
            }
        }

        public void Run()
        {
            _sseh = SysEventsCheck;
            SystemEvents.SessionSwitch += _sseh;
        }

        public void Dispose()
        {
            SystemEvents.SessionSwitch -= _sseh;
        }
    }
}