namespace Sudoku.Concepts;

/// <summary>
/// Represents an entry for blossom loop data used.
/// </summary>
/// <param name="Start">Indicates the start candidate.</param>
/// <param name="StartIsOn">Indicates whether the start node is on.</param>
/// <param name="End">Indicates the end candidate.</param>
/// <param name="EndIsOn">Indicates whether the end node is on.</param>
[StructLayout(LayoutKind.Explicit)]
public readonly record struct BlossomLoopEntry(
	[field: FieldOffset(0)] Candidate Start,
	[field: FieldOffset(3)] bool StartIsOn,
	[field: FieldOffset(4)] Candidate End,
	[field: FieldOffset(7)] bool EndIsOn
);
