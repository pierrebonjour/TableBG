using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableBG
{
    class CommunicationHandler
    {
        private List<Double> listToUpdate = null;
        private SerialPort port = null;
        private string remainingData = "";
        private int listSize;

        public CommunicationHandler(List<Double> list, int maxValuesInList, Action < String> callbackInCaseOfFailure = null)
        {
            listSize = maxValuesInList;
            listToUpdate = list;
            try
            {
                port = new SerialPort("COM5", 9600, Parity.None, 8, StopBits.One);
                port.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
                port.Open();
            }
            catch
            {
                if(callbackInCaseOfFailure != null)
                {
                    callbackInCaseOfFailure("Unable to open Com Port");
                }
            }

        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Show all the incoming data in the port's buffer
            string data = port.ReadExisting();
            data = remainingData + data;
            data = data.Replace('.', ',');

            for (; data.Contains(';');)
            {
                string dataAsString = data.Split(';')[0];
                try
                {
                    Double dataInDouble = Convert.ToDouble(dataAsString);
                    ToolBox.insertInListWithMax(listToUpdate, dataInDouble, listSize);
                    Controller.onNewData();
                }
                catch
                {
                    data = "";
                    break;
                }
                if (dataAsString.Length + 1 < data.Length) data = data.Substring(dataAsString.Length + 1);
                else
                {
                    data = "";
                }
            }

            remainingData = data;
        }

    }
}