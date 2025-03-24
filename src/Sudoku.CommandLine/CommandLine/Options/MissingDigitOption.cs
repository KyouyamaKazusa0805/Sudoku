namespace Sudoku.CommandLine.Options;

/// <summary>
/// Provides a missing digit option.
/// </summary>
public sealed class MissingDigitOption : Option<int>, IOption<int>
{
	/// <summary>
	/// Initializes a <see cref="MissingDigitOption"/> instance.
	/// </summary>
	public MissingDigitOption() : base(
		["--missing-digit", "-d"],
		"Specifies the missing digit, which won't be appeared in target puzzle"
	)
	{
		Arity = ArgumentArity.ExactlyOne;
		IsRequired = false;
		SetDefaultValue(-1);
	}


	/// <inheritdoc/>
	static int IOptionOrArgument<int>.ParseArgument(ArgumentResult result) => throw new NotImplementedException();
}
