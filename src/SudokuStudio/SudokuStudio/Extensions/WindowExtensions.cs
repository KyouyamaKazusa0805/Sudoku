namespace Microsoft.UI.Xaml;

/// <summary>
/// Provides with extension methods on <see cref="Window"/>.
/// </summary>
public static class WindowExtensions
{
	/// <summary>
	/// Gets <see cref="AppWindow"/> instance for the current <see cref="Window"/> instance.
	/// </summary>
	/// <param name="this">The current <see cref="Window"/> instance.</param>
	/// <param name="hWnd">Indicates the handle of the window. The value is an <see cref="nint"/> integer value.</param>
	/// <param name="wndId">Indicates the handle of the window. The value is a <see cref="WindowId"/> sturcture instance.</param>
	/// <returns>A valid <see cref="AppWindow"/> instance.</returns>
	public static AppWindow GetAppWindow(this Window @this, out nint hWnd, out WindowId wndId)
	{
		hWnd = WindowNative.GetWindowHandle(@this);
		wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
		return AppWindow.GetFromWindowId(wndId);
	}
}
