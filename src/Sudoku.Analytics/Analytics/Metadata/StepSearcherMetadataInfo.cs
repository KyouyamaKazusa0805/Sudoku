namespace Sudoku.Analytics.Metadata;

/// <summary>
/// Represents the metadata implementation details for a <see cref="StepSearcher"/>.
/// </summary>
/// <param name="stepSearcher">The step searcher instance.</param>
/// <param name="backAttribute">The bound step searcher attribute.</param>
/// <seealso cref="StepSearcher"/>
public sealed partial class StepSearcherMetadataInfo(
	[PrimaryConstructorParameter(MemberKinds.Field)] StepSearcher stepSearcher,
	[PrimaryConstructorParameter(MemberKinds.Field)] StepSearcherAttribute backAttribute
)
{
	/// <summary>
	/// Determines whether the current step searcher contains split configuration,
	/// meaning it can be created as multiple instances in a same step searchers collection.
	/// </summary>
	public bool IsSplit => _stepSearcher.GetType().GetCustomAttribute<SplitStepSearcherAttribute>() is not null;

	/// <summary>
	/// Determines whether the current step searcher is a pure one, which means it doesn't use cached fields.
	/// </summary>
	public bool IsPure => _backAttribute.IsPure;

	/// <summary>
	/// Determines whether we can adjust the ordering of the current step searcher as a customized configuration option before solving a puzzle.
	/// </summary>
	public bool IsFixed => _backAttribute.IsFixed;

	/// <summary>
	/// Determines whether we can toggle availability of the step searcher.
	/// </summary>
	public bool IsReadOnly => _backAttribute.IsReadOnly;

	/// <summary>
	/// Determines whether the current step searcher supports sukaku solving.
	/// </summary>
	public bool SupportsSukaku => _backAttribute.SupportedSudokuTypes.HasFlag(SudokuType.Sukaku);

	/// <summary>
	/// Determines whether the current step searcher is disabled
	/// by option <see cref="StepSearcherRuntimeFlags.TimeComplexity"/> being configured.
	/// </summary>
	/// <seealso cref="StepSearcherRuntimeFlags.TimeComplexity"/>
	public bool IsConfiguredSlow => _backAttribute.RuntimeFlags is { } cases && cases.HasFlag(StepSearcherRuntimeFlags.TimeComplexity);

	/// <summary>
	/// Determines whether the current step searcher is disabled
	/// by option <see cref="StepSearcherRuntimeFlags.SpaceComplexity"/> being configured.
	/// </summary>
	/// <seealso cref="StepSearcherRuntimeFlags.SpaceComplexity"/>
	public bool IsConfiguredHighAllocation
		=> _backAttribute.RuntimeFlags is { } cases && cases.HasFlag(StepSearcherRuntimeFlags.SpaceComplexity);

	/// <summary>
	/// Determines whether the current step searcher is only run for direct view.
	/// </summary>
	public bool IsOnlyRunForDirectViews
		=> _backAttribute.RuntimeFlags is { } cases && cases.HasFlag(StepSearcherRuntimeFlags.DirectTechniquesOnly);

	/// <summary>
	/// Determines whether the current step searcher is only run for indirect view.
	/// </summary>
	public bool IsOnlyRunForIndirectViews
		=> _backAttribute.RuntimeFlags is { } cases && cases.HasFlag(StepSearcherRuntimeFlags.IndirectTechniquesOnly);

	/// <summary>
	/// Indicates the <see cref="DifficultyLevel"/>s whose corresponding step can be produced by the current step searcher instance.
	/// </summary>
	public DifficultyLevel[] DifficultyLevelRange => _backAttribute.DifficultyLevels.GetAllFlags();

	/// <inheritdoc cref="StepSearcherAttribute.SupportedTechniques"/>
	public TechniqueSet SupportedTechniques => _backAttribute.SupportedTechniques;


	/// <summary>
	/// Gets the name of the step searcher, using the specified culture.
	/// </summary>
	/// <param name="culture">The culture information.</param>
	/// <returns>The name.</returns>
	public string GetName(CultureInfo? culture)
		=> _stepSearcher.GetType() switch
		{
			{ Name: var typeName } type => type.GetCustomAttribute<StepSearcherAttribute>() switch
			{
				{ NameKey: { } r } => ResourceDictionary.Get(r, culture),
				_ => ResourceDictionary.TryGet($"StepSearcherName_{typeName}", out var resource, culture ?? CultureInfo.CurrentUICulture)
					? resource
					: typeName
			}
		};
}
