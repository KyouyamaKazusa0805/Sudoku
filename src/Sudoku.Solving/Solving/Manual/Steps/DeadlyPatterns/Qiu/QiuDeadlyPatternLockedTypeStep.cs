using Sudoku.Collections;
using Sudoku.Presentation;
using Sudoku.Solving.Collections;
using Sudoku.Solving.Manual.Text;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Steps;

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
	public override int Type => 5;

	[FormatItem]
	internal string CandidateStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Candidates(Candidates).ToString();
	}

	[FormatItem]
	internal string Quantifier
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Candidates.Count switch { 1 => string.Empty, 2 => " both", _ => " all" };
	}

	[FormatItem]
	internal string Number
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Candidates.Count == 1 ? " the" : $" {Candidates.Count}";
	}

	[FormatItem]
	internal string SingularOrPlural
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Candidates.Count == 1 ? "candidate" : "candidates";
	}

	[FormatItem]
	internal string BeVerb
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Candidates.Count == 1 ? "is" : "are";
	}
}
