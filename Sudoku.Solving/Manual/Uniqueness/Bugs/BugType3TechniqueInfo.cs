using System.Collections.Generic;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Uniqueness.Bugs
{
	/// <summary>
	/// Provides a usage of <b>bivalue universal grave</b> (BUG) type 3 technique.
	/// </summary>
	public sealed class BugType3TechniqueInfo : UniquenessTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="trueCandidates">All true candidates.</param>
		/// <param name="digits">All digits.</param>
		/// <param name="cells">All cells.</param>
		/// <param name="isNaked">Indicates whether the subset is naked.</param>
		public BugType3TechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			IReadOnlyList<int> trueCandidates, IReadOnlyList<int> digits,
			IReadOnlyList<int> cells, bool isNaked)
			: base(conclusions, views) =>
			(TrueCandidates, Digits, Cells, IsNaked) = (trueCandidates, digits, cells, isNaked);


		/// <summary>
		/// Indicates all true candidates.
		/// </summary>
		public IReadOnlyList<int> TrueCandidates { get; }

		/// <summary>
		/// Indicates all digits.
		/// </summary>
		public IReadOnlyList<int> Digits { get; }

		/// <summary>
		/// Indicates all cells.
		/// </summary>
		public IReadOnlyList<int> Cells { get; }

		/// <summary>
		/// Indicates whether the technique is with naked subsets.
		/// </summary>
		public bool IsNaked { get; }

		/// <inheritdoc/>
		public override string Name => "Bivalue Universal Grave (Type 3)";

		/// <inheritdoc/>
		public override decimal Difficulty => 5.6M + Digits.Count * .1M + (IsNaked ? 0 : .1M);

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.VeryHard;


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = new DigitCollection(Digits).ToString();
			string cellsStr = new CellCollection(Cells).ToString();
			string elimStr = ConclusionCollection.ToString(Conclusions);
			string sizeStr = SubsetUtils.GetNameBy(Digits.Count).ToLower();
			string trueCandidatesStr = CandidateCollection.ToString(TrueCandidates);
			string subsetTypeStr = IsNaked ? "naked" : "hidden";
			return
				$"{Name}: True candidates {trueCandidatesStr} with {subsetTypeStr} {sizeStr} {digitsStr} " +
				$"in cells {cellsStr} => {elimStr}";
		}
	}
}
