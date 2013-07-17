namespace Reflecta2._0
{
    partial class TestCalibration
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestCalibration));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.RealValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.average_numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.channel_combobox = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.average_numericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.RealValueColumn});
            this.dataGridView.Location = new System.Drawing.Point(0, 57);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.Size = new System.Drawing.Size(551, 391);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.RowValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.CalibRowValidated);
            // 
            // RealValueColumn
            // 
            this.RealValueColumn.HeaderText = "Реальное зн-е";
            this.RealValueColumn.Name = "RealValueColumn";
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(305, 18);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(232, 23);
            this.progressBar.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial Narrow", 12.25F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Location = new System.Drawing.Point(193, 2);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 22);
            this.label2.TabIndex = 12;
            this.label2.Text = "Усреднений";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Location = new System.Drawing.Point(8, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 23);
            this.label1.TabIndex = 11;
            this.label1.Text = "Канал";
            // 
            // average_numericUpDown
            // 
            this.average_numericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.average_numericUpDown.Location = new System.Drawing.Point(197, 25);
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
            this.average_numericUpDown.Size = new System.Drawing.Size(86, 23);
            this.average_numericUpDown.TabIndex = 10;
            this.average_numericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.average_numericUpDown.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // channel_combobox
            // 
            this.channel_combobox.FormattingEnabled = true;
            this.channel_combobox.Location = new System.Drawing.Point(12, 27);
            this.channel_combobox.Name = "channel_combobox";
            this.channel_combobox.Size = new System.Drawing.Size(169, 21);
            this.channel_combobox.TabIndex = 9;
            // 
            // TestCalibration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(549, 447);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.average_numericUpDown);
            this.Controls.Add(this.channel_combobox);
            this.Controls.Add(this.dataGridView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TestCalibration";
            this.Text = "Тестирование калибровочных ф-й";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.average_numericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown average_numericUpDown;
        private System.Windows.Forms.ComboBox channel_combobox;
        private System.Windows.Forms.DataGridViewTextBoxColumn RealValueColumn;
    }
}