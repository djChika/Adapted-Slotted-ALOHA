using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MathNet.Numerics.Distributions;

namespace Adapted_Slotted_ALOHA
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 1;
            comboBox2.SelectedIndex = 4;
            comboBox3.SelectedIndex = 5;
        }

        private Server _server;
        private Statistics _statistics;
        List<Station> _stations = new List<Station>();

        private void CreateObjects()
        {
            for (var i = 0; i < Convert.ToInt32(comboBox1.Text); i++)
            {
                var station = new Station();
                _stations.Add(station);
            }
            Station.Poisson = new Poisson(Convert.ToDouble(comboBox2.Text) / Convert.ToDouble(comboBox1.Text));
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
            for (var i = 0; i < Convert.ToInt32(comboBox1.Text); i++)
                if (!_stations[i].IsPackageExist())
                {
                    _stations[i].GeneratePackage();
                    if (_stations[i].IsPackageExist())
                    {
                        _stations[i].ResetLifeTime();
                        _statistics.Packages++;
                    }
                }
        }

        private void GenerateRandomProbabilities()
        {
            for (var i = 0; i < Convert.ToInt32(comboBox1.Text); i++)
                _stations[i].GenerateRandomProbability();
        }

        private void SendPackages()
        {
            for (var i = 0; i < Convert.ToInt32(comboBox1.Text); i++)
                _server.Frames[i, _server.CurrentFrame] = _stations[i].Package(_server.Estimation);
        }

        private void DecreaseBacklogTimers()
        {
            for (var i = 0; i < Convert.ToInt32(comboBox1.Text); i++)
                _stations[i].DecreaseBacklogTime();
        }

        public void IncreasePackagesLifeTime()
        {
            for (int i = 0; i < Convert.ToInt32(comboBox1.Text); i++)
            {
                if (_stations[i].IsPackageExist())
                    _stations[i].IncreaseLifeTime();
            }
        }

        private void CheckCollision()
        {
            if (!_server.IsCollision(_server.CurrentFrame))
            {
                for (var i = 0; i < Convert.ToInt32(comboBox1.Text); i++)
                    if (_server.IsPackageSent(i, _server.CurrentFrame))
                    {
                        _stations[i].DestroyPackage();
                        _statistics.PackagesLifeTime += _stations[i].LifeTime;
                        _stations[i].ResetLifeTime();
                        _statistics.PackagesLeavedSystem++;
                    }
                _server.CheckEstimationAfterSuccessfulOrEmpty();
            }
            else if (_server.IsCollision(_server.CurrentFrame))
            {
                for (var i = 0; i < Convert.ToInt32(comboBox1.Text); i++)
                    if (_server.IsPackageSent(i, _server.CurrentFrame))
                    {
                        _stations[i].GenerateBacklogTime();
                        _statistics.Collisions++;
                    }
                _server.CheckEstimationAfterConflict();
            }
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

        private void UpdateInfo()
        {
            textBox1.Text = $"{_statistics.Packages}";
            textBox2.Text = $"{_statistics.PackagesLeavedSystem}";
            textBox3.Text = $"{_statistics.BackloggedPackages()}";
            textBox4.Text = $"{_statistics.Collisions}";
            textBox5.Text = $"{Math.Round(_statistics.AverageOfBackloggedPackages(), 3)}";
            textBox6.Text = $"{Math.Round(_statistics.AverageOfPackagesLifeTime(), 3)}";
        }

        private void Calculation()
        {
            for (int i = 0; i < Convert.ToInt32(comboBox3.Text); i++)
            {
                GeneratePackages();
                GenerateRandomProbabilities();
                SendPackages();
                CheckCollision();
                DecreaseBacklogTimers();
                IncreasePackagesLifeTime();
                _statistics.IncreaseNumberOfBackloggedFramesAndPackages();
                _server.IncreaseCurrentFrameCounter();
            }
            groupBox2.Enabled = true;
            MessageBox.Show("Вычисление прошло успешно!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DestroyObjects();
            CleanInfo();
            CreateObjects();
            Calculation();
            UpdateInfo();
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            DestroyObjects();
        }
    }
}
