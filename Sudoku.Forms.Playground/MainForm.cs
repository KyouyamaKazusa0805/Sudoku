using System.Reflection;
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
			ShowTitle();
		}

		private void ShowTitle()
		{
			var assembly = Assembly.GetExecutingAssembly();
			string version = assembly.GetName().Version.ToString();
			string title = assembly.GetCustomAttribute<AssemblyTitleAttribute>().Title;

			Text = $"{title} Ver {version}";
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
	}
}
