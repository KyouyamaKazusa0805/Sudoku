namespace Sudoku.Solving.Manual.Uniqueness.Reversal;

/// <summary>
/// Provides a usage of <b>reverse bi-value universal grave</b> technique.
/// </summary>
/// <param name="Conclusions">All conclusions.</param>
/// <param name="Views">All views.</param>
/// <param name="Loop">All cells used.</param>
/// <param name="Digit1">Indicates the digit 1.</param>
/// <param name="Digit2">Indicates the digit 2.</param>
[AutoHashCode(nameof(Loop), nameof(Digit1), nameof(Digit2), nameof(TechniqueCode))]
public abstract partial record ReverseBugStepInfo(
	IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, in Cells Loop, int Digit1, int Digit2
) : UniquenessStepInfo(Conclusions, Views)
{
	/// <inheritdoc/>
	public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.ReverseBug;

	/// <summary>
	/// Indicates the digits string.
	/// </summary>
	[FormatItem]
	protected string DigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(stackalloc[] { Digit1, Digit2 }).ToString();
	}


	/// <inheritdoc/>
	public virtual bool Equals(ReverseBugStepInfo? other) =>
		other is not null && Loop == other.Loop
		&& Digit1 == other.Digit1 && Digit2 == other.Digit2 && TechniqueCode == other.TechniqueCode;
}
