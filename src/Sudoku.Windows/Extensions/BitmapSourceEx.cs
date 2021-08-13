namespace Sudoku.Windows.Extensions;

/// <summary>
/// Provides extension methods on <see cref="BitmapSource"/>.
/// </summary>
/// <seealso cref="BitmapSource"/>
public static class BitmapSourceEx
{
	/// <summary>
	/// Get the <see cref="BitmapImage"/> from the specified <see cref="BitmapSource"/>.
	/// </summary>
	/// <param name="this">The bitmap source.</param>
	/// <returns>The image.</returns>
	public static BitmapImage GetBitmapImage(this BitmapSource @this)
	{
		var encoder = new JpegBitmapEncoder();
		using var memoryStream = new MemoryStream();
		var bImg = new BitmapImage();

		encoder.Frames.Add(BitmapFrame.Create(@this));
		encoder.Save(memoryStream);

		memoryStream.Position = 0;
		bImg.BeginInit();
		bImg.StreamSource = memoryStream;
		bImg.EndInit();

		return bImg;
	}
}
