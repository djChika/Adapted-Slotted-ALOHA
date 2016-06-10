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
            comboBox3.SelectedIndex = 6;
        }

        private Server _server;
        private Statistics _statistics;
        List<Station> _stations = new List<Station>();

        private void CreateObjects(int numberOfStations)
        {
            for (var i = 0; i < numberOfStations; i++)
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
            _server = null;
            Station.Poisson = null;
            Station.Random = null;
            _statistics = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DestroyObjects();
            CreateObjects(Convert.ToInt32(comboBox1.SelectedItem));
            groupBox2.Enabled = true;
        }
    }
}
