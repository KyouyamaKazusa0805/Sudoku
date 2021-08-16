namespace Sudoku.Drawing;

/// <summary>
/// Defines the basic preferences.
/// </summary>
public interface IPreference : ICloneable<IPreference>
{
	/// <summary>
	/// Indicates whether the form shows candidates.
	/// </summary>
	bool ShowCandidates { get; set; }

	/// <summary>
	/// Indicates whether the grid painter will use new algorithm to render a region (lighter).
	/// </summary>
	bool ShowLightRegion { get; set; }

	/// <summary>
	/// Indicates the scale of values.
	/// </summary>
	decimal ValueScale { get; set; }

	/// <summary>
	/// Indicates the scale of candidates.
	/// </summary>
	decimal CandidateScale { get; set; }

	/// <summary>
	/// Indicates the grid line width of the sudoku grid to render.
	/// </summary>
	float GridLineWidth { get; set; }

	/// <summary>
	/// Indicates the block line width of the sudoku grid to render.
	/// </summary>
	float BlockLineWidth { get; set; }

	/// <summary>
	/// Indicates the font of given digits to render.
	/// </summary>
	string GivenFontName { get; set; }

	/// <summary>
	/// Indicates the font of modifiable digits to render.
	/// </summary>
	string ModifiableFontName { get; set; }

	/// <summary>
	/// Indicates the font of candidate digits to render.
	/// </summary>
	string CandidateFontName { get; set; }

	/// <summary>
	/// Indicates the font style of the givens.
	/// </summary>
	FontStyle GivenFontStyle { get; set; }

	/// <summary>
	/// Indicates the font style of the modifiables.
	/// </summary>
	FontStyle ModifiableFontStyle { get; set; }

	/// <summary>
	/// Indicates the font style of the candidates.
	/// </summary>
	FontStyle CandidateFontStyle { get; set; }

	/// <summary>
	/// Indicates the font style of an unknown identifier.
	/// </summary>
	FontStyle UnknownIdentfierFontStyle { get; set; }

	/// <summary>
	/// Indicates the given digits to render.
	/// </summary>
	Color GivenColor { get; set; }

	/// <summary>
	/// Indicates the modifiable digits to render.
	/// </summary>
	Color ModifiableColor { get; set; }

	/// <summary>
	/// Indicates the candidate digits to render.
	/// </summary>
	Color CandidateColor { get; set; }

	/// <summary>
	/// Indicates the color used for painting for focused cells.
	/// </summary>
	Color FocusedCellColor { get; set; }

	/// <summary>
	/// Indicates the elimination color.
	/// </summary>
	Color EliminationColor { get; set; }

	/// <summary>
	/// Indicates the cannibalism color.
	/// </summary>
	Color CannibalismColor { get; set; }

	/// <summary>
	/// Indicates the chain color.
	/// </summary>
	Color ChainColor { get; set; }

	/// <summary>
	/// Indicates the background color of the sudoku grid to render.
	/// </summary>
	Color BackgroundColor { get; set; }

	/// <summary>
	/// Indicates the grid line color of the sudoku grid to render.
	/// </summary>
	Color GridLineColor { get; set; }

	/// <summary>
	/// Indicates the block line color of the sudoku grid to render.
	/// </summary>
	Color BlockLineColor { get; set; }

	/// <summary>
	/// Indicates the color of the crosshatching outline.
	/// </summary>
	Color CrosshatchingOutlineColor { get; set; }

	/// <summary>
	/// Indicates the color of the crosshatching inner.
	/// </summary>
	Color CrosshatchingInnerColor { get; set; }

	/// <summary>
	/// Indicates the color of the unknown identifier color.
	/// </summary>
	Color UnknownIdentifierColor { get; set; }

	/// <summary>
	/// Indicates the color of the cross sign.
	/// </summary>
	Color CrossSignColor { get; set; }

	/// <summary>
	/// Indicates the color 1.
	/// </summary>
	Color Color1 { get; set; }

	/// <summary>
	/// Indicates the color 2.
	/// </summary>
	Color Color2 { get; set; }

	/// <summary>
	/// Indicates the color 3.
	/// </summary>
	Color Color3 { get; set; }

	/// <summary>
	/// Indicates the color 4.
	/// </summary>
	Color Color4 { get; set; }

	/// <summary>
	/// Indicates the color 5.
	/// </summary>
	Color Color5 { get; set; }

	/// <summary>
	/// Indicates the color 6.
	/// </summary>
	Color Color6 { get; set; }

	/// <summary>
	/// Indicates the color 7.
	/// </summary>
	Color Color7 { get; set; }

	/// <summary>
	/// Indicates the color 8.
	/// </summary>
	Color Color8 { get; set; }

	/// <summary>
	/// Indicates the color 9.
	/// </summary>
	Color Color9 { get; set; }

	/// <summary>
	/// Indicates the color 10.
	/// </summary>
	Color Color10 { get; set; }

	/// <summary>
	/// Indicates the color 11.
	/// </summary>
	Color Color11 { get; set; }

	/// <summary>
	/// Indicates the color 12.
	/// </summary>
	Color Color12 { get; set; }

	/// <summary>
	/// Indicates the color 13.
	/// </summary>
	Color Color13 { get; set; }

	/// <summary>
	/// Indicates the color 14.
	/// </summary>
	Color Color14 { get; set; }

	/// <summary>
	/// Indicates the color 15.
	/// </summary>
	Color Color15 { get; set; }


	/// <summary>
	/// Copies and covers the current instance via the newer instance.
	/// </summary>
	/// <param name="newPreferences">The newer instance to copy.</param>
	void CoverBy(IPreference newPreferences);

	/// <summary>
	/// Try to get the result color value.
	/// </summary>
	/// <param name="colorIdentifier">The color identifier.</param>
	/// <param name="result">The result color got.</param>
	/// <returns>The <see cref="bool"/> result.</returns>
	protected internal bool TryGetColor(ColorIdentifier colorIdentifier, out Color result)
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
			result = Color.Transparent;
			return false;
		}
	}
}
