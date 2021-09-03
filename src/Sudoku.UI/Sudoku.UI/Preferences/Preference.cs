namespace Sudoku.UI.Preferences;

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
	public float GridLineWidth { get; set; }

	/// <inheritdoc/>
	public float BlockLineWidth { get; set; }

	/// <inheritdoc/>
	public string? GivenFontName { get; set; }

	/// <inheritdoc/>
	public string? ModifiableFontName { get; set; }

	/// <inheritdoc/>
	public string? CandidateFontName { get; set; }

	/// <inheritdoc/>
	public gdip::FontStyle GivenFontStyle { get; set; }

	/// <inheritdoc/>
	public gdip::FontStyle ModifiableFontStyle { get; set; }

	/// <inheritdoc/>
	public gdip::FontStyle CandidateFontStyle { get; set; }

	/// <inheritdoc/>
	public gdip::FontStyle UnknownIdentfierFontStyle { get; set; }

	/// <inheritdoc/>
	public gdip::Color GivenColor { get; set; }

	/// <inheritdoc/>
	public gdip::Color ModifiableColor { get; set; }

	/// <inheritdoc/>
	public gdip::Color CandidateColor { get; set; }

	/// <inheritdoc/>
	public gdip::Color FocusedCellColor { get; set; }

	/// <inheritdoc/>
	public gdip::Color EliminationColor { get; set; }

	/// <inheritdoc/>
	public gdip::Color CannibalismColor { get; set; }

	/// <inheritdoc/>
	public gdip::Color ChainColor { get; set; }

	/// <inheritdoc/>
	public gdip::Color BackgroundColor { get; set; }

	/// <inheritdoc/>
	public gdip::Color GridLineColor { get; set; }

	/// <inheritdoc/>
	public gdip::Color BlockLineColor { get; set; }

	/// <inheritdoc/>
	public gdip::Color CrosshatchingOutlineColor { get; set; }

	/// <inheritdoc/>
	public gdip::Color CrosshatchingInnerColor { get; set; }

	/// <inheritdoc/>
	public gdip::Color UnknownIdentifierColor { get; set; }

	/// <inheritdoc/>
	public gdip::Color CrossSignColor { get; set; }

	/// <inheritdoc/>
	public gdip::Color Color1 { get; set; }

	/// <inheritdoc/>
	public gdip::Color Color2 { get; set; }

	/// <inheritdoc/>
	public gdip::Color Color3 { get; set; }

	/// <inheritdoc/>
	public gdip::Color Color4 { get; set; }

	/// <inheritdoc/>
	public gdip::Color Color5 { get; set; }

	/// <inheritdoc/>
	public gdip::Color Color6 { get; set; }

	/// <inheritdoc/>
	public gdip::Color Color7 { get; set; }

	/// <inheritdoc/>
	public gdip::Color Color8 { get; set; }

	/// <inheritdoc/>
	public gdip::Color Color9 { get; set; }

	/// <inheritdoc/>
	public gdip::Color Color10 { get; set; }

	/// <inheritdoc/>
	public gdip::Color Color11 { get; set; }

	/// <inheritdoc/>
	public gdip::Color Color12 { get; set; }

	/// <inheritdoc/>
	public gdip::Color Color13 { get; set; }

	/// <inheritdoc/>
	public gdip::Color Color14 { get; set; }

	/// <inheritdoc/>
	public gdip::Color Color15 { get; set; }


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
