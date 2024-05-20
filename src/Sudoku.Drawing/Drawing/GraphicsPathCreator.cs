namespace Sudoku.Drawing;

/// <summary>
/// Represents a <see cref="GraphicsPath"/> creator method.
/// </summary>
/// <param name="x">Indicates the x.</param>
/// <param name="y">Indicaets the y.</param>
/// <returns>The <see cref="GraphicsPath"/> instance.</returns>
internal delegate GraphicsPath GraphicsPathCreator(float x, float y);
