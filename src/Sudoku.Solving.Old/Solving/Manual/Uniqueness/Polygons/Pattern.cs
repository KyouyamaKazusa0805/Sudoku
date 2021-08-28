namespace Sudoku.Solving.Manual.Uniqueness.Polygons;

/// <summary>
/// Indicates the borescoper's deadly pattern.
/// </summary>
[AutoEquality(nameof(_mask))]
public readonly partial struct Pattern : IValueEquatable<Pattern>
{
	/// <summary>
	/// Indicates the internal structure.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This mask is of type <see cref="long"/>:
	/// <code>
	/// 0      7     14     21     28     35     42     49     56
	/// ↓      ↓      ↓      ↓      ↓      ↓      ↓      ↓      ↓
	/// |-------|-------|-------|-------|-------|-------|-------|-------|
	/// </code>
	/// where the bit [0..56] is for 8 cells, the last 7 bits determine the pattern is a
	/// heptagon or a octagon. If the value is 127 (not available), the pattern will be a heptagon.
	/// </para>
	/// <para>
	/// Due to the rendering engine, you have to check this file rather than the tip window.
	/// </para>
	/// </remarks>
	private readonly long _mask;


	/// <summary>
	/// Initializes an instance with the specified mask.
	/// </summary>
	/// <param name="mask">The mask.</param>
	public Pattern(long mask) => _mask = mask;


	/// <summary>
	/// Indicates whether the specified pattern is a heptagon.
	/// </summary>
	public bool IsHeptagon => (_mask >> 28 & 127) == 127;

	/// <summary>
	/// Indicates the map of pair 1 cells.
	/// </summary>
	public Cells Pair1Map => new() { Pair1.Item1, Pair1.Item2 };

	/// <summary>
	/// Indicates the map of pair 2 cells.
	/// </summary>
	public Cells Pair2Map => new() { Pair2.Item1, Pair2.Item2 };

	/// <summary>
	/// The map of other three (or four) cells.
	/// </summary>
	public Cells CenterCellsMap
	{
		get
		{
			var (a, b, c, d) = CenterCells;
			return IsHeptagon ? new() { a, b, c } : new Cells { a, b, c, d };
		}
	}

	/// <summary>
	/// The map.
	/// </summary>
	public Cells Map => Pair1Map | Pair2Map | CenterCellsMap;

	/// <summary>
	/// Indicates the pair 1.
	/// </summary>
	private (int, int) Pair1 => ((int)(_mask >> 7 & 127), (int)(_mask & 127));

	/// <summary>
	/// Indicates the pair 2.
	/// </summary>
	private (int, int) Pair2 => ((int)(_mask >> 21 & 127), (int)(_mask >> 14 & 127));

	/// <summary>
	/// Indicates the other three (or four) cells.
	/// </summary>
	/// <remarks>
	/// <b>If and only if</b> the fourth value in the returned quadruple is available.
	/// </remarks>
	private (int, int, int, int) CenterCells => (
		(int)(_mask >> 49 & 127),
		(int)(_mask >> 42 & 127),
		(int)(_mask >> 35 & 127),
		(int)(_mask >> 28 & 127)
	);


	/// <inheritdoc cref="object.GetHashCode"/>
	public override int GetHashCode() => (int)_mask;

	/// <inheritdoc cref="object.ToString"/>
	public override string ToString() => Map.ToString();
}
