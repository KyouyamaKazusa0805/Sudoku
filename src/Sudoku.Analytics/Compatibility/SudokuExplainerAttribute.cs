namespace Sudoku.Compatibility;

/// <summary>
/// Represents compatibility rules for Sudoku Explainer.
/// </summary>
/// <param name="rating">Indicates the rating value.</param>
/// <param name="techniqueDefined">Indicates the defined technique enumeration field in Sudoku Explainer.</param>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed partial class SudokuExplainerAttribute(
	double rating = double.MinValue,
	[PrimaryConstructorParameter(SetterExpression = "set")] SudokuExplainerTechnique techniqueDefined = (SudokuExplainerTechnique)int.MinValue
) : ProgramMetadataAttribute<double, SudokuExplainerTechnique>(rating, techniqueDefined);
