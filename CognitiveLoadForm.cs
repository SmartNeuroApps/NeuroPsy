using System;
using System.Collections.Generic;
using System.Configuration;
//using System.Reflection.PortableExecutable;
using System.Threading;
using com.Brainsoft.MatrixVR.EEGAcquisition;
using Serilog;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace com.Brainsoft.NeurPsy.Experiments
{
    public partial class CognitiveLoadForm : CharSequenceForm
    {
        //======================================================================
        //----------------------------------------------------------------------
        // CONSTRUCTORS/DESTRUCTORS
        //----------------------------------------------------------------------
        //======================================================================
        public CognitiveLoadForm() : base()
        {
            InitializeComponent();
            //beginTestButton.Click += BeginTestButton_Click;
            charLabel.Click += OnInput;

            memory = new Memory();
            LoadConfiguration();
            matchDisplayTimes = new List<double>(matchCount);
            matchDisplaychar = new List<double>(matchCount);
            latencyTimes = new List<double>(matchCount);
            falsePosTimes = new List<double>(matchCount);
            detectSuccess = new bool[matchCount];
            randTrials = new Random(); // Random generator for the trials
            randChars = new Random(); // random generator for the characters
            randbool = new Random(); // Random gfenerator for upper and lower case characters
        } // end constructor()


        //======================================================================
        //----------------------------------------------------------------------
        // PRIVATE MEMBER VARIABLES
        //----------------------------------------------------------------------
        //======================================================================
        // Compile-time constants
        private static readonly int charRange = ('Z' - 'A'); // ([0-9][A-Z])

        // Compile-time configuration defaults
        private static readonly int defaultCharCount = 64;
        private static readonly int defaultCharIntervalMs = 2000;
        private static readonly int defaultMatchCount = 16;
        private static readonly int defaultNBack = 2;
        private static readonly int defaultResponseTimeLimitMs = 500;
        private static readonly int defaultnBlocks = 3;

        // Shared runtime resources
        private static Random randTrials;
        private static Random randChars;
        private static Random randbool;

        // Runtime configuration values
        private int charCount;
        private int matchCount;
        private int nBack;
        private int nBlocks; // The number of blockjs by experiment.

        // Instance members
        private Memory memory;
        private List<double> matchDisplayTimes; // Collect the time when the matching char showed up
        private List<double> matchDisplaychar; // Put the char that hold the n-back. That is used for verification
        private List<double> latencyTimes; // Time for response 
        private List<double> falsePosTimes; //Time when user clicked but there were no match
        private bool[] detectSuccess; //Takes values true or false for each match, where true is for identification of the match 

        private DateTime? spotTime;// Used to track last match time
        private int matchCounter = 0; // counter for the matches

        private EEGReader eegReader = new EEGReader();


        private void LoadConfiguration()
        {
            charCount = Helpers.LoadPositiveIntCfgValue("charCount", defaultCharCount);
            DisplayInterval = Helpers.LoadPositiveIntCfgValue("cognitiveLoadCharDisplayIntervalMs", defaultCharIntervalMs);
            matchCount = Helpers.LoadPositiveIntCfgValue("matchCount", defaultMatchCount); // TODO: Can this be zero?
            nBack = Helpers.LoadPositiveIntCfgValue("nBack", defaultNBack);
            nBlocks = Helpers.LoadPositiveIntCfgValue("nBlocksExperiment", defaultnBlocks);

            if (nBack <= 1)
            {
                nBack = defaultNBack;
                Log.Error($"nBack must be greater than 1. Defaulting to {nBack}.");
            }
            memory.InputTimeLimit = DisplayInterval / 1000;

            if (charCount > 26)
                Log.Warning($"The number of trials is {0}. Database cannot support more than 26 trials per sequence. Trial results after 26 will not be saved.", charCount);
        } // end LoadConfiguration()

        //======================================================================

        private char[] GenerateSequence(int charCount, int matchCount, int nBack)
        {

            var seq = new char[charCount];
            int rTrials;
            int rChar;

            //Step 1 -- Generate the matches
            int nlastMatch = charCount - defaultNBack; // Account for adding the match

            for (int i = 0; i < matchCount; i++)
            {
                rTrials = randTrials.Next(nlastMatch);
                if ((seq[rTrials] == '\0') && (seq[rTrials + nBack] == '\0'))
                {
                    rChar = randChars.Next(charRange);
                    rChar = randbool.Next(2) == 0 ? rChar : rChar + 32;
                    seq[rTrials] = (char)(randChars.Next(charRange) + 'A');
                    seq[rTrials + nBack] = seq[rTrials]; // Set the match
                }
                else
                    i--; // Do not count this trial as the position is filled out
            }

            //Part 1- Fill out the positions before the nback
            for (int i = 0; i < nBack; i++)
            {
                if (seq[i] == '\0')
                {
                    rChar = randChars.Next(charRange);
                    rChar = randbool.Next(2) == 0 ? rChar : rChar + 32;
                    seq[i] = (char)(randChars.Next(charRange) + 'A');

                    if (seq[i] == seq[i + nBack])
                    {
                        seq[i] = '\0';
                        i--;
                    }
                }
            }

            // Part 2 - Fill out the characters after teg nback and before the nlastmatch
            for (int i = nBack; i < nlastMatch; i++)
            {
                if (seq[i] == '\0')
                {
                    // Get the next random characters
                    // Repeat if the charcter was already see to avoid more matches than allows
                    rChar = randChars.Next(charRange);
                    rChar = randbool.Next(2) == 0 ? rChar : rChar + 32;
                    seq[i] = (char)(randChars.Next(charRange) + 'A');
                    //If the character will cause a match repeat the trial

                    if ((seq[i] == seq[i - nBack]) || (seq[i] == seq[i + nBack]))
                    {
                        seq[i] = '\0';
                        i--;
                    }
                }
            }

            // Part 3- Fill out the positions after the nlastMatch
            for (int i = nlastMatch; i < seq.Length; i++)
            {
                if (seq[i] == '\0')
                {
                    rChar = randChars.Next(charRange);
                    rChar = randbool.Next(2) == 0 ? rChar : rChar + 32;
                    seq[i] = (char)(randChars.Next(charRange) + 'A');

                    if (seq[i] == seq[i - nBack])
                    {
                        seq[i] = '\0';
                        i--;
                    }
                }
            }

            //Log.Debug("Sequence {0}", seq);

            return seq;
        }

        //This function returns the number of matches
        private int verifyMatches(char[] sequence, int nback)
        {
            int nMatches = 0;
            matchDisplaychar.Clear(); //Ensure that the array contains the char from the last verification

            for (int i = 0; i < (sequence.Length - nback); i++)
            {

                if (sequence[i] == sequence[i + nback])
                {
                    matchDisplaychar.Add(sequence[i]); // This is used to ensure the matches charectares
                    nMatches++;
                }
            }

            return nMatches;
        }

        //remove extra matches
        private char[] removeMatches(char[] sequence, int nback, int nextramatches)
        {
            int i = nback;
            int rChar;

            while ((nextramatches > 0) && (i < (sequence.Length - nback)))
            {
                if (sequence[i] == sequence[i + nback])
                {
                    rChar = randChars.Next(charRange);
                    rChar = randbool.Next(2) == 0 ? rChar : rChar + 32;
                    sequence[i] = (char)(randChars.Next(charRange) + 'A');

                    //Ensure that there is no new match
                    if ((sequence[i] != sequence[i + nback]) && (sequence[i] != sequence[i - nback]))
                        nextramatches--;
                }
                i++;

            }

            return sequence;
        }

        //Add  matches
        private char[] addMatches(char[] sequence, int nback, int nextraMatches)
        {
            int i = 0;

            while ((nextraMatches > 0) && (i < (sequence.Length - nback)))
            {
                if (sequence[i] != sequence[i + nback])
                {
                    sequence[i] = sequence[i + nback];
                    nextraMatches--;
                }
                i++;

            }

            return sequence;
        }
        private double computeAvgresponseTime(double[] ResponseTimes)
        {
            double avgresponseTime = 0;
            //

            for (int i = 0; i < ResponseTimes.Count(); i++)
            {
                avgresponseTime += ResponseTimes[i];
            }

            return avgresponseTime / ResponseTimes.Count();
        }

        private async void OnEndExperiment()
        {

            // Show the end of the experiments
            charLabel.Text = "End ... Thank you";
            charLabel.Refresh();

            Log.Debug("End time {0} - diff {1} - sequence size {2}", DateTime.Now, memory.Timestamp, Sequence.Length);
            try
            {
                memory.SubjectName = subjectNameTextBox.Text;
                memory.MatchDisplayTimes = matchDisplayTimes.ToArray();
                memory.LatencyTimes = latencyTimes.ToArray();
                memory.FalsePosTimes = falsePosTimes.ToArray();
                memory.DetectSuccess = detectSuccess.ToArray();

                Log.Information("\n\nmemory.MatchTimes {0}", memory.MatchDisplayTimes.Select(t => t));
                Log.Information("memory.ResponseTimes {0}\n", memory.LatencyTimes.Select(t => t));
                Log.Information("FalsePosTimes {0}\n", memory.FalsePosTimes.Select(t => t));
                Log.Information("DetectSuccess {0}\n", detectSuccess.Select(t => t));

                await Memory.DB.Insert(memory);
                //End data collection
                eegReader.End(memory.Index);

                Log.Information("Cognitive Load - On end finished.");

                ExperimentDescription.Text = "Thank you for participating in this experiment " + memory.Index + ". \n " +
     " Time interval for showing the characters is " + DisplayInterval + " milliseconds\n" +
     "\n\n Your measurements are:  \n" +
     "\t Number of matches: " + memory.LatencyTimes.Count() + "  out of " + memory.MatchDisplayTimes.Count() + ".\n" +
 "\t Number of false positives: " + memory.FalsePosTimes.Count() + "\n" +
    "\t Average response time: " + computeAvgresponseTime(memory.LatencyTimes).ToString("0.##") + ".\n";
                ExperimentDescription.Visible = true;
                ExperimentDescription.Refresh();
                ResumeDrawing(ExperimentDescription);
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        } // end OnEnd()

        //======================================================================
        private void OnInput(object sender, EventArgs e)
        {
            double x;

            if (spotTime == null)
                spotTime = DateTime.Now;

            //double triggerTime = (DateTime.Now - spotTime).Value.TotalMilliseconds;
            if (sequencePos >= nBack && Sequence[sequencePos] == Sequence[sequencePos - nBack])
            {
                // record the latency time
                x = (DateTime.Now - spotTime).Value.TotalMilliseconds;
                latencyTimes.Add(x);
                detectSuccess[matchCounter - 1] = true; //Mark as detected
            }

            else  //False positve

            {
                x = (DateTime.Now - memory.Timestamp).Value.TotalMilliseconds;
                //Log.Debug("Folse char{0} Pos Spot {1}-x {2}", Sequence[sequencePos], DateTime.Now, x);
                falsePosTimes.Add(x);
            }

        } // end OnInput()

        //======================================================================
        private void OnUpdate(int pos)
        {
            double x;
            if (pos >= nBack && Sequence[pos] == Sequence[pos - nBack])
            {
                spotTime = DateTime.Now; // record the time to compute the latency
                x = (spotTime - memory.Timestamp).Value.TotalMilliseconds;
                matchDisplayTimes.Add(x);

                detectSuccess[matchCounter] = false; //Mark initially as not detected.
                matchCounter++;

            }

            GC.Collect();

        } // end OnUpdate()


        //======================================================================
        //----------------------------------------------------------------------
        // EVENT HANDLERS
        //----------------------------------------------------------------------
        //======================================================================
        private void BeginTestButton_Click(object sender, EventArgs e)
        {

            GC.Collect(); //Clean the session

            memory.SubjectName = subjectNameTextBox.Text;
            memory.Timestamp = DateTime.Now;
            matchDisplayTimes.Clear();
            matchDisplaychar.Clear();
            latencyTimes.Clear();
            falsePosTimes.Clear();
            detectSuccess = new bool[matchCount];


            //ensure the name of the participant is filled out
            if (subjectNameTextBox.Text.Length < 1)
            {
                ExperimentDescription.Text = "Please provide the name of the participant and click the start button again.\n\n" + ExperimentDescription.Text;
                ExperimentDescription.Visible = true;
                ExperimentDescription.Refresh();
            }

            try
            {
                bool headsetinitialized;
                //Collect EEG Data
                eegReader.Begin(memory.Index, false, subjectNameTextBox.Text);

                //wait until the application subscribed to the EEG headset 
                if ((headsetinitialized = eegReader.getEEGInitialized()) == false)
                {
                    Log.Information("Start Test -- EEG headset initialized {0}", eegReader.getEEGInitialized());

                    ExperimentDescription.Text = "EEG headset is not ready. Ensure the setup is correct and try again.\n\n" + ExperimentDescription.Text;
                    ExperimentDescription.Visible = true;
                    ExperimentDescription.Refresh();
                }
                else
                {
                    ExperimentDescription.Visible = false;
                    ExperimentDescription.Refresh();

                    memory.Timestamp = DateTime.Now;

                    Log.Debug("Start time {0}  - sequence size {2}", memory.Timestamp, Sequence.Length);

                    if (!RunExperiment(OnUpdate, OnEndExperiment))
                        Log.Error("The cognitive load experiment failed.");
                }


            }
            catch (Exception ex)
            {
                Log.Information("CognitiveLoad - Button - Exception {0}", ex.Message);

            }

        } // end BeginTestButton_Click()

        private void CognitiveLoadForm_Load(object sender, EventArgs e)
        {

            LoadConfiguration(); // Load the parameters

            int n = 0;

            //Generate the sequence of Charaters
            Sequence = GenerateSequence(charCount, matchCount, nBack);
            n = verifyMatches(Sequence, nBack);

            // Do a maintenance of the list if teh number of matches is not correct

            if (n > matchCount)
            {
                Sequence = removeMatches(Sequence, nBack, n - matchCount);
                n = verifyMatches(Sequence, nBack);
            }

            if (n < matchCount)
            {

                Sequence = addMatches(Sequence, nBack, matchCount - n);
                n = verifyMatches(Sequence, nBack);
            }

            Log.Debug($"Sequence = {new string(Sequence)} nmatches = {n}");

            Log.Information("Char for the marches {0}\n", matchDisplaychar.Select(t => (char)t));

            if (n != matchCount)
                MessageBox.Show("Number of matches" + Convert.ToString(n));



            lblExperimentTitle.Text = "Cognitive Load Experiment";


            ExperimentDescription.Text = "Thank you for accepting to participate in this experiment. The goal of this experiment is to associate brain activity to working memory. \n\n" +
                "Before starting the experiment make sure that the EEG headset is set correctly.\n\n" +
                "In this experiment, you will see a sequence of " + Sequence.Length + " characters, each for " + DisplayInterval + " seconds." +
                " Your goal is to identify all the '" + nBack + "' sequences. Success metrics are the " +
                "number of correct matches and average response time. Correct matches measures the number " +
                "of times you correctly identified the n-back sequences. Average response time measures the " +
                "average time you take to identify correctly the sequences.\n\n" +
                "Recall for a 2-black experiment, you should see [a Character][any character][a Character]. " +
                "Click button 'Start the exericse' when ready!";

            ExperimentDescription.Refresh();
            ResumeDrawing(ExperimentDescription);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            eegReader.End(memory.Index);
            this.Close();
            Thread.Yield();
        }
    } // end class CognitiveLoadForm
}

