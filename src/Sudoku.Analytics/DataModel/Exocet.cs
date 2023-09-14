using System.Runtime.CompilerServices;
using Sudoku.Concepts;

namespace Sudoku.DataModel;

/// <summary>
/// <para>
/// Indicates an exocet pattern. The pattern will be like:
/// <code><![CDATA[
/// .-------.-------.-------.
/// | B B E | E . . | E . . |
/// | . . E | Q . . | R . . |
/// | . . E | Q . . | R . . |
/// :-------+-------+-------:
/// | . . S | S . . | S . . |
/// | . . S | S . . | S . . |
/// | . . S | S . . | S . . |
/// :-------+-------+-------:
/// | . . S | S . . | S . . |
/// | . . S | S . . | S . . |
/// | . . S | S . . | S . . |
/// '-------'-------'-------'
/// ]]></code>
/// Where:
/// <list type="table">
/// <item><term>B</term><description>Base Cells.</description></item>
/// <item><term>Q</term><description>1st Object Pair (Target cells pair 1).</description></item>
/// <item><term>R</term><description>2nd Object Pair (Target cells pair 2).</description></item>
/// <item><term>S</term><description>Cross-line Cells.</description></item>
/// <item><term>E</term><description>Escape Cells.</description></item>
/// </list>
/// </para>
/// <para>
/// In the data pattern, all letters will be used as the same one in this exemplar.
/// In addition, if senior exocet, one of two target cells will lie in cross-line cells,
/// and the lines of two target cells lying on can't contain any base digits.
/// </para>
/// </summary>
/// <param name="Base1">Indicates the first base cell.</param>
/// <param name="Base2">Indicates the second base cell.</param>
/// <param name="TargetQ1">Indicates the first target cell in the Q part.</param>
/// <param name="TargetQ2">Indicates the second target cell in the Q part.</param>
/// <param name="TargetR1">Indicates the first target cell in the R part.</param>
/// <param name="TargetR2">Indicates the second target cell in the R part.</param>
/// <param name="MirrorQ1">Indicates the first mirror cell in the Q part.</param>
/// <param name="MirrorQ2">Indicates the second mirror cell in the Q part.</param>
/// <param name="MirrorR1">Indicates the first mirror cell in the R part.</param>
/// <param name="MirrorR2">Indicates the second mirror cell in the R part.</param>
/// <param name="CrossLine">Indicates the cross-line cells.</param>
public sealed record Exocet(
	Cell Base1,
	Cell Base2,
	Cell TargetQ1,
	Cell TargetQ2,
	Cell TargetR1,
	Cell TargetR2,
	scoped in CellMap CrossLine,
	scoped in CellMap MirrorQ1,
	scoped in CellMap MirrorQ2,
	scoped in CellMap MirrorR1,
	scoped in CellMap MirrorR2
)
{
	/// <inheritdoc/>
	public CellMap Map => CrossLine | TargetCellsMap | BaseCellsMap;

	/// <summary>
	/// Indicates the mirror cells.
	/// </summary>
	public CellMap MirrorCellsMap => MirrorQ1 | MirrorQ2 | MirrorR1 | MirrorR2;

	/// <summary>
	/// Indicates the full map, with mirror cells.
	/// </summary>
	public CellMap MapWithMirrors => Map | MirrorCellsMap;

	/// <summary>
	/// Indicates the base cells.
	/// </summary>
	public CellMap BaseCellsMap => [Base1, Base2];

	/// <summary>
	/// Indicates the target cells.
	/// </summary>
	public CellMap TargetCellsMap => [TargetQ1, TargetQ2, TargetR1, TargetR2];


	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(scoped in Exocet other)
		=> Base1 == other.Base1 && Base2 == other.Base2 && TargetQ1 == other.TargetQ1 && TargetQ2 == other.TargetQ2
		&& TargetR1 == other.TargetR1 && TargetR2 == other.TargetR2 && MirrorQ1 == other.MirrorQ1
		&& MirrorQ2 == other.MirrorQ2 && MirrorR1 == other.MirrorR1 && MirrorR2 == other.MirrorR2
		&& BaseCellsMap == other.BaseCellsMap && TargetCellsMap == other.TargetCellsMap
		&& CrossLine == other.CrossLine;

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => $"Exocet: base {BaseCellsMap}, target {TargetCellsMap}";
}
