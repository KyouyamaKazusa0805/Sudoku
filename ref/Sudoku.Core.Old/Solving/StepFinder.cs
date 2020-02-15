using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Sudoku.Data.Meta;

namespace Sudoku.Solving
{
	/// <summary>
	/// Represents a technique solver.
	/// </summary>
	/// <remarks>
	/// Technique contains:
	/// <list type="bullet">
	/// <item><term>Hidden Single</term><description>1.0, 1.2 or 1.5</description></item>
	/// <item><term>Naked Single</term><description>2.3</description></item>
	/// <item><term>Pointing</term><description>2.6</description></item>
	/// <item><term>Claiming</term><description>2.8</description></item>
	/// <item><term>Locked Subsets</term><description>2.0 or 2.5</description></item>
	/// <item><term>Naked Subsets</term><description>3.0, 3.6 or 5.0</description></item>
	/// <item><term>Naked Subsets Extension</term><description>3.1, 3.7 or 5.2</description></item>
	/// <item><term>Hidden Subsets</term><description>3.4, 4.0 or 5.4</description></item>
	/// <item><term>X-Wing</term><description>3.2</description></item>
	/// <item><term>Swordfish</term><description>3.8</description></item>
	/// <item><term>Jellyfish</term><description>5.2</description></item>
	/// <item><term>Finned X-Wing</term><description>3.4</description></item>
	/// <item><term>Finned Swordfish</term><description>4.0</description></item>
	/// <item><term>Finned Jellyfish</term><description>5.4</description></item>
	/// <item><term>Sashimi finned X-Wing</term><description>3.5</description></item>
	/// <item><term>Sashimi finned Swordfish</term><description>4.1</description></item>
	/// <item><term>Sashimi finned Jellyfish</term><description>5.6</description></item>
	/// </list>
	/// </remarks>
	public abstract class StepFinder
	{
		/// <summary>
		/// Find and record all steps at current grid.
		/// </summary>
		/// <param name="grid">A specified grid.</param>
		/// <param name="accumulator">
		/// <para>
		/// The step accumulator to record all steps had been found so far.
		/// </para>
		/// <para>
		/// The parameter is passed by reference, so you may decide to modify
		/// the collection is assigned to a new one or not
		/// (i.e. The reference can be modified any time you want).
		/// </para>
		/// </param>
		public void RecordSteps(Grid grid, ref List<TechniqueInfo> accumulator) =>
			accumulator.AddRange(TakeAll(grid));

		/// <summary>
		/// Get a single step from the top of the step list.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="callingLinqOne">
		/// To specify calling the method which uses LINQ or not.
		/// </param>
		/// <returns>
		/// A single step. If the step doesn't exist,
		/// the return value will be <see langword="null"/>.
		/// </returns>
		public TechniqueInfo? TakeOne(Grid grid)
		{
			var steps = TakeAll(grid);
			return steps.Any() ? steps.First() : null;
		}

		/// <summary>
		/// Get the specified count of the steps from the solving step list.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="count">The count of steps you want to take.</param>
		/// <returns>These steps.</returns>
		public IEnumerable<TechniqueInfo> Take(Grid grid, int count)
		{
			Contract.Requires(count >= 1);

			// `Take` method will never throw exceptions when
			// count is greater than the step count of the list.
			return TakeAll(grid).Take(count);
		}

		/// <summary>
		/// Get all technique steps at current grid.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <returns>
		/// All steps found, you can use the method
		/// <see cref="Enumerable.Take{TSource}(IEnumerable{TSource}, int)"/>
		/// to take some of them.
		/// </returns>
		public abstract IEnumerable<TechniqueInfo> TakeAll(Grid grid);
	}
}
