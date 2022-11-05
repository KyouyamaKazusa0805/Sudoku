namespace Sudoku.Text.Formatting;

/// <summary>
/// Indicates the factory that creates the cell map formatter.
/// </summary>
internal static class CellMapFormatterFactory
{
	/// <summary>
	/// Get a built-in <see cref="ICellMapFormatter"/> instance according to the specified format.
	/// </summary>
	/// <param name="format">The format.</param>
	/// <returns>The grid formatter.</returns>
	/// <exception cref="FormatException">Throws when the format string is invalid.</exception>
	public static ICellMapFormatter? GetBuiltInFormatter(string? format)
		=> format switch
		{
#pragma warning disable format
			null or "N" or "n"			=> RxCyFormat.Default,
			"K" or "k"					=> K9Format.Default,
			"T" or "t"					=> CellMapTableFormat.Default,
			"B" or "b" or "B+" or "b+"	=> CellMapBinaryFormat.Default,
			"B-" or "b-"				=> CellMapBinaryFormat.Default with { WithSeparator = false },
			_							=> null
#pragma warning restore format
		};
}
