namespace Sudoku.CommandLine.Options;

/// <summary>
/// Represents an output information option.
/// </summary>
public sealed class OutputInfoOption : Option<bool>, IOption<bool>
{
	/// <summary>
	/// Initializes an <see cref="OutputInfoOption"/> instance.
	/// </summary>
	public OutputInfoOption() : base(
		["--output-info", "-i"],
		"Specifies whether the output text will also print filtered information"
	)
	{
		Arity = ArgumentArity.ZeroOrOne;
		IsRequired = false;
	}


	/// <inheritdoc/>
	static bool IOptionOrArgument<bool>.ParseArgument(ArgumentResult result) => throw new NotImplementedException();
}
