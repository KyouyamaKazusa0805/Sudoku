namespace Sudoku.UI.Interoperability;

/// <summary>
/// Provides native methods on application window.
/// </summary>
internal static class AppWindowMarshal
{
	/// <summary>
	/// Queries the dots per inch (dpi) of a display.
	/// </summary>
	/// <param name="hmonitor">Handle of the monitor being queried.</param>
	/// <param name="dpiType">
	/// <para>The type of DPI being queried. Possible values are from the <see cref="Monitor_DPI_Type"/> enumeration.</para>
	/// <para>
	/// Docs reference: <see href="https://docs.microsoft.com/en-us/windows/desktop/api/shellscalingapi/ne-shellscalingapi-monitor_dpi_type">type <c>MONITOR_DPI_TYPE</c></see>.
	/// </para>
	/// </param>
	/// <param name="dpiX">
	/// The value of the DPI along the X axis.
	/// This value always refers to the horizontal edge, even when the screen is rotated.
	/// </param>
	/// <param name="dpiY">
	/// The value of the DPI along the Y axis.
	/// This value always refers to the vertical edge, even when the screen is rotated.
	/// </param>
	/// <returns>
	/// This function returns one of the following values.
	/// <list type="table">
	/// <listheader>
	/// <term>Return code</term>
	/// <description>Description</description>
	/// </listheader>
	/// <item>
	/// <term><c>S_OK</c></term>
	/// <description>The function successfully returns the X and Y DPI values for the specified monitor.</description>
	/// </item>
	/// <item>
	/// <term>E_INVALIDARG</term>
	/// <description>The handle, DPI type, or pointers passed in are not valid.</description>
	/// </item>
	/// </list>
	/// </returns>
	/// <remarks>
	/// <para>
	/// This API is not DPI aware and should not be used if the calling thread is per-monitor DPI aware.
	/// For the DPI-aware version of this API, see
	/// <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getdpiforwindow"><c>
	/// GetDpiForWindow
	/// </c></see>.
	/// </para>
	/// <para>
	/// When you call <c>GetDpiForMonitor</c>, you will receive different DPI values depending on the DPI awareness
	/// of the calling application. DPI awareness is an application-level property usually defined
	/// in the application manifest. For more information about DPI awareness values, see
	/// <see href="https://docs.microsoft.com/en-us/windows/desktop/api/shellscalingapi/ne-shellscalingapi-process_dpi_awareness"><c>
	/// PROCESS_DPI_AWARENESS
	/// </c></see>. The following table indicates how the results will differ based on the <c>PROCESS_DPI_AWARENESS</c>
	/// value of your application.
	/// <list type="table">
	/// <item>
	/// <term><c>PROCESS_DPI_UNAWARE</c></term>
	/// <description>96 because the app is unaware of any other scale factors.</description>
	/// </item>
	/// <item>
	/// <term><c>PROCESS_SYSTEM_DPI_AWARE</c></term>
	/// <description>
	/// A value set to the system DPI because the app assumes all applications use the system DPI.
	/// </description>
	/// </item>
	/// <item>
	/// <term><c>PROCESS_PER_MONITOR_DPI_AWARE</c></term>
	/// <description>The actual DPI value set by the user for that display.</description>
	/// </item>
	/// </list>
	/// </para>
	/// <para>
	/// The values of <c>*<paramref name="dpiX"/></c> and <c>*<paramref name="dpiY"/></c> are identical.
	/// You only need to record one of the values to determine the DPI and respond appropriately.
	/// </para>
	/// <para>
	/// When <see href="https://docs.microsoft.com/en-us/windows/desktop/api/shellscalingapi/ne-shellscalingapi-monitor_dpi_type"><c>
	/// MONITOR_DPI_TYPE
	/// </c></see> is <see cref="Monitor_DPI_Type.MDT_Angular_DPI"/> or <see cref="Monitor_DPI_Type.MDT_Raw_DPI"/>,
	/// the returned DPI value does not include any changes that the user made to the DPI
	/// by using the desktop scaling override slider control in Control Panel.
	/// </para>
	/// <para>
	/// For more information about DPI settings in Control Panel, see
	/// <see href="https://docs.microsoft.com/en-us/windows/win32/hidpi/high-dpi-desktop-application-development-on-windows">
	/// the Writing DPI-Aware Desktop Applications in Windows 8.1 Preview white paper
	/// </see>.
	/// </para>
	/// </remarks>
	/// <seealso href="https://docs.microsoft.com/en-us/windows/desktop/api/shellscalingapi/ne-shellscalingapi-monitor_dpi_type">Type <c>MONITOR_DPI_TYPE</c></seealso>
	[DllImport("Shcore", SetLastError = true)]
	internal static extern int GetDpiForMonitor(
		[In] IntPtr hmonitor,
		[In] Monitor_DPI_Type dpiType,
		[Out] out uint dpiX,
		[Out] out uint dpiY
	);
}
