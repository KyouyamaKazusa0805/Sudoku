using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Extensions;

namespace Sudoku.Solving.Manual.Uniqueness.Square
{
	/// <summary>
	/// Provides a usage of <b>unique square type 1</b> (US) technique.
	/// </summary>
	public sealed class UsType1TechniqueInfo : UsTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="cells">The cells.</param>
		/// <param name="digitsMask">The digits mask.</param>
		/// <param name="candidate">The candidate.</param>
		public UsType1TechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, GridMap cells, short digitsMask,
			int candidate) : base(conclusions, views, cells, digitsMask) => Candidate = candidate;


		/// <summary>
		/// Indicates the true candidate.
		/// </summary>
		public int Candidate { get; }

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.UsType1;


		/// <inheritdoc/>
		public override string ToString()
		{
			string candStr = new CandidateCollection(Candidate).ToString();
			string digitsStr = new DigitCollection(DigitsMask.GetAllSets()).ToString();
			string cellsStr = new CellCollection(Cells).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return
				$"{Name}: Digits {digitsStr} in cells {cellsStr} will form a deadly pattern if " +
				$"the candidate {candStr} is false => {elimStr}";
		}
	}
}
