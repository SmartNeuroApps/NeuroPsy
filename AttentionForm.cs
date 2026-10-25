using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
//using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using com.Brainsoft.MatrixVR.EEGAcquisition;
using MathNet.Numerics;
using Serilog;

namespace com.Brainsoft.NeurPsy.Experiments
{
    public partial class AttentionForm : CharSequenceForm
    {
        //======================================================================
        //----------------------------------------------------------------------
        // CONSTRUCTORS/DESTRUCTORS
        //----------------------------------------------------------------------
        //======================================================================
        public AttentionForm() : base()
        {
            InitializeComponent();

            charLabel.MouseClick += charLabel_MouseClick;
            eegReader = new EEGReader();
        } // end constructor()

        //======================================================================
        //----------------------------------------------------------------------
        // PRIVATE MEMBER VARIABLES
        //----------------------------------------------------------------------
        //======================================================================
        //private static readonly int charRange = (('9' - '0') + 1) + (('Z' - 'A') + 1);

        private char[] spotSeq;
        private DateTime spotTime;
        private int iCurrentSeqPos;
        int sizeSequence; //Size of teh sequence
        int numMartches; // Number of matches

        private Attention attndata = new Attention();
        private EEGReader eegReader = new EEGReader();
        private List<double> matchTimes;
        private List<double> latency_times;
        private List<double> falsePositives;
        private bool[] detectSuccess;

        private List<char> chResponseTimes;
        private List<char> chFalsePositives;
        private List<char> chMatchTimes;

        private int matchCounter = 0; // counter for the matches
        //======================================================================
        //----------------------------------------------------------------------
        // PRIVATE METHODS
        //----------------------------------------------------------------------
        //======================================================================

        //----------------------------------------------------------------------

        /*
         *  Description: This method generates a sequence of characters such that
         *  the character at "AttentionToFindSequence" is repeated the number 
         *  of times specified by configuration variable NumberofMatches
         *  Typically we look for 144 "AX" in 192 sequence.
         *  
         */
        private void GenerateSequences()
        {
            try
            {



                Sequence = new char[sizeSequence]; // sizeSequence characters
                int[] intArray = new int[sizeSequence - numMartches];
                int iSeqCount = 0; //Cpunts the number of sequences that we are looking for.

                Random rand = new Random();
                int nRandom = (sizeSequence - numMartches) / 2;
                numMartches = numMartches / 2; // Caracters are inserted in pairs. We need NumMatches of AX 

                //Size the arrays to get teh data
                matchTimes = new List<double>(numMartches);
                latency_times = new List<double>(numMartches);
                falsePositives = new List<double>(numMartches);
                detectSuccess = new bool[numMartches]; //Track success or failure of each occurrence
                chResponseTimes = new List<char>(numMartches);
                chFalsePositives = new List<char>(numMartches);
                chMatchTimes = new List<char>(numMartches);

                intArray = Enumerable.Range(0, sizeSequence).OrderBy(t => rand.Next()).Take(intArray.Length).ToArray();
                Array.Sort(intArray);

                int i = 0; //Index in the array of random numbers

                for (int pos = 0; pos < Sequence.Length; pos += 2)
                {

                    if (intArray[i] % 2 != 0) // Ensure that we read even numbers
                        intArray[i] += 1;

                    if (pos == intArray[i])
                    {
                        Sequence[pos] = (char)('A' + (intArray[i] % 26));
                        Sequence[pos + 1] = (char)('A' + (intArray[i + 1] % 26));
                        if (i < (intArray.Length - 2))
                            i += 2; //Move to the next random numbers
                    }
                    else
                    {
                        Sequence[pos] = spotSeq[0];
                        Sequence[pos + 1] = spotSeq[1];
                        iSeqCount++;
                    }

                }
            }
            catch (Exception ex)
            { Log.Error("Attention - Generate sequence {0}", ex.StackTrace); }
        }// end GenerateSequences()


        //This function returns the number of matches
        private int verifySeqSpot(char[] sequence, char[] secSpot)
        {
            int nMatches = 0;
            for (int i = 0; i < (sequence.Length - 1); i++)
            {

                if ((sequence[i] == secSpot[0]) && (sequence[i + 1] == secSpot[1]))
                    nMatches++;
            }

            return nMatches;
        }

        private double computeAvglatencyTime(double[] ResponseTimes)
        {
            double avgresponseTime = 0;
            int notcount = 0;
            //

            for (int i = 0; i < latency_times.Count(); i++)
            {
                //Disregard items that exceded the diplay time. These are due to processing freeze
                //if (latency_times[i] <= DisplayInterval)
                    
                avgresponseTime += latency_times[i];
                
                //else
                 //   notcount++;
            }

            return avgresponseTime / latency_times.Count();
        }
        //----------------------------------------------------------------------
        private void OnCharUpdate(int pos)
        {

            iCurrentSeqPos = pos; //Save the position for tracking 
            double difft = 0;
            // SpotTime includes current time 
            // A null value indicates that there is no match, which would be used to identify false positives
            if (pos > 0 && Sequence[pos - 1] == spotSeq[0] && Sequence[pos] == spotSeq[1])
            {

                spotTime = DateTime.Now;
                //Log.Information("Attention - Match {0}{1} at {2}", Sequence[pos - 1], Sequence[pos], spotTime);
                difft = (spotTime - attndata.Timestamp).TotalMilliseconds;

                matchTimes.Add(difft);
                chMatchTimes.Add(Sequence[pos]); // Add the Characters for checking

                detectSuccess[matchCounter] = false; //mark as there is a match but not detected.
                matchCounter++;

            }
            else
                spotTime = attndata.Timestamp;

            GC.Collect();
        } // end OnCharUpdate()

        //----------------------------------------------------------------------
        private async void OnEndExperiment()
        {
            var x = DateTime.Now - attndata.Timestamp;
            Log.Debug("End time {0} - diff {1} - sequence size {2}", DateTime.Now, x, Sequence.Length);

            // Show the end of the experiments
            charLabel.Text = "End ... Thank you";
            charLabel.Refresh();

            attndata.SubjectName = subjectNameTextBox.Text;

            attndata.MatchTimes = matchTimes.ToArray();
            attndata.latency_times = latency_times.ToArray();
            attndata.FalsePositives = falsePositives.ToArray();
            attndata.detectSuccess = detectSuccess.ToArray();

            try
            {
                Log.Information("attndata.MatchTimes {0}", attndata.MatchTimes.Select(t => t));
                Log.Information("attndata.latency_times {0}", attndata.latency_times.Select(t => t));
                Log.Information("attndata.FalsePositives {0}", attndata.FalsePositives.Select(t => t));
                Log.Information("attndata.MatchTimes {0}", chMatchTimes.Select(t => t));
                Log.Information("attndata.ResponseTimes {0}", chResponseTimes.Select(t => t));
                Log.Information("attndata.FalsePositives {0}", chFalsePositives.Select(t => t));
                Log.Information("attndata.detectSuccess {0}", attndata.detectSuccess.Select(t => t));

                await Attention.DB.Insert(attndata);
                Log.Information("Insertion of Attention test results complete!");

                //End data collection
                eegReader.End(attndata.Index);

                string seqSuccess = string.Format("{0}", detectSuccess.Select(t => t));

                //Log.Information("Attention - On end finished. seqSuccess {0}", seqSuccess);

                ExperimentDescription.Text = "Thank you for participating in this experiment " + attndata.Index + ". \n " +
                    " Time interval for showing the characters is " + DisplayInterval + " milliseconds\n" +
                    "\n\n Your measurements are:  \n" +
                    "\t Number of detection: " + attndata.latency_times.Count() + "  out of " + attndata.MatchTimes.Count() + ".\n" +
                //"Sequence of detection: " + seqSuccess + "\n" +
                "\t Number of false positives: " + attndata.FalsePositives.Count() + "\n" +
                   "\t Average latency time: " + computeAvglatencyTime(attndata.latency_times).ToString("0.##") + ".\n";

                ExperimentDescription.Visible = true;
                ExperimentDescription.Refresh();
                ResumeDrawing(ExperimentDescription);
            }
            catch (Exception e)
            {
                Log.Error("Attention - OnEndExperiment . Error {0}", e.Message);
            }

            //List the threads
            //lisTtThreads();

        } // end OnEndTest()




        //======================================================================
        //----------------------------------------------------------------------
        // EVENT HANDLERS
        //----------------------------------------------------------------------
        //======================================================================

        //----------------------------------------------------------------------
        private void beginTestButton_Click(object sender, EventArgs e)
        {
            GC.Collect(); //Clean the session

            attndata.SubjectName = subjectNameTextBox.Text;
            attndata.Timestamp = DateTime.Now;
            matchTimes.Clear();
            latency_times.Clear();
            falsePositives.Clear();
            detectSuccess = new bool[numMartches];
            spotTime = DateTime.Now;

            //ensure the name of the participant is filled out
            if (subjectNameTextBox.Text.Length < 1)
            {
                ExperimentDescription.Text = "Please provide the name of the participant and click the start button again.\n\n" + ExperimentDescription.Text;
                ExperimentDescription.Visible = true;
                ExperimentDescription.Refresh();
            }

            try
            {

                eegReader.Begin(attndata.Index, false, subjectNameTextBox.Text);

                //wait until the application subscribed to the EEG headset 
                if (eegReader.getEEGInitialized() == false)
                {
                    ExperimentDescription.Text = "EEG headset is not ready. Ensure the setup is correct and try again.\n\n" + ExperimentDescription.Text;
                    ExperimentDescription.Visible = true;
                    ExperimentDescription.Refresh();
                }
                else
                {
                    ExperimentDescription.Visible = false;
                    ExperimentDescription.Refresh();

                    Log.Debug(" start time {0} - sequence size {1}", attndata.Timestamp, Sequence.Length);

                    if (!RunExperiment(OnCharUpdate, OnEndExperiment))
                        Log.Error("The attention experiment failed.");

                }
                GC.Collect();
            }
            catch (Exception ex)
            {
                Log.Error("Attention - Button - Exception {0}", ex.Message);

            }
        } // end beginButton_Click()

        //----------------------------------------------------------------------
        private void charLabel_MouseClick(object sender, MouseEventArgs e)
        {

            double triggerTime = 0;

            if (iCurrentSeqPos > 0 && Sequence[iCurrentSeqPos - 1] == spotSeq[0] && Sequence[iCurrentSeqPos] == spotSeq[1])
            {
                //Log.Information("Catch {0} at {1}", Sequence[iCurrentSeqPos], spotTime);
                triggerTime = (DateTime.Now - spotTime).TotalMilliseconds;
                latency_times.Add(triggerTime);
                chResponseTimes.Add(Sequence[iCurrentSeqPos]);
                detectSuccess[matchCounter - 1] = true; //mark as detected.
            }
            else
            {
                //Log.Information("False positive: char {0} at {1}", Sequence[iCurrentSeqPos], DateTime.Now);
                triggerTime = (DateTime.Now - attndata.Timestamp).TotalMilliseconds;
                falsePositives.Add(triggerTime);
                chFalsePositives.Add(Sequence[iCurrentSeqPos]);
            }

           

        } // end charDisplay_MouseClick()

        private void AttentionForm_Load(object sender, EventArgs e)
        {

            // Read the sequence generation parameters.
            numMartches = Convert.ToInt32(ConfigurationManager.AppSettings.Get("AttentionNumberofMatches"));
            spotSeq = ConfigurationManager.AppSettings.Get("AttentionToFindSequence").ToCharArray();
            sizeSequence = Convert.ToInt32(ConfigurationManager.AppSettings.Get("AttentionSizeSequence"));
            //Sets the interval for changing the Characters
            DisplayInterval = Helpers.LoadPositiveIntCfgValue("attentionCharDisplayIntervalMs", 250);

            lblExperimentTitle.Text = "Selective Attention Experiment";

            ExperimentDescription.Text = "Thank you for accepting to participate in this experiment. The goal of this experiment is to associate brain activity to attention control. \n\n" +
                "Before starting the experiment make sure that the EEG headset is set correctly.\n\n" +
                "In this experiment, you will see a sequence of " + sizeSequence + " characters, each for " + DisplayInterval + " milliseconds." +
                " Your goal is to identify all sequences of '" + spotSeq[0] + spotSeq[1] + "' characters. Success metrics are the " +
                "number of correct matches and average response time. Correct matches measures the number " +
                "of times you correctly identified the sequence '" + spotSeq[0] + spotSeq[1] + "'. Average response time measures the " +
                "average time you take to identify correctly the character sequence.\n\n" +
                "Click button 'Start the exericse' when ready!";

            ExperimentDescription.Refresh();
            ResumeDrawing(ExperimentDescription);

           //Generate the sequence.
            GenerateSequences();

            Log.Debug("Sequence {0}. \n spotSeq {1}{2}, Number repetition {3}", Sequence, spotSeq[0], spotSeq[1], verifySeqSpot(Sequence, spotSeq));



            ExperimentDescription.Visible = true;

        }


        private void lisTtThreads()
        {

            Log.Debug("\n\n+++++List of threads");
            int counter = 0;
            ProcessThreadCollection currentThreads = Process.GetCurrentProcess().Threads;

            foreach (ProcessThread thread in currentThreads)
            {
                counter++;
                Log.Debug("Thread name {0} id {1} Container{2}, TotalProcessorTime {3}, CurrentPriority {4}, start time {5}, state {6} ", thread.GetType(), thread.Id.ToString("X"), thread.Container, thread.TotalProcessorTime, thread.CurrentPriority, thread.StartTime, thread.ThreadState);
            }
            Log.Debug("\n Number of threads is {0}", counter);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {

            eegReader.End(attndata.Index);
            //Close the form
            this.Close();
            Thread.Yield();

        }
    } // end class AttentionForm
}
