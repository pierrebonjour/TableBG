using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableBG
{
    static class BusinessConf
    {
        public const Double maxWeightDifferenceToBeConsideredTheSame = 10;
        public const int maxValue = 100; //--> 100 is 10000 the max value for a stability
        public const int stabilityThreshold = 500;
        public const int sizeOfStabilityPointList = 50000; // should be less

        public const int sizeOfLists = 50000;

        public const int sizeOfEntryList = sizeOfLists;
        public const int sizeOfSmoothedList = sizeOfLists;
        public const int sizeOfMinMaxList = sizeOfLists;
        public const int sizeOfAlignmentsList = sizeOfLists;
        public const int sizeOfFreqStabList = sizeOfLists;
        public const int sizeOfAmpStabList = sizeOfLists;
        public const int sizeOfReboundConsolidatedList = sizeOfLists;
        public const int sizeOfSlipList = sizeOfLists;
        public const int sizeOfPerfectFlatList = sizeOfLists;
        public const int sizeOfFreqStabListWIDE = sizeOfLists;
        public const int sizeOfFlatConsolidatedList = sizeOfLists;
        public const int sizeOfOverallConsolidatedList = sizeOfLists;

        //public const Double PerfectFlatMaxAmp = 50.0;
        public const Double PerfectFlatMaxAmp = 30.0;

        public const Double SlipRatio = 1.0 / 5.0;

        public const Double AmpStabRatio = 2.5 / 4.0;
        //public const Double AmpStabRatio = 1 / 15.0;

        public const Double FreqStabRatio = 1.5; // 1.5 --> Si 2 alors droit à 2 ou 3
        public const Double FreqStabRatioWIDE = 3.0;

        public static readonly Double[] smoothFilter = ToolBox.normalizeArray(new Double[]
        {0.25, 0.75, 1.5, 2.5, 3.5, 4.5, 5.5, 6.5, 7.5, 8.5, 9, 9, 8.5, 7.5, 6.5, 5.5, 4.5, 3.5, 2.5, 1.5, 0.75, 0.25});

        public static readonly Flou isExtremelySmall = new Flou(new Double[,] {{0,100}, {0.5,50}, {1.5,0}});

        //public static readonly Flou isExtremelySmall = new Flou(new Double[,] { { 0, 100 }, { 5, 50 }, { 15, 10 } });

        public const int sizeOfStabilityList = 5000;

    }
}
