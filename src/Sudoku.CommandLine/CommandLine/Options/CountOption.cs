namespace Sudoku.CommandLine.Options;

/// <summary>
/// Provides with a count option.
/// </summary>
internal sealed class CountOption : Option<int>, IOption<int>
{
	/// <summary>
	/// Initializes a <see cref="CountOption"/> instance.
	/// </summary>
	public CountOption() : base(["--count", "-C"], "Specifies the count of the generated puzzle.")
	{
		Arity = ArgumentArity.ExactlyOne;
		IsRequired = false;
		SetDefaultValue(1);
	}


	/// <inheritdoc/>
	static int IMySymbol<int>.ParseArgument(ArgumentResult result) => throw new NotImplementedException();
}
