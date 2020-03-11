using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Provides a usage of alternating inference chain (AIC) technique.
	/// </summary>
	[SuppressMessage("", "CS0660")]
	public sealed class AlternatingInferenceChainTechniqueInfo : ChainTechniqueInfo, IEquatable<AlternatingInferenceChainTechniqueInfo>
	{
		/// <summary>
		/// Indicates the last index of a collection.
		/// </summary>
		private static readonly Index LastIndex = ^1;


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
			: base(conclusions, views)
		{
			Nodes = KeepNodeMinimum(nodes);
			IsContinuousNiceLoop = isContinuousNiceLoop;
		}


		/// <summary>
		/// Indicates whether the structure is a continuous nice loop.
		/// </summary>
		public bool IsContinuousNiceLoop { get; }

		/// <summary>
		/// Indicates all inferences.
		/// </summary>
		public IReadOnlyList<Node> Nodes { get; }

		/// <summary>
		/// Indicates the length of the instance.
		/// </summary>
		public int Length => IsContinuousNiceLoop ? Nodes.Count : Nodes.Count - 1;

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
					false => Length switch
					{
						5 => true switch
						{
							_ when IsXyChain() => "XY-Wing",
							_ when IsWWing() => "W-Wing",
							_ when IsMWing() => "M-Wing",
							_ when IsHybridWing() => "Hybrid-Wing",
							_ when IsLocalWing() => "Local-Wing",
							_ when IsSplitWing() => "Split-Wing",
							_ => "Other Wing",
						},
						_ => true switch
						{
							_ when IsXChain() => "X-Chain",
							_ when !IsHeadTailSame() => Conclusions.Count switch
							{
								1 => "Discontinuous Nice Loop",
								2 => "XY-X-Chain",
								_ => throw Throwing.ImpossibleCase
							},
							_ when IsXyChain() => "XY-Chain",
							_ when IsHeadCollisionChain() => "Discontinuous Nice Loop",
							_ => "Alternating Inference Chain"
						}
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
					"XY-Wing" => 4.2M,
					"W-Wing" => 4.4M,
					"M-Wing" => 4.5M,
					"Local-Wing" => 4.7M,
					"Split-Wing" => 4.7M,
					"Hybrid-Wing" => 4.7M,
					"X-Chain" => 4.5M,
					"Fishy Cycle" => 4.5M,
					"XY-Chain" => 4.8M,
					"XY-Cycle" => 4.7M,
					"Continuous Nice Loop" => 4.8M,
					//"XY-X-Chain" => 4.9M,
					//"Discontinuous Nice Loop" => 4.9M,
					//"Alternating Inference Chain" => 4.9M,
					_ => 4.9M
				} + ChainingDifficultyRatingUtils.GetExtraDifficultyByLength(Length);
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
			if (IsContinuousNiceLoop)
			{
				if (Length != other.Length)
				{
					return false;
				}

				var thisMap = FullGridMap.Empty;
				var otherMap = FullGridMap.Empty;
				for (int i = 0; i < Length; i++)
				{
					thisMap[Nodes[i].Candidate] = true;
					otherMap[Nodes[i].Candidate] = true;
				}

				return thisMap == otherMap;
			}
			else
			{
				return GetHashCode() == other.GetHashCode();
			}
		}

		/// <inheritdoc/>
		public override int GetHashCode() => Nodes[0].Candidate << 20 + Nodes[LastIndex].Candidate;

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
		/// Indicates whether the chain is W-Wing (<c>(x = y) - y = y - (y = x)</c>).
		/// </summary>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool IsWWing()
		{
			int a = Nodes[0].Candidate, b = Nodes[1].Candidate;
			int c = Nodes[2].Candidate, d = Nodes[3].Candidate;
			int e = Nodes[4].Candidate, f = Nodes[5].Candidate;
			return a % 9 == f % 9 && b % 9 == e % 9 // Head link and tail link are both in cells.
				&& a / 9 == b / 9 && e / 9 == f / 9 // Symmetrical nodes has same digit.
				&& c % 9 == d % 9 && b % 9 == c % 9;
		}

		/// <summary>
		/// Indicates whether the chain is M-Wing (<c>(x = y) - y = (y - x) = x</c>).
		/// </summary>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool IsMWing()
		{
			int a = Nodes[0].Candidate, b = Nodes[1].Candidate;
			int c = Nodes[2].Candidate, d = Nodes[3].Candidate;
			int e = Nodes[4].Candidate, f = Nodes[5].Candidate;
			return a / 9 == b / 9 && d / 9 == e / 9
				&& b % 9 == c % 9 && c % 9 == d % 9
				&& a % 9 == e % 9 && e % 9 == f % 9
				|| f / 9 == e / 9 && c / 9 == b / 9 // Reverse case.
				&& d % 9 == e % 9 && c % 9 == d % 9
				&& b % 9 == f % 9 && a % 9 == b % 9;
		}

		/// <summary>
		/// Indicates whether the chain is Split-Wing (<c>x = x - (x = y) - y = y</c>).
		/// </summary>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool IsSplitWing()
		{
			int a = Nodes[0].Candidate, b = Nodes[1].Candidate;
			int c = Nodes[2].Candidate, d = Nodes[3].Candidate;
			int e = Nodes[4].Candidate, f = Nodes[5].Candidate;
			return a % 9 == b % 9 && b % 9 == c % 9 // First three nodes hold a same digit.
				&& d % 9 == e % 9 && e % 9 == f % 9 // Last three nodes hold a same digit.
				&& c / 9 == d / 9; // In same cell.
		}

		/// <summary>
		/// Indicates whether the chain is Hybrid-Wing.
		/// This wing has two types:
		/// <list type="bullet">
		/// <item><c>(x = y) - y = (y - z) = z</c></item>
		/// <item><c>(x = y) - (y = z) - z = z</c></item>
		/// </list>
		/// </summary>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool IsHybridWing()
		{
			int a = Nodes[0].Candidate, b = Nodes[1].Candidate;
			int c = Nodes[2].Candidate, d = Nodes[3].Candidate;
			int e = Nodes[4].Candidate, f = Nodes[5].Candidate;
			return a / 9 == b / 9 && d / 9 == e / 9
				&& b % 9 == c % 9 && c % 9 == d % 9
				&& e % 9 == f % 9
				|| e / 9 == f / 9 && b / 9 == c / 9
				&& d % 9 == e % 9 && c % 9 == d % 9
				&& a % 9 == b % 9
				|| a / 9 == b / 9 && c / 9 == d / 9 // Reverse case.
				&& b % 9 == c % 9
				&& d % 9 == e % 9 && e % 9 == f % 9
				|| e / 9 == f / 9 && c / 9 == d / 9
				&& d % 9 == e % 9
				&& b % 9 == c % 9 && a % 9 == b % 9;
		}

		/// <summary>
		/// Indicates whether the chain is Local-Wing (<c>x = (x - z) = (z - y) = y</c>).
		/// </summary>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool IsLocalWing()
		{
			int a = Nodes[0].Candidate, b = Nodes[1].Candidate;
			int c = Nodes[2].Candidate, d = Nodes[3].Candidate;
			int e = Nodes[4].Candidate, f = Nodes[5].Candidate;
			return b / 9 == c / 9 && d / 9 == e / 9
				&& a % 9 == b % 9 && c % 9 == d % 9 && e % 9 == f % 9;
		}

		/// <summary>
		/// Indicates whether the chain is discontinuous nice loop whose head and tail
		/// is a same node.
		/// </summary>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool IsHeadCollisionChain() => Nodes[0].Candidate == Nodes[LastIndex].Candidate;

		/// <summary>
		/// Keep the head node is lower than tail node.
		/// </summary>
		/// <param name="nodes">The nodes.</param>
		/// <returns>The list.</returns>
		private IReadOnlyList<Node> KeepNodeMinimum(IReadOnlyList<Node> nodes)
		{
			if (nodes[0].Candidate > nodes[LastIndex].Candidate)
			{
				var list = new List<Node>(nodes);
				for (int i = 0; i < nodes.Count >> 1; i++)
				{
					var temp = list[i];
					list[i] = list[^(i + 1)];
					list[^(i + 1)] = temp;
				}
				return list;
			}
			else
			{
				return nodes;
			}
		}

		/// <summary>
		/// Indicates whether the head and tail node contain the different digit.
		/// </summary>
		/// <returns>
		/// <see langword="true"/> is for same digit; otherwise, <see langword="false"/>.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool IsHeadTailSame() => Nodes[0].Candidate % 9 == Nodes[LastIndex].Candidate % 9;


		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(
			AlternatingInferenceChainTechniqueInfo left, AlternatingInferenceChainTechniqueInfo right) =>
			left.Equals(right);

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(
			AlternatingInferenceChainTechniqueInfo left, AlternatingInferenceChainTechniqueInfo right) =>
			!(left == right);
	}
}
