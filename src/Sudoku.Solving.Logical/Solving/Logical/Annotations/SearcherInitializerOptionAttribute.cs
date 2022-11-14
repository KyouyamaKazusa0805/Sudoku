namespace Sudoku.Solving.Logical.Annotations;

/// <summary>
/// Defines an attribute that can be applied to the solving assembly,
/// to tell the source generator that the searcher option instance will be generated in the specified type.
/// </summary>
/// <typeparam name="T">The type of the step searcher.</typeparam>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
public sealed class SearcherConfigurationAttribute<T> : Attribute where T : class, IStepSearcher
{
	/// <summary>
	/// Initializes a <see cref="SearcherConfigurationAttribute{T}"/> instance
	/// via the specified displaying level.
	/// </summary>
	/// <param name="displayingLevel">The displaying level.</param>
	public SearcherConfigurationAttribute(SearcherDisplayingLevel displayingLevel) => DisplayingLevel = displayingLevel;


	/// <summary>
	/// Indicates the priority of the step searcher. The priority value must be unique,
	/// which means that different step searchers must hold different priority values.
	/// </summary>
	public int Priority { get; }

	/// <summary>
	/// Indicates the level of the step searcher.
	/// </summary>
	public SearcherDisplayingLevel DisplayingLevel { get; }

	/// <summary>
	/// Indicates the area that the step searcher can be used and available.
	/// </summary>
	public SearcherEnabledArea EnabledArea { get; init; } = SearcherEnabledArea.Default | SearcherEnabledArea.Gathering;

	/// <summary>
	/// Indicates why the step searcher is disabled, which means the property <see cref="EnabledArea"/>
	/// is <see cref="SearcherEnabledArea.None"/>.
	/// </summary>
	/// <seealso cref="SearcherEnabledArea.None"/>
	public SearcherDisabledReason DisabledReason { get; init; } = SearcherDisabledReason.None;
}
