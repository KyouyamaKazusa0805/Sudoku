namespace Sudoku.Forms.Subpages
{
	partial class AboutPanel
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
			this._labelAuthor = new System.Windows.Forms.Label();
			this._labelAuthorName = new System.Windows.Forms.Label();
			this._labelGitHub = new System.Windows.Forms.Label();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this._groupBoxPageName.SuspendLayout();
			this._tableLayoutPanelMain.SuspendLayout();
			this.SuspendLayout();
			// 
			// _groupBoxPageName
			// 
			this._groupBoxPageName.Controls.Add(this._tableLayoutPanelMain);
			this._groupBoxPageName.Text = "About";
			// 
			// _tableLayoutPanelMain
			// 
			this._tableLayoutPanelMain.ColumnCount = 2;
			this._tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this._tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this._tableLayoutPanelMain.Controls.Add(this._labelGitHub, 0, 1);
			this._tableLayoutPanelMain.Controls.Add(this._labelAuthorName, 1, 0);
			this._tableLayoutPanelMain.Controls.Add(this._labelAuthor, 0, 0);
			this._tableLayoutPanelMain.Controls.Add(this.linkLabel1, 1, 1);
			this._tableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tableLayoutPanelMain.Location = new System.Drawing.Point(3, 23);
			this._tableLayoutPanelMain.Name = "_tableLayoutPanelMain";
			this._tableLayoutPanelMain.RowCount = 5;
			this._tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this._tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this._tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this._tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this._tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this._tableLayoutPanelMain.Size = new System.Drawing.Size(1082, 616);
			this._tableLayoutPanelMain.TabIndex = 0;
			// 
			// _labelAuthor
			// 
			this._labelAuthor.Dock = System.Windows.Forms.DockStyle.Fill;
			this._labelAuthor.Font = new System.Drawing.Font("Microsoft YaHei UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this._labelAuthor.Location = new System.Drawing.Point(3, 0);
			this._labelAuthor.Name = "_labelAuthor";
			this._labelAuthor.Size = new System.Drawing.Size(535, 123);
			this._labelAuthor.TabIndex = 0;
			this._labelAuthor.Text = "Author";
			this._labelAuthor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// _labelAuthorName
			// 
			this._labelAuthorName.Dock = System.Windows.Forms.DockStyle.Fill;
			this._labelAuthorName.Font = new System.Drawing.Font("Microsoft YaHei UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this._labelAuthorName.Location = new System.Drawing.Point(544, 0);
			this._labelAuthorName.Name = "_labelAuthorName";
			this._labelAuthorName.Size = new System.Drawing.Size(535, 123);
			this._labelAuthorName.TabIndex = 1;
			this._labelAuthorName.Text = "Sunnie";
			this._labelAuthorName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// _labelGitHub
			// 
			this._labelGitHub.Dock = System.Windows.Forms.DockStyle.Fill;
			this._labelGitHub.Font = new System.Drawing.Font("Microsoft YaHei UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this._labelGitHub.Location = new System.Drawing.Point(3, 123);
			this._labelGitHub.Name = "_labelGitHub";
			this._labelGitHub.Size = new System.Drawing.Size(535, 123);
			this._labelGitHub.TabIndex = 2;
			this._labelGitHub.Text = "GitHub";
			this._labelGitHub.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// linkLabel1
			// 
			this.linkLabel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.linkLabel1.Font = new System.Drawing.Font("Microsoft YaHei UI", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.linkLabel1.Location = new System.Drawing.Point(544, 123);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(535, 123);
			this.linkLabel1.TabIndex = 10;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "https://github.com/Sunnie-Shine";
			this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// AboutPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Name = "AboutPanel";
			this._groupBoxPageName.ResumeLayout(false);
			this._tableLayoutPanelMain.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel _tableLayoutPanelMain;
		private System.Windows.Forms.Label _labelAuthor;
		private System.Windows.Forms.Label _labelGitHub;
		private System.Windows.Forms.Label _labelAuthorName;
		private System.Windows.Forms.LinkLabel linkLabel1;
	}
}
