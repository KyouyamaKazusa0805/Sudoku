namespace SudokuStudio.Views.Pages;

/// <summary>
/// The hub page of introduction of techniques.
/// </summary>
public sealed partial class TechniqueFilePage : Page
{
	/// <summary>
	/// 
	/// </summary>
	private readonly ObservableCollection<TechniquePageRoutingData> _singles = new()
	{
		new()
		{
			Technique = Technique.FullHouse,
			RoutingPageTypeName = typeof(FullHousePage).Name
		},
		new()
		{
			Technique = Technique.LastDigit,
			RoutingPageTypeName = typeof(FullHousePage).Name
		},
		new()
		{
			Technique = Technique.HiddenSingleBlock,
			RoutingPageTypeName = typeof(FullHousePage).Name
		},
		new()
		{
			Technique = Technique.HiddenSingleRow,
			RoutingPageTypeName = typeof(FullHousePage).Name
		},
		new()
		{
			Technique = Technique.HiddenSingleColumn,
			RoutingPageTypeName = typeof(FullHousePage).Name
		},
		new()
		{
			Technique = Technique.NakedSingle,
			RoutingPageTypeName = typeof(FullHousePage).Name
		}
	};


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
