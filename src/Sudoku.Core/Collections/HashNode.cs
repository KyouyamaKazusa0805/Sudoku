namespace Sudoku.Collections;

/// <summary>
/// Defines a hash node that is used for storing a chain node.
/// </summary>
#if DEBUG
[DebuggerDisplay($$"""{{{nameof(ToString)}}(),nq}""")]
#endif
public readonly unsafe struct HashNode :
	IEquatable<HashNode>
#if FEATURE_GENERIC_MATH
	,
	IEqualityOperators<HashNode, HashNode>
#endif
{
	/// <summary>
	/// Indicates the ID of the current hash node.
	/// </summary>
	private readonly int _id = 0;

	/// <summary>
	/// Indicates the pointer that points to the next node.
	/// </summary>
	private readonly HashNode* _nextPtr = null;


	/// <summary>
	/// Initializes a <see cref="HashNode"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public HashNode()
	{
	}

	/// <summary>
	/// Initializes a <see cref="HashNode"/> instance with the specified ID.
	/// </summary>
	/// <param name="id">The ID.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public HashNode(int id) => _id = id;

	/// <summary>
	/// Initializes a <see cref="HashNode"/> instance with the specified ID and the pointer value
	/// that points to the next node.
	/// </summary>
	/// <param name="id">The ID.</param>
	/// <param name="nextPtr">The pointer that points to the next node.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public HashNode(int id, HashNode* nextPtr)
	{
		_id = id;
		_nextPtr = nextPtr;
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] object? obj) => obj is HashNode comparer && Equals(comparer);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(HashNode other) => _id == other._id;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => _id;

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() =>
		$"{nameof(HashNode)} {{ ID = {_id}, NextPtr = {(_nextPtr == null ? "<null>" : $"{_nextPtr->_id}")} }}";


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(HashNode left, HashNode right) => left.Equals(right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(HashNode left, HashNode right) => !(left == right);
}
