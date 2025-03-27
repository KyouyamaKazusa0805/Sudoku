namespace Sudoku.CommandLine.Arguments;

/// <summary>
/// Represents a cell map argument.
/// </summary>
internal sealed class CellMapArgument : Argument<CellMap>, IArgument<CellMap>
{
	/// <summary>
	/// Initializes a <see cref="CellMapArgument"/> instance.
	/// </summary>
	public CellMapArgument(bool allowsParsingGrid) : base(
		null,
		result => ParseArgument(result, allowsParsingGrid),
		false,
		"Specifies the cells used, using 1 to describe the cell is used, and 0 as placeholders"
	) => Arity = ArgumentArity.ExactlyOne;


	/// <inheritdoc/>
	static CellMap IOptionOrArgument<CellMap>.ParseArgument(ArgumentResult result) => ParseArgument(result, default);

	/// <inheritdoc cref="IOptionOrArgument{T}.ParseArgument"/>
	private static CellMap ParseArgument(ArgumentResult result, bool allowsParsingGrid)
	{
		if (result.Tokens is not [{ Value: var token }])
		{
			result.ErrorMessage = "Argument expected.";
			return default;
		}

		if (allowsParsingGrid && Grid.TryParse(token, out var resultGrid))
		{
			return resultGrid.GivenCells;
		}

		if (!CellMap.TryParse(token, new BitmapCellMapFormatInfo(), out var resultMap))
		{
			result.ErrorMessage = "The specified string cannot be parsed as value cell list. The string must contains 81 characters 0 or 1.";
			return default;
		}

		return resultMap;
	}
}
