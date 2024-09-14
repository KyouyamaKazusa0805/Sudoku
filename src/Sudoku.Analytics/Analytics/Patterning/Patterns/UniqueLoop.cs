namespace Sudoku.Analytics.Patterning.Patterns;

/// <summary>
/// Represents for a data set that describes the complete information about a unique loop technique.
/// </summary>
/// <param name="Loop">Indicates the cells used in this whole unique loop.</param>
/// <param name="Path">Indicates the detail path of the loop.</param>
/// <param name="DigitsMask">Indicates the digits used, represented as a mask of type <see cref="Mask"/>.</param>
[TypeImpl(TypeImplFlag.Object_GetHashCode)]
public readonly partial record struct UniqueLoop(ref readonly CellMap Loop, Cell[] Path, Mask DigitsMask)
{
	[HashCodeMember]
	private int LoopHashCode => Loop.GetHashCode();


	/// <inheritdoc/>
	public bool Equals(UniqueLoop other) => Loop == other.Loop;

	/// <include
	///     file="../../global-doc-comments.xml"
	///     path="/g/csharp9/feature[@name='records']/target[@name='method' and @cref='PrintMembers']"/>
	private bool PrintMembers(StringBuilder builder)
	{
		builder.Append($"{nameof(Loop)} = {Loop}");
		builder.Append(", ");
		builder.Append($"{nameof(Path)} = [");
		for (var i = 0; i < Path.Length; i++)
		{
			builder.Append(Path[i]);
			if (i != Path.Length - 1)
			{
				builder.Append(", ");
			}
		}
		builder.Append("], ");
		builder.Append($"{nameof(DigitsMask)} = {DigitsMask} ({MaskOperations.ToBinaryString(DigitsMask)})");
		return true;
	}


	/// <summary>
	/// Determine whether the specified loop is a valid unique loop or unique rectangle pattern.
	/// </summary>
	/// <typeparam name="TCollection">The type of the collection that can iterate on each <see cref="Cell"/> instances.</typeparam>
	/// <param name="loopPath">The path of the loop.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <remarks>
	/// <para>
	/// This method uses a trick way (node-coloring) to check whether a loop is a unique loop.
	/// The parameter <paramref name="loopPath"/> holds a list of cells, which are nodes of the loop.
	/// Then we begin to colorize each node with 2 different colors <b><i>A</i></b> and <b><i>B</i></b>.
	/// The only point we should notice is that the color between 2 adjacent nodes should be different
	/// (i.e. one is colored with <b><i>A</i></b> and the other should be colored with <b><i>B</i></b>).
	/// If at least one pair of cells in a same house are colored with a same color, it won't be a valid unique loop.
	/// Those colors stands for the final filling digits. Therefore, "Two cells in a same house are colored same"
	/// means "Two cells in a same house are filled with a same digit", which disobeys the basic sudoku rule.
	/// </para>
	/// <para>
	/// This method won't check for whether the loop is a unique rectangle (of length 4). It means, a valid unique rectangle
	/// can also make this method return <see langword="true"/>.
	/// </para>
	/// </remarks>
	public static bool IsValid<TCollection>(TCollection loopPath) where TCollection : IEnumerable<Cell>, allows ref struct
	{
		var (visitedOdd, visitedEven, isOdd) = (0, 0, false);
		foreach (var cell in loopPath)
		{
			foreach (var houseType in HouseTypes)
			{
				var house = cell.ToHouse(houseType);
				if (isOdd)
				{
					if ((visitedOdd >> house & 1) != 0)
					{
						return false;
					}
					visitedOdd |= 1 << house;
				}
				else
				{
					if ((visitedEven >> house & 1) != 0)
					{
						return false;
					}
					visitedEven |= 1 << house;
				}
			}
			isOdd = !isOdd;
		}
		return visitedEven == visitedOdd;
	}
}
