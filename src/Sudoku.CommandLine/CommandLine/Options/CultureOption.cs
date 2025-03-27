namespace Sudoku.CommandLine.Options;

/// <summary>
/// Represents a culture option.
/// </summary>
internal sealed class CultureOption : Option<string>, IOption<string>
{
	/// <summary>
	/// Initializes a <see cref="CultureOption"/> instance.
	/// </summary>
	public CultureOption() : base(["--culture", "-C"], "Specifies the culture like 'zh-CN', 'en-US'")
	{
		Arity = ArgumentArity.ExactlyOne;
		IsRequired = false;
		SetDefaultValue(null);
	}


	/// <inheritdoc/>
	public static string ParseArgument(ArgumentResult result) => throw new NotImplementedException();
}
