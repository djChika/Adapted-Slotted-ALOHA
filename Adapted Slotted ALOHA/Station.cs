using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adapted_Slotted_ALOHA
{
    internal class Station
    {
        private int _package;
        private int _backlogTime;
        private static readonly Random Random = new Random();

        public void GivePackage()
        {
            _package = 1;
        }

        public void GeneratePackage()
        {
            _package = Random.Next(10);
        }

        public void GenerateBacklogTime()
        {
            if (IsPackageExist())
                _backlogTime = Random.Next(10);
        }

        public bool IsPackageExist()
        {
            return _package == 1;
        }

        public void DecreaseBacklogTime()
        {
            if (IsBacklogged())
                _backlogTime--;
        }

        public int BacklogTime()
        {
            return _backlogTime;
        }

        private bool IsBacklogged()
        {
            return _backlogTime != 0;
        }

        public bool IsAllow(double estimation)
        {
            return 1 / estimation >= 1;
        }

        public int Package(double estimation)
        {
            if (IsPackageExist() && !IsBacklogged())
                if (IsAllow(estimation))
                {
                    return _package;
                }
                else
                {
                    GenerateBacklogTime();
                }

            return 0;
        }

        public void DestroyPackage()
        {
            _package = 0;
        }
    }
}
