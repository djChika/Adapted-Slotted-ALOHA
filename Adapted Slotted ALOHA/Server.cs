using Adapted_Slotted_ALOHA.Properties;

namespace Adapted_Slotted_ALOHA
{
    internal class Server
    {
        public int[,] Frames { get; set; } = new int[1000, Settings.Default.MaxNumberOfFrames];
        public int CurrentFrame { get; set; }
        public void IncreaseCurrentFrameCounter()
        {
            CurrentFrame++;
        }

        public bool IsCollision(int frame)
        {
            var collision = 0;
            for (var i = 0; i < Settings.Default.NumberOfStations; i++)
            {
                if (IsPackageSent(i, frame))
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
            return Frames[station, frame] != 0;
        }

        public double PreviousEstimation { get; set; }
        public double Estimation { get; set; }

        public void CheckEstimationAfterSuccessfulOrEmpty()
        {
            PreviousEstimation = Estimation;
            Estimation = PreviousEstimation + Settings.Default.Lambda - 1;
            if (Estimation < Settings.Default.Lambda)
                Estimation = Settings.Default.Lambda;
        }

        public void CheckEstimationAfterConflict()
        {
            PreviousEstimation = Estimation;
            Estimation = PreviousEstimation + Settings.Default.Lambda + 1.39;
        }
    }
}
