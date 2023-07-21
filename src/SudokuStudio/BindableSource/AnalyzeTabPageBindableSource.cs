namespace SudokuStudio.BindableSource;

/// <summary>
/// Represents a type that binds with a analyze tab page type that implements <see cref="IAnalyzeTabPage"/>.
/// </summary>
/// <seealso cref="IAnalyzeTabPage"/>
internal sealed class AnalyzeTabPageBindableSource
{
	/// <summary>
	/// Indicates the header.
	/// </summary>
	public required string Header { get; init; }

	/// <summary>
	/// Indicates the icon source.
	/// </summary>
	public required IconSource IconSource { get; init; }

	/// <summary>
	/// Indicates the target page instance.
	/// </summary>
	public required IAnalyzeTabPage Page { get; init; }
}
