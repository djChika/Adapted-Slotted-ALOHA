using System;
using System.Windows.Forms;
using Adapted_Slotted_ALOHA.Properties;
using MathNet.Numerics.Distributions;

namespace Adapted_Slotted_ALOHA
{
    internal class Station
    {
        private int _package;
        private int _backlogTime;
        public static Poisson Poisson;
        public static Random Random = new Random();

        public void GivePackage()
        {
            _package = 1;
        }

        public void GeneratePackage()
        {
            _package = Poisson.Sample();
        }

        public void GenerateBacklogTime()
        {
            if (IsPackageExist())
                _backlogTime = Random.Next(1, 20);
        }

        public bool IsPackageExist()
        {
            return _package != 0;
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
            return Random.NextDouble() <= 1 / estimation;
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
