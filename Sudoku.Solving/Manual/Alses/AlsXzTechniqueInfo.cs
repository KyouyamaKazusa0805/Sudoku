using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Alses
{
	/// <summary>
	/// Provides a usage of <b>almost locked sets XZ rule</b>
	/// or <b>extended subset principle</b> technique.
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
		public override string Name
		{
			get
			{
				return isEsp() ? "Extended Subset Principle" : "Almost Locked Sets XZ Rule";

				bool isEsp()
				{
					short v1 = Rcc.Als1.RelativePosMask, v2 = Rcc.Als2.RelativePosMask;
					return (v1 & (v1 - 1)) == 0 || (v2 & (v2 - 1)) == 0;
				}
			}
		}

		/// <inheritdoc/>
		public override decimal Difficulty => 5.5M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;


		/// <inheritdoc/>
		public override bool Equals(TechniqueInfo other) =>
			other is AlsXzTechniqueInfo comparer && Equals(comparer);

		/// <include file='../../../GlobalDocComments.xml' path='comments/method[@name="Equals" and @paramType="__any"]'/>
		public bool Equals(AlsXzTechniqueInfo other) => Rcc == other.Rcc;

		/// <inheritdoc/>
		public override string ToString()
		{
			string elimStr = ConclusionCollection.ToString(Conclusions);
			return $"{Name}: {Rcc} => {elimStr}";
		}
	}
}
