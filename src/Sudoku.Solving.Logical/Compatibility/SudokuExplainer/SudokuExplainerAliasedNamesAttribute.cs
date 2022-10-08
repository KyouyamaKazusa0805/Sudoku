namespace Sudoku.Compatibility.SudokuExplainer;

/// <summary>
/// Defines an attribute that is applied to a field of type <see cref="Technique"/>,
/// indicating the aliased name (or names) of specified technique that is defined by Sudoku Explainer.
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
internal sealed class SudokuExplainerAliasedNamesAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="SudokuExplainerAliasedNamesAttribute"/> instance
	/// via the specified aliases of specified technique.
	/// </summary>
	/// <param name="aliases">The aliased names of specified technique.</param>
	public SudokuExplainerAliasedNamesAttribute(params string[] aliases) => Aliases = aliases;


	/// <summary>
	/// Indicates the aliased names of the technique.
	/// </summary>
	public string[] Aliases { get; }
}
