namespace Sudoku.CommandLine.Options;

/// <summary>
/// Represents a solving method option.
/// </summary>
public sealed class SolvingMethodOption : Option<ISolver>, IOption<ISolver>
{
	/// <summary>
	/// Indicates the backing method map.
	/// </summary>
	private static readonly Dictionary<string, Func<ISolver>> MethodMap = new(StringComparer.OrdinalIgnoreCase)
	{
		{ "bitwise", static () => new BitwiseSolver() },
		{ "backtracking-bfs", static () => new BacktrackingSolver { UseBreadthFirstSearch = true } },
		{ "backtracking-dfs", static () => new BacktrackingSolver() },
		{ "backtracking", static () => new BacktrackingSolver() },
		{ "dancing-links", static () => new DancingLinksSolver() },
		{ "dlx", static () => new DancingLinksSolver() },
		{ "enumerable-query", static () => new EnumerableQuerySolver() },
		{ "dictionary-query", static () => new DictionaryQuerySolver() }
	};


	/// <summary>
	/// Initializes a <see cref="SolvingMethodOption"/> instance.
	/// </summary>
	public SolvingMethodOption() : base(["--method", "-m"], ParseArgument, false, "Specifies the solving method")
	{
		Arity = ArgumentArity.ExactlyOne;
		IsRequired = false;
		SetDefaultValueFactory(static () => new BitwiseSolver());
	}


	/// <inheritdoc/>
	static ISolver IOption<ISolver>.ParseArgument(ArgumentResult result) => ParseArgument(result);

	/// <inheritdoc cref="IOption{T}.ParseArgument"/>
	private static ISolver ParseArgument(ArgumentResult result)
	{
		var token = result.Tokens is [{ Value: var f }] ? f : null;
		if (token is null)
		{
			result.ErrorMessage = "Argument expected.";
			return null!;
		}

		var names = string.Join(", ", MethodMap.Keys);
		if (!MethodMap.TryGetValue(token, out var solverCreation))
		{
			result.ErrorMessage = $"Invalid token. The expected values are {names}.";
			return null!;
		}

		return solverCreation();
	}
}
