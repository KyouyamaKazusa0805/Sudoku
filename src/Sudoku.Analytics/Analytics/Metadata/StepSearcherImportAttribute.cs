namespace Sudoku.Analytics.Metadata;

/// <summary>
/// Defines an attribute that can be applied to the solving assembly,
/// to tell the source generator that the searcher option instance will be generated in the specified type.
/// </summary>
/// <typeparam name="T">The type of the step searcher.</typeparam>
/// <param name="level">
/// Indicates the step searcher level. The higher the value be, the higher difficulty of the step the step searcher will search for.
/// The value can be 0, 1, 2, 3 and 4.
/// </param>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
public sealed partial class StepSearcherImportAttribute<T>([Data] int level) : Attribute where T : StepSearcher
{
	/// <summary>
	/// Indicates the area that the step searcher can be used and available.
	/// </summary>
	/// <remarks>
	/// The default value of this property is both <see cref="StepSearcherRunningArea.Searching"/>
	/// and <see cref="StepSearcherRunningArea.Collecting"/>.
	/// </remarks>
	public StepSearcherRunningArea Areas { get; init; } = StepSearcherRunningArea.Searching | StepSearcherRunningArea.Collecting;
}
