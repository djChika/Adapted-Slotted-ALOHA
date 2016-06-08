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
            comboBox1.SelectedItem = Properties.Settings.Default.NumberOfStations;
            comboBox2.SelectedItem = Properties.Settings.Default.Lambda;
            comboBox3.SelectedItem = Properties.Settings.Default.NumberOfFrames;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1 != null) Properties.Settings.Default.NumberOfStations = Convert.ToInt32(comboBox1.Text);
            else
                MessageBox.Show("Проверьте введенные данные!");
            if (comboBox2.Text != null) Properties.Settings.Default.Lambda = Convert.ToDouble(comboBox2.Text);
            else
                MessageBox.Show("Проверьте введенные данные!");
            if (comboBox3.Text != null) Properties.Settings.Default.NumberOfFrames = Convert.ToInt32(comboBox3.Text);
            else
                MessageBox.Show("Проверьте введенные данные!");
            Close();
        }
    }
}
