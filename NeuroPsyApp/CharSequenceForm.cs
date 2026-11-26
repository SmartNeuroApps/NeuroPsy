using Serilog;
using System;
using System.Drawing;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace com.Brainsoft.NeurPsy.Experiments
{
    public partial class CharSequenceForm : Form
    {
        //======================================================================
        //----------------------------------------------------------------------
        // PUBLIC TYPES
        //----------------------------------------------------------------------
        //======================================================================
        public delegate void CharUpdateCallback(int pos);
        public delegate void EndTestCallback();

        //======================================================================
        //----------------------------------------------------------------------
        // CONSTRUCTORS/DESTRUCTORS
        //----------------------------------------------------------------------
        //======================================================================
        public CharSequenceForm()
        {
            InitializeComponent();
            charLabel = new Label { AutoSize = true, Visible = false };
            charLabel.BackColor = Color.White;
            charLabel.ForeColor = Color.Blue;
            Controls.Add(charLabel);
            charLabelGraphics = charLabel.CreateGraphics();
            updateTimer = new Timer();
            updateTimer.Tick += OnTimerTick;

            sequencePos = -1; // This is will be incremented before dispalying.
        } // end constructor()

        ~CharSequenceForm()
        {
            charLabelGraphics.Dispose();
        } // end destructor()

        //======================================================================
        //----------------------------------------------------------------------
        // PUBLIC PROPERTIES
        //----------------------------------------------------------------------
        //======================================================================
        //the sequence of characters
        public char[] Sequence { get; set; }
        public int DisplayInterval
        {
            get { return updateTimer.Interval; }
            set { updateTimer.Interval = value; }
        }

        //======================================================================
        //----------------------------------------------------------------------
        // PUBLIC METHODS
        //----------------------------------------------------------------------
        //======================================================================
        public bool RunExperiment(
                CharUpdateCallback updateCallback,
                EndTestCallback endCallback)
        {
            if (Sequence == null || updateTimer.Interval == 0)
                return false;
            this.updateCallback = updateCallback;
            this.endCallback = endCallback;
            sequencePos = 0;
            ScaleCharToForm();

            updateTimer.Enabled = true;
            OnTimerTick(null, null);
            return true;
        } // end RunTest()

        //======================================================================
        //----------------------------------------------------------------------
        // PROTECTED MEMBER VARIABLES
        //----------------------------------------------------------------------
        //======================================================================
        protected static readonly uint WM_SETREDRAW = 11;

        protected int sequencePos;
        protected Label charLabel;
        protected Graphics charLabelGraphics;
        protected Timer updateTimer;

        protected CharUpdateCallback updateCallback;
        protected EndTestCallback endCallback;

        //======================================================================
        //----------------------------------------------------------------------
        // PROTECTED EXTERNAL METHOD DECLARATIONS
        //----------------------------------------------------------------------
        //======================================================================
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(
                IntPtr hWnd, // Window handle
                uint Msg,
                bool wParam,
                IntPtr lParam
        ); // end SendMessage()

        //======================================================================
        //----------------------------------------------------------------------
        // PROTECTED METHODS
        //----------------------------------------------------------------------
        //======================================================================

        //----------------------------------------------------------------------
        protected void DisplayCentered(string s)
        {
            try
            {
                // Every property change will request a redraw, but only a single redraw,
                // once all changes have been made, is desired.

                SuspendDrawing(charLabel);
                charLabel.Text = s;
                SizeF size = charLabelGraphics.MeasureString(charLabel.Text, charLabel.Font);
                int YPos = this.Height / 2 - charLabel.Height / 2; // Center the Char
                charLabel.Location = new Point(Width / 2 - (int)size.Width / 2, YPos);
                charLabel.Visible = true;
                ResumeDrawing(charLabel);
                // Redraw everything in the form.
                // Erases potential lingering artefacts left by previous characters.

                Refresh();
            }catch (Exception) { return; }

        } // end PrintCentered()

        //----------------------------------------------------------------------
        protected static void ResumeDrawing(Control ctrl)
        {
            try
            {
                // Solution source: https://stackoverflow.com/a/487757
                SendMessage(ctrl.Handle, WM_SETREDRAW, true, IntPtr.Zero);
            }
            catch (Exception)
            { return; }
        } // end ResumeDrawing()

        //----------------------------------------------------------------------
        protected void OnTimerTick(object sender, EventArgs e)
        {
           
            try
            {
                    if (Sequence != null && sequencePos < Sequence.Length-1)
                    {
                        if (updateCallback != null)
                        {
                            sequencePos++; // Gets the position in teh sequence
                            DisplayCentered(new string(Sequence[sequencePos], 1));
                            updateCallback(sequencePos);
                        }
                    }
                    else
                    {
                        updateTimer.Enabled = false;
                        charLabel.Visible = false;
                        updateCallback = null;
                        if (endCallback != null)
                            endCallback();
                        endCallback = null;
                    }
                
            }
            catch (Exception ex)
            { Log.Error("Timer Exception {0}", ex.Message); }
        } // end OnTimerTick()

        //----------------------------------------------------------------------
        // Changes the size of `charLabel`'s font size
        // (with the current font family) to fit the entire height of the
        // form without cutting any characters off.
        //----------------------------------------------------------------------
        protected void ScaleCharToForm()
        {
            if (Sequence == null || Sequence.Length == 0)
                return;
            SizeF size = charLabelGraphics.MeasureString(new string(Sequence), charLabel.Font);
            float scale = Height / (size.Height*2); // size the Char to fit half of the form high
            charLabel.Font = new Font(charLabel.Font.FontFamily, scale * charLabel.Font.SizeInPoints, GraphicsUnit.Point);
            charLabel.Size = size.ToSize();
        } // end ScaleCharHeight()

        //----------------------------------------------------------------------
        protected static void SuspendDrawing(Control ctrl)
        {
            try
            {
                // Solution source: https://stackoverflow.com/a/487757
                SendMessage(ctrl.Handle, WM_SETREDRAW, false, IntPtr.Zero);
            }
            catch (Exception)
            { return; }
        }
        // end SuspendDrawing()

        //======================================================================
        //----------------------------------------------------------------------
        // EVENT HANDLERS
        //----------------------------------------------------------------------
        //======================================================================

        //----------------------------------------------------------------------
        private void CharSequenceForm_Resize(object sender, EventArgs e)
        {
            ScaleCharToForm();
        }

        //----------------------------------------------------------------------
        private void SubjectNameTextBox_TextChanged(object sender, EventArgs e)
        {
            //beginTestButton.Enabled = subjectNameTextBox.Text.Length != 0;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            //Close the form
            this.Close();
        }
    } // end class CharSequenceForm
}
