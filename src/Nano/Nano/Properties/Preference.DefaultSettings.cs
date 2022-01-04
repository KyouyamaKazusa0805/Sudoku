namespace Nano.Properties;

partial class Preference
{
	internal static readonly double BlockLineWidth_Default = 3;
	internal static readonly double GridLineWidth_Default = 1;
	internal static readonly double CellBorderWidth_Default = 1.5;
	internal static readonly double CandidateBorderWidth_Default = 1.5;
	internal static readonly double RegionBorderWidth_Default = 1.5;
	internal static readonly decimal ValueScale_Default = .6M;
	internal static readonly decimal CandidateScale_Default = .8M;
	internal static readonly string GivenFontName_Default = "Fira Code";
	internal static readonly string CandidateFontName_Default = "Times New Roman";
	internal static readonly Color GridLineColorLight_Default = Colors.DimGray;
	internal static readonly Color GridLineColorDark_Default = Colors.LightGray;
	internal static readonly Color GivenColorLight_Default = Colors.Black;
	internal static readonly Color GivenColorDark_Default = Colors.White;
	internal static readonly Color CandidateColorLight_Default = Colors.DimGray;
	internal static readonly Color CandidateColorDark_Default = Colors.LightGray;
	internal static readonly Color CellBorderColorLight_Default = Colors.Blue;
	internal static readonly Color CellBorderColorDark_Default = Color.FromArgb(255, 86, 156, 214);
	internal static readonly Color CandidateBorderColorLight_Default = Colors.Blue;
	internal static readonly Color CandidateBorderColorDark_Default = Color.FromArgb(255, 86, 156, 214);
	internal static readonly Color RegionBorderColorLight_Default = Colors.Blue;
	internal static readonly Color RegionBorderColorDark_Default = Color.FromArgb(255, 86, 156, 214);
	internal static readonly Color CellBorderBackgroundColorLight_Default = Color.FromArgb(64, 0, 0, 255);
	internal static readonly Color CellBorderBackgroundColorDark_Default = Color.FromArgb(64, 86, 156, 214);
	internal static readonly Color CandidateBorderBackgroundColorLight_Default = Color.FromArgb(64, 0, 0, 255);
	internal static readonly Color CandidateBorderBackgroundColorDark_Default = Color.FromArgb(64, 86, 156, 214);
	internal static readonly Color RegionBorderBackgroundColorLight_Default = Color.FromArgb(64, 0, 0, 255);
	internal static readonly Color RegionBorderBackgroundColorDark_Default = Color.FromArgb(64, 86, 156, 214);
	internal static readonly FontStyle GivenFontStyle_Default = FontStyle.Normal;
	internal static readonly FontStyle CandidateFontStyle_Default = FontStyle.Normal;

	/// <summary>
	/// Indicates whether the current theme is light theme.
	/// </summary>
	[JsonIgnore]
	public static bool IsLightTheme => Application.Current.RequestedTheme == ApplicationTheme.Light;

	/// <summary>
	/// Indicates the current UI project uses the light mode or the dark mode.
	/// </summary>
	[JsonIgnore]
	public static ApplicationTheme LightOrDarkMode => Application.Current.RequestedTheme;

	[JsonIgnore]
	internal static Color GivenColor_Default => IsLightTheme ? GivenColorLight_Default : GivenColorDark_Default;

	[JsonIgnore]
	internal static Color CandidateColor_Default => IsLightTheme ? CandidateColorLight_Default : CandidateColorDark_Default;
}
