namespace SudokuStudio.Views.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about pencilmark text.
/// </summary>
internal static class PencilmarkTextConversion
{
	public static double GetFontSize(double globalFontSize, double scale) => globalFontSize * scale;

	public static Brush GetBrush(
		Color pencilmarkColor,
		Color deltaColor,
		CellStatus cellStatus,
		Grid solution,
		int cell,
		short candidatesMask,
		int digit
	) => cellStatus switch
	{
		CellStatus.Given or CellStatus.Modifiable => new SolidColorBrush(),
		CellStatus.Empty when solution[cell] == digit && (candidatesMask >> digit & 1) == 0 => new SolidColorBrush(deltaColor),
		CellStatus.Empty when (candidatesMask >> digit & 1) != 0 => new SolidColorBrush(pencilmarkColor),
		CellStatus.Empty => new SolidColorBrush(),
		_ => throw new ArgumentOutOfRangeException(nameof(cellStatus))
	};
}
