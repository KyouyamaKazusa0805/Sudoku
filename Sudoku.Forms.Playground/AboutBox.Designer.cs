namespace Sudoku.Forms.Playground
{
	partial class AboutBox
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBox));
			this._tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this._pictureBoxLogo = new System.Windows.Forms.PictureBox();
			this._labelProductName = new System.Windows.Forms.Label();
			this._labelVersion = new System.Windows.Forms.Label();
			this._labelCopyright = new System.Windows.Forms.Label();
			this._labelCompanyName = new System.Windows.Forms.Label();
			this._textBoxDescription = new System.Windows.Forms.TextBox();
			this._buttonOk = new System.Windows.Forms.Button();
			this._tableLayoutPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._pictureBoxLogo)).BeginInit();
			this.SuspendLayout();
			// 
			// _tableLayoutPanel
			// 
			this._tableLayoutPanel.ColumnCount = 2;
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 67F));
			this._tableLayoutPanel.Controls.Add(this._pictureBoxLogo, 0, 0);
			this._tableLayoutPanel.Controls.Add(this._labelProductName, 1, 0);
			this._tableLayoutPanel.Controls.Add(this._labelVersion, 1, 1);
			this._tableLayoutPanel.Controls.Add(this._labelCopyright, 1, 2);
			this._tableLayoutPanel.Controls.Add(this._labelCompanyName, 1, 3);
			this._tableLayoutPanel.Controls.Add(this._textBoxDescription, 1, 4);
			this._tableLayoutPanel.Controls.Add(this._buttonOk, 1, 5);
			this._tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tableLayoutPanel.Location = new System.Drawing.Point(14, 13);
			this._tableLayoutPanel.Margin = new System.Windows.Forms.Padding(4);
			this._tableLayoutPanel.Name = "_tableLayoutPanel";
			this._tableLayoutPanel.RowCount = 6;
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this._tableLayoutPanel.Size = new System.Drawing.Size(624, 410);
			this._tableLayoutPanel.TabIndex = 0;
			// 
			// _pictureBoxLogo
			// 
			this._pictureBoxLogo.Dock = System.Windows.Forms.DockStyle.Fill;
			this._pictureBoxLogo.Image = ((System.Drawing.Image)(resources.GetObject("_pictureBoxLogo.Image")));
			this._pictureBoxLogo.Location = new System.Drawing.Point(4, 4);
			this._pictureBoxLogo.Margin = new System.Windows.Forms.Padding(4);
			this._pictureBoxLogo.Name = "_pictureBoxLogo";
			this._tableLayoutPanel.SetRowSpan(this._pictureBoxLogo, 6);
			this._pictureBoxLogo.Size = new System.Drawing.Size(197, 402);
			this._pictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this._pictureBoxLogo.TabIndex = 12;
			this._pictureBoxLogo.TabStop = false;
			// 
			// _labelProductName
			// 
			this._labelProductName.Dock = System.Windows.Forms.DockStyle.Fill;
			this._labelProductName.Location = new System.Drawing.Point(214, 0);
			this._labelProductName.Margin = new System.Windows.Forms.Padding(9, 0, 4, 0);
			this._labelProductName.MaximumSize = new System.Drawing.Size(0, 27);
			this._labelProductName.Name = "_labelProductName";
			this._labelProductName.Size = new System.Drawing.Size(406, 27);
			this._labelProductName.TabIndex = 19;
			this._labelProductName.Text = "Product Name";
			this._labelProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// _labelVersion
			// 
			this._labelVersion.Dock = System.Windows.Forms.DockStyle.Fill;
			this._labelVersion.Location = new System.Drawing.Point(214, 41);
			this._labelVersion.Margin = new System.Windows.Forms.Padding(9, 0, 4, 0);
			this._labelVersion.MaximumSize = new System.Drawing.Size(0, 27);
			this._labelVersion.Name = "_labelVersion";
			this._labelVersion.Size = new System.Drawing.Size(406, 27);
			this._labelVersion.TabIndex = 0;
			this._labelVersion.Text = "Version";
			this._labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// _labelCopyright
			// 
			this._labelCopyright.Dock = System.Windows.Forms.DockStyle.Fill;
			this._labelCopyright.Location = new System.Drawing.Point(214, 82);
			this._labelCopyright.Margin = new System.Windows.Forms.Padding(9, 0, 4, 0);
			this._labelCopyright.MaximumSize = new System.Drawing.Size(0, 27);
			this._labelCopyright.Name = "_labelCopyright";
			this._labelCopyright.Size = new System.Drawing.Size(406, 27);
			this._labelCopyright.TabIndex = 21;
			this._labelCopyright.Text = "Copyright";
			this._labelCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// _labelCompanyName
			// 
			this._labelCompanyName.Dock = System.Windows.Forms.DockStyle.Fill;
			this._labelCompanyName.Location = new System.Drawing.Point(214, 123);
			this._labelCompanyName.Margin = new System.Windows.Forms.Padding(9, 0, 4, 0);
			this._labelCompanyName.MaximumSize = new System.Drawing.Size(0, 27);
			this._labelCompanyName.Name = "_labelCompanyName";
			this._labelCompanyName.Size = new System.Drawing.Size(406, 27);
			this._labelCompanyName.TabIndex = 22;
			this._labelCompanyName.Text = "Company Name";
			this._labelCompanyName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// _textBoxDescription
			// 
			this._textBoxDescription.Dock = System.Windows.Forms.DockStyle.Fill;
			this._textBoxDescription.Location = new System.Drawing.Point(214, 168);
			this._textBoxDescription.Margin = new System.Windows.Forms.Padding(9, 4, 4, 4);
			this._textBoxDescription.Multiline = true;
			this._textBoxDescription.Name = "_textBoxDescription";
			this._textBoxDescription.ReadOnly = true;
			this._textBoxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this._textBoxDescription.Size = new System.Drawing.Size(406, 197);
			this._textBoxDescription.TabIndex = 23;
			this._textBoxDescription.TabStop = false;
			this._textBoxDescription.Text = "Description";
			// 
			// _buttonOk
			// 
			this._buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._buttonOk.Location = new System.Drawing.Point(508, 373);
			this._buttonOk.Margin = new System.Windows.Forms.Padding(4);
			this._buttonOk.Name = "_buttonOk";
			this._buttonOk.Size = new System.Drawing.Size(112, 33);
			this._buttonOk.TabIndex = 24;
			this._buttonOk.Text = "&OK";
			this._buttonOk.Click += new System.EventHandler(this.ButtonOk_Click);
			// 
			// AboutBox
			// 
			this.AcceptButton = this._buttonOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(652, 436);
			this.Controls.Add(this._tableLayoutPanel);
			this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutBox";
			this.Padding = new System.Windows.Forms.Padding(14, 13, 14, 13);
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "About Sudoku";
			this._tableLayoutPanel.ResumeLayout(false);
			this._tableLayoutPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._pictureBoxLogo)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel _tableLayoutPanel;
		private System.Windows.Forms.PictureBox _pictureBoxLogo;
		private System.Windows.Forms.Label _labelProductName;
		private System.Windows.Forms.Label _labelVersion;
		private System.Windows.Forms.Label _labelCopyright;
		private System.Windows.Forms.Label _labelCompanyName;
		private System.Windows.Forms.TextBox _textBoxDescription;
		private System.Windows.Forms.Button _buttonOk;
	}
}
