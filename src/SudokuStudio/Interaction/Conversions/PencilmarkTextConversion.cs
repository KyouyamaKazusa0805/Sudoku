namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about pencilmark text.
/// </summary>
internal static class PencilmarkTextConversion
{
	public static double GetFontSize(double globalFontSize, decimal givenFontScale, decimal modifiableFontScale, CellState state)
		=> globalFontSize * (double)(state == CellState.Modifiable ? modifiableFontScale : givenFontScale);

	public static double GetFontSizeSimple(double globalFontSize, decimal scale) => globalFontSize * (double)scale;

	public static Brush GetBrush(
		Color pencilmarkColor,
		Color deltaColor,
		CellState cellState,
		Grid solution,
		Cell cell,
		Mask candidatesMask,
		Digit digit,
		bool displayCandidates,
		bool useDifferentColorToDisplayDeltaDigits,
		ViewUnitBindableSource? viewUnit
	)
	{
		// Special case: If the candidate is drawn by a view node, display it.
		if (cellState == CellState.Empty && (viewUnit?.CandidateContains(cell * 9 + digit) ?? false))
		{
			return new SolidColorBrush(pencilmarkColor);
		}

		var defaultBrush = new SolidColorBrush();

		// If the cell is not empty, don't display it.
		if (cellState is CellState.Given or CellState.Modifiable)
		{
			return defaultBrush;
		}

		// If candidates should be hidden, don't display it.
		if (!displayCandidates)
		{
			return defaultBrush;
		}

		var solutionHasThatCandidate = !solution.IsUndefined && solution.GetDigit(cell) == digit;
		var candidatesContainThatDigit = (candidatesMask >> digit & 1) != 0;

		// If the candidate has been already deleted, just display it in a wrong color or normal color if not enabled "Display Delta Digits".
		if (solutionHasThatCandidate && !candidatesContainThatDigit)
		{
			return new SolidColorBrush(useDifferentColorToDisplayDeltaDigits ? deltaColor : pencilmarkColor);
		}

		// Display it in a normal color.
		return candidatesContainThatDigit ? new SolidColorBrush(pencilmarkColor) : defaultBrush;
	}
}
