using System.Runtime.CompilerServices;
using Microsoft.Maui.Graphics;

namespace Sudoku.Graphics;

/// <summary>
/// Indicates the type that creates a canvas to draw the sudoku grid.
/// </summary>
/// <param name="PictureSize">
/// Indicates the picture size indicating the area to allow drawing onto the screen.
/// Of type <see cref="float"/>.
/// </param>
/// <param name="GridOutlineOffset">
/// Indicates the offset value that measures the length of pixels as the padding.
/// </param>
public readonly partial record struct GridCanvas(float PictureSize, float GridOutlineOffset)
{
	/// <summary>
	/// Indicates the size of the drawing field.
	/// </summary>
	public RectangleF DrawingField
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new(0, 0, PictureSize, PictureSize);
	}

	/// <summary>
	/// Indicates the size of the grid. Of type <see cref="float"/>.
	/// </summary>
	private float GridSize
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => PictureSize - 2 * GridOutlineOffset;
	}

	/// <summary>
	/// Indicates the size of each block. Of type <see cref="float"/>.
	/// </summary>
	private float BlockSize
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => GridSize / 3;
	}

	/// <summary>
	/// Indicates the size of each cell. Of type <see cref="float"/>.
	/// </summary>
	private float CellSize
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => GridSize / 9;
	}

	/// <summary>
	/// Indicates the size of each candidate. Of type <see cref="float"/>.
	/// </summary>
	private float CandidateSize
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => GridSize / 27;
	}


	/// <summary>
	/// Try to draw.
	/// </summary>
	/// <param name="canvas">Indicates the canvas used for drawing.</param>
	public void Draw(ICanvas canvas)
	{
		canvas.Antialias = true;
		canvas.BlendMode = BlendMode.Normal;

		DrawOutlines(canvas);
	}


	partial void DrawOutlines(ICanvas canvas);
}
