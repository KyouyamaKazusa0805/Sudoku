namespace Sudoku.Concepts.Coloring;

/// <summary>
/// <para>Represents one state of a candidate in a <see cref="Cluster"/>.</para>
/// <para>
/// Please visit <see href="http://sudopedia.enjoysudoku.com/Parity.html">this link</see>
/// to learn more information about this concept.
/// </para>
/// </summary>
/// <param name="ParityFlag">
/// <para>Indicates which parity the current set belongs to.</para>
/// <para>
/// Due to limitation of this concept, there can only be 2 parities in one cluster.
/// Therefore, this value is a <see cref="bool"/> indicating "on" and "off" state - they are opposite to each other.
/// </para>
/// </param>
/// <param name="Cells">Indicates the cells used.</param>
/// <seealso cref="Cluster"/>
/// <seealso href="http://sudopedia.enjoysudoku.com/Parity.html">Sudopedia Mirror - Parity</seealso>
public readonly record struct Parity(bool ParityFlag, in CellMap Cells) : IEqualityOperators<Parity, Parity, bool>
{
	/// <summary>
	/// Try to get all pairs of parities of all components of the specified graph.
	/// </summary>
	/// <param name="graph">The graph.</param>
	/// <returns>A list of pairs of parities of components of the specified graph.</returns>
	public static ReadOnlySpan<(Parity On, Parity Off)> Create(in CellGraph graph)
	{
		var result = new List<(Parity, Parity)>();
		foreach (var component in graph.Components)
		{
			foreach (var startCell in component[1])
			{
				graph.GetComponentOf(startCell, out var depths);
				var group =
					from depth in depths
					let depthKey = (depth.Depth & 1) == 1
					group depth by depthKey into depthGroup
					let depthCells = (from depth in depthGroup select depth.Cell).AsCellMap()
					select (DepthValueIsOdd: depthGroup.Key, Cells: depthCells);
				result.Add((new(true, group[0].Cells), new(false, group[1].Cells)));
				break;
			}
		}
		return result.AsSpan();
	}
}
