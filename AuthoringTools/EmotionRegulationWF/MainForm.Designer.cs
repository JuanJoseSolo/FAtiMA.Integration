
namespace EmotionRegulationWF
{
    partial class MainForm
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
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.buttonAppVariables = new System.Windows.Forms.Button();
            this.buttonDuplicateAppraisalRule = new System.Windows.Forms.Button();
            this.buttonEditAppraisalRule = new System.Windows.Forms.Button();
            this.buttonRemoveAppraisalRule = new System.Windows.Forms.Button();
            this.dataGridER = new System.Windows.Forms.DataGridView();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridER)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.buttonAppVariables);
            this.groupBox7.Controls.Add(this.buttonDuplicateAppraisalRule);
            this.groupBox7.Controls.Add(this.buttonEditAppraisalRule);
            this.groupBox7.Controls.Add(this.buttonRemoveAppraisalRule);
            this.groupBox7.Controls.Add(this.dataGridER);
            this.groupBox7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox7.Location = new System.Drawing.Point(0, 0);
            this.groupBox7.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox7.Size = new System.Drawing.Size(800, 450);
            this.groupBox7.TabIndex = 2;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Appraisal Rules";
            // 
            // buttonAppVariables
            // 
            this.buttonAppVariables.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonAppVariables.Location = new System.Drawing.Point(404, 24);
            this.buttonAppVariables.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonAppVariables.Name = "buttonAppVariables";
            this.buttonAppVariables.Size = new System.Drawing.Size(170, 36);
            this.buttonAppVariables.TabIndex = 11;
            this.buttonAppVariables.Text = "Appraisal Variables";
            this.buttonAppVariables.UseVisualStyleBackColor = true;
            // 
            // buttonDuplicateAppraisalRule
            // 
            this.buttonDuplicateAppraisalRule.Location = new System.Drawing.Point(185, 24);
            this.buttonDuplicateAppraisalRule.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonDuplicateAppraisalRule.Name = "buttonDuplicateAppraisalRule";
            this.buttonDuplicateAppraisalRule.Size = new System.Drawing.Size(96, 36);
            this.buttonDuplicateAppraisalRule.TabIndex = 10;
            this.buttonDuplicateAppraisalRule.Text = "Duplicate";
            this.buttonDuplicateAppraisalRule.UseVisualStyleBackColor = true;
            // 
            // buttonEditAppraisalRule
            // 
            this.buttonEditAppraisalRule.Location = new System.Drawing.Point(98, 24);
            this.buttonEditAppraisalRule.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonEditAppraisalRule.Name = "buttonEditAppraisalRule";
            this.buttonEditAppraisalRule.Size = new System.Drawing.Size(81, 36);
            this.buttonEditAppraisalRule.TabIndex = 9;
            this.buttonEditAppraisalRule.Text = "Edit";
            this.buttonEditAppraisalRule.UseVisualStyleBackColor = true;
            this.buttonEditAppraisalRule.Click += new System.EventHandler(this.buttonEditAppraisalRule_Click);
            // 
            // buttonRemoveAppraisalRule
            // 
            this.buttonRemoveAppraisalRule.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRemoveAppraisalRule.Location = new System.Drawing.Point(288, 24);
            this.buttonRemoveAppraisalRule.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonRemoveAppraisalRule.Name = "buttonRemoveAppraisalRule";
            this.buttonRemoveAppraisalRule.Size = new System.Drawing.Size(110, 36);
            this.buttonRemoveAppraisalRule.TabIndex = 8;
            this.buttonRemoveAppraisalRule.Text = "Remove";
            this.buttonRemoveAppraisalRule.UseVisualStyleBackColor = true;
            // 
            // dataGridER
            // 
            this.dataGridER.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.dataGridER.AllowUserToAddRows = false;
            this.dataGridER.AllowUserToDeleteRows = false;
            this.dataGridER.AllowUserToOrderColumns = true;
            this.dataGridER.AllowUserToResizeRows = false;
            this.dataGridER.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridER.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridER.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dataGridER.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridER.ImeMode = System.Windows.Forms.ImeMode.On;
            this.dataGridER.Location = new System.Drawing.Point(8, 69);
            this.dataGridER.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.dataGridER.Name = "dataGridER";
            this.dataGridER.ReadOnly = true;
            this.dataGridER.RowHeadersVisible = false;
            this.dataGridER.RowHeadersWidth = 51;
            this.dataGridER.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridER.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridER.Size = new System.Drawing.Size(788, 119);
            this.dataGridER.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.groupBox7);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridER)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Button buttonAppVariables;
        private System.Windows.Forms.Button buttonDuplicateAppraisalRule;
        private System.Windows.Forms.Button buttonEditAppraisalRule;
        private System.Windows.Forms.Button buttonRemoveAppraisalRule;
        private System.Windows.Forms.DataGridView dataGridER;
    }
}

