namespace Sudoku.Concepts;

/// <summary>
/// Represents a data structure whose equivalent concept to a <see href="https://sunnieshine.github.io/Sudoku/terms/chute">Chute</see>.
/// </summary>
/// <param name="Cells">Indicates the cells used in this chute.</param>
/// <param name="IsRow">Indicates whether the current chute is for a row.</param>
/// <param name="HousesMask">Indicates the houses used. The target value is a <see cref="short"/> value of 9 bits used.</param>
/// <remarks>
/// <include file="../../global-doc-comments.xml" path="/g/large-structure"/>
/// </remarks>
public readonly record struct Chute(scoped in CellMap Cells, bool IsRow, short HousesMask);
