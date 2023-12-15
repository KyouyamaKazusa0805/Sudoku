using System.Runtime.CompilerServices;

namespace Sudoku.Concepts;

/// <summary>
/// Provides extension methods on <see cref="HouseType"/>.
/// </summary>
/// <seealso cref="HouseType"/>
public static class HouseTypeExtensions
{
	/// <summary>
	/// Try to get the label of the specified house type.
	/// </summary>
	/// <param name="this">The house type.</param>
	/// <returns>A character that represents a house type.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument is not defined.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static char GetLabel(this HouseType @this)
		=> @this switch
		{
			HouseType.Row => 'r',
			HouseType.Column => 'c',
			HouseType.Block => 'b',
			_ => throw new ArgumentOutOfRangeException(nameof(@this))
		};

	/// <summary>
	/// Gets the ordering of the house type. The result value will be 0, 1 and 2.
	/// </summary>
	/// <param name="this">The house type.</param>
	/// <returns>The program order.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static int GetProgramOrder(this HouseType @this)
		=> @this switch { HouseType.Block => 2, HouseType.Row => 0, HouseType.Column => 1 };
}
