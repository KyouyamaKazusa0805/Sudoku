namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Aligned Exclusion</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="cells">Indicates the target cells used in the pattern.</param>
/// <param name="lockedCombinations">Indicates all locked combinations.</param>
public sealed partial class AlignedExclusionStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryConstructorParameter] scoped ref readonly CellMap cells,
	[PrimaryConstructorParameter] (Digit[], Cell)[] lockedCombinations
) : PermutationStep(conclusions, views, options), ISizeTrait
{
	/// <inheritdoc/>
	public override int BaseDifficulty
		=> Size switch
		{
			2 => 62,
			3 => 75,
			4 => 81,
			5 => 84,
			_ => throw new NotSupportedException(ResourceDictionary.ExceptionMessage("SubsetSizeExceeds"))
		};

	/// <inheritdoc/>
	public int Size => Cells.Count;

	/// <inheritdoc/>
	public override Technique Code
		=> Size switch
		{
			>= 2 and <= 5 => Technique.AlignedPairExclusion + (short)(Size - 2),
			_ => throw new NotSupportedException(ResourceDictionary.ExceptionMessage("SubsetSizeExceeds"))
		};

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(ChineseLanguage, [CellsStr, ConclusionNegatedStr]), new(EnglishLanguage, [CellsStr, ConclusionNegatedStr])];

	private string CellsStr => Options.Converter.CellConverter(Cells);

	private string ConclusionNegatedStr => Options.Converter.ConclusionConverter(from c in Conclusions select ~c);


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Step? other) => other is AlignedExclusionStep comparer && Cells == comparer.Cells;
}
