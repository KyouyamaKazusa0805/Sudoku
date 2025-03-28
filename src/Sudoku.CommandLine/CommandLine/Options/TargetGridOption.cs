namespace Sudoku.CommandLine.Options;

/// <summary>
/// Provides with output target grid option.
/// </summary>
internal sealed class OutputTargetGridOption : Option<bool>, IOption<bool>
{
	/// <summary>
	/// Initializes an <see cref="OutputTargetGridOption"/> instance.
	/// </summary>
	public OutputTargetGridOption() : base(
		["--target-grid", "-g"],
		"Specifies whether the output grid will be the target grid satisfying the filters, rather than the original grid"
	)
	{
		Arity = ArgumentArity.ZeroOrOne;
		IsRequired = false;
		SetDefaultValue(false);
	}


	/// <inheritdoc/>
	static bool IOptionOrArgument<bool>.ParseArgument(ArgumentResult result) => throw new NotImplementedException();
}
