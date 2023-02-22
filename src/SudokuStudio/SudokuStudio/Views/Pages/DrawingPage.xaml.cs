namespace SudokuStudio.Views.Pages;

/// <summary>
/// Defines a drawing page.
/// </summary>
[DependencyProperty<int>("SelectedColorIndex", DefaultValue = -1, Accessibility = GeneralizedAccessibility.Internal, DocSummary = "Indicates the selected color index.")]
[DependencyProperty<DrawingMode>("SelectedMode", DefaultValue = DrawingMode.Cell, Accessibility = GeneralizedAccessibility.Internal, DocSummary = "Indicates the selected drawing mode.")]
[DependencyProperty<ColorPalette>("UserDefinedColorPalette", Accessibility = GeneralizedAccessibility.Internal, DocReferencedMemberName = "global::SudokuStudio.Configuration.UIPreferenceGroup.UserDefinedColorPalette")]
public sealed partial class DrawingPage : Page
{
	[DefaultValue]
	private static readonly ColorPalette UserDefinedColorPaletteDefaultValue =
		((App)Application.Current).Preference.UIPreferences.UserDefinedColorPalette;


	/// <summary>
	/// Initializes a <see cref="DrawingPage"/> instance.
	/// </summary>
	public DrawingPage() => InitializeComponent();


	/// <inheritdoc/>
	protected override void OnNavigatedTo(NavigationEventArgs e)
	{
		base.OnNavigatedTo(e);

		switch (e)
		{
			case { NavigationMode: NavigationMode.New, Parameter: Grid grid }:
			{
				SudokuPane.Puzzle = grid;

				break;
			}
		}
	}

	private void SetSelectedMode(int selectedIndex) => SelectedMode = (DrawingMode)(selectedIndex + 1);


	private void ColorPaletteButton_Click(object sender, RoutedEventArgs e)
	{
		if (sender is not Button { Tag: string s } || !int.TryParse(s, out var i))
		{
			return;
		}

		SelectedColorIndex = i;
	}
}
