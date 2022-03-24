
namespace EmotionRegulationWF
{
    partial class Form1
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
            this.addOrEditPersonalityButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // addOrEditPersonalityButton
            // 
            this.addOrEditPersonalityButton.FlatAppearance.BorderColor = System.Drawing.SystemColors.Desktop;
            this.addOrEditPersonalityButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Blue;
            this.addOrEditPersonalityButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.addOrEditPersonalityButton.Location = new System.Drawing.Point(122, 368);
            this.addOrEditPersonalityButton.Margin = new System.Windows.Forms.Padding(4);
            this.addOrEditPersonalityButton.Name = "addOrEditPersonalityButton";
            this.addOrEditPersonalityButton.Size = new System.Drawing.Size(100, 28);
            this.addOrEditPersonalityButton.TabIndex = 10;
            this.addOrEditPersonalityButton.Text = "Add";
            this.addOrEditPersonalityButton.UseVisualStyleBackColor = true;
            this.addOrEditPersonalityButton.Click += new System.EventHandler(this.addOrEditPersonalityButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(346, 450);
            this.Controls.Add(this.addOrEditPersonalityButton);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button addOrEditPersonalityButton;
    }
}