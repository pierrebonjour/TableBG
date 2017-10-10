using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TableBG
{
    static class Controller
    {
        public static Double currentMinimum = Double.MaxValue;
        public static DateTime currentMinimumTimeStamp = DateTime.Now;
        public static bool aboveThreshold = false;
        public static int stabilityMax = 0;
        private static GUIHandler guiHandler = null;
        private static CommunicationHandler commHandler = null; //requested not to loose the reference

        public static void StartController(Form1 form)
        {
            guiHandler = new GUIHandler(form);
            //Entry list constantly populated by sensor data (or with random data) and calls onNewData on each update
            commHandler = new CommunicationHandler(Model.entryList, BusinessConf.sizeOfEntryList, CommunicationFailure);
            Thread updateDisplayThread = new Thread(new ThreadStart(guiHandler.updateDisplay));
            updateDisplayThread.Start();
        }

        private static void CommunicationFailure(String error)
        {
            try
            {
                File.ReadAllLines("dump.txt");
                MessageBox.Show("ERROR : " + error + " but dump.txt is present --> using dump.txt !");
                Thread updateWithDumpTxt = new Thread(new ThreadStart(ToolBox.addDumpDataToEntryList));
                updateWithDumpTxt.Start();
            }
            catch
            {
                MessageBox.Show("ERROR : " + error + " & no dump.txt --> Generating random data !");
                Thread updateWithRandomData = new Thread(new ThreadStart(ToolBox.addRandomDataToEntryList));
                updateWithRandomData.Start();
            }

        }

        public static void onNewData()
        {
            if (Model.entryList[0] < currentMinimum)
            {
                currentMinimum = Model.entryList[0];
                currentMinimumTimeStamp = DateTime.Now;
            }

                //Find Min and Max
                BusinessTools.UpdateMinAndMaxListsAndAddValueToList(Model.entryList, Model.MinMaxList, BusinessConf.sizeOfMinMaxList);

            //Extract freq, amp and Alignment stabilities
            BusinessTools.findAlignmentsAndFreqStabilityAndAmpStabilityAndSlipAndFlat(
                Model.MinMaxList, Model.alignmentsList, BusinessConf.sizeOfAlignmentsList, Model.freqStabilityList, BusinessConf.sizeOfFreqStabList,
                Model.ampStabilityList, BusinessConf.sizeOfAmpStabList, Model.verticalSlipList,BusinessConf.sizeOfSlipList,
                Model.perfectFlatList, BusinessConf.sizeOfPerfectFlatList, Model.freqStabilityListWIDE, BusinessConf.sizeOfFreqStabListWIDE,
                Model.reboundConsolidatedList, BusinessConf.sizeOfReboundConsolidatedList,
                Model.FlatConsolidatedList, BusinessConf.sizeOfFlatConsolidatedList, Model.OverallStabilityConsolidatedList, BusinessConf.sizeOfOverallConsolidatedList);



            //Lissage
            /*
            BusinessTools.smoothAndAddValueToList(Model.entryList, Model.smoothList, BusinessConf.sizeOfSmoothedList, BusinessConf.smoothFilter);
            BusinessTools.findStabilityAndAddValueToList(Model.smoothList, Model.stabilityList, BusinessConf.sizeOfStabilityList);
            */

            //We need a global counter for min in the controller and restart it when resUpdate == 1
            //in the triggerWhenNewStabilityPointFoundAndUpdateList we need to send this counter value with its timestamp
            //if resUpdate == 2 check that old min is not lower than actual min

            int resUpdate = BusinessTools.triggerWhenNewStabilityPointFoundAndUpdateList(Model.OverallStabilityConsolidatedList, Model.stabilityPointList, BusinessConf.sizeOfStabilityPointList);
            if(resUpdate == 1 || resUpdate == 2)
            {

                guiHandler.ListToChartSafe("chart3", "Conso", Model.stabilityPointList, Convert.ToInt32(guiHandler.nbPointsToDisplay),true);


                //For chart2 only
                ToolBox.insertInListWithMax(Model.StabilityPointsForGUIOnlyList, Model.stabilityPointList[0].debugLastWeight, BusinessConf.sizeOfStabilityPointList);


                if (resUpdate == 1)
                {
                    currentMinimum = Model.entryList[0];
                    currentMinimumTimeStamp = DateTime.Now;
                }
            }
            else
            {
                ToolBox.insertInListWithMax(Model.StabilityPointsForGUIOnlyList, 0, BusinessConf.sizeOfStabilityPointList);
            }
        }

        public static void onPrePaintChart(object sender, System.Windows.Forms.DataVisualization.Charting.ChartPaintEventArgs e)
        {
            guiHandler.onPrePaintChart(sender, e);
        }


        public static void onPrePaintHisto(object sender, System.Windows.Forms.DataVisualization.Charting.ChartPaintEventArgs e)
        {
            guiHandler.onPrePaintHisto(sender, e);
        }


        public static void addTextToTextBox(String content)
        {
            guiHandler.ChangeTextOfTextBoxSafe("textBox1", content);
        }

    }
}
