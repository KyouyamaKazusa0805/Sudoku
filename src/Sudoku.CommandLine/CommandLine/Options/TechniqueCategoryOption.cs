namespace Sudoku.CommandLine.Options;

/// <summary>
/// Provides a technique category option.
/// </summary>
public sealed class TechniqueCategoryOption : Option<TechniqueGroup>, IOption<TechniqueGroup>
{
	/// <summary>
	/// Initializes a <see cref="TechniqueCategoryOption"/> instance.
	/// </summary>
	public TechniqueCategoryOption() : base(["--category", "-c"], "Specifies the category of the techniques to print")
	{
		Arity = ArgumentArity.ExactlyOne;
		IsRequired = false;
		SetDefaultValue(TechniqueGroup.None);
	}


	/// <inheritdoc/>
	public static TechniqueGroup ParseArgument(ArgumentResult result) => throw new NotImplementedException();
}
