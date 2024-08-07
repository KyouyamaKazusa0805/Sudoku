namespace Sudoku.Concepts;

/// <summary>
/// Represents for a data set that describes the complete information about a unique loop technique.
/// </summary>
/// <param name="Loop">Indicates the cells used in this whole unique loop.</param>
/// <param name="Path">Indicates the detail path of the loop.</param>
/// <param name="DigitsMask">Indicates the digits used, represented as a mask of type <see cref="Mask"/>.</param>
public readonly record struct UniqueLoop(ref readonly CellMap Loop, Cell[] Path, Mask DigitsMask)
{
	/// <inheritdoc/>
	public override int GetHashCode() => Loop.GetHashCode();

	/// <inheritdoc/>
	public bool Equals(UniqueLoop other) => Loop == other.Loop;

	[SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
	private bool PrintMembers(StringBuilder stringBuilder)
	{
		stringBuilder.Append($"{nameof(Loop)} = {Loop}");
		stringBuilder.Append(", ");
		stringBuilder.Append($"{nameof(Path)} = [");
		for (var i = 0; i < Path.Length; i++)
		{
			var cell = Path[i];
			stringBuilder.Append(cell);
			if (i != Path.Length - 1)
			{
				stringBuilder.Append(", ");
			}
		}
		stringBuilder.Append("], ");
		stringBuilder.Append($"{nameof(DigitsMask)} = {DigitsMask} (0b{Convert.ToString(DigitsMask, 2).PadLeft(9, '0')})");
		return true;
	}
}
