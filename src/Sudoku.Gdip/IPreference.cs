namespace Sudoku.Gdip;

/// <summary>
/// Defines the basic preferences.
/// </summary>
public interface IPreference : ICloneable
{
	/// <summary>
	/// Indicates whether the form shows candidates.
	/// </summary>
	public abstract bool ShowCandidates { get; set; }

	/// <summary>
	/// Indicates whether the grid painter will use new algorithm to render a house (lighter).
	/// </summary>
	public abstract bool ShowLightHouse { get; set; }

	/// <summary>
	/// Indicates the scale of values.
	/// </summary>
	public abstract decimal ValueScale { get; set; }

	/// <summary>
	/// Indicates the scale of candidates.
	/// </summary>
	public abstract decimal CandidateScale { get; set; }

	/// <summary>
	/// Indicates the grid line width of the sudoku grid to render.
	/// </summary>
	public abstract float GridLineWidth { get; set; }

	/// <summary>
	/// Indicates the block line width of the sudoku grid to render.
	/// </summary>
	public abstract float BlockLineWidth { get; set; }

	/// <summary>
	/// Indicates the font of given digits to render.
	/// </summary>
	public abstract string? GivenFontName { get; set; }

	/// <summary>
	/// Indicates the font of modifiable digits to render.
	/// </summary>
	public abstract string? ModifiableFontName { get; set; }

	/// <summary>
	/// Indicates the font of candidate digits to render.
	/// </summary>
	public abstract string? CandidateFontName { get; set; }

	/// <summary>
	/// Indicates the font style of the givens.
	/// </summary>
	public abstract FontStyle GivenFontStyle { get; set; }

	/// <summary>
	/// Indicates the font style of the modifiables.
	/// </summary>
	public abstract FontStyle ModifiableFontStyle { get; set; }

	/// <summary>
	/// Indicates the font style of the candidates.
	/// </summary>
	public abstract FontStyle CandidateFontStyle { get; set; }

	/// <summary>
	/// Indicates the font style of an unknown.
	/// </summary>
	public abstract FontStyle UnknownFontStyle { get; set; }

	/// <summary>
	/// Indicates the given digits to render.
	/// </summary>
	public abstract Color GivenColor { get; set; }

	/// <summary>
	/// Indicates the modifiable digits to render.
	/// </summary>
	public abstract Color ModifiableColor { get; set; }

	/// <summary>
	/// Indicates the candidate digits to render.
	/// </summary>
	public abstract Color CandidateColor { get; set; }

	/// <summary>
	/// Indicates the color used for painting for focused cells.
	/// </summary>
	public abstract Color FocusedCellColor { get; set; }

	/// <summary>
	/// Indicates the elimination color.
	/// </summary>
	public abstract Color EliminationColor { get; set; }

	/// <summary>
	/// Indicates the cannibalism color.
	/// </summary>
	public abstract Color CannibalismColor { get; set; }

	/// <summary>
	/// Indicates the chain color.
	/// </summary>
	public abstract Color ChainColor { get; set; }

	/// <summary>
	/// Indicates the background color of the sudoku grid to render.
	/// </summary>
	public abstract Color BackgroundColor { get; set; }

	/// <summary>
	/// Indicates the grid line color of the sudoku grid to render.
	/// </summary>
	public abstract Color GridLineColor { get; set; }

	/// <summary>
	/// Indicates the block line color of the sudoku grid to render.
	/// </summary>
	public abstract Color BlockLineColor { get; set; }

	/// <summary>
	/// Indicates the color of the crosshatching outline.
	/// </summary>
	public abstract Color CrosshatchingOutlineColor { get; set; }

	/// <summary>
	/// Indicates the color of the crosshatching inner.
	/// </summary>
	public abstract Color CrosshatchingInnerColor { get; set; }

	/// <summary>
	/// Indicates the color of the unknown identifier color.
	/// </summary>
	public abstract Color UnknownIdentifierColor { get; set; }

	/// <summary>
	/// Indicates the color of the cross sign.
	/// </summary>
	public abstract Color CrossSignColor { get; set; }

	/// <summary>
	/// Indicates the color 1.
	/// </summary>
	public abstract Color Color1 { get; set; }

	/// <summary>
	/// Indicates the color 2.
	/// </summary>
	public abstract Color Color2 { get; set; }

	/// <summary>
	/// Indicates the color 3.
	/// </summary>
	public abstract Color Color3 { get; set; }

	/// <summary>
	/// Indicates the color 4.
	/// </summary>
	public abstract Color Color4 { get; set; }

	/// <summary>
	/// Indicates the color 5.
	/// </summary>
	public abstract Color Color5 { get; set; }

	/// <summary>
	/// Indicates the color 6.
	/// </summary>
	public abstract Color Color6 { get; set; }

	/// <summary>
	/// Indicates the color 7.
	/// </summary>
	public abstract Color Color7 { get; set; }

	/// <summary>
	/// Indicates the color 8.
	/// </summary>
	public abstract Color Color8 { get; set; }

	/// <summary>
	/// Indicates the color 9.
	/// </summary>
	public abstract Color Color9 { get; set; }

	/// <summary>
	/// Indicates the color 10.
	/// </summary>
	public abstract Color Color10 { get; set; }

	/// <summary>
	/// Indicates the color 11.
	/// </summary>
	public abstract Color Color11 { get; set; }

	/// <summary>
	/// Indicates the color 12.
	/// </summary>
	public abstract Color Color12 { get; set; }

	/// <summary>
	/// Indicates the color 13.
	/// </summary>
	public abstract Color Color13 { get; set; }

	/// <summary>
	/// Indicates the color 14.
	/// </summary>
	public abstract Color Color14 { get; set; }

	/// <summary>
	/// Indicates the color 15.
	/// </summary>
	Color Color15 { get; set; }


	/// <summary>
	/// Copies and covers the current instance via the newer instance.
	/// </summary>
	/// <param name="newPreferences">The newer instance to copy.</param>
	public abstract void CoverBy(IPreference newPreferences);

	/// <summary>
	/// Try to get the result color value.
	/// </summary>
	/// <param name="colorIdentifier">The color identifier.</param>
	/// <param name="result">The result color got.</param>
	/// <returns>The <see cref="bool"/> result.</returns>
	/// <exception cref="InvalidOperationException">Throws when the ID is invalid.</exception>
	protected internal sealed bool TryGetColor(Identifier colorIdentifier, out Color result)
	{
		if (colorIdentifier is { Mode: IdentifierColorMode.Id, Id: var id })
		{
			result = id switch
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
				15 => Color15,
				_ => throw new InvalidOperationException("The specified ID is invalid.")
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
