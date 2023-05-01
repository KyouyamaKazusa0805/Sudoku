namespace Microsoft.UI.Xaml;

/// <summary>
/// Provides with extension methods on <see cref="UIElement"/>.
/// </summary>
/// <seealso cref="UIElement"/>
public static class UIElementExtensions
{
	/// <summary>
	/// Renders the specified UI control to the target file as a picture.
	/// </summary>
	/// <typeparam name="TUIElement">The type of the UI control.</typeparam>
	/// <typeparam name="TStorageFileOrRandomAccessStream">
	/// The type of the argument <paramref name="storageFileOrRandomAccessStream"/>.
	/// The target type can be <see cref="StorageFile"/> or <see cref="IRandomAccessStream"/>;
	/// otherwise, a <see cref="NotSupportedException"/> will be thrown.
	/// </typeparam>
	/// <param name="this">The control.</param>
	/// <param name="storageFileOrRandomAccessStream">
	/// The target instance. This value can be a <see cref="StorageFile"/> or <see cref="IRandomAccessStream"/>.
	/// </param>
	/// <param name="scaledWidth">
	/// <inheritdoc cref="RenderTargetBitmap.RenderAsync(UIElement, int, int)" path="/param[@name='scaledWidth']"/>
	/// </param>
	/// <param name="scaledHeight">
	/// <inheritdoc cref="RenderTargetBitmap.RenderAsync(UIElement, int, int)" path="/param[@name='scaledHeight']"/>
	/// </param>
	/// <exception cref="NotSupportedException">
	/// Throws when the argument <paramref name="storageFileOrRandomAccessStream"/> is not supported.
	/// </exception>
	public static async Task RenderToAsync<TUIElement, TStorageFileOrRandomAccessStream>(
		this TUIElement @this,
		TStorageFileOrRandomAccessStream storageFileOrRandomAccessStream,
		int scaledWidth = 0,
		int scaledHeight = 0
	)
		where TUIElement : UIElement
		where TStorageFileOrRandomAccessStream : class
	{
		// Gets the snapshot of the control.
		var rtb = new RenderTargetBitmap();
		await rtb.RenderAsync(@this, scaledWidth, scaledHeight);

		// Creates the pixel buffer.
		var pixelBuffer = await rtb.GetPixelsAsync();

		// Gets the DPI value.
#if true
		var hWnd = WindowNative.GetWindowHandle(((App)Application.Current).WindowManager.GetWindowForElement(@this));
		var dpi = User32.GetDpiForWindow(hWnd) / 96F;
#else
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

		// Creates an encoder.
		switch (storageFileOrRandomAccessStream)
		{
			case StorageFile file:
			{
				using var pictureFileStream = await file.OpenAsync(FileAccessMode.ReadWrite);
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
				var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
				setPixelData(encoder, rtb, pixelBuffer, dpi);

				// Flushes the encoder.
				await encoder.FlushAsync();

				break;
			}
			default:
			{
				throw new NotSupportedException($"The target type of argument '{nameof(storageFileOrRandomAccessStream)}' is not supported.");
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
