namespace Sudoku.Text.Converters;

/// <summary>
/// Represents a delegate type that creates a <see cref="string"/> value via the specified <see cref="HouseMask"/> instance.
/// </summary>
/// <param name="housesMask">A list of houses.</param>
/// <returns>An equivalent <see cref="string"/> value to the specified argument <paramref name="housesMask"/>.</returns>
public delegate string HouseNotationConverter(HouseMask housesMask);
