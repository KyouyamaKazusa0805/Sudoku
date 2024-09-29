namespace Sudoku.Analytics.Construction.Patterns;

/// <summary>
/// Defines a pattern that is a Qiu's deadly pattern technique pattern in theory. The sketch is like:
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
/// <item><term>P</term><description>Corner Cells.</description></item>
/// <item><term>S</term><description>Cross-line Cells.</description></item>
/// <item><term>B</term><description>Base-line Cells.</description></item>
/// </list>
/// </summary>
/// <param name="Corner">The corner cells that is <c>P</c> in that sketch.</param>
/// <param name="Lines">The base-line cells that is <c>B</c> in that sketch.</param>
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.Object_GetHashCode | TypeImplFlag.EqualityOperators)]
public sealed partial class QiuDeadlyPattern1(
	[PrimaryConstructorParameter] ref readonly CellMap Corner,
	[PrimaryConstructorParameter] HouseMask Lines
) : IEquatable<QiuDeadlyPattern1>, IEqualityOperators<QiuDeadlyPattern1, QiuDeadlyPattern1, bool>
{
	/// <summary>
	/// Indicates the crossline cells.
	/// </summary>
	public CellMap Crossline
	{
		get
		{
			var l1 = HouseMask.TrailingZeroCount(Lines);
			var l2 = Lines.GetNextSet(l1);
			return (HousesMap[l1] | HousesMap[l2]) & PeersMap[Corner[0]] | (HousesMap[l1] | HousesMap[l2]) & PeersMap[Corner[1]];
		}
	}

	/// <summary>
	/// Indicates the mirror cells.
	/// </summary>
	public CellMap Mirror
	{
		get
		{
			Crossline.InOneHouse(out var block);
			var l1 = HouseMask.TrailingZeroCount(Lines);
			var l2 = Lines.GetNextSet(l1);
			return HousesMap[block] & ~(HousesMap[l1] | HousesMap[l2]);
		}
	}


	/// <inheritdoc/>
	public bool Equals([NotNullWhen(true)] QiuDeadlyPattern1? other)
		=> other is not null && Crossline == other.Crossline && Lines == other.Lines;
}
