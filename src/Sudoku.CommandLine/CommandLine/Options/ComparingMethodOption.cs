namespace Sudoku.CommandLine.Options;

/// <summary>
/// Represents a comparing method option.
/// </summary>
internal sealed class ComparingMethodOption : Option<BoardComparison>, IOption<BoardComparison>
{
	/// <summary>
	/// Initializes a <see cref="ComparingMethodOption"/> instance.
	/// </summary>
	public ComparingMethodOption() : base(["-m", "--method"], "Indicates the method to be compared")
	{
		Arity = ArgumentArity.ExactlyOne;
		IsRequired = false;
		SetDefaultValue(BoardComparison.Default);
	}


	/// <inheritdoc/>
	static BoardComparison IMySymbol<BoardComparison>.ParseArgument(ArgumentResult result)
		=> throw new NotImplementedException();
}
