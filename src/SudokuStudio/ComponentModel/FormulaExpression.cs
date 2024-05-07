namespace SudokuStudio.ComponentModel;

/// <summary>
/// Represents a formula expression.
/// </summary>
public sealed class FormulaExpression
{
	/// <summary>
	/// Indicates the name of the formula.
	/// </summary>
	public string Name { get; init; } = "";

	/// <summary>
	/// Indicates the file ID.
	/// </summary>
	public string FileId { get; init; } = "";

	/// <summary>
	/// Indicates the description of the formula.
	/// </summary>
	public string Description { get; init; } = "";

	/// <summary>
	/// Indicates the expression of the formula.
	/// </summary>
	public string Expression { get; init; } = "";
}
