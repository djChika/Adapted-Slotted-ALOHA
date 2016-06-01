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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            stations.OnRepaint += Repaint;
            tableLayoutPanel1.BorderStyle = BorderStyle.FixedSingle;
            tableLayoutPanel2.BorderStyle = BorderStyle.FixedSingle;
        }
        Stations stations = new Stations();
        List<Label> _stations = new List<Label>();
        List<Label> _packages = new List<Label>();

        private void GenerateStationsAndSlots(int numberOfStations, int numberOfColums)
        {
            tableLayoutPanel1.RowCount = numberOfStations;
            for (var i = 0; i < numberOfStations; i++)
            {
                var label = new Label
                {
                    BorderStyle = BorderStyle.Fixed3D,
                    Height = 30,
                    Margin = new Padding(0, 0, 3, 3),
                    BackColor = Color.Gray,
                    TextAlign = ContentAlignment.MiddleCenter,
                };
                _stations.Add(label);
                tableLayoutPanel1.Controls.Add(_stations[i]);

            }

            tableLayoutPanel2.RowCount = numberOfStations;
            tableLayoutPanel2.ColumnCount = numberOfColums;
            for (var i = 0; i < numberOfStations * numberOfColums; i++)
            {
                var label = new Label
                {
                    BorderStyle = BorderStyle.FixedSingle,
                    Height = 30,
                    Margin = new Padding(0, 0, 10, 3),
                    BackColor = Color.Transparent,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                _packages.Add(label);
                tableLayoutPanel2.Controls.Add(_packages[i]);
            }
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            GenerateStationsAndSlots(Properties.Settings.Default.NumberOfStations, Properties.Settings.Default.NumberOfColums);
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 4; i++)
            {
                stations.RandomSendPackages(i);
            }
        }

        public void Repaint(int[,] frames)
        {
            MessageBox.Show(frames[1,1].ToString());
        }
    }
}
