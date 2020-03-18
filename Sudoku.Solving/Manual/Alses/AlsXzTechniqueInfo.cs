using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Alses
{
	/// <summary>
	/// Provides a usage of <b>ALS-XZ</b> technique.
	/// </summary>
	public sealed class AlsXzTechniqueInfo : AlsTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="rcc">The RCC used.</param>
		public AlsXzTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, Rcc rcc)
			: base(conclusions, views) => Rcc = rcc;


		/// <summary>
		/// Indicates the RCC used.
		/// </summary>
		public Rcc Rcc { get; }

		/// <inheritdoc/>
		public override string Name => "Almost Locked Sets XZ Rule";

		/// <inheritdoc/>
		public override decimal Difficulty => 5.5M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;


		/// <inheritdoc/>
		public override string ToString()
		{
			string elimStr = ConclusionCollection.ToString(Conclusions);
			return $"{Name}: {Rcc} => {elimStr}";
		}
	}
}
