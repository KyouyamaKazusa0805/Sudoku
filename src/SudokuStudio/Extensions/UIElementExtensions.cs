namespace Microsoft.UI.Xaml;

/// <summary>
/// Provides with extension methods on <see cref="UIElement"/>.
/// </summary>
/// <seealso cref="UIElement"/>
public static class UIElementExtensions
{
	/// <summary>
	/// Draws the specified UI control to the target file as a picture.
	/// </summary>
	/// <typeparam name="TUIElement">The type of the UI control.</typeparam>
	/// <typeparam name="TStorageFileOrRandomAccessStream">
	/// The type of the argument <paramref name="fileOrStream"/>.
	/// The target type can be <see cref="StorageFile"/> or <see cref="IRandomAccessStream"/>;
	/// otherwise, a <see cref="NotSupportedException"/> will be thrown.
	/// </typeparam>
	/// <param name="this">The control.</param>
	/// <param name="fileOrStream">
	/// The target instance. This value can be a <see cref="StorageFile"/> or <see cref="IRandomAccessStream"/>.
	/// </param>
	/// <exception cref="NotSupportedException">
	/// Throws when the argument <paramref name="fileOrStream"/> is not supported.
	/// </exception>
	public static async Task RenderToAsync<TUIElement, TStorageFileOrRandomAccessStream>(
		this TUIElement @this,
		TStorageFileOrRandomAccessStream fileOrStream
	)
		where TUIElement : UIElement
		where TStorageFileOrRandomAccessStream : class
	{
		// Gets the snapshot of the control.
		var rtb = new RenderTargetBitmap();
		await rtb.RenderAsync(@this);

		// Creates the pixel buffer.
		var pixelBuffer = await rtb.GetPixelsAsync();

		// Gets the DPI value.
#if true
		// using Windows.Win32;
		var hWnd = WindowNative.GetWindowHandle(((App)Application.Current).WindowManager.GetWindowForElement(@this));
		var dpi = PInvoke.GetDpiForWindow(new(hWnd)) / 96F;
#else
		// using System.Runtime.InteropServices;
		// using Windows.Graphics.Display;
		float dpi;
		try
		{
			dpi = DisplayInformation.GetForCurrentView().LogicalDpi;
		}
		catch (COMException ex) when (ex.ErrorCode == unchecked((int)0x80070490))
		{
			dpi = 96;
		}
#endif

		switch (fileOrStream)
		{
			case StorageFile file:
			{
				using var pictureFileStream = await file.OpenAsync(FileAccessMode.ReadWrite);

				// Creates an encoder.
				var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, pictureFileStream);
				setPixelData(encoder, rtb, pixelBuffer, dpi);

				// Flushes the encoder.
				// Please don't move 'FlushAsync' method invocation outside of this block,
				// because the using variable 'pictureFileStream' is created and bound with the encoder.
				// If we flush the encoder instance, this variable will also be handled. If we put this invocation
				// outside of this block, variable 'pictureFileStream' will be released when executed all code in this block,
				// and then 'encoder.FlushAsync()' won't find 'pictureFileStream', then a COMException will be thrown.
				await encoder.FlushAsync();

				break;
			}
			case IRandomAccessStream stream:
			{
				// Creates an encoder.
				var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
				setPixelData(encoder, rtb, pixelBuffer, dpi);

				// Flushes the encoder.
				await encoder.FlushAsync();

				break;
			}
			default:
			{
				throw new NotSupportedException($"The target type of argument '{nameof(fileOrStream)}' is not supported.");
			}
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void setPixelData(BitmapEncoder encoder, RenderTargetBitmap rtb, IBuffer pixelBuffer, float dpi)
			=> encoder.SetPixelData(
				BitmapPixelFormat.Bgra8,
				BitmapAlphaMode.Ignore,
				(uint)rtb.PixelWidth,
				(uint)rtb.PixelHeight,
				dpi,
				dpi,
				pixelBuffer.ToArray()
			);
	}
}
