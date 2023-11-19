using System.Runtime.CompilerServices;

namespace Sudoku.Concepts.ObjectModel;

/// <summary>
/// Represents a chain node list with head candidate.
/// </summary>
/// <param name="capacity">The capacity.</param>
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public sealed class ChainNodeListWithHeadCandidate(int capacity) : Dictionary<ChainNode, Candidate>(capacity);
