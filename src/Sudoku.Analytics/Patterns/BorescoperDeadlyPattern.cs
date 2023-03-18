﻿namespace Sudoku.Analytics.Patterns;

/// <summary>
/// Defines a pattern that is a Borescoper's Deadly Pattern technique structure in theory. The sketch is like:
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
/// <para><inheritdoc cref="_mask" path="/summary"/></para>
/// <para><inheritdoc cref="_mask" path="//remarks/para[1]"/></para>
/// <para><inheritdoc cref="_mask" path="//remarks/para[2]"/></para>
/// </param>
public readonly partial struct BorescoperDeadlyPattern(long mask)
{
	/// <summary>
	/// Indicates the internal mask.
	/// </summary>
	/// <remarks>
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
	/// Due to the rendering engine, you have to check this file rather than the tip window.
	/// </para>
	/// </remarks>
	private readonly long _mask = mask;


	/// <summary>
	/// Indicates whether the specified pattern is a heptagon.
	/// </summary>
	public bool IsHeptagon => (_mask >> 28 & 127) == 127;

	/// <summary>
	/// Indicates the map of pair 1 cells.
	/// </summary>
	public CellMap Pair1Map => CellsMap[Pair1.A] + Pair1.B;

	/// <summary>
	/// Indicates the map of pair 2 cells.
	/// </summary>
	public CellMap Pair2Map => CellsMap[Pair2.A] + Pair2.B;

	/// <summary>
	/// The map of other three (or four) cells.
	/// </summary>
	public CellMap CenterCellsMap
		=> this switch
		{
			{ CenterCells: var (a, b, c, _), IsHeptagon: true } => CellsMap[a] + b + c,
			{ CenterCells: var (a, b, c, d), IsHeptagon: false } => CellsMap[a] + b + c + d
		};

	/// <inheritdoc/>
	public CellMap Map => Pair1Map | Pair2Map | CenterCellsMap;

	/// <summary>
	/// Indicates the pair 1.
	/// </summary>
	private (int A, int B) Pair1 => ((int)(_mask >> 7 & 127), (int)(_mask & 127));

	/// <summary>
	/// Indicates the pair 2.
	/// </summary>
	private (int A, int B) Pair2 => ((int)(_mask >> 21 & 127), (int)(_mask >> 14 & 127));

	/// <summary>
	/// Indicates the other three (or four) cells.
	/// </summary>
	/// <remarks>
	/// <b>If and only if</b> the fourth value in the returned quadruple is available.
	/// </remarks>
	private (int A, int B, int C, int D) CenterCells
		=> ((int)(_mask >> 49 & 127), (int)(_mask >> 42 & 127), (int)(_mask >> 35 & 127), (int)(_mask >> 28 & 127));


	[DeconstructionMethod]
	internal partial void Deconstruct(out (int A, int B) pair1, out (int A, int B) pair2, out (int A, int B, int C, int D) centerCells);
}
