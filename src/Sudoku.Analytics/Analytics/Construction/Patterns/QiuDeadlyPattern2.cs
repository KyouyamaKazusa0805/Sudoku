namespace Sudoku.Analytics.Construction.Patterns;

/// <summary>
/// Defines a pattern that is a Qiu's deadly pattern technique pattern in theory. The sketch is like:
/// <code><![CDATA[
/// .-------.-------.-------.
/// | B B . | . . . | . . . |
/// | B B . | . . . | . . . |
/// | B B . | . . . | . . . |
/// :-------+-------+-------:
/// | S S B | B B B | B B B |
/// | S S B | B B B | B B B |
/// | B B . | . . . | . . . |
/// :-------+-------+-------:
/// | B B . | . . . | . . . |
/// | B B . | . . . | . . . |
/// | B B . | . . . | . . . |
/// '-------'-------'-------'
/// ]]></code>
/// Where:
/// <list type="table">
/// <item><term>S</term><description>Cross-line Cells.</description></item>
/// <item><term>B</term><description>Base-line Cells.</description></item>
/// </list>
/// </summary>
/// <param name="Lines1">The first pair of lines.</param>
/// <param name="Lines2">The second pair of lines.</param>
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.Object_GetHashCode | TypeImplFlag.EqualityOperators)]
public sealed partial class QiuDeadlyPattern2(
	[PrimaryConstructorParameter] HouseMask Lines1,
	[PrimaryConstructorParameter] HouseMask Lines2
) : IEquatable<QiuDeadlyPattern2>, IEqualityOperators<QiuDeadlyPattern2, QiuDeadlyPattern2, bool>
{
	/// <summary>
	/// Indicates the crossline cells.
	/// </summary>
	public CellMap Crossline
	{
		get
		{
			var l11 = HouseMask.TrailingZeroCount(Lines1);
			var l21 = Lines1.GetNextSet(l11);
			var l12 = HouseMask.TrailingZeroCount(Lines2);
			var l22 = Lines2.GetNextSet(l12);
			var result = CellMap.Empty;
			foreach (var (a, b) in ((l11, l12), (l11, l22), (l21, l12), (l21, l22)))
			{
				result |= HousesMap[a] & HousesMap[b];
			}
			return result;
		}
	}


	/// <inheritdoc/>
	public bool Equals([NotNullWhen(true)] QiuDeadlyPattern2? other)
		=> other is not null && Lines1 == other.Lines1 && Lines2 == other.Lines2;
}
