using System;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Graphics;

namespace Sudoku.Graphics;

/// <summary>
/// Indicates the type that creates a canvas to draw the sudoku grid.
/// </summary>
public sealed partial class GridCanvas
{
	/// <summary>
	/// The back field of the property <see cref="PictureSize"/>.
	/// </summary>
	/// <seealso cref="PictureSize"/>
	private float _pictureSize;

	/// <summary>
	/// The back field of the property <see cref="GridOutlineOffset"/>.
	/// </summary>
	/// <seealso cref="GridOutlineOffset"/>
	private float _gridOutlineOffset;


	/// <summary>
	/// Indicates the picture size indicating the area to allow drawing onto the screen.
	/// Of type <see cref="float"/>.
	/// </summary>
	/// <value>The value to set. The value must be greater than 0.</value>
	/// <returns>
	/// The current size of the picture that indicates the area to allow drawing onto the screen.
	/// </returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the value isn't greater than 0.</exception>
	public float PictureSize
	{
		get => _pictureSize;

		set
		{
			if (value <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(value));
			}

			if (_pictureSize.NearlyEquals(value))
			{
				return;
			}

			_pictureSize = value;
			PictureSizeChanged?.Invoke(this, new(value));
		}
	}

	/// <summary>
	/// Indicates the offset value that measures the length of pixels as the padding.
	/// </summary>
	/// <value>The value to set. The value must be greater than 0.</value>
	/// <returns>The current offset value that measures the length of pixels as the padding.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the value isn't greater than 0.</exception>
	public float GridOutlineOffset
	{
		get => _gridOutlineOffset;

		set
		{
			if (value <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(value));
			}

			if (_gridOutlineOffset.NearlyEquals(value))
			{
				return;
			}

			_gridOutlineOffset = value;
			GridOutlineOffsetChanged?.Invoke(this, new(value));
		}
	}

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
	/// Indicates the event that triggers when the property <see cref="PictureSize"/> is changed.
	/// </summary>
	public event PictureSizeChangedEventHandler? PictureSizeChanged;

	/// <summary>
	/// Indicates the event that triggers when the property <see cref="GridOutlineOffset"/> is changed.
	/// </summary>
	public event GridOutlineOffsetChangedEventHandler? GridOutlineOffsetChanged;


	/// <summary>
	/// Try to draw onto an <see cref="IImage"/>.
	/// </summary>
	/// <returns>The result.</returns>
	/// <seealso cref="IImage"/>
	public IPicture Draw()
	{
		using var canvas = new PictureCanvas(0, 0, PictureSize, PictureSize)
		{
			Antialias = true,
			BlendMode = BlendMode.Normal
		};

		DrawOutlines(canvas);

		return canvas.Picture;
	}


	partial void DrawOutlines(ICanvas canvas);
}
