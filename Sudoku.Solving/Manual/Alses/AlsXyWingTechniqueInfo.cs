using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Alses
{
	/// <summary>
	/// Provides a usage of <b>ALS-XY-Wing</b> technique.
	/// </summary>
	public sealed class AlsXyWingTechniqueInfo : AlsTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="rcc1">The RCC 1.</param>
		/// <param name="rcc2">The RCC 2.</param>
		public AlsXyWingTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			Rcc rcc1, Rcc rcc2) : base(conclusions, views) => (Rcc1, Rcc2) = (rcc1, rcc2);


		/// <summary>
		/// Indicates the RCC 1.
		/// </summary>
		public Rcc Rcc1 { get; }

		/// <summary>
		/// Indicates the RCC 2.
		/// </summary>
		public Rcc Rcc2 { get; }

		/// <inheritdoc/>
		public override string Name => "Almost Locked Sets XY-Wing";

		/// <inheritdoc/>
		public override decimal Difficulty => 6M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;


		/// <inheritdoc/>
		public override string ToString()
		{
			string elimStr = ConclusionCollection.ToString(Conclusions);
			return $"{Name}: {Rcc1} + {Rcc2} => {elimStr}";
		}
	}
}
