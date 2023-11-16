using System.Runtime.CompilerServices;
using static Sudoku.SolutionWideReadOnlyFields;

namespace Sudoku.Concepts;

/// <summary>
/// Provides extension methods on <see cref="HouseType"/>.
/// </summary>
/// <seealso cref="HouseType"/>
public static class HouseTypeExtensions
{
	/// <summary>
	/// Gets the row, column and block value and copies to the specified array that represents by a pointer
	/// of 3 elements, where the first element stores the block index, second element stores the row index
	/// and the third element stores the column index.
	/// </summary>
	/// <param name="cell">The cell. The available values must be between 0 and 80.</param>
	/// <param name="reference">
	/// The specified reference to the first element in a sequence. The sequence type can be an array or a <see cref="Span{T}"/>,
	/// only if the sequence can store at least 3 values.
	/// </param>
	/// <exception cref="ArgumentNullRefException">
	/// Throws when the argument <paramref name="reference"/> references to <see langword="null"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void CopyHouseInfo(this Cell cell, scoped ref House reference)
	{
		Ref.ThrowIfNullRef(in reference);

		reference = BlockTable[cell];
		Unsafe.AddByteOffset(ref reference, sizeof(House)) = RowTable[cell];
		Unsafe.AddByteOffset(ref reference, 2 * sizeof(House)) = ColumnTable[cell];
	}

	/// <summary>
	/// Get the house index (0..27 for block 1-9, row 1-9 and column 1-9)
	/// for the specified cell and the house type.
	/// </summary>
	/// <param name="cell">The cell. The available values must be between 0 and 80.</param>
	/// <param name="houseType">The house type.</param>
	/// <returns>The house index. The return value must be between 0 and 26.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="houseType"/> is not defined.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static House ToHouseIndex(this byte cell, HouseType houseType)
		=> houseType switch
		{
			HouseType.Block => BlockTable[cell],
			HouseType.Row => RowTable[cell],
			HouseType.Column => ColumnTable[cell],
			_ => throw new ArgumentOutOfRangeException(nameof(houseType))
		};

	/// <inheritdoc cref="ToHouseIndex(byte, HouseType)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static House ToHouseIndex(this Cell cell, HouseType houseType)
		=> houseType switch
		{
			HouseType.Block => BlockTable[cell],
			HouseType.Row => RowTable[cell],
			HouseType.Column => ColumnTable[cell],
			_ => throw new ArgumentOutOfRangeException(nameof(houseType))
		};

	/// <summary>
	/// Get the house indices for the specified cell.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <returns>A <see cref="HouseMask"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static HouseMask ToHouseIndices(this byte cell)
	{
		var result = 0;
		result |= cell.ToHouseIndex(HouseType.Block);
		result |= cell.ToHouseIndex(HouseType.Row);
		result |= cell.ToHouseIndex(HouseType.Column);

		return result;
	}

	/// <inheritdoc cref="ToHouseIndices(byte)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static HouseMask ToHouseIndices(this Cell cell)
	{
		var result = 0;
		result |= cell.ToHouseIndex(HouseType.Block);
		result |= cell.ToHouseIndex(HouseType.Row);
		result |= cell.ToHouseIndex(HouseType.Column);

		return result;
	}

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
