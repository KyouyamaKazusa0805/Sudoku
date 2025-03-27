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
	static BoardComparison IOptionOrArgument<BoardComparison>.ParseArgument(ArgumentResult result) => ParseArgument(result);

	/// <inheritdoc cref="IOptionOrArgument{T}.ParseArgument"/>
	private static BoardComparison ParseArgument(ArgumentResult result) => throw new NotImplementedException();
}
