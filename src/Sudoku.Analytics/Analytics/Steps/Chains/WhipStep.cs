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
	[Property] ICollection truths,
	[Property] ICollection links
) : ChainStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override bool IsMultiple => false;

	/// <inheritdoc/>
	public override bool IsDynamic => true;

	/// <inheritdoc/>
	public override int Complexity => throw new NotImplementedException("Will be implemented later.");

	/// <inheritdoc/>
	public override int BaseDifficulty => 80;

	/// <summary>
	/// Indicates the rank of the pattern.
	/// </summary>
	public int Rank => Links.Count - Truths.Count;

	/// <inheritdoc/>
	public override Technique Code => Technique.Whip;

	/// <inheritdoc/>
	public override Mask DigitsUsed => throw new NotImplementedException("Will be implemented later.");

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

	private string TruthsStr => throw new NotImplementedException("Will be implemented later.");

	private string LinksStr => throw new NotImplementedException("Will be implemented later.");


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Step? other)
	{
		if (other is not WhipStep comparer)
		{
			return false;
		}

		if (Rank != comparer.Rank)
		{
			return false;
		}

		if (Conclusions.Span[0] != comparer.Conclusions.Span[0])
		{
			return false;
		}

		// TODO: Compares with truths and links.
		throw new NotImplementedException("Will be implemented later.");
	}

	/// <inheritdoc/>
	public override int CompareTo(Step? other)
		=> other is WhipStep comparer
			? Rank.CompareTo(comparer.Rank) is var rankComparisonResult and not 0
				? rankComparisonResult
				: Conclusions.Span[0].CompareTo(comparer.Conclusions.Span[0]) is var conclusionComparisonResult and not 0
					? conclusionComparisonResult
					// TODO: Compares with truths and links.
					: throw new NotImplementedException("Will be implemented later.")
			: -1;

	/// <inheritdoc/>
	public override string GetName(IFormatProvider? formatProvider) => $"{base.GetName(formatProvider)}[{Truths.Count}]";
}
