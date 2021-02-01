using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

namespace Sudoku.Painting.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Image"/>.
	/// </summary>
	/// <seealso cref="Image"/>
	public static class ImageEx
	{
		/// <summary>
		/// Convert the <see cref="Image"/> to <see cref="ImageSource"/>.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The image.</param>
		/// <returns>The target image result.</returns>
		/// <seealso cref="ImageSource"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImageSource ToImageSource(this Image @this)
		{
			// Save the bitmap.
			var ms = new MemoryStream();
			@this.Save(ms, ImageFormat.Bmp);

			var image = new BitmapImage();
			image.SetSource(ms.AsRandomAccessStream());
			return image;
		}
	}
}
