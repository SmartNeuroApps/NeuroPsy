namespace com.Brainsoft.NeurPsy.Experiments
{
    partial class CognitiveLoadForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // beginTestButton
            // 
            this.beginTestButton.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.beginTestButton.Location = new System.Drawing.Point(1043, 116);
            this.beginTestButton.Size = new System.Drawing.Size(242, 57);
            this.beginTestButton.Click += new System.EventHandler(this.BeginTestButton_Click);
            // 
            // subjectNameLabel
            // 
            this.subjectNameLabel.Location = new System.Drawing.Point(39, 127);
            // 
            // subjectNameTextBox
            // 
            this.subjectNameTextBox.Location = new System.Drawing.Point(224, 124);
            this.subjectNameTextBox.Size = new System.Drawing.Size(793, 38);
            // 
            // ExperimentDescription
            // 
            this.ExperimentDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ExperimentDescription.Size = new System.Drawing.Size(1515, 425);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnClose.Location = new System.Drawing.Point(1301, 116);
            this.btnClose.Size = new System.Drawing.Size(228, 52);
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // CognitiveLoadForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.ClientSize = new System.Drawing.Size(1551, 707);
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "CognitiveLoadForm";
            this.Text = "Cognitive load experiment";
            this.Load += new System.EventHandler(this.CognitiveLoadForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}