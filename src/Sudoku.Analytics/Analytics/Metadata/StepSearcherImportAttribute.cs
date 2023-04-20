namespace Sudoku.Analytics.Metadata;

/// <summary>
/// Defines an attribute that can be applied to the solving assembly,
/// to tell the source generator that the searcher option instance will be generated in the specified type.
/// </summary>
/// <typeparam name="T">The type of the step searcher.</typeparam>
/// <param name="level">Indicates the step searcher level. For more information please visit type <see cref="StepSearcherLevel"/>.</param>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
public sealed partial class StepSearcherImportAttribute<T>([PrimaryConstructorParameter] StepSearcherLevel level) :
	StepSearcherMetadataAttribute
	where T : StepSearcher
{
	/// <summary>
	/// Indicates the area that the step searcher can be used and available.
	/// </summary>
	/// <remarks>
	/// The default value of this property is both <see cref="StepSearcherRunningArea.Searching"/>
	/// and <see cref="StepSearcherRunningArea.Gathering"/>.
	/// </remarks>
	public StepSearcherRunningArea Areas { get; init; } = StepSearcherRunningArea.Searching | StepSearcherRunningArea.Gathering;
}
