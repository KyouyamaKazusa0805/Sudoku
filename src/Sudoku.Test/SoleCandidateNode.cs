using Sudoku.Collections;

namespace Sudoku.Test;

/// <summary>
/// Provides with the node that stores a sole candidate.
/// </summary>
public sealed class SoleCandidateNode : Node
{
	/// <summary>
	/// Initializes a <see cref="SoleCandidateNode"/> instance via the candidate and its current status.
	/// </summary>
	/// <param name="cell">Indicates the cell used.</param>
	/// <param name="digit">Indicates the digit used.</param>
	/// <param name="isOn">Indicates whether the node is on.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SoleCandidateNode(byte cell, byte digit)
	{
		Cell = cell;
		Digit = digit;
	}


	/// <summary>
	/// Indicates the cell used.
	/// </summary>
	public byte Cell { get; }

	/// <summary>
	/// Indicates the digit used.
	/// </summary>
	public byte Digit { get; }

	/// <summary>
	/// Indicates the candidate used.
	/// </summary>
	public int Candidate
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Cell * 9 + Digit;
	}

	/// <inheritdoc/>
	public override NodeType Type
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => NodeType.Sole;
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] Node? other) =>
		other is SoleCandidateNode comparer && Cell == comparer.Cell && Digit == comparer.Digit;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => ((int)Type << 1) + Cell * 9 + Digit;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => $"Sole candidate {{ {nameof(Candidate)} = {ToSimpleString()} }}";

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToSimpleString() => $"{Digit + 1}{new Cells { Cell }}";
}
