using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adapted_Slotted_ALOHA
{
    internal class Station
    {
        private bool _backlogStatus;
        private int _backlogTime;
        private Random _random = new Random();

        public int SendPackage()
        {
            return !GetBacklogStatus() ? 1 : 0;
        }

        public int RandomSendPackages()
        {
            return !GetBacklogStatus() ? _random.Next(2) : 0;
        }

        public void EnableBacklogStatus()
        {
            _backlogStatus = true;
            _backlogTime = _random.Next(10);
        }

        public void DisableBacklogStatus()
        {
            _backlogStatus = false;
            _backlogTime = 0;
        }

        public bool GetBacklogStatus()
        {
            return _backlogStatus;
        }

        public int GetBacklogTime()
        {
            return _backlogTime;
        }
    }
}
