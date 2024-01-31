namespace Sudoku.Compatibility.SudokuExplainer;

/// <summary>
/// Represents an attribute type that describes the corresponding technique usage designed by Sudoku Explainer.
/// </summary>
/// <param name="technique">Indicates the technique.</param>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed partial class SudokuExplainerTechniqueAttribute([RecordParameter] SudokuExplainerTechnique technique) : Attribute
{
	/// <summary>
	/// Indicates whether the specified technique is defined by advanced version of Sudoku Explainer,
	/// which is not original program, or other implementations compatible with original Sudoku Explainer's
	/// rating system.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="false"/>.
	/// </remarks>
	public bool IsAdvancedDefined { get; init; }
}
