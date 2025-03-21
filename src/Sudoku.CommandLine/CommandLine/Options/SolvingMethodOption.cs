namespace Sudoku.CommandLine.Options;

/// <summary>
/// Represents a solving method option.
/// </summary>
public sealed class SolvingMethodOption : Option<ISolver>, IOption<ISolver>
{
	/// <summary>
	/// Indicates the backing method map.
	/// </summary>
	internal static FrozenDictionary<string, Func<ISolver>> MethodMap = null!;


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
		if (!MethodMap.TryGetValue(token, out var solverCreator))
		{
			result.ErrorMessage = $"Invalid token. The expected values are {names}.";
			return null!;
		}

		return solverCreator();
	}
}
