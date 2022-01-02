namespace Nano;

/// <summary>
/// An empty window that can be used on its own or navigated to within a <see cref="Frame"/>.
/// </summary>
public sealed partial class MainWindow : Window
{
	/// <summary>
	/// Initializes a <see cref="MainWindow"/> instance.
	/// </summary>
	public MainWindow() => InitializeComponent();


	private void MyButton_Click([IsDiscard] object sender, [IsDiscard] RoutedEventArgs e) =>
		_MyButton.Content = "Clicked";
}
