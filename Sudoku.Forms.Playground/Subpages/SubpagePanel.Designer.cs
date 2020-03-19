namespace Sudoku.Forms.Subpages
{
	partial class SubpagePanel
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
			this._groupBoxPageName = new System.Windows.Forms.GroupBox();
			this.SuspendLayout();
			// 
			// _groupBoxPageName
			// 
			this._groupBoxPageName.Dock = System.Windows.Forms.DockStyle.Fill;
			this._groupBoxPageName.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this._groupBoxPageName.Location = new System.Drawing.Point(0, 0);
			this._groupBoxPageName.Name = "_groupBoxPageName";
			this._groupBoxPageName.Size = new System.Drawing.Size(1088, 642);
			this._groupBoxPageName.TabIndex = 0;
			this._groupBoxPageName.TabStop = false;
			this._groupBoxPageName.Text = "[_groupBoxPageName]";
			// 
			// SubpagePanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._groupBoxPageName);
			this.Name = "SubpagePanel";
			this.Size = new System.Drawing.Size(1088, 642);
			this.ResumeLayout(false);

		}

		#endregion

		protected System.Windows.Forms.GroupBox _groupBoxPageName;
	}
}
