namespace Sudoku.Drawing.Ocr;

/// <summary>
/// Provides extension methods on <see cref="Bitmap"/>.
/// </summary>
/// <remarks>
/// Some methods of this file is copied by 
/// <see href="https://github.com/emgucv/emgucv/blob/6ee487ad2709d1258cc014103deab2719b026303/Emgu.CV.NativeImage/BitmapExtension.cs">
/// the EmguCV's repository
/// </see>.
/// </remarks>
/// <seealso cref="Bitmap"/>
public static class ImageHandler
{
	/// <summary>
	/// Get true if contour is rectangle with angles within <c>[lowAngle, upAngle]</c> degree.
	/// The default case is <c>[75, 105]</c> given by <paramref name="lowerAngle"/> and
	/// <paramref name="upperAngle"/>.
	/// </summary>
	/// <param name="contour">The contour.</param>
	/// <param name="lowerAngle">The lower angle. The default value is <c>75</c>.</param>
	/// <param name="upperAngle">The upper angle. The default value is <c>105</c>.</param>
	/// <param name="ratio">The ratio. The default value is <c>.35</c>.</param>
	/// <returns>A <see cref="bool"/> value.</returns>
	public static bool IsRectangle(this PointF[] contour, int lowerAngle = 75, int upperAngle = 105, double ratio = .35)
	{
		if (contour.Length > 4)
		{
			return false;
		}

		var sides = (stackalloc[]
		{
			new LineSegment2DF(contour[0], contour[1]),
			new LineSegment2DF(contour[1], contour[3]),
			new LineSegment2DF(contour[2], contour[3]),
			new LineSegment2DF(contour[0], contour[2])
		});

		// Check angles between common sides.
		for (var j = 0; j < 4; j++)
		{
			var angle = Math.Abs(sides[(j + 1) % sides.Length].GetExteriorAngleDegree(sides[j]));
			if (angle < lowerAngle || angle > upperAngle)
			{
				return false;
			}
		}

		// Check ratio between all sides, it can't be more than allowed.
		for (var i = 0; i < 4; i++)
		{
			for (var j = 0; j < 4; j++)
			{
				if (sides[i].Length / sides[j].Length < ratio || sides[i].Length / sides[j].Length > 1 + ratio)
				{
					return false;
				}
			}
		}

		return true;
	}

	/// <summary>
	/// To correct the orientation.
	/// </summary>
	/// <param name="this">The bitmap.</param>
	/// <returns>Returns argument <paramref name="this"/> itself.</returns>
	public static Bitmap CorrectOrientation(this Bitmap @this)
	{
		if (Array.IndexOf(@this.PropertyIdList, 274) != -1)
		{
			if (
				@this.GetPropertyItem(274)!.Value![0] switch
				{
					1 => null, // No rotation required.
					2 => RotateFlipType.RotateNoneFlipX,
					3 => RotateFlipType.Rotate180FlipNone,
					4 => RotateFlipType.Rotate180FlipX,
					5 => RotateFlipType.Rotate90FlipX,
					6 => RotateFlipType.Rotate90FlipNone,
					7 => RotateFlipType.Rotate270FlipX,
					8 => RotateFlipType.Rotate270FlipNone,
					_ => default(RotateFlipType?)
				} is { } flipType
			)
			{
				@this.RotateFlip(flipType);
			}

			// This EXIF data is now invalid and should be removed.
			@this.RemovePropertyItem(274);
		}

		return @this;
	}

	/// <summary>
	/// Create an <see cref="Image{TColor, TDepth}"/> from <see cref="Bitmap"/>.
	/// </summary>
	/// <typeparam name="TColor">
	/// The type of the color model. The type should be implemented the interface <see cref="IColor"/>,
	/// and must be a <see langword="struct"/>.
	/// </typeparam>
	/// <typeparam name="TDepth">
	/// The type of the depth model. The only condition is the type should contain
	/// a parameterless constructor.
	/// </typeparam>
	/// <seealso cref="Image{TColor, TDepth}"/>
	/// <seealso cref="Bitmap"/>
	public static Image<TColor, TDepth> ToImage<TColor, TDepth>(this Bitmap bitmap) where TColor : struct, IColor where TDepth : new()
	{
		var size = bitmap.Size;
		var image = new Image<TColor, TDepth>(size);

		switch (bitmap.PixelFormat)
		{
			case PixelFormat.Format32bppRgb when colorIs<Bgr>() && depthIs<byte>():
			{
				var data = bitmap.LockBits(new(Point.Empty, size), ImageLockMode.ReadOnly, bitmap.PixelFormat);
				using var mat = new Image<Bgra, byte>(size.Width, size.Height, data.Stride, data.Scan0);
				CvInvoke.MixChannels(mat, image, [0, 0, 1, 1, 2, 2]);
				bitmap.UnlockBits(data);
				break;
			}
			case PixelFormat.Format32bppRgb:
			{
				using var tmp = bitmap.ToImage<Bgr, byte>();
				image.ConvertFrom(tmp);
				break;
			}
			case PixelFormat.Format32bppArgb when colorIs<Bgra>() && depthIs<byte>():
			{
				image.CopyFromBitmap(bitmap);
				break;
			}
			case PixelFormat.Format32bppArgb:
			{
				var data = bitmap.LockBits(new(Point.Empty, size), ImageLockMode.ReadOnly, bitmap.PixelFormat);

				using var tmp = new Image<Bgra, byte>(size.Width, size.Height, data.Stride, data.Scan0);
				image.ConvertFrom(tmp);
				bitmap.UnlockBits(data);
				break;
			}
			case PixelFormat.Format8bppIndexed when colorIs<Bgra>() && depthIs<byte>():
			{
				ColorPaletteToLookupTable(bitmap.Palette, out var bTable, out var gTable, out var rTable, out var aTable);

				var data = bitmap.LockBits(new(Point.Empty, size), ImageLockMode.ReadOnly, bitmap.PixelFormat);
				using var indexValue = new Image<Gray, byte>(size.Width, size.Height, data.Stride, data.Scan0);
				using Mat a = new(), r = new(), g = new(), b = new();
				using var mv = new VectorOfMat([b, g, r, a]);
				try
				{
					CvInvoke.LUT(indexValue, bTable, b);
					CvInvoke.LUT(indexValue, gTable, g);
					CvInvoke.LUT(indexValue, rTable, r);
					CvInvoke.LUT(indexValue, aTable, a);
					CvInvoke.Merge(mv, image);

					bitmap.UnlockBits(data);
				}
				finally
				{
					bTable?.Dispose();
					gTable?.Dispose();
					rTable?.Dispose();
					aTable?.Dispose();
				}
				break;
			}
			case PixelFormat.Format8bppIndexed:
			{
				using var tmp = bitmap.ToImage<Bgra, byte>();
				image.ConvertFrom(tmp);
				break;
			}
			case PixelFormat.Format24bppRgb when colorIs<Bgr>() && depthIs<byte>():
			{
				image.CopyFromBitmap(bitmap);
				break;
			}
			case PixelFormat.Format24bppRgb:
			{
				var data = bitmap.LockBits(new(Point.Empty, size), ImageLockMode.ReadOnly, bitmap.PixelFormat);
				using var tmp = new Image<Bgr, byte>(size.Width, size.Height, data.Stride, data.Scan0);
				image.ConvertFrom(tmp);
				bitmap.UnlockBits(data);
				break;
			}
			case PixelFormat.Format1bppIndexed when colorIs<Gray>() && depthIs<byte>():
			{
				var rows = size.Height;
				var cols = size.Width;
				var data = bitmap.LockBits(new(Point.Empty, size), ImageLockMode.ReadOnly, bitmap.PixelFormat);
				var fullByteCount = cols >> 3;
				var partialBitCount = cols & 7;
				const int mask = 1 << 7;
				var srcAddress = data.Scan0.ToInt64();
				var imageData = (image.Data as byte[,,])!;
				var row = new byte[fullByteCount + (partialBitCount == 0 ? 0 : 1)];
				var v = 0;
				for (var i = 0; i < rows; i++, srcAddress += data.Stride)
				{
					Marshal.Copy((nint)srcAddress, row, 0, row.Length);
					for (var j = 0; j < cols; j++, v <<= 1)
					{
						if ((j & 7) == 0)
						{
							//fetch the next byte 
							v = row[j >> 3];
						}

						imageData[i, j, 0] = (v & mask) == 0 ? byte.MinValue : byte.MaxValue;
					}
				}
				break;
			}
			case PixelFormat.Format1bppIndexed:
			{
				using var tmp = bitmap.ToImage<Gray, byte>();
				image.ConvertFrom(tmp);
				break;
			}
			default:
			{
				using var temp = new Image<Bgra, byte>(size);
				var data = temp.Data;
				for (var (i, width) = (0, size.Width); i < width; i++)
				{
					for (var (j, height) = (0, size.Height); j < height; j++)
					{
						var color = bitmap.GetPixel(i, j);
						data[j, i, 0] = color.B;
						data[j, i, 1] = color.G;
						data[j, i, 2] = color.R;
						data[j, i, 3] = color.A;
					}
				}
				image.ConvertFrom(temp);
				break;
			}
		}

		return image;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool colorIs<T>() where T : struct, IColor, allows ref struct => typeof(TColor) == typeof(T);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool depthIs<T>() where T : new(), allows ref struct => typeof(TDepth) == typeof(T);
	}

	/// <summary>
	/// Convert the color palette to four lookup tables.
	/// </summary>
	/// <param name="palette">The color palette to transform.</param>
	/// <param name="bTable">Lookup table for the B channel.</param>
	/// <param name="gTable">Lookup table for the G channel.</param>
	/// <param name="rTable">Lookup table for the R channel.</param>
	/// <param name="aTable">Lookup table for the A channel.</param>
	public static void ColorPaletteToLookupTable(
		ColorPalette palette,
		out Matrix<byte> bTable,
		out Matrix<byte> gTable,
		out Matrix<byte> rTable,
		out Matrix<byte> aTable
	)
	{
		bTable = new(256, 1);
		gTable = new(256, 1);
		rTable = new(256, 1);
		aTable = new(256, 1);
		var bData = bTable.Data;
		var gData = gTable.Data;
		var rData = rTable.Data;
		var aData = aTable.Data;
		var colors = palette.Entries;
		for (var (i, length) = (0, colors.Length); i < length; i++)
		{
			var c = colors[i];
			bData[i, 0] = c.B;
			gData[i, 0] = c.G;
			rData[i, 0] = c.R;
			aData[i, 0] = c.A;
		}
	}

	/// <summary>
	/// Utility function for converting <see cref="Bitmap"/> to <see cref="Image"/>.
	/// </summary>
	/// <param name="image">The image to copy data to.</param>
	/// <param name="bmp">the bitmap to copy data from.</param>
	private static void CopyFromBitmap<TColor, TDepth>(this Image<TColor, TDepth> image, Bitmap bmp)
		where TColor : struct, IColor
		where TDepth : new()
	{
		var data = bmp.LockBits(new(Point.Empty, bmp.Size), ImageLockMode.ReadOnly, bmp.PixelFormat);
		using var mat = new Matrix<TDepth>(bmp.Height, bmp.Width, image.NumberOfChannels, data.Scan0, data.Stride);
		CvInvoke.cvCopy(mat.Ptr, image.Ptr, 0);
		bmp.UnlockBits(data);
	}
}
