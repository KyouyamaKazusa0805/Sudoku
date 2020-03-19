namespace Sudoku.Forms.Subpages
{
	partial class GridPanel
	{
		/// <summary> 
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// 清理所有正在使用的资源。
		/// </summary>
		/// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region 组件设计器生成的代码

		/// <summary> 
		/// 设计器支持所需的方法 - 不要修改
		/// 使用代码编辑器修改此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			this._pictureBoxGrid = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this._pictureBoxGrid)).BeginInit();
			this.SuspendLayout();
			// 
			// _pictureBoxGrid
			// 
			this._pictureBoxGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._pictureBoxGrid.Dock = System.Windows.Forms.DockStyle.Left;
			this._pictureBoxGrid.Location = new System.Drawing.Point(0, 0);
			this._pictureBoxGrid.Margin = new System.Windows.Forms.Padding(0);
			this._pictureBoxGrid.Name = "_pictureBoxGrid";
			this._pictureBoxGrid.Size = new System.Drawing.Size(540, 540);
			this._pictureBoxGrid.TabIndex = 2;
			this._pictureBoxGrid.TabStop = false;
			// 
			// GridPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._pictureBoxGrid);
			this.Name = "GridPanel";
			this.Size = new System.Drawing.Size(948, 540);
			this.Load += new System.EventHandler(this.GridPanel_Load);
			this.SizeChanged += new System.EventHandler(this.GridPanel_SizeChanged);
			((System.ComponentModel.ISupportInitialize)(this._pictureBoxGrid)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox _pictureBoxGrid;
	}
}
