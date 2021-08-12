using System.Windows;
using Sudoku.IO;

namespace Sudoku.Windows;

/// <summary>
/// Interaction logic for <c>BatchWindow.xaml</c>.
/// </summary>
public partial class BatchWindow : Window
{
	/// <summary>
	/// The internal settings.
	/// </summary>
	private readonly WindowsSettings _settings;


	/// <summary>
	/// Initializes a <see cref="BatchWindow"/> instance, with a <see cref="WindowsSettings"/> instance.
	/// </summary>
	/// <param name="settings"></param>
	public BatchWindow(WindowsSettings settings)
	{
		InitializeComponent();

		_settings = settings;
	}


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
