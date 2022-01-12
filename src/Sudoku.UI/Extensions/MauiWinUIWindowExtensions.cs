#if WINDOWS
using System;
using System.Runtime.Versioning;
using WinRT;
using static PInvoke.User32;

namespace Microsoft.Maui;

/// <summary>
/// Provides extension methods on <see cref="MauiWinUIWindow"/>.
/// </summary>
/// <seealso cref="MauiWinUIWindow"/>
public static class MauiWinUIWindowExtensions
{
	/// <summary>
	/// Gets the handle of the window.
	/// </summary>
	/// <param name="window">The window.</param>
	/// <returns>The handle.</returns>
	[SupportedOSPlatform("windows10.0.17763")]
	[UnsupportedOSPlatform("ios")]
	[UnsupportedOSPlatform("android")]
	[UnsupportedOSPlatform("maccatalyst")]
	public static IntPtr GetNativeWindowHandle(this MauiWinUIWindow window)
	{
		var nativeWindow = window.As<IWindowNative>();
		return nativeWindow.WindowHandle;
	}

	/// <summary>
	/// Sets the icon onto the window.
	/// </summary>
	/// <param name="window">The specified window.</param>
	/// <param name="fileName">The file name of the icon.</param>
	[SupportedOSPlatform("windows10.0.17763")]
	[UnsupportedOSPlatform("ios")]
	[UnsupportedOSPlatform("android")]
	[UnsupportedOSPlatform("maccatalyst")]
	public static void SetIcon(this MauiWinUIWindow window, string fileName)
	{
		var hwnd = window.GetNativeWindowHandle();
		var hIcon = LoadImage(IntPtr.Zero, fileName, ImageType.IMAGE_ICON, 16, 16, LoadImageFlags.LR_LOADFROMFILE);
		SendMessage(hwnd, WindowMessage.WM_SETICON, (IntPtr)0, hIcon);
	}
}
#endif