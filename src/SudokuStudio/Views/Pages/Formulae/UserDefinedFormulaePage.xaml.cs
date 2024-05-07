namespace SudokuStudio.Views.Pages.Formulae;

/// <summary>
/// Represents a user-defined formulae page.
/// </summary>
public sealed partial class UserDefinedFormulaePage : Page
{
	/// <summary>
	/// Initializes a <see cref="UserDefinedFormulaePage"/> instance.
	/// </summary>
	public UserDefinedFormulaePage() => InitializeComponent();


	private void AddFormulaButton_Click(object sender, RoutedEventArgs e) => App.GetMainWindow(this).NavigateToPage<AddFormulaPage>();

	private void Page_Loaded(object sender, RoutedEventArgs e)
	{
		var di = new DirectoryInfo(CommonPaths.Formulae);
		if (!di.Exists)
		{
			di.Create();
			return;
		}

		foreach (var fi in di.EnumerateFiles())
		{
			if (!FormulaExpression.TryLoadFromLocal(fi.FullName, out var result))
			{
				continue;
			}

			FormulaeDisplayer.Children.Add(
				new SettingsCard
				{
					Header = result.Name,
					HeaderIcon = new FontIcon { Glyph = "\uE94E" },
					Description = result.Description is var description && !string.IsNullOrWhiteSpace(description)
						? description
						: ResourceDictionary.Get("UserDefinedFormulaePage_NoDescription", App.CurrentCulture),
					Content = result.Expression
				}
			);
		}
	}
}
