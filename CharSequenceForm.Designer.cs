using Serilog;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace com.Brainsoft.NeurPsy.Experiments
{
    public partial class CharSequenceForm
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

        // #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        /// 
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CharSequenceForm));
            this.beginTestButton = new System.Windows.Forms.Button();
            this.subjectNameLabel = new System.Windows.Forms.Label();
            this.subjectNameTextBox = new System.Windows.Forms.TextBox();
            this.lblExperimentTitle = new System.Windows.Forms.Label();
            this.ExperimentDescription = new System.Windows.Forms.RichTextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // beginTestButton
            // 
            this.beginTestButton.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.beginTestButton.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.beginTestButton.Location = new System.Drawing.Point(911, 119);
            this.beginTestButton.Name = "beginTestButton";
            this.beginTestButton.Size = new System.Drawing.Size(277, 63);
            this.beginTestButton.TabIndex = 0;
            this.beginTestButton.Text = "Start the exercise";
            this.beginTestButton.UseVisualStyleBackColor = false;
            this.beginTestButton.Click += new System.EventHandler(this.beginTestButton_Click_1);
            // 
            // subjectNameLabel
            // 
            this.subjectNameLabel.AutoSize = true;
            this.subjectNameLabel.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.subjectNameLabel.Location = new System.Drawing.Point(41, 134);
            this.subjectNameLabel.Name = "subjectNameLabel";
            this.subjectNameLabel.Size = new System.Drawing.Size(157, 31);
            this.subjectNameLabel.TabIndex = 1;
            this.subjectNameLabel.Text = "Subject Name";
            // 
            // subjectNameTextBox
            // 
            this.subjectNameTextBox.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.subjectNameTextBox.Location = new System.Drawing.Point(224, 131);
            this.subjectNameTextBox.Name = "subjectNameTextBox";
            this.subjectNameTextBox.Size = new System.Drawing.Size(546, 38);
            this.subjectNameTextBox.TabIndex = 2;
            // 
            // lblExperimentTitle
            // 
            this.lblExperimentTitle.AutoSize = true;
            this.lblExperimentTitle.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblExperimentTitle.Location = new System.Drawing.Point(524, 19);
            this.lblExperimentTitle.Name = "lblExperimentTitle";
            this.lblExperimentTitle.Size = new System.Drawing.Size(156, 38);
            this.lblExperimentTitle.TabIndex = 3;
            this.lblExperimentTitle.Text = "Experiment";
            // 
            // ExperimentDescription
            // 
            this.ExperimentDescription.BackColor = System.Drawing.SystemColors.Window;
            this.ExperimentDescription.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExperimentDescription.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ExperimentDescription.Location = new System.Drawing.Point(14, 239);
            this.ExperimentDescription.Name = "ExperimentDescription";
            this.ExperimentDescription.Size = new System.Drawing.Size(1174, 393);
            this.ExperimentDescription.TabIndex = 4;
            this.ExperimentDescription.Text = "";
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(911, 19);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(277, 63);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // CharSequenceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1228, 706);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.ExperimentDescription);
            this.Controls.Add(this.lblExperimentTitle);
            this.Controls.Add(this.subjectNameTextBox);
            this.Controls.Add(this.subjectNameLabel);
            this.Controls.Add(this.beginTestButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "CharSequenceForm";
            this.Text = "Neuropsychology experiments ";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void beginTestButton_Click_1(object sender, EventArgs e)
        {

            Log.Information("Test button on chart sequence");

        }

        protected Button beginTestButton;
        protected Label subjectNameLabel;
        protected TextBox subjectNameTextBox;
        protected Label lblExperimentTitle;
        protected internal RichTextBox ExperimentDescription;
        protected Button btnClose;
    }

}