namespace Sudoku.Concepts.Solving.ChainNodes;

/// <summary>
/// Defines a chain node.
/// </summary>
public abstract class Node :
	IEquatable<Node>
#if FEATURE_GENERIC_MATH
	,
	IEqualityOperators<Node, Node>
#endif
{
	/// <summary>
	/// Indicates the number of bits that is preserved by the bit mask field <see cref="_other"/>.
	/// </summary>
	/// <seealso cref="_other"/>
	protected const int PreservedBitsCount = 7;


	/// <summary>
	/// Indicates the bits used. <see cref="_higher"/> and <see cref="_lower"/>
	/// store the basic data of the cells used.
	/// </summary>
	/// <remarks>
	/// Please note that the lower 7 bits in the field <see cref="_other"/> are reserved ones,
	/// which represents the basic data for the digit used, and the type of the node.
	/// You cannot modify them.
	/// </remarks>
	protected readonly long _higher, _lower, _other;


	/// <summary>
	/// Initializes a <see cref="Node"/> instance via the basic data.
	/// </summary>
	/// <param name="nodeType">The node type.</param>
	/// <param name="digit">The digit used.</param>
	/// <param name="cells">The cells used.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected Node(NodeType nodeType, byte digit, in Cells cells)
	{
		var vector = cells.ToVector();
		_higher = vector.GetElement(0);
		_lower = vector.GetElement(1);
		_other = (int)nodeType << 4 | digit;
	}

	/// <summary>
	/// Initializes a <see cref="Node"/> instance via the basic data.
	/// </summary>
	/// <param name="nodeType">The node type.</param>
	/// <param name="digit">The digit used.</param>
	/// <param name="cells">The cells used.</param>
	/// <param name="otherMask">
	/// The other mask. Here the mask provided must preserve the lower 7 bits zeroed
	/// because they are reserved one.
	/// </param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected Node(NodeType nodeType, byte digit, in Cells cells, long otherMask) :
		this(nodeType, digit, cells) => _other |= otherMask;

	/// <summary>
	/// Initializes a <see cref="Node"/> instance via the basic data.
	/// </summary>
	/// <param name="higher">The higher 64 bits.</param>
	/// <param name="lower">The lower 64 bits.</param>
	/// <param name="other">The other 64 bits.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected Node(long higher, long lower, long other)
	{
		_higher = higher;
		_lower = lower;
		_other = other;
	}


	/// <summary>
	/// Indicates the digit used.
	/// </summary>
	public byte Digit
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (byte)(_other & 15);
	}

	/// <summary>
	/// Indicates whether the current node is a grouped node, which means it uses more than 1 cell.
	/// </summary>
	public bool IsGroupedNode
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Cells(_higher, _lower).Count >= 2;
	}

	/// <summary>
	/// Indicates the cells used.
	/// </summary>
	public Cells Cells
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new(_higher, _lower);
	}

	/// <summary>
	/// Indicates the type of the node.
	/// </summary>
	public NodeType Type
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (NodeType)(int)(_other >> 4 & 7);
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed override bool Equals([NotNullWhen(true)] object? obj) => Equals(obj as Node);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals([NotNullWhen(true)] Node? other) =>
		other is { Cells: var c, Digit: var d } && c == Cells && d == Digit;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed override int GetHashCode() => HashCode.Combine(Cells, Digit);

	/// <summary>
	/// Gets the simplified string value that only displays the important information.
	/// </summary>
	/// <returns>The string value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual string ToSimpleString() => $"{Digit + 1}{Cells}";

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed override string ToString()
	{
		const string defaultName = "<Unnamed>";

		string nodeTypeName = defaultName;
		if (typeof(NodeType).GetField(Type.ToString()) is { } fieldInfo)
		{
			nodeTypeName = fieldInfo.GetCustomAttribute<NodeTypeNameAttribute>() is { Name: var name }
				? name
				: GetType().FullName ?? defaultName;
		}

		return $"{nodeTypeName}: {ToSimpleString()}";
	}


	/// <summary>
	/// Determines whether two <see cref="Node"/>s are same.
	/// </summary>
	/// <param name="left">Indicates the left-side instance to compare.</param>
	/// <param name="right">Indicates the right-side instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Node? left, Node? right) =>
		(left, right) switch { (null, null) => true, (not null, not null) => left.Equals(right), _ => false };

	/// <summary>
	/// Determines whether two <see cref="Node"/>s are not totally same.
	/// </summary>
	/// <param name="left">Indicates the left-side instance to compare.</param>
	/// <param name="right">Indicates the right-side instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Node? left, Node? right) => !(left == right);
}
