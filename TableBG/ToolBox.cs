using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TableBG
{
    static class ToolBox
    {

        public static void insertInListWithMax(List<Model.Data> list, Model.Data value, int max)
        {
            list.Insert(0, value);

            int listC = list.Count;
            if (listC > max)
            {
                for (int i = max; i < listC; i++)
                    list.RemoveAt(list.Count - 1);
            }
        }


        public static void insertInListWithMax(List<Model.stabilityPoint> list, Model.stabilityPoint value, int max)
        {
            list.Insert(0, value);

            int listC = list.Count;
            if (listC > max)
            {
                for (int i = max; i < listC; i++)
                    list.RemoveAt(list.Count - 1);
            }
        }

        public static void insertInListWithMax(List<Double> list, Double value, int max)
        {
                list.Insert(0, value);

                int listC = list.Count;
                if (listC > max)
                {
                    for (int i = max; i < listC; i++)
                        list.RemoveAt(list.Count - 1);
                }
        }




        public static void addRandomToList(List<double> list, int maxSizeOfList, int minValue, int maxValue)
        {
            int diff = maxValue - minValue;
            double lastValue = minValue + diff / 2;
            double beforelastValue = minValue + diff / 2;
            if (list.Count > 0) lastValue = list[0];
            if (list.Count > 1) beforelastValue = list[1];
            double diffReb = Math.Abs(beforelastValue - lastValue) + 0.1;

            Random rand = new Random();
            int biais = rand.Next(Convert.ToInt32(-diff / diffReb), Convert.ToInt32(diff / diffReb));

            int candidate = (lastValue + biais > maxValue || lastValue + biais < minValue) ? Convert.ToInt32(lastValue - biais) : Convert.ToInt32(lastValue + biais);
            candidate = (candidate > maxValue || candidate < minValue) ? Convert.ToInt32(lastValue + biais/100) : candidate;


            if (list.Count>5)
            {
                Double subListAvr = list.GetRange(0, 5).Average();

                insertInListWithMax(list,(candidate + subListAvr*2)/3, maxSizeOfList);
            } else
            {
                insertInListWithMax(list, candidate, maxSizeOfList);
            }
        }

        public static void addRandomDataToEntryList()
        {
            DateTime startTime = DateTime.Now;
            for (;;)
            {
                if ((DateTime.Now - startTime).TotalMilliseconds > SystemConf.randomGenerationIntervalInMs)
                {
                    ToolBox.addRandomToList(Model.entryList, BusinessConf.sizeOfEntryList, SystemConf.randomGenerationMinVal, SystemConf.randomGenerationMaxVal);
                    startTime = DateTime.Now;
                    Controller.onNewData();
                }
            }
        }

        public static void addDumpDataToEntryList()
        {
            String[] dumpContentS = File.ReadAllLines("dump.txt");
            List<Double> doubles = dumpContentS.Select(i => Convert.ToDouble(i)).ToList();
            int sizeOfDoubles = doubles.Count;
            int indexCounter = 0;
            DateTime startTime = DateTime.Now;
            for (;;)
            {
                if ((DateTime.Now - startTime).TotalMilliseconds > SystemConf.DumpDataInjectionIntervalInMs)
                {
                    ToolBox.insertInListWithMax(Model.entryList, doubles[sizeOfDoubles - 1 - indexCounter], BusinessConf.sizeOfEntryList);
                    indexCounter++;
                    if (indexCounter >= sizeOfDoubles) indexCounter = 0;           
                    startTime = DateTime.Now;
                    Controller.onNewData();
                }
            }
        }

        public static Double[] normalizeArray(Double[] array)
        {
                Double[] normArray = array;

                Double sum = normArray.Sum();

                for (int i = normArray.Count() - 1; i >= 0; i--)
                {
                    normArray[i] = normArray[i] / sum;
                }

                return normArray;
        }
    }
}
