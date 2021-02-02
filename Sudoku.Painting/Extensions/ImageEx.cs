using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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
		public static async Task<ImageSource> ToImageSourceAsync(this Image @this)
		{
			// Create a memory stream.
			var ms = new MemoryStream();

			// Then we should save the image (picture, that parameter) into the stream.
			// Here we must use 'ImageFormat.Png'.
			// The image (picture) is created by GDI+, so the image should be a 'Bitmap'.
			//
			// (File: Sudoku.Painting\GridPainter.cs)
			//
			// The image format should be 'ImageFormat.MemoryBmp'.
			// However, in library implementation, the code is as follows:
			//
			// ImageCodecInfo? encoder = FindEncoderForFormat(format);
			// if (encoder == null)
			//     throw new ArgumentException(SR.Format(SR.NoCodecAvailableForFormat, format.Guid));
			//
			// Here we can find that if 'encoder' is null, the following code will throw an exception.
			// While in the method 'FindEncoderForFormat', we can see this:
			//
			// if (format.Guid.Equals(ImageFormat.MemoryBmp.Guid))
			//     format = ImageFormat.Png;
			//
			// This can solve the problem here. If we set the image format to 'ImageFormat.MemoryBmp',
			// then the code will change the value to 'ImageFormat.Png'.
			// Therefore, no matter what kind of image format we chose, the return format is always
			// 'ImageFormat.Png'.
			// On the other hand, to my surprise, if we set 'ImageFormat.MemoryBmp' here,
			// we'll always get an exception that tells us "argument cannot be null.",
			// and the argument name is always "Arg_Param_Name", which makes me confused...
			// I suspect the problem is because the library is on WinUI: the handling is different
			// with the normal cases. Of course, this point of view is only in my opinion. Nonetheless,
			// 'ImageFormat.Png' does work and 'ImageFormat.MemoryBmp' doesn't.
			@this.Save(ms, ImageFormat.Png);

			// Rewind the stream pointer.
			ms.Position = 0;

			// Finally, set the resource to the image (control).
			var image = new BitmapImage();
			await image.SetSourceAsync(ms.AsRandomAccessStream());
			return image;
		}
	}
}
