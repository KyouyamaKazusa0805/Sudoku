namespace SudokuStudio.Views.Pages;

/// <summary>
/// The hub page of introduction of techniques.
/// </summary>
public sealed partial class TechniqueFilePage : Page
{
	/// <summary>
	/// Initializes a <see cref="TechniqueFilePage"/> instance.
	/// </summary>
	public TechniqueFilePage() => InitializeComponent();


	/// <summary>
	/// Gets the main window.
	/// </summary>
	/// <returns>The main window.</returns>
	/// <exception cref="InvalidOperationException">Throws when the base window cannot be found.</exception>
	private MainWindow GetMainWindow()
		=> ((App)Application.Current).WindowManager.GetWindowForElement(this) switch
		{
			MainWindow mainWindow => mainWindow,
			_ => throw new InvalidOperationException("Main window cannot be found.")
		};


	private void GridView_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (e is not { ClickedItem: TechniquePageRoutingData { RoutingPageTypeName: var typeName } })
		{
			return;
		}

		GetMainWindow().NavigateToPage(GetType().Assembly.GetType(typeName)!);
	}
}
