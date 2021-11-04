namespace Sudoku.Solving.Manual.Exocets;

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
[AutoDeconstructLambda(nameof(BaseCellsMap), nameof(TargetCellsMap), nameof(CrossLine))]
[AutoDeconstruct(nameof(Base1), nameof(Base2), nameof(TargetQ1), nameof(TargetQ2), nameof(TargetR1), nameof(TargetR2))]
[AutoDeconstruct(nameof(Base1), nameof(Base2), nameof(TargetQ1), nameof(TargetQ2), nameof(TargetR1), nameof(TargetR2), nameof(CrossLine), nameof(MirrorQ1), nameof(MirrorQ2), nameof(MirrorR1), nameof(MirrorR2), nameof(BaseCellsMap), nameof(TargetCellsMap))]
[AutoGetHashCode(nameof(Base1), nameof(Base2), nameof(TargetQ1), nameof(TargetQ2), nameof(TargetR1), nameof(TargetR2), nameof(CrossLine), nameof(MirrorQ1), nameof(MirrorQ2), nameof(MirrorR1), nameof(MirrorR2), nameof(BaseCellsMap), nameof(TargetCellsMap))]
[AutoEquality(nameof(Base1), nameof(Base2), nameof(TargetQ1), nameof(TargetQ2), nameof(TargetR1), nameof(TargetR2), nameof(CrossLine), nameof(MirrorQ1), nameof(MirrorQ2), nameof(MirrorR1), nameof(MirrorR2), nameof(BaseCellsMap), nameof(TargetCellsMap))]
public readonly partial struct Pattern : IValueEquatable<Pattern>
{
	/// <summary>
	/// Initializes an instance with the specified cells.
	/// </summary>
	/// <param name="base1">The base cell 1.</param>
	/// <param name="base2">The base cell 2.</param>
	/// <param name="tq1">The target Q1 cell.</param>
	/// <param name="tq2">The target Q2 cell.</param>
	/// <param name="tr1">The target R1 cell.</param>
	/// <param name="tr2">The target R2 cell.</param>
	/// <param name="crossline">The cross line cells.</param>
	/// <param name="mq1">The mirror Q1 cell.</param>
	/// <param name="mq2">The mirror Q2 cell.</param>
	/// <param name="mr1">The mirror R1 cell.</param>
	/// <param name="mr2">The mirror R2 cell.</param>
	public Pattern(
		int base1,
		int base2,
		int tq1,
		int tq2,
		int tr1,
		int tr2,
		in Cells crossline,
		in Cells mq1,
		in Cells mq2,
		in Cells mr1,
		in Cells mr2
	)
	{
		CrossLine = crossline;
		Base1 = base1;
		Base2 = base2;
		TargetQ1 = tq1;
		TargetQ2 = tq2;
		TargetR1 = tr1;
		TargetR2 = tr2;
		MirrorQ1 = mq1;
		MirrorQ2 = mq2;
		MirrorR1 = mr1;
		MirrorR2 = mr2;
	}


	/// <summary>
	/// Indicates the base cell 1.
	/// </summary>
	public int Base1 { get; }

	/// <summary>
	/// Indicates the base cell 2.
	/// </summary>
	public int Base2 { get; }

	/// <summary>
	/// Indicates the target Q1 cell.
	/// </summary>
	public int TargetQ1 { get; }

	/// <summary>
	/// Indicates the target Q2 cell.
	/// </summary>
	public int TargetQ2 { get; }

	/// <summary>
	/// Indicates the target R1 cell.
	/// </summary>
	public int TargetR1 { get; }

	/// <summary>
	/// Indicates the target R2 cell.
	/// </summary>
	public int TargetR2 { get; }

	/// <summary>
	/// Indicates the cross line cells.
	/// </summary>
	public Cells CrossLine { get; }

	/// <summary>
	/// Indicates the mirror Q1 cell.
	/// </summary>
	public Cells MirrorQ1 { get; }

	/// <summary>
	/// Indicates the mirror Q2 cell.
	/// </summary>
	public Cells MirrorQ2 { get; }

	/// <summary>
	/// Indicates the mirror R1 cell.
	/// </summary>
	public Cells MirrorR1 { get; }

	/// <summary>
	/// Indicates the mirror R2 cell.
	/// </summary>
	public Cells MirrorR2 { get; }

	/// <summary>
	/// Indicates the base cells.
	/// </summary>
	private Cells BaseCellsMap
	{
		[LambdaBody]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new() { Base1, Base2 };
	}

	/// <summary>
	/// Indicates the target cells.
	/// </summary>
	private Cells TargetCellsMap
	{
		[LambdaBody]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new() { TargetQ1, TargetQ2, TargetR1, TargetR2 };
	}


	/// <inheritdoc cref="object.ToString"/>
	public override string ToString()
	{
		string baseCellsStr = new Cells { Base1, Base2 }.ToString();
		string targetCellsStr = new Cells { TargetQ1, TargetQ2, TargetR1, TargetR2 }.ToString();
		return $"Exocet: base {baseCellsStr}, target {targetCellsStr}";
	}
}
