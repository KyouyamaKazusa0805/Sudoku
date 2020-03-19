namespace Sudoku.Forms
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
			this._menuStripMain = new System.Windows.Forms.MenuStrip();
			this._toolStripMenuItem_file = new System.Windows.Forms.ToolStripMenuItem();
			this._toolStripMenuItem_about = new System.Windows.Forms.ToolStripMenuItem();
			this._pictureBoxGrid = new System.Windows.Forms.PictureBox();
			this._menuStripMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._pictureBoxGrid)).BeginInit();
			this.SuspendLayout();
			// 
			// _menuStripMain
			// 
			this._menuStripMain.ImageScalingSize = new System.Drawing.Size(20, 20);
			this._menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._toolStripMenuItem_file,
            this._toolStripMenuItem_about});
			this._menuStripMain.Location = new System.Drawing.Point(0, 0);
			this._menuStripMain.Name = "_menuStripMain";
			this._menuStripMain.Padding = new System.Windows.Forms.Padding(7, 3, 0, 3);
			this._menuStripMain.Size = new System.Drawing.Size(1228, 30);
			this._menuStripMain.TabIndex = 0;
			this._menuStripMain.Text = "menuStrip1";
			// 
			// _toolStripMenuItem_file
			// 
			this._toolStripMenuItem_file.Name = "_toolStripMenuItem_file";
			this._toolStripMenuItem_file.Size = new System.Drawing.Size(70, 24);
			this._toolStripMenuItem_file.Text = "File (&F)";
			// 
			// _toolStripMenuItem_about
			// 
			this._toolStripMenuItem_about.Name = "_toolStripMenuItem_about";
			this._toolStripMenuItem_about.Size = new System.Drawing.Size(94, 24);
			this._toolStripMenuItem_about.Text = "About (&A)";
			// 
			// _pictureBoxGrid
			// 
			this._pictureBoxGrid.Location = new System.Drawing.Point(12, 33);
			this._pictureBoxGrid.Name = "_pictureBoxGrid";
			this._pictureBoxGrid.Size = new System.Drawing.Size(540, 540);
			this._pictureBoxGrid.TabIndex = 1;
			this._pictureBoxGrid.TabStop = false;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1228, 991);
			this.Controls.Add(this._pictureBoxGrid);
			this.Controls.Add(this._menuStripMain);
			this.MainMenuStrip = this._menuStripMain;
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Name = "MainForm";
			this.Text = "[MainForm]";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this._menuStripMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this._pictureBoxGrid)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip _menuStripMain;
		private System.Windows.Forms.ToolStripMenuItem _toolStripMenuItem_file;
		private System.Windows.Forms.ToolStripMenuItem _toolStripMenuItem_about;
		private System.Windows.Forms.PictureBox _pictureBoxGrid;
	}
}

