namespace Microsoft.UI.Xaml;

/// <summary>
/// Provides with extension methods on <see cref="UIElement"/>.
/// </summary>
/// <seealso cref="UIElement"/>
public static class UIElementExtensions
{
	/// <summary>
	/// Sets the property <see cref="UIElement.Visibility"/> with the specified value.
	/// </summary>
	/// <param name="this">The property.</param>
	/// <param name="visibility">The value.</param>
	/// <returns>The instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TUIElement WithVisibility<TUIElement>(this TUIElement @this, Visibility visibility)
		where TUIElement : UIElement
	{
		@this.Visibility = visibility;
		return @this;
	}

	/// <summary>
	/// Renders the specified UI control to the target file as a picture.
	/// </summary>
	/// <typeparam name="TUIElement">The type of the UI control.</typeparam>
	/// <param name="this">The control.</param>
	/// <param name="file">The target file.</param>
	public static async Task RenderToAsync<TUIElement>(this TUIElement @this, StorageFile file)
		where TUIElement : UIElement
	{
		// Render.
		var rtb = new RenderTargetBitmap();
		await rtb.RenderAsync(@this);

		// Creates the pixel buffer.
		var pixelBuffer = await rtb.GetPixelsAsync();

		// Gets the DPI value.
		float dpi = TryGetLogicalDpi();

		// Encodes the image to the selected file on disk.
		using var pictureFileStream = await file.OpenAsync(FileAccessMode.ReadWrite);
		var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, pictureFileStream);
		encoder.SetPixelData(
			BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)rtb.PixelWidth, (uint)rtb.PixelHeight,
			dpi, dpi, pixelBuffer.ToArray());

		// Flushes the encoder.
		await encoder.FlushAsync();
	}

	/// <summary>
	/// Renders the specified UI control to the target stream as a picture.
	/// </summary>
	/// <typeparam name="TUIElement">The type of the UI control.</typeparam>
	/// <param name="this">The control.</param>
	/// <param name="stream">The target stream.</param>
	public static async Task RenderToAsync<TUIElement>(this TUIElement @this, IRandomAccessStream stream)
		where TUIElement : UIElement
	{
		// Gets the snapshot of the control.
		var rtb = new RenderTargetBitmap();
		await rtb.RenderAsync(@this);

		// Creates the pixel buffer.
		var pixelBuffer = await rtb.GetPixelsAsync();

		// Gets the DPI value.
		float dpi = TryGetLogicalDpi();

		// Creates an encoder.
		var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
		encoder.SetPixelData(
			BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)rtb.PixelWidth, (uint)rtb.PixelHeight,
			dpi, dpi, pixelBuffer.ToArray());

		// Flushes the encoder.
		await encoder.FlushAsync();
	}

	/// <summary>
	/// Try to get the logical DPI value.
	/// </summary>
	/// <param name="default">The default DPI value. The default value is 96.</param>
	/// <returns>The DPI value to get.</returns>
	private static float TryGetLogicalDpi(float @default = 96)
	{
		float dpi;
		try
		{
			dpi = DisplayInformation.GetForCurrentView().LogicalDpi;
		}
		catch (COMException ex) when (ex.ErrorCode == unchecked((int)0x80070490))
		{
			// Cannot find the element.
			dpi = @default;
		}

		return dpi;
	}
}
