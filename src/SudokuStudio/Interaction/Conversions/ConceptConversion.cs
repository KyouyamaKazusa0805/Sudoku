namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about sudoku concepts.
/// </summary>
internal static class ConceptConversion
{
	public static string GetDigitString(Digit digit) => (digit + 1).ToString();

	public static Cell GetCell(Candidate candidate) => candidate / 9;

	public static RowIndex GetRowIndex(Candidate candidate) => GetCell(candidate) / 9;

	public static ColumnIndex GetColumnIndex(Candidate candidate) => GetCell(candidate) % 9;

	public static Digit GetDigit(Candidate candidate) => candidate % 9;
}
