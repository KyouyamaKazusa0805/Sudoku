namespace Sudoku.CommandLine.Options;

/// <summary>
/// Represents a clues count option.
/// </summary>
internal sealed class CluesCountOption : Option<int>, IOption<int>
{
	/// <summary>
	/// Initializes a <see cref="CluesCountOption"/> instance.
	/// </summary>
	public CluesCountOption() : base(["--clues-count", "-c"], "Specifies the number of clues")
	{
		Arity = ArgumentArity.ExactlyOne;
		IsRequired = false;
		SetDefaultValue(-1);
	}


	/// <inheritdoc/>
	static int IOptionOrArgument<int>.ParseArgument(ArgumentResult result) => throw new NotImplementedException();
}
