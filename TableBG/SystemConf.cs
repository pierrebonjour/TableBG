using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableBG
{
    static class SystemConf
    {

        

        //Il faut 60s pour parcourir les 5000 valeurs capteurs 1000 = 1Kg
        public static int refreshIntervalInMs = 20;
        public static int randomGenerationIntervalInMs = 10;
        public static int DumpDataInjectionIntervalInMs = 30;
        public static int randomGenerationMinVal = -5;
        public static int randomGenerationMaxVal = 4000;
        public static Double percentageOfinitialListNotDisplayed = 0; //0 --> complete list

    }
}
