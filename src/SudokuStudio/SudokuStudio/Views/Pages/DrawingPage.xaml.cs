namespace SudokuStudio.Views.Pages;

/// <summary>
/// Defines a drawing page.
/// </summary>
[DependencyProperty<DrawingMode>("SelectedMode", DefaultValue = DrawingMode.Cell, Accessibility = GeneralizedAccessibility.Internal, DocSummary = "Indicates the selected drawing mode.")]
public sealed partial class DrawingPage : Page
{
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
}
