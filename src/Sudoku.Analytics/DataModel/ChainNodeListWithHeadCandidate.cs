namespace Sudoku.DataModel;

/// <summary>
/// Represents a chain node list with head candidate.
/// </summary>
/// <remarks>
/// Initializes a <see cref="ChainNodeListWithHeadCandidate"/> instance.
/// </remarks>
/// <param name="capacity">The capacity.</param>
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public sealed class ChainNodeListWithHeadCandidate(int capacity) : Dictionary<ChainNode, Candidate>(capacity);
