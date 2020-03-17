using System;
using Sudoku.Solving.Manual;

namespace Sudoku.Solving
{
	/// <summary>
	/// To mark on a technique searcher, it means that this technique searcher
	/// will have a slow speed to calculate.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class SlowAttribute : Attribute
	{
		/// <summary>
		/// <para>
		/// Indicates whether the searcher is slow but necessary.
		/// If the value is <see langword="true"/>, the search will not
		/// be optimized by <see cref="ManualSolver"/>, and skip this searcher;
		/// otherwise, this searcher will be skipped.
		/// </para>
		/// <para>
		/// The value is <see langword="false"/> in default case.
		/// </para>
		/// </summary>
		/// <seealso cref="ManualSolver"/>
		public bool SlowButNecessary { get; set; }
	}
}
