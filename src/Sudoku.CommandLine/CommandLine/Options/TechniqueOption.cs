namespace Sudoku.CommandLine.Options;

/// <summary>
/// Provides a technique option.
/// </summary>
internal sealed class TechniqueOption : Option<Technique>, IOption<Technique>
{
	/// <summary>
	/// Initializes a <see cref="TechniqueOption"/> instance.
	/// </summary>
	/// <param name="isRequired">Specifies whether the option is required.</param>
	public TechniqueOption(bool isRequired) : base(
		["--technique", "-t"],
		"Specifies the technique. Use command 'print techniques' to view all techniques implemented in the program"
	)
	{
		Arity = ArgumentArity.ExactlyOne;
		IsRequired = isRequired;
		if (!isRequired)
		{
			SetDefaultValue(Technique.None);
		}
	}


	/// <inheritdoc/>
	static Technique IOptionOrArgument<Technique>.ParseArgument(ArgumentResult result) => throw new NotImplementedException();
}
