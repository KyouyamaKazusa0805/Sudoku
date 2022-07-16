namespace Sudoku.Drawing;

/// <summary>
/// Indicates a preference instance.
/// </summary>
public sealed class Perference : IPreference
{
	/// <summary>
	/// Initializes a <see cref="Perference"/> instance.
	/// </summary>
	private Perference()
	{
	}


	/// <inheritdoc/>
	public bool ShowCandidates { get; set; } = true;

	/// <inheritdoc/>
	public bool ShowLightHouse { get; set; } = true;

	/// <inheritdoc/>
	public decimal ValueScale { get; set; } = .9M;

	/// <inheritdoc/>
	public decimal CandidateScale { get; set; } = .3M;

	/// <inheritdoc/>
	public float GridLineWidth { get; set; } = 1.5F;

	/// <inheritdoc/>
	public float BlockLineWidth { get; set; } = 3F;

	/// <inheritdoc/>
	public string? GivenFontName { get; set; } = "MiSans";

	/// <inheritdoc/>
	public string? ModifiableFontName { get; set; } = "MiSans";

	/// <inheritdoc/>
	public string? CandidateFontName { get; set; } = "MiSans";

	/// <inheritdoc/>
	public FontStyle GivenFontStyle { get; set; } = FontStyle.Regular;

	/// <inheritdoc/>
	public FontStyle ModifiableFontStyle { get; set; } = FontStyle.Regular;

	/// <inheritdoc/>
	public FontStyle CandidateFontStyle { get; set; } = FontStyle.Regular;

	/// <inheritdoc/>
	public FontStyle UnknownFontStyle { get; set; } = FontStyle.Regular;

	/// <inheritdoc/>
	public Color GivenColor { get; set; } = Color.Black;

	/// <inheritdoc/>
	public Color ModifiableColor { get; set; } = Color.Blue;

	/// <inheritdoc/>
	public Color CandidateColor { get; set; } = Color.DimGray;

	/// <inheritdoc/>
	public Color FocusedCellColor { get; set; } = Color.FromArgb(32, Color.Yellow);

	/// <inheritdoc/>
	public Color EliminationColor { get; set; } = Color.FromArgb(255, 118, 132);

	/// <inheritdoc/>
	public Color CannibalismColor { get; set; } = Color.FromArgb(235, 0, 0);

	/// <inheritdoc/>
	public Color ChainColor { get; set; } = Color.Red;

	/// <inheritdoc/>
	public Color BackgroundColor { get; set; } = Color.White;

	/// <inheritdoc/>
	public Color GridLineColor { get; set; } = Color.Black;

	/// <inheritdoc/>
	public Color BlockLineColor { get; set; } = Color.Black;

	/// <inheritdoc/>
	public Color CrosshatchingOutlineColor { get; set; } = Color.FromArgb(192, Color.Black);

	/// <inheritdoc/>
	public Color CrosshatchingInnerColor { get; set; } = Color.Transparent;

	/// <inheritdoc/>
	public Color UnknownIdentifierColor { get; set; } = Color.FromArgb(192, Color.Red);

	/// <inheritdoc/>
	public Color CrossSignColor { get; set; } = Color.FromArgb(192, Color.Black);

	/// <inheritdoc/>
	public Color Color1 { get; set; } = Color.FromArgb(63, 218, 101);

	/// <inheritdoc/>
	public Color Color2 { get; set; } = Color.FromArgb(255, 192, 89);

	/// <inheritdoc/>
	public Color Color3 { get; set; } = Color.FromArgb(127, 187, 255);

	/// <inheritdoc/>
	public Color Color4 { get; set; } = Color.FromArgb(216, 178, 255);

	/// <inheritdoc/>
	public Color Color5 { get; set; } = Color.FromArgb(197, 232, 140);

	/// <inheritdoc/>
	public Color Color6 { get; set; } = Color.FromArgb(255, 203, 203);

	/// <inheritdoc/>
	public Color Color7 { get; set; } = Color.FromArgb(178, 223, 223);

	/// <inheritdoc/>
	public Color Color8 { get; set; } = Color.FromArgb(252, 220, 165);

	/// <inheritdoc/>
	public Color Color9 { get; set; } = Color.FromArgb(255, 255, 150);

	/// <inheritdoc/>
	public Color Color10 { get; set; } = Color.FromArgb(247, 222, 143);

	/// <inheritdoc/>
	public Color Color11 { get; set; } = Color.FromArgb(220, 212, 252);

	/// <inheritdoc/>
	public Color Color12 { get; set; } = Color.FromArgb(255, 118, 132);

	/// <inheritdoc/>
	public Color Color13 { get; set; } = Color.FromArgb(206, 251, 237);

	/// <inheritdoc/>
	public Color Color14 { get; set; } = Color.FromArgb(215, 255, 215);

	/// <inheritdoc/>
	public Color Color15 { get; set; } = Color.FromArgb(192, 192, 192);


	/// <summary>
	/// Indicates the default instance.
	/// </summary>
	public static Perference Default => new();


	/// <inheritdoc/>
	public object Clone()
	{
		var instance = Default;
		foreach (var propertyInfo in typeof(Perference).GetProperties(BindingFlags.Instance | BindingFlags.Public))
		{
			if (propertyInfo is { CanRead: true, CanWrite: true })
			{
				object? originalValue = propertyInfo.GetValue(this);
				propertyInfo.SetValue(instance, originalValue);
			}
		}

		return instance;
	}

	/// <inheritdoc/>
	public void CoverBy(IPreference newPreferences)
	{
		foreach (var propertyInfo in typeof(Perference).GetProperties(BindingFlags.Instance | BindingFlags.Public))
		{
			if (propertyInfo is { CanRead: true, CanWrite: true })
			{
				object? originalValue = propertyInfo.GetValue(newPreferences);
				propertyInfo.SetValue(this, originalValue);
			}
		}
	}
}
