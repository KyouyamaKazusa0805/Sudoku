namespace Sudoku.Forms.Playground
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
			this._toolStripMenuItem_aboutAuthor = new System.Windows.Forms.ToolStripMenuItem();
			this._grid = new System.Windows.Forms.PictureBox();
			this._toolStripMenuItem_fileQuit = new System.Windows.Forms.ToolStripMenuItem();
			this._menuStripMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._grid)).BeginInit();
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
			this._menuStripMain.Padding = new System.Windows.Forms.Padding(9, 3, 0, 3);
			this._menuStripMain.Size = new System.Drawing.Size(1006, 30);
			this._menuStripMain.TabIndex = 0;
			this._menuStripMain.Text = "[_menuStripMain]";
			// 
			// _toolStripMenuItem_file
			// 
			this._toolStripMenuItem_file.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._toolStripMenuItem_fileQuit});
			this._toolStripMenuItem_file.Name = "_toolStripMenuItem_file";
			this._toolStripMenuItem_file.Size = new System.Drawing.Size(70, 24);
			this._toolStripMenuItem_file.Text = "File (&F)";
			// 
			// _toolStripMenuItem_about
			// 
			this._toolStripMenuItem_about.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._toolStripMenuItem_aboutAuthor});
			this._toolStripMenuItem_about.Name = "_toolStripMenuItem_about";
			this._toolStripMenuItem_about.Size = new System.Drawing.Size(94, 24);
			this._toolStripMenuItem_about.Text = "About (&A)";
			// 
			// _toolStripMenuItem_aboutAuthor
			// 
			this._toolStripMenuItem_aboutAuthor.Name = "_toolStripMenuItem_aboutAuthor";
			this._toolStripMenuItem_aboutAuthor.Size = new System.Drawing.Size(224, 26);
			this._toolStripMenuItem_aboutAuthor.Text = "Author";
			this._toolStripMenuItem_aboutAuthor.Click += new System.EventHandler(this.ToolStripMenuItem_aboutAuthor_Click);
			// 
			// _grid
			// 
			this._grid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._grid.Location = new System.Drawing.Point(12, 33);
			this._grid.Name = "_grid";
			this._grid.Size = new System.Drawing.Size(540, 540);
			this._grid.TabIndex = 1;
			this._grid.TabStop = false;
			// 
			// _toolStripMenuItem_fileQuit
			// 
			this._toolStripMenuItem_fileQuit.Name = "_toolStripMenuItem_fileQuit";
			this._toolStripMenuItem_fileQuit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
			this._toolStripMenuItem_fileQuit.Size = new System.Drawing.Size(224, 26);
			this._toolStripMenuItem_fileQuit.Text = "Quit (&Q)";
			this._toolStripMenuItem_fileQuit.Click += new System.EventHandler(this.ToolStripMenuItem_fileQuit_Click);
			// 
			// MainForm
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1006, 721);
			this.Controls.Add(this._grid);
			this.Controls.Add(this._menuStripMain);
			this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MainMenuStrip = this._menuStripMain;
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Name = "MainForm";
			this.Text = "[MainForm]";
			this._menuStripMain.ResumeLayout(false);
			this._menuStripMain.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._grid)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip _menuStripMain;
		private System.Windows.Forms.ToolStripMenuItem _toolStripMenuItem_file;
		private System.Windows.Forms.ToolStripMenuItem _toolStripMenuItem_about;
		private System.Windows.Forms.ToolStripMenuItem _toolStripMenuItem_aboutAuthor;
		private System.Windows.Forms.PictureBox _grid;
		private System.Windows.Forms.ToolStripMenuItem _toolStripMenuItem_fileQuit;
	}
}

