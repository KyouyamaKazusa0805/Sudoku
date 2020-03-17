using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.LastResorts
{
	/// <summary>
	/// Provides a usage of <b>chute clue cover</b> (CCC) technique.
	/// </summary>
	public sealed class CccTechniqueInfo : LastResortTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="count">The total number of combinations traversed.</param>
		public CccTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			int count) : base(conclusions, views) => Count = count;


		/// <summary>
		/// Indicates the total number of combinations traversed.
		/// </summary>
		public int Count { get; }

		/// <inheritdoc/>
		public override string Name => "Chute Clue Cover";

		/// <inheritdoc/>
		public override decimal Difficulty => 9M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.LastResort;


		/// <inheritdoc/>
		public override string ToString()
		{
			string elimStr = ConclusionCollection.ToString(Conclusions);
			return $"{Name}: Traversed {Count} combinations => {elimStr}";
		}
	}
}
