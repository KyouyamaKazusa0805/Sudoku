namespace Sudoku.UI.Interoperability;

/// <summary>
/// Identifies the dots per inch (dpi) setting for a monitor.
/// </summary>
/// <remarks>
/// All of these settings are affected by the
/// <see href="https://docs.microsoft.com/en-us/windows/desktop/api/shellscalingapi/ne-shellscalingapi-process_dpi_awareness"><c>
/// PROCESS_DPI_AWARENESS
/// </c></see> of your application.
/// </remarks>
internal enum Monitor_DPI_Type
{
	/// <summary>
	/// The effective DPI. This value should be used when determining the correct scale factor for scaling UI elements.
	/// This incorporates the scale factor set by the user for this specific display.
	/// </summary>
	MDT_Effective_DPI = 0,

	/// <summary>
	/// The angular DPI. This DPI ensures rendering at a compliant angular resolution on the screen.
	/// This does not include the scale factor set by the user for this specific display.
	/// </summary>
	MDT_Angular_DPI = 1,

	/// <summary>
	/// The raw DPI. This value is the linear DPI of the screen as measured on the screen itself.
	/// Use this value when you want to read the pixel density and not the recommended scaling setting.
	/// This does not include the scale factor set by the user for this specific display and is not guaranteed
	/// to be a supported DPI value.
	/// </summary>
	MDT_Raw_DPI = 2,

	/// <summary>
	/// The default DPI setting for a monitor is <see cref="MDT_Effective_DPI"/>.
	/// </summary>
	MDT_Default = MDT_Effective_DPI
}
