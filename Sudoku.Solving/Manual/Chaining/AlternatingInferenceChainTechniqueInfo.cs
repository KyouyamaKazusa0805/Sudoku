using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Chaining
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
		/// <param name="nodes">All nodes.</param>
		public AlternatingInferenceChainTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			IReadOnlyList<Node> nodes)
			: base(conclusions, views) => Nodes = nodes;


		/// <summary>
		/// Indicates all inferences.
		/// </summary>
		public IReadOnlyList<Node> Nodes { get; }

		/// <inheritdoc/>
		public override string Name
		{
			get
			{
				// TODO: Rename.
				return true switch
				{
					_ when IsXChain() => "X-Chain",
					_ => "Alternating Inference Chain"
				};
			}
		}

		private bool IsXChain()
		{
			int i = 0;
			int cand = default;
			bool isX = true;
			foreach (var node in Nodes)
			{
				if (i++ == 0)
				{
					cand = node.Candidate % 9;
				}
				else if (cand != node.Candidate % 9)
				{
					isX = false;
					break;
				}
			}

			return isX;
		}

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
				} + ChainingDifficultyRatingUtils.GetExtraDifficultyByLength(Nodes.Count);
			}
		}

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;


		/// <inheritdoc/>
		public override string ToString()
		{
			string elimStr = ConclusionCollection.ToString(Conclusions);
			string nodesStr = NodeCollection.ToString(Nodes);
			return $"{Name}: {nodesStr} => {elimStr}";
		}
	}
}
