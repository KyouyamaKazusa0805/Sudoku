namespace Sudoku.Concepts;

/// <summary>
/// Defines a tuple of <see cref="CellMap"/> instances that represents the house cells used,
/// especially used by <see cref="Intersection.Map"/> property.
/// </summary>
/// <param name="LineMap">The map of the line.</param>
/// <param name="BlockMap">The map of the block.</param>
/// <param name="IntersectionMap">The map of the intersection.</param>
/// <param name="OtherBlocks">
/// Other blocks that the intersection map data does not cover. This property will be used by techniques such as Sue de Coq.
/// </param>
/// <seealso cref="Intersection.Map"/>
public readonly record struct IntersectionResult(ref readonly CellMap LineMap, ref readonly CellMap BlockMap, ref readonly CellMap IntersectionMap, byte[] OtherBlocks);
