using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.Json.Serialization;
using Sudoku.DocComments;

namespace Sudoku.Painting
{
	/// <summary>
	/// Indicates a theme.
	/// </summary>
	public sealed class Theme : ICloneable<Theme>
	{
		/// <inheritdoc cref="DefaultConstructor"/>
		public Theme()
		{
		}

		/// <summary>
		/// (Copy constructor) Copies the old theme to the current instance.
		/// </summary>
		/// <param name="oldTheme">The old theme.</param>
		public Theme(Theme oldTheme) => oldTheme.CopyTo(this);


		/// <summary>
		/// Copies the current instance to the new instance specified as the parameter.
		/// </summary>
		/// <param name="newTheme">The new theme.</param>
		public void CopyTo(Theme newTheme)
		{
			newTheme.BackgroundColor = BackgroundColor;
			newTheme.GridLineColor = GridLineColor;
			newTheme.BlockLineColor = BlockLineColor;
			newTheme.FocusedCellsColor = FocusedCellsColor;
			newTheme.EliminationColor = EliminationColor;
			newTheme.CannibalismColor = CannibalismColor;
			newTheme.GivenColor = GivenColor;
			newTheme.ModifiableColor = ModifiableColor;
			newTheme.CandidateColor = CandidateColor;
			newTheme.ChainColor = ChainColor;
			newTheme.CrosshatchingOutlineColor = CrosshatchingOutlineColor;
			newTheme.CrosshatchingInnerColor = CrosshatchingInnerColor;
			newTheme.CrossSignColor = CrossSignColor;
			newTheme.Color1 = Color1;
			newTheme.Color2 = Color2;
			newTheme.Color3 = Color3;
			newTheme.Color4 = Color4;
			newTheme.Color5 = Color5;
			newTheme.Color6 = Color6;
			newTheme.Color7 = Color7;
			newTheme.Color8 = Color8;
			newTheme.Color9 = Color9;
			newTheme.Color10 = Color10;
			newTheme.Color11 = Color11;
			newTheme.Color12 = Color12;
			newTheme.Color13 = Color13;
			newTheme.Color14 = Color14;
			newTheme.Color15 = Color15;
		}


		/// <summary>
		/// Indicates the background color of the grid. The default value is <see cref="Color.White"/>.
		/// </summary>
		public Color BackgroundColor { get; set; } = Color.White;

		/// <summary>
		/// Indicates the grid line color of the grid. The default value is <see cref="Color.Gray"/>.
		/// </summary>
		public Color GridLineColor { get; set; } = Color.Black;

		/// <summary>
		/// Indicates the block line color of the grid. The default value is <see cref="Color.Gray"/>.
		/// </summary>
		public Color BlockLineColor { get; set; } = Color.Black;

		/// <summary>
		/// Indicates the focused cells color. The default value is <see cref="Color.Yellow"/>
		/// with the alpha 32.
		/// </summary>
		public Color FocusedCellsColor { get; set; } = Color.FromArgb(32, Color.Yellow);

		/// <summary>
		/// Indicates the elimination color.
		/// </summary>
		public Color EliminationColor { get; set; } = Color.FromArgb(255, 118, 132);

		/// <summary>
		/// Indicates the cannibalism color.
		/// </summary>
		public Color CannibalismColor { get; set; } = Color.FromArgb(235, 0, 0);

		/// <summary>
		/// Indicates the given digits to render. The default value is <see cref="Color.Black"/>.
		/// </summary>
		public Color GivenColor { get; set; } = Color.Black;

		/// <summary>
		/// Indicates the modifiable digits to render. The default value is <see cref="Color.Blue"/>.
		/// </summary>
		public Color ModifiableColor { get; set; } = Color.Blue;

		/// <summary>
		/// Indicates the candidate digits to render. The default value is <see cref="Color.DimGray"/>.
		/// </summary>
		public Color CandidateColor { get; set; } = Color.DimGray;

		/// <summary>
		/// Indicates the chain color.
		/// </summary>
		public Color ChainColor { get; set; } = Color.Red;

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
				[1] = Color2,
				[2] = Color3,
				[3] = Color4,
				[4] = Color9,
				[5] = Color10,
				[6] = Color11,
				[7] = Color12,
				[8] = Color13,
				[9] = Color14,
				[10] = Color15,
			};


		/// <inheritdoc/>
		public Theme Clone() =>
			new()
			{
				BackgroundColor = BackgroundColor,
				GridLineColor = GridLineColor,
				BlockLineColor = BlockLineColor,
				FocusedCellsColor = FocusedCellsColor,
				EliminationColor = EliminationColor,
				CannibalismColor = CannibalismColor,
				GivenColor = GivenColor,
				ModifiableColor = ModifiableColor,
				CandidateColor = CandidateColor,
				ChainColor = ChainColor,
				CrosshatchingOutlineColor = CrosshatchingOutlineColor,
				CrosshatchingInnerColor = CrosshatchingInnerColor,
				CrossSignColor = CrossSignColor,
				Color1 = Color1,
				Color2 = Color2,
				Color3 = Color3,
				Color4 = Color4,
				Color5 = Color5,
				Color6 = Color6,
				Color7 = Color7,
				Color8 = Color8,
				Color9 = Color9,
				Color10 = Color10,
				Color11 = Color11,
				Color12 = Color12,
				Color13 = Color13,
				Color14 = Color14,
				Color15 = Color15
			};
	}
}
