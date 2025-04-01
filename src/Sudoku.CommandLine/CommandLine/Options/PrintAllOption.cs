namespace Sudoku.CommandLine.Options;

/// <summary>
/// Provides with a print-all option.
/// </summary>
internal sealed class PrintAllOption : Option<bool>, IOption<bool>
{
	/// <summary>
	/// Initializes a <see cref="PrintAllOption"/> instance.
	/// </summary>
	public PrintAllOption() : base(["--print-all", "-a"], "Specifies whether all satisfied elements (steps, etc.) will be printed")
	{
		Arity = ArgumentArity.ZeroOrOne;
		IsRequired = false;
		SetDefaultValue(false);
	}


	/// <inheritdoc/>
	static bool IMySymbol<bool>.ParseArgument(ArgumentResult result) => throw new NotImplementedException();
}
