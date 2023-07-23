namespace SudokuStudio.BindableSource;

/// <summary>
/// Indicates the technique bindable source.
/// </summary>
public sealed class TechniqueBindableSource
{
	/// <summary>
	/// The technique enumeration field.
	/// </summary>
	public Technique Technique { get; set; }

	/// <summary>
	/// The display name.
	/// </summary>
	public string DisplayName { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the feature for the current technique.
	/// </summary>
	public TechniqueFeature Feature { get; set; }
}
