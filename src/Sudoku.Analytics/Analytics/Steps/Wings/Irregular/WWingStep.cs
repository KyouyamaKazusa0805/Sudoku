namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>W-Wing</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="startCell">Indicates the start cell.</param>
/// <param name="endCell">Indicates the end cell.</param>
/// <param name="conjugatePair">Indicates the conjugate pair connecting with cells <see cref="StartCell"/> and <see cref="EndCell"/>.</param>
public sealed partial class WWingStep(
	Conclusion[] conclusions,
	View[]? views,
	[PrimaryConstructorParameter] Cell startCell,
	[PrimaryConstructorParameter] Cell endCell,
	[PrimaryConstructorParameter] scoped in Conjugate conjugatePair
) : IrregularWingStep(conclusions, views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 4.4M;

	/// <inheritdoc/>
	public override Technique Code => Technique.WWing;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { StartCellStr, EndCellStr, ConjStr } },
			{ "zh", new[] { StartCellStr, EndCellStr, ConjStr } }
		};

	private string StartCellStr => RxCyNotation.ToCellString(StartCell);

	private string EndCellStr => RxCyNotation.ToCellString(EndCell);

	private string ConjStr => ConjugatePair.ToString();
}
