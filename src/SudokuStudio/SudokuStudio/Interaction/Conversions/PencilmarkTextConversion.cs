namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about pencilmark text.
/// </summary>
internal static class PencilmarkTextConversion
{
	public static double GetFontSize(double globalFontSize, double givenFontScale, double modifiableFontScale, CellStatus status)
		=> globalFontSize * (status == CellStatus.Modifiable ? modifiableFontScale : givenFontScale);

	public static double GetFontSizeSimple(double globalFontSize, double scale) => globalFontSize * scale;

	public static Brush GetBrush(
		Color pencilmarkColor,
		Color deltaColor,
		CellStatus cellStatus,
		Grid solution,
		int cell,
		short candidatesMask,
		int digit,
		bool displayCandidates,
		bool useDifferentColorToDisplayDeltaDigits
	)
	{
		var defaultBrush = new SolidColorBrush();
		return (displayCandidates, cellStatus) switch
		{
			(false, _) => defaultBrush,
			(_, CellStatus.Given or CellStatus.Modifiable) => defaultBrush,
			(_, CellStatus.Empty) => (!solution.IsUndefined && solution[cell] == digit, candidatesMask >> digit & 1, useDifferentColorToDisplayDeltaDigits) switch
			{
				(true, 0, true) => new SolidColorBrush(deltaColor),
				(_, not 0, _) => new SolidColorBrush(pencilmarkColor),
				_ => defaultBrush
			},
			_ => throw new ArgumentOutOfRangeException(nameof(cellStatus))
		};
	}
}
