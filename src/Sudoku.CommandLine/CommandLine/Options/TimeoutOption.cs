namespace Sudoku.CommandLine.Options;

/// <summary>
/// Provides a timeout option.
/// </summary>
public sealed class TimeoutOption : Option<int>, IOption<int>
{
	/// <summary>
	/// Initializes a <see cref="TimeoutOption"/> instance.
	/// </summary>
	public TimeoutOption() : base(["--timeout", "-t"], "Specifies the timeout, in milliseconds")
	{
		Arity = ArgumentArity.ExactlyOne;
		IsRequired = false;
		SetDefaultValue(-1);
	}


	/// <inheritdoc/>
	static int IOptionOrArgument<int>.ParseArgument(ArgumentResult result) => throw new NotImplementedException();
}
