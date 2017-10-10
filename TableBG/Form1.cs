using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TableBG
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void onPrePaintChart(object sender, System.Windows.Forms.DataVisualization.Charting.ChartPaintEventArgs e)
        {
            Controller.onPrePaintChart(sender, e);
        }

        private void onPrePaintHisto(object sender, System.Windows.Forms.DataVisualization.Charting.ChartPaintEventArgs e)
        {
            Controller.onPrePaintHisto(sender, e);
        }

    private void trackBar1_Scroll(object sender, EventArgs e)
        {
            SystemConf.percentageOfinitialListNotDisplayed = trackBar1.Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<String> doubles2 = Model.stabilityPointList.Select(i => i.weight.ToString() + "with min = " + i.minWeight.ToString()).ToList();
            File.WriteAllLines("conso.txt", doubles2.ToArray());
            List<String> doubles = Model.entryList.Select(i => i.ToString()).ToList();
            File.WriteAllLines("dump.txt", doubles.ToArray());

        }

    }
}
