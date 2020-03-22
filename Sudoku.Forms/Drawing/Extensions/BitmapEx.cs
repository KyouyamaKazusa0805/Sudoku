using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
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
	[DebuggerStepThrough]
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
			return Imaging.CreateBitmapSourceFromHBitmap(
				@this.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
				BitmapSizeOptions.FromEmptyOptions());
		}
	}
}
