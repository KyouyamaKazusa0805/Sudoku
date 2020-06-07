using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.LastResorts
{
	/// <summary>
	/// Provides a usage of <b>chute clue cover</b> (CCC) technique.
	/// </summary>
	public sealed class CccTechniqueInfo : LastResortTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="count">The total number of combinations traversed.</param>
		public CccTechniqueInfo(IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, int count)
			: base(conclusions, views) => Count = count;


		/// <summary>
		/// Indicates the total number of combinations traversed.
		/// </summary>
		public int Count { get; }

		/// <inheritdoc/>
		public override decimal Difficulty => 9M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.LastResort;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.Ccc;


		/// <inheritdoc/>
		public override string ToString()
		{
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: Traversed {Count} combinations => {elimStr}";
		}
	}
}
