namespace Sudoku.CommandLine.Arguments;

/// <summary>
/// Represents grid argument.
/// </summary>
internal sealed class GridArgument : Argument<Grid>, IArgument<Grid>
{
	/// <summary>
	/// Initializes a <see cref="GridArgument"/> instance.
	/// </summary>
	public GridArgument() : base(null, ParseArgument, false, "Specifies the grid") => Arity = ArgumentArity.ExactlyOne;


	/// <inheritdoc/>
	public static Grid ParseArgument(ArgumentResult result)
	{
		if (result.Tokens is not [{ Value: var gridString }])
		{
			result.ErrorMessage = "Unexpected tokens - You must specify a token.";
			return default;
		}

		if (!Grid.TryParse(gridString, out var grid))
		{
			result.ErrorMessage = "String value must be parseable, casting into grid.";
			return default;
		}

		return grid;
	}
}
