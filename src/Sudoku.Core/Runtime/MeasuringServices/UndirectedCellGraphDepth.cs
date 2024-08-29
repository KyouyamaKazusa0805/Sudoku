namespace Sudoku.Runtime.MeasuringServices;

/// <summary>
/// Represents a node that describes for a graph node inside a <see cref="UndirectedCellGraph"/>, with depth from a node.
/// </summary>
/// <param name="Depth">Indicates the depth.</param>
/// <param name="Cell">The cell.</param>
public readonly record struct UndirectedCellGraphDepth(int Depth, Cell Cell);
