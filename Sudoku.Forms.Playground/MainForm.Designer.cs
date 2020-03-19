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
			this._panelMenu = new System.Windows.Forms.Panel();
			this._panelSelection = new System.Windows.Forms.Panel();
			this._panelSolutionName = new System.Windows.Forms.Panel();
			this._labelSolutionName = new System.Windows.Forms.Label();
			this._buttonAbout = new System.Windows.Forms.Button();
			this._buttonFile = new System.Windows.Forms.Button();
			this._buttonMainGrid = new System.Windows.Forms.Button();
			this._panelSubpage = new System.Windows.Forms.Panel();
			this.panel1 = new System.Windows.Forms.Panel();
			this._panelMenu.SuspendLayout();
			this._panelSolutionName.SuspendLayout();
			this.SuspendLayout();
			// 
			// _panelMenu
			// 
			this._panelMenu.BackColor = System.Drawing.Color.Silver;
			this._panelMenu.Controls.Add(this._panelSelection);
			this._panelMenu.Controls.Add(this._panelSolutionName);
			this._panelMenu.Controls.Add(this._buttonAbout);
			this._panelMenu.Controls.Add(this._buttonFile);
			this._panelMenu.Controls.Add(this._buttonMainGrid);
			this._panelMenu.Dock = System.Windows.Forms.DockStyle.Left;
			this._panelMenu.Location = new System.Drawing.Point(0, 0);
			this._panelMenu.Name = "_panelMenu";
			this._panelMenu.Size = new System.Drawing.Size(200, 721);
			this._panelMenu.TabIndex = 0;
			// 
			// _panelSelection
			// 
			this._panelSelection.BackColor = System.Drawing.Color.Gray;
			this._panelSelection.Location = new System.Drawing.Point(0, 138);
			this._panelSelection.Margin = new System.Windows.Forms.Padding(0);
			this._panelSelection.Name = "_panelSelection";
			this._panelSelection.Size = new System.Drawing.Size(14, 76);
			this._panelSelection.TabIndex = 2;
			// 
			// _panelSolutionName
			// 
			this._panelSolutionName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._panelSolutionName.Controls.Add(this._labelSolutionName);
			this._panelSolutionName.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this._panelSolutionName.Location = new System.Drawing.Point(0, 0);
			this._panelSolutionName.Margin = new System.Windows.Forms.Padding(0);
			this._panelSolutionName.Name = "_panelSolutionName";
			this._panelSolutionName.Size = new System.Drawing.Size(200, 138);
			this._panelSolutionName.TabIndex = 1;
			// 
			// _labelSolutionName
			// 
			this._labelSolutionName.Dock = System.Windows.Forms.DockStyle.Fill;
			this._labelSolutionName.Location = new System.Drawing.Point(0, 0);
			this._labelSolutionName.Margin = new System.Windows.Forms.Padding(0);
			this._labelSolutionName.Name = "_labelSolutionName";
			this._labelSolutionName.Size = new System.Drawing.Size(200, 138);
			this._labelSolutionName.TabIndex = 0;
			this._labelSolutionName.Text = "Sunnie\'s sudoku Solution";
			this._labelSolutionName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// _buttonAbout
			// 
			this._buttonAbout.FlatAppearance.BorderSize = 0;
			this._buttonAbout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonAbout.Location = new System.Drawing.Point(0, 290);
			this._buttonAbout.Margin = new System.Windows.Forms.Padding(0);
			this._buttonAbout.Name = "_buttonAbout";
			this._buttonAbout.Size = new System.Drawing.Size(200, 76);
			this._buttonAbout.TabIndex = 0;
			this._buttonAbout.Text = "About";
			this._buttonAbout.UseVisualStyleBackColor = true;
			this._buttonAbout.Click += new System.EventHandler(this.ButtonAbout_Click);
			// 
			// _buttonFile
			// 
			this._buttonFile.FlatAppearance.BorderSize = 0;
			this._buttonFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonFile.Location = new System.Drawing.Point(0, 214);
			this._buttonFile.Margin = new System.Windows.Forms.Padding(0);
			this._buttonFile.Name = "_buttonFile";
			this._buttonFile.Size = new System.Drawing.Size(200, 76);
			this._buttonFile.TabIndex = 0;
			this._buttonFile.Text = "File";
			this._buttonFile.UseVisualStyleBackColor = true;
			this._buttonFile.Click += new System.EventHandler(this.ButtonFile_Click);
			// 
			// _buttonMainGrid
			// 
			this._buttonMainGrid.FlatAppearance.BorderSize = 0;
			this._buttonMainGrid.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonMainGrid.Location = new System.Drawing.Point(0, 138);
			this._buttonMainGrid.Margin = new System.Windows.Forms.Padding(0);
			this._buttonMainGrid.Name = "_buttonMainGrid";
			this._buttonMainGrid.Size = new System.Drawing.Size(200, 76);
			this._buttonMainGrid.TabIndex = 0;
			this._buttonMainGrid.Text = "Main Grid";
			this._buttonMainGrid.UseVisualStyleBackColor = true;
			this._buttonMainGrid.Click += new System.EventHandler(this.ButtonMainGrid_Click);
			// 
			// _panelSubpage
			// 
			this._panelSubpage.Dock = System.Windows.Forms.DockStyle.Bottom;
			this._panelSubpage.Location = new System.Drawing.Point(200, 85);
			this._panelSubpage.Name = "_panelSubpage";
			this._panelSubpage.Size = new System.Drawing.Size(1088, 636);
			this._panelSubpage.TabIndex = 1;
			// 
			// panel1
			// 
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(200, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1088, 85);
			this.panel1.TabIndex = 2;
			// 
			// MainForm
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1288, 721);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this._panelSubpage);
			this.Controls.Add(this._panelMenu);
			this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Name = "MainForm";
			this.Text = "[MainForm]";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this._panelMenu.ResumeLayout(false);
			this._panelSolutionName.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel _panelMenu;
		private System.Windows.Forms.Panel _panelSubpage;
		private System.Windows.Forms.Button _buttonMainGrid;
		private System.Windows.Forms.Panel _panelSolutionName;
		private System.Windows.Forms.Label _labelSolutionName;
		private System.Windows.Forms.Panel _panelSelection;
		private System.Windows.Forms.Button _buttonAbout;
		private System.Windows.Forms.Button _buttonFile;
		private System.Windows.Forms.Panel panel1;
	}
}

