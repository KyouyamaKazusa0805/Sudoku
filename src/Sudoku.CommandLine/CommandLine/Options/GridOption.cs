namespace Sudoku.CommandLine.Options;

/// <summary>
/// Represents a grid option.
/// </summary>
public sealed class GridOption : Option<Grid>, IOption<Grid>
{
	/// <summary>
	/// Initializes a <see cref="GridOption"/> instance.
	/// </summary>
	public GridOption() : base(["--grid", "-g"], ParseArgument, false, "Specifies the grid input")
	{
		Arity = ArgumentArity.ExactlyOne;
		IsRequired = true;
	}


	/// <inheritdoc/>
	static Grid IOption<Grid>.ParseArgument(ArgumentResult result) => ParseArgument(result);

	/// <inheritdoc cref="IOption{T}.ParseArgument"/>
	private static Grid ParseArgument(ArgumentResult result)
	{
		var token = result.Tokens is [{ Value: var f }] ? f : null;
		if (token is null)
		{
			result.ErrorMessage = "Grid expected.";
			return default;
		}

		if (!Grid.TryParse(token, out var grid))
		{
			result.ErrorMessage = "Invalid grid format.";
			return default;
		}

		return grid;
	}
}
