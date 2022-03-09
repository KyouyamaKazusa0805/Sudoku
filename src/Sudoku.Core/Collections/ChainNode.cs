namespace Sudoku.Collections;

/// <summary>
/// Defines a chain node.
/// </summary>
public readonly struct ChainNode :
	IEquatable<ChainNode>
#if FEATURE_GENERIC_MATH
	,
	IEqualityOperators<ChainNode, ChainNode>
#endif
{
	/// <summary>
	/// Initializes a <see cref="ChainNode"/> instance via the bit mask.
	/// </summary>
	/// <param name="mask">The mask of 64 bits.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ChainNode(long mask) => Mask = mask;


	/// <summary>
	/// Indicates the type of the chain node.
	/// </summary>
	public ChainNodeType Type
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (ChainNodeType)(Mask >> 60);
	}

	/// <summary>
	/// Gets or sets the cell.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	/// Throws when the chain node type is not <see cref="ChainNodeType.Sole"/>.
	/// </exception>
	public byte Cell
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get =>
			Type == ChainNodeType.Sole
				? (byte)(Mask & 81)
				: throw new InvalidOperationException("The chain node type is invalid.");

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private init => Mask |= value;
	}

	/// <summary>
	/// Gets or sets the digit.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	/// Throws when the chain node type is not <see cref="ChainNodeType.Sole"/>.
	/// </exception>
	public byte Digit
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get =>
			Type == ChainNodeType.Sole
				? (byte)(Mask >> 56 & 15)
				: throw new InvalidOperationException("The chain node type is invalid.");

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private init => Mask |= (long)value << 56;
	}

	/// <summary>
	/// Indicates the inner mask. The highest 4 bits are the type of the node. For more information
	/// please visit the type <see cref="ChainNodeType"/>.
	/// </summary>
	/// <seealso cref="ChainNodeType"/>
	public long Mask { get; private init; }


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] object? obj) =>
		obj is ChainNode comparer && Equals(comparer);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(ChainNode other) => Mask == other.Mask;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() =>
		Type switch
		{
			ChainNodeType.Sole => Cell * 9 + Digit,
			_ => 0 // Undefined.
		};

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() =>
		Type switch
		{
			ChainNodeType.Sole => $"{nameof(ChainNodeType.Sole)}: Digit {Digit + 1}, Cell {new Cells { Cell }}",
			_ => "<Undefined Type>"
		};


	/// <summary>
	/// Creates a <see cref="ChainNode"/> instance via the specified digit and the cell used,
	/// as the type <see cref="ChainNodeType.Sole"/>.
	/// </summary>
	/// <param name="digit">The digit.</param>
	/// <param name="cell">The cell.</param>
	/// <returns>The <see cref="ChainNode"/> instance.</returns>
	/// <seealso cref="ChainNodeType.Sole"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ChainNode FromSingleCandidate(byte digit, byte cell) =>
#if true
		new((long)digit << 56 | cell);
#else
		// More clear but slower.
		new() { Cell = cell, Digit = digit };
#endif


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(ChainNode left, ChainNode right) => left.Mask == right.Mask;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(ChainNode left, ChainNode right) => left.Mask != right.Mask;
}
