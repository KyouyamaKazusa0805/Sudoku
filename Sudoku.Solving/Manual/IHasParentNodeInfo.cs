#if DOUBLE_LAYERED_ASSUMPTION

using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Solving.Manual.Chaining;

namespace Sudoku.Solving.Manual
{
	/// <summary>
	/// Provides the indirect technique information that are able to tell what
	/// <see cref="Node"/>s have been set to off before this rule cound be applied.
	/// </summary>
	/// <remarks>
	/// This interface is used in <see cref="ChainingTechniqueInfo"/> only.
	/// </remarks>
	/// <seealso cref="Node"/>
	/// <seealso cref="ChainingTechniqueInfo"/>
	public interface IHasParentNodeInfo
	{
		/// <summary>
		/// Get the nodes that were removed from the initial grid before this rule cound be applied.
		/// </summary>
		/// <param name="initialGrid">
		/// (<see langword="in"/> parameter) The initial grid on which this rule can't be applied.
		/// </param>
		/// <param name="currentGrid">
		/// (<see langword="in"/> parameter) The current grid on which this rule is revealed.
		/// </param>
		/// <returns>The nodes that were removed from the initial grid.</returns>
		IEnumerable<Node> GetRuleParents(in SudokuGrid initialGrid, in SudokuGrid currentGrid);
	}
}

#endif