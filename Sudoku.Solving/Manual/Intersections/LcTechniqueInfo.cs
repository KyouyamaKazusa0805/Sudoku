using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Intersections
{
	/// <summary>
	/// Provides a usage of <b>locked candidates</b> (LC) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Digit">The digit.</param>
	/// <param name="BaseSet">The base region.</param>
	/// <param name="CoverSet">The cover region.</param>
	public sealed record LcTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Digit, int BaseSet, int CoverSet)
		: IntersectionTechniqueInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => BaseSet < 9 ? 2.6M : 2.8M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Moderate;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => BaseSet < 9 ? TechniqueCode.Pointing : TechniqueCode.Claiming;


		/// <inheritdoc/>
		public override string ToString()
		{
			int value = Digit + 1;
			string baseSetStr = new RegionCollection(BaseSet).ToString();
			string coverSetStr = new RegionCollection(CoverSet).ToString();
			using var elims = new ConclusionCollection(Conclusions);
			string elimStr = elims.ToString();
			return $@"{Name}: {value} in {baseSetStr}\{coverSetStr} => {elimStr}";
		}
	}
}
