namespace SudokuStudio.Runtime.InteropServices;

/// <summary>
/// Provides with a set of methods that is defined as native methods (written by other programming languages such as C++).
/// </summary>
internal static partial class Interoperability
{
	[LibraryImport("Microsoft.ui.xaml")]
	public static partial void XamlCheckProcessRequirements();

#if UI_FEATURE_CUSTOMIZED_TITLE_BAR
	[LibraryImport("Shcore", SetLastError = true)]
	public static partial int GetDpiForMonitor(nint hmonitor, MonitorDpiType dpiType, out uint dpiX, out uint dpiY);

	[DllImport("CoreMessaging")]
	public static extern int CreateDispatcherQueueController(
		[In] DispatcherQueueOptions options,
		[In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object? dispatcherQueueController
	);
#endif
}
