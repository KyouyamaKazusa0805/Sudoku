namespace Sudoku.Cli.Options;

/// <summary>
/// Represents a symmetric type option.
/// </summary>
public sealed class SymmetricTypeOption : IOption<SymmetricTypeOption, SymmetricType>
{
	/// <inheritdoc/>
	public static string Description => "Indicates the symmetric type to be set.";

	/// <inheritdoc/>
	public static string[] Aliases => ["--symmetric-type", "-s"];

	/// <inheritdoc/>
	public static SymmetricType DefaultValue => SymmetricType.Central;
}
