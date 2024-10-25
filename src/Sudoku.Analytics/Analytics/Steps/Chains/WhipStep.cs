namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Whip</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="truths">Indicates all truths.</param>
/// <param name="links">Indicates all links.</param>
public sealed partial class WhipStep(
	StepConclusions conclusions,
	View[]? views,
	StepGathererOptions options,
	[Property] ReadOnlyMemory<Space> truths,
	[Property] ReadOnlyMemory<Space> links
) : ChainStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override bool IsMultiple => false;

	/// <inheritdoc/>
	public override bool IsDynamic => true;

	/// <inheritdoc/>
	public override int Complexity => Views![0].OfType<CandidateViewNode>().Length; // A tricky way to check number of nodes used.

	/// <inheritdoc/>
	public override int BaseDifficulty => 80;

	/// <inheritdoc/>
	public override Technique Code => Technique.Whip;

	/// <inheritdoc/>
	public override Mask DigitsUsed
		=> (Mask)(
			MaskOperations.Create((from t in Truths select t.Digit).Span)
				| MaskOperations.Create((from l in Links select l.Digit).Span)
		);

	/// <inheritdoc/>
	public override FactorArray Factors
		=> [
			Factor.Create(
				"Factor_WhipComplexityFactor",
				[nameof(Complexity)],
				GetType(),
				static args => ChainingLength.GetLengthDifficulty((int)args[0]!)
			)
		];

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [new(SR.EnglishLanguage, [TruthsStr, LinksStr]), new(SR.ChineseLanguage, [TruthsStr, LinksStr])];

	private string TruthsStr => string.Join(' ', from t in Truths.Span select t.ToString());

	private string LinksStr => string.Join(' ', from l in Links.Span select l.ToString());


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Step? other)
		=> other is WhipStep comparer && Conclusions.Span[0] == comparer.Conclusions.Span[0];

	/// <inheritdoc/>
	public override int CompareTo(Step? other)
		=> other is WhipStep comparer
			? Conclusions.Span[0].CompareTo(comparer.Conclusions.Span[0]) is var conclusionComparisonResult and not 0
				? conclusionComparisonResult
				: 0
			: -1;
}
