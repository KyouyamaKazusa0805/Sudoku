namespace Sudoku.Behaviors.Diff;

/// <summary>
/// Represents an exception that describes two grids are different, and cannot analyze difference between them.
/// </summary>
[IntroducedSince(3, 4)]
public sealed class GridDiffTooMuchException : Exception;
