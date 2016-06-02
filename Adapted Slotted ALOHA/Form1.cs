using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static Adapted_Slotted_ALOHA.Properties.Settings;
using NLog;

namespace Adapted_Slotted_ALOHA
{
    internal partial class Form1 : Form
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();

        public Form1()
        {
            InitializeComponent();
            tableLayoutPanel1.BorderStyle = BorderStyle.FixedSingle;
            tableLayoutPanel2.BorderStyle = BorderStyle.FixedSingle;
        }

        List<Label> _stationsUI = new List<Label>();
        List<Label> _packagesUI = new List<Label>();
        List<Station> _stations = new List<Station>();
        Server _server = new Server();
        private int currentFrame = 0;

        private void InitializeUI(int numberOfStations, int numberOfColums)
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
                    TextAlign = ContentAlignment.MiddleCenter
                };
                var i1 = i;
                label.Click += delegate { _server.Frames[i1, currentFrame] = _stations[i1].SendPackage(); };
                _stationsUI.Add(label);
                tableLayoutPanel1.Controls.Add(_stationsUI[i]);

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
                _packagesUI.Add(label);
                tableLayoutPanel2.Controls.Add(_packagesUI[i]);
            }
        }

        private void CreateStations(int numberOfStations)
        {
            for (var i = 0; i < numberOfStations; i++)
            {
                var station = new Station();
                _stations.Add(station);
            }
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            InitializeUI(Default.NumberOfStations, Default.NumberOfColums);
            CreateStations(Default.NumberOfStations);
            RepaintUI();
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            currentFrame++;
        }

        private void RepaintUI()
        {

        }

        private void debugButton_Click(object sender, EventArgs e)
        {
            RepaintUI();
        }
    }
}
