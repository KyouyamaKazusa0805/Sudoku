namespace SudokuStudio.Views.Controls;

/// <summary>
/// Defines a cell displayed in a <see cref="SudokuPane"/>.
/// </summary>
/// <seealso cref="SudokuPane"/>
[DependencyProperty<short>("CandidatesMask", DefaultValue = 511)]
[DependencyProperty<CellStatus>("Status", DefaultValue = CellStatus.Empty)]
internal sealed partial class SudokuPaneCell : UserControl
{
	/// <summary>
	/// Indicates the selected cell. The value is temporarily assigned into here, from <see cref="BasePane"/> property.
	/// </summary>
	/// <remarks>
	/// This field is represented as a rescue to get the selected cell from parent control (<see cref="BasePane"/> property).
	/// If the context flyout is opened, the parent control will return -1 of its property <see cref="SudokuPane.SelectedCell"/>.
	/// here we should get that value before the flyout is opened.
	/// Therefore, this field will be used and assigned by method <see cref="Flyout_Opening(object, object)"/>,
	/// and the method is an only one to modify the field.
	/// </remarks>
	/// <seealso cref="BasePane"/>
	/// <seealso cref="Flyout_Opening(object, object)"/>
	private int _temporarySelectedCell = -1;


	/// <summary>
	/// Initializes a <see cref="SudokuPaneCell"/> instance.
	/// </summary>
	public SudokuPaneCell() => InitializeComponent();


#nullable disable
	/// <summary>
	/// Indicates the base pane.
	/// </summary>
	public SudokuPane BasePane { get; set; }
#nullable restore

	/// <summary>
	/// Indicates the cell index.
	/// </summary>
	internal int CellIndex { get; init; }


	private void UserControl_PointerEntered(object sender, PointerRoutedEventArgs e) => BasePane.SelectedCell = CellIndex;

	private void UserControl_PointerExited(object sender, PointerRoutedEventArgs e) => BasePane.SelectedCell = -1;

	private void TextBlock_PointerEntered(object sender, PointerRoutedEventArgs e) => BasePane.SelectedCell = CellIndex;

	private void TextBlock_PointerExited(object sender, PointerRoutedEventArgs e) => BasePane.SelectedCell = CellIndex;

	private void TextBlock_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
	{
		if (!BasePane.EnableDoubleTapFilling)
		{
			return;
		}

		if ((this, sender) is not ({ BasePane: { Puzzle: var modified, SelectedCell: var cell and not -1 } }, TextBlock { Text: var text }))
		{
			return;
		}

		if (!int.TryParse(text, out var originalDigit)
			|| originalDigit - 1 is not (var digit and >= 0 and < 9)
			|| modified.GetStatus(cell) != CellStatus.Empty
			|| (modified.GetCandidates(cell) >> digit & 1) == 0)
		{
			return;
		}

		modified[cell] = digit;
		BasePane.SetPuzzle(modified);

		BasePane.TriggerGridUpdated(GridUpdatedBehavior.Assignment, cell * 9 + digit);
	}

	private void TextBlock_RightTapped(object sender, RightTappedRoutedEventArgs e)
	{
		if (!BasePane.EnableRightTapRemoving)
		{
			return;
		}

		if ((this, sender) is not ({ BasePane: { Puzzle: var modified, SelectedCell: var cell and not -1 } }, TextBlock { Text: var text }))
		{
			return;
		}

		if (!int.TryParse(text, out var originalDigit)
			|| originalDigit - 1 is not (var digit and >= 0 and < 9)
			|| modified.GetStatus(cell) != CellStatus.Empty
			|| (modified.GetCandidates(cell) >> digit & 1) == 0)
		{
			return;
		}

		modified[cell, digit] = false;
		BasePane.SetPuzzle(modified);

		BasePane.TriggerGridUpdated(GridUpdatedBehavior.Elimination, cell * 9 + digit);
	}

	private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
	{
		if ((this, sender) is not ({ BasePane.SelectedCell: var cell and not -1 }, TextBlock { Text: var text }))
		{
			return;
		}

		if (!int.TryParse(text, out var originalDigit) || originalDigit - 1 is not (var digit and >= 0 and < 9))
		{
			return;
		}

		BasePane.TriggerClicked(cell * 9 + digit);
	}

	private void InputSetter_KeyDown(object sender, KeyRoutedEventArgs e)
	{
		if (
#pragma warning disable format
			(this, sender, e) is not (
				{ BasePane.Puzzle: var modified, _temporarySelectedCell: var cell and not -1 },
				TextBox { Text: var text, Parent: StackPanel { Parent: FlyoutPresenter { Parent: Popup p } } },
				{ Key: VirtualKey.Enter }
			)
#pragma warning restore format
		)
		{
			return;
		}

		if (modified.GetStatus(cell) is not (var cellStatus and not CellStatus.Given))
		{
			return;
		}

		if (!DigitRange.TryParse(text, out var digitRange))
		{
			return;
		}

		_ = digitRange is { DigitsMask: var digits, ConclusionType: var type };
		if (type == Assignment)
		{
			if (cellStatus == CellStatus.Modifiable)
			{
				// A rescue.
				modified[cell] = -1;
			}

			var digit = TrailingZeroCount(digits);
			modified[cell] = digit;

			BasePane.SetPuzzle(modified);

			BasePane.TriggerGridUpdated(GridUpdatedBehavior.Assignment, cell * 9 + digit);
		}
		else if (type == Elimination && cellStatus == CellStatus.Empty)
		{
			foreach (var digit in digits)
			{
				if (modified.Exists(cell, digit) is true)
				{
					modified[cell, digit] = false;
				}
			}

			BasePane.SetPuzzle(modified);

			BasePane.TriggerGridUpdated(GridUpdatedBehavior.EliminationMultiple, (short)(cell << 9) | digits);
		}

		// Closes the flyout manually.
		p.IsOpen = false;
	}

	private void Flyout_Opening(object sender, object e)
	{
		if (BasePane is not { SelectedCell: var cell and not -1, Puzzle: var puzzle, DisableFlyout: var disableFlyout }
			|| puzzle.GetStatus(cell) != CellStatus.Empty
			|| disableFlyout)
		{
			MainGridContextFlyout.Hide();
			return;
		}

		_temporarySelectedCell = cell;
	}
}
