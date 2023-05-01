namespace SudokuStudio.Views.Pages;

/// <summary>
/// Defines a drawing page.
/// </summary>
[DependencyProperty<int>("SelectedColorIndex", DefaultValue = -1, Accessibility = GeneralizedAccessibility.Internal, DocSummary = "Indicates the selected color index.")]
[DependencyProperty<string>("BabaGroupNameInput", IsNullable = true, Accessibility = GeneralizedAccessibility.Internal, DocSummary = "Indicates the input character that is used as a baba group variable.")]
[DependencyProperty<DrawingMode>("SelectedMode", DefaultValue = DrawingMode.Cell, Accessibility = GeneralizedAccessibility.Internal, DocSummary = "Indicates the selected drawing mode.")]
[DependencyProperty<Inference>("LinkKind", DefaultValue = Inference.Strong, Accessibility = GeneralizedAccessibility.Internal, DocSummary = "Indicates the link type.")]
[DependencyProperty<ColorPalette>("UserDefinedColorPalette", Accessibility = GeneralizedAccessibility.Internal, DocReferencedMemberName = "global::SudokuStudio.Configuration.UIPreferenceGroup.UserDefinedColorPalette")]
public sealed partial class DrawingPage : Page
{
	[DefaultValue]
	private static readonly ColorPalette UserDefinedColorPaletteDefaultValue =
		((App)Application.Current).Preference.UIPreferences.UserDefinedColorPalette;


	/// <summary>
	/// Defines a local view.
	/// </summary>
	private readonly ViewUnitBindableSource _localView = new() { Conclusions = Array.Empty<Conclusion>(), View = View.Empty };

	/// <summary>
	/// Indicates the previously selected candidate.
	/// </summary>
	private Candidate? _previousSelectedCandidate;


	/// <summary>
	/// Initializes a <see cref="DrawingPage"/> instance.
	/// </summary>
	public DrawingPage() => InitializeComponent();


	/// <inheritdoc/>
	protected override void OnNavigatedTo(NavigationEventArgs e)
	{
		base.OnNavigatedTo(e);

		if (e is not { NavigationMode: NavigationMode.New, Parameter: Grid grid })
		{
			return;
		}

		SudokuPane.Puzzle = grid;
	}

	private void SetSelectedMode(int selectedIndex) => SelectedMode = (DrawingMode)(selectedIndex + 1);

	private void SetLinkType(int selectedIndex)
		=> LinkKind = Enum.Parse<Inference>((string)((ComboBoxItem)LinkTypeChoser.Items[selectedIndex]).Tag!);

	/// <summary>
	/// Try to update view unit.
	/// </summary>
	private void UpdateViewUnit()
	{
		SudokuPane.ViewUnit = null; // Change the reference to update view.
		SudokuPane.ViewUnit = _localView;
	}

	private bool CheckCellNode(int index, GridClickedEventArgs e)
	{
		switch (e)
		{
			case { Cell: var cell, MouseButton: MouseButton.Left }:
			{
				if (_localView.View.Find(node => node is CellViewNode { Cell: var c } && c == cell) is { } foundNode)
				{
					_localView.View.Remove(foundNode);
				}
				else
				{
					var id = UserDefinedColorPalette[index].GetIdentifier();
					_localView.View.Add(new CellViewNode(id, cell));
				}

				UpdateViewUnit();

				break;
			}
		}

		return true;
	}

	private bool CheckCandidateNode(int index, GridClickedEventArgs e)
	{
		switch (e)
		{
			case { Candidate: var candidate, MouseButton: MouseButton.Left }:
			{
				if (_localView.View.Find(node => node is CandidateViewNode { Candidate: var c } && c == candidate) is { } foundNode)
				{
					_localView.View.Remove(foundNode);
				}
				else
				{
					var id = UserDefinedColorPalette[index].GetIdentifier();
					_localView.View.Add(new CandidateViewNode(id, candidate));
				}

				UpdateViewUnit();

				break;
			}
		}

		return true;
	}

	private bool CheckHouseNode(int index, GridClickedEventArgs e)
	{
		switch (e)
		{
			case { Candidate: var candidate2, MouseButton: MouseButton.Left }:
			{
				if (_previousSelectedCandidate is not { } candidate1)
				{
					_previousSelectedCandidate = candidate2;
					return false;
				}

				var cell1 = candidate1 / 9;
				var cell2 = candidate2 / 9;
				if ((CellsMap[cell1] + cell2).CoveredHouses is not (var coveredHouses and not 0))
				{
					_previousSelectedCandidate = null;
					return true;
				}

				var house = TrailingZeroCount(coveredHouses);
				if (_localView.View.Find(node => node is HouseViewNode { House: var h } && h == house) is { } foundNode)
				{
					_localView.View.Remove(foundNode);
				}
				else
				{
					var id = UserDefinedColorPalette[index].GetIdentifier();
					_localView.View.Add(new HouseViewNode(id, house));
				}

				UpdateViewUnit();

				break;
			}
		}

		return true;
	}

	private bool CheckChuteNode(int index, GridClickedEventArgs e)
	{
		switch (e)
		{
			case { Candidate: var candidate2, MouseButton: MouseButton.Left }:
			{
				if (_previousSelectedCandidate is not { } candidate1)
				{
					_previousSelectedCandidate = candidate2;
					return false;
				}

				var (mr1, mc1) = GridClickedEventArgs.GetChute(candidate1);
				var (mr2, mc2) = GridClickedEventArgs.GetChute(candidate2);
				if (mr1 == mr2)
				{
					if (_localView.View.Find(node => node is ChuteViewNode { ChuteIndex: var c } && c == mr1) is { } foundNode)
					{
						_localView.View.Remove(foundNode);
					}
					else
					{
						var id = UserDefinedColorPalette[index].GetIdentifier();
						_localView.View.Add(new ChuteViewNode(id, mr1));
					}

					UpdateViewUnit();

					break;
				}

				if (mc1 == mc2)
				{
					if (_localView.View.Find(node => node is ChuteViewNode { ChuteIndex: var c } && c - 3 == mc1) is { } foundNode)
					{
						_localView.View.Remove(foundNode);
					}
					else
					{
						var id = UserDefinedColorPalette[index].GetIdentifier();
						_localView.View.Add(new ChuteViewNode(id, mc1 + 3));
					}

					UpdateViewUnit();

					break;
				}

				break;
			}
		}

		return true;
	}

	private bool CheckLinkNode(GridClickedEventArgs e)
	{
		switch (e)
		{
			case { Candidate: var candidate2, MouseButton: MouseButton.Left }:
			{
				if (_previousSelectedCandidate is not { } candidate1)
				{
					_previousSelectedCandidate = candidate2;
					return false;
				}

				var cell1 = candidate1 / 9;
				var cell2 = candidate2 / 9;
				var digit1 = candidate1 % 9;
				var digit2 = candidate2 % 9;
				if (_localView.View.Find(predicate) is { } foundNode)
				{
					_localView.View.Remove(foundNode);
				}
				else
				{
					var lt1 = new LockedTarget(candidate1 % 9, CellsMap[candidate1 / 9]);
					var lt2 = new LockedTarget(candidate2 % 9, CellsMap[candidate2 / 9]);
					_localView.View.Add(new LinkViewNode(default!, lt1, lt2, LinkKind)); // Link nodes don't use identifier to display colors.
				}

				UpdateViewUnit();

				break;


				bool predicate(ViewNode element)
					=> element switch
					{
						LinkViewNode { Start.Cells: [var c1], End.Cells: [var c2], Inference: Inference.Default }
							=> c1 == cell1 && c2 == cell2 || c2 == cell1 && c1 == cell2,
						LinkViewNode { Start: { Cells: [var c1], Digit: var d1 }, End: { Cells: [var c2], Digit: var d2 } }
							=> c1 == cell1 && c2 == cell2 && d1 == digit1 && d2 == digit2
							|| c2 == cell1 && c1 == cell2 && d2 == digit1 && d1 == digit2
					};
			}
		}

		return true;
	}

	private bool CheckBabaGroupingNode(int index, GridClickedEventArgs e)
	{
		switch (BabaGroupNameInput)
		{
			case null or []:
			{
				return true;
			}
			case [var character]:
			{
				switch (e)
				{
					case { Candidate: var candidate }:
					{
						var cell = candidate / 9;
						if (_localView.View.Find(node => node is BabaGroupViewNode { Cell: var c } && c == cell) is { } foundNode)
						{
							_localView.View.Remove(foundNode);
						}
						else
						{
							var id = UserDefinedColorPalette[index].GetIdentifier();
							_localView.View.Add(new BabaGroupViewNode(id, cell, (Utf8Char)character, Grid.MaxCandidatesMask));
						}

						UpdateViewUnit();

						break;
					}
				}

				break;
			}
			default:
			{
				InvalidInputInfoDisplayer.Visibility = Visibility.Visible;
				break;
			}
		}

		return true;
	}


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
		if (e.MouseButton == MouseButton.Right)
		{
			goto ClearField;
		}

		var shouldClearValue = this switch
		{
			{ SelectedMode: DrawingMode.Cell, SelectedColorIndex: var index and not -1 } => CheckCellNode(index, e),
			{ SelectedMode: DrawingMode.Candidate, SelectedColorIndex: var index and not -1 } => CheckCandidateNode(index, e),
			{ SelectedMode: DrawingMode.House, SelectedColorIndex: var index and not -1 } => CheckHouseNode(index, e),
			{ SelectedMode: DrawingMode.Chute, SelectedColorIndex: var index and not -1 } => CheckChuteNode(index, e),
			{ SelectedMode: DrawingMode.Link } => CheckLinkNode(e),
			{ SelectedMode: DrawingMode.BabaGrouping, SelectedColorIndex: var index and not -1 } => CheckBabaGroupingNode(index, e),
			_ => true
		};
		if (shouldClearValue)
		{
			goto ClearField;
		}

		return;

	ClearField:
		_previousSelectedCandidate = null;
	}
}
