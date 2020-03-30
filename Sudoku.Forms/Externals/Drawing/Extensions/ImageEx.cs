using System.Diagnostics;
using System.Drawing;

namespace Sudoku.Drawing.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Image"/>.
	/// </summary>
	/// <seealso cref="Image"/>
	[DebuggerStepThrough]
	public static class ImageEx
	{
		/// <summary>
		/// Clip the image through a <see cref="Rectangle"/> range.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The image to clip.</param>
		/// <param name="rectangle">The rectangle range.</param>
		/// <returns>
		/// The image clipped. If failed to process, this value will be <see langword="null"/>.
		/// </returns>
		public static Image? ClipImageOn(this Image @this, Rectangle rectangle)
		{
			if (@this.Width < rectangle.X + rectangle.Width || @this.Height < rectangle.Y + rectangle.Height)
			{
				return null;
			}

			var bitmap = new Bitmap(@this);
			var result = bitmap.Clone(rectangle, @this.PixelFormat);
			bitmap.Dispose();
			return result;
		}
	}
}
