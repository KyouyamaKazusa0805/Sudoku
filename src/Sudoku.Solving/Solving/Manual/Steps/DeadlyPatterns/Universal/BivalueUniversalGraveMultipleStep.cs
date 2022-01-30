using Sudoku.Collections;
using Sudoku.Data;
using Sudoku.Presentation;
using Sudoku.Solving.Manual.Text;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Steps.DeadlyPatterns.Universal;

/// <summary>
/// Provides with a step that is a <b>Bivalue Universal Grave + n</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Candidates">Indicates the true candidates.</param>
public sealed record BivalueUniversalGraveMultipleStep(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	IReadOnlyList<int> Candidates
) : BivalueUniversalGraveStep(Conclusions, Views)
{
	/// <summary>
	/// The table of extra difficulty values.
	/// </summary>
	private static readonly decimal[] ExtraDifficulty =
	{
		.1M, .2M, .2M, .3M, .3M, .3M, .4M, .4M, .4M, .4M,
		.5M, .5M, .5M, .5M, .5M, .6M, .6M, .6M
	};


	/// <inheritdoc/>
	public override string Name => $"{base.Name} + {Candidates.Count}";

	/// <inheritdoc/>
	public override decimal Difficulty => base.Difficulty + .1M + ExtraDifficulty[Candidates.Count - 1];

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.BivalueUniversalGravePlusN;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	[FormatItem]
	private string CandidatesStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Candidates(Candidates).ToString();
	}
}
