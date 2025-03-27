namespace Sudoku.CommandLine.Options;

/// <summary>
/// Represents a solving method option.
/// </summary>
internal sealed class SolvingMethodOption : Option<SolverType>, IOption<SolverType>
{
	/// <summary>
	/// Initializes a <see cref="SolvingMethodOption"/> instance.
	/// </summary>
	public SolvingMethodOption() : base(["--method", "-m"], "Specifies the solving method")
	{
		Arity = ArgumentArity.ExactlyOne;
		IsRequired = false;
		SetDefaultValueFactory(static () => new BitwiseSolver());
	}


	/// <inheritdoc/>
	static SolverType IOptionOrArgument<SolverType>.ParseArgument(ArgumentResult result) => ParseArgument(result);

	/// <inheritdoc cref="IOptionOrArgument{T}.ParseArgument"/>
	private static SolverType ParseArgument(ArgumentResult result) => throw new NotImplementedException();
}
