using Microsoft.Maui.Graphics;

namespace Sudoku.Graphics;

partial record struct GridCanvas
{
	/// <summary>
	/// Draw the grid, block and cell outlines.
	/// </summary>
	partial void DrawOutlines(ICanvas canvas)
	{
		canvas.StrokeLineJoin = LineJoin.Miter;
		canvas.StrokeColor = Colors.Black;

		// Draw block outlines.
		canvas.StrokeSize = 3F;
		for (int i = 0; i < 4; i++) // Row index.
		{
			for (int j = 0; j < 4; j++) // Column index.
			{
				canvas.DrawLine(GridOutlineOffset, GridOutlineOffset + BlockSize * j, PictureSize - GridOutlineOffset, GridOutlineOffset + BlockSize * j);
				canvas.DrawLine(GridOutlineOffset + BlockSize * i, GridOutlineOffset, GridOutlineOffset + BlockSize * i, PictureSize - GridOutlineOffset);
			}
		}

		// Draw cell outlines.
		canvas.StrokeSize = 1F;
		for (int i = 0; i < 10; i++) // Row index.
		{
			for (int j = 0; j < 10; j++) // Column index.
			{
				// We can skip the cascaded cell outlines having been already drawn as the block outlines.
				if (i % 3 == 0 || j % 3 == 0)
				{
					continue;
				}

				canvas.DrawLine(GridOutlineOffset, GridOutlineOffset + CellSize * j, PictureSize - GridOutlineOffset, GridOutlineOffset + CellSize * j);
				canvas.DrawLine(GridOutlineOffset + CellSize * i, GridOutlineOffset, GridOutlineOffset + CellSize * i, PictureSize - GridOutlineOffset);
			}
		}
	}
}
