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
[TypeImpl(TypeImplFlag.Object_GetHashCode)]
public sealed partial class FireworkPattern(
	[Property, HashCodeMember] ref readonly CellMap map,
	[Property, HashCodeMember] Cell? pivot
) : Pattern
{
	/// <inheritdoc/>
	public override bool IsChainingCompatible => false;


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Pattern? other)
		=> other is FireworkPattern comparer && Map == comparer.Map && Pivot == comparer.Pivot;

	/// <inheritdoc/>
	public override FireworkPattern Clone() => new(Map, Pivot);
}
