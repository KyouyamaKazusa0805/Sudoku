namespace Sudoku.Text.Serialization;

/// <summary>
/// The internal color structure.
/// </summary>
/// <param name="A">The alpha value.</param>
/// <param name="R">The red value.</param>
/// <param name="G">The green value.</param>
/// <param name="B">The blue value.</param>
internal readonly record struct ColorInternal(byte A, byte R, byte G, byte B);
