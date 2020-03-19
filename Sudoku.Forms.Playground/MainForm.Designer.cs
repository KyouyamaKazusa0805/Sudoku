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
			this._panelTitle = new System.Windows.Forms.Panel();
			this._buttonMainGrid = new System.Windows.Forms.Button();
			this._panelSolutionName = new System.Windows.Forms.Panel();
			this._labelSolutionName = new System.Windows.Forms.Label();
			this._panelMenu.SuspendLayout();
			this._panelSolutionName.SuspendLayout();
			this.SuspendLayout();
			// 
			// _panelMenu
			// 
			this._panelMenu.Controls.Add(this._panelSolutionName);
			this._panelMenu.Controls.Add(this._buttonMainGrid);
			this._panelMenu.Dock = System.Windows.Forms.DockStyle.Left;
			this._panelMenu.Location = new System.Drawing.Point(0, 0);
			this._panelMenu.Name = "_panelMenu";
			this._panelMenu.Size = new System.Drawing.Size(200, 721);
			this._panelMenu.TabIndex = 0;
			// 
			// _panelTitle
			// 
			this._panelTitle.Dock = System.Windows.Forms.DockStyle.Top;
			this._panelTitle.Location = new System.Drawing.Point(200, 0);
			this._panelTitle.Name = "_panelTitle";
			this._panelTitle.Size = new System.Drawing.Size(1088, 100);
			this._panelTitle.TabIndex = 1;
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
			// 
			// _panelSolutionName
			// 
			this._panelSolutionName.Controls.Add(this._labelSolutionName);
			this._panelSolutionName.Dock = System.Windows.Forms.DockStyle.Top;
			this._panelSolutionName.Location = new System.Drawing.Point(0, 0);
			this._panelSolutionName.Margin = new System.Windows.Forms.Padding(0);
			this._panelSolutionName.Name = "_panelSolutionName";
			this._panelSolutionName.Size = new System.Drawing.Size(200, 138);
			this._panelSolutionName.TabIndex = 1;
			// 
			// _labelSolutionName
			// 
			this._labelSolutionName.AutoSize = true;
			this._labelSolutionName.Location = new System.Drawing.Point(6, 56);
			this._labelSolutionName.Margin = new System.Windows.Forms.Padding(0);
			this._labelSolutionName.Name = "_labelSolutionName";
			this._labelSolutionName.Size = new System.Drawing.Size(191, 20);
			this._labelSolutionName.TabIndex = 0;
			this._labelSolutionName.Text = "Sunnie\'s sudoku Solution";
			this._labelSolutionName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// MainForm
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1288, 721);
			this.Controls.Add(this._panelTitle);
			this.Controls.Add(this._panelMenu);
			this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Name = "MainForm";
			this.Text = "[MainForm]";
			this._panelMenu.ResumeLayout(false);
			this._panelSolutionName.ResumeLayout(false);
			this._panelSolutionName.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel _panelMenu;
		private System.Windows.Forms.Panel _panelTitle;
		private System.Windows.Forms.Button _buttonMainGrid;
		private System.Windows.Forms.Panel _panelSolutionName;
		private System.Windows.Forms.Label _labelSolutionName;
	}
}

