namespace SudokuStudio.BindableSource;

/// <summary>
/// Indicates the techique bindable source.
/// </summary>
public sealed class TechniqueBindableSource : IBindableSource
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
