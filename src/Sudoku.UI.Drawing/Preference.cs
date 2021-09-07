namespace Sudoku.UI.Drawing;

/// <summary>
/// Provides with the preference instance.
/// </summary>
public sealed class Preference : IPreference
{
	/// <inheritdoc/>
	public bool ShowCandidates { get; set; }

	/// <inheritdoc/>
	public bool ShowLightRegion { get; set; }

	/// <inheritdoc/>
	public decimal ValueScale { get; set; }

	/// <inheritdoc/>
	public decimal CandidateScale { get; set; }

	/// <inheritdoc/>
	public double GridLineWidth { get; set; }

	/// <inheritdoc/>
	public double BlockLineWidth { get; set; }

	/// <inheritdoc/>
	public double CrosshatchingOutlineWidth { get; set; }

	/// <inheritdoc/>
	public double CrossSignWidth { get; set; }

	/// <inheritdoc/>
	public string? GivenFontName { get; set; }

	/// <inheritdoc/>
	public string? ModifiableFontName { get; set; }

	/// <inheritdoc/>
	public string? CandidateFontName { get; set; }

	/// <inheritdoc/>
	public FontStyle GivenFontStyle { get; set; }

	/// <inheritdoc/>
	public FontStyle ModifiableFontStyle { get; set; }

	/// <inheritdoc/>
	public FontStyle CandidateFontStyle { get; set; }

	/// <inheritdoc/>
	public FontStyle UnknownIdentfierFontStyle { get; set; }

	/// <inheritdoc/>
	public Color GivenColor { get; set; }

	/// <inheritdoc/>
	public Color ModifiableColor { get; set; }

	/// <inheritdoc/>
	public Color CandidateColor { get; set; }

	/// <inheritdoc/>
	public Color FocusedCellColor { get; set; }

	/// <inheritdoc/>
	public Color EliminationColor { get; set; }

	/// <inheritdoc/>
	public Color CannibalismColor { get; set; }

	/// <inheritdoc/>
	public Color ChainColor { get; set; }

	/// <inheritdoc/>
	public Color BackgroundColor { get; set; }

	/// <inheritdoc/>
	public Color GridLineColor { get; set; }

	/// <inheritdoc/>
	public Color BlockLineColor { get; set; }

	/// <inheritdoc/>
	public Color CrosshatchingOutlineColor { get; set; }

	/// <inheritdoc/>
	public Color CrosshatchingInnerColor { get; set; }

	/// <inheritdoc/>
	public Color UnknownIdentifierColor { get; set; }

	/// <inheritdoc/>
	public Color CrossSignColor { get; set; }

	/// <inheritdoc/>
	public Color Color1 { get; set; }

	/// <inheritdoc/>
	public Color Color2 { get; set; }

	/// <inheritdoc/>
	public Color Color3 { get; set; }

	/// <inheritdoc/>
	public Color Color4 { get; set; }

	/// <inheritdoc/>
	public Color Color5 { get; set; }

	/// <inheritdoc/>
	public Color Color6 { get; set; }

	/// <inheritdoc/>
	public Color Color7 { get; set; }

	/// <inheritdoc/>
	public Color Color8 { get; set; }

	/// <inheritdoc/>
	public Color Color9 { get; set; }

	/// <inheritdoc/>
	public Color Color10 { get; set; }

	/// <inheritdoc/>
	public Color Color11 { get; set; }

	/// <inheritdoc/>
	public Color Color12 { get; set; }

	/// <inheritdoc/>
	public Color Color13 { get; set; }

	/// <inheritdoc/>
	public Color Color14 { get; set; }

	/// <inheritdoc/>
	public Color Color15 { get; set; }


	/// <inheritdoc/>
	public IPreference Clone()
	{
		var result = new Preference();
		foreach (var property in from property in GetType().GetProperties() where property.CanWrite select property)
		{
			property.SetValue(result, property.GetValue(this));
		}

		return result;
	}

	/// <inheritdoc/>
	public void CoverBy(IPreference newPreferences)
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
}
