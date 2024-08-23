namespace SudokuStudio;

/// <summary>
/// Represents a list of constant or <see langword="static readonly"/> fields used in the whole project.
/// </summary>
internal static class ProjectWideConstants
{
	/// <summary>
	/// Indicates the instance that is used for synchronization for asynchronous environment,
	/// used for the operations that is analyzing a puzzle, or collecting steps.
	/// </summary>
	/// <seealso cref="Step"/>
	public static readonly Lock AnalyzingRelatedSyncRoot = new();

	/// <summary>
	/// Indicates the current assembly, of type <see cref="Assembly"/>.
	/// </summary>
	/// <seealso cref="Assembly"/>
	public static readonly Assembly CurrentAssembly = typeof(ProjectWideConstants).Assembly;
}
