namespace Sudoku.Analytics.Metadata;

/// <summary>
/// Represents the metadata implementation details for a <see cref="StepSearcher"/>.
/// </summary>
/// <param name="stepSearcher">The step searcher instance.</param>
/// <param name="stepSearcherAttribute">The bound step searcher attribute.</param>
/// <param name="stepSearcherFlagsAttribute">The bound step searcher flags attribute.</param>
/// <seealso cref="StepSearcher"/>
public sealed partial class StepSearcherMetadataInfo(
	[PrimaryConstructorParameter(MemberKinds.Field)] StepSearcher stepSearcher,
	[PrimaryConstructorParameter(MemberKinds.Field)] StepSearcherAttribute stepSearcherAttribute,
	[PrimaryConstructorParameter(MemberKinds.Field)] StepSearcherFlagsAttribute? stepSearcherFlagsAttribute
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
	public bool IsPure => _stepSearcherAttribute.IsPure;

	/// <summary>
	/// Determines whether we can adjust the ordering of the current step searcher as a customized configuration option before solving a puzzle.
	/// </summary>
	public bool IsFixed => _stepSearcherAttribute.IsFixed;

	/// <summary>
	/// Determines whether we can toggle availability of the step searcher.
	/// </summary>
	public bool IsReadOnly => _stepSearcherAttribute.IsReadOnly;

	/// <summary>
	/// Determines whether the current step searcher is not supported for sukaku solving mode.
	/// </summary>
	public bool IsNotSupportedForSukaku => _stepSearcherFlagsAttribute?.Flags is { } cases && cases.HasFlag(StepSearcherFlags.Standard);

	/// <summary>
	/// Determines whether the current step searcher is disabled
	/// by option <see cref="StepSearcherFlags.TimeComplexity"/> being configured.
	/// </summary>
	/// <seealso cref="StepSearcherFlags.TimeComplexity"/>
	public bool IsConfiguredSlow => _stepSearcherFlagsAttribute?.Flags is { } cases && cases.HasFlag(StepSearcherFlags.TimeComplexity);

	/// <summary>
	/// Determines whether the current step searcher is disabled
	/// by option <see cref="StepSearcherFlags.SpaceComplexity"/> being configured.
	/// </summary>
	/// <seealso cref="StepSearcherFlags.SpaceComplexity"/>
	public bool IsConfiguredHighAllocation => _stepSearcherFlagsAttribute?.Flags is { } cases && cases.HasFlag(StepSearcherFlags.SpaceComplexity);

	/// <summary>
	/// Determines whether the current step searcher is only run for direct view.
	/// </summary>
	public bool IsOnlyRunForDirectViews => _stepSearcherFlagsAttribute?.Flags is { } cases && cases.HasFlag(StepSearcherFlags.DirectTechniquesOnly);

	/// <summary>
	/// Determines whether the current step searcher is only run for indirect view.
	/// </summary>
	public bool IsOnlyRunForIndirectViews => _stepSearcherFlagsAttribute?.Flags is { } cases && cases.HasFlag(StepSearcherFlags.IndirectTechniquesOnly);

	/// <inheritdoc cref="StepSearcherAttribute.SupportedTechniques"/>
	public TechniqueSet SupportedTechniques => _stepSearcherAttribute.SupportedTechniques;

	/// <summary>
	/// Indicates the <see cref="DifficultyLevel"/>s whose corresponding step can be produced by the current step searcher instance.
	/// </summary>
	public DifficultyLevel[] DifficultyLevelRange => _stepSearcherAttribute.DifficultyLevels.GetAllFlags();


	/// <summary>
	/// Gets the name of the step searcher, using the specified culture.
	/// </summary>
	/// <param name="culture">The culture information.</param>
	/// <returns>The name.</returns>
	public string GetName(CultureInfo? culture)
		=> _stepSearcher.GetType() switch
		{
			{ Name: var typeName } type => type.GetCustomAttribute<StepSearcherRuntimeNameAttribute>() switch
			{
				{ } p when p.GetFactName(culture) is { } factName => factName,
				_ => ResourceDictionary.TryGet($"StepSearcherName_{typeName}", out var resource, culture ?? CultureInfo.CurrentUICulture)
					? resource
					: typeName
			}
		};

	/// <summary>
	/// Fetch the the implementation details for the specified <see cref="StepSearcher"/> instance.
	/// </summary>
	/// <param name="stepSearcher">The step searcher instance.</param>
	/// <returns>The final result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static StepSearcherMetadataInfo GetFor(StepSearcher stepSearcher)
		=> new(
			stepSearcher,
			stepSearcher.GetType().GetCustomAttribute<StepSearcherAttribute>()!,
			stepSearcher.GetType().GetCustomAttribute<StepSearcherFlagsAttribute>()
		);
}
