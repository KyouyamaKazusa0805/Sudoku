using System.Windows;
using Sudoku.DocComments;
using Sudoku.IO;

namespace Sudoku.Windows
{
	/// <summary>
	/// Interaction logic for <c>BatchWindow.xaml</c>.
	/// </summary>
	public partial class BatchWindow : Window
	{
		/// <summary>
		/// The internal settings.
		/// </summary>
		private readonly WindowsSettings _settings;


		/// <inheritdoc cref="DefaultConstructor"/>
		public BatchWindow(WindowsSettings settings)
		{
			InitializeComponent();

			_settings = settings;
		}


		/// <inheritdoc cref="Events.Click(object?, System.EventArgs)"/>
		private void ButtonBatch_Click(object sender, RoutedEventArgs e)
		{
			if (!BatchExecutor.TryParse(_textBoxBatch.Text, _settings, out var result))
			{
				e.Handled = true;
				return;
			}

			result.Execute();

			Messagings.SaveSuccess();
		}
	}
}
