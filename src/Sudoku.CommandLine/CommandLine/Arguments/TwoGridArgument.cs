namespace Sudoku.CommandLine.Arguments;

/// <summary>
/// Represents two grid argument.
/// </summary>
internal sealed class TwoGridArgument : Argument<(Grid Left, Grid Right)>, IArgument<(Grid Left, Grid Right)>
{
	/// <summary>
	/// Initializes a <see cref="TwoGridArgument"/> instance.
	/// </summary>
	public TwoGridArgument() : base(null, ParseArgument, false, "Two grids to be compared") => Arity = new(2, 2);


	/// <inheritdoc/>
	public static (Grid Left, Grid Right) ParseArgument(ArgumentResult result)
	{
		if (result.Tokens is not [{ Value: var l }, { Value: var r }])
		{
			result.ErrorMessage = "Unexpected tokens - You must pass 2 values to be parsed.";
			return default;
		}

		if (!Grid.TryParse(l, out var left) || !Grid.TryParse(r, out var right))
		{
			result.ErrorMessage = "Two string values must be parseable, casting into grid.";
			return default;
		}

		return (left, right);
	}
}
