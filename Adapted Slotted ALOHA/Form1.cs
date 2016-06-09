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
            показыватьНомераФреймовToolStripMenuItem.CheckOnClick = true;
            крупныйToolStripMenuItem.CheckOnClick = true;
            среднийToolStripMenuItem.CheckOnClick = true;
            мелкийToolStripMenuItem.CheckOnClick = true;
        }

        List<Label> _stationsUI = new List<Label>();
        List<Label> UIBackloggedPackages = new List<Label>();
        List<List<Label>> UIPackages = new List<List<Label>>();
        List<Station> _stations = new List<Station>();
        private Server _server;
        private Statistics _statistics;

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
            _statistics = new Statistics();
        }

        private void DestroyObjects()
        {
            _server = null;
            Station.Poisson = null;
            Station.Random = null;
            _statistics = null;
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
                        _stations[i].ResetLifeTime();
                        _statistics.Packages++;
                    }
                }
        }

        private void GenerateRandomProbabilities()
        {
            for (var i = 0; i < Default.NumberOfStations; i++)
                _stations[i].GenerateRandomProbability();
        }

        private void SendPackages()
        {
            for (var i = 0; i < Default.NumberOfStations; i++)
                _server.Frames[i, _server.CurrentFrame] = _stations[i].Package(_server.Estimation);
            RepaintPackages(_server.CurrentFrame);
        }

        private void DecreaseBacklogTimers()
        {
            for (var i = 0; i < Default.NumberOfStations; i++)
                _stations[i].DecreaseBacklogTime();
        }

        public void IncreasePackagesLifeTime()
        {
            for (int i = 0; i < Default.NumberOfStations; i++)
            {
                if (_stations[i].IsPackageExist())
                    _stations[i].IncreaseLifeTime();
            }
        }

        private void CheckCollision()
        {
            if (!_server.IsCollision(_server.CurrentFrame))
            {
                for (var i = 0; i < Default.NumberOfStations; i++)
                    if (_server.IsPackageSent(i, _server.CurrentFrame))
                    {
                        _stations[i].DestroyPackage();
                        _statistics.PackagesLifeTime += _stations[i].LifeTime;
                        _stations[i].ResetLifeTime();
                        UIBackloggedPackages[i].BackColor = Color.Transparent;
                        _statistics.PackagesLeavedSystem++;
                    }
                _server.CheckEstimationAfterSuccessfulOrEmpty();
            }
            else if (_server.IsCollision(_server.CurrentFrame))
            {
                for (var i = 0; i < Default.NumberOfStations; i++)
                    if (_server.IsPackageSent(i, _server.CurrentFrame))
                    {
                        _stations[i].GenerateBacklogTime();
                        UIBackloggedPackages[i].BackColor = Color.IndianRed;
                    }
                _server.CheckEstimationAfterConflict();
            }
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
                    if (!_stations[i].IsBacklogged() && _stations[i].IsAllowToSend(_server.Estimation))
                        _stationsUI[i].BackColor = Color.Green;
                }
            }
        }

        private void UpdateInfo()
        {
            if (_server != null) label1.Text = $"Оценка: {Math.Round(_server.Estimation, 3)}";
            textBox1.Text = $"{_statistics.Packages}";
            textBox2.Text = $"{_statistics.PackagesLeavedSystem}";
            textBox3.Text = $"{_statistics.BackloggedPackages()}";
            textBox4.Text = $"{Math.Round(_statistics.AverageOfBackloggedPackages(_server.CurrentFrame))}";
            textBox6.Text = $"{Math.Round(_statistics.AverageOfPackagesLifeTime())}";
        }

        private void CleanInfo()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            SendPackages();
            CheckCollision();
            DecreaseBacklogTimers();
            GeneratePackages();
            GenerateRandomProbabilities();
            UpdateBackloggedText();
            _statistics.RegisterNumberOfBackLoggedPackages();
            UpdateInfo();
            IncreasePackagesLifeTime();
            _server.IncreaseCurrentFrameCounter();

        }

        private void создатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form2 = new Form2();
            if (form2.ShowDialog() == DialogResult.OK)
            {
                NextButton.Enabled = true;
                очиститьToolStripMenuItem.Enabled = true;
                создатьToolStripMenuItem.Enabled = false;
                видToolStripMenuItem.Enabled = true;
                анализToolStripMenuItem.Enabled = true;
                крупныйToolStripMenuItem.Checked = true;
                InitializeUI();
                CreateObjects(Default.NumberOfStations);
                GeneratePackages();
                GenerateRandomProbabilities();
                UpdateBackloggedText();
                UpdateInfo();
            }
        }

        private void очиститьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NextButton.Enabled = false;
            видToolStripMenuItem.Enabled = false;
            создатьToolStripMenuItem.Enabled = true;
            очиститьToolStripMenuItem.Enabled = false;
            анализToolStripMenuItem.Enabled = false;
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel2.Controls.Clear();
            tableLayoutPanel3.Controls.Clear();
            _stations.Clear();
            _stationsUI.Clear();
            UIBackloggedPackages.Clear();
            UIPackages.Clear();
            DestroyObjects();
            CleanInfo();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void показыватьНомераФреймовToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            Default.IsFramesNumbersTextEnabled = показыватьНомераФреймовToolStripMenuItem.Checked;
            RepaintPackages(_server.CurrentFrame - 1);
        }

        private void крупныйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            среднийToolStripMenuItem.Checked = false;
            мелкийToolStripMenuItem.Checked = false;
            Default.NumberOfColums = 4;
            Default.WidthOfColums = 140;
            InitializePackagesUI();
            RepaintPackages(_server.CurrentFrame - 1);
        }

        private void среднийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            крупныйToolStripMenuItem.Checked = false;
            мелкийToolStripMenuItem.Checked = false;
            Default.NumberOfColums = 8;
            Default.WidthOfColums = 65;
            InitializePackagesUI();
            RepaintPackages(_server.CurrentFrame - 1);
        }

        private void мелкийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            крупныйToolStripMenuItem.Checked = false;
            среднийToolStripMenuItem.Checked = false;
            Default.NumberOfColums = 12;
            Default.WidthOfColums = 40;
            InitializePackagesUI();
            RepaintPackages(_server.CurrentFrame - 1);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
