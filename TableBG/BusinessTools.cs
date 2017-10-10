using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TableBG
{
    static class BusinessTools
    {

        static public void UpdateMinAndMaxListsAndAddValueToList(List<Double> inputList, List<Model.Data> MinMaxList, int SizeOfMinMaxList)
        {
            //Update lists 
            foreach (Model.Data dataPoints in MinMaxList)
            {
                dataPoints.x++;
            }

            if (inputList.Count < 3) return; //We need at least 3 values to detect a min or max

            if (inputList[1] <= inputList[0] && inputList[1] <= inputList[2])
            {
                //A potential new minimum has been found, let's confirm it is a real one
                if (MinMaxList.Count == 0) //If list is empty let's create a minimum !
                {
                    //We create a new minimum
                    ToolBox.insertInListWithMax(MinMaxList, new Model.Data(1, new double[] { inputList[1],0,0,0,0,0,0,0,0,0 }, "MIN"), SizeOfMinMaxList);
                    return;
                }
                else
                {
                    //We first make shure that the last entry wasn't a minimum
                    if (MinMaxList[0].descriptor != "MIN")
                    {
                        ToolBox.insertInListWithMax(MinMaxList, new Model.Data(1, new double[] { inputList[1], 0,0, 0,0,0,0,0,0,0 }, "MIN"), SizeOfMinMaxList);
                        return;
                    }

                }
            }

            if (inputList[1] >= inputList[0] && inputList[1] >= inputList[2])
            {
                //A potential new maximum has been found, let's confirm it is a real one
                if (MinMaxList.Count == 0) //If list is empty let's create a maximum !
                {
                    //We create a new maximum
                    ToolBox.insertInListWithMax(MinMaxList, new Model.Data(1, new double[] { inputList[1], 0, 0,0, 0,0, 0,0,0,0 }, "MAX"), SizeOfMinMaxList);
                    return;
                }
                else
                {
                    //We first make shure that the last entry wasn't a minimum
                    if (MinMaxList[0].descriptor != "MAX")
                    {
                        ToolBox.insertInListWithMax(MinMaxList, new Model.Data(1, new double[] { inputList[1], 0, 0,0,0,0,0,0,0,0 }, "MAX"), SizeOfMinMaxList);
                        return;
                    }

                }
            }

        }

        static public void smoothAndAddValueToList(List<Double> listToBeSmoothed, List<Double> smoothedListToBeModified, int SizeOfListToBeModified, Double[] filter)
        {

            int sizeOfFilter = filter.Count();

            if (listToBeSmoothed.Count() < sizeOfFilter)
            {
                ToolBox.insertInListWithMax(smoothedListToBeModified, listToBeSmoothed.Average(), BusinessConf.sizeOfSmoothedList);
                return;
            }

            Double finalValue = 0;

            for (int i = sizeOfFilter - 1; i >= 0; i--)
            {
                finalValue += listToBeSmoothed[i] * filter[i];
            }

            ToolBox.insertInListWithMax(smoothedListToBeModified, finalValue, BusinessConf.sizeOfSmoothedList);
        }


        static public void findStabilityAndAddValueToList(List<Double> listUsedToFindStability, List<Double> stabilityListToBeModified, int sizeOfStabilityList)
        {
            //start with max over 5 values
            if (listUsedToFindStability.Count <= 5)
            {
                ToolBox.insertInListWithMax(stabilityListToBeModified, 0, sizeOfStabilityList);
                return;
            }

            List<Double> compareList = new List<Double>();
            for (int i = 0; i < 5; i++)
            {
                compareList.Add(Math.Abs(listUsedToFindStability[i] - listUsedToFindStability[i + 1]));
            }

            Flou result = new Flou(BusinessConf.isExtremelySmall.x(compareList.Max()));

            ToolBox.insertInListWithMax(stabilityListToBeModified, result.y, sizeOfStabilityList);

        }

        static private Double updateAndReturnAlignment(List<Model.Data> minMaxList)
        {
            if (minMaxList.Count == 0) return 0;
            if (minMaxList[0].x == 1) //If there has been a brand new min or max found !
            {
                //datas[0] -> y du min ou max (renseigné avant)
                //datas[1] -> y du milieu avec le min ou max de gauche, ie le précédent (sauf si y'en a pas)
                //datas[2] -> compteur de milieux alignés avec le min et max suivant et précédent

                if (minMaxList.Count == 1) return 0;
                minMaxList[0].datas[1] = (minMaxList[0].datas[0] + minMaxList[1].datas[0]) / 2; //on renseigne le milieu


                if (minMaxList.Count == 2) return 0;
                //update the previous MinMax.datas[2]
                if (Math.Abs(minMaxList[1].datas[1] - minMaxList[1].datas[0]) < Math.Abs(minMaxList[0].datas[0] - minMaxList[1].datas[0]))
                {
                    minMaxList[1].datas[2] += 1;
                }
                else
                {
                    minMaxList[1].datas[2] = 0;
                }

                if (Math.Abs(minMaxList[2].datas[0] - minMaxList[1].datas[0]) > Math.Abs(minMaxList[0].datas[1] - minMaxList[1].datas[0]))
                {
                    minMaxList[0].datas[2] = minMaxList[1].datas[2] + 1;
                    if (minMaxList[0].datas[2] > BusinessConf.maxValue*2) minMaxList[0].datas[2] = 0;
                }
                else
                {
                    minMaxList[0].datas[2] = 0;
                }

                return (minMaxList[0].datas[2]) / 2;
            }
            else
            {
                //return the last value in the list !
                if (minMaxList.Count > 0)
                {
                    return (minMaxList[0].datas[2]) / 2;
                }
                else return 0;
            }
        }



        static private Double updateAndReturnAmpStability(List<Model.Data> minMaxList)
        {
            if (minMaxList.Count == 0) return 0;
            if (minMaxList[0].x == 1) //If there has been a brand new min or max found !
            {
                //datas[0] -> y du min ou max (renseigné avant)
                //datas[1] -> y du milieu avec le min ou max de gauche, ie le précédent (sauf si y'en a pas)
                //datas[2] -> compteur de milieux alignés avec le min et max suivant et précédent
                //datas[3] -> amplitude entre minMax courant et celui de gauche, ie précent (sauf si y'en a pas)
                //datas[4] -> compteur d'amplitudes qui respectent la regle

                if (minMaxList.Count == 1) return 0;
                minMaxList[0].datas[3] = Math.Abs(minMaxList[0].datas[0] - minMaxList[1].datas[0]); //on renseigne l'amplitude

                if (minMaxList.Count == 2 ) return 0;

                //Si l'amplitude de minmax[0] * AmpStabRatio est inférieure à l'amplitude du minmax[1] alors on incrémente

                if (minMaxList[0].datas[3] * BusinessConf.AmpStabRatio < minMaxList[1].datas[3])
                {
                    minMaxList[0].datas[4] = minMaxList[1].datas[4] + 1;
                    if (minMaxList[0].datas[4] > BusinessConf.maxValue) minMaxList[0].datas[4] = 0;
                }
                else
                {
                    minMaxList[0].datas[4] = 0;
                }

                return minMaxList[0].datas[4];
            }
            else
            {
                //return the last value in the list !
                if (minMaxList.Count > 0)
                {
                    return minMaxList[0].datas[4];
                }
                else return 0;
            }
        }


        static private Double updateAndReturnFreqStability(List<Model.Data> minMaxList)
        {
            if (minMaxList.Count == 0) return 0;
            if (minMaxList[0].x == 1) //If there has been a brand new min or max found !
            {
                //datas[0] -> y du min ou max (renseigné avant)
                //datas[1] -> y du milieu avec le min ou max de gauche, ie le précédent (sauf si y'en a pas)
                //datas[2] -> compteur de milieux alignés avec le min et max suivant et précédent
                //datas[3] -> amplitude entre minMax courant et celui de gauche, ie précent (sauf si y'en a pas)
                //datas[4] -> compteur d'amplitudes qui respectent la regle
                //datas[5] -> diff de nombre de valeures entre le minMax et le précédent (sauf si y'en a pas)
                //datas[6] -> compteur de fréquences qui respectent la regle

                if (minMaxList.Count == 1) return 0;
                minMaxList[0].datas[5] = minMaxList[1].x - minMaxList[0].x; //on renseigne la diff de x

                if (minMaxList.Count == 2) return 0;

                //Si la diff de x < diff de x précédent * FreqStabRatio && diff de x > diff de x précedent / FreqStabRatio alors on incrémente


                if (minMaxList[0].datas[5] <= minMaxList[1].datas[5] * BusinessConf.FreqStabRatio && minMaxList[0].datas[5] >= minMaxList[1].datas[5] / BusinessConf.FreqStabRatio)
                {
                    minMaxList[0].datas[6] = minMaxList[1].datas[6] + 1;
                    if (minMaxList[0].datas[6] > BusinessConf.maxValue) minMaxList[0].datas[6] = 0;
                }
                else
                {
                    minMaxList[0].datas[6] = 0;
                }

                return minMaxList[0].datas[6];
            }
            else
            {
                //return the last value in the list !
                if (minMaxList.Count > 0)
                {
                    return minMaxList[0].datas[6];
                }
                else return 0;
            }
        }



        static private Double updateAndReturnFreqWideStability(List<Model.Data> minMaxList)
        {
            if (minMaxList.Count == 0) return 0;
            if (minMaxList[0].x == 1) //If there has been a brand new min or max found !
            {
                //datas[0] -> y du min ou max (renseigné avant)
                //datas[1] -> y du milieu avec le min ou max de gauche, ie le précédent (sauf si y'en a pas)
                //datas[2] -> compteur de milieux alignés avec le min et max suivant et précédent
                //datas[3] -> amplitude entre minMax courant et celui de gauche, ie précent (sauf si y'en a pas)
                //datas[4] -> compteur d'amplitudes qui respectent la regle
                //datas[5] -> diff de nombre de valeures entre le minMax et le précédent (sauf si y'en a pas)
                //datas[6] -> compteur de fréquences qui respectent la regle
                //datas[7] -> compteur de slips
                //datas[8] -> compteur de stabilité parfaite
                //datas[9] -> compteur de wide freq stab

                if (minMaxList.Count == 1) return 0;
                //minMaxList[0].datas[5] = minMaxList[1].x - minMaxList[0].x; // diff de x deja renseignée

                if (minMaxList.Count == 2) return 0;

                //Si la diff de x < diff de x précédent * FreqStabRatio && diff de x > diff de x précedent / FreqStabRatio alors on incrémente


                if (minMaxList[0].datas[5] <= minMaxList[1].datas[5] * BusinessConf.FreqStabRatioWIDE && minMaxList[0].datas[5] >= minMaxList[1].datas[5] / BusinessConf.FreqStabRatioWIDE)
                {
                    minMaxList[0].datas[9] = minMaxList[1].datas[9] + 1;
                    if (minMaxList[0].datas[9] > BusinessConf.maxValue*4.0) minMaxList[0].datas[9] = 0;
                }
                else
                {
                    minMaxList[0].datas[9] = 0;
                }

                return minMaxList[0].datas[9]/4.0;
            }
            else
            {
                //return the last value in the list !
                if (minMaxList.Count > 0)
                {
                    return minMaxList[0].datas[9]/4.0;
                }
                else return 0;
            }
        }




        static private Double updateAndReturnSlip(List<Model.Data> minMaxList)
        {
            if (minMaxList.Count == 0) return 0;
            if (minMaxList[0].x == 1) //If there has been a brand new min or max found !
            {
                //datas[0] -> y du min ou max (renseigné avant)
                //datas[1] -> y du milieu avec le min ou max de gauche, ie le précédent (sauf si y'en a pas)
                //datas[2] -> compteur de milieux alignés avec le min et max suivant et précédent
                //datas[3] -> amplitude entre minMax courant et celui de gauche, ie précent (sauf si y'en a pas)
                //datas[4] -> compteur d'amplitudes qui respectent la regle
                //datas[5] -> diff de nombre de valeures entre le minMax et le précédent (sauf si y'en a pas)
                //datas[6] -> compteur de fréquences qui respectent la regle
                //datas[7] -> compteur de slips
                //datas[8] -> compteur de stabilité parfaite
                //datas[9] -> compteur de wide freq stab

                if (minMaxList.Count < 4) return 0;

                if ((minMaxList[0].datas[0] < minMaxList[2].datas[0] && minMaxList[1].datas[0] < minMaxList[3].datas[0]) ||
                    (minMaxList[0].datas[0] > minMaxList[2].datas[0] && minMaxList[1].datas[0] > minMaxList[3].datas[0]))
                {
                    Double maxSlip = Math.Abs(minMaxList[3].datas[0] - minMaxList[2].datas[0]) * BusinessConf.SlipRatio;
                    //Le décallage est dans le même sens
                    if ((Math.Abs(minMaxList[0].datas[0] - minMaxList[2].datas[0]) > maxSlip) &&
                        (Math.Abs(minMaxList[1].datas[0] - minMaxList[3].datas[0]) > maxSlip))
                    {
                        return 0;
                    }
                }

                minMaxList[0].datas[7] = minMaxList[1].datas[7] + 1;
                if (minMaxList[0].datas[7] > BusinessConf.maxValue) minMaxList[0].datas[7] = 0;
                return minMaxList[0].datas[7];

            }
            else
            {
                //return the last value in the list !
                if (minMaxList.Count > 0)
                {
                    return minMaxList[0].datas[7];
                }
                else return 0;
            }
        }

        static private Double updateAndReturnPerfectFlat(List<Model.Data> minMaxList)
        {
            if (minMaxList.Count == 0) return 0;
            if (minMaxList[0].x == 1) //If there has been a brand new min or max found !
            {
                //datas[0] -> y du min ou max (renseigné avant)
                //datas[1] -> y du milieu avec le min ou max de gauche, ie le précédent (sauf si y'en a pas)
                //datas[2] -> compteur de milieux alignés avec le min et max suivant et précédent
                //datas[3] -> amplitude entre minMax courant et celui de gauche, ie précent (sauf si y'en a pas)
                //datas[4] -> compteur d'amplitudes qui respectent la regle
                //datas[5] -> diff de nombre de valeures entre le minMax et le précédent (sauf si y'en a pas)
                //datas[6] -> compteur de fréquences qui respectent la regle
                //datas[7] -> compteur de slips
                //datas[8] -> compteur de stabilité parfaite

                if (minMaxList.Count < 4) return 0;

                if (Math.Max(Math.Max(Math.Max(minMaxList[0].datas[0], minMaxList[1].datas[0]), minMaxList[2].datas[0]), minMaxList[3].datas[0]) -
                    Math.Min(Math.Min(Math.Min(minMaxList[0].datas[0], minMaxList[1].datas[0]), minMaxList[2].datas[0]), minMaxList[3].datas[0]) <
                    BusinessConf.PerfectFlatMaxAmp)
                {
                    minMaxList[0].datas[8] = minMaxList[1].datas[8] + 1;

                    if (minMaxList[0].datas[8] > BusinessConf.maxValue * 4.0) minMaxList[0].datas[8] = 0;

                    return minMaxList[0].datas[8]/4.0;
                }

                return 0;

            }
            else
            {
                //return the last value in the list !
                if (minMaxList.Count > 0)
                {
                    return minMaxList[0].datas[8] / 4.0;
                }
                else return 0;
            }
        }
        
        static public void findAlignmentsAndFreqStabilityAndAmpStabilityAndSlipAndFlat(List<Model.Data> minMaxList, List<Double> alignmentsListToBeModified, int sizeOfAlignmentList, List<Double> freqStabListToBeModified, int sizeOfFreqStabList, List<Double> ampStabListToBeModified, int sizeOfAmpStabList, List<Double> slipList, int sizeOfSlipList,List<Double> perfectFlatList, int sizeOfPerfectFlatList, List<Double> freqListWIDE, int sizeOffreqListWIDE, List<Double> reinforcedReboundFinalListToBeModified, int sizeOfReinforcedList, List<Double> reinforcedFlatListToBeModified, int sizeOfFlatReinforcedList, List<Double> overallConsolidatedStabListToBeModified, int sizeOfOverallStabConsoList)
        {
            Double nextAlignmentValue = updateAndReturnAlignment(minMaxList) * 100;
            ToolBox.insertInListWithMax(alignmentsListToBeModified, nextAlignmentValue, sizeOfAlignmentList);
            Double nextAmpStabValue = updateAndReturnAmpStability(minMaxList) * 100;
            ToolBox.insertInListWithMax(ampStabListToBeModified, nextAmpStabValue, sizeOfAmpStabList);
            Double nextFreqStabValue = updateAndReturnFreqStability(minMaxList) * 100;
            ToolBox.insertInListWithMax(freqStabListToBeModified, nextFreqStabValue, sizeOfFreqStabList);
            Double nextSlipValue = updateAndReturnSlip(minMaxList) * 100;
            ToolBox.insertInListWithMax(slipList, nextSlipValue, sizeOfSlipList);

            ToolBox.insertInListWithMax(reinforcedReboundFinalListToBeModified, Math.Min(Math.Min(Math.Min( nextSlipValue,nextAlignmentValue), nextAmpStabValue), nextFreqStabValue), sizeOfReinforcedList);



            Double nextperfectFlatValue = updateAndReturnPerfectFlat(minMaxList) * 100;
            ToolBox.insertInListWithMax(perfectFlatList, nextperfectFlatValue, sizeOfPerfectFlatList);

            Double nextFreqWIDEValue = updateAndReturnFreqWideStability(minMaxList) * 100;
            ToolBox.insertInListWithMax(freqListWIDE, nextFreqWIDEValue, sizeOffreqListWIDE);

            ToolBox.insertInListWithMax(reinforcedFlatListToBeModified, Math.Min(nextperfectFlatValue, nextFreqWIDEValue), sizeOfFlatReinforcedList);

            ToolBox.insertInListWithMax(overallConsolidatedStabListToBeModified, Math.Max(reinforcedFlatListToBeModified[0], reinforcedReboundFinalListToBeModified[0]), sizeOfOverallStabConsoList);

        }

        public static int triggerWhenNewStabilityPointFoundAndUpdateList(List<Double> OverallStabilityConsolidatedList, List<Model.stabilityPoint> stabilityPointList, int sizeOfStabilityPointList)
        {

            //THIS IS WHERE WE WANT TO SEND DATA TO REMOTE SERVER


            if (Controller.aboveThreshold)
            {

                if (OverallStabilityConsolidatedList[0] <= BusinessConf.stabilityThreshold)
                {
                    Controller.aboveThreshold = false;

                    //Compute the weight on the 3 last values
                    Double weight = (Model.MinMaxList[3].datas[1] + Model.MinMaxList[2].datas[1] + Model.MinMaxList[1].datas[1]) / 3.0;


                    // Si les poids sont complettement différents alors on retourne 1 et on fait ci dessous
                    if (stabilityPointList.Count == 0)
                    {
                        ToolBox.insertInListWithMax(stabilityPointList, new Model.stabilityPoint(weight, Controller.stabilityMax, weight, Controller.currentMinimum, Controller.currentMinimumTimeStamp), sizeOfStabilityPointList);
                        return 1;
                    }
                    else
                    {
                        if (Math.Abs(weight - stabilityPointList[0].weight) > BusinessConf.maxWeightDifferenceToBeConsideredTheSame)
                        {
                            ToolBox.insertInListWithMax(stabilityPointList, new Model.stabilityPoint(weight, Controller.stabilityMax, weight, Controller.currentMinimum, Controller.currentMinimumTimeStamp), sizeOfStabilityPointList);
                            return 1;
                        }
                        else
                        {
                            //remplacer la valeur 0
                            Double consolidatedWeight = (stabilityPointList[0].weight + weight) / 2.0;
                            int consolidatedStability = Math.Max(stabilityPointList[0].stabilityEstimation, Controller.stabilityMax);
                            //stabilityPointList[0] = new Model.stabilityPoint(consolidatedWeight, consolidatedStability, weight, Controller.currentMinimum, Controller.currentMinimumTimeStamp);  
                            stabilityPointList[0] = new Model.stabilityPoint(consolidatedWeight, consolidatedStability, weight, stabilityPointList[0].minWeight, stabilityPointList[0].minWeightTimeStamp);
                            return 2;
                        }
                    }

                }
                else
                {
                    //On est dans le cas ou la stabilité est toujours au dessus du threshold, on update le max
                    Double currentStability = (OverallStabilityConsolidatedList[0] - BusinessConf.stabilityThreshold) / 100.0;
                    if (Controller.stabilityMax < currentStability)
                    {
                        //on update le stabilityMax
                        Controller.stabilityMax = Convert.ToInt32(currentStability);
                    }
                }
            }
            else
            {
                Controller.stabilityMax = 0;
                if (OverallStabilityConsolidatedList[0] > BusinessConf.stabilityThreshold)
                {
                    Controller.aboveThreshold = true;
                    Controller.stabilityMax = Convert.ToInt32((OverallStabilityConsolidatedList[0] - BusinessConf.stabilityThreshold) / 100.0);
                }
            }
            return 0;
        }


    }
}
