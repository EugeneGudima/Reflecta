namespace Reflecta2._0
{
    partial class Calibration
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Calibration));
            this.channel_combobox = new System.Windows.Forms.ComboBox();
            this.average_numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.CalibrationParameter = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DelayColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TemperatureColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.save_btn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.Open_btn = new System.Windows.Forms.Button();
            this.Unlock_btn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.average_numericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // channel_combobox
            // 
            this.channel_combobox.FormattingEnabled = true;
            this.channel_combobox.Location = new System.Drawing.Point(9, 34);
            this.channel_combobox.Name = "channel_combobox";
            this.channel_combobox.Size = new System.Drawing.Size(169, 21);
            this.channel_combobox.TabIndex = 0;
            // 
            // average_numericUpDown
            // 
            this.average_numericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F);
            this.average_numericUpDown.Location = new System.Drawing.Point(194, 32);
            this.average_numericUpDown.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.average_numericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.average_numericUpDown.Name = "average_numericUpDown";
            this.average_numericUpDown.Size = new System.Drawing.Size(86, 26);
            this.average_numericUpDown.TabIndex = 1;
            this.average_numericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.average_numericUpDown.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // dataGridView
            // 
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CalibrationParameter,
            this.Value,
            this.DelayColumn,
            this.TemperatureColumn});
            this.dataGridView.Location = new System.Drawing.Point(9, 66);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.Size = new System.Drawing.Size(431, 335);
            this.dataGridView.TabIndex = 2;
            this.dataGridView.RowValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.StatRowValidated);
            // 
            // CalibrationParameter
            // 
            this.CalibrationParameter.HeaderText = "Параметр калибровки";
            this.CalibrationParameter.Name = "CalibrationParameter";
            this.CalibrationParameter.Width = 110;
            // 
            // Value
            // 
            this.Value.HeaderText = "Усредн. зн-е уров.";
            this.Value.Name = "Value";
            this.Value.ReadOnly = true;
            // 
            // DelayColumn
            // 
            this.DelayColumn.HeaderText = "Усреднен. зн-е задержки";
            this.DelayColumn.Name = "DelayColumn";
            this.DelayColumn.Width = 110;
            // 
            // TemperatureColumn
            // 
            this.TemperatureColumn.HeaderText = "Т";
            this.TemperatureColumn.Name = "TemperatureColumn";
            this.TemperatureColumn.Width = 50;
            // 
            // save_btn
            // 
            this.save_btn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.save_btn.Enabled = false;
            this.save_btn.Font = new System.Drawing.Font("Arial Narrow", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.save_btn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.save_btn.Location = new System.Drawing.Point(223, 437);
            this.save_btn.Name = "save_btn";
            this.save_btn.Size = new System.Drawing.Size(217, 50);
            this.save_btn.TabIndex = 5;
            this.save_btn.Text = "Сохранить";
            this.save_btn.UseVisualStyleBackColor = true;
            this.save_btn.Click += new System.EventHandler(this.save_btn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Location = new System.Drawing.Point(5, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 23);
            this.label1.TabIndex = 6;
            this.label1.Text = "Канал";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial Narrow", 12.25F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Location = new System.Drawing.Point(190, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 22);
            this.label2.TabIndex = 7;
            this.label2.Text = "Усреднений";
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(9, 408);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(431, 23);
            this.progressBar.TabIndex = 8;
            // 
            // Open_btn
            // 
            this.Open_btn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Open_btn.Font = new System.Drawing.Font("Arial Narrow", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Open_btn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Open_btn.Location = new System.Drawing.Point(9, 437);
            this.Open_btn.Name = "Open_btn";
            this.Open_btn.Size = new System.Drawing.Size(208, 50);
            this.Open_btn.TabIndex = 9;
            this.Open_btn.Text = "Открыть";
            this.Open_btn.UseVisualStyleBackColor = true;
            this.Open_btn.Click += new System.EventHandler(this.Open_btn_Click);
            // 
            // Unlock_btn
            // 
            this.Unlock_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Unlock_btn.FlatAppearance.BorderSize = 0;
            this.Unlock_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Unlock_btn.Image = global::Reflecta2._0.Properties.Resources.icon_ban_circle;
            this.Unlock_btn.Location = new System.Drawing.Point(390, 10);
            this.Unlock_btn.Name = "Unlock_btn";
            this.Unlock_btn.Size = new System.Drawing.Size(50, 50);
            this.Unlock_btn.TabIndex = 10;
            this.Unlock_btn.UseVisualStyleBackColor = true;
            this.Unlock_btn.Click += new System.EventHandler(this.Unlock_btn_Click);
            // 
            // Calibration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 501);
            this.Controls.Add(this.Unlock_btn);
            this.Controls.Add(this.Open_btn);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.save_btn);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.average_numericUpDown);
            this.Controls.Add(this.channel_combobox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(470, 1050);
            this.MinimumSize = new System.Drawing.Size(470, 450);
            this.Name = "Calibration";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Калибровка";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.average_numericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox channel_combobox;
        private System.Windows.Forms.NumericUpDown average_numericUpDown;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Button save_btn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button Open_btn;
        private System.Windows.Forms.DataGridViewTextBoxColumn CalibrationParameter;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.DataGridViewTextBoxColumn DelayColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn TemperatureColumn;
        private System.Windows.Forms.Button Unlock_btn;
    }
}