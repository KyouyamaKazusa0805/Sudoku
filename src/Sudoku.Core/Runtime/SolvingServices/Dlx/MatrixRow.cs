namespace Sudoku.Runtime.SolvingServices.Dlx;

/// <summary>
/// Represents a type describing for a matrix row.
/// </summary>
public record struct MatrixRow(DancingLinkNode Cell, DancingLinkNode Row, DancingLinkNode Column, DancingLinkNode Block);
