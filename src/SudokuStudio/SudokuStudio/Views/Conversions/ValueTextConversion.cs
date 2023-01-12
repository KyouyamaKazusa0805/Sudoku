namespace SudokuStudio.Views.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about value text.
/// </summary>
internal static class ValueTextConversion
{
	public static string GetText(CellStatus cellStatus, short candidatesMask)
		=> cellStatus == CellStatus.Empty || candidatesMask is not (>= 0 and < 511)
			? string.Empty
			: (TrailingZeroCount((uint)candidatesMask) + 1).ToString();

	public static Brush GetValueFontColor(
		Grid grid,
		Grid solution,
		int cell,
		short candidatesMask,
		Color modifiableColor,
		Color givenColor,
		Color deltaColor
	) => new SolidColorBrush(
		solution[cell] != (TrailingZeroCount(candidatesMask) is var digit and not InvalidValidOfTrailingZeroCountMethodFallback ? digit : -1)
			? deltaColor
			: grid.GetStatus(cell) == CellStatus.Modifiable ? modifiableColor : givenColor
	);
}
