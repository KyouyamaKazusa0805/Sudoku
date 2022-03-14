using Sudoku.Collections;
using static Sudoku.Constants.Tables;

namespace Sudoku.Test;

/// <summary>
/// Defines a chain node that provides with the data for a locked candidates.
/// </summary>
public sealed class LockedCandidatesNode : Node
{
	/// <summary>
	/// Indicates the maximum global ID that the current-typed instance can be reached.
	/// </summary>
	internal const int MaxGlobalId = 1944;


	/// <summary>
	/// Indicates the lookup table that can get the cells used via the ID value.
	/// </summary>
	private static readonly Dictionary<byte, Cells> IdToCellsLookup;

	/// <summary>
	/// Indicates the lookup table that can get the ID value via the cells used.
	/// </summary>
	private static readonly Dictionary<Cells, byte> CellsToIdLookup;


	/// <summary>
	/// Indicates the ID used.
	/// </summary>
	private readonly byte _id;


	/// <summary>
	/// Initializes a <see cref="LockedCandidatesNode"/> instance via the digit used,
	/// and two cells used.
	/// </summary>
	/// <param name="digit">The digit used.</param>
	/// <param name="cell1">The cell 1.</param>
	/// <param name="cell2">The cell 2.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public LockedCandidatesNode(byte digit, byte cell1, byte cell2) :
		this(digit, CellsToIdLookup[new() { cell1, cell2 }])
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
		this(digit, CellsToIdLookup[new() { cell1, cell2, cell3 }])
	{
	}

	/// <summary>
	/// Initializes a <see cref="LockedCandidatesNode"/> instance via the digit used,
	/// and the global ID that corresponds to the unique locked candidate structure.
	/// </summary>
	/// <param name="digit">The digit used.</param>
	/// <param name="id">The global ID value that corresponds to the unique locked candidate structure.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private LockedCandidatesNode(byte digit, byte id)
	{
		_id = id;
		Digit = digit;
	}


	/// <include file="../../global-doc-comments.xml" path='g/static-constructor'/>
	static LockedCandidatesNode()
	{
		IdToCellsLookup = new(216);
		CellsToIdLookup = new(216);

		byte i = 0;
		foreach (var ((_, _), (_, _, map, _)) in IntersectionMaps)
		{
			var cellCombinations2 = map & 2;
			var cellCombinations3 = map & 3;
			IdToCellsLookup.Add(i, cellCombinations2[0]);
			CellsToIdLookup.Add(cellCombinations2[0], i++);
			IdToCellsLookup.Add(i, cellCombinations2[1]);
			CellsToIdLookup.Add(cellCombinations2[1], i++);
			IdToCellsLookup.Add(i, cellCombinations2[2]);
			CellsToIdLookup.Add(cellCombinations2[2], i++);
			IdToCellsLookup.Add(i, cellCombinations3[0]);
			CellsToIdLookup.Add(cellCombinations3[0], i++);
		}
	}


	/// <summary>
	/// Indicates the digit used.
	/// </summary>
	public byte Digit { get; }

	/// <summary>
	/// Indicates the cells used.
	/// </summary>
	public Cells Cells
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => IdToCellsLookup[_id];
	}

	/// <inheritdoc/>
	public override NodeType Type
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => NodeType.LockedCandidates;
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override LockedCandidatesNode Clone() => new(Digit, _id);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] Node? other) =>
		other is LockedCandidatesNode comparer && _id == comparer._id && Digit == comparer.Digit;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => SoleCandidateNode.MaxGlobalId + _id * 9 + Digit;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToSimpleString() => $"{Digit + 1}{Cells}";

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => $"Locked candidates node: {ToSimpleString()}";
}
