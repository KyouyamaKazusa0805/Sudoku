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
		Color deltaColor,
		bool useDifferentColorToDisplayDeltaDigits
	)
	{
		var digit = TrailingZeroCount(candidatesMask) is var d and not InvalidValidOfTrailingZeroCountMethodFallback ? d : -1;
		return new SolidColorBrush(
			/// Implicit behavior: argument <param name="solution"/> can be <see cref="Grid.Undefined"/>.
			/// Therefore, we must check validity first.
			!solution.IsUndefined && solution[cell] != digit && useDifferentColorToDisplayDeltaDigits
				? deltaColor
				: grid.GetStatus(cell) == CellStatus.Modifiable ? modifiableColor : givenColor
		);
	}
}
