namespace Sudoku.Concepts;

/// <summary>
/// Represents a chain node.
/// </summary>
/// <param name="map">Indicates the backing map.</param>
/// <param name="isOn">Indicates whether the node is on.</param>
/// <param name="isAdvanced">
/// <para>Indicates whether the node is advanced one.</para>
/// <para><inheritdoc cref="Node" path="/shared-comments"/></para>
/// </param>
/// <param name="parent">
/// <para>Indicates the parent node. The value can be <see langword="null"/> in handling.</para>
/// <para><inheritdoc cref="Node" path="/shared-comments"/></para>
/// </param>
/// <shared-comments>
/// Please note that this value doesn't participate in equality comparison.
/// </shared-comments>
[TypeImpl(TypeImplFlag.AllObjectMethods | TypeImplFlag.AllOperators)]
public sealed partial class Node(
	[PrimaryConstructorParameter(MemberKinds.Field), HashCodeMember] ref readonly CandidateMap map,
	[PrimaryConstructorParameter, HashCodeMember] bool isOn,
	[PrimaryConstructorParameter] bool isAdvanced,
	[PrimaryConstructorParameter(SetterExpression = "set")] Node? parent = null
) :
	IComparable<Node>,
	IComparisonOperators<Node, Node, bool>,
	IEquatable<Node>,
	IEqualityOperators<Node, Node, bool>,
	IFormattable
{
	/// <summary>
	/// Indicates the map format string.
	/// </summary>
	private const string MapFormatString = "m";

	/// <summary>
	/// Indicates the property <see cref="IsOn"/> format string.
	/// </summary>
	private const string IsOnFormatString = "s";


	/// <summary>
	/// Indicates whether the node is a grouped node.
	/// </summary>
	public bool IsGroupedNode => _map.Count >= 2;

	/// <summary>
	/// Indicates the length of ancestors.
	/// </summary>
	public int AncestorsLength
	{
		get
		{
			var result = 0;
			for (var node = this; node is not null; node = node.Parent)
			{
				result++;
			}
			return result;
		}
	}

	/// <summary>
	/// Indicates the map of candidates the node uses.
	/// </summary>
	public ref readonly CandidateMap Map => ref _map;

	/// <summary>
	/// The backing comparing value on <see cref="IsOn"/> property.
	/// </summary>
	/// <seealso cref="IsOn"/>
	private int IsOnPropertyValue => IsOn ? 1 : 0;


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out bool isGroupedNode, out CandidateMap map) => (isGroupedNode, map) = (IsGroupedNode, _map);

	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out bool isGroupedNode, out CandidateMap map, out Node? parent)
		=> ((isGroupedNode, map), parent) = (this, Parent);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals([NotNullWhen(true)] Node? other) => Equals(other, NodeComparison.IncludeIsOn);

	/// <summary>
	/// Compares with two <see cref="Node"/> instances, based on the specified comparison rule.
	/// </summary>
	/// <param name="other">The other instance to be compared.</param>
	/// <param name="comparison">The comparison rule.</param>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="comparison"/> is not defined.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals([NotNullWhen(true)] Node? other, NodeComparison comparison)
		=> other is not null
		&& comparison switch
		{
			NodeComparison.IncludeIsOn => _map == other._map && IsOn == other.IsOn,
			NodeComparison.IgnoreIsOn => _map == other._map,
			_ => throw new ArgumentOutOfRangeException(nameof(comparison))
		};

	/// <summary>
	/// Determines whether the current node is an ancestor of the specified node. 
	/// </summary>
	/// <param name="childNode">The node to be checked.</param>
	/// <param name="nodeComparison">The comparison rule on nodes.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public bool IsAncestorOf(Node childNode, NodeComparison nodeComparison)
	{
		for (var node = childNode; node is not null; node = node.Parent)
		{
			if (Equals(node, nodeComparison))
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Calculates the hash code on the current instance.
	/// </summary>
	/// <param name="comparison">The comparison rule.</param>
	/// <returns>An <see cref="int"/> value as the result.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="comparison"/> is not defined.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetHashCode(NodeComparison comparison)
		=> comparison switch
		{
			NodeComparison.IncludeIsOn => HashCode.Combine(_map.GetHashCode(), IsOn),
			NodeComparison.IgnoreIsOn => HashCode.Combine(_map.GetHashCode()),
			_ => throw new ArgumentOutOfRangeException(nameof(comparison))
		};

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(Node? other) => CompareTo(other, NodeComparison.IgnoreIsOn);

	/// <inheritdoc cref="CompareTo(Node?)"/>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="comparison"/> is not defined.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(Node? other, NodeComparison comparison)
		=> other is null
			? -1
			: comparison switch
			{
				NodeComparison.IncludeIsOn => IsOnPropertyValue.CompareTo(other.IsOnPropertyValue) switch
				{
					var r and not 0 => r,
					_ => _map.CompareTo(in other._map)
				},
				NodeComparison.IgnoreIsOn => _map.CompareTo(in other._map),
				_ => throw new ArgumentOutOfRangeException(nameof(comparison))
			};

	/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(IFormatProvider? formatProvider)
	{
		var converter = CoordinateConverter.GetConverter(formatProvider);
		return $"{converter.CandidateConverter(_map)}: {IsOn}";
	}

	/// <inheritdoc/>
	/// <remarks>
	/// Format description:
	/// <list type="table">
	/// <listheader>
	/// <term>Format character</term>
	/// <description>Description</description>
	/// </listheader>
	/// <item>
	/// <term><c>m</c></term>
	/// <description>The map text. For example, <c>r1c23(4)</c></description>
	/// </item>
	/// <item>
	/// <term><c>S</c> and <c>s</c></term>
	/// <description>
	/// The <see cref="IsOn"/> property value (<see langword="true"/> or <see langword="false"/>).
	/// If the character <c>s</c> is upper-cased, the result text will be upper-cased on initial letter.
	/// </description>
	/// </item>
	/// </list>
	/// For example, format value <c>"m: S"</c> will be replaced with value <c>"r1c23(4): True"</c>.
	/// </remarks>
	public string ToString(string? format, IFormatProvider? formatProvider)
		=> (format ?? $"{MapFormatString}: {IsOnFormatString}")
			.Replace(MapFormatString, _map.ToString(formatProvider))
			.Replace(IsOnFormatString, IsOn.ToString().ToLower());

	/// <summary>
	/// Creates a copy of the current instance.
	/// </summary>
	/// <returns>A cloned instance whose internal values are same as the current instance, independent.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Node Clone() => new(in _map, IsOn, IsAdvanced) { Parent = Parent };


	/// <summary>
	/// Negates the node with <see cref="IsOn"/> property value.
	/// </summary>
	/// <param name="value">The current node.</param>
	/// <returns>The node negated.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Node operator ~(Node value) => new(in value._map, !value.IsOn, value.IsAdvanced) { Parent = value.Parent };

	/// <summary>
	/// Creates a <see cref="Node"/> instance with parent node.
	/// </summary>
	/// <param name="current">The current node.</param>
	/// <param name="parent">The parent node.</param>
	/// <returns>The new node created.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Node operator >>(Node current, Node? parent) => new(in current._map, current.IsOn, current.IsAdvanced, parent);
}
