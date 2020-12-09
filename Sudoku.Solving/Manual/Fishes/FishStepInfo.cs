using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Fishes
{
	/// <summary>
	/// Provides a usage of <b>fish</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Digit">The digit used.</param>
	/// <param name="BaseSets">The base sets.</param>
	/// <param name="CoverSets">The cover sets.</param>
	public abstract record FishStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Digit, IReadOnlyList<int> BaseSets,
		IReadOnlyList<int> CoverSets) : StepInfo(Conclusions, Views)
	{
		/// <summary>
		/// Indicates the size of this fish instance.
		/// </summary>
		public int Size => BaseSets.Count;

		/// <summary>
		/// Indicates the rank of the fish.
		/// </summary>
		public int Rank => CoverSets.Count - BaseSets.Count;
	}
}
