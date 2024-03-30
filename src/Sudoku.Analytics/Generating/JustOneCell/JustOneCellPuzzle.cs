namespace Sudoku.Generating.JustOneCell;

/// <summary>
/// Represents the answer to a just-one-cell sudoku puzzle.
/// </summary>
/// <param name="cell">Indicates the target cell.</param>
/// <param name="digit">Indicates the target digit.</param>
/// <param name="step">
/// <para>Indiactes the step for the pattern.</para>
/// <para>
/// Assign a not-<see langword="null"/> value to this parameter
/// if argument <see cref="PuzzleBase.FailedReason"/> is <see cref="GeneratingFailedReason.None"/>.
/// Set the arguments of constructor <see cref="Step.Step(Conclusion[], View[], StepSearcherOptions)"/>
/// to be <c>[]</c>, <c>[]</c> and <c><see langword="new"/>()</c> respectively,
/// in order to avoid the potential bug on displaying details.
/// </para>
/// </param>
/// <param name="interferingCells">Indicates the interfering digits.</param>
/// <param name="interferingRatio">Indicates the interfering ratio.</param>
[GetHashCode]
public abstract partial class JustOneCellPuzzle(
	[PrimaryConstructorParameter] Cell cell,
	[PrimaryConstructorParameter] Digit digit,
	[PrimaryConstructorParameter, HashCodeMember] Step? step,
	[PrimaryConstructorParameter, HashCodeMember] scoped ref readonly CellMap interferingCells,
	[PrimaryConstructorParameter, HashCodeMember] double interferingRatio
) : PuzzleBase
{
	/// <inheritdoc/>
	[MemberNotNullWhen(true, nameof(Step))]
	public override bool Success => base.Success;


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] PuzzleBase? other)
		=> other is JustOneCellPuzzle comparer
		&& (FailedReason, Puzzle, Cell, Digit) == (comparer.FailedReason, comparer.Puzzle, comparer.Cell, comparer.Digit)
		&& (InterferingCells, InterferingRatio) == (comparer.InterferingCells, comparer.InterferingRatio)
		&& Step == comparer.Step;
}
