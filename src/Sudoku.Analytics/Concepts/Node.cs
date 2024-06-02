namespace Sudoku.Concepts;

/// <summary>
/// Represents a chain node.
/// </summary>
/// <param name="map">Indicates the backing map.</param>
[StructLayout(LayoutKind.Auto)]
[TypeImpl(TypeImplFlag.AllObjectMethods | TypeImplFlag.AllOperators)]
public readonly partial struct Node([PrimaryConstructorParameter(MemberKinds.Field), HashCodeMember] CandidateMap map) :
	IComparable<Node>,
	IComparisonOperators<Node, Node, bool>,
	ICoordinateConvertible<Node>,
	IEquatable<Node>,
	IEqualityOperators<Node, Node, bool>,
	IFormattable
{
	/// <summary>
	/// Indicates whether the node is a grouped node.
	/// </summary>
	public bool IsGroupedNode => Type != NodeType.SingleCandidate;

	/// <summary>
	/// Indicates the node type.
	/// </summary>
	[HashCodeMember]
	public NodeType Type { get; }

	/// <summary>
	/// Indicates the map of candidates the node uses.
	/// </summary>
	[UnscopedRef]
	public ref readonly CandidateMap Map => ref _map;


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Node other) => Type == other.Type && _map == other._map;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(Node other) => Type > other.Type ? 1 : Type < other.Type ? -1 : _map.CompareTo(in other._map);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString<T>(T converter) where T : CoordinateConverter => converter.CandidateConverter(_map);

	/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(IFormatProvider? formatProvider)
		=> ToString(
			formatProvider switch
			{
				CultureInfo c => CoordinateConverter.GetConverter(c),
				_ => CoordinateConverter.InvariantCultureConverter
			}
		);

	/// <inheritdoc/>
	string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString(formatProvider);
}
