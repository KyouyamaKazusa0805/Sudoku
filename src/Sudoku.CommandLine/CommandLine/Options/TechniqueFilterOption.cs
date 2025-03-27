namespace Sudoku.CommandLine.Options;

/// <summary>
/// Provides a technique filter option.
/// </summary>
public sealed class TechniqueFilterOption : Option<Technique>, IOption<Technique>
{
	/// <summary>
	/// Initializes a <see cref="TechniqueFilterOption"/> instance.
	/// </summary>
	public TechniqueFilterOption() : base(
		["--technique", "-t"],
		"Specifies the technique. Use command 'print techniques' to view all techniques implemented in the program"
	)
	{
		Arity = ArgumentArity.ExactlyOne;
		IsRequired = false;
		SetDefaultValue(Technique.None);
	}


	/// <inheritdoc/>
	static Technique IOptionOrArgument<Technique>.ParseArgument(ArgumentResult result) => throw new NotImplementedException();
}
