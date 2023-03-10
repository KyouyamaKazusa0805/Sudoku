﻿namespace Sudoku.Solving.Logical.Annotations;

/// <summary>
/// Defines an attribute that applies to a step searcher, to define more options on UI configurations.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class StepSearcherMetadataAttribute : Attribute
{
	/// <summary>
	/// <para>Indicates whether the option is fixed that can't be modified in UI.</para>
	/// <para><i>The default value is <see langword="false"/>.</i></para>
	/// </summary>
	public bool IsOptionsFixed { get; init; }

	/// <summary>
	/// <para>
	/// Indicates the technique searcher can or can't be used in some scenarios
	/// where they aren't in traversing mode to call <see cref="IStepSearcher.GetAll(ref LogicalAnalysisContext)"/>
	/// in <see cref="IStepSearcher"/>s one by one.
	/// </para>
	/// <para>
	/// If <see langword="true"/>, the searcher can't use those <see langword="static"/>
	/// properties such as <see cref="CandidatesMap"/> in its method
	/// <see cref="IStepSearcher.GetAll(ref LogicalAnalysisContext)"/>.
	/// </para>
	/// <para><i>The default value is <see langword="false"/>.</i></para>
	/// </summary>
	/// <remarks>
	/// <para>
	/// All disallowed properties are:
	/// <list type="bullet">
	/// <item><see cref="DigitsMap"/></item>
	/// <item><see cref="ValuesMap"/></item>
	/// <item><see cref="CandidatesMap"/></item>
	/// <item><see cref="BivalueCells"/></item>
	/// <item><see cref="EmptyCells"/></item>
	/// </list>
	/// The disallowed method is:
	/// <list type="bullet">
	/// <item><see cref="Initialize"/></item>
	/// </list>
	/// </para>
	/// <para>
	/// Those properties or methods can optimize the performance to analyze a sudoku grid, but
	/// sometimes they may cause a potential bug that is hard to find and fix. The attribute
	/// is created and used for solving the problem.
	/// </para>
	/// </remarks>
	/// <seealso cref="IStepSearcher"/>
	/// <seealso cref="CachedCellMaps"/>
	public bool IsDirect { get; init; }

	/// <summary>
	/// <para>Indicates whether the specified step searcher doesn't rely on the puzzle.</para>
	/// <para><i>The default value is <see langword="false"/>.</i></para>
	/// </summary>
	public bool PuzzleNotRelying { get; init; }
}
