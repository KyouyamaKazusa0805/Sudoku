namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle</b> technique.
/// </summary>
public abstract class UniqueRectangleStep(
	Conclusion[] conclusions,
	View[]? views,
	Technique code,
	int digit1,
	int digit2,
	scoped in CellMap cells,
	bool isAvoidable,
	int absoluteOffset
) : DeadlyPatternStep(conclusions, views), IEquatableStep<UniqueRectangleStep>
{
	/// <summary>
	/// Indicates whether the current rectangle is an avoidable rectangle.
	/// If <see langword="true"/>, an avoidable rectangle; otherwise, a unique rectangle.
	/// </summary>
	public bool IsAvoidable { get; } = isAvoidable;

	/// <inheritdoc/>
	public override decimal BaseDifficulty => 4.5M;

	/// <summary>
	/// Indicates the first digit used.
	/// </summary>
	public int Digit1 { get; } = digit1;

	/// <summary>
	/// Indicates the second digit used. This value is always greater than <see cref="Digit1"/>.
	/// </summary>
	/// <seealso cref="Digit1"/>
	public int Digit2 { get; } = digit2;

	/// <summary>
	/// Indicates the absolute offset.
	/// </summary>
	/// <remarks>
	/// The value will be an <see cref="int"/> value to compare all possible cases
	/// of unique rectangle structures to be iterated. The greater the value is,
	/// the later the unique rectangle structure will be processed. The value must be between 0 and 485.
	/// Other values are invalid and useless. The number of all possible unique rectangle structures is 486.
	/// </remarks>
	public int AbsoluteOffset { get; } = absoluteOffset;

	/// <inheritdoc/>
	public sealed override Technique Code => code;

	/// <summary>
	/// Indicates the cells used in this pattern.
	/// </summary>
	public CellMap Cells { get; } = cells;

	private protected string D1Str => (Digit1 + 1).ToString();

	private protected string D2Str => (Digit2 + 1).ToString();

	private protected string CellsStr => Cells.ToString();


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(UniqueRectangleStep left, UniqueRectangleStep right)
		=> (left.Code, left.AbsoluteOffset, left.Digit1, left.Digit2) == (right.Code, right.AbsoluteOffset, right.Digit1, right.Digit2);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(UniqueRectangleStep left, UniqueRectangleStep right) => !(left == right);
}
