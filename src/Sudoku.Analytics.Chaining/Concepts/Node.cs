namespace Sudoku.Concepts;

/// <summary>
/// Represents a chain node.
/// </summary>
/// <param name="map">Indicates the backing map.</param>
[TypeImpl(TypeImplFlag.AllObjectMethods | TypeImplFlag.AllOperators)]
public sealed partial class Node([PrimaryConstructorParameter(MemberKinds.Field), HashCodeMember] ref readonly CandidateMap map) :
	IComparable<Node>,
	IComparisonOperators<Node, Node, bool>,
	ICoordinateConvertible<Node>,
	IEquatable<Node>,
	IEqualityOperators<Node, Node, bool>,
	IFormattable
{
	/// <summary>
	/// Initializes a <see cref="Node"/> instance via the specified candidate.
	/// </summary>
	/// <param name="candidate">A candidate.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Node(Candidate candidate) : this(candidate.AsCandidateMap())
	{
	}

	/// <summary>
	/// Initializes a <see cref="Node"/> instance via the specified <see cref="LockedTarget"/> instance.
	/// </summary>
	/// <param name="lockedTarget">A <see cref="LockedTarget"/> instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Node(ref readonly LockedTarget lockedTarget) : this(Subview.ExpandedCellFromDigit(lockedTarget.Cells, lockedTarget.Digit))
	{
	}


	/// <summary>
	/// Indicates whether the node is a grouped node.
	/// </summary>
	public bool IsGroupedNode => _map.Count >= 2;

	/// <summary>
	/// Indicates the map of candidates the node uses.
	/// </summary>
	public ref readonly CandidateMap Map => ref _map;


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals([NotNullWhen(true)] Node? other) => other is not null && _map == other._map;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(Node? other) => other is null ? 1 : _map.CompareTo(in other._map);

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
