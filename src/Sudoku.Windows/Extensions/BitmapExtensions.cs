using I = System.Windows.Interop.Imaging;

namespace System.Drawing;

/// <summary>
/// Provides extension methods on <see cref="Bitmap"/>.
/// </summary>
/// <seealso cref="Bitmap"/>
internal static class BitmapExtensions
{
	/// <summary>
	/// Convert the <see cref="Bitmap"/> to <see cref="ImageSource"/>.
	/// </summary>
	/// <param name="this">The bitmap image.</param>
	/// <returns>The target image result.</returns>
	/// <seealso cref="ImageSource"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ImageSource ToImageSource(this Bitmap @this)
	{
		var hBitmap = @this.GetHbitmap();
		var result = I.CreateBitmapSourceFromHBitmap(
			bitmap: hBitmap,
			palette: IntPtr.Zero,
			sourceRect: Int32Rect.Empty,
			sizeOptions: BitmapSizeOptions.FromEmptyOptions()
		);
		delete(hBitmap);
		return result;


		[method: DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool delete([In] IntPtr hObject);
	}
}
