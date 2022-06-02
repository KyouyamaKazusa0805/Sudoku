namespace Sudoku.UI.Views.Pages;

/// <summary>
/// Indicates the documentation page.
/// </summary>
public sealed partial class DocumentationPage : Page
{
	/// <summary>
	/// The tree nodes used for displaying by <see cref="TreeView"/> instance.
	/// </summary>
	private IList<DocumentationFileInfo> _docNamesList;


	/// <summary>
	/// Initializes a <see cref="DocumentationPage"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DocumentationPage()
	{
		InitializeComponent();

		InitializeDocListViewNodes();
	}


	/// <summary>
	/// Initializes the name list of all docs.
	/// </summary>
	[MemberNotNull(nameof(_docNamesList))]
	private void InitializeDocListViewNodes()
	{
		string cultureInfoName = CultureInfo.CurrentUICulture.Name;
		bool p(ResourceDictionary d) => d.Source.AbsolutePath.Contains(cultureInfoName, StringComparison.InvariantCultureIgnoreCase);
		var resourceDic = Application.Current.Resources.MergedDictionaries.FirstOrDefault(p);

		const string queryPrefix = "Docs_";
		_docNamesList = (
			from key in resourceDic?.Keys.OfType<string>() ?? Array.Empty<string>()
			where key.StartsWith(queryPrefix)
			let result = resourceDic![key] as string
			where result is not null
			let pipeOperatorIndex = result.IndexOf('|')
			where pipeOperatorIndex != -1
			let displayName = result[..pipeOperatorIndex].Trim()
			let filePath = result[(pipeOperatorIndex + 1)..].Trim()
			let filePathUri = filePath
			select new DocumentationFileInfo { DisplayName = displayName, FilePath = filePath }
		).ToArray();
	}

	/// <summary>
	/// Loads the local documentation file.
	/// </summary>
	/// <returns>The task that operates the current method.</returns>
	private async Task LoadAssetFileAsync()
	{
		const string pathEnd = """Assets\docs\MainPage.md""";
		string location = typeof(MainWindow).Assembly.Location;
		string path = Pathing.Combine(location[..(location.IndexOf("bin") - 1)], pathEnd);

		_cMarkdownTextBlock.Text = await File.ReadAllTextAsync(path);
	}


	/// <summary>
	/// Triggers when the current page is loaded.
	/// </summary>
	/// <param name="sender">The object which triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private async void Page_LoadedAsync(object sender, RoutedEventArgs e) => await LoadAssetFileAsync();
}
