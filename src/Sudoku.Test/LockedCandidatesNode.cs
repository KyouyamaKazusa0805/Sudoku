using Sudoku.Collections;

namespace Sudoku.Test;

/// <summary>
/// Defines a chain node that provides with the data for a locked candidates.
/// </summary>
public sealed class LockedCandidatesNode : Node
{
	/// <summary>
	/// Indicates the mask used.
	/// </summary>
	private readonly int _mask;


	/// <summary>
	/// Initializes a <see cref="LockedCandidatesNode"/> instance via the digit used,
	/// and two cells used.
	/// </summary>
	/// <param name="digit">The digit used.</param>
	/// <param name="cell1">The cell 1.</param>
	/// <param name="cell2">The cell 2.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public LockedCandidatesNode(byte digit, byte cell1, byte cell2) : this(digit << 21 | cell1 | cell2 << 7)
	{
	}

	/// <summary>
	/// Initializes a <see cref="LockedCandidatesNode"/> instance via the digit used,
	/// and three cells used.
	/// </summary>
	/// <param name="digit">The digit used.</param>
	/// <param name="cell1">The cell 1.</param>
	/// <param name="cell2">The cell 2.</param>
	/// <param name="cell3">The cell 3.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public LockedCandidatesNode(byte digit, byte cell1, byte cell2, byte cell3) :
		this(digit << 21 | cell1 | cell2 << 7 | cell3 << 14)
	{
	}

	/// <summary>
	/// Initializes a <see cref="LockedCandidatesNode"/> instance via the mask.
	/// </summary>
	/// <param name="mask">The mask that can provide with the basic data of the node.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private LockedCandidatesNode(int mask) => _mask = mask;


	/// <summary>
	/// Indicates the digit used.
	/// </summary>
	public byte Digit
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (byte)(_mask >> 21);
	}

	/// <summary>
	/// Indicates the cells used.
	/// </summary>
	public byte[] Cells
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			byte c1 = (byte)(_mask & 127);
			byte c2 = (byte)(_mask >> 7 & 127);
			byte c3 = (byte)(_mask >> 14 & 127);
			return c3 == 0 ? new[] { c1, c2 } : new[] { c1, c2, c3 };
		}
	}

	/// <inheritdoc/>
	public override NodeType Type
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => NodeType.LockedCandidates;
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override LockedCandidatesNode Clone() => new(_mask);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] Node? other) =>
		other is LockedCandidatesNode comparer && _mask == comparer._mask;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => _mask + 729;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToSimpleString() => $"{Digit + 1}{(Cells)Cells}";

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => $"Locked candidates node: {ToSimpleString()}";
}
