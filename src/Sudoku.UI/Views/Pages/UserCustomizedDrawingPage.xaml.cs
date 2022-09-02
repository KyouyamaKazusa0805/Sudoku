namespace Sudoku.UI.Views.Pages;

/// <summary>
/// Defines a page that can draw onto a sudoku grid.
/// </summary>
[Page]
public sealed partial class UserCustomizedDrawingPage : Page
{
	/// <summary>
	/// Initializes a <see cref="UserCustomizedDrawingPage"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public UserCustomizedDrawingPage() => InitializeComponent();


	/// <summary>
	/// Triggers when the page is loaded.
	/// </summary>
	/// <param name="sender">The object triggering this event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void Page_Loaded(object sender, RoutedEventArgs e)
	{
	}
}
