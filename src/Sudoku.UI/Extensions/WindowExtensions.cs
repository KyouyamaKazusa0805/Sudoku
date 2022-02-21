using Microsoft.UI.Windowing;
using WinRT.Interop;

namespace Microsoft.UI.Xaml;

/// <summary>
/// Provides extension methods on <see cref="Window"/>.
/// </summary>
/// <seealso cref="Window"/>
internal static class WindowExtensions
{
	/// <summary>
	/// Gets the <see cref="AppWindow"/> instance via the specified <see cref="Window"/>.
	/// </summary>
	/// <typeparam name="TWindow">The type of the window.</typeparam>
	/// <param name="this">The <typeparamref name="TWindow"/>-typed instance.</param>
	/// <returns>The <see cref="AppWindow"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static AppWindow GetAppWindow<TWindow>(this TWindow @this) where TWindow : Window
	{
		var hWnd = WindowNative.GetWindowHandle(@this);
		var wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
		return AppWindow.GetFromWindowId(wndId);
	}
}
