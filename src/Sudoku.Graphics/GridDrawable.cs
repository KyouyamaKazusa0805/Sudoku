using System;
using Microsoft.Maui.Graphics;

namespace Sudoku.Graphics;

/// <summary>
/// Defines a type that draws the grid basic displaying items (such as the grid outines,
/// grid digits and so on).
/// </summary>
public sealed class GridDrawable : IDrawable
{
	/// <summary>
	/// Indicates the outline offset.
	/// </summary>
	public float Offset { get; set; }


	/// <inheritdoc/>
	public void Draw(ICanvas canvas, RectangleF dirtyRect)
	{
		const float epsilon = 1E-3F;

		_ = dirtyRect is { Width: var width, Height: var height };
		if (width.NearlyEquals(0, epsilon) || height.NearlyEquals(0, epsilon))
		{
			throw new ArgumentException("The width or height value is too small.");
		}

		if (!width.NearlyEquals(height))
		{
			throw new ArgumentException("The drawing must require the width equals to the height.");
		}

		new GridCanvas(width, Offset).Draw(canvas);
	}
}
