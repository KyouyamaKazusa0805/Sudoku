namespace Nano.Properties;

/// <summary>
/// Defines a set of properties that is the main settings in the program.
/// </summary>
public sealed partial class Preference
{
	/// <summary>
	/// Initializes a <see cref="Preference"/> instance.
	/// </summary>
	public Preference()
	{
	}


	/// <summary>
	/// Indicates the current UI project uses the light mode or the dark mode.
	/// </summary>
	[JsonIgnore]
	public static ApplicationTheme LightOrDarkMode => Application.Current.RequestedTheme;


	/// <summary>
	/// <para>Indicates the width of the block lines.</para>
	/// <para><i>The default value is 3.</i></para>
	/// </summary>
	public double BlockLineWidth { get; set; } = 3;

	/// <summary>
	/// <para>Indicates the width of the cell border lines.</para>
	/// <para><i>The default value is 1.</i></para>
	/// </summary>
	public double GridLineWidth { get; set; } = 1;

	/// <summary>
	/// <para>Indicates the width of the highlight cell border.</para>
	/// <para><i>The default value is 1.5.</i></para>
	/// </summary>
	public double CellBorderWidth { get; set; } = 1.5;

	/// <summary>
	/// <para>Indicates the width of the highlight candidate border.</para>
	/// <para><i>The default value is 1.5.</i></para>
	/// </summary>
	public double CandidateBorderWidth { get; set; } = 1.5;

	/// <summary>
	/// <para>Indicates the width of the highlight region border.</para>
	/// <para><i>The default value is 1.5.</i></para>
	/// </summary>
	public double RegionBorderWidth { get; set; } = 1.5;

	/// <summary>
	/// <para>Indicates the scale value on rendering given or modifiable numbers in a grid.</para>
	/// <para><i>The default value is 0.6.</i></para>
	/// </summary>
	public decimal ValueScale { get; set; } = .6M;

	/// <summary>
	/// <para>Indicates the scale value on rendering candidate numbers in a grid.</para>
	/// <para><i>The default value is 0.8.</i></para>
	/// </summary>
	public decimal CandidateScale { get; set; } = .8M;

	/// <summary>
	/// <para>Indicates the font name for the given numbers on rendering.</para>
	/// <para><i>The default value is "Fira Code".</i></para>
	/// </summary>
	public string GivenFontName { get; set; } = "Fira Code";

	/// <summary>
	/// <para>Indicates the font name for the candidate numbers on rendering.</para>
	/// <para><i>The default value is "Times New Roman".</i></para>
	/// </summary>
	public string CandidateFontName { get; set; } = "Times New Roman";

	/// <summary>
	/// Indicates the color of the cell border lines.
	/// </summary>
	/// <remarks>
	/// The property will route the real values via the current theme kind.
	/// </remarks>
	[JsonIgnore]
	public Color GridLineColor =>
		LightOrDarkMode == ApplicationTheme.Light ? GridLineColorLight : GridLineColorDark;

	/// <summary>
	/// <para>Indicates the color of the cell border lines in the light theme.</para>
	/// <para><i>The default value is <see cref="Colors.Black"/>.</i></para>
	/// </summary>
	public Color GridLineColorLight { get; set; } = Colors.Black;

	/// <summary>
	/// <para>Indicates the color of the cell border lines in the dark theme.</para>
	/// <para><i>The default value is <see cref="Colors.LightGray"/>.</i></para>
	/// </summary>
	public Color GridLineColorDark { get; set; } = Colors.LightGray;

	/// <summary>
	/// Indicates the color of the given values on rendering.
	/// </summary>
	/// <remarks>
	/// The property will route the real values via the current theme kind.
	/// </remarks>
	[JsonIgnore]
	public Color GivenColor => LightOrDarkMode == ApplicationTheme.Light ? GivenColorLight : GivenColorDark;

	/// <summary>
	/// <para>Indicates the color of the given values on rendering in the light theme.</para>
	/// <para><i>The default value is <see cref="Colors.Black"/>.</i></para>
	/// </summary>
	public Color GivenColorLight { get; set; } = Colors.Black;

	/// <summary>
	/// <para>Indicates the color of the given values on rendering in the dark theme.</para>
	/// <para><i>The default value is <see cref="Colors.LightGray"/>.</i></para>
	/// </summary>
	public Color GivenColorDark { get; set; } = Colors.LightGray;

	/// <summary>
	/// Indicates the color of the candidate values on rendering.
	/// </summary>
	/// <remarks>
	/// The property will route the real values via the current theme kind.
	/// </remarks>
	[JsonIgnore]
	public Color CandidateColor =>
		LightOrDarkMode == ApplicationTheme.Light ? CandidateColorLight : CandidateColorDark;

	/// <summary>
	/// <para>Indicates the color of the candidate values on rendering in the light theme.</para>
	/// <para><i>The default value is <see cref="Colors.DimGray"/>.</i></para>
	/// </summary>
	public Color CandidateColorLight { get; set; } = Colors.DimGray;

	/// <summary>
	/// <para>Indicates the color of the candidate values on rendering in the dark theme.</para>
	/// <para><i>The default value is <see cref="Colors.Gray"/>.</i></para>
	/// </summary>
	public Color CandidateColorDark { get; set; } = Colors.Gray;

	/// <summary>
	/// Indicates the color of the highlight cell border lines on rendering.
	/// </summary>
	/// <remarks>
	/// The property will route the real values via the current theme kind.
	/// </remarks>
	[JsonIgnore]
	public Color CellBorderColor =>
		LightOrDarkMode == ApplicationTheme.Light ? CellBorderColorLight : CellBorderColorDark;

	/// <summary>
	/// <para>Indicates the color of the highlight cell border lines on rendering in the light theme.</para>
	/// <para><i>The default value is <see cref="Colors.Blue"/>.</i></para>
	/// </summary>
	public Color CellBorderColorLight { get; set; } = Colors.Blue;

	/// <summary>
	/// <para>Indicates the color of the highlight cell border lines on rendering in the dark theme.</para>
	/// <para><i>The default value is <c>{ R = 86, G = 156, B = 214 }</c>.</i></para>
	/// </summary>
	public Color CellBorderColorDark { get; set; } = Color.FromArgb(255, 86, 156, 214);

	/// <summary>
	/// Indicates the color of the highlight candidate border lines on rendering.
	/// </summary>
	/// <remarks>
	/// The property will route the real values via the current theme kind.
	/// </remarks>
	[JsonIgnore]
	public Color CandidateBorderColor =>
		LightOrDarkMode == ApplicationTheme.Light ? CandidateBorderColorLight : CandidateBorderColorDark;

	/// <summary>
	/// <para>Indicates the color of the highlight candidate border lines on rendering in the light theme.</para>
	/// <para><i>The default value is <see cref="Colors.Blue"/>.</i></para>
	/// </summary>
	public Color CandidateBorderColorLight { get; set; } = Colors.Blue;

	/// <summary>
	/// <para>Indicates the color of the highlight candidate border lines on rendering in the dark theme.</para>
	/// <para><i>The default value is <c>{ R = 86, G = 156, B = 214 }</c>.</i></para>
	/// </summary>
	public Color CandidateBorderColorDark { get; set; } = Color.FromArgb(255, 86, 156, 214);

	/// <summary>
	/// Indicates the color of the highlight region border lines on rendering.
	/// </summary>
	/// <remarks>
	/// The property will route the real values via the current theme kind.
	/// </remarks>
	[JsonIgnore]
	public Color RegionBorderColor =>
		LightOrDarkMode == ApplicationTheme.Light ? RegionBorderColorLight : RegionBorderColorDark;

	/// <summary>
	/// <para>Indicates the color of the highlight region border lines on rendering in the light theme.</para>
	/// <para><i>The default value is <see cref="Colors.Blue"/>.</i></para>
	/// </summary>
	public Color RegionBorderColorLight { get; set; } = Colors.Blue;

	/// <summary>
	/// <para>Indicates the color of the highlight region border lines on rendering in the dark theme.</para>
	/// <para><i>The default value is <c>{ R = 86, G = 156, B = 214 }</c>.</i></para>
	/// </summary>
	public Color RegionBorderColorDark { get; set; } = Color.FromArgb(255, 86, 156, 214);

	/// <summary>
	/// Indicates the color of the background color surrounding by the highlight cell border lines
	/// on rendering.
	/// </summary>
	/// <remarks>
	/// The property will route the real values via the current theme kind.
	/// </remarks>
	[JsonIgnore]
	public Color CellBorderBackgroundColor =>
		LightOrDarkMode == ApplicationTheme.Light
			? CellBorderBackgroundColorLight
			: CellBorderBackgroundColorDark;

	/// <summary>
	/// <para>
	/// Indicates the color of the background color surrounding by the highlight cell border lines
	/// on rendering in the light mode.
	/// </para>
	/// <para><i>The default value is <see cref="Colors.Blue"/> with the alpha 64.</i></para>
	/// </summary>
	public Color CellBorderBackgroundColorLight { get; set; } = Color.FromArgb(64, 0, 0, 255);

	/// <summary>
	/// <para>
	/// Indicates the color of the background color surrounding by the highlight cell border lines
	/// on rendering in the dark mode.
	/// </para>
	/// <para><i>The default value is <c>{ A = 64, R = 86, G = 156, B = 214 }</c>.</i></para>
	/// </summary>
	public Color CellBorderBackgroundColorDark { get; set; } = Color.FromArgb(64, 86, 156, 214);

	/// <summary>
	/// Indicates the color of the background color surrounding by the highlight candidate border lines
	/// on rendering.
	/// </summary>
	/// <remarks>
	/// The property will route the real values via the current theme kind.
	/// </remarks>
	[JsonIgnore]
	public Color CandidateBorderBackgroundColor =>
		LightOrDarkMode == ApplicationTheme.Light
			? CandidateBorderBackgroundColorLight
			: CandidateBorderBackgroundColorDark;

	/// <summary>
	/// <para>
	/// Indicates the color of the background color surrounding by the highlight candidate border lines
	/// on rendering in the light mode.
	/// </para>
	/// <para><i>The default value is <see cref="Colors.Blue"/> with the alpha 64.</i></para>
	/// </summary>
	public Color CandidateBorderBackgroundColorLight { get; set; } = Color.FromArgb(64, 0, 0, 255);

	/// <summary>
	/// <para>
	/// Indicates the color of the background color surrounding by the highlight candidate border lines
	/// on rendering in the dark mode.
	/// </para>
	/// <para><i>The default value is <c>{ A = 64, R = 86, G = 156, B = 214 }</c>.</i></para>
	/// </summary>
	public Color CandidateBorderBackgroundColorDark { get; set; } = Color.FromArgb(64, 86, 156, 214);

	/// <summary>
	/// Indicates the color of the background color surrounding by the highlight region border lines
	/// on rendering.
	/// </summary>
	/// <remarks>
	/// The property will route the real values via the current theme kind.
	/// </remarks>
	[JsonIgnore]
	public Color RegionBorderBackgroundColor =>
		LightOrDarkMode == ApplicationTheme.Light
			? RegionBorderBackgroundColorLight
			: RegionBorderBackgroundColorDark;

	/// <summary>
	/// <para>
	/// Indicates the color of the background color surrounding by the highlight region border lines
	/// on rendering in the light mode.
	/// </para>
	/// <para><i>The default value is <see cref="Colors.Blue"/> with the alpha 64.</i></para>
	/// </summary>
	public Color RegionBorderBackgroundColorLight { get; set; } = Color.FromArgb(64, 0, 0, 255);

	/// <summary>
	/// <para>
	/// Indicates the color of the background color surrounding by the highlight region border lines
	/// on rendering in the dark mode.
	/// </para>
	/// <para><i>The default value is <c>{ A = 64, R = 86, G = 156, B = 214 }</c>.</i></para>
	/// </summary>
	public Color RegionBorderBackgroundColorDark { get; set; } = Color.FromArgb(64, 86, 156, 214);

	/// <summary>
	/// <para>Indicates the font style on rendering given numbers.</para>
	/// <para><i>The default value is <see cref="FontStyle.Normal"/>.</i></para>
	/// </summary>
	public FontStyle GivenFontStyle { get; set; } = FontStyle.Normal;

	/// <summary>
	/// <para>Indicates the font style on rendering candidate numbers.</para>
	/// <para><i>The default value is <see cref="FontStyle.Normal"/>.</i></para>
	/// </summary>
	public FontStyle CandidateFontStyle { get; set; } = FontStyle.Normal;
}
