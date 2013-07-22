namespace Reflecta2._0
{
    partial class OpenExperimentsGroupsDialog
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
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.OpenExperimentGroup_btn = new System.Windows.Forms.Button();
            this.DeleteExperimentGroup_btn = new System.Windows.Forms.Button();
            this.Select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.GroupID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DateStart_column = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ChannelNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DataNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PowerDelay = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CommDelay = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToOrderColumns = true;
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Select,
            this.GroupID,
            this.DateStart_column,
            this.Time,
            this.ChannelNum,
            this.DataNum,
            this.PowerDelay,
            this.CommDelay,
            this.Description});
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.Size = new System.Drawing.Size(834, 452);
            this.dataGridView.TabIndex = 0;
            // 
            // OpenExperimentGroup_btn
            // 
            this.OpenExperimentGroup_btn.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.OpenExperimentGroup_btn.Location = new System.Drawing.Point(553, 460);
            this.OpenExperimentGroup_btn.Name = "OpenExperimentGroup_btn";
            this.OpenExperimentGroup_btn.Size = new System.Drawing.Size(114, 32);
            this.OpenExperimentGroup_btn.TabIndex = 1;
            this.OpenExperimentGroup_btn.Text = "Открыть";
            this.OpenExperimentGroup_btn.UseVisualStyleBackColor = true;
            this.OpenExperimentGroup_btn.Click += new System.EventHandler(this.OpenExperimentGroup_btn_Click);
            // 
            // DeleteExperimentGroup_btn
            // 
            this.DeleteExperimentGroup_btn.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DeleteExperimentGroup_btn.Location = new System.Drawing.Point(150, 461);
            this.DeleteExperimentGroup_btn.Name = "DeleteExperimentGroup_btn";
            this.DeleteExperimentGroup_btn.Size = new System.Drawing.Size(114, 32);
            this.DeleteExperimentGroup_btn.TabIndex = 2;
            this.DeleteExperimentGroup_btn.Text = "Удалить";
            this.DeleteExperimentGroup_btn.UseVisualStyleBackColor = true;
            this.DeleteExperimentGroup_btn.Click += new System.EventHandler(this.DeleteExperimentGroup_btn_Click);
            // 
            // Select
            // 
            this.Select.HeaderText = "";
            this.Select.Name = "Select";
            this.Select.Width = 30;
            // 
            // GroupID
            // 
            this.GroupID.HeaderText = "#";
            this.GroupID.Name = "GroupID";
            this.GroupID.Width = 40;
            // 
            // DateStart_column
            // 
            this.DateStart_column.HeaderText = "Старт";
            this.DateStart_column.MinimumWidth = 125;
            this.DateStart_column.Name = "DateStart_column";
            this.DateStart_column.ReadOnly = true;
            this.DateStart_column.Width = 125;
            // 
            // Time
            // 
            this.Time.HeaderText = "Длительн., мин.";
            this.Time.Name = "Time";
            this.Time.Width = 85;
            // 
            // ChannelNum
            // 
            this.ChannelNum.HeaderText = "Кол-во каналов";
            this.ChannelNum.Name = "ChannelNum";
            this.ChannelNum.ReadOnly = true;
            this.ChannelNum.Width = 60;
            // 
            // DataNum
            // 
            this.DataNum.HeaderText = "Кол-во точек";
            this.DataNum.Name = "DataNum";
            this.DataNum.ReadOnly = true;
            // 
            // PowerDelay
            // 
            this.PowerDelay.HeaderText = "Зад. питания";
            this.PowerDelay.MinimumWidth = 55;
            this.PowerDelay.Name = "PowerDelay";
            this.PowerDelay.ReadOnly = true;
            this.PowerDelay.Width = 55;
            // 
            // CommDelay
            // 
            this.CommDelay.HeaderText = "Зад. перекл.";
            this.CommDelay.MinimumWidth = 55;
            this.CommDelay.Name = "CommDelay";
            this.CommDelay.ReadOnly = true;
            this.CommDelay.Width = 55;
            // 
            // Description
            // 
            this.Description.HeaderText = "Описание";
            this.Description.MinimumWidth = 220;
            this.Description.Name = "Description";
            this.Description.ReadOnly = true;
            this.Description.Width = 220;
            // 
            // OpenExperimentsGroupsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(834, 501);
            this.Controls.Add(this.DeleteExperimentGroup_btn);
            this.Controls.Add(this.OpenExperimentGroup_btn);
            this.Controls.Add(this.dataGridView);
            this.MaximumSize = new System.Drawing.Size(850, 600);
            this.MinimumSize = new System.Drawing.Size(850, 540);
            this.Name = "OpenExperimentsGroupsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OpenDialog";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Button OpenExperimentGroup_btn;
        private System.Windows.Forms.Button DeleteExperimentGroup_btn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Select;
        private System.Windows.Forms.DataGridViewTextBoxColumn GroupID;
        private System.Windows.Forms.DataGridViewTextBoxColumn DateStart_column;
        private System.Windows.Forms.DataGridViewTextBoxColumn Time;
        private System.Windows.Forms.DataGridViewTextBoxColumn ChannelNum;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataNum;
        private System.Windows.Forms.DataGridViewTextBoxColumn PowerDelay;
        private System.Windows.Forms.DataGridViewTextBoxColumn CommDelay;
        private System.Windows.Forms.DataGridViewTextBoxColumn Description;
    }
}