using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Chaining
{
	/// <summary>
	/// Provides a usage of alternating inference chain (AIC) technique.
	/// </summary>
	public sealed class AlternatingInferenceChainTechniqueInfo : ChainTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="inferences">All inferences.</param>
		public AlternatingInferenceChainTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			IReadOnlyList<Inference> inferences)
			: base(conclusions, views) => Inferences = inferences;


		/// <summary>
		/// Indicates all inferences.
		/// </summary>
		public IReadOnlyList<Inference> Inferences { get; }

		/// <inheritdoc/>
		public override string Name => throw new System.NotImplementedException();

		/// <inheritdoc/>
		public override decimal Difficulty
		{
			get
			{
				return Name switch
				{
					"X-Chain" => 4.5m,
					"XY-Chain" => 4.8m,
					"Alternating Inference Chain" => 4.9m,
					_ => throw Throwing.ImpossibleCase
				} + ChainingDifficultyRatingUtils.GetExtraDifficultyByLength(Inferences.Count);
			}
		}

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;


		/// <inheritdoc/>
		public override string ToString()
		{
			string elimStr = ConclusionCollection.ToString(Conclusions);
			return $"{Name}: => {elimStr}";
		}
	}
}
