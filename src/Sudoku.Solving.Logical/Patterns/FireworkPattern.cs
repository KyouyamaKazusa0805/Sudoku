namespace Sudoku.Techniques.Patterns;

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
/// <param name="Map"><inheritdoc/></param>
/// <param name="Pivot">The pivot cell. This property can be <see langword="null"/> if four cells are used.</param>
public readonly record struct FireworkPattern(scoped in CellMap Map, int? Pivot) : ITechniquePattern<FireworkPattern>;
