namespace Sudoku.CommandLine.Options;

/// <summary>
/// Represents a symmetric type option.
/// </summary>
internal sealed class SymmetricTypeOption : Option<SymmetricType>, IOption<SymmetricType>
{
	/// <summary>
	/// Initializes a <see cref="SymmetricTypeOption"/> instance.
	/// </summary>
	public SymmetricTypeOption() : base(["--symmetric-type", "-s"], "Specifies the symmetric type")
	{
		Arity = ArgumentArity.ExactlyOne;
		IsRequired = false;
		SetDefaultValue(SymmetricType.Central);
	}


	/// <inheritdoc/>
	static SymmetricType IMySymbol<SymmetricType>.ParseArgument(ArgumentResult result) => throw new NotImplementedException();
}
