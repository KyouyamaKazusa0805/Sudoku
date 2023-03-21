namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Oddagon</b> technique.
/// </summary>
public abstract class BivalueOddagonStep(Conclusion[] conclusions, View[]? views, scoped in CellMap loopCells, int digit1, int digit2) :
	NegativeRankStep(conclusions, views),
	IEquatableStep<BivalueOddagonStep>
{
	/// <summary>
	/// Indicates the first digit used.
	/// </summary>
	public int Digit1 { get; } = digit1;

	/// <summary>
	/// Indicates the second digit used.
	/// </summary>
	public int Digit2 { get; } = digit2;

	/// <summary>
	/// Indicates the type of the technique.
	/// </summary>
	public abstract int Type { get; }

	/// <inheritdoc/>
	public override decimal BaseDifficulty => 6.3M;

	/// <inheritdoc/>
	public sealed override TechniqueGroup Group => TechniqueGroup.BivalueOddagon;

	/// <inheritdoc/>
	public sealed override Technique Code => Enum.Parse<Technique>($"BivalueOddagonType{Type}");

	/// <summary>
	/// Indicates the loop of cells used.
	/// </summary>
	public CellMap LoopCells { get; } = loopCells;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[] { new(ExtraDifficultyCaseNames.Size, (LoopCells.Count >> 1) * .1M) };

	private protected string LoopStr => LoopCells.ToString();


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(BivalueOddagonStep left, BivalueOddagonStep right)
		=> (left.Type, left.Digit1, left.Digit2, left.LoopCells) == (right.Type, right.Digit1, right.Digit2, right.LoopCells);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(BivalueOddagonStep left, BivalueOddagonStep right) => !(left == right);
}
