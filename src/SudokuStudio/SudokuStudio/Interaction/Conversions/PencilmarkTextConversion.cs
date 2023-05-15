namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about pencilmark text.
/// </summary>
internal static class PencilmarkTextConversion
{
	public static double GetFontSize(double globalFontSize, decimal givenFontScale, decimal modifiableFontScale, CellStatus status)
		=> globalFontSize * (double)(status == CellStatus.Modifiable ? modifiableFontScale : givenFontScale);

	public static double GetFontSizeSimple(double globalFontSize, decimal scale) => globalFontSize * (double)scale;

	public static Brush GetBrush(
		Color pencilmarkColor,
		Color deltaColor,
		CellStatus cellStatus,
		Grid solution,
		Cell cell,
		Mask candidatesMask,
		Digit digit,
		bool displayCandidates,
		bool useDifferentColorToDisplayDeltaDigits,
		CandidateMap usedCandidates
	)
	{
		var defaultBrush = new SolidColorBrush();
		return (displayCandidates, cellStatus) switch
		{
			(false, CellStatus.Empty)
				=> (!solution.IsUndefined && solution[cell] == digit, candidatesMask >> digit & 1, useDifferentColorToDisplayDeltaDigits) switch
				{
					(true, 0, true) when usedCandidates.Contains(cell * 9 + digit) => new SolidColorBrush(deltaColor),
					(_, not 0, _) when usedCandidates.Contains(cell * 9 + digit) => new SolidColorBrush(pencilmarkColor),
					_ => defaultBrush
				},
			(false, _)
				=> defaultBrush,
			(_, CellStatus.Given or CellStatus.Modifiable)
				=> defaultBrush,
			(_, CellStatus.Empty)
				=> (!solution.IsUndefined && solution[cell] == digit, candidatesMask >> digit & 1, useDifferentColorToDisplayDeltaDigits) switch
				{
					(true, 0, true) => new SolidColorBrush(deltaColor),
					(_, not 0, _) => new SolidColorBrush(pencilmarkColor),
					_ => defaultBrush
				},
			_
				=> throw new ArgumentOutOfRangeException(nameof(cellStatus))
		};
	}
}
