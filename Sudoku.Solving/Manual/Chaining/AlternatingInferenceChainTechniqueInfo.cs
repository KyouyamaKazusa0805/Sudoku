using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Provides a usage of alternating inference chain (AIC) technique.
	/// </summary>
	public sealed class AlternatingInferenceChainTechniqueInfo : ChainTechniqueInfo, IEquatable<AlternatingInferenceChainTechniqueInfo>
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="nodes">All nodes.</param>
		/// <param name="isContinuousNiceLoop">
		/// Indicates whether the structure is a continuous nice loop.
		/// </param>
		public AlternatingInferenceChainTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			IReadOnlyList<Node> nodes, bool isContinuousNiceLoop)
			: base(conclusions, views) =>
			(Nodes, IsContinuousNiceLoop) = (nodes, isContinuousNiceLoop);


		/// <summary>
		/// Indicates whether the structure is a continuous nice loop.
		/// </summary>
		public bool IsContinuousNiceLoop { get; }

		/// <summary>
		/// Indicates all inferences.
		/// </summary>
		public IReadOnlyList<Node> Nodes { get; }

		/// <inheritdoc/>
		public override string Name
		{
			get
			{
				return IsContinuousNiceLoop switch
				{
					true => true switch
					{
						_ when IsXChain() => "Fishy Cycle",
						_ when IsXyChain() => "XY-Cycle",
						_ => "Continuous Nice Loop"
					},
					false => true switch
					{
						_ when IsXChain() => "X-Chain",
						_ when !IsHeadTailSame() => Conclusions.Count switch
						{
							1 => "Discontinuous Nice Loop",
							2 => "XY-X-Chain",
							_ => throw Throwing.ImpossibleCase
						},
						_ when IsXyChain() => "XY-Chain",
						_ => "Alternating Inference Chain"
					}
				};
			}
		}

		/// <inheritdoc/>
		public override decimal Difficulty
		{
			get
			{
				return Name switch
				{
					"X-Chain" => 4.5m,
					"Fishy Cycle" => 4.5m,
					"XY-Chain" => 4.8m,
					"XY-Cycle" => 4.7m,
					"Continuous Nice Loop" => 4.8m,
					//"XY-X-Chain" => 4.9m,
					//"Discontinuous Nice Loop" => 4.9m,
					//"Alternating Inference Chain" => 4.9m,
					_ => 4.9m
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

		/// <inheritdoc/>
		public override bool Equals(TechniqueInfo obj) =>
			obj is AlternatingInferenceChainTechniqueInfo comparer && Equals(comparer);

		/// <inheritdoc/>
		public bool Equals(AlternatingInferenceChainTechniqueInfo other)
		{
			int head = Nodes[0].Candidate;
			int tail = Nodes[^1].Candidate;
			int head2 = other.Nodes[0].Candidate;
			int tail2 = other.Nodes[^1].Candidate;
			return head == head2 && tail == tail2 || head == tail2 && tail == head2;
		}

		/// <inheritdoc/>
		public override int GetHashCode() => Nodes[0].Candidate << 20 + Nodes[^1].Candidate;

		/// <summary>
		/// Indicates whether the chain is X-Chain.
		/// </summary>
		/// <returns>A <see cref="bool"/> value.</returns>
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

		/// <summary>
		/// Indicates whether the chain is XY-Chain.
		/// </summary>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		private bool IsXyChain()
		{
			for (int i = 0; i < Nodes.Count; i += 2)
			{
				if (Nodes[i].Candidate / 9 != Nodes[i + 1].Candidate / 9)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Indicates whether the head and tail node contain the different digit.
		/// </summary>
		/// <returns>
		/// <see langword="true"/> is for same digit; otherwise, <see langword="false"/>.
		/// </returns>
		private bool IsHeadTailSame() => Nodes[0].Candidate % 9 == Nodes[^1].Candidate % 9;
	}
}
