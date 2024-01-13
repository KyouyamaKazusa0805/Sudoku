#if UI_FEATURE_CUSTOMIZED_TITLE_BAR

namespace SudokuStudio.Runtime.InteropServices;

/// <summary>
/// Identifies the dots per inch (dpi) setting for a monitor.
/// </summary>
/// <remarks>
/// <para>
/// All of these settings are affected by the
/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/shellscalingapi/ne-shellscalingapi-process_dpi_awareness">PROCESS_DPI_AWARENESS</see>
/// of your application.
/// </para>
/// <para><b>Requirements</b></para>
/// <para>
/// <list type="table">
/// <item>
/// <term>Minimum supported client</term>
/// <description>Windows 8.1 [desktop apps only]</description>
/// </item>
/// <item>
/// <term>Minimum supported server</term>
/// <description>Windows Server 2012 R2 [desktop apps only]</description>
/// </item>
/// <item>
/// <term>Header</term>
/// <description><c>shellscalingapi.h</c></description>
/// </item>
/// </list>
/// </para>
/// <para>
/// Source: <see href="https://learn.microsoft.com/en-us/windows/win32/api/shellscalingapi/ne-shellscalingapi-monitor_dpi_type">MONITOR_DPI_TYPE enumeration (shellscalingapi.h)</see>.
/// </para>
/// </remarks>
internal enum MonitorDpiType
{
	/// <summary>
	/// <para>Value: <i>0</i></para>
	/// <para>
	/// The effective DPI. This value should be used when determining the correct scale factor for scaling UI elements.
	/// This incorporates the scale factor set by the user for this specific display.
	/// </para>
	/// </summary>
	MDT_Effective_DPI = 0,

	/// <summary>
	/// <para>Value: <i>1</i></para>
	/// <para>
	/// The angular DPI. This DPI ensures rendering at a compliant angular resolution on the screen.
	/// This does not include the scale factor set by the user for this specific display.
	/// </para>
	/// </summary>
	MDT_Angular_DPI = 1,

	/// <summary>
	/// <para>Value: <i>2</i></para>
	/// <para>
	/// The raw DPI. This value is the linear DPI of the screen as measured on the screen itself.
	/// Use this value when you want to read the pixel density and not the recommended scaling setting.
	/// This does not include the scale factor set by the user for this specific display and is not guaranteed to be a supported DPI value.
	/// </para>
	/// </summary>
	MDT_Raw_DPI = 2,

	/// <summary>
	/// The default DPI setting for a monitor is <see cref="MDT_Effective_DPI"/>.
	/// </summary>
	MDT_Default = MDT_Effective_DPI
}
#endif