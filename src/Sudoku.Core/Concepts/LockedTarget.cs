namespace Sudoku.Concepts;

/// <summary>
/// Defines the data structure that stores a set of cells and a digit, indicating the information
/// about the locked candidate node.
/// </summary>
/// <param name="digit">Indicates the digit used.</param>
/// <param name="cells">Indicates the cells used.</param>
/// <remarks>
/// <include file="../../global-doc-comments.xml" path="/g/large-structure"/>
/// </remarks>
[StructLayout(LayoutKind.Auto)]
[LargeStructure]
[Equals]
[GetHashCode]
[ToString]
[EqualityOperators]
[method: JsonConstructor]
public readonly partial struct LockedTarget(
	[PrimaryConstructorParameter, HashCodeMember] Digit digit,
	[PrimaryConstructorParameter, HashCodeMember, StringMember] CellMap cells
) :
	IEquatable<LockedTarget>,
	IEqualityOperators<LockedTarget, LockedTarget, bool>
{
	/// <summary>
	/// Initializes a <see cref="LockedTarget"/> instance via the specified cell and the specified digit used.
	/// </summary>
	/// <param name="digit">Indicates the digit used.</param>
	/// <param name="cell">Indicates the cell used.</param>
	public LockedTarget(Digit digit, Cell cell) : this(digit, [cell])
	{
	}


	/// <summary>
	/// Indicates whether the number of cells is 1.
	/// </summary>
	[JsonIgnore]
	public bool IsSole => Cells.Count == 1;

	/// <summary>
	/// The digit string value.
	/// </summary>
	[StringMember(nameof(Digit))]
	private string DigitString => (Digit + 1).ToString();


	[DeconstructionMethod]
	public partial void Deconstruct(out CellMap cells, out Digit digit);

	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(scoped in LockedTarget other) => Digit == other.Digit && Cells == other.Cells;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool IEquatable<LockedTarget>.Equals(LockedTarget other) => Equals(other);
}
