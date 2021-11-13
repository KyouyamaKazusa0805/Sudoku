namespace Sudoku.Solving.Manual.Steps.DeadlyPatterns.Qiu;

/// <summary>
/// Provides with a step that is a <b>Qiu's Deadly Pattern Locked Type</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Pattern"><inheritdoc/></param>
/// <param name="Candidates">Indicates the candidates used.</param>
public sealed record QiuDeadlyPatternLockedTypeStep(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	in QiuDeadlyPattern Pattern,
	IReadOnlyList<int> Candidates
) : QiuDeadlyPatternStep(Conclusions, Views, Pattern)
{
	/// <inheritdoc/>
	public override decimal Difficulty => base.Difficulty + .2M;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.LockedQdp;

	[FormatItem]
	private string CandidateStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Candidates(Candidates).ToString();
	}

	[FormatItem]
	private string Quantifier
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Candidates.Count switch { 1 => string.Empty, 2 => " both", _ => " all" };
	}

	[FormatItem]
	private string Number
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Candidates.Count == 1 ? " the" : $" {Candidates.Count}";
	}

	[FormatItem]
	private string SingularOrPlural
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Candidates.Count == 1 ? "candidate" : "candidates";
	}

	[FormatItem]
	private string BeVerb
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Candidates.Count == 1 ? "is" : "are";
	}
}
