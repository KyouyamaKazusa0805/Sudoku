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
	/// Gets the text from property value <see cref="Key"/> and <see cref="ModifierKey"/>.
	/// </summary>
	/// <param name="modifiers">The modifier keys.</param>
	/// <param name="virtualKeys">The virtual keys.</param>
	/// <returns>The string text.</returns>
	/// <seealso cref="Key"/>
	/// <seealso cref="ModifierKey"/>
	public static unsafe string GetText(ModifierKey modifiers, Key[] virtualKeys)
	{
		switch (modifiers)
		{
			case ModifierKey.None:
			{
				return ConvertVirtualKeyToName(virtualKeys);
			}
			case var mods:
			{
				var sb = new StringHandler(100);
				sb.AppendRangeWithSeparatorUnsafe(mods, &f, " + ");

				return $"{sb.ToStringAndClear()} + {ConvertVirtualKeyToName(virtualKeys)}";
			}


			static string f(ModifierKey mod)
				=> mod switch { ModifierKey.Menu => "Alt", ModifierKey.Control => "Ctrl", _ => mod.ToString() };
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
	/// Converts the specified virtual key into a string representation.
	/// </summary>
	/// <param name="virtualKeys">The virtual key.</param>
	/// <returns>The string representation.</returns>
	/// <exception cref="ArgumentNullException">
	/// Throws when the argument <paramref name="virtualKeys"/> is <see langword="null"/>.
	/// </exception>
	/// <exception cref="ArgumentException">Throws when the argument <paramref name="virtualKeys"/> is empty.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static string ConvertVirtualKeyToName(Key[]? virtualKeys)
	{
		return virtualKeys switch
		{
			// Null.
			null => throw new ArgumentNullException(nameof(virtualKeys)),

			// Empty array.
			[] => throw new ArgumentException("The argument cannot be empty here.", nameof(virtualKeys)),

			// If the key is a digit.
			[var virtualKey] => virtualKey switch
			{
				>= Key.Number0 and <= Key.Number9 => (virtualKey - Key.Number0).ToString(),
				>= Key.NumberPad0 and <= Key.NumberPad9 => (virtualKey - Key.NumberPad0).ToString(),
				>= Key.Left and <= Key.Down or Key.Space => R[$"VirtualKey_{virtualKey}"]!,
				Key.Back => R["VirtualKey_Backspace"]!,
				Key.Subtract => R["VirtualKey_Subtract"]!,
				(Key)187 => R["VirtualKey_187"]!,
				_ => virtualKey.ToString()
			},

			// If two keys are both digit and same value.
			[
				var a and >= Key.Number0 and <= Key.Number9,
				var b and >= Key.NumberPad0 and <= Key.NumberPad9
			]
			when a - Key.Number0 == b - Key.NumberPad0 => (a - Key.Number0).ToString(),

			// If two keys are both digit and same value.
			[
				var a and >= Key.NumberPad0 and <= Key.NumberPad9,
				var b and >= Key.Number0 and <= Key.Number9
			]
			when a - Key.NumberPad0 == b - Key.Number0 => (b - Key.Number0).ToString(),

			// Consecutive digit keys.
			[
				>= Key.Number0 and <= Key.Number9 or >= Key.NumberPad0 and <= Key.NumberPad9,
				..,
				>= Key.Number0 and <= Key.Number9 or >= Key.NumberPad0 and <= Key.NumberPad9,
			]
			when isConsecutiveNumber(virtualKeys, out int a, out int b) => $"{a}-{b}",

			// Consecutive letter keys.
			[>= Key.A and <= Key.Z, .., >= Key.A and <= Key.Z]
			when isConsecutiveLetters(virtualKeys, out char a, out char b) => $"{a}-{b}",

			// Otherwise.
			_ => string.Join('/', from virtualKey in virtualKeys select ConvertVirtualKeyToName(new[] { virtualKey }))
		};


		static bool isConsecutiveNumber(Key[] keys, out int a, out int b)
		{
			var first = keys[0];
			foreach (var key in keys[1..])
			{
				if (key - first != 1)
				{
					a = b = -1;
					return false;
				}

				first = key;
			}

			a = keys[0] is var keyA && keyA is >= Key.Number0 and <= Key.Number9
				? keyA - Key.Number0
				: keyA - Key.NumberPad0;
			b = keys[^1] is var keyB && keyB is >= Key.Number0 and <= Key.Number9
				? keyB - Key.Number0
				: keyB - Key.NumberPad0;
			return true;
		}

		static bool isConsecutiveLetters(Key[] keys, out char a, out char b)
		{
			var first = keys[0];
			foreach (var key in keys[1..])
			{
				if (key - first != 1)
				{
					a = b = '\0';
					return false;
				}

				first = key;
			}

			a = (char)(keys[0] - Key.A + 'A');
			b = (char)(keys[^1] - Key.A + 'A');
			return true;
		}
	}
}
