using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using static Sudoku.Solving.Constants.Processings;

namespace Sudoku.Solving.Manual.Uniqueness.Bugs
{
	/// <summary>
	/// Provides a usage of <b>bivalue universal grave</b> (BUG) type 3 technique.
	/// </summary>
	public sealed class BugType3TechniqueInfo : UniquenessTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
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
		public override decimal Difficulty => 5.6M + Digits.Count * .1M + (IsNaked ? 0 : .1M);

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.BugType3;


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = new DigitCollection(Digits).ToString();
			string cellsStr = new CellCollection(Cells).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			string sizeStr = SubsetNames[Digits.Count].ToLower();
			string trueCandidatesStr = new CandidateCollection(TrueCandidates).ToString();
			string subsetTypeStr = IsNaked ? "naked" : "hidden";
			return
				$"{Name}: True candidates {trueCandidatesStr} with {subsetTypeStr} {sizeStr} {digitsStr} " +
				$"in cells {cellsStr} => {elimStr}";
		}
	}
}
