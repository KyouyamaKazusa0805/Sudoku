namespace Sudoku.Analytics.Construction.Patterns;

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
/// <param name="map">Indicates the full map of all cells used.</param>
/// <param name="pivot">The pivot cell. This property can be <see langword="null"/> if four cells are used.</param>
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.Object_GetHashCode | TypeImplFlag.EqualityOperators)]
public sealed partial class Firework(
	[PrimaryConstructorParameter, HashCodeMember] ref readonly CellMap map,
	[PrimaryConstructorParameter, HashCodeMember] Cell? pivot
) : IEquatable<Firework>, IEqualityOperators<Firework, Firework, bool>
{
	/// <inheritdoc/>
	public bool Equals([NotNullWhen(true)] Firework? other) => other is not null && Map == other.Map && Pivot == other.Pivot;
}
