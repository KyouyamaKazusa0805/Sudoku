namespace Sudoku.Solving;

/// <summary>
/// The matrix row.
/// </summary>
public record struct MatrixRow(DancingLinkNode Cell, DancingLinkNode Row, DancingLinkNode Column, DancingLinkNode Block);
