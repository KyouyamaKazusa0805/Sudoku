namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about value text.
/// </summary>
internal static class ValueTextConversion
{
	public static string GetText(CellStatus cellStatus, Mask candidatesMask)
		=> cellStatus == CellStatus.Empty || candidatesMask is not (>= 0 and < 511)
			? string.Empty
			: (TrailingZeroCount((uint)candidatesMask) + 1).ToString();

	public static Brush GetValueFontColor(
		Grid grid,
		Grid solution,
		Cell cell,
		Mask candidatesMask,
		Color modifiableColor,
		Color givenColor,
		Color deltaColor,
		bool useDifferentColorToDisplayDeltaDigits
	)
	{
		var digit = TrailingZeroCount(candidatesMask) is var d and not InvalidTrailingZeroCountMethodFallback ? d : -1;

		// Implicit behavior: argument 'solution' can be 'Grid.Undefined'.
		// Therefore, we must check validity first.
		return new SolidColorBrush(
			!solution.IsUndefined && solution.GetDigit(cell) != digit && useDifferentColorToDisplayDeltaDigits
				? deltaColor
				: grid.GetStatus(cell) == CellStatus.Modifiable ? modifiableColor : givenColor
		);
	}
}
