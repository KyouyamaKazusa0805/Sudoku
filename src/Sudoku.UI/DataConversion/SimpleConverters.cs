namespace Sudoku.UI.DataConversion;

/// <summary>
/// Provides with a set of methods as simple converters that can be used and called by XAML files.
/// </summary>
internal static class SimpleConverters
{
	private static readonly SolidColorBrush[] DifficultyLevel_Foregrounds =
	{
		new(Color.FromArgb(255,   0,  51, 204)),
		new(Color.FromArgb(255,   0, 102,   0)),
		new(Color.FromArgb(255, 102,  51,   0)),
		new(Color.FromArgb(255, 102,  51,   0)),
		new(Color.FromArgb(255, 102,   0,   0))
	};

	private static readonly SolidColorBrush[] DifficultyLevel_Backgrounds =
	{
		new(Color.FromArgb(255, 204, 204, 255)),
		new(Color.FromArgb(255, 100, 255, 100)),
		new(Color.FromArgb(255, 255, 255, 100)),
		new(Color.FromArgb(255, 255, 150,  80)),
		new(Color.FromArgb(255, 255, 100, 100))
	};


	/// <summary>
	/// Indicates the license displaying value on <see cref="RepositoryInfo.OpenSourceLicense"/>.
	/// </summary>
	/// <param name="input">The license name.</param>
	/// <returns>The converted result string.</returns>
	/// <seealso cref="RepositoryInfo.OpenSourceLicense"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string License(string input) => $"{input} {R["AboutPage_License"]}";

	/// <summary>
	/// Indicates the conversion on <see cref="RepositoryInfo.IsForReference"/>.
	/// </summary>
	/// <param name="input">The input value.</param>
	/// <returns>The converted result string value.</returns>
	/// <seealso cref="RepositoryInfo.IsForReference"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ForReference(bool input) => input ? R["AboutPage_ForReference"]! : string.Empty;

	/// <summary>
	/// Gets the title of the info bar via its severity.
	/// </summary>
	/// <param name="severity">The severity.</param>
	/// <returns>The title of the info bar.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the severity is not defined.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string InfoBarTitle(InfoBarSeverity severity)
		=> R[
			severity switch
			{
				InfoBarSeverity.Informational => "SudokuPage_InfoBar_SeverityInfo",
				InfoBarSeverity.Success => "SudokuPage_InfoBar_SeveritySuccess",
				InfoBarSeverity.Warning => "SudokuPage_InfoBar_SeverityWarning",
				InfoBarSeverity.Error => "SudokuPage_InfoBar_SeverityError",
				_ => throw new ArgumentOutOfRangeException(nameof(severity))
			}
		]!;

	public static string SliderPossibleValueString(double min, double max, double stepFrequency, double tickFrequency)
		=> SliderPossibleValueStringWithFormat(min, max, stepFrequency, tickFrequency, "0.0");

	public static string SliderPossibleValueStringWithFormat(double min, double max, double stepFrequency, double tickFrequency, string format)
		=> $"{R["SliderPossibleValue"]}{min.ToString(format)} - {max.ToString(format)}{R["SliderStepFrequency"]}{stepFrequency.ToString(format)}{R["SliderTickFrequency"]}{tickFrequency.ToString(format)}";

	public static string DifficultyLevelToResourceText(DifficultyLevel difficultyLevel)
		=> (
			difficultyLevel switch
			{
				DifficultyLevel.Easy => R["SudokuPage_AnalysisResultColumn_Easy"],
				DifficultyLevel.Moderate => R["SudokuPage_AnalysisResultColumn_Moderate"],
				DifficultyLevel.Hard => R["SudokuPage_AnalysisResultColumn_Hard"],
				DifficultyLevel.Fiendish => R["SudokuPage_AnalysisResultColumn_Fiendish"],
				DifficultyLevel.Nightmare => R["SudokuPage_AnalysisResultColumn_Nightmare"],
				_ => string.Empty
			}
		)!;

	public static string ToRgbString(Color color) => $"{color.R}, {color.G}, {color.B}";

	/// <summary>
	/// Gets the text from property value <see cref="VirtualKey"/> and <see cref="VirtualKeyModifiers"/>.
	/// </summary>
	/// <returns>The string text.</returns>
	/// <seealso cref="VirtualKey"/>
	/// <seealso cref="VirtualKeyModifiers"/>
	public static unsafe string GetText(VirtualKeyModifiers modifiers, VirtualKey VirtualKey)
	{
		switch (modifiers)
		{
			case VirtualKeyModifiers.None:
			{
				return ConvertVirtualKeyToName(VirtualKey);
			}
			case var mods:
			{
				var sb = new StringHandler();
				sb.AppendRangeUnsafe(mods, &f);

				return $"{sb.ToStringAndClear()} + {ConvertVirtualKeyToName(VirtualKey)}";
			}


			static string f(VirtualKeyModifiers mod) => mod == VirtualKeyModifiers.Menu ? "Alt" : mod.ToString();
		}
	}

	/// <summary>
	/// Gets the date value from the raw string.
	/// </summary>
	/// <param name="dateRawString">The date raw string.</param>
	/// <returns>The target <see cref="DateOnly"/> instance.</returns>
	public static DateOnly GetDate(string dateRawString) => DateOnly.Parse(dateRawString);

	/// <summary>
	/// Gets the date value from the raw string.
	/// </summary>
	/// <param name="dateRawString">The date raw string.</param>
	/// <param name="format">The format.</param>
	/// <returns>The target <see cref="DateOnly"/> instance.</returns>
	public static DateOnly GetDateWithFormat(string dateRawString, string format) => DateOnly.ParseExact(dateRawString, format);

	public static Visibility StringToVisibility(string? s)
		=> string.IsNullOrWhiteSpace(s) ? Visibility.Collapsed : Visibility.Visible;

	public static SolidColorBrush DifficultyLevelToForeground(DifficultyLevel difficultyLevel)
		=> difficultyLevel switch
		{
			0 or > DifficultyLevel.Nightmare => new(Colors.Transparent),
			_ => DifficultyLevel_Foregrounds[Log2((byte)difficultyLevel)]
		};

	public static SolidColorBrush DifficultyLevelToBackground(DifficultyLevel difficultyLevel)
		=> difficultyLevel switch
		{
			0 or > DifficultyLevel.Nightmare => new(Colors.Transparent),
			_ => DifficultyLevel_Backgrounds[Log2((byte)difficultyLevel)]
		};

	public static SolidColorBrush CreateBrushFromColor(Color color) => new(color);

	/// <summary>
	/// Gets the timeline information.
	/// </summary>
	/// <param name="versionTimelineItem">The version timeline item value.</param>
	/// <returns>The text block instance.</returns>
	public static string GetTimelineInfo(VersionTimelineItem versionTimelineItem)
		=> versionTimelineItem.Description is var description && versionTimelineItem.Date is { } date
			? $"{R["Token_OpenBrace"]}{date.ToShortDateString()}{R["Token_ClosedBrace"]}{description}" 
			: description;

	public static IList<string> GetFontNames()
		=> (from fontName in CanvasTextFormat.GetSystemFontFamilies() orderby fontName select fontName).ToList();

	/// <summary>
	/// Converts the specified virtual VirtualKey into a string representation.
	/// </summary>
	/// <param name="VirtualKey">The virtual VirtualKey.</param>
	/// <returns>The string representation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static string ConvertVirtualKeyToName(VirtualKey VirtualKey)
		=> VirtualKey switch
		{
			>= VirtualKey.Number0 and <= VirtualKey.Number9 => (VirtualKey - VirtualKey.Number0).ToString(),
			>= VirtualKey.NumberPad0 and <= VirtualKey.NumberPad9 => (VirtualKey - VirtualKey.NumberPad0).ToString(),
			_ => VirtualKey.ToString()
		};
}
