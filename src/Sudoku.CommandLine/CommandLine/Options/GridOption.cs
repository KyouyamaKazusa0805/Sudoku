namespace Sudoku.CommandLine.Options;

/// <summary>
/// Represents a grid option.
/// </summary>
public sealed class GridOption : Option<Grid>, IOption<Grid>
{
	/// <summary>
	/// Initializes a <see cref="GridOption"/> instance.
	/// </summary>
	public GridOption() : this(false)
	{
	}

	/// <summary>
	/// Initializes a <see cref="GridOption"/> instance.
	/// </summary>
	public GridOption(bool requiresInitialState) : base(
		["--grid", "-g"],
		result => ParseArgument(result, requiresInitialState),
		false,
		"Specifies the grid input"
	)
	{
		Arity = ArgumentArity.ExactlyOne;
		IsRequired = true;
	}


	/// <inheritdoc/>
	static Grid IOptionOrArgument<Grid>.ParseArgument(ArgumentResult result) => ParseArgument(result, default);

	/// <inheritdoc cref="IOptionOrArgument{T}.ParseArgument"/>
	private static Grid ParseArgument(ArgumentResult result, bool requiresInitialState)
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

		if (requiresInitialState)
		{
			var str = grid.ToString("#");
			if (str.Contains('+') || str.Contains(':'))
			{
				grid = Grid.Parse($"{grid:.}");
				//result.ErrorMessage = "The grid must be the initial state.";
				//return default;
			}
		}
		return grid;
	}
}
