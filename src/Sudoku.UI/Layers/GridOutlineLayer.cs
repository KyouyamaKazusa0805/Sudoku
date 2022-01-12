namespace Sudoku.UI.Layers;

/// <summary>
/// Defines the grid outline layer that only handles and draws the outlines of the grid,
/// i.e. the block outlines and the cell outlines.
/// </summary>
public sealed class GridOutlineLayer : IDrawable
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

		canvas.StrokeLineJoin = LineJoin.Miter;
		canvas.StrokeColor = Colors.Black;

		float gridSize = width - Offset * 2;
		float blockSize = gridSize / 3;
		float cellSize = gridSize / 9;
		//float candidateSize = gridSize / 27;

		// Draw block outlines.
		canvas.StrokeSize = 3F;
		for (int i = 0; i < 4; i++) // Row index.
		{
			for (int j = 0; j < 4; j++) // Column index.
			{
				canvas.DrawLine(Offset, Offset + blockSize * j, width - Offset, Offset + blockSize * j);
				canvas.DrawLine(Offset + blockSize * i, Offset, Offset + blockSize * i, width - Offset);
			}
		}

		// Draw cell outlines.
		canvas.StrokeSize = 1F;
		for (int i = 0; i < 10; i++) // Row index.
		{
			for (int j = 0; j < 10; j++) // Column index.
			{
				// We can omit the cascaded cell outlines
				// that has been already drawn as the block outlines.
				if (i % 3 == 0 || j % 3 == 0)
				{
					continue;
				}

				canvas.DrawLine(Offset, Offset + cellSize * j, width - Offset, Offset + cellSize * j);
				canvas.DrawLine(Offset + cellSize * i, Offset, Offset + cellSize * i, width - Offset);
			}
		}
	}
}
