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
        public int PackagesIsBacklogged { get; set; }

        public int[] NumberOfBackloggedPackages { get; set; } = new int[Properties.Settings.Default.NumberOfFrames];

        public void AddNumberOfBackloggedPackages(int frame)
        {
            NumberOfBackloggedPackages[frame]++;
        }

        public double CalculateAverageNumberOfBackloggedPackages(int frame)
        {
            if(frame>0)
            return (double)NumberOfBackloggedPackages.Sum()/frame;
            return 0;
        }
    }
}
