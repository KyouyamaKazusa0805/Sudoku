namespace SudokuStudio;

/// <summary>
/// Represents a list of constant or <see langword="static readonly"/> fields used in the whole project.
/// </summary>
internal static class ProjectWideConstants
{
	/// <summary>
	/// Indicates the instance that is used for synchronization for asynchronous environment,
	/// used for the operations that is analyzing a puzzle, or gathering steps.
	/// </summary>
	/// <seealso cref="Step"/>
	public static readonly object AnalyzingRelatedSyncRoot = new();

	/// <summary>
	/// Indicates the file header.
	/// </summary>
	public static readonly string LibraryConfigFileHeader = "$ Header of the File $";

	/// <summary>
	/// Indicates the current assembly, of type <see cref="Assembly"/>.
	/// </summary>
	/// <seealso cref="Assembly"/>
	public static readonly Assembly CurrentAssembly = typeof(ProjectWideConstants).Assembly;

	/// <summary>
	/// Indicates the default <see cref="NavigationTransitionInfo"/> instance that describes the details of data while switching pages.
	/// </summary>
	public static readonly NavigationTransitionInfo DefaultNavigationTransitionInfo =
		new SlideNavigationTransitionInfo { Effect = SlideNavigationTransitionEffect.FromRight };
}
