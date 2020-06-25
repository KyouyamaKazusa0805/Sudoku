using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Provides a usage of <b>alternating inference chain</b> (AIC) technique.
	/// </summary>
	public sealed class AicTechniqueInfo : ChainingTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="inferences">The inferences.</param>
		/// <param name="isContinuousNiceLoop">
		/// A <see cref="bool"/> value indicating whether the structure forms a continuous nice loop.
		/// </param>
		public AicTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			IReadOnlyList<Link> inferences, bool isContinuousNiceLoop)
			: base(conclusions, views) => (Inferences, IsContinuousNiceLoop) = (inferences, isContinuousNiceLoop);


		/// <summary>
		/// Indicates whether the structure forms a continuous nice loop.
		/// </summary>
		public bool IsContinuousNiceLoop { get; }

		/// <summary>
		/// Indicates the inferences used.
		/// </summary>
		public IReadOnlyList<Link> Inferences { get; }

		/// <inheritdoc/>
		public override decimal Difficulty => 4.5M;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.Aic;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;


		/// <inheritdoc/>
		public override string ToString()
		{
			string chainStr = new LinkCollection(Inferences).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: {chainStr} => {elimStr}";
		}
	}
}
