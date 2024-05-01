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
[LargeStructure]
[Equals]
[GetHashCode]
[EqualityOperators]
[StructLayout(LayoutKind.Auto)]
[method: JsonConstructor]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly partial struct LockedTarget(
	[PrimaryConstructorParameter, HashCodeMember] Digit digit,
	[PrimaryConstructorParameter, HashCodeMember, StringMember] CellMap cells
) :
	ICultureFormattable,
	IEquatable<LockedTarget>,
	IEqualityOperators<LockedTarget, LockedTarget, bool>,
	ISudokuConceptConvertible<LockedTarget>
{
	/// <summary>
	/// Indicates whether the number of cells is 1.
	/// </summary>
	[JsonIgnore]
	public bool IsSole => Cells.Count == 1;

	/// <summary>
	/// The digit string value.
	/// </summary>
	[StringMember(nameof(Digit))]
	private string DigitString => GlobalizedConverter.InvariantCultureConverter.DigitConverter((Mask)(1 << Digit));


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out CellMap cells, out Digit digit) => (cells, digit) = (Cells, Digit);

	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(ref readonly LockedTarget other) => Digit == other.Digit && Cells == other.Cells;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => ToString(GlobalizedConverter.InvariantCultureConverter);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(CultureInfo? culture = null) => ToString(GlobalizedConverter.GetConverter(culture ?? CultureInfo.CurrentUICulture));

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString<T>(T converter) where T : CoordinateConverter => converter.CandidateConverter(Subview.ExpandedCellFromDigit(Cells, Digit));

	/// <inheritdoc/>
	bool IEquatable<LockedTarget>.Equals(LockedTarget other) => Equals(in other);
}
