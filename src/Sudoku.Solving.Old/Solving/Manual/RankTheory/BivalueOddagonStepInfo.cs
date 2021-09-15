namespace Sudoku.Solving.Manual.RankTheory;

/// <summary>
/// Provides a usage of <b>bi-value oddagon</b> technique.
/// </summary>
/// <param name="Conclusions">All conclusions.</param>
/// <param name="Views">All views.</param>
/// <param name="Loop">The loop used.</param>
/// <param name="Digit1">The digit 1.</param>
/// <param name="Digit2">The digit 2.</param>
[AutoHashCode(nameof(Code), nameof(Loop), nameof(Digit1), nameof(Digit2))]
public abstract partial record class BivalueOddagonStepInfo(
	IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
	in Cells Loop, int Digit1, int Digit2
) : RankTheoryStepInfo(Conclusions, Views)
{
	/// <inheritdoc/>
	public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.BivalueOddagon;

	/// <summary>
	/// Indicates the code.
	/// </summary>
	private Technique Code => TechniqueCode;


	/// <inheritdoc/>
	public virtual bool Equals(BivalueOddagonStepInfo? other) =>
		other is not null && Loop == other.Loop && Digit1 == other.Digit1 && Digit2 == other.Digit2;
}
