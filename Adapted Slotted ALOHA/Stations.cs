using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adapted_Slotted_ALOHA
{
    class Stations
    {
        public static int MaxStationsNumber { get; } = 100;
        private const int MaxFramesNumber = 1000;
        private static bool[] _backlogStatus = new bool[MaxStationsNumber];
        private static int[] _backlogTime = new int[MaxStationsNumber];
        public static int[,] Frames { get; } = new int[MaxStationsNumber, MaxFramesNumber];
        private static Random _random = new Random();

        private static int GetPackage()
        {
            return _random.Next(2);
        }

        public void SendPackage(int station, int frame)
        {
            if (!GetBacklogStatus(station))
            {
                Frames[station, frame] = 1;
            }
        }

        public void RandomSendPackages(int stationsNumber, int frame)
        {
            for (var i = 0; i < stationsNumber; i++)
            {
                if (GetPackage() == 1)
                    SendPackage(i, frame);
            }
        }

        public void EnableBacklogStatus(int station)
        {
            _backlogStatus[station] = true;
            _backlogTime[station] = _random.Next(10);
        }

        public void DisableBacklogStatus(int station)
        {
            _backlogStatus[station] = false;
            _backlogTime[station] = 0;
        }

        public bool GetBacklogStatus(int station)
        {
            return _backlogStatus[station];
        }

        public int GetBacklogTime(int station)
        {
            return _backlogTime[station];
        }

        public bool CheckCollision(int stationsNumber, int frame)
        {
            var collision = 0;
            for (var i = 0; i < stationsNumber; i++)
            {
                if (Frames[i, frame] == 1)
                {
                    collision++;
                }
            }
            switch (collision)
            {
                case 0:
                    return false;
                case 1:
                    for (var i = 0; i < stationsNumber; i++)
                    {
                        if (Frames[i, frame] == 1)
                            DisableBacklogStatus(i);
                    }
                    return false;
                default:
                    for (var i = 0; i < stationsNumber; i++)
                    {
                        if (Frames[i, frame] == 1)
                            EnableBacklogStatus(i);
                    }
                    return true;
            }
        }
    }
}
