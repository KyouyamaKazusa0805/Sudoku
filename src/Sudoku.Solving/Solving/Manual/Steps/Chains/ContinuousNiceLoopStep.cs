using Sudoku.Collections;
using Sudoku.Presentation;
using Sudoku.Solving.Manual.Text;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Continuous Nice Loop</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="XEnabled"><inheritdoc/></param>
/// <param name="YEnabled"><inheritdoc/></param>
/// <param name="Target">Indicates the target cell.</param>
public sealed record ContinuousNiceLoopStep(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	bool XEnabled,
	bool YEnabled,
	Node Target
) : ChainStep(Conclusions, Views, XEnabled, YEnabled, false, false, false, 0), IChainStep, ILoopStep
{
	/// <inheritdoc/>
	public override int FlatComplexity => Target.AncestorsCount;

	/// <inheritdoc/>
	public override decimal Difficulty =>
		(XEnabled && YEnabled ? 5.0M : 4.5M) + IChainLikeStep.GetExtraDifficultyByLength(FlatComplexity - 2);

	/// <inheritdoc/>
	public override Technique TechniqueCode =>
		IsXCycle ? Technique.FishyCycle : IsXyCycle ? Technique.XyCycle : Technique.ContinuousNiceLoop;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => TechniqueTags.LongChaining;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	/// <remarks>
	/// I separated them into three branches in that <see langword="switch"/> expression
	/// in order to extend the branches in the future.
	/// </remarks>
	public override Rarity Rarity =>
		TechniqueCode switch
		{
			Technique.FishyCycle => Rarity.Sometimes,
			Technique.XyCycle => Rarity.Sometimes,
			Technique.ContinuousNiceLoop => Rarity.Sometimes
		};

	/// <inheritdoc/>
	public override ChainTypeCode SortKey => Enum.Parse<ChainTypeCode>(TechniqueCode.ToString());

	/// <summary>
	/// Indicates whether the specified chain is an XY-Cycle.
	/// </summary>
	private bool IsXyCycle
	{
		get
		{
			if (Views[0].Links is { } links)
			{
				for (int i = 0, count = links.Count; i < count; i += 2)
				{
					var (link, _) = links[i];
					if (link.StartCandidate / 9 != link.EndCandidate / 9)
					{
						goto ReturnFalse;
					}
				}

				return true;
			}

		ReturnFalse:
			return false;
		}
	}

	/// <summary>
	/// Indicates whether the specified cycle is an X-Cycle.
	/// </summary>
	private bool IsXCycle => XEnabled && !YEnabled;

	[FormatItem]
	internal string ChainStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new LinkCollection(from pair in Views[0].Links! select pair.Link).ToString();
	}
}
