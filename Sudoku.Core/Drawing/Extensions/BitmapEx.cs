#if SUDOKU_RECOGNIZING
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using static Sudoku.Extensions.TypeEx;

namespace Sudoku.Drawing.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Bitmap"/>.
	/// </summary>
	/// <remarks>
	/// Some methods of this file is copied by the <see cref="Emgu.CV"/>'s
	/// <a href="https://github.com/emgucv/emgucv/blob/6ee487ad2709d1258cc014103deab2719b026303/Emgu.CV.NativeImage/BitmapExtension.cs">site</a>.
	/// </remarks>
	/// <seealso cref="Bitmap"/>
	[DebuggerStepThrough]
	public static class BitmapEx
	{
		/// <summary>
		/// Co-rect orientation.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The bitmap.</param>
		public static void CorectOrientation(this Bitmap @this)
		{
			if (Array.IndexOf(@this.PropertyIdList, 274) > -1)
			{
				switch (@this.GetPropertyItem(274).Value[0])
				{
					case 1:
					{
						// No rotation required.
						break;
					}
					case 2:
					{
						@this.RotateFlip(RotateFlipType.RotateNoneFlipX);
						break;
					}
					case 3:
					{
						@this.RotateFlip(RotateFlipType.Rotate180FlipNone);
						break;
					}
					case 4:
					{
						@this.RotateFlip(RotateFlipType.Rotate180FlipX);
						break;
					}
					case 5:
					{
						@this.RotateFlip(RotateFlipType.Rotate90FlipX);
						break;
					}
					case 6:
					{
						@this.RotateFlip(RotateFlipType.Rotate90FlipNone);
						break;
					}
					case 7:
					{
						@this.RotateFlip(RotateFlipType.Rotate270FlipX);
						break;
					}
					case 8:
					{
						@this.RotateFlip(RotateFlipType.Rotate270FlipNone);
						break;
					}
				}

				// This EXIF data is now invalid and should be removed.
				@this.RemovePropertyItem(274);
			}
		}

		/// <summary>
		/// Create an Image &lt; TColor, TDepth &gt; from Bitmap
		/// </summary>
		public static Image<TColor, TDepth> ToImage<TColor, TDepth>(this Bitmap bitmap)
			where TColor : struct, IColor
			where TDepth : new()
		{
			var size = bitmap.Size;
			var image = new Image<TColor, TDepth>(size);

			switch (bitmap.PixelFormat)
			{
				case PixelFormat.Format32bppRgb:
				{
					if (TypeEquals<TColor, Bgr>() && TypeEquals<TDepth, byte>())
					{
						var data = bitmap.LockBits(
							new Rectangle(Point.Empty, size),
							ImageLockMode.ReadOnly,
							bitmap.PixelFormat);

						using (var mat = new Image<Bgra, byte>(size.Width, size.Height, data.Stride, data.Scan0))
						{
							CvInvoke.MixChannels(mat, image, new[] { 0, 0, 1, 1, 2, 2 });
						}

						bitmap.UnlockBits(data);
					}
					else
					{
						using var tmp = bitmap.ToImage<Bgr, byte>();
						image.ConvertFrom(tmp);
					}

					break;
				}
				case PixelFormat.Format32bppArgb:
				{
					if (TypeEquals<TColor, Bgra>() && TypeEquals<TDepth, byte>())
					{
						image.CopyFromBitmap(bitmap);
					}
					else
					{
						var data = bitmap.LockBits(
							new Rectangle(Point.Empty, size),
							ImageLockMode.ReadOnly,
							bitmap.PixelFormat);
						using (var tmp =
							new Image<Bgra, byte>(size.Width, size.Height, data.Stride, data.Scan0))
							image.ConvertFrom(tmp);
						bitmap.UnlockBits(data);
					}

					break;
				}
				case PixelFormat.Format8bppIndexed:
				{
					if (TypeEquals<TColor, Bgra>() && TypeEquals<TDepth, byte>())
					{
						ColorPaletteToLookupTable(
							bitmap.Palette, out var bTable, out var gTable, out var rTable, out var aTable);

						var data = bitmap.LockBits(
							new Rectangle(Point.Empty, size),
							ImageLockMode.ReadOnly,
							bitmap.PixelFormat);
						using var indexValue = new Image<Gray, byte>(size.Width, size.Height, data.Stride, data.Scan0);
						using var b = new Mat();
						using var g = new Mat();
						using var r = new Mat();
						using var a = new Mat();
						using var mv = new VectorOfMat(new Mat[] { b, g, r, a });
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
					}
					else
					{
						using var tmp = bitmap.ToImage<Bgra, byte>();
						image.ConvertFrom(tmp);
					}

					break;
				}
				case PixelFormat.Format24bppRgb:
				{
					if (TypeEquals<TColor, Bgr>() && TypeEquals<TDepth, byte>())
					{
						image.CopyFromBitmap(bitmap);
					}
					else
					{
						var data = bitmap.LockBits(
							new Rectangle(Point.Empty, size), ImageLockMode.ReadOnly, bitmap.PixelFormat);
						using var tmp = new Image<Bgr, byte>(size.Width, size.Height, data.Stride, data.Scan0);
						image.ConvertFrom(tmp);
						bitmap.UnlockBits(data);
					}

					break;
				}
				case PixelFormat.Format1bppIndexed:
				{
					if (TypeEquals<TColor, Gray>() && TypeEquals<TDepth, byte>())
					{
						int rows = size.Height;
						int cols = size.Width;
						var data = bitmap.LockBits(
							new Rectangle(Point.Empty, size), ImageLockMode.ReadOnly, bitmap.PixelFormat);

						int fullByteCount = cols >> 3;
						int partialBitCount = cols & 7;

						int mask = 1 << 7;

						long srcAddress = data.Scan0.ToInt64();
						byte[,,] imagedata = (image.Data as byte[,,])!;

						byte[] row = new byte[fullByteCount + (partialBitCount == 0 ? 0 : 1)];

						int v = 0;
						for (int i = 0; i < rows; i++, srcAddress += data.Stride)
						{
							Marshal.Copy((IntPtr)srcAddress, row, 0, row.Length);

							for (int j = 0; j < cols; j++, v <<= 1)
							{
								if ((j & 7) == 0)
								{
									//fetch the next byte 
									v = row[j >> 3];
								}

								imagedata[i, j, 0] = (v & mask) == 0 ? (byte)0 : (byte)255;
							}
						}
					}
					else
					{
						using var tmp = bitmap.ToImage<Gray, byte>();
						image.ConvertFrom(tmp);
					}

					break;
				}
				default:
				{
					using var temp = new Image<Bgra, byte>(size);
					byte[,,] data = temp.Data;
					for (int i = 0; i < size.Width; i++)
					{
						for (int j = 0; j < size.Height; j++)
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
			ColorPalette palette, out Matrix<byte> bTable,
			out Matrix<byte> gTable, out Matrix<byte> rTable, out Matrix<byte> aTable)
		{
			bTable = new Matrix<byte>(256, 1);
			gTable = new Matrix<byte>(256, 1);
			rTable = new Matrix<byte>(256, 1);
			aTable = new Matrix<byte>(256, 1);
			byte[,] bData = bTable.Data;
			byte[,] gData = gTable.Data;
			byte[,] rData = rTable.Data;
			byte[,] aData = aTable.Data;

			var colors = palette.Entries;
			for (int i = 0; i < colors.Length; i++)
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
		/// <param name="bmp">the bitmap to copy data from.</param>
		/// <param name="image">The image to copy data to.</param>
		private static void CopyFromBitmap<TColor, TDepth>(this Image<TColor, TDepth> image, Bitmap bmp)
			where TColor : struct, IColor
			where TDepth : new()
		{
			var data = bmp.LockBits(new Rectangle(Point.Empty, bmp.Size), ImageLockMode.ReadOnly, bmp.PixelFormat);
			using var mat = new Matrix<TDepth>(bmp.Height, bmp.Width, image.NumberOfChannels, data.Scan0, data.Stride);
			CvInvoke.cvCopy(mat.Ptr, image.Ptr, IntPtr.Zero);
			bmp.UnlockBits(data);
		}
	}
}
#endif