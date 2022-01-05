namespace Sudoku.UI.Windows;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
	/// <summary>
	/// Initializes a <see cref="MainWindow"/> instance.
	/// </summary>
	public MainWindow() => InitializeComponent();


	private void myButton_Click(object sender, RoutedEventArgs e)
	{
		myButton.Content = "Clicked";
	}
}
