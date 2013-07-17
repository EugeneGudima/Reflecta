namespace Reflecta2._0
{
    partial class ReflectogramViewer
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
            this.components = new System.ComponentModel.Container();
            this.refl_graph = new ZedGraph.ZedGraphControl();
            this.SuspendLayout();
            // 
            // refl_graph
            // 
            this.refl_graph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.refl_graph.Location = new System.Drawing.Point(0, 0);
            this.refl_graph.Margin = new System.Windows.Forms.Padding(5, 8, 5, 8);
            this.refl_graph.Name = "refl_graph";
            this.refl_graph.ScrollGrace = 0D;
            this.refl_graph.ScrollMaxX = 0D;
            this.refl_graph.ScrollMaxY = 0D;
            this.refl_graph.ScrollMaxY2 = 0D;
            this.refl_graph.ScrollMinX = 0D;
            this.refl_graph.ScrollMinY = 0D;
            this.refl_graph.ScrollMinY2 = 0D;
            this.refl_graph.Size = new System.Drawing.Size(752, 491);
            this.refl_graph.TabIndex = 11;
            // 
            // ReflectogramViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(753, 491);
            this.Controls.Add(this.refl_graph);
            this.Name = "ReflectogramViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ReflectogramViewer";
            this.ResumeLayout(false);

        }

        private ZedGraph.ZedGraphControl refl_graph;

        #endregion
    }
}