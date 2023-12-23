namespace Sudoku.Concepts;

/// <summary>
/// Defines a tuple of <see cref="CellMap"/> instances that represents the house cells used, especially used by <see cref="IntersectionMaps"/> property.
/// </summary>
/// <param name="LineMap">The map of the line.</param>
/// <param name="BlockMap">The map of the block.</param>
/// <param name="IntersectionMap">The map of the intersection.</param>
/// <param name="OtherBlocks">
/// Other blocks that the intersection map data does not cover. This property will be used by techniques such as Sue de Coq.
/// </param>
/// <seealso cref="IntersectionMaps"/>
public readonly record struct IntersectionResult(scoped ref readonly CellMap LineMap, scoped ref readonly CellMap BlockMap, scoped ref readonly CellMap IntersectionMap, byte[] OtherBlocks);
