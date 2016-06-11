using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Adapted_Slotted_ALOHA.Properties;
using MathNet.Numerics.Distributions;

namespace Adapted_Slotted_ALOHA
{
    internal partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            показыватьНомераФреймовToolStripMenuItem.Checked = Settings.Default.IsFramesNumbersTextEnabled;
            tableLayoutPanel1.BorderStyle = BorderStyle.FixedSingle;
            tableLayoutPanel2.BorderStyle = BorderStyle.FixedSingle;
            tableLayoutPanel3.BorderStyle = BorderStyle.FixedSingle;
            показыватьНомераФреймовToolStripMenuItem.CheckOnClick = true;
            крупныйToolStripMenuItem.CheckOnClick = true;
            среднийToolStripMenuItem.CheckOnClick = true;
            мелкийToolStripMenuItem.CheckOnClick = true;
        }

        List<Label> UIstations = new List<Label>();
        List<Label> UIBackloggedPackages = new List<Label>();
        List<List<Label>> UIPackages = new List<List<Label>>();
        List<Station> _stations = new List<Station>();
        private Server _server;
        private Statistics _statistics;

        private void InitializePackagesUI()
        {
            tableLayoutPanel3.Controls.Clear();
            tableLayoutPanel3.RowCount = Settings.Default.NumberOfStations;
            tableLayoutPanel3.ColumnCount = Settings.Default.NumberOfColums;
            UIPackages.Clear();
            for (var i = 0; i < Settings.Default.NumberOfStations; i++)
            {
                var stationPackages = new List<Label>();
                for (var j = 0; j < Settings.Default.NumberOfColums; j++)
                {
                    var label = new Label
                    {
                        BorderStyle = BorderStyle.FixedSingle,
                        Height = 30,
                        Width = Settings.Default.WidthOfColums,
                        Margin = new Padding(0, 0, 10, 3),
                        BackColor = Color.Transparent,
                        TextAlign = ContentAlignment.MiddleCenter
                    };
                    stationPackages.Add(label);
                }
                UIPackages.Add(stationPackages);
            }
            for (var ii = 0; ii < Settings.Default.NumberOfStations; ii++)
            {
                for (var jj = 0; jj < Settings.Default.NumberOfColums; jj++)
                {
                    tableLayoutPanel3.Controls.Add(UIPackages[ii][jj]);
                }
            }
        }

        private void InitializeStationsUI()
        {
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.RowCount = Settings.Default.NumberOfStations;
            for (var i = 0; i < Settings.Default.NumberOfStations; i++)
            {
                var label = new Label
                {
                    BorderStyle = BorderStyle.Fixed3D,
                    Height = 30,
                    Margin = new Padding(0, 0, 3, 3),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font(Font, FontStyle.Bold),
                    Text = $"#{i + 1}"
                };
                UIstations.Add(label);
                tableLayoutPanel1.Controls.Add(UIstations[i]);

            }
        }

        private void InitializeBackloggedPackagesUI()
        {
            tableLayoutPanel2.Controls.Clear();
            tableLayoutPanel2.RowCount = Settings.Default.NumberOfStations;
            for (var i = 0; i < Settings.Default.NumberOfStations; i++)
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
            Station.Poisson = new Poisson(Settings.Default.Lambda / Settings.Default.NumberOfStations);
            Station.Random = new Random();
            _server = new Server();
            _statistics = new Statistics();
        }

        private void DestroyObjects()
        {
            _stations.Clear();
            _server = null;
            Station.Poisson = null;
            Station.Random = null;
            _statistics = null;
        }

        private void GeneratePackages()
        {
            for (var i = 0; i < Settings.Default.NumberOfStations; i++)
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
            for (var i = 0; i < Settings.Default.NumberOfStations; i++)
                _stations[i].GenerateRandomProbability();
        }

        private void SendPackages()
        {
            for (var i = 0; i < Settings.Default.NumberOfStations; i++)
                _server.Frames[i, _server.CurrentFrame] = _stations[i].Package(_server.Estimation);
            RepaintPackages(_server.CurrentFrame);
        }

        private void DecreaseBacklogTimers()
        {
            for (var i = 0; i < Settings.Default.NumberOfStations; i++)
                _stations[i].DecreaseBacklogTime();
        }

        public void IncreasePackagesLifeTime()
        {
            for (var i = 0; i < Settings.Default.NumberOfStations; i++)
            {
                if (_stations[i].IsPackageExist())
                    _stations[i].IncreaseLifeTime();
            }
        }

        private void CheckCollision()
        {
            if (!_server.IsCollision(_server.CurrentFrame))
            {
                for (var i = 0; i < Settings.Default.NumberOfStations; i++)
                    if (_server.IsPackageSent(i, _server.CurrentFrame))
                    {
                        _stations[i].DestroyPackage();
                        _statistics.PackagesLifeTime += _stations[i].LifeTime;
                        _stations[i].ResetLifeTime();
                        _statistics.PackagesLeavedSystem++;
                        UIBackloggedPackages[i].BackColor = Color.Transparent;
                    }
                _server.CheckEstimationAfterSuccessfulOrEmpty();
            }
            else if (_server.IsCollision(_server.CurrentFrame))
            {
                for (var i = 0; i < Settings.Default.NumberOfStations; i++)
                    if (_server.IsPackageSent(i, _server.CurrentFrame))
                    {
                        _stations[i].GenerateBacklogTime();
                        UIBackloggedPackages[i].BackColor = Color.IndianRed;
                    }
                _statistics.Collisions++;
                _server.CheckEstimationAfterConflict();
            }
        }

        private void RepaintPackages(int selectedFrame)
        {
            for (var i = 0; i < Settings.Default.NumberOfColums && selectedFrame >= 0; i++, selectedFrame--)
                for (var j = 0; j < Settings.Default.NumberOfStations; j++)
                {
                    UIPackages[j][i].BackColor = Color.Transparent;
                    if (_server.IsPackageSent(j, selectedFrame))
                    {
                        UIPackages[j][i].BackColor = _server.IsCollision(selectedFrame) ? Color.DarkRed : Color.LimeGreen;
                    }
                    UIPackages[j][i].Text = Settings.Default.IsFramesNumbersTextEnabled ? selectedFrame.ToString() : "";
                }
        }

        private void UpdateBackloggedText()
        {
            for (var i = 0; i < Settings.Default.NumberOfStations; i++)
            {
                UIstations[i].BackColor = Color.Empty;
                UIBackloggedPackages[i].Text = "";
                if (_stations[i].IsPackageExist())
                {
                    UIBackloggedPackages[i].Text = _stations[i].BacklogTime().ToString();
                    if (!_stations[i].IsBacklogged() && _stations[i].IsAllowToSend(_server.Estimation))
                        UIstations[i].BackColor = Color.Green;
                }
            }
        }

        private void UpdateInfo()
        {
            textBox1.Text = $"{_server.CurrentFrame}";
            textBox2.Text = $"{Math.Round(_server.Estimation, 3)}";
            textBox3.Text = $"{_statistics.Collisions}";
            textBox4.Text = $"{Settings.Default.Lambda}";
            textBox5.Text = $"{_statistics.Packages}";
            textBox6.Text = $"{_statistics.PackagesLeavedSystem}";
            textBox7.Text = $"{_statistics.BackloggedPackages()}";
            textBox8.Text = $"{Math.Round(_statistics.AverageOfBackloggedPackages(), 2)}";
            textBox9.Text = $"{Math.Round(_statistics.AverageOfPackagesLifeTime(), 2)}";
        }

        private void CleanInfo()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            textBox7.Clear();
            textBox8.Clear();
            textBox9.Clear();
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            SendPackages();
            CheckCollision();
            DecreaseBacklogTimers();
            GeneratePackages();
            GenerateRandomProbabilities();
            _statistics.IncreaseNumberOfBackloggedFramesAndPackages();
            UpdateBackloggedText();
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
                groupBox1.Enabled = true;
                очиститьToolStripMenuItem.Enabled = true;
                создатьToolStripMenuItem.Enabled = false;
                видToolStripMenuItem.Enabled = true;
                крупныйToolStripMenuItem.Checked = true;
                InitializeUI();
                CreateObjects(Settings.Default.NumberOfStations);
                GeneratePackages();
                GenerateRandomProbabilities();
                UpdateBackloggedText();
                UpdateInfo();
            }
        }

        private void очиститьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NextButton.Enabled = false;
            groupBox1.Enabled = false;
            видToolStripMenuItem.Enabled = false;
            создатьToolStripMenuItem.Enabled = true;
            очиститьToolStripMenuItem.Enabled = false;
            крупныйToolStripMenuItem.Checked = false;
            среднийToolStripMenuItem.Checked = false;
            мелкийToolStripMenuItem.Checked = false;
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel2.Controls.Clear();
            tableLayoutPanel3.Controls.Clear();
            UIstations.Clear();
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
            Settings.Default.IsFramesNumbersTextEnabled = показыватьНомераФреймовToolStripMenuItem.Checked;
            RepaintPackages(_server.CurrentFrame - 1);
        }

        private void крупныйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            среднийToolStripMenuItem.Checked = false;
            мелкийToolStripMenuItem.Checked = false;
            Settings.Default.NumberOfColums = 4;
            Settings.Default.WidthOfColums = 140;
            InitializePackagesUI();
            RepaintPackages(_server.CurrentFrame - 1);
        }

        private void среднийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            крупныйToolStripMenuItem.Checked = false;
            мелкийToolStripMenuItem.Checked = false;
            Settings.Default.NumberOfColums = 8;
            Settings.Default.WidthOfColums = 65;
            InitializePackagesUI();
            RepaintPackages(_server.CurrentFrame - 1);
        }

        private void мелкийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            крупныйToolStripMenuItem.Checked = false;
            среднийToolStripMenuItem.Checked = false;
            Settings.Default.NumberOfColums = 12;
            Settings.Default.WidthOfColums = 40;
            InitializePackagesUI();
            RepaintPackages(_server.CurrentFrame - 1);
        }

        private void анализToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NextButton.Enabled = false;
            groupBox1.Enabled = false;
            видToolStripMenuItem.Enabled = false;
            создатьToolStripMenuItem.Enabled = true;
            очиститьToolStripMenuItem.Enabled = false;
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel2.Controls.Clear();
            tableLayoutPanel3.Controls.Clear();
            UIstations.Clear();
            UIBackloggedPackages.Clear();
            UIPackages.Clear();
            DestroyObjects();
            CleanInfo();
            var form3 = new Form3();
            form3.ShowDialog();
        }
    }
}
