using System;
using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.LastResorts
{
	/// <summary>
	/// Provides a usage of brute force technique.
	/// </summary>
	public sealed class BruteForceTechniqueInfo : LastResortTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		public BruteForceTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views)
			: base(conclusions, views)
		{
		}


		/// <inheritdoc/>
		public override string Name => "Brute Force";

		/// <inheritdoc/>
		public override decimal Difficulty => 20.0m;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.LastResort;


		/// <inheritdoc/>
		public override string ToString() =>
			$"{Name}: {ConclusionCollection.ToString(Conclusions)}";
	}
}
