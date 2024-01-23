namespace Sudoku.Analytics.Metadata;

/// <summary>
/// Indicates a type marked this attribute is a runnable <see cref="StepSearcher"/>.
/// </summary>
/// <param name="supportedTechniques">All supported techniques.</param>
/// <seealso cref="StepSearcher"/>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class StepSearcherAttribute(params Technique[] supportedTechniques) : Attribute
{
	/// <summary>
	/// Indicates the searching logic only uses cached fields in type <see cref="CachedFields"/>,
	/// which means it can conclude result without using field <see cref="AnalysisContext.Grid"/>.
	/// </summary>
	/// <seealso cref="CachedFields"/>
	/// <seealso cref="AnalysisContext"/>
	public bool OnlyUsesCachedFields { get; init; }

	/// <summary>
	/// Indicates the technique searcher can or can't be used in some scenarios
	/// where they aren't in traversing mode to call core collecting method <c>Collect</c> in <see cref="StepSearcher"/>
	/// for <see cref="StepSearcher"/> instances one by one.
	/// </summary>
	/// <!--
	/// <remarks>
	/// <para>
	/// All disallowed fields are:
	/// <list type="bullet">
	/// <item><see cref="DigitsMap"/></item>
	/// <item><see cref="ValuesMap"/></item>
	/// <item><see cref="CandidatesMap"/></item>
	/// <item><see cref="BivalueCells"/></item>
	/// <item><see cref="EmptyCells"/></item>
	/// </list>
	/// The disallowed method is:
	/// <list type="bullet">
	/// <item><see cref="Initialize(in Grid, in Grid)"/></item>
	/// </list>
	/// </para>
	/// <para>
	/// Those fields or methods can optimize the performance to analyze a sudoku grid, but
	/// sometimes they may cause a potential bug that is hard to find and fix. The attribute
	/// is created and used for solving the problem.
	/// </para>
	/// </remarks>
	/// -->
	/// <seealso cref="CachedFields"/>
	public bool IsPure { get; init; }

	/// <summary>
	/// Indicates whether the option is fixed that can't be modified in UI.
	/// </summary>
	public bool IsFixed { get; init; }

	/// <summary>
	/// Indicates what difficulty levels the current step searcher can produce.
	/// </summary>
	public DifficultyLevel DifficultyLevels
		=> (from technique in SupportedTechniques select technique.GetDifficultyLevel()).Aggregate(CommonMethods.EnumFlagMerger);

	/// <summary>
	/// <inheritdoc cref="StepSearcherAttribute" path="/param[@name='supportedTechniques']"/>
	/// </summary>
	public TechniqueSet SupportedTechniques { get; } = [.. supportedTechniques];
}
