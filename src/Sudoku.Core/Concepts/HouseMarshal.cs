using System.Runtime.CompilerServices;

namespace Sudoku.Concepts;

/// <summary>
/// Represents a list of extension methods that operates with house instances, as <see cref="House"/> representation.
/// </summary>
/// <seealso cref="House"/>
public static class HouseMarshal
{
	/// <summary>
	/// Get the house type for the specified house index.
	/// </summary>
	/// <param name="houseIndex">The house index.</param>
	/// <returns>
	/// The house type. The possible return values are:
	/// <list type="table">
	/// <listheader>
	/// <term>House indices</term>
	/// <description>Return value</description>
	/// </listheader>
	/// <item>
	/// <term><paramref name="houseIndex"/> is <![CDATA[>= 0 and < 9]]></term>
	/// <description><see cref="HouseType.Block"/></description>
	/// </item>
	/// <item>
	/// <term><paramref name="houseIndex"/> is <![CDATA[>= 9 and < 18]]></term>
	/// <description><see cref="HouseType.Row"/></description>
	/// </item>
	/// <item>
	/// <term><paramref name="houseIndex"/> is <![CDATA[>= 18 and < 27]]></term>
	/// <description><see cref="HouseType.Column"/></description>
	/// </item>
	/// </list>
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static HouseType ToHouseType(this House houseIndex) => (HouseType)(houseIndex / 9);
}
