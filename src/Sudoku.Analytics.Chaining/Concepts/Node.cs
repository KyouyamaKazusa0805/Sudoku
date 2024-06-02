namespace Sudoku.Concepts;

/// <summary>
/// Represents a chain node.
/// </summary>
/// <param name="map">Indicates the backing map.</param>
[StructLayout(LayoutKind.Auto)]
[TypeImpl(TypeImplFlag.AllObjectMethods | TypeImplFlag.AllOperators, IsLargeStructure = true)]
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


	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(ref readonly Node other) => Type == other.Type && _map == other._map;

	/// <inheritdoc cref="IComparable{T}.CompareTo(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(ref readonly Node other) => Type > other.Type ? 1 : Type < other.Type ? -1 : _map.CompareTo(in other._map);

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
	bool IEquatable<Node>.Equals(Node other) => Equals(in other);

	/// <inheritdoc/>
	int IComparable<Node>.CompareTo(Node other) => CompareTo(in other);

	/// <inheritdoc/>
	string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString(formatProvider);
}
