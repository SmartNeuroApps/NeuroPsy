using com.Brainsoft.MatrixVR.EEGAcquisition;
using MatrixAICommon;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Serilog;
using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;
using static System.Net.WebRequestMethods;
using System.IO;
using System.Runtime.InteropServices;

namespace com.Brainsoft.NeurPsy.Experiments
{
    public class EEGReader
    {
        private Thread runner;
        private string experimenttitle;
        private Boolean blnSendEEGRealTime;
        private int sessionCouter = 0;
        private string eeguserName = "";

        private bool isEEgHeadsetInitialized = false;

        public void Begin(ulong index, Boolean isRealTimeEEG, string userName = "")
        {
            if (sessionCouter++ < 1)
            {
                Log.Information("Start EEG session ....");
                End(index);
                blnSendEEGRealTime = isRealTimeEEG; //Set the flag to store the EEG data on the server or not
                eeguserName = userName;

                experimenttitle = $"{index}";
                Log.Information("About to initiate thread on acquireEEGBlocking()");


                runner = new Thread(acquireEEGBlocking);
                runner.Name = "acquireEEGBlocking";
                runner.IsBackground = true;

                runner.Start();

            }
            else
                Log.Information("EEG session is already on.");
        } // end Begin()


        // This method return true if the headset is initialized otherwise false
        public void isEEGHeadsetSubsecribed(bool blnstatus)
        {
            isEEgHeadsetInitialized = blnstatus;
            Log.Information("\nisEEGHeadsetSubsecribed called back status {0}\n\n", blnstatus);
        }


        //A callback method to mark the EEG headset is initialized
        public bool getEEGInitialized()
        {
            return isEEgHeadsetInitialized;
        }
        public async void End(ulong index)
        {
            if (runner != null)
            {
                try
                {

                    EEGDataCollection.endEEGDataCollection();

                    EEGRawToPowerBand powGenerator;   //Power Generator

                    //Create Powgenerator
                    powGenerator = new EEGRawToPowerBand("NeuroPsy");
                    List<double[]> powerBandData = new List<double[]>();

                    string logFileName = EEGDataCollection.getCurrentEEGFileName();
                    powerBandData = powGenerator.generatePowData(logFileName, index.ToString(), "1");

                    EEGDataStorage.reInitialize();//Reintialize the memory for new data storage

                    string strBaseFolder = ConfigurationManager.AppSettings.Get("OutPath");

                    Log.Debug("Date {0}", DateTime.Now);

                    Task<HttpResponseMessage> eegres = EEGDB.Import(index, DateTime.Now, logFileName);

                    string strPBFileName = $"{strBaseFolder}NeuroPsyLog\\Pow\\PowData_{index}1.txt";

                    Task<HttpResponseMessage> pbres = PowerbandDB.Import(index, DateTime.Now, strPBFileName);

                    await eegres;
                    await pbres;

                    runner.Join();
                    runner = null;

                    Log.Information("EEG file {0} and Power band file {1} are stored on the server. The index is {2}. The number of Power band recprds is {3}", logFileName, strPBFileName, index, powerBandData.Count);

                }
                catch (Exception ex)
                {
                    Log.Error("NeuroPsy- End. Exception:{0} ", ex.Message);
                }

            }
        } // end End()

        private void acquireEEGBlocking()
        {
            try

            {

                Log.Information("\n Start acquiring EEG data ....................");

                //Get the name of the experiment
                if (experimenttitle.Length == 0)
                {
                    Log.Error("EEGReader: The name of the experiment is missing.");
                    return;
                }

                Log.Information($"{experimenttitle}: Acquisition of EEG Data.\n Output {ConfigurationManager.AppSettings.Get("OutPath")}");

                // Store the block of EEG data into the microservice
                string[] markers = new string[2];
                markers[0] = "markers";

                //Initialize the EEG storage Write EEG Data to the server
                EEGDataStorage.intializeStorage(experimenttitle, eeguserName, "NeuroPsy", markers, DateTime.Now);

                Log.Information("EEg Headset is initialized");

                //Mark the EEG headset as initialized.

                sessionCouter += 1;

                //Collect data and generate Powerband dataset for command scenario and option state
                string logfilename = EEGDataCollection.collectEEGData("NeuroPsy", experimenttitle, 0, 1, blnSendEEGRealTime, isEEGHeadsetSubsecribed);

                if (logfilename.Length < 1)
                {
                    Log.Information("There is no EEG Data. the file is empty");

                    return;
                }
                else
                {
                    Log.Information("EEG Data for experiment is acquired, file {0} experiment {1}.", logfilename, experimenttitle);
                }

                //Check if data are available

                if (EEGDataStorage.getEEGRowCount() == 0)
                {
                    Log.Information($"{experimenttitle}: There is no EEG Data in the buffer");
                    return;
                }

                DateTime start = DateTime.Now; //Timer

                Log.Information($"At{start} - {experimenttitle}: Computing power bands and saving the results into the database...");

                Log.Information("End of EEG Data acquisition ");

                //Reinitialize the microservice
                EEGDataStorage.reInitialize();
            }
            catch (Exception e)
            {
                Log.Error($"EEG Data acquisition - Error {e.Message} -{e.ToString()}");

            }
        } // end acquireEEGBlocking()


        private static List<double[]> getCurrentPowerBandData(string sScenario, string sState, string logSessionID)
        {
            EEGRawToPowerBand powGenerator;   //Power Generator

            //Create Powgenerator
            powGenerator = new EEGRawToPowerBand(sScenario);
            List<double[]> powerBandData = new List<double[]>();

            string logFileName = EEGDataCollection.getCurrentEEGFileName();
            powerBandData = powGenerator.generatePowData(logFileName, sState, logSessionID);

            return powerBandData;

        } // end getCurrentPowerBandData()

        // Get teh name of the EEG file
        public string getEEGFileName()
        {
            return EEGDataCollection.getCurrentEEGFileName();

        }

        public string getPowerBandFielName()
        {
            return "";//

        }


        private static void computeAndStorePowerBandData(string sScenario, string sState, string logSessionID)
        {
            try
            {
                EEGRawToPowerBand powGenerator;   //Power Generator

                //Create Powgenerator
                powGenerator = new EEGRawToPowerBand(sScenario);
                List<double[]> powerBandData = new List<double[]>();

                string logFileName = EEGDataCollection.getCurrentEEGFileName();
                powerBandData = powGenerator.generatePowData(logFileName, sState, logSessionID);

                int intNumbPowRows = powerBandData.Count;
                int pbInserResult = 0; // Result of inserting teh PB data on the server
                for (int i = 0; i < intNumbPowRows; i++)
                {
                    pbInserResult = EEGDataStorage.storePowerBandRecord("NeuroPsy", "emotiv", powerBandData[i]);
                }

                EEGDataStorage.reInitialize();//Reintialize the memory for new data storage

                Log.Information("Attention - {0} power band records stored on the server.", intNumbPowRows);
                return;

            }
            catch (Exception ex)
            {
                Log.Error("Cognitive load - computeAndStorePowerBandData. Exception: ", ex.Message);
            }
        }


    } // end getCurrentPowerBandData()

} // end class EEGReader

