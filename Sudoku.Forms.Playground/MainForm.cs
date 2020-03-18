using System;
using System.Windows.Forms;

namespace Sudoku.Forms.Playground
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
			InitializeAfterBase();
		}


		private void InitializeAfterBase()
		{
			// TODO: Initialization for other controls.
			//_grid.Width = _grid.Height;
		}

		private void ShowForm<TForm>(bool byDialog)
			where TForm : Form, new()
		{
			var form = new TForm();
			if (byDialog)
			{
				form.ShowDialog();
			}
			else
			{
				form.Show();
			}
		}


		private void ToolStripMenuItem_aboutAuthor_Click(object sender, EventArgs e) =>
			ShowForm<AboutBox>(false);

		private void ToolStripMenuItem_fileQuit_Click(object sender, EventArgs e) =>
			Close();
	}
}
