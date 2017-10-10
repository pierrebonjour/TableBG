using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace TableBG
{
    class GUIHandler
    {
        private Form1 FormToModify = null;
        public Double nbPointsToDisplay = 0;
        public GUIHandler(Form1 form)
        {
            FormToModify = form;
        }

        private static void DisplayListToChartActualFunction(Chart chart, String NameOfSerie, List<Double> list, int maxDisplayedValues, bool finalHistogram)
        {
            //We remove empty series and the previous serie of the same name

            bool ListAlreadyExisted = false;

            for (int seriesIterator = chart.Series.Count-1;seriesIterator>=0;seriesIterator--)
            {
                if(chart.Series[seriesIterator].Name == NameOfSerie)
                {
                    //clear points only, do not change place
                    chart.Series[seriesIterator].Points.Clear();
                    ListAlreadyExisted = true;
                }
                else if (chart.Series[seriesIterator].Points.Count == 0)
                {
                    //remove
                    chart.Series.Remove(chart.Series[seriesIterator]);
                }
            }
            if (!ListAlreadyExisted)
            {
                chart.Series.Add(NameOfSerie);

                if(finalHistogram) chart.Series[NameOfSerie].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
                else chart.Series[NameOfSerie].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
                //chart.Series[NameOfSerie].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastPoint;
                chart.Series[NameOfSerie].BorderWidth = 1;
            }
            int listCount = list.Count();

            int iteratorStartVal = (maxDisplayedValues == 0) ? listCount - 1 : (listCount > maxDisplayedValues) ? maxDisplayedValues - 1 : listCount - 1;

            for (int listIterator = iteratorStartVal, listInitialCount = listIterator; listIterator >= 0; listIterator--)
            {
                chart.Series[NameOfSerie].Points.Add(new DataPoint(listInitialCount-listIterator,list[listIterator]));
            }
        }

        private static void DisplayListToChartActualFunction(Chart chart, String NameOfSerie, List<Model.stabilityPoint> list, int maxDisplayedValues, bool finalHistogram)
        {
            //We remove empty series and the previous serie of the same name

            bool ListAlreadyExisted = false;

            for (int seriesIterator = chart.Series.Count - 1; seriesIterator >= 0; seriesIterator--)
            {
                if (chart.Series[seriesIterator].Name == NameOfSerie)
                {
                    //clear points only, do not change place
                    chart.Series[seriesIterator].Points.Clear();
                    ListAlreadyExisted = true;
                }
                else if (chart.Series[seriesIterator].Points.Count == 0)
                {
                    //remove
                    chart.Series.Remove(chart.Series[seriesIterator]);
                }
            }
            if (!ListAlreadyExisted)
            {
                chart.Series.Add(NameOfSerie);

                if (finalHistogram) chart.Series[NameOfSerie].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
                else chart.Series[NameOfSerie].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
                //chart.Series[NameOfSerie].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastPoint;
                chart.Series[NameOfSerie].BorderWidth = 1;
            }
            int listCount = list.Count();

            int iteratorStartVal = (maxDisplayedValues == 0) ? listCount - 1 : (listCount > maxDisplayedValues) ? maxDisplayedValues - 1 : listCount - 1;

            for (int listIterator = iteratorStartVal, listInitialCount = listIterator; listIterator >= 0; listIterator--)
            {
                DataPoint pointToDisplay = new DataPoint(listInitialCount - listIterator, list[listIterator].weight);
                pointToDisplay.Color = Color.FromArgb(200 - list[listIterator].stabilityEstimation * 2, 200 - list[listIterator].stabilityEstimation * 2, 200 - list[listIterator].stabilityEstimation*2);
                chart.Series[NameOfSerie].Points.Add(pointToDisplay);
            }
        }

        public delegate void DisplayListToChartDelegate(Chart chart, String NameOfSerie, List<Double> list, int maxDisplayedValues, bool finalHistogram);
        public delegate void DisplayListToChartDelegateStability(Chart chart, String NameOfSerie, List<Model.stabilityPoint> list, int maxDisplayedValues, bool finalHistogram);
        public DisplayListToChartDelegate DisplayListToChart = delegate (Chart chart, String NameOfSerie, List<Double> list, int maxDisplayedValues, bool finalHistogram)
        {
            DisplayListToChartActualFunction(chart, NameOfSerie, list, maxDisplayedValues, finalHistogram);
        };

        public DisplayListToChartDelegateStability DisplayListToChartStability = delegate (Chart chart, String NameOfSerie, List<Model.stabilityPoint> list, int maxDisplayedValues, bool finalHistogram)
        {
            DisplayListToChartActualFunction(chart, NameOfSerie, list, maxDisplayedValues, finalHistogram);
        };
        public void ListToChartSafe(String chartName, String NameOfSerie, List<Double> list, int maxDisplayedValues = 0, bool finalHistogram = false)
        {
            Chart chart = (Chart)FormToModify.Controls[chartName];

            if (chart.InvokeRequired)
            {
               chart.Invoke(DisplayListToChart, chart, NameOfSerie, list, maxDisplayedValues, finalHistogram);
            }
            else
            {
                DisplayListToChartActualFunction(chart, NameOfSerie, list, maxDisplayedValues, finalHistogram);
            }
        }

        public void ListToChartSafe(String chartName, String NameOfSerie, List<Model.stabilityPoint> list, int maxDisplayedValues = 0, bool finalHistogram = false)
        {
            Chart chart = (Chart)FormToModify.Controls[chartName];

            if (chart.InvokeRequired)
            {
                chart.Invoke(DisplayListToChartStability, chart, NameOfSerie, list, maxDisplayedValues, finalHistogram);
            }
            else
            {
                DisplayListToChartActualFunction(chart, NameOfSerie, list, maxDisplayedValues, finalHistogram);
            }
        }

        private delegate void ModifyTextBox(System.Windows.Forms.TextBox textBox, string text);
        private ModifyTextBox ChangeTextOfTextBoxDelegate = delegate (System.Windows.Forms.TextBox textBox, string text)
        {
            textBox.Text = text;
        };
        public void ChangeTextOfTextBoxSafe(String textBoxName, string text)
        {
            try
            {
                TextBox textBox = (TextBox)FormToModify.Controls[textBoxName];

                if (textBox.InvokeRequired)
                {
                    textBox.Invoke(ChangeTextOfTextBoxDelegate, textBox, text);
                }
                else
                {
                    textBox.Text = text;
                }
            }
            catch
            {

            }
        }


        public void onPrePaintHisto(object sender, ChartPaintEventArgs e)
        {
            Double indexOfPointFarRight = Math.Min((BusinessConf.sizeOfStabilityPointList - (SystemConf.percentageOfinitialListNotDisplayed / 100) * BusinessConf.sizeOfStabilityPointList), Model.stabilityPointList.Count);

            using (Pen pen = new Pen(Color.Blue, 1.5f))
            {
                for (int count = Model.stabilityPointList.Count, i = 0; i < count; i++)
                {
                    Model.stabilityPoint dataPoint = Model.stabilityPointList[i];
                    if (i > indexOfPointFarRight) break;

                    Double xPosUsed = indexOfPointFarRight - 1 - i;
                    if (Model.stabilityPointList.Count == 1)
                    {
                        xPosUsed = 1;
                    }
                    Double x = e.Chart.ChartAreas[0].AxisX.ValueToPixelPosition(xPosUsed);
                    Double y = e.Chart.ChartAreas[0].AxisY.ValueToPixelPosition(dataPoint.minWeight);

                    Double yw = e.Chart.ChartAreas[0].AxisY.ValueToPixelPosition(dataPoint.weight);

                    e.ChartGraphics.Graphics.DrawLine(pen, (float)x, (float)yw, (float)x, (float)y);
                    e.ChartGraphics.Graphics.DrawLine(pen, (float)x-2, (float)y, (float)x+2, (float)y);
                }

            }
        }

            public void onPrePaintChart(object sender, ChartPaintEventArgs e)
        {
            Double indexOfPointFarRight = Math.Min((BusinessConf.sizeOfEntryList - (SystemConf.percentageOfinitialListNotDisplayed / 100) * BusinessConf.sizeOfEntryList), Model.entryList.Count);

            //ARROW
            using (Pen pen = new Pen(Color.Blue, 1.5f))
            {
                Double x = e.Chart.ChartAreas[0].AxisX.ValueToPixelPosition(indexOfPointFarRight-1);
                Double y = e.Chart.ChartAreas[0].AxisY.ValueToPixelPosition(Model.entryList[0]);
                e.ChartGraphics.Graphics.DrawLine(pen, (float)x + 10, (float)y - 10, (float)x, (float)y);
                e.ChartGraphics.Graphics.DrawLine(pen, (float)x + 3, (float)y, (float)x, (float)y);
                e.ChartGraphics.Graphics.DrawLine(pen, (float)x, (float)y - 3, (float)x, (float)y);
            }

            //MINS AND MAXS
            for(int count = Model.MinMaxList.Count, i=0;i<count; i++)
            {
                Model.Data dataPoint = Model.MinMaxList[i];
                if (dataPoint.x > indexOfPointFarRight) break;
                Double x = e.Chart.ChartAreas[0].AxisX.ValueToPixelPosition(indexOfPointFarRight - 1 - dataPoint.x);
                Double y = e.Chart.ChartAreas[0].AxisY.ValueToPixelPosition(dataPoint.datas[0]);
                if (dataPoint.descriptor == "MIN")
                {
                    using (Pen pen = new Pen(Color.Green, 1.5f))
                    {
                        e.ChartGraphics.Graphics.DrawLine(pen, (float)x + 1, (float)y + 1, (float)x, (float)y);
                        e.ChartGraphics.Graphics.DrawLine(pen, (float)x - 1, (float)y + 1, (float)x, (float)y);
                        e.ChartGraphics.Graphics.DrawLine(pen, (float)x, (float)y + 5, (float)x, (float)y);
                    }
                }
                else if (dataPoint.descriptor == "MAX")
                {
                    using (Pen pen = new Pen(Color.Red, 1.5f))
                    {
                        e.ChartGraphics.Graphics.DrawLine(pen, (float)x + 1, (float)y - 1, (float)x, (float)y);
                        e.ChartGraphics.Graphics.DrawLine(pen, (float)x - 1, (float)y - 1, (float)x, (float)y);
                        e.ChartGraphics.Graphics.DrawLine(pen, (float)x, (float)y - 5, (float)x, (float)y);
                    }
                }
            }


        }


        public void updateDisplay()
        {
            DateTime startTime = DateTime.Now;
            for (;;)
            {
                if ((DateTime.Now - startTime).TotalMilliseconds > SystemConf.refreshIntervalInMs)
                {
                    nbPointsToDisplay = BusinessConf.sizeOfEntryList - (SystemConf.percentageOfinitialListNotDisplayed / 100) * BusinessConf.sizeOfEntryList;
                    ListToChartSafe("chart1", "raw input", Model.entryList, Convert.ToInt32(nbPointsToDisplay));
                    //ListToChartSafe("chart1", "smoothed", Model.smoothList, Convert.ToInt32(nbPointsToDisplay));



                    /* OTHER OPTIONALS
                    ListToChartSafe("chart1", "freq", Model.freqStabilityList, Convert.ToInt32(nbPointsToDisplay));
                    ListToChartSafe("chart1", "alignments", Model.alignmentsList, Convert.ToInt32(nbPointsToDisplay));
                    ListToChartSafe("chart1", "amps", Model.ampStabilityList, Convert.ToInt32(nbPointsToDisplay));           
                    ListToChartSafe("chart1", "slip", Model.verticalSlipList, Convert.ToInt32(nbPointsToDisplay));
                    ListToChartSafe("chart1", "freq WIDE", Model.freqStabilityListWIDE, Convert.ToInt32(nbPointsToDisplay));
                    ListToChartSafe("chart1", "perfect Flat", Model.perfectFlatList, Convert.ToInt32(nbPointsToDisplay));
                    */

                    //*****THE TWO MAIN COMPONENTS :

                    //---> ListToChartSafe("chart1", "Flat Consolidated", Model.FlatConsolidatedList, Convert.ToInt32(nbPointsToDisplay));
                    //ListToChartSafe("chart1", "consolidated Rebound", Model.reboundConsolidatedList, Convert.ToInt32(nbPointsToDisplay));


                    ListToChartSafe("chart1", "Stability", Model.OverallStabilityConsolidatedList, Convert.ToInt32(nbPointsToDisplay));
                    ListToChartSafe("chart2", "sw", Model.StabilityPointsForGUIOnlyList, Convert.ToInt32(nbPointsToDisplay));


                    startTime = DateTime.Now;
                }
            }
        }

    }
}
