namespace Sudoku.Compatibility;

/// <summary>
/// Represents compatibility rules for Sudoku Explainer.
/// </summary>
/// <param name="ratingValue">Indicates the rating value.</param>
/// <param name="techniqueDefined">Indicates the defined technique enumeration field in Sudoku Explainer.</param>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed partial class SudokuExplainerAttribute(
	[PrimaryConstructorParameter(Accessibility = "public override")] double ratingValue = int.MinValue,
	[PrimaryConstructorParameter(Accessibility = "public override", GeneratedMemberName = "DifficultyLevel")] SudokuExplainerTechnique techniqueDefined = (SudokuExplainerTechnique)int.MinValue
) : ProgramMetadataAttribute<double, SudokuExplainerTechnique>;
