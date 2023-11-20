using System.Reflection;
using System.Runtime.CompilerServices;
using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Metadata;

/// <summary>
/// Represents the metadata implementation details for a <see cref="StepSearcher"/>.
/// </summary>
/// <param name="stepSearcher">The step searcher instance.</param>
/// <param name="stepSearcherAttribute">The bound step searcher attribute.</param>
/// <seealso cref="StepSearcher"/>
public sealed partial class StepSearcherMetadataInfo(
	[Data(DataMemberKinds.Field)] StepSearcher stepSearcher,
	[Data(DataMemberKinds.Field)] StepSearcherAttribute stepSearcherAttribute
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
	/// Determines whether the current step searcher is not supported for sukaku solving mode.
	/// </summary>
	public bool IsNotSupportedForSukaku => _stepSearcherAttribute.Flags is var cases && cases.Flags(ConditionalFlags.Standard);

	/// <summary>
	/// Determines whether the current step searcher is disabled
	/// by option <see cref="ConditionalFlags.TimeComplexity"/> being configured.
	/// </summary>
	/// <seealso cref="ConditionalFlags.TimeComplexity"/>
	public bool IsConfiguredSlow => _stepSearcherAttribute.Flags is var cases && cases.Flags(ConditionalFlags.TimeComplexity);

	/// <summary>
	/// Determines whether the current step searcher is disabled
	/// by option <see cref="ConditionalFlags.SpaceComplexity"/> being configured.
	/// </summary>
	/// <seealso cref="ConditionalFlags.SpaceComplexity"/>
	public bool IsConfiguredHighAllocation => _stepSearcherAttribute.Flags is var cases && cases.Flags(ConditionalFlags.SpaceComplexity);

	/// <summary>
	/// Returns the real name of this instance.
	/// </summary>
	public string Name => _stepSearcher.GetType().Name switch { var rawTypeName => GetString($"StepSearcherName_{rawTypeName}") ?? rawTypeName };

	/// <inheritdoc cref="StepSearcherAttribute.SupportedTechniques"/>
	public TechniqueSet SupportedTechniques => [.. _stepSearcherAttribute.SupportedTechniques];

	/// <summary>
	/// Indicates the <see cref="DifficultyLevel"/>s whose corresponding step can be produced by the current step searcher instance.
	/// </summary>
	public DifficultyLevel[] DifficultyLevelRange => _stepSearcherAttribute.DifficultyLevels.GetAllFlags();


	/// <summary>
	/// Fetch the the implementation details for the specified <see cref="StepSearcher"/> instance.
	/// </summary>
	/// <param name="stepSearcher">The step searcher instance.</param>
	/// <returns>The final result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static StepSearcherMetadataInfo GetFor(StepSearcher stepSearcher)
		=> new(stepSearcher, stepSearcher.GetType().GetCustomAttribute<StepSearcherAttribute>()!);
}
