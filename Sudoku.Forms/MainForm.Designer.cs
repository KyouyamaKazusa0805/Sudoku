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
			this._pictureBoxGrid = new System.Windows.Forms.PictureBox();
			this._panelMenu = new System.Windows.Forms.Panel();
			this._buttonExit = new System.Windows.Forms.Button();
			this._buttonAbout = new System.Windows.Forms.Button();
			this._panelSelection = new System.Windows.Forms.Panel();
			this._buttonMainGrid = new System.Windows.Forms.Button();
			this._panelTitle = new System.Windows.Forms.Panel();
			this._panelSolutionName = new System.Windows.Forms.Panel();
			this._labelSolutionName = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this._pictureBoxGrid)).BeginInit();
			this._panelMenu.SuspendLayout();
			this._panelSolutionName.SuspendLayout();
			this.SuspendLayout();
			// 
			// _pictureBoxGrid
			// 
			this._pictureBoxGrid.Location = new System.Drawing.Point(457, 174);
			this._pictureBoxGrid.Margin = new System.Windows.Forms.Padding(0);
			this._pictureBoxGrid.Name = "_pictureBoxGrid";
			this._pictureBoxGrid.Size = new System.Drawing.Size(540, 540);
			this._pictureBoxGrid.TabIndex = 1;
			this._pictureBoxGrid.TabStop = false;
			// 
			// _panelMenu
			// 
			this._panelMenu.Controls.Add(this._panelSelection);
			this._panelMenu.Controls.Add(this._buttonMainGrid);
			this._panelMenu.Controls.Add(this._panelSolutionName);
			this._panelMenu.Controls.Add(this._buttonAbout);
			this._panelMenu.Controls.Add(this._buttonExit);
			this._panelMenu.Dock = System.Windows.Forms.DockStyle.Left;
			this._panelMenu.Location = new System.Drawing.Point(0, 0);
			this._panelMenu.Name = "_panelMenu";
			this._panelMenu.Size = new System.Drawing.Size(235, 775);
			this._panelMenu.TabIndex = 2;
			// 
			// _buttonExit
			// 
			this._buttonExit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonExit.BackColor = System.Drawing.Color.Transparent;
			this._buttonExit.FlatAppearance.BorderSize = 0;
			this._buttonExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonExit.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this._buttonExit.ForeColor = System.Drawing.Color.Black;
			this._buttonExit.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this._buttonExit.Location = new System.Drawing.Point(0, 292);
			this._buttonExit.Margin = new System.Windows.Forms.Padding(0);
			this._buttonExit.Name = "_buttonExit";
			this._buttonExit.Size = new System.Drawing.Size(235, 82);
			this._buttonExit.TabIndex = 0;
			this._buttonExit.Text = "Exit";
			this._buttonExit.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
			this._buttonExit.UseVisualStyleBackColor = false;
			this._buttonExit.Click += new System.EventHandler(this.ButtonExit_Click);
			this._buttonExit.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ButtonExit_MouseUp);
			// 
			// _buttonAbout
			// 
			this._buttonAbout.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonAbout.BackColor = System.Drawing.Color.Transparent;
			this._buttonAbout.FlatAppearance.BorderSize = 0;
			this._buttonAbout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonAbout.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this._buttonAbout.ForeColor = System.Drawing.Color.Black;
			this._buttonAbout.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this._buttonAbout.Location = new System.Drawing.Point(0, 210);
			this._buttonAbout.Margin = new System.Windows.Forms.Padding(0);
			this._buttonAbout.Name = "_buttonAbout";
			this._buttonAbout.Size = new System.Drawing.Size(235, 82);
			this._buttonAbout.TabIndex = 0;
			this._buttonAbout.Text = "About";
			this._buttonAbout.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
			this._buttonAbout.UseVisualStyleBackColor = false;
			this._buttonAbout.Click += new System.EventHandler(this.ButtonAbout_Click);
			this._buttonAbout.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ButtonAbout_MouseUp);
			// 
			// _panelSelection
			// 
			this._panelSelection.BackColor = System.Drawing.Color.Gray;
			this._panelSelection.Location = new System.Drawing.Point(0, 128);
			this._panelSelection.Name = "_panelSelection";
			this._panelSelection.Size = new System.Drawing.Size(13, 82);
			this._panelSelection.TabIndex = 3;
			// 
			// _buttonMainGrid
			// 
			this._buttonMainGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonMainGrid.BackColor = System.Drawing.Color.Transparent;
			this._buttonMainGrid.FlatAppearance.BorderSize = 0;
			this._buttonMainGrid.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonMainGrid.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this._buttonMainGrid.ForeColor = System.Drawing.Color.Black;
			this._buttonMainGrid.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this._buttonMainGrid.Location = new System.Drawing.Point(0, 128);
			this._buttonMainGrid.Margin = new System.Windows.Forms.Padding(0);
			this._buttonMainGrid.Name = "_buttonMainGrid";
			this._buttonMainGrid.Size = new System.Drawing.Size(235, 82);
			this._buttonMainGrid.TabIndex = 0;
			this._buttonMainGrid.Text = "Main Grid";
			this._buttonMainGrid.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
			this._buttonMainGrid.UseVisualStyleBackColor = false;
			this._buttonMainGrid.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ButtonMainGrid_MouseUp);
			// 
			// _panelTitle
			// 
			this._panelTitle.Dock = System.Windows.Forms.DockStyle.Top;
			this._panelTitle.Location = new System.Drawing.Point(235, 0);
			this._panelTitle.Name = "_panelTitle";
			this._panelTitle.Size = new System.Drawing.Size(993, 106);
			this._panelTitle.TabIndex = 3;
			// 
			// _panelSolutionName
			// 
			this._panelSolutionName.Controls.Add(this._labelSolutionName);
			this._panelSolutionName.Dock = System.Windows.Forms.DockStyle.Top;
			this._panelSolutionName.Location = new System.Drawing.Point(0, 0);
			this._panelSolutionName.Name = "_panelSolutionName";
			this._panelSolutionName.Size = new System.Drawing.Size(235, 125);
			this._panelSolutionName.TabIndex = 5;
			// 
			// _labelSolutionName
			// 
			this._labelSolutionName.AutoSize = true;
			this._labelSolutionName.BackColor = System.Drawing.Color.Transparent;
			this._labelSolutionName.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this._labelSolutionName.ForeColor = System.Drawing.Color.Black;
			this._labelSolutionName.Location = new System.Drawing.Point(3, 51);
			this._labelSolutionName.Margin = new System.Windows.Forms.Padding(0);
			this._labelSolutionName.Name = "_labelSolutionName";
			this._labelSolutionName.Size = new System.Drawing.Size(228, 23);
			this._labelSolutionName.TabIndex = 4;
			this._labelSolutionName.Text = "Sunnie\'s Sudoku Solution";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.CancelButton = this._buttonExit;
			this.ClientSize = new System.Drawing.Size(1228, 775);
			this.Controls.Add(this._pictureBoxGrid);
			this.Controls.Add(this._panelTitle);
			this.Controls.Add(this._panelMenu);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Name = "MainForm";
			this.Text = "[MainForm]";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseDown);
			((System.ComponentModel.ISupportInitialize)(this._pictureBoxGrid)).EndInit();
			this._panelMenu.ResumeLayout(false);
			this._panelSolutionName.ResumeLayout(false);
			this._panelSolutionName.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.PictureBox _pictureBoxGrid;
		private System.Windows.Forms.Panel _panelMenu;
		private System.Windows.Forms.Button _buttonMainGrid;
		private System.Windows.Forms.Button _buttonAbout;
		private System.Windows.Forms.Panel _panelSelection;
		private System.Windows.Forms.Button _buttonExit;
		private System.Windows.Forms.Panel _panelTitle;
		private System.Windows.Forms.Panel _panelSolutionName;
		private System.Windows.Forms.Label _labelSolutionName;
	}
}

