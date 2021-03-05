using System.Collections.Generic;
using System.Drawing;
using System.Text.Json.Serialization;

namespace Sudoku.Drawing
{
	partial class Settings
	{
		/// <summary>
		/// <para>Indicates whether the form shows candidates.</para>
		/// <para>The value is <see langword="true"/> in default case.</para>
		/// </summary>
		public bool ShowCandidates { get; set; } = true;

		/// <summary>
		/// <para>Indicates whether the grid painter will use new algorithm to render a region (lighter).</para>
		/// <para>The value is <see langword="true"/> in author reserved case.</para>
		/// </summary>
		public bool ShowLightRegion { get; set; }
#if AUTHOR_RESERVED
		= true;
#endif

		/// <summary>
		/// <para>Indicates the scale of values.</para>
		/// <para>The value is <c>0.9M</c> in default case.</para>
		/// </summary>
		public decimal ValueScale { get; set; } = .9M;

		/// <summary>
		/// <para>Indicates the scale of candidates.</para>
		/// <para>The value is <c>0.3M</c> in default case.</para>
		/// </summary>
		public decimal CandidateScale { get; set; } = .3M;

		/// <summary>
		/// <para>
		/// Indicates the grid line width of the sudoku grid to render.
		/// </para>
		/// <para>The value is <c>1.5F</c> in default case.</para>
		/// </summary>
		public float GridLineWidth { get; set; } = 1.5F;

		/// <summary>
		/// <para>
		/// Indicates the block line width of the sudoku grid to render.
		/// </para>
		/// <para>The value is <c>3F</c> in default case.</para>
		/// </summary>
		public float BlockLineWidth { get; set; } = 3F;

		/// <summary>
		/// <para>
		/// Indicates the font of given digits to render.
		/// </para>
		/// <para>
		/// The value is <c>"Fira Code"</c> in author-reserved environment,
		/// <c>"Arial"</c> in other ones.
		/// </para>
		/// </summary>
		public string GivenFontName { get; set; }
#if AUTHOR_RESERVED
		= "Fira Code";
#else
		= "Arial";
#endif

		/// <summary>
		/// <para>
		/// Indicates the font of modifiable digits to render.
		/// </para>
		/// <para>
		/// The value is <c>"Fira Code"</c> in author-reserved environment,
		/// <c>"Arial"</c> in other ones.
		/// </para>
		/// </summary>
		public string ModifiableFontName { get; set; }
#if AUTHOR_RESERVED
		= "Fira Code";
#else
		= "Arial";
#endif

		/// <summary>
		/// <para>
		/// Indicates the font of candidate digits to render.
		/// </para>
		/// <para>
		/// The value is <c>"Fira Code"</c> in author-reserved environment,
		/// <c>"Arial"</c> in other ones.
		/// </para>
		/// </summary>
		public string CandidateFontName { get; set; }
#if AUTHOR_RESERVED
		= "Fira Code";
#else
		= "Arial";
#endif

		/// <summary>
		/// <para>Indicates the given digits to render.</para>
		/// <para>The value is <see cref="Color.Black"/> in default case.</para>
		/// </summary>
		public Color GivenColor { get; set; } = Color.Black;

		/// <summary>
		/// <para>Indicates the modifiable digits to render.</para>
		/// <para>The value is <see cref="Color.Blue"/> in default case.</para>
		/// </summary>
		public Color ModifiableColor { get; set; } = Color.Blue;

		/// <summary>
		/// <para>Indicates the candidate digits to render.</para>
		/// <para>The value is <see cref="Color.DimGray"/> in default case.</para>
		/// </summary>
		public Color CandidateColor { get; set; } = Color.DimGray;

		/// <summary>
		/// <para>Indicates the color used for painting for focused cells.</para>
		/// <para>
		/// The value is <c>#20FFFF00</c> (<see cref="Color.Yellow"/>
		/// with alpha <c>32</c>) in default case.
		/// </para>
		/// </summary>
		public Color FocusedCellColor { get; set; } = Color.FromArgb(32, Color.Yellow);

		/// <summary>
		/// Indicates the elimination color.
		/// </summary>
		public Color EliminationColor { get; set; } = Color.FromArgb(255, 118, 132);

		/// <summary>
		/// Indicates the cannibalism color.
		/// </summary>
		public Color CannibalismColor { get; set; } = Color.FromArgb(235, 0, 0);

		/// <summary>
		/// Indicates the chain color.
		/// </summary>
		public Color ChainColor { get; set; } = Color.Red;

		/// <summary>
		/// <para>Indicates the background color of the sudoku grid to render.</para>
		/// <para>The value is <see cref="Color.White"/> in default case.</para>
		/// </summary>
		public Color BackgroundColor { get; set; } = Color.White;

		/// <summary>
		/// <para>Indicates the grid line color of the sudoku grid to render.</para>
		/// <para>The value is <see cref="Color.Black"/> in default case.</para>
		/// </summary>
		public Color GridLineColor { get; set; } = Color.Black;

		/// <summary>
		/// <para>
		/// Indicates the block line color of the sudoku grid to render.
		/// </para>
		/// <para>The value is <see cref="Color.Black"/> in default case.</para>
		/// </summary>
		public Color BlockLineColor { get; set; } = Color.Black;

		/// <summary>
		/// <para>
		/// Indicates the color of the crosshatching outline.
		/// </para>
		/// <para>The value is <see cref="Color.Black"/> with the alpha 192 in debugger mode,
		/// <see cref="Color.SkyBlue"/> with the alpha 192 in release mode in default case.</para>
		/// </summary>
		public Color CrosshatchingOutlineColor { get; set; }
#if AUTHOR_RESERVED
		= Color.FromArgb(192, Color.Black);
#else
		= Color.FromArgb(192, Color.SkyBlue);
#endif

		/// <summary>
		/// <para>
		/// Indicates the color of the crosshatching inner.
		/// </para>
		/// <para>
		/// The value is <see cref="Color.Transparent"/> in author-reserved environment,
		/// <see cref="Color.SkyBlue"/> in other ones in default case.
		/// </para>
		/// </summary>
		public Color CrosshatchingInnerColor { get; set; }
#if AUTHOR_RESERVED
		= Color.Transparent;
#else
		= Color.SkyBlue;
#endif

		/// <summary>
		/// <para>
		/// Indicates the color of the cross sign.
		/// </para>
		/// <para>The value is <see cref="Color.Red"/> with the alpha 192 in default case.</para>
		/// </summary>
		public Color CrossSignColor { get; set; } = Color.FromArgb(192, Color.Red);

		/// <summary>
		/// Indicates the color 1.
		/// </summary>
		public Color Color1 { get; set; } = Color.FromArgb(63, 218, 101);

		/// <summary>
		/// Indicates the color 2.
		/// </summary>
		public Color Color2 { get; set; } = Color.FromArgb(255, 192, 89);

		/// <summary>
		/// Indicates the color 3.
		/// </summary>
		public Color Color3 { get; set; } = Color.FromArgb(127, 187, 255);

		/// <summary>
		/// Indicates the color 4.
		/// </summary>
		public Color Color4 { get; set; } = Color.FromArgb(216, 178, 255);

		/// <summary>
		/// Indicates the color 5.
		/// </summary>
		public Color Color5 { get; set; } = Color.FromArgb(197, 232, 140);

		/// <summary>
		/// Indicates the color 6.
		/// </summary>
		public Color Color6 { get; set; } = Color.FromArgb(255, 203, 203);

		/// <summary>
		/// Indicates the color 7.
		/// </summary>
		public Color Color7 { get; set; } = Color.FromArgb(178, 223, 223);

		/// <summary>
		/// Indicates the color 8.
		/// </summary>
		public Color Color8 { get; set; } = Color.FromArgb(252, 220, 165);

		/// <summary>
		/// Indicates the color 9.
		/// </summary>
		public Color Color9 { get; set; } = Color.FromArgb(255, 255, 150);

		/// <summary>
		/// Indicates the color 10.
		/// </summary>
		public Color Color10 { get; set; } = Color.FromArgb(247, 222, 143);

		/// <summary>
		/// Indicates the color 11.
		/// </summary>
		public Color Color11 { get; set; } = Color.FromArgb(220, 212, 252);

		/// <summary>
		/// Indicates the color 12.
		/// </summary>
		public Color Color12 { get; set; } = Color.FromArgb(255, 118, 132);

		/// <summary>
		/// Indicates the color 13.
		/// </summary>
		public Color Color13 { get; set; } = Color.FromArgb(206, 251, 237);

		/// <summary>
		/// Indicates the color 14.
		/// </summary>
		public Color Color14 { get; set; } = Color.FromArgb(215, 255, 215);

		/// <summary>
		/// Indicates the color 15.
		/// </summary>
		public Color Color15 { get; set; } = Color.FromArgb(192, 192, 192);

		/// <summary>
		/// Indicates the palette colors.
		/// </summary>
		[JsonIgnore]
		public IReadOnlyDictionary<long, Color> PaletteColors =>
			new Dictionary<long, Color>
			{
				[-4] = Color8,
				[-3] = Color7,
				[-2] = Color6,
				[-1] = Color5,
				[0] = Color1,
				[1] = Color3,
				[2] = Color2,
				[3] = Color4,
				[4] = Color9,
				[5] = Color10,
				[6] = Color11,
				[7] = Color12,
				[8] = Color13,
				[9] = Color14,
				[10] = Color15,
			};
	}
}
