using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adapted_Slotted_ALOHA.Properties;

namespace Adapted_Slotted_ALOHA
{
    internal class Server
    {
        public int[,] Frames { get; set; } = new int[Settings.Default.NumberOfStations, 1000];
        public int FramesCounter { get; set; }
        public void IncreaseFrameCounter()
        {
            FramesCounter++;
        }

        public bool IsCollision(int frame)
        {
            var collision = 0;
            for (var i = 0; i < Settings.Default.NumberOfStations; i++)
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
                    return false;
                default:
                    return true;
            }
        }

        public bool IsPackageSent(int station, int frame)
        {
            return Frames[station, frame] == 1;
        }

        public double PreviousEstimation { get; set; } = 0;
        public double Estimation { get; set; } = 0;
        public double Lambda { get; set; } = 0.2;

        public void CheckEstimationAfterSuccessful()
        {
            PreviousEstimation = Estimation;
            Estimation = PreviousEstimation + Lambda - 1;
            if (Estimation < Lambda)
                Estimation = Lambda;
        }

        public void CheckEstimationAfterConflict()
        {
            PreviousEstimation = Estimation;
            Estimation = PreviousEstimation + Lambda + 1.39;
        }
    }
}
