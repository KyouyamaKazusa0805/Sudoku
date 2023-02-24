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
	/// Defines a local view.
	/// </summary>
	private readonly ViewUnit _localView = new() { Conclusions = ImmutableArray<Conclusion>.Empty, View = View.Empty };


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

	private void SudokuPane_Clicked(SudokuPane sender, GridClickedEventArgs e)
	{
		switch (this)
		{
			case { SelectedMode: DrawingMode.Cell, SelectedColorIndex: var index and not -1 }:
			{
				switch (e)
				{
					case { Cell: var cell, MouseButton: MouseButton.Left }:
					{
						if (_localView.View.Exists(element => element is CellViewNode { Cell: var c } && c == cell, out var foundNode))
						{
							_localView.View.Remove(foundNode);
						}
						else
						{
							var id = UserDefinedColorPalette[index].GetIdentifier();
							_localView.View.Add(new CellViewNode(id, cell));
						}

						SudokuPane.ViewUnit = null; // Change the reference to update view.
						SudokuPane.ViewUnit = _localView;

						break;
					}
				}

				break;
			}
			case { SelectedMode: DrawingMode.Candidate, SelectedColorIndex: var index and not -1 }:
			{
				switch (e)
				{
					case { Candidate: var candidate, MouseButton: MouseButton.Left }:
					{
						if (_localView.View.Exists(element => element is CandidateViewNode { Candidate: var c } && c == candidate, out var foundNode))
						{
							_localView.View.Remove(foundNode);
						}
						else
						{
							var id = UserDefinedColorPalette[index].GetIdentifier();
							_localView.View.Add(new CandidateViewNode(id, candidate));
						}

						SudokuPane.ViewUnit = null; // Change the reference to update view.
						SudokuPane.ViewUnit = _localView;

						break;
					}
				}

				break;
			}
		}
	}
}
