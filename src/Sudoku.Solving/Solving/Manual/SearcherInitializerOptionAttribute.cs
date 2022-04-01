namespace Sudoku.Solving.Manual;

/// <summary>
/// Defines an attribute that can be applied to the solving assembly,
/// to tell the source generator that the searcher option instance will be generated in the specified type.
/// </summary>
/// <typeparam name="TStepSearcher">The type of the step searcher.</typeparam>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
public sealed class SearcherConfigurationAttribute<TStepSearcher> : Attribute
	where TStepSearcher : class, IStepSearcher
{
	/// <summary>
	/// Initializes a <see cref="SearcherConfigurationAttribute{TStepSearcher}"/> instance
	/// via the specified priority.
	/// </summary>
	/// <param name="priority">The priority value. The value must be unique.</param>
	/// <param name="displayingLevel">The displaying level.</param>
	public SearcherConfigurationAttribute(int priority, DisplayingLevel displayingLevel)
	{
		Priority = priority;
		DisplayingLevel = displayingLevel;
	}


	/// <summary>
	/// Indicates the priority of the step searcher. The priority value must be unique,
	/// which means that different step searchers must hold different priority values.
	/// </summary>
	public int Priority { get; }

	/// <summary>
	/// Indicates the level of the step searcher.
	/// </summary>
	public DisplayingLevel DisplayingLevel { get; }

	/// <summary>
	/// Indicates the area that the step searcher can be used and available.
	/// </summary>
	public EnabledAreas EnabledAreas { get; init; } = EnabledAreas.Default | EnabledAreas.Gathering;

	/// <summary>
	/// Indicates why the step searcher is disabled, which means the property <see cref="EnabledAreas"/>
	/// is <see cref="EnabledAreas.None"/>.
	/// </summary>
	/// <seealso cref="EnabledAreas.None"/>
	public DisabledReason DisabledReason { get; init; } = DisabledReason.None;
}
