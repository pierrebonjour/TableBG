using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TableBG
{
    static class Model
    {
        public static List<double> entryList = new List<double>();
        //old stability shit
        public static List<double> smoothList = new List<double>();
        public static List<double> stabilityList = new List<double>();
        ////////////////////

        public static List<double> alignmentsList = new List<double>();
        public static List<double> freqStabilityList = new List<double>();
        public static List<double> ampStabilityList = new List<double>();
        public static List<double> verticalSlipList = new List<double>();
        public static List<double> perfectFlatList = new List<double>();
        public static List<double> freqStabilityListWIDE = new List<double>();

        public static List<double> reboundConsolidatedList = new List<double>();
        public static List<double> FlatConsolidatedList = new List<double>();

        public static List<double> OverallStabilityConsolidatedList = new List<double>();
        public static List<double> StabilityPointsForGUIOnlyList = new List<double>();


        public class Data
        {
            public Double x = 0;
            public String descriptor = "";
            public Double[] datas;
            public Data(Double x, Double[] datas, String descriptor="")
            {
                this.x = x;
                this.datas = datas;
                this.descriptor = descriptor;
            }
        }

        public static List<Data> MinMaxList = new List<Data>();

        public class stabilityPoint
        {
            public Double weight;
            public Double debugLastWeight;
            public DateTime lastRecordedDate;
            public int stabilityEstimation;
            public Double minWeight;
            public DateTime minWeightTimeStamp;

            public stabilityPoint(Double weight, int stabilityEstimation, Double debugLastWeight, Double minWeight, DateTime minWeightTimeStamp)
            {
                this.weight = weight;
                this.debugLastWeight = debugLastWeight;
                this.lastRecordedDate = DateTime.Now;
                this.stabilityEstimation = stabilityEstimation;
                this.minWeight = minWeight;
                this.minWeightTimeStamp = minWeightTimeStamp;
            }
        }

        public static List<stabilityPoint> stabilityPointList = new List<stabilityPoint>();
    }
}
