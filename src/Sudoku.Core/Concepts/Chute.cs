namespace Sudoku.Concepts;

/// <summary>
/// Defines a chute.
/// </summary>
/// <param name="Index">Index of the chute. The value is between 0 and 6.</param>
/// <param name="Cells">The cells used.</param>
/// <param name="IsRow">Indicates whether the chute is in a mega-row.</param>
/// <param name="HousesMask">Indicates the houses used.</param>
public readonly record struct Chute(int Index, scoped ref readonly CellMap Cells, bool IsRow, HouseMask HousesMask) : ICoordinateObject<Chute>
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(CoordinateConverter converter) => converter.ChuteConverter([this]);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Chute ParseExact(string str, CoordinateParser parser)
		=> parser.ChuteParser(str) is [var result] ? result : throw new FormatException("Multiple chute values found.");
}
