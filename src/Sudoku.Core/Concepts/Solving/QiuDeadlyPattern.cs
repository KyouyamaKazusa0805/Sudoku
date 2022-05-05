namespace Sudoku.Concepts.Solving;

/// <summary>
/// Defines a pattern that is a Qiu's deadly pattern technique structure in theory. The sketch is like:
/// <code><![CDATA[
/// .-------.-------.-------.
/// | . . . | . . . | . . . |
/// | . . . | . . . | . . . |
/// | P P . | . . . | . . . |
/// :-------+-------+-------:
/// | S S B | B B B | B B B |
/// | S S B | B B B | B B B |
/// | . . . | . . . | . . . |
/// :-------+-------+-------:
/// | . . . | . . . | . . . |
/// | . . . | . . . | . . . |
/// | . . . | . . . | . . . |
/// '-------'-------'-------'
/// ]]></code>
/// Where:
/// <list type="table">
/// <item><term>P</term><description>Pair Cells.</description></item>
/// <item><term>S</term><description>Square Cells.</description></item>
/// <item><term>B</term><description>Base-line Cells.</description></item>
/// </list>
/// </summary>
/// <param name="Square">The square cells that is <c>S</c> in that sketch.</param>
/// <param name="BaseLine">The base-line cells that is <c>B</c> in that sketch.</param>
/// <param name="Pair">The pair cells that is <c>P</c> in that sketch.</param>
[AutoOverridesToString(nameof(Map))]
[AutoOverridesGetHashCode(nameof(Square), nameof(BaseLine), nameof(Pair))]
[AutoOverridesEquals(nameof(Square), nameof(BaseLine), nameof(Pair), EmitInKeyword = true)]
public readonly partial record struct QiuDeadlyPattern(in Cells Square, in Cells BaseLine, in Cells Pair) :
	ITechniquePattern<QiuDeadlyPattern>
{
	/// <inheritdoc/>
	public Cells Map
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Square | BaseLine | Pair;
	}
}
