namespace Sudoku.Painting
{
	partial class PreferencesBase
	{
		/// <summary>
		/// Indicates whether the form shows candidates. The default value is <see langword="true"/>.
		/// </summary>
		public bool ShowCandidates { get; set; } = true;

		/// <summary>
		/// Indicates whether the program uses the light theme. The default value is <see langword="true"/>.
		/// </summary>
		public bool IsLightTheme { get; set; } = true;

		/// <summary>
		/// Indicates the scale of values. The default value is 0.9.
		/// </summary>
		public float ValueScale { get; set; } = .9F;

		/// <summary>
		/// Indicates the scale of candidates. The default value is 0.3.
		/// </summary>
		public float CandidateScale { get; set; } = .3F;

		/// <summary>
		/// Indicates the grid line width. The default value is 1.5.
		/// </summary>
		public float GridLineWidth { get; set; } = 1.5F;

		/// <summary>
		/// Indicates the block line width. The default value is 3.
		/// </summary>
		public float BlockLineWidth { get; set; } = 3F;

		/// <summary>
		/// Indicates the font of given digits to render. The default value is:
		/// <list type="table">
		/// <item>
		/// <term><c>"Fira Code"</c></term>
		/// <description>When the conditional compilation symbol <c>AUTHOR_RESERVED</c> is enabled</description>
		/// </item>
		/// <item>
		/// <term><c>"Arial"</c></term>
		/// <description>In other cases.</description>
		/// </item>
		/// </list>
		/// </summary>
		public string GivenFontName { get; set; }
#if AUTHOR_RESERVED
		= "Fira Code";
#else
		= "Arial";
#endif

		/// <summary>
		/// Indicates the font of modifiable digits to render. The default value is:
		/// <list type="table">
		/// <item>
		/// <term><c>"Fira Code"</c></term>
		/// <description>When the conditional compilation symbol <c>AUTHOR_RESERVED</c> is enabled</description>
		/// </item>
		/// <item>
		/// <term><c>"Arial"</c></term>
		/// <description>In other cases</description>
		/// </item>
		/// </list>
		/// </summary>
		public string ModifiableFontName { get; set; }
#if AUTHOR_RESERVED
		= "Fira Code";
#else
		= "Arial";
#endif

		/// <summary>
		/// Indicates the font of candidate digits to render. The default value is:
		/// <list type="table">
		/// <item>
		/// <term><c>"Fira Code"</c></term>
		/// <description>When the conditional compilation symbol <c>AUTHOR_RESERVED</c> is enabled</description>
		/// </item>
		/// <item>
		/// <term><c>"Arial"</c></term>
		/// <description>In other cases</description>
		/// </item>
		/// </list>
		/// </summary>
		public string CandidateFontName { get; set; }
#if AUTHOR_RESERVED
		= "Fira Code";
#else
		= "Arial";
#endif

		/// <summary>
		/// Indicates the light theme.
		/// </summary>
		public Theme LightTheme { get; set; } = new(Themes.LightTheme);

		/// <summary>
		/// Indicates the dark theme.
		/// </summary>
		public Theme DarkTheme { get; set; } = new(Themes.DarkTheme);
	}
}
