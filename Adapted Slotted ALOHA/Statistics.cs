using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Adapted_Slotted_ALOHA
{
    class Statistics
    {
        public int Packages { get; set; }
        public int PackagesLeavedSystem { get; set; }
        public int PackagesLifeTime { get; set; }
        public int NumberOfBackloggedPackages { get; set; }

        public int BackloggedPackages()
        {
            return Packages - PackagesLeavedSystem;
        }

        public void RegisterNumberOfBackLoggedPackages()
        {
            NumberOfBackloggedPackages += Packages - PackagesLeavedSystem;
        }

        public double AverageOfBackloggedPackages(int frame)
        {
            if (frame > 0)
                return (double)NumberOfBackloggedPackages / frame;
            return 0;
        }

        public double AverageOfPackagesLifeTime()
        {
            if (PackagesLeavedSystem > 0)
                return (double)PackagesLifeTime / PackagesLeavedSystem;
            return 0;
        }
    }
}
