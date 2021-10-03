namespace Sudoku.UI.Data;

/// <summary>
/// Provides with the preference instance.
/// </summary>
public sealed partial class Preference : ICloneable<Preference>
{
	/// <summary>
	/// Indicates whether the form shows candidates.
	/// </summary>
	public bool ShowCandidates { get; set; }

	/// <summary>
	/// Indicates whether the grid painter will use new algorithm to render a region (lighter).
	/// </summary>
	public bool ShowLightRegion { get; set; }

	/// <summary>
	/// <para>
	/// Indicates whether the sashimi fishes are displayed with the modifier "finned",
	/// e.g. "Finned Sashimi Fish".
	/// </para>
	/// <para>The default value is <c><see langword="false"/></c>.</para>
	/// </summary>
	[PreferenceItem<ToggleSwitch, ToggleSwitchReceivingBooleanConverter>(nameof(SettingsPage.ToggleSwitch_SashimiFishContainsKeywordFinned))]
	[PreferenceItemRoute(nameof(SettingsPage.ToggleSwitch_SashimiFishContainsKeywordFinned), nameof(SettingsPage.OptionItem_SashimiFishContainsKeywordFinned))]
	[DefaultValue("false")]
	public bool SashimiFishContainsKeywordFinned { get; set; }

	/// <summary>
	/// <para>Indicates whether the fish name is used the sized one, e.g. "3-Fish", "4-Fish", etc.. </para>
	/// <para>The default value is <c><see langword="false"/></c>.</para>
	/// </summary>
	[PreferenceItem<ToggleSwitch, ToggleSwitchReceivingBooleanConverter>(nameof(SettingsPage.ToggleSwitch_UseSizedFishName))]
	[PreferenceItemRoute(nameof(SettingsPage.ToggleSwitch_UseSizedFishName), nameof(SettingsPage.OptionItem_UseSizedFishName))]
	[DefaultValue("false")]
	public bool UseSizedFishName { get; set; }

	/// <summary>
	/// <para>Indicates the scale of values.</para>
	/// <para>The default value is <c>0.(6)</c>, i.e. <c>2.0 / 3.0</c>.</para>
	/// </summary>
	[DefaultValue("2M / 3M")]
	public decimal ValueScale { get; set; }

	/// <summary>
	/// <para>Indicates the scale of candidates.</para>
	/// <para>The default value is <c>0.8</c>, i.e. <c>4.0 / 5.0</c>.</para>
	/// </summary>
	[DefaultValue("4M / 5M")]
	public decimal CandidateScale { get; set; }

	/// <summary>
	/// <para>Indicates the grid line width of the sudoku grid to render.</para>
	/// <para>The default value is <c>1</c>.</para>
	/// </summary>
	[DefaultValue("1")]
	public double GridLineWidth { get; set; }

	/// <summary>
	/// <para>Indicates the block line width of the sudoku grid to render.</para>
	/// <para>The default value is <c>3</c>.</para>
	/// </summary>
	[DefaultValue("3")]
	public double BlockLineWidth { get; set; }

	/// <summary>
	/// Indicates the size of the cross-hatching outline width.
	/// </summary>
	public double CrosshatchingOutlineWidth { get; set; }

	/// <summary>
	/// Indicates the stroke thickness of the cross-hatching inner cross sign.
	/// </summary>
	public double CrossSignWidth { get; set; }

	/// <summary>
	/// <para>Indicates the width of the highlight cell borders.</para>
	/// <para>The default value is <c>1.5</c>.</para>
	/// </summary>
	[DefaultValue("1.5")]
	public double CellBorderWidth { get; set; }

	/// <summary>
	/// <para>Indicates the width of the highlight candidate borders.</para>
	/// <para>The default value is <c>1.5</c>.</para>
	/// </summary>
	[DefaultValue("1.5")]
	public double CandidateBorderWidth { get; set; }

	/// <summary>
	/// <para>Indicates the width of the highlight region borders.</para>
	/// <para>The default value is <c>1.5</c>.</para>
	/// </summary>
	[DefaultValue("1.5")]
	public double RegionBorderWidth { get; set; }

	/// <summary>
	/// <para>Indicates the font of given digits to render.</para>
	/// <para>The default value is <c>Fira Code</c> on symbol <c>AUTHOR_RESERVED</c> is set.</para>
	/// </summary>
	[DefaultValue("AUTHOR_RESERVED", "Fira Code", null, "_")]
	[DisallowNull]
	public string? GivenFontName { get; set; }

	/// <summary>
	/// <para>Indicates the font of modifiable digits to render.</para>
	/// <para>The default value is <c>Fira Code</c> on symbol <c>AUTHOR_RESERVED</c> is set.</para>
	/// </summary>
	[DefaultValue("AUTHOR_RESERVED", "Fira Code", null, "_")]
	[DisallowNull]
	public string? ModifiableFontName { get; set; }

	/// <summary>
	/// <para>Indicates the font of candidate digits to render.</para>
	/// <para>The default value is <c>Times New Roman</c>.</para>
	/// </summary>
	[DefaultValue("Times New Roman")]
	[DisallowNull]
	public string? CandidateFontName { get; set; }

	/// <summary>
	/// <para>Indicates the font style of the givens.</para>
	/// <para>The default value is <c><see cref="FontStyle.Normal"/></c>.</para>
	/// </summary>
	[DefaultValue("FontStyle.Normal")]
	public FontStyle GivenFontStyle { get; set; }

	/// <summary>
	/// <para>Indicates the font style of the modifiables.</para>
	/// <para>The default value is <c><see cref="FontStyle.Normal"/></c>.</para>
	/// </summary>
	[DefaultValue("FontStyle.Normal")]
	public FontStyle ModifiableFontStyle { get; set; }

	/// <summary>
	/// <para>Indicates the font style of the candidates.</para>
	/// <para>The default value is <c><see cref="FontStyle.Normal"/></c>.</para>
	/// </summary>
	[DefaultValue("FontStyle.Normal")]
	public FontStyle CandidateFontStyle { get; set; }

	/// <summary>
	/// <para>Indicates the font style of an unknown identifier.</para>
	/// <para>The default value is <c><see cref="FontStyle.Normal"/></c>.</para>
	/// </summary>
	[DefaultValue("FontStyle.Normal")]
	public FontStyle UnknownIdentfierFontStyle { get; set; }

	/// <summary>
	/// Indicates the application theme that the program behaves and displays the UI.
	/// </summary>
	/// <remarks>
	/// This property is special. It will be initialized by <see cref="Page"/> instance.
	/// </remarks>
	[PreferenceItem<ToggleSwitch, ToggleSwitchReceivingApplicationThemeConverter>(nameof(SettingsPage.ToggleSwitch_ApplicationTheme))]
	[PreferenceItemRoute(nameof(SettingsPage.ToggleSwitch_ApplicationTheme), nameof(SettingsPage.OptionItem_ApplicationTheme), PreferenceSetterMethodName = nameof(SettingsPage.Route_ToggleSwitch_ApplicationTheme))]
	public ApplicationTheme ApplicationTheme { get; set; }

	/// <summary>
	/// <para>Indicates the given digits to render which is used in light mode.</para>
	/// <para>The default value is <c>~<see cref="Colors.WhiteSmoke"/></c>.</para>
	/// </summary>
	[DefaultValue("~(UIColor)Colors.WhiteSmoke")]
	public UIColor GivenColorLight { get; set; }

	/// <summary>
	/// <para>Indicates the given digits to render which is used in dark mode.</para>
	/// <para>The default value is <c><see cref="Colors.WhiteSmoke"/></c>.</para>
	/// </summary>
	[DefaultValue("Colors.WhiteSmoke")]
	public UIColor GivenColorDark { get; set; }

	/// <summary>
	/// Indicates the color that renders modifiable digits which is used in light mode.
	/// </summary>
	[DefaultValue("Colors.Blue")]
	public UIColor ModifiableColorLight { get; set; }

	/// <summary>
	/// <para>Indicates the color that renders modifiable digits which is used in dark mode.</para>
	/// <para>The default value is <c>(R: 86, G: 156, B: 214)</c>.</para>
	/// </summary>
	[DefaultValue("new(86, 156, 214)")]
	public UIColor ModifiableColorDark { get; set; }

	/// <summary>
	/// <para>Indicates the candidate digits to render which is used in light mode.</para>
	/// <para>The default value is <c><see cref="Colors.Gray"/></c>.</para>
	/// </summary>
	[DefaultValue("Colors.Gray")]
	public UIColor CandidateColorLight { get; set; }

	/// <summary>
	/// <para>Indicates the candidate digits to render which is used in dark mode.</para>
	/// <para>The default value is <c>~<see cref="Colors.Gray"/></c>.</para>
	/// </summary>
	[DefaultValue("~(UIColor)Colors.Gray")]
	public UIColor CandidateColorDark { get; set; }

	/// <summary>
	/// <para>Indicates the color of the highlight cell borders which is used in light mode.</para>
	/// <para>The default value is <c><see cref="Colors.Blue"/></c>.</para>
	/// </summary>
	[DefaultValue("Colors.Blue")]
	public UIColor CellBorderColorLight { get; set; }

	/// <summary>
	/// <para>Indicates the color of the highlight cell borders which is used in dark mode.</para>
	/// <para>The default value is <c>(R: 86, G: 156, B: 214)</c>.</para>
	/// </summary>
	[DefaultValue("new(86, 156, 214)")]
	public UIColor CellBorderColorDark { get; set; }

	/// <summary>
	/// <para>Indicates the color of the highlight candidate borders which is used in light mode.</para>
	/// <para>The default value is <c><see cref="Colors.Blue"/></c>.</para>
	/// </summary>
	[DefaultValue("Colors.Blue")]
	public UIColor CandidateBorderColorLight { get; set; }

	/// <summary>
	/// <para>Indicates the color of the highlight candidate borders which is used in dark mode.</para>
	/// <para>The default value is <c>(R: 86, G: 156, B: 214)</c>.</para>
	/// </summary>
	[DefaultValue("new(86, 156, 214)")]
	public UIColor CandidateBorderColorDark { get; set; }

	/// <summary>
	/// <para>Indicates the color of the highlight region borders which is used in light mode.</para>
	/// <para>The default value is <c><see cref="Colors.Blue"/></c>.</para>
	/// </summary>
	[DefaultValue("Colors.Blue")]
	public UIColor RegionBorderColorLight { get; set; }

	/// <summary>
	/// <para>Indicates the color of the highlight region borders which is used in dark mode.</para>
	/// <para>The default value is <c>(R: 86, G: 156, B: 214)</c>.</para>
	/// </summary>
	[DefaultValue("new(86, 156, 214)")]
	public UIColor RegionBorderColorDark { get; set; }

	/// <summary>
	/// <para>Indicates the background color of the highlight cell borders which is used in light mode.</para>
	/// <para>The default value is <c><see cref="Colors.Blue"/></c> with the alpha <c>64</c>.</para>
	/// </summary>
	[DefaultValue("new(64, Colors.Blue)")]
	public UIColor CellBorderBackgroundColorLight { get; set; }

	/// <summary>
	/// <para>Indicates the background color of the highlight cell borders which is used in dark mode.</para>
	/// <para>The default value is <c>(A: 64, R: 86, G: 156, B: 214)</c>.</para>
	/// </summary>
	[DefaultValue("new(64, 86, 156, 214)")]
	public UIColor CellBorderBackgroundColorDark { get; set; }

	/// <summary>
	/// <para>
	/// Indicates the background color of the highlight candidate borders which is used in light mode.
	/// </para>
	/// <para>The default value is <c><see cref="Colors.Blue"/></c> with the alpha <c>64</c>.</para>
	/// </summary>
	[DefaultValue("new(64, Colors.Blue)")]
	public UIColor CandidateBorderBackgroundColorLight { get; set; }

	/// <summary>
	/// <para>
	/// Indicates the background color of the highlight candidate borders which is used in dark mode.
	/// </para>
	/// <para>The default value is <c>(A: 64, R: 86, G: 156, B: 214)</c>.</para>
	/// </summary>
	[DefaultValue("new(64, 86, 156, 214)")]
	public UIColor CandidateBorderBackgroundColorDark { get; set; }

	/// <summary>
	/// <para>Indicates the background color of the highlight region borders which is used in light mode.</para>
	/// <para>The default value is <c><see cref="Colors.Blue"/></c> with the alpha <c>64</c>.</para>
	/// </summary>
	[DefaultValue("new(64, Colors.Blue)")]
	public UIColor RegionBorderBackgroundColorLight { get; set; }

	/// <summary>
	/// <para>Indicates the background color of the highlight region borders which is used in dark mode.</para>
	/// <para>The default value is <c>(A: 64, R: 86, G: 156, B: 214)</c>.</para>
	/// </summary>
	[DefaultValue("new(64, 86, 156, 214)")]
	public UIColor RegionBorderBackgroundColorDark { get; set; }

	/// <summary>
	/// Indicates the color used for painting for focused cells.
	/// </summary>
	public UIColor FocusedCellColor { get; set; }

	/// <summary>
	/// Indicates the elimination color.
	/// </summary>
	public UIColor EliminationColor { get; set; }

	/// <summary>
	/// Indicates the cannibalism color.
	/// </summary>
	public UIColor CannibalismColor { get; set; }

	/// <summary>
	/// Indicates the chain color.
	/// </summary>
	public UIColor ChainColor { get; set; }

	/// <summary>
	/// Indicates the background color of the sudoku grid to render.
	/// </summary>
	public UIColor BackgroundColor { get; set; }

	/// <summary>
	/// <para>Indicates the grid line color of the sudoku grid to render which is used in light mode.</para>
	/// <para>The default value is <c><see cref="Colors.Black"/></c>.</para>
	/// </summary>
	[DefaultValue("Colors.Black")]
	public UIColor GridLineColorLight { get; set; }

	/// <summary>
	/// <para>Indicates the grid line color of the sudoku grid to render which is used in dark mode.</para>
	/// <para>The default value is <c><see cref="Colors.White"/></c>.</para>
	/// </summary>
	[DefaultValue("Colors.White")]
	public UIColor GridLineColorDark { get; set; }

	/// <summary>
	/// Indicates the color of the crosshatching outline.
	/// </summary>
	public UIColor CrosshatchingOutlineColor { get; set; }

	/// <summary>
	/// Indicates the color of the crosshatching inner.
	/// </summary>
	public UIColor CrosshatchingInnerColor { get; set; }

	/// <summary>
	/// Indicates the color of the unknown identifier color.
	/// </summary>
	public UIColor UnknownIdentifierColor { get; set; }

	/// <summary>
	/// Indicates the color of the cross sign.
	/// </summary>
	public UIColor CrossSignColor { get; set; }

	/// <summary>
	/// Indicates the color 1.
	/// </summary>
	public UIColor Color1 { get; set; }

	/// <summary>
	/// Indicates the color 2.
	/// </summary>
	public UIColor Color2 { get; set; }

	/// <summary>
	/// Indicates the color 3.
	/// </summary>
	public UIColor Color3 { get; set; }

	/// <summary>
	/// Indicates the color 4.
	/// </summary>
	public UIColor Color4 { get; set; }

	/// <summary>
	/// Indicates the color 5.
	/// </summary>
	public UIColor Color5 { get; set; }

	/// <summary>
	/// Indicates the color 6.
	/// </summary>
	public UIColor Color6 { get; set; }

	/// <summary>
	/// Indicates the color 7.
	/// </summary>
	public UIColor Color7 { get; set; }

	/// <summary>
	/// Indicates the color 8.
	/// </summary>
	public UIColor Color8 { get; set; }

	/// <summary>
	/// Indicates the color 9.
	/// </summary>
	public UIColor Color9 { get; set; }

	/// <summary>
	/// Indicates the color 10.
	/// </summary>
	public UIColor Color10 { get; set; }

	/// <summary>
	/// Indicates the color 11.
	/// </summary>
	public UIColor Color11 { get; set; }

	/// <summary>
	/// Indicates the color 12.
	/// </summary>
	public UIColor Color12 { get; set; }

	/// <summary>
	/// Indicates the color 13.
	/// </summary>
	public UIColor Color13 { get; set; }

	/// <summary>
	/// Indicates the color 14.
	/// </summary>
	public UIColor Color14 { get; set; }

	/// <summary>
	/// Indicates the color 15.
	/// </summary>
	public UIColor Color15 { get; set; }


	/// <inheritdoc/>
	public Preference Clone()
	{
		var result = new Preference();
		foreach (var property in from property in GetType().GetProperties() where property.CanWrite select property)
		{
			property.SetValue(result, property.GetValue(this));
		}

		return result;
	}

	/// <summary>
	/// Copies and covers the current instance via the newer instance.
	/// </summary>
	/// <param name="newPreferences">The newer instance to copy.</param>
	public void CoverBy(Preference newPreferences)
	{
		if (newPreferences is not Preference p)
		{
			return;
		}

		foreach (var property in from property in GetType().GetProperties() where property.CanWrite select property)
		{
			property.SetValue(this, property.GetValue(p));
		}
	}


	/// <summary>
	/// Try to get the result color value.
	/// </summary>
	/// <param name="colorIdentifier">The color identifier.</param>
	/// <param name="result">The result color got.</param>
	/// <returns>The <see cref="bool"/> result.</returns>
	internal bool TryGetColor(ColorIdentifier colorIdentifier, [DiscardWhen(false)] out Color result)
	{
		if (colorIdentifier.UseId)
		{
			result = colorIdentifier.Id switch
			{
				1 => Color1,
				2 => Color2,
				3 => Color3,
				4 => Color4,
				5 => Color5,
				6 => Color6,
				7 => Color7,
				8 => Color8,
				9 => Color9,
				10 => Color10,
				11 => Color11,
				12 => Color12,
				13 => Color13,
				14 => Color14,
				15 => Color15
			};

			return true;
		}
		else
		{
			result = Colors.Transparent;
			return false;
		}
	}
}
