namespace Sudoku.Analytics.Construction.Patterns;

/// <summary>
/// Represents a domino loop pattern.
/// </summary>
/// <param name="cells">Indicates the cells used.</param>
[TypeImpl(TypeImplFlag.AllObjectMethods | TypeImplFlag.EqualityOperators)]
public sealed partial class DominoLoop([PrimaryConstructorParameter] Cell[] cells) :
	IEquatable<DominoLoop>,
	IEqualityOperators<DominoLoop, DominoLoop, bool>
{
	/// <summary>
	/// Indicates the cells used.
	/// </summary>
	[HashCodeMember]
	public CellMap Map => [.. Cells];

	[StringMember(nameof(Map))]
	private string MapString => Map.ToString();


	/// <inheritdoc/>
	public bool Equals([NotNullWhen(true)] DominoLoop? other) => other is not null && Map == other.Map;
}
