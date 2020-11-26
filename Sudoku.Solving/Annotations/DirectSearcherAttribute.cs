using System;
using System.Collections.Generic;
using Sudoku.Data;

namespace Sudoku.Solving.Annotations
{
	/// <summary>
	/// <para>
	/// Indicates the technique searcher can or can't be used in some scenarios
	/// where they aren't in traversing mode to call
	/// <see cref="TechniqueSearcher.GetAll(IList{TechniqueInfo}, in SudokuGrid)"/>
	/// in <see cref="TechniqueSearcher"/>s one by one.
	/// </para>
	/// <para>
	/// If <see langword="true"/>, the searcher can't use those <see langword="static"/>
	/// properties such as <see cref="TechniqueSearcher.CandMaps"/> in its method
	/// <see cref="TechniqueSearcher.GetAll(IList{TechniqueInfo}, in SudokuGrid)"/>.
	/// </para>
	/// </summary>
	/// <remarks>
	/// <para>
	/// All disallowed properties are:
	/// <list type="bullet">
	/// <item><see cref="TechniqueSearcher.DigitMaps"/></item>
	/// <item><see cref="TechniqueSearcher.ValueMaps"/></item>
	/// <item><see cref="TechniqueSearcher.CandMaps"/></item>
	/// <item><see cref="TechniqueSearcher.BivalueMap"/></item>
	/// <item><see cref="TechniqueSearcher.EmptyMap"/></item>
	/// </list>
	/// The disallowed method is:
	/// <list type="bullet">
	/// <item><see cref="TechniqueSearcher.InitializeMaps(in SudokuGrid)"/></item>
	/// </list>
	/// </para>
	/// <para>
	/// Those properties can optimize the performance to analyze a sudoku grid, but
	/// sometimes they may cause a potential bug that is hard to find and fix. The attribute
	/// is created and used for solving the problem.
	/// </para>
	/// </remarks>
	/// <seealso cref="TechniqueSearcher"/>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class DirectSearcherAttribute : Attribute
	{
		/// <summary>
		/// Indicates the ability that can or can't be used in non-traversing scenarios.
		/// The default value is <see langword="true"/>.
		/// </summary>
		public bool IsAllow { get; init; } = true;
	}
}
