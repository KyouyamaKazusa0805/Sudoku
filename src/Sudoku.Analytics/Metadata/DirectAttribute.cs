namespace Sudoku.Analytics.Metadata;

/// <summary>
/// <para>
/// Indicates the technique searcher can or can't be used in some scenarios
/// where they aren't in traversing mode to call <see cref="StepSearcher.GetAll(ref AnalysisContext)"/>
/// in <see cref="StepSearcher"/>s one by one.
/// </para>
/// <para>
/// If <see langword="true"/>, the searcher can't use those <see langword="static"/>
/// fields such as <see cref="CandidatesMap"/> in its method
/// <see cref="StepSearcher.GetAll(ref AnalysisContext)"/>.
/// </para>
/// </summary>
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
/// <seealso cref="StepSearcher"/>
/// <seealso cref="CachedFields"/>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class DirectAttribute : StepSearcherMetadataAttribute;
