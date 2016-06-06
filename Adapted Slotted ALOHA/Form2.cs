using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Adapted_Slotted_ALOHA
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            for (int i = 1; i < 20; i++)
            {
                comboBox1.Items.Add(i);
            }
            comboBox1.SelectedItem = Properties.Settings.Default.NumberOfStations;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.NumberOfStations = Convert.ToInt32(comboBox1.SelectedItem);
            Close();
        }
    }
}
