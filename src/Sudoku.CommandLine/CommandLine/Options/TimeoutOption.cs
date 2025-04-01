namespace Sudoku.CommandLine.Options;

/// <summary>
/// Provides a timeout option.
/// </summary>
internal sealed class TimeoutOption : Option<int>, IOption<int>
{
	/// <summary>
	/// Initializes a <see cref="TimeoutOption"/> instance.
	/// </summary>
	public TimeoutOption() : base(["--timeout", "-T"], "Specifies the timeout, in milliseconds")
	{
		Arity = ArgumentArity.ExactlyOne;
		IsRequired = false;
		SetDefaultValue(-1);
	}


	/// <inheritdoc/>
	static int IMySymbol<int>.ParseArgument(ArgumentResult result) => throw new NotImplementedException();
}
