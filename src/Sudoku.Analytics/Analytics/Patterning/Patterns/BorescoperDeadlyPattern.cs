namespace Sudoku.Analytics.Patterning.Patterns;

/// <summary>
/// Defines a pattern that is a Borescoper's Deadly Pattern technique pattern in theory. The sketch is like:
/// <code><![CDATA[
/// .-------.-------.-------.
/// | . . . | . . . | . . . |
/// | . . . | . . . | . . . |
/// | . . . | . . . | . . . |
/// :-------+-------+-------:
/// | . . . | . . . | . . . |
/// | . . . | . . . | . . . |
/// | . . . | . . . | P P . |
/// :-------+-------+-------:
/// | . . . | . . Q | S S . |
/// | . . . | . . Q | S(S). |
/// | . . . | . . . | . . . |
/// '-------'-------'-------'
/// ]]></code>
/// Where:
/// <list type="table">
/// <item><term>P</term><description>The first group of cells.</description></item>
/// <item><term>Q</term><description>The second group of cells.</description></item>
/// <item>
/// <term>S</term>
/// <description>
/// The square cells of size 3 or 4. The cell with the bracket (r8c8 in the picture)
/// is optional.
/// </description>
/// </item>
/// </list>
/// </summary>
/// <param name="mask">
/// <para>Indicates the internal mask.</para>
/// <para>
/// This mask is of type <see cref="long"/>, where the distribution of each bit is as follows:
/// <code><![CDATA[
/// 0      7     14     21     28     35     42     49     56
/// ↓      ↓      ↓      ↓      ↓      ↓      ↓      ↓      ↓
/// |-------|-------|-------|-------|-------|-------|-------|-------|
/// ↑       ↑       ↑       ↑       ↑       ↑       ↑       ↑       ↑
/// 0       8      16      24      32      40      48      56      64
/// ]]></code>
/// where the bit <c>[0..56]</c> is for 8 cells, the last 7 bits determine the pattern is a
/// heptagon or a octagon. If the value is 127 (not available), the pattern will be a heptagon.
/// </para>
/// <para>
/// Due to the drawing API, you have to check this file rather than the tip window.
/// </para>
/// </param>
public readonly partial struct BorescoperDeadlyPattern([PrimaryConstructorParameter(MemberKinds.Field)] long mask)
{
	/// <summary>
	/// Indicates whether the specified pattern is a heptagon.
	/// </summary>
	public bool IsHeptagon => (_mask >> 28 & 127) == 127;

	/// <summary>
	/// Indicates the map of pair 1 cells.
	/// </summary>
	public CellMap Pair1Map => [Pair1.A, Pair1.B];

	/// <summary>
	/// Indicates the map of pair 2 cells.
	/// </summary>
	public CellMap Pair2Map => [Pair2.A, Pair2.B];

	/// <summary>
	/// The map of other three (or four) cells.
	/// </summary>
	public CellMap CenterCellsMap
		=> this switch
		{
			{ CenterCells: var (a, b, c, _), IsHeptagon: true } => [a, b, c],
			{ CenterCells: var (a, b, c, d), IsHeptagon: false } => [a, b, c, d]
		};

	/// <summary>
	/// Indicates the full map of all cells used in this pattern.
	/// </summary>
	public CellMap Map => Pair1Map | Pair2Map | CenterCellsMap;

	/// <summary>
	/// Indicates the pair 1.
	/// </summary>
	private (Cell A, Cell B) Pair1 => ((Cell)(_mask >> 7 & 127), (Cell)(_mask & 127));

	/// <summary>
	/// Indicates the pair 2.
	/// </summary>
	private (Cell A, Cell B) Pair2 => ((Cell)(_mask >> 21 & 127), (Cell)(_mask >> 14 & 127));

	/// <summary>
	/// Indicates the other three (or four) cells.
	/// </summary>
	/// <remarks>
	/// <b>If and only if</b> the fourth value in the returned quadruple is available.
	/// </remarks>
	private (Cell A, Cell B, Cell C, Cell D) CenterCells
		=> ((Cell)(_mask >> 49 & 127), (Cell)(_mask >> 42 & 127), (Cell)(_mask >> 35 & 127), (Cell)(_mask >> 28 & 127));


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void Deconstruct(out (Cell A, Cell B) pair1, out (Cell A, Cell B) pair2, out (Cell A, Cell B, Cell C, Cell D) centerCells)
		=> (pair1, pair2, centerCells) = (Pair1, Pair2, CenterCells);
}
