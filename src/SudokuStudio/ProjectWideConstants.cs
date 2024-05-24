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
#if NET9_0_OR_GREATER
	public static readonly Lock AnalyzingRelatedSyncRoot = new();
#else
	public static readonly object AnalyzingRelatedSyncRoot = new();
#endif

	/// <summary>
	/// Indicates the current assembly, of type <see cref="Assembly"/>.
	/// </summary>
	/// <seealso cref="Assembly"/>
	public static readonly Assembly CurrentAssembly = typeof(ProjectWideConstants).Assembly;
}
