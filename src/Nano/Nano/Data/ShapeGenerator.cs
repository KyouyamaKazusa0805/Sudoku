namespace Nano.Data;

/// <summary>
/// Provides with a set of methods that creates a <see cref="Shape"/> instance
/// via the specified information of <see cref="SudokuGrid"/>.
/// </summary>
/// <seealso cref="Shape"/>
/// <seealso cref="SudokuGrid"/>
public static class ShapeGenerator
{
	/// <summary>
	/// Creates a <see cref="TextBlock"/> control that displays for a cell.
	/// </summary>
	/// <param name="cell">Indicates the cell value. The value must be between 0 and 80.</param>
	/// <param name="preference">
	/// The preference to define the settings.
	/// If <see langword="null"/>, default settings are enabled and applied.
	/// </param>
	/// <param name="textSizeFactor">The default text size. The default value is 60.</param>
	/// <returns>The <see cref="TextBlock"/> result.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="cell"/> is not between 0 and 80,
	/// or the argument <paramref name="textSizeFactor"/> is below 0.
	/// </exception>
	public static TextBlock CreateCellText(int cell, Preference? preference = null, int textSizeFactor = 60) =>
		cell is not (>= 0 and < 81)
			? throw new ArgumentOutOfRangeException(nameof(cell))
			: (cell / 9, cell % 9) is var (row, column)
				? new TextBlock
				{
					Visibility = Visibility.Collapsed,
					Foreground = new SolidColorBrush(preference?.GivenColor ?? Preference.GivenColor_Default),
					FontSize = (double)(textSizeFactor * preference?.ValueScale ?? Preference.ValueScale_Default),
					FontFamily = new(preference?.GivenFontName ?? Preference.GivenFontName_Default),
					FontStyle = preference?.GivenFontStyle ?? Preference.GivenFontStyle_Default,
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					HorizontalTextAlignment = TextAlignment.Center
				}
				.WithGridRow(row * 3)
				.WithGridRowSpan(3)
				.WithGridColumn(column * 3)
				.WithGridColumnSpan(3)
				: throw null;

	/// <summary>
	/// Creates a <see cref="TextBlock"/> control that displays for a candidate.
	/// </summary>
	/// <param name="cell">Indicates the cell value. The value must be between 0 and 80.</param>
	/// <param name="digit">Indicates the digit value. The value must be between 0 and 8.</param>
	/// <param name="preference">
	/// The preference to define the settings.
	/// If <see langword="null"/>, default settings are enabled and applied.
	/// </param>
	/// <param name="textSizeFactor">The default text size. The default value is 60.</param>
	/// <returns>The <see cref="TextBlock"/> result.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="cell"/> is not between 0 and 80,
	/// or the argument <paramref name="digit"/> is not between 0 and 8,
	/// or the argument <paramref name="textSizeFactor"/> is below 0.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TextBlock CreateCandidateText(
		int cell, int digit, Preference? preference = null, int textSizeFactor = 60) =>
		textSizeFactor < 0
			? throw new ArgumentOutOfRangeException(nameof(textSizeFactor))
			: cell is not (>= 0 and < 81)
				? throw new ArgumentOutOfRangeException(nameof(cell))
				: digit is not (>= 0 and < 9)
					? throw new ArgumentOutOfRangeException(nameof(digit))
					: (cell / 9, cell % 9) is var (row, column)
						? new TextBlock
						{
							Visibility = Visibility.Visible,
							Foreground = new SolidColorBrush(preference?.CandidateColor ?? Preference.CandidateColor_Default),
							FontSize = (double)(textSizeFactor * (preference?.CandidateScale ?? Preference.CandidateScale_Default)),
							FontFamily = new(preference?.CandidateFontName ?? Preference.CandidateFontName_Default),
							FontStyle = preference?.CandidateFontStyle ?? Preference.CandidateFontStyle_Default,
							HorizontalAlignment = HorizontalAlignment.Center,
							VerticalAlignment = VerticalAlignment.Center,
							HorizontalTextAlignment = TextAlignment.Center
						}
						.WithGridRow(row * 3 + digit / 3)
						.WithGridColumn(column * 3 + digit % 3)
						: throw null;

	/// <summary>
	/// Creates a <see cref="TextBlock"/> control that displays for a candidate.
	/// </summary>
	/// <param name="candidate">The candidate value. The value must be betweeen 0 and 728.</param>
	/// <param name="preference">
	/// The preference to define the settings.
	/// If <see langword="null"/>, default settings are enabled and applied.
	/// </param>
	/// <param name="textSizeFactor">The default text size. The default value is 60.</param>
	/// <returns>The <see cref="TextBlock"/> result.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="candidate"/> is not between 0 and 728,
	/// or the argument <paramref name="textSizeFactor"/> is below 0.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TextBlock CreateCandidateText(
		int candidate, Preference? preference = null, int textSizeFactor = 60) =>
		CreateCandidateText(candidate / 9, candidate % 9, preference, textSizeFactor);
}
