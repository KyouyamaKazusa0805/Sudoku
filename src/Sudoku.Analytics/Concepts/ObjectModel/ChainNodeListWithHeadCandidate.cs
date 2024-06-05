namespace Sudoku.Concepts.ObjectModel;

/// <summary>
/// Represents a chain node list with head candidate.
/// </summary>
/// <param name="capacity">The capacity.</param>
[Obsolete]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public sealed class ChainNodeListWithHeadCandidate(int capacity) : Dictionary<ChainNode, Candidate>(capacity);
