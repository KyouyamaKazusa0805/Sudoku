using System.Collections;
using System.Collections.Generic;
using Sudoku.Solving.Manual;

namespace Sudoku.Solving.Tracing
{
	/// <summary>
	/// Define a traceable instance.
	/// </summary>
	public interface ITraceable : IEnumerable<(int Candidate, StepInfo Info)>
	{
		/// <summary>
		/// Get the bound step of a specified candidate.
		/// </summary>
		/// <param name="candidate">The candidate.</param>
		/// <returns>
		/// The bound step information instance. If the grid doesn't bind with the conclusion,
		/// the return value will be <see langword="null"/>.
		/// </returns>
		StepInfo? this[int candidate] { get; }

		/// <summary>
		/// Get the bound step of a specified candidate.
		/// </summary>
		/// <param name="cell">The cell of that candidate.</param>
		/// <param name="digit">The digit of that candidate.</param>
		/// <returns>
		/// The bound step information instance. If the grid doesn't bind with the conclusion,
		/// the return value will be <see langword="null"/>.
		/// </returns>
		StepInfo? this[int cell, int digit] { get; }


		/// <summary>
		/// To bind a specified step with a candidate, which is specified in the step.
		/// </summary>
		/// <param name="stepInfo">The step information.</param>
		void BindStep(StepInfo stepInfo);

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<(int, StepInfo)>)this).GetEnumerator();
	}
}
