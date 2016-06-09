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
            comboBox1.SelectedIndex = 1;
            comboBox2.SelectedIndex = 3;
            comboBox3.SelectedIndex = 6;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(comboBox1.Text) && !string.IsNullOrEmpty(comboBox2.Text) &&
                !string.IsNullOrEmpty(comboBox3.Text))
            {
                Properties.Settings.Default.NumberOfStations = Convert.ToInt32(comboBox1.Text);
                Properties.Settings.Default.Lambda = Convert.ToDouble(comboBox2.Text);
                Properties.Settings.Default.NumberOfFrames = Convert.ToInt32(comboBox3.Text);
                DialogResult = DialogResult.OK;
                Close();
            }
            else
                MessageBox.Show("Проверьте введенные данные.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
