namespace SudokuStudio.Configuration;

/// <summary>
/// Represents a kind of backdrop.
/// </summary>
public enum BackdropKind
{
	/// <summary>
	/// Indicates the backdrop is none.
	/// </summary>
	Default,

	/// <summary>
	/// Indicates the Mica backdrop.
	/// </summary>
	Mica,

	/// <summary>
	/// Indicates the Mica backdrop, with alternating background support.
	/// </summary>
	MicaDeep,

	/// <summary>
	/// Indicates the acrylic backdrop.
	/// </summary>
	Acrylic,

	/// <summary>
	/// Indicates the arcylic thin backdrop.
	/// </summary>
	AcrylicThin,

	/// <summary>
	/// Indicates the transparent backdrop.
	/// </summary>
	Transparent
}
