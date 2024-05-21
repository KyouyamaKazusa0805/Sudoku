namespace Sudoku.Concepts.Formatting;

/// <summary>
/// Represents a delegate type that creates a <see cref="string"/> value via the specified <see cref="Mask"/> instance.
/// </summary>
/// <param name="digitsMask">A list of digits.</param>
/// <returns>An equivalent <see cref="string"/> value to the specified argument <paramref name="digitsMask"/>.</returns>
public delegate string DigitNotationConverter(Mask digitsMask);
