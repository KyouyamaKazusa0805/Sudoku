namespace Sudoku.CommandLine.Options;

/// <summary>
/// Provides with a count option.
/// </summary>
public sealed class CountOption : Option<int>, IOption<int>
{
	/// <summary>
	/// Initializes a <see cref="CountOption"/> instance.
	/// </summary>
	public CountOption() : base(["--count", "-C"], "Speciifes the count of the generated puzzle.")
	{
		Arity = ArgumentArity.ExactlyOne;
		IsRequired = false;
		SetDefaultValue(1);
	}


	/// <inheritdoc/>
	static int IOptionOrArgument<int>.ParseArgument(ArgumentResult result) => throw new NotImplementedException();
}
