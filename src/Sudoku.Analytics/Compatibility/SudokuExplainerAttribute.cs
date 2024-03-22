namespace Sudoku.Compatibility;

/// <summary>
/// Represents compatibility rules for Sudoku Explainer.
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class SudokuExplainerAttribute : ProgramMetadataAttribute<double, SudokuExplainerTechnique>
{
#pragma warning disable format
	/// <inheritdoc/>
	public override double Rating
	{
		get => RatingValueOriginal switch
		{
			[var p] => p,
			_ => throw new InvalidOperationException(ResourceDictionary.ExceptionMessage("SudokuExplainerAttributeInvalidRating"))
		};

		init => RatingValueOriginal = [value];
	}
#pragma warning restore format

	/// <summary>
	/// Indicates the defined technique enumeration field in Sudoku Explainer.
	/// </summary>
	public SudokuExplainerTechnique TechniqueDefined { get; init; }
}
