namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Universal Grave + n</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Candidates">Indicates the true candidates.</param>
public sealed record BivalueUniversalGraveMultipleStep(
	ConclusionList Conclusions,
	ViewList Views,
	IReadOnlyList<int> Candidates
) : BivalueUniversalGraveStep(Conclusions, Views), IStepWithPhasedDifficulty
{
	/// <inheritdoc/>
	public override string Name => $"{base.Name} + {Candidates.Count}";

	/// <inheritdoc/>
	public override decimal Difficulty => ((IStepWithPhasedDifficulty)this).TotalDifficulty;

	/// <inheritdoc/>
	public decimal BaseDifficulty => base.Difficulty;

	/// <inheritdoc/>
	public (string Name, decimal Value)[] ExtraDifficultyValues
		=> new[] { ("Offset", .1M), ("Size", A002024(Candidates.Count) * .1M) };

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
