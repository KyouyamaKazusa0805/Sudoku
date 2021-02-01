using System.Drawing;
using Sudoku.DocComments;

namespace Sudoku.Painting
{
	/// <summary>
	/// Indicates a theme.
	/// </summary>
	public sealed partial class Theme
	{
		/// <inheritdoc cref="DefaultConstructor"/>
		public Theme()
		{
		}

		/// <summary>
		/// (Copy constructor) Copies the old theme to the current instance. After the constructor
		/// called, two instances are totally independent.
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
		/// Try to get the palette color using the specified index.
		/// </summary>
		/// <param name="id">The ID value.</param>
		/// <param name="result">(<see langword="out"/> parameter) The result color.</param>
		/// <returns>A <see cref="bool"/> indicating whether the operation is successful.</returns>
		public bool TryGetPaletteColor(long id, out Color result)
		{
			if (id is >= 0 and < 15)
			{
				result = id switch
				{
					0 => Color1,
					1 => Color2,
					2 => Color3,
					3 => Color4,
					4 => Color5,
					5 => Color6,
					6 => Color7,
					7 => Color8,
					8 => Color9,
					9 => Color10,
					10 => Color11,
					11 => Color12,
					12 => Color13,
					13 => Color14,
					14 => Color15
				};
				return true;
			}
			else
			{
				result = default;
				return false;
			}
		}
	}
}
