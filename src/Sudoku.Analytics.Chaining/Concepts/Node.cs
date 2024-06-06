namespace Sudoku.Concepts;

/// <summary>
/// Represents a chain node.
/// </summary>
/// <param name="map">Indicates the backing map.</param>
/// <param name="isOn">Indicates whether the node is on.</param>
[TypeImpl(TypeImplFlag.AllObjectMethods | TypeImplFlag.AllOperators)]
public sealed partial class Node(
	[PrimaryConstructorParameter(MemberKinds.Field), HashCodeMember] ref readonly CandidateMap map,
	[PrimaryConstructorParameter, HashCodeMember] bool isOn
) :
	IComparable<Node>,
	IComparisonOperators<Node, Node, bool>,
	ICoordinateConvertible<Node>,
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
	/// Indicates the extra cells map.
	/// </summary>
	private readonly CandidateMap _extraMap;


	/// <summary>
	/// Initializes a <see cref="Node"/> instance via the specified candidate.
	/// </summary>
	/// <param name="candidate">A candidate.</param>
	/// <param name="isOn">Indicates whether the node is on.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Node(Candidate candidate, bool isOn) : this(candidate.AsCandidateMap(), isOn)
	{
	}

	/// <summary>
	/// Initializes a <see cref="Node"/> instance via the specified candidate, and extra map.
	/// </summary>
	/// <param name="candidate">A candidate.</param>
	/// <param name="isOn">Indicates whether the node is on.</param>
	/// <param name="extraMap">The extra map.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Node(Candidate candidate, bool isOn, ref readonly CandidateMap extraMap) : this(candidate, isOn) => _extraMap = extraMap;

	/// <summary>
	/// Initializes a <see cref="Node"/> instance via two maps.
	/// </summary>
	/// <param name="map">The basic pattern map of candidates used.</param>
	/// <param name="isOn">Indicates whether the node is on.</param>
	/// <param name="extraMap">The extra map.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Node(ref readonly CandidateMap map, bool isOn, ref readonly CandidateMap extraMap) : this(in map, isOn)
		 => _extraMap = extraMap;

	/// <summary>
	/// Initializes a <see cref="Node"/> instance via the specified cell and digit.
	/// </summary>
	/// <param name="cell">A cell.</param>
	/// <param name="digit">A digit.</param>
	/// <param name="isOn">Indicates whether the node is on.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Node(Cell cell, Digit digit, bool isOn) : this(cell * 9 + digit, isOn)
	{
	}

	/// <summary>
	/// Initializes a <see cref="Node"/> instance via the specified candidate, and extra map.
	/// </summary>
	/// <param name="cell">A cell.</param>
	/// <param name="digit">A digit.</param>
	/// <param name="isOn">Indicates whether the node is on.</param>
	/// <param name="extraMap">The extra map.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Node(Cell cell, Digit digit, bool isOn, ref readonly CandidateMap extraMap) : this(cell, digit, isOn)
		=> _extraMap = extraMap;

	/// <summary>
	/// Copies and creates a <see cref="Node"/> instance from argument <paramref name="base"/>,
	/// and appends its parent node.
	/// </summary>
	/// <param name="base">The data provider.</param>
	/// <param name="parent">The parent node.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Node(Node @base, Node? parent) : this(in @base._map, @base.IsOn) => Parent = parent;

	/// <summary>
	/// Copies and creates a <see cref="Node"/> instance from argument <paramref name="base"/>,
	/// and appends its parent node, and modify <see cref="IsOn"/> property value.
	/// </summary>
	/// <param name="base">The data provider.</param>
	/// <param name="isOn">Indicates whether the node is on.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Node(Node @base, bool isOn) : this(in @base._map, isOn) => Parent = @base.Parent;


	/// <summary>
	/// Indicates whether the node is a grouped node.
	/// </summary>
	public bool IsGroupedNode => _map.Count >= 2;

	/// <summary>
	/// Indicates the map of candidates the node uses.
	/// </summary>
	public ref readonly CandidateMap Map => ref _map;

	/// <summary>
	/// Indicates the extra cells map to hold. This is used by representing extra candidates used inside the pattern.
	/// </summary>
	/// <remarks>
	/// Please note that this property won't be used as comparison,
	/// neither <see cref="Equals(Node?)"/> or <see cref="CompareTo(Node?)"/>.
	/// </remarks>
	/// <seealso cref="Equals(Node?)"/>
	/// <seealso cref="CompareTo(Node?)"/>
	public ref readonly CandidateMap ExtraMap => ref _extraMap;

	/// <summary>
	/// Indicates the length of ancestors.
	/// </summary>
	internal int AncestorsLength
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
	/// Indicates the parent node. The value doesn't participate in equality comparison.
	/// </summary>
	internal Node? Parent { get; init; }

	/// <summary>
	/// The backing comparing value on <see cref="IsOn"/> property.
	/// </summary>
	/// <seealso cref="IsOn"/>
	private int IsOnPropertyValue => IsOn ? 1 : 0;


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
	public int CompareTo(Node? other) => CompareTo(other, NodeComparison.IncludeIsOn);

	/// <inheritdoc cref="CompareTo(Node?)"/>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="comparison"/> is not defined.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(Node? other, NodeComparison comparison)
		=> other is null
			? 1
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

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString<T>(T converter) where T : CoordinateConverter => $"{converter.CandidateConverter(_map)}: {IsOn}";

	/// <summary>
	/// Returns a string that represents the current object.
	/// </summary>
	/// <typeparam name="T">The type of the converter.</typeparam>
	/// <param name="format">The format.</param>
	/// <param name="converter">The converter.s</param>
	/// <returns>A string that represents the current object.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString<T>(string? format, T converter) where T : CoordinateConverter
		=> (format ?? $"{MapFormatString}: {IsOnFormatString}")
			.Replace(MapFormatString, _map.ToString(converter))
			.Replace(IsOnFormatString, IsOn.ToString().ToLower());

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
	public Node Clone() => new(in _map, IsOn) { Parent = Parent };


	/// <summary>
	/// Negates the node with <see cref="IsOn"/> property value.
	/// </summary>
	/// <param name="value">The current node.</param>
	/// <returns>The node negated.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Node operator ~(Node value) => new(value, !value.IsOn);
}
