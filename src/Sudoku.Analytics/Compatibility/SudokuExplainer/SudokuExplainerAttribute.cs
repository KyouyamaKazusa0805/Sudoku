namespace Sudoku.Compatibility.SudokuExplainer;

/// <summary>
/// Represents compatibility rules for Sudoku Explainer.
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class SudokuExplainerAttribute : ProgramMetadataAttribute
{
	/// <summary>
	/// Indicates the rating value that is defined in original program.
	/// </summary>
	/// <remarks>
	/// The value of this property is an array of 1 or 2 elements.
	/// If two, the first one is the minimal possible rating value of the technique;
	/// and the second one is the maximum possible rating value.
	/// </remarks>
	[DisallowNull]
	public double[]? RatingOriginal { get; init; }

	/// <summary>
	/// Indicates the rating value that is defined in advanced concept.
	/// </summary>
	/// <remarks><inheritdoc cref="RatingOriginal" path="/remarks"/></remarks>
	[DisallowNull]
	public double[]? RatingAdvanced { get; init; }

	/// <summary>
	/// Indicates the defined technique enumeration field in Sudoku Explainer.
	/// </summary>
	public SudokuExplainerTechnique Technique { get; init; }
}
