
using System;
using System.Windows.Forms;

namespace com.Brainsoft.NeurPsy.Experiments
{
    partial class AttentionForm : CharSequenceForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AttentionForm));
            this.SuspendLayout();
            // 
            // updateTimer
            // 
            this.updateTimer.Enabled = true;
            // 
            // beginTestButton
            // 
            this.beginTestButton.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.beginTestButton.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.beginTestButton.Location = new System.Drawing.Point(877, 97);
            this.beginTestButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.beginTestButton.Size = new System.Drawing.Size(230, 48);
            this.beginTestButton.Click += new System.EventHandler(this.beginTestButton_Click);
            // 
            // subjectNameLabel
            // 
            this.subjectNameLabel.Font = new System.Drawing.Font("Segoe UI", 13.8F);
            this.subjectNameLabel.Location = new System.Drawing.Point(29, 106);
            // 
            // subjectNameTextBox
            // 
            this.subjectNameTextBox.Font = new System.Drawing.Font("Segoe UI", 13.8F);
            this.subjectNameTextBox.Location = new System.Drawing.Point(217, 103);
            this.subjectNameTextBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.subjectNameTextBox.Size = new System.Drawing.Size(586, 38);
            // 
            // ExperimentDescription
            // 
            this.ExperimentDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ExperimentDescription.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ExperimentDescription.ForeColor = System.Drawing.SystemColors.Desktop;
            this.ExperimentDescription.Size = new System.Drawing.Size(1393, 393);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnClose.Location = new System.Drawing.Point(1150, 97);
            this.btnClose.Size = new System.Drawing.Size(248, 49);
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // AttentionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1419, 741);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "AttentionForm";
            this.Text = "Attention Experiment";
            this.Load += new System.EventHandler(this.AttentionForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void subjectNameTextBox_TextChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        #endregion
    }
}