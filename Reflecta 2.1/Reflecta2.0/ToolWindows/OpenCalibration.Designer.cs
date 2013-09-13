namespace Reflecta2._0
{
    partial class OpenCalibration
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OpenCalibration));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.CheckColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.IDColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DateColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NumOfAverage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SensorSerialColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FirmwareColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DeleteCalibrationGroup_btn = new System.Windows.Forms.Button();
            this.OpenCalibrationGroup_btn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CheckColumn,
            this.IDColumn,
            this.DateColumn,
            this.NumOfAverage,
            this.SensorSerialColumn,
            this.FirmwareColumn});
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.Size = new System.Drawing.Size(424, 449);
            this.dataGridView.TabIndex = 0;
            // 
            // CheckColumn
            // 
            this.CheckColumn.HeaderText = "";
            this.CheckColumn.Name = "CheckColumn";
            this.CheckColumn.Width = 25;
            // 
            // IDColumn
            // 
            this.IDColumn.HeaderText = "ID";
            this.IDColumn.Name = "IDColumn";
            this.IDColumn.Width = 60;
            // 
            // DateColumn
            // 
            this.DateColumn.HeaderText = "Дата";
            this.DateColumn.Name = "DateColumn";
            this.DateColumn.Width = 110;
            // 
            // NumOfAverage
            // 
            this.NumOfAverage.HeaderText = "Кол-во усредн.";
            this.NumOfAverage.Name = "NumOfAverage";
            this.NumOfAverage.Width = 60;
            // 
            // SensorSerialColumn
            // 
            this.SensorSerialColumn.HeaderText = "Серийный номер";
            this.SensorSerialColumn.Name = "SensorSerialColumn";
            this.SensorSerialColumn.Width = 70;
            // 
            // FirmwareColumn
            // 
            this.FirmwareColumn.HeaderText = "Прошивка";
            this.FirmwareColumn.Name = "FirmwareColumn";
            this.FirmwareColumn.Width = 70;
            // 
            // DeleteCalibrationGroup_btn
            // 
            this.DeleteCalibrationGroup_btn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DeleteCalibrationGroup_btn.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DeleteCalibrationGroup_btn.Location = new System.Drawing.Point(298, 457);
            this.DeleteCalibrationGroup_btn.Name = "DeleteCalibrationGroup_btn";
            this.DeleteCalibrationGroup_btn.Size = new System.Drawing.Size(114, 32);
            this.DeleteCalibrationGroup_btn.TabIndex = 4;
            this.DeleteCalibrationGroup_btn.Text = "Удалить";
            this.DeleteCalibrationGroup_btn.UseVisualStyleBackColor = true;
            this.DeleteCalibrationGroup_btn.Click += new System.EventHandler(this.DeleteCalibrationGroup_btn_Click);
            // 
            // OpenCalibrationGroup_btn
            // 
            this.OpenCalibrationGroup_btn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OpenCalibrationGroup_btn.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.OpenCalibrationGroup_btn.Location = new System.Drawing.Point(12, 457);
            this.OpenCalibrationGroup_btn.Name = "OpenCalibrationGroup_btn";
            this.OpenCalibrationGroup_btn.Size = new System.Drawing.Size(114, 32);
            this.OpenCalibrationGroup_btn.TabIndex = 3;
            this.OpenCalibrationGroup_btn.Text = "Проверить";
            this.OpenCalibrationGroup_btn.UseVisualStyleBackColor = true;
            this.OpenCalibrationGroup_btn.Click += new System.EventHandler(this.OpenCalibrationGroup_btn_Click);
            // 
            // OpenCalibration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 501);
            this.Controls.Add(this.DeleteCalibrationGroup_btn);
            this.Controls.Add(this.OpenCalibrationGroup_btn);
            this.Controls.Add(this.dataGridView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(440, 1080);
            this.MinimumSize = new System.Drawing.Size(440, 540);
            this.Name = "OpenCalibration";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Калибровочные ф-и";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.DataGridViewCheckBoxColumn CheckColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn IDColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn DateColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn NumOfAverage;
        private System.Windows.Forms.DataGridViewTextBoxColumn SensorSerialColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn FirmwareColumn;
        private System.Windows.Forms.Button DeleteCalibrationGroup_btn;
        private System.Windows.Forms.Button OpenCalibrationGroup_btn;
    }
}