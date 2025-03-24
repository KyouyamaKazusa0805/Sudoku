namespace Sudoku.CommandLine.Options;

/// <summary>
/// Provides with <see cref="CellMap"/> option.
/// </summary>
public sealed class CellMapOption : Option<CellMap>, IOption<CellMap>
{
	/// <summary>
	/// Initializes a <see cref="CellMapOption"/> instance.
	/// </summary>
	/// <param name="allowsParsingGrid">Indicates whether the option allows for parsing grid.</param>
	public CellMapOption(bool allowsParsingGrid) : base(
		["--cells", "-c"],
		result => ParseArgument(result, allowsParsingGrid),
		false,
		"Specifies the cells used, using 1 to describe the cell is used, and 0 as placeholders"
	)
	{
		Arity = ArgumentArity.ExactlyOne;
		IsRequired = true;
	}


	/// <inheritdoc/>
	static CellMap IOptionOrArgument<CellMap>.ParseArgument(ArgumentResult result) => ParseArgument(result, false);

	/// <inheritdoc/>
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
