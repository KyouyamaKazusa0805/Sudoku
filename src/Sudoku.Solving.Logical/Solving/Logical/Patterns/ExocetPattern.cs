namespace Sudoku.Solving.Logical.Patterns;

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
/// In the data structure, all letters will be used as the same one in this exemplar.
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
public readonly record struct ExocetPattern(
	int Base1,
	int Base2,
	int TargetQ1,
	int TargetQ2,
	int TargetR1,
	int TargetR2,
	scoped in CellMap CrossLine,
	scoped in CellMap MirrorQ1,
	scoped in CellMap MirrorQ2,
	scoped in CellMap MirrorR1,
	scoped in CellMap MirrorR2
) : IEquatable<ExocetPattern>, IEqualityOperators<ExocetPattern, ExocetPattern, bool>, ITechniquePattern<ExocetPattern>
{
	/// <inheritdoc/>
	public CellMap Map
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => CrossLine + TargetQ1 + TargetQ2 + TargetR1 + TargetR2 + Base1 + Base2;
	}

	/// <summary>
	/// Indicates the full map, with mirror cells.
	/// </summary>
	public CellMap MapWithMirrors
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Map | MirrorQ1 | MirrorQ2 | MirrorR1 | MirrorR2;
	}

	/// <summary>
	/// Indicates the base cells.
	/// </summary>
	public CellMap BaseCellsMap
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => CellsMap[Base1] + Base2;
	}

	/// <summary>
	/// Indicates the target cells.
	/// </summary>
	public CellMap TargetCellsMap
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => CellsMap[TargetQ1] + TargetQ2 + TargetR1 + TargetR2;
	}


	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(scoped in ExocetPattern other)
		=> Base1 == other.Base1 && Base2 == other.Base2 && TargetQ1 == other.TargetQ1 && TargetQ2 == other.TargetQ2
		&& TargetR1 == other.TargetR1 && TargetR2 == other.TargetR2 && MirrorQ1 == other.MirrorQ1
		&& MirrorQ2 == other.MirrorQ2 && MirrorR1 == other.MirrorR1 && MirrorR2 == other.MirrorR2
		&& BaseCellsMap == other.BaseCellsMap && TargetCellsMap == other.TargetCellsMap
		&& CrossLine == other.CrossLine;

	/// <inheritdoc cref="object.GetHashCode"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode()
		=> HashCode.Combine(
			CellsMap[Base1] + Base2,
			CellsMap[TargetQ1] + TargetQ2 + TargetR1 + TargetR2,
			MirrorQ1 | MirrorQ2 | MirrorR1 | MirrorR2,
			BaseCellsMap | TargetCellsMap
		);

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
	{
		var baseCellsStr = (CellsMap[Base1] + Base2).ToString();
		var targetCellsStr = (CellsMap[TargetQ1] + TargetQ2 + TargetR1 + TargetR2).ToString();
		return $"Exocet: base {baseCellsStr}, target {targetCellsStr}";
	}
}
