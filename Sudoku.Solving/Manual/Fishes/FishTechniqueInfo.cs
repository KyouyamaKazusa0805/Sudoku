using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Fishes
{
	/// <summary>
	/// Provides a usage of <b>fish</b> technique.
	/// </summary>
	public abstract class FishTechniqueInfo : TechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="digit">The digit.</param>
		/// <param name="baseSets">The base sets.</param>
		/// <param name="coverSets">The cover sets.</param>
		protected FishTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			int digit, IReadOnlyList<int> baseSets, IReadOnlyList<int> coverSets)
			: base(conclusions, views) =>
			(Digit, BaseSets, CoverSets) = (digit, baseSets, coverSets);


		/// <summary>
		/// Indicates the digit of this fish instance.
		/// </summary>
		public int Digit { get; }

		/// <summary>
		/// Indicates the size of this fish instance.
		/// </summary>
		public int Size => BaseSets.Count;

		/// <summary>
		/// Indicates the rank of the fish.
		/// </summary>
		public int Rank => CoverSets.Count - BaseSets.Count;

		/// <summary>
		/// All base sets.
		/// </summary>
		public IReadOnlyList<int> BaseSets { get; }

		/// <summary>
		/// All cover sets.
		/// </summary>
		public IReadOnlyList<int> CoverSets { get; }
	}
}
