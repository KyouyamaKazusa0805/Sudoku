using Sudoku.Collections;
using Sudoku.Presentation;
using Sudoku.Solving.Manual.Text;
using Sudoku.Techniques;
using static System.Algorithm.Sequences;

namespace Sudoku.Solving.Manual.Steps;

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
	/// <inheritdoc/>
	public override string Name => $"{base.Name} + {Candidates.Count}";

	/// <inheritdoc/>
	public override decimal Difficulty => base.Difficulty + .1M + A002024(Candidates.Count) * .1M;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.BivalueUniversalGravePlusN;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	[FormatItem]
	internal string CandidatesStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Candidates(Candidates).ToString();
	}
}
