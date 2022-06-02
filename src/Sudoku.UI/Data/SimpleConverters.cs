namespace Sudoku.UI.Data;

/// <summary>
/// Provides with a set of methods as simple converters that can be used and called by XAML files.
/// </summary>
internal static class SimpleConverters
{
	/// <summary>
	/// Indicates the license displaying value on <see cref="RepositoryInfo.OpenSourceLicense"/>.
	/// </summary>
	/// <param name="input">The license name.</param>
	/// <returns>The converted result string.</returns>
	/// <seealso cref="RepositoryInfo.OpenSourceLicense"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string License(string input) => $"{input} {Get("AboutPage_License")}";

	/// <summary>
	/// Indicates the conversion on <see cref="RepositoryInfo.IsForReference"/>.
	/// </summary>
	/// <param name="input">The input value.</param>
	/// <returns>The converted result string value.</returns>
	/// <seealso cref="RepositoryInfo.IsForReference"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ForReference(bool input) => input ? Get("AboutPage_ForReference") : string.Empty;

	/// <summary>
	/// Gets the title of the info bar via its severity.
	/// </summary>
	/// <param name="severity">The severity.</param>
	/// <returns>The title of the info bar.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the severity is not defined.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string InfoBarTitle(InfoBarSeverity severity)
		=> Get(
			severity switch
			{
				InfoBarSeverity.Informational => "SudokuPage_InfoBar_SeverityInfo",
				InfoBarSeverity.Success => "SudokuPage_InfoBar_SeveritySuccess",
				InfoBarSeverity.Warning => "SudokuPage_InfoBar_SeverityWarning",
				InfoBarSeverity.Error => "SudokuPage_InfoBar_SeverityError",
				_ => throw new ArgumentOutOfRangeException(nameof(severity))
			}
		);

	public static string SliderPossibleValueString(double min, double max, double stepFrequency, double tickFrequency)
		=> $"{Get("SliderPossibleValue")}{min:0.0} - {max:0.0}{Get("SliderStepFrequency")}{stepFrequency:0.0}{Get("SliderTickFrequency")}{tickFrequency:0.0}";

	public static string SliderPossibleValueStringWithFormat(double min, double max, double stepFrequency, double tickFrequency, string format)
		=> $"{Get("SliderPossibleValue")}{min.ToString(format)} - {max.ToString(format)}{Get("SliderStepFrequency")}{stepFrequency.ToString(format)}{Get("SliderTickFrequency")}{tickFrequency.ToString(format)}";

	public static Visibility StringToVisibility(string? s)
		=> string.IsNullOrWhiteSpace(s) ? Visibility.Collapsed : Visibility.Visible;

	public static IList<string> GetFontNames()
		=> (from fontName in CanvasTextFormat.GetSystemFontFamilies() orderby fontName select fontName).ToList();
}
