using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using MathNet.Numerics.Distributions;
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
            показыватьНомераФреймовToolStripMenuItem.Checked = Default.IsFramesNumbersTextEnabled;
            tableLayoutPanel1.BorderStyle = BorderStyle.FixedSingle;
            tableLayoutPanel2.BorderStyle = BorderStyle.FixedSingle;
            tableLayoutPanel3.BorderStyle = BorderStyle.FixedSingle;
            label1.Text = "";
        }

        List<Label> _stationsUI = new List<Label>();
        List<Label> UIBackloggedPackages = new List<Label>();
        List<List<Label>> UIPackages = new List<List<Label>>();
        List<Station> _stations = new List<Station>();
        private Server _server;

        private void InitializePackagesUI()
        {
            tableLayoutPanel3.Controls.Clear();
            tableLayoutPanel3.RowCount = Default.NumberOfStations;
            tableLayoutPanel3.ColumnCount = Default.NumberOfColums;
            UIPackages.Clear();
            for (var i = 0; i < Default.NumberOfStations; i++)
            {
                var stationPackages = new List<Label>();
                for (var j = 0; j < Default.NumberOfColums; j++)
                {
                    var label = new Label
                    {
                        BorderStyle = BorderStyle.FixedSingle,
                        Height = 30,
                        Width = Default.WidthOfColums,
                        Margin = new Padding(0, 0, 10, 3),
                        BackColor = Color.Transparent,
                        TextAlign = ContentAlignment.MiddleCenter
                    };
                    stationPackages.Add(label);
                }
                UIPackages.Add(stationPackages);
            }
            for (var ii = 0; ii < Default.NumberOfStations; ii++)
            {
                for (var jj = 0; jj < Default.NumberOfColums; jj++)
                {
                    tableLayoutPanel3.Controls.Add(UIPackages[ii][jj]);
                }
            }
        }

        private void InitializeStationsUI()
        {
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.RowCount = Default.NumberOfStations;
            for (var i = 0; i < Default.NumberOfStations; i++)
            {
                var label = new Label
                {
                    BorderStyle = BorderStyle.Fixed3D,
                    Height = 30,
                    Margin = new Padding(0, 0, 3, 3),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font(Font, FontStyle.Bold),
                    Text = $"#{i}"
                };
                _stationsUI.Add(label);
                tableLayoutPanel1.Controls.Add(_stationsUI[i]);

            }
        }

        private void InitializeBackloggedPackagesUI()
        {
            tableLayoutPanel2.Controls.Clear();
            tableLayoutPanel2.RowCount = Default.NumberOfStations;
            for (var i = 0; i < Default.NumberOfStations; i++)
            {
                var label = new Label
                {
                    BorderStyle = BorderStyle.FixedSingle,
                    Height = 30,
                    Margin = new Padding(0, 0, 3, 3),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                UIBackloggedPackages.Add(label);
                tableLayoutPanel2.Controls.Add(UIBackloggedPackages[i]);

            }
        }

        private void InitializeUI()
        {
            InitializePackagesUI();
            InitializeStationsUI();
            InitializeBackloggedPackagesUI();
        }

        private void CreateObjects(int numberOfStations)
        {
            for (var i = 0; i < numberOfStations; i++)
            {
                var station = new Station();
                _stations.Add(station);
            }
            Station.Poisson = new Poisson(Default.Lambda / Default.NumberOfStations);
            Station.Random = new Random();
            _server = new Server();
        }

        private void DestroyObjects()
        {
            _server = null;
            Station.Poisson = null;
            Station.Random = null;
        }

        private void GeneratePackages()
        {
            for (var i = 0; i < Default.NumberOfStations; i++)
                if (!_stations[i].IsPackageExist())
                {
                    _stations[i].GeneratePackage();
                    if (_stations[i].IsPackageExist())
                    {
                        UIBackloggedPackages[i].BackColor = Color.DarkCyan;
                        _stations[i].GenerateBacklogTime();
                    }
                }
            UpdateBackloggedText();
        }

        private void SendPackages()
        {
            for (var i = 0; i < Default.NumberOfStations; i++)
                _server.Frames[i, _server.FramesCounter] = _stations[i].Package(_server.Estimation);
            RepaintPackages(_server.FramesCounter);
        }

        private void DecreaseBacklogTimers()
        {
            for (var i = 0; i < Default.NumberOfStations; i++)
                _stations[i].DecreaseBacklogTime();
        }

        private void CheckCollision()
        {
            if (!_server.IsCollision(_server.FramesCounter))
            {
                for (var i = 0; i < Default.NumberOfStations; i++)
                    if (_server.IsPackageSent(i, _server.FramesCounter))
                    {
                        _stations[i].DestroyPackage();
                        UIBackloggedPackages[i].BackColor = Color.Transparent;
                    }
                _server.CheckEstimationAfterSuccessful();
            }
            else if (_server.IsCollision(_server.FramesCounter))
            {
                for (var i = 0; i < Default.NumberOfStations; i++)
                    if (_server.IsPackageSent(i, _server.FramesCounter))
                    {
                        _stations[i].GenerateBacklogTime();
                        UIBackloggedPackages[i].BackColor = Color.IndianRed;
                    }
                _server.CheckEstimationAfterConflict();
            }
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            SendPackages();
            CheckCollision();
            DecreaseBacklogTimers();
            GeneratePackages();
            _server.IncreaseFrameCounter();
            label1.Text = "Оценка: " + _server.Estimation;
        }

        private void RepaintPackages(int selectedFrame)
        {
            for (var i = 0; i < Default.NumberOfColums && selectedFrame >= 0; i++, selectedFrame--)
                for (var j = 0; j < Default.NumberOfStations; j++)
                {
                    UIPackages[j][i].BackColor = Color.Transparent;
                    if (_server.IsPackageSent(j, selectedFrame))
                    {
                        UIPackages[j][i].BackColor = _server.IsCollision(selectedFrame) ? Color.DarkRed : Color.LimeGreen;
                    }
                    UIPackages[j][i].Text = Default.IsFramesNumbersTextEnabled ? selectedFrame.ToString() : "";
                }
        }

        private void UpdateBackloggedText()
        {
            for (var i = 0; i < Default.NumberOfStations; i++)
            {
                _stationsUI[i].BackColor = Color.Empty;
                UIBackloggedPackages[i].Text = "";
                if (_stations[i].IsPackageExist())
                {
                    UIBackloggedPackages[i].Text = _stations[i].BacklogTime().ToString();
                    if (_stations[i].BacklogTime() == 0 && _stations[i].IsAllow(_server.Estimation))
                    {
                        _stationsUI[i].BackColor = Color.Green;
                    }
                }
            }
        }

        private void стартToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            if (form2.ShowDialog() == DialogResult.OK) ;
            {
                NextButton.Enabled = true;
                сбросToolStripMenuItem.Enabled = true;
                стартToolStripMenuItem.Enabled = false;
                настройкиToolStripMenuItem.Enabled = true;
                InitializeUI();
                CreateObjects(Default.NumberOfStations);
            };

        }

        private void сбросToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NextButton.Enabled = false;
            настройкиToolStripMenuItem.Enabled = false;
            стартToolStripMenuItem.Enabled = true;
            сбросToolStripMenuItem.Enabled = false;
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel2.Controls.Clear();
            tableLayoutPanel3.Controls.Clear();
            _stations.Clear();
            _stationsUI.Clear();
            UIBackloggedPackages.Clear();
            UIPackages.Clear();
            DestroyObjects();
            label1.Text = "";
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void показыватьНомераФлеймовToolStripMenuItem_Click(object sender, EventArgs e)
        {
            показыватьНомераФреймовToolStripMenuItem.Checked = !показыватьНомераФреймовToolStripMenuItem.Checked;
        }

        private void показыватьНомераФлеймовToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            Default.IsFramesNumbersTextEnabled = показыватьНомераФреймовToolStripMenuItem.Checked;
            RepaintPackages(_server.FramesCounter - 1);
        }

        private void крупныйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Default.NumberOfColums = 4;
            Default.WidthOfColums = 140;
            InitializePackagesUI();
            RepaintPackages(_server.FramesCounter - 1);
        }

        private void среднийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Default.NumberOfColums = 8;
            Default.WidthOfColums = 65;
            InitializePackagesUI();
            RepaintPackages(_server.FramesCounter - 1);
        }

        private void мелкийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Default.NumberOfColums = 12;
            Default.WidthOfColums = 40;
            InitializePackagesUI();
            RepaintPackages(_server.FramesCounter - 1);
        }
    }
}
