using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Sudoku.Drawing.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Bitmap"/>.
	/// </summary>
	/// <seealso cref="Bitmap"/>
	public static class BitmapEx
	{
		/// <summary>
		/// Convert the <see cref="Bitmap"/> to <see cref="ImageSource"/>.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The bitmap image.</param>
		/// <returns>The target image result.</returns>
		/// <seealso cref="ImageSource"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImageSource ToImageSource(this Bitmap @this)
		{
			var hBitmap = @this.GetHbitmap();
			var result = Imaging.CreateBitmapSourceFromHBitmap(
				hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
			DeleteObject(hBitmap);
			return result;
		}

		/// <summary>
		/// Delete the object.
		/// </summary>
		/// <param name="hObject">The handle of the object.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		[method: DllImport("gdi32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool DeleteObject([In] IntPtr hObject);
	}
}
