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
	/// <para>Indicates the width of the block lines.</para>
	/// <para><i>The default value is 3.</i></para>
	/// </summary>
	public double BlockLineWidth { get; set; } = BlockLineWidth_Default;

	/// <summary>
	/// <para>Indicates the width of the cell border lines.</para>
	/// <para><i>The default value is 1.</i></para>
	/// </summary>
	public double GridLineWidth { get; set; } = GridLineWidth_Default;

	/// <summary>
	/// <para>Indicates the width of the highlight cell border.</para>
	/// <para><i>The default value is 1.5.</i></para>
	/// </summary>
	public double CellBorderWidth { get; set; } = CellBorderWidth_Default;

	/// <summary>
	/// <para>Indicates the width of the highlight candidate border.</para>
	/// <para><i>The default value is 1.5.</i></para>
	/// </summary>
	public double CandidateBorderWidth { get; set; } = CandidateBorderWidth_Default;

	/// <summary>
	/// <para>Indicates the width of the highlight region border.</para>
	/// <para><i>The default value is 1.5.</i></para>
	/// </summary>
	public double RegionBorderWidth { get; set; } = RegionBorderWidth_Default;

	/// <summary>
	/// <para>Indicates the scale value on rendering given or modifiable numbers in a grid.</para>
	/// <para><i>The default value is 0.6.</i></para>
	/// </summary>
	public decimal ValueScale { get; set; } = ValueScale_Default;

	/// <summary>
	/// <para>Indicates the scale value on rendering candidate numbers in a grid.</para>
	/// <para><i>The default value is 0.8.</i></para>
	/// </summary>
	public decimal CandidateScale { get; set; } = CandidateScale_Default;

	/// <summary>
	/// <para>Indicates the font name for the given numbers on rendering.</para>
	/// <para><i>The default value is "Fira Code".</i></para>
	/// </summary>
	public string GivenFontName { get; set; } = GivenFontName_Default;

	/// <summary>
	/// <para>Indicates the font name for the candidate numbers on rendering.</para>
	/// <para><i>The default value is "Times New Roman".</i></para>
	/// </summary>
	public string CandidateFontName { get; set; } = CandidateFontName_Default;

	/// <summary>
	/// Indicates the color of the cell border lines.
	/// </summary>
	/// <remarks>
	/// The property will route the real values via the current theme kind.
	/// </remarks>
	[JsonIgnore]
	public Color GridLineColor => IsLightTheme ? GridLineColorLight : GridLineColorDark;

	/// <summary>
	/// <para>Indicates the color of the cell border lines in the light theme.</para>
	/// <para><i>The default value is <see cref="Colors.Black"/>.</i></para>
	/// </summary>
	public Color GridLineColorLight { get; set; } = GridLineColorLight_Default;

	/// <summary>
	/// <para>Indicates the color of the cell border lines in the dark theme.</para>
	/// <para><i>The default value is <see cref="Colors.LightGray"/>.</i></para>
	/// </summary>
	public Color GridLineColorDark { get; set; } = GridLineColorDark_Default;

	/// <summary>
	/// Indicates the color of the given values on rendering.
	/// </summary>
	/// <remarks>
	/// The property will route the real values via the current theme kind.
	/// </remarks>
	[JsonIgnore]
	public Color GivenColor => IsLightTheme ? GivenColorLight : GivenColorDark;

	/// <summary>
	/// <para>Indicates the color of the given values on rendering in the light theme.</para>
	/// <para><i>The default value is <see cref="Colors.Black"/>.</i></para>
	/// </summary>
	public Color GivenColorLight { get; set; } = GivenColorLight_Default;

	/// <summary>
	/// <para>Indicates the color of the given values on rendering in the dark theme.</para>
	/// <para><i>The default value is <see cref="Colors.White"/>.</i></para>
	/// </summary>
	public Color GivenColorDark { get; set; } = GivenColorDark_Default;

	/// <summary>
	/// Indicates the color of the candidate values on rendering.
	/// </summary>
	/// <remarks>
	/// The property will route the real values via the current theme kind.
	/// </remarks>
	[JsonIgnore]
	public Color CandidateColor => IsLightTheme ? CandidateColorLight : CandidateColorDark;

	/// <summary>
	/// <para>Indicates the color of the candidate values on rendering in the light theme.</para>
	/// <para><i>The default value is <see cref="Colors.DimGray"/>.</i></para>
	/// </summary>
	public Color CandidateColorLight { get; set; } = CandidateColorLight_Default;

	/// <summary>
	/// <para>Indicates the color of the candidate values on rendering in the dark theme.</para>
	/// <para><i>The default value is <see cref="Colors.LightGray"/>.</i></para>
	/// </summary>
	public Color CandidateColorDark { get; set; } = CandidateColorDark_Default;

	/// <summary>
	/// Indicates the color of the highlight cell border lines on rendering.
	/// </summary>
	/// <remarks>
	/// The property will route the real values via the current theme kind.
	/// </remarks>
	[JsonIgnore]
	public Color CellBorderColor => IsLightTheme ? CellBorderColorLight : CellBorderColorDark;

	/// <summary>
	/// <para>Indicates the color of the highlight cell border lines on rendering in the light theme.</para>
	/// <para><i>The default value is <see cref="Colors.Blue"/>.</i></para>
	/// </summary>
	public Color CellBorderColorLight { get; set; } = CellBorderColorLight_Default;

	/// <summary>
	/// <para>Indicates the color of the highlight cell border lines on rendering in the dark theme.</para>
	/// <para><i>The default value is <c>{ R = 86, G = 156, B = 214 }</c>.</i></para>
	/// </summary>
	public Color CellBorderColorDark { get; set; } = CellBorderColorDark_Default;

	/// <summary>
	/// Indicates the color of the highlight candidate border lines on rendering.
	/// </summary>
	/// <remarks>
	/// The property will route the real values via the current theme kind.
	/// </remarks>
	[JsonIgnore]
	public Color CandidateBorderColor => IsLightTheme ? CandidateBorderColorLight : CandidateBorderColorDark;

	/// <summary>
	/// <para>Indicates the color of the highlight candidate border lines on rendering in the light theme.</para>
	/// <para><i>The default value is <see cref="Colors.Blue"/>.</i></para>
	/// </summary>
	public Color CandidateBorderColorLight { get; set; } = CandidateBorderColorLight_Default;

	/// <summary>
	/// <para>Indicates the color of the highlight candidate border lines on rendering in the dark theme.</para>
	/// <para><i>The default value is <c>{ R = 86, G = 156, B = 214 }</c>.</i></para>
	/// </summary>
	public Color CandidateBorderColorDark { get; set; } = CandidateBorderColorDark_Default;

	/// <summary>
	/// Indicates the color of the highlight region border lines on rendering.
	/// </summary>
	/// <remarks>
	/// The property will route the real values via the current theme kind.
	/// </remarks>
	[JsonIgnore]
	public Color RegionBorderColor => IsLightTheme ? RegionBorderColorLight : RegionBorderColorDark;

	/// <summary>
	/// <para>Indicates the color of the highlight region border lines on rendering in the light theme.</para>
	/// <para><i>The default value is <see cref="Colors.Blue"/>.</i></para>
	/// </summary>
	public Color RegionBorderColorLight { get; set; } = RegionBorderColorLight_Default;

	/// <summary>
	/// <para>Indicates the color of the highlight region border lines on rendering in the dark theme.</para>
	/// <para><i>The default value is <c>{ R = 86, G = 156, B = 214 }</c>.</i></para>
	/// </summary>
	public Color RegionBorderColorDark { get; set; } = RegionBorderColorDark_Default;

	/// <summary>
	/// Indicates the color of the background color surrounding by the highlight cell border lines
	/// on rendering.
	/// </summary>
	/// <remarks>
	/// The property will route the real values via the current theme kind.
	/// </remarks>
	[JsonIgnore]
	public Color CellBorderBackgroundColor =>
		IsLightTheme ? CellBorderBackgroundColorLight : CellBorderBackgroundColorDark;

	/// <summary>
	/// <para>
	/// Indicates the color of the background color surrounding by the highlight cell border lines
	/// on rendering in the light mode.
	/// </para>
	/// <para><i>The default value is <see cref="Colors.Blue"/> with the alpha 64.</i></para>
	/// </summary>
	public Color CellBorderBackgroundColorLight { get; set; } = CellBorderBackgroundColorLight_Default;

	/// <summary>
	/// <para>
	/// Indicates the color of the background color surrounding by the highlight cell border lines
	/// on rendering in the dark mode.
	/// </para>
	/// <para><i>The default value is <c>{ A = 64, R = 86, G = 156, B = 214 }</c>.</i></para>
	/// </summary>
	public Color CellBorderBackgroundColorDark { get; set; } = CellBorderBackgroundColorDark_Default;

	/// <summary>
	/// Indicates the color of the background color surrounding by the highlight candidate border lines
	/// on rendering.
	/// </summary>
	/// <remarks>
	/// The property will route the real values via the current theme kind.
	/// </remarks>
	[JsonIgnore]
	public Color CandidateBorderBackgroundColor =>
		IsLightTheme ? CandidateBorderBackgroundColorLight : CandidateBorderBackgroundColorDark;

	/// <summary>
	/// <para>
	/// Indicates the color of the background color surrounding by the highlight candidate border lines
	/// on rendering in the light mode.
	/// </para>
	/// <para><i>The default value is <see cref="Colors.Blue"/> with the alpha 64.</i></para>
	/// </summary>
	public Color CandidateBorderBackgroundColorLight { get; set; } = CandidateBorderBackgroundColorLight_Default;

	/// <summary>
	/// <para>
	/// Indicates the color of the background color surrounding by the highlight candidate border lines
	/// on rendering in the dark mode.
	/// </para>
	/// <para><i>The default value is <c>{ A = 64, R = 86, G = 156, B = 214 }</c>.</i></para>
	/// </summary>
	public Color CandidateBorderBackgroundColorDark { get; set; } = CandidateBorderBackgroundColorDark_Default;

	/// <summary>
	/// Indicates the color of the background color surrounding by the highlight region border lines
	/// on rendering.
	/// </summary>
	/// <remarks>
	/// The property will route the real values via the current theme kind.
	/// </remarks>
	[JsonIgnore]
	public Color RegionBorderBackgroundColor =>
		IsLightTheme ? RegionBorderBackgroundColorLight : RegionBorderBackgroundColorDark;

	/// <summary>
	/// <para>
	/// Indicates the color of the background color surrounding by the highlight region border lines
	/// on rendering in the light mode.
	/// </para>
	/// <para><i>The default value is <see cref="Colors.Blue"/> with the alpha 64.</i></para>
	/// </summary>
	public Color RegionBorderBackgroundColorLight { get; set; } = RegionBorderBackgroundColorLight_Default;

	/// <summary>
	/// <para>
	/// Indicates the color of the background color surrounding by the highlight region border lines
	/// on rendering in the dark mode.
	/// </para>
	/// <para><i>The default value is <c>{ A = 64, R = 86, G = 156, B = 214 }</c>.</i></para>
	/// </summary>
	public Color RegionBorderBackgroundColorDark { get; set; } = RegionBorderBackgroundColorDark_Default;

	/// <summary>
	/// <para>Indicates the font style on rendering given numbers.</para>
	/// <para><i>The default value is <see cref="FontStyle.Normal"/>.</i></para>
	/// </summary>
	public FontStyle GivenFontStyle { get; set; } = GivenFontStyle_Default;

	/// <summary>
	/// <para>Indicates the font style on rendering candidate numbers.</para>
	/// <para><i>The default value is <see cref="FontStyle.Normal"/>.</i></para>
	/// </summary>
	public FontStyle CandidateFontStyle { get; set; } = CandidateFontStyle_Default;
}
