namespace Sudoku.UI.Data.Metadata;

/// <summary>
/// Provides with a set of preference group names.
/// </summary>
public static class PreferenceGroupNames
{
	/// <summary>
	/// Indicates the basic options.
	/// </summary>
	[PreferenceGroupOrder(0)]
	public const string Basic = nameof(Basic);

	/// <summary>
	/// Indicates the solving options.
	/// </summary>
	[PreferenceGroupOrder(1)]
	public const string Solving = nameof(Solving);

	/// <summary>
	/// Indicates the rendering options.
	/// </summary>
	[PreferenceGroupOrder(2)]
	public const string Rendering = nameof(Rendering);

	/// <summary>
	/// Indicates the miscellaneous options.
	/// </summary>
	[PreferenceGroupOrder(3)]
	public const string Miscellaneous = nameof(Miscellaneous);
}
