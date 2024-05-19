namespace Sudoku.Analytics.Metadata;

/// <summary>
/// Indicates a type marked this attribute is a runnable <see cref="StepSearcher"/>.
/// </summary>
/// <param name="nameKey">Indicates the key in resource dictionary.</param>
/// <param name="supportedTechniques">All supported techniques.</param>
/// <seealso cref="StepSearcher"/>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed partial class StepSearcherAttribute(
	[PrimaryConstructorParameter] string nameKey,
	params Technique[] supportedTechniques
) : Attribute
{
	/// <summary>
	/// <para>
	/// Indicates the technique searcher doesn't use any cached fields in implementation, i.e. caching-free or caching-safe.
	/// </para>
	/// <para>
	/// This property can be used by checking free-caching rules, in order to inline-declaring data members,
	/// storing inside another step searcher type, or creating a temporary logic to call this searcher.
	/// </para>
	/// </summary>
	public bool IsCachingSafe { get; init; }

	/// <summary>
	/// <para>
	/// Indicates the technique searcher always uses cached fields in implementation, i.e. caching-unsafe.
	/// </para>
	/// <para>
	/// As a property reversed from <see cref="IsCachingSafe"/>, this property tags for a step searcher not using
	/// <see cref="AnalysisContext.Grid"/>. If a step searcher type always use caching fields without any doubt,
	/// this property should be set with <see langword="true"/>.
	/// </para>
	/// <para><i>
	/// This property won't be used in API, but it may be used in future versions.
	/// </i></para>
	/// </summary>
	/// <seealso cref="IsCachingSafe"/>
	/// <seealso cref="AnalysisContext.Grid"/>
	public bool IsCachingUnsafe { get; init; }

	/// <summary>
	/// Indicates whether the option is read-only that cannot be modified in UI,
	/// meaning a user cannot modify the ordering of this step searcher.
	/// </summary>
	public bool IsOrderingFixed { get; init; }

	/// <summary>
	/// Indicates whether the option is read-only that cannot be modified in UI,
	/// meaning a user cannot disable the current step searcher.
	/// </summary>
	public bool IsAvailabilityReadOnly { get; init; }

	/// <summary>
	/// Indicates whether the step searcher can be invoked by puzzles containing multiple solutions.
	/// By default the value is <see langword="true"/>.
	/// </summary>
	public bool SupportAnalyzingMultipleSolutionsPuzzle { get; init; } = true;

	/// <summary>
	/// Indicates the runtime options that controls extra behaviors.
	/// </summary>
	public StepSearcherRuntimeFlags RuntimeFlags { get; init; }

	/// <summary>
	/// Indicates what difficulty levels the current step searcher can produce.
	/// </summary>
	public DifficultyLevel DifficultyLevels => (from t in SupportedTechniques select t.GetDifficultyLevel()).Aggregate(@delegate.EnumFlagMerger);

	/// <summary>
	/// Indicates the supported sudoku types.
	/// </summary>
	public SudokuType SupportedSudokuTypes { get; init; } = SudokuType.Standard | SudokuType.Sukaku | SudokuType.JustOneCell;

	/// <summary>
	/// <inheritdoc cref="StepSearcherAttribute" path="/param[@name='supportedTechniques']"/>
	/// </summary>
	public TechniqueSet SupportedTechniques { get; } = [.. supportedTechniques];
}
