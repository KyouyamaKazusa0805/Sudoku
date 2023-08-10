namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Oddagon</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="loopCells">Indicates the loop of cells used.</param>
/// <param name="digit1">Indicates the first digit used.</param>
/// <param name="digit2">Indicates the second digit used.</param>
public abstract partial class BivalueOddagonStep(
	Conclusion[] conclusions,
	View[]? views,
	[DataMember] scoped in CellMap loopCells,
	[DataMember] Digit digit1,
	[DataMember] Digit digit2
) : NegativeRankStep(conclusions, views), IEquatableStep<BivalueOddagonStep>
{
	/// <summary>
	/// Indicates the type of the technique.
	/// </summary>
	public abstract int Type { get; }

	/// <inheritdoc/>
	public override decimal BaseDifficulty => 6.3M;

	/// <inheritdoc/>
	public sealed override Technique Code => Enum.Parse<Technique>($"BivalueOddagonType{Type}");

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases => [new(ExtraDifficultyCaseNames.Size, (LoopCells.Count >> 1) * .1M)];

	private protected string LoopStr => LoopCells.ToString();


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEquatableStep<BivalueOddagonStep>.operator ==(BivalueOddagonStep left, BivalueOddagonStep right)
		=> (left.Type, left.Digit1, left.Digit2, left.LoopCells) == (right.Type, right.Digit1, right.Digit2, right.LoopCells);
}
