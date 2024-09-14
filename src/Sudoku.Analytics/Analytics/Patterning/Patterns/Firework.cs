namespace Sudoku.Concepts;

/// <summary>
/// Indicates a firework pattern. The pattern will be like:
/// <code><![CDATA[
/// .-------.-------.-------.
/// | . . . | . . . | . . . |
/// | . . . | . . . | . . . |
/// | . . . | . . . | . . . |
/// :-------+-------+-------:
/// | . . . | B . . | . C . |
/// | . . . | . . . | . . . |
/// | . . . | . . . | . . . |
/// :-------+-------+-------:
/// | . . . | . . . | . . . |
/// | . . . | . . . | . . . |
/// | . . . | A . . | .(D). |
/// '-------'-------'-------'
/// ]]></code>
/// </summary>
/// <param name="Map">Indicates the full map of all cells used.</param>
/// <param name="Pivot">The pivot cell. This property can be <see langword="null"/> if four cells are used.</param>
public readonly record struct Firework(ref readonly CellMap Map, Cell? Pivot);
