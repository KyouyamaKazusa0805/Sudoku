namespace Sudoku.UI.Data;

/// <summary>
/// Provides with the preference instance.
/// </summary>
public sealed class Preference : ICloneable<Preference>
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
	/// Indicates whether the sashimi fishes are displayed with the modifier "finned", i.e. Finned Sashimi Fish.
	/// </summary>
	public bool SashimiFishContainsKeywordFinned { get; set; }

	/// <summary>
	/// Indicates whether the fish name is used the sized one, i.e. 3-Fish, 4-Fish, etc..
	/// </summary>
	public bool UseSizedFishName { get; set; }

	/// <summary>
	/// Indicates the scale of values.
	/// </summary>
	public decimal ValueScale { get; set; }

	/// <summary>
	/// Indicates the scale of candidates.
	/// </summary>
	public decimal CandidateScale { get; set; }

	/// <summary>
	/// Indicates the grid line width of the sudoku grid to render.
	/// </summary>
	public double GridLineWidth { get; set; }

	/// <summary>
	/// Indicates the block line width of the sudoku grid to render.
	/// </summary>
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
	/// Indicates the font of given digits to render.
	/// </summary>
	public string? GivenFontName { get; set; }

	/// <summary>
	/// Indicates the font of modifiable digits to render.
	/// </summary>
	public string? ModifiableFontName { get; set; }

	/// <summary>
	/// Indicates the font of candidate digits to render.
	/// </summary>
	public string? CandidateFontName { get; set; }

	/// <summary>
	/// Indicates the font style of the givens.
	/// </summary>
	public FontStyle GivenFontStyle { get; set; }

	/// <summary>
	/// Indicates the font style of the modifiables.
	/// </summary>
	public FontStyle ModifiableFontStyle { get; set; }

	/// <summary>
	/// Indicates the font style of the candidates.
	/// </summary>
	public FontStyle CandidateFontStyle { get; set; }

	/// <summary>
	/// Indicates the font style of an unknown identifier.
	/// </summary>
	public FontStyle UnknownIdentfierFontStyle { get; set; }

	/// <summary>
	/// Indicates the given digits to render.
	/// </summary>
	public Color GivenColor { get; set; }

	/// <summary>
	/// Indicates the modifiable digits to render.
	/// </summary>
	public Color ModifiableColor { get; set; }

	/// <summary>
	/// Indicates the candidate digits to render.
	/// </summary>
	public Color CandidateColor { get; set; }

	/// <summary>
	/// Indicates the color used for painting for focused cells.
	/// </summary>
	public Color FocusedCellColor { get; set; }

	/// <summary>
	/// Indicates the elimination color.
	/// </summary>
	public Color EliminationColor { get; set; }

	/// <summary>
	/// Indicates the cannibalism color.
	/// </summary>
	public Color CannibalismColor { get; set; }

	/// <summary>
	/// Indicates the chain color.
	/// </summary>
	public Color ChainColor { get; set; }

	/// <summary>
	/// Indicates the background color of the sudoku grid to render.
	/// </summary>
	public Color BackgroundColor { get; set; }

	/// <summary>
	/// Indicates the grid line color of the sudoku grid to render.
	/// </summary>
	public Color GridLineColor { get; set; }

	/// <summary>
	/// Indicates the block line color of the sudoku grid to render.
	/// </summary>
	public Color BlockLineColor { get; set; }

	/// <summary>
	/// Indicates the color of the crosshatching outline.
	/// </summary>
	public Color CrosshatchingOutlineColor { get; set; }

	/// <summary>
	/// Indicates the color of the crosshatching inner.
	/// </summary>
	public Color CrosshatchingInnerColor { get; set; }

	/// <summary>
	/// Indicates the color of the unknown identifier color.
	/// </summary>
	public Color UnknownIdentifierColor { get; set; }

	/// <summary>
	/// Indicates the color of the cross sign.
	/// </summary>
	public Color CrossSignColor { get; set; }

	/// <summary>
	/// Indicates the color 1.
	/// </summary>
	public Color Color1 { get; set; }

	/// <summary>
	/// Indicates the color 2.
	/// </summary>
	public Color Color2 { get; set; }

	/// <summary>
	/// Indicates the color 3.
	/// </summary>
	public Color Color3 { get; set; }

	/// <summary>
	/// Indicates the color 4.
	/// </summary>
	public Color Color4 { get; set; }

	/// <summary>
	/// Indicates the color 5.
	/// </summary>
	public Color Color5 { get; set; }

	/// <summary>
	/// Indicates the color 6.
	/// </summary>
	public Color Color6 { get; set; }

	/// <summary>
	/// Indicates the color 7.
	/// </summary>
	public Color Color7 { get; set; }

	/// <summary>
	/// Indicates the color 8.
	/// </summary>
	public Color Color8 { get; set; }

	/// <summary>
	/// Indicates the color 9.
	/// </summary>
	public Color Color9 { get; set; }

	/// <summary>
	/// Indicates the color 10.
	/// </summary>
	public Color Color10 { get; set; }

	/// <summary>
	/// Indicates the color 11.
	/// </summary>
	public Color Color11 { get; set; }

	/// <summary>
	/// Indicates the color 12.
	/// </summary>
	public Color Color12 { get; set; }

	/// <summary>
	/// Indicates the color 13.
	/// </summary>
	public Color Color13 { get; set; }

	/// <summary>
	/// Indicates the color 14.
	/// </summary>
	public Color Color14 { get; set; }

	/// <summary>
	/// Indicates the color 15.
	/// </summary>
	public Color Color15 { get; set; }


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
	internal bool TryGetColor(ColorIdentifier colorIdentifier, out Color result)
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
