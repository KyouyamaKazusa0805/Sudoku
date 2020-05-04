using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Provides a usage of (<b>grouped</b>) <b>alternating inference chain</b> (AIC) technique.
	/// In fact this searcher can also search for basic AICs.
	/// </summary>
	public sealed class GroupedAicTechniqueInfo : ChainTechniqueInfo
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
		public GroupedAicTechniqueInfo(
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
				string groupedSuffix() => IsGroupedChain() ? "Grouped " : string.Empty;
				return IsContinuousNiceLoop switch
				{
					true => true switch
					{
						_ when IsXChain() => $"{groupedSuffix()}Fishy Cycle",
						_ when IsXyChain() => $"{groupedSuffix()}XY-Cycle",
						_ => $"{groupedSuffix()}Continuous Nice Loop"
					},
					false => Length switch
					{
						5 => true switch
						{
							_ when IsXChain() => $"{groupedSuffix()}X-Chain",
							_ when IsXyChain() => $"{groupedSuffix()}XY-Wing",
							_ when IsWWing() => $"{groupedSuffix()}W-Wing",
							_ when IsMWing() => $"{groupedSuffix()}M-Wing",
							_ when IsHybridWing() => $"{groupedSuffix()}Hybrid-Wing",
							_ when IsLocalWing() => $"{groupedSuffix()}Local-Wing",
							_ when IsSplitWing() => $"{groupedSuffix()}Split-Wing",
							_ => $"{groupedSuffix()}Purple Cow",
						},
						_ => true switch
						{
							_ when IsXChain() => $"{groupedSuffix()}X-Chain",
							_ when IsXyChain() => $"{groupedSuffix()}XY-Chain",
							_ when IsHeadTailSame() => $"{groupedSuffix()}Discontinuous Nice Loop",
							_ when !IsHeadTailSameDigit() => Conclusions.Count switch
							{
								1 => $"{groupedSuffix()}Discontinuous Nice Loop",
								2 => $"{groupedSuffix()}XY-X-Chain",
								_ => $"{groupedSuffix()}Alternating Inference Chain"
							},
							_ when IsHeadCollisionChain() => $"{groupedSuffix()}Discontinuous Nice Loop",
							_ => $"{groupedSuffix()}Alternating Inference Chain"
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
					"XY-X-Chain" => 4.9M,
					"Discontinuous Nice Loop" => 4.9M,
					"Alternating Inference Chain" => 4.9M,
					"Grouped W-Wing" => 4.6M,
					"Grouped M-Wing" => 4.7M,
					"Grouped Local-Wing" => 4.9M,
					"Grouped Split-Wing" => 4.9M,
					"Grouped Hybrid-Wing" => 4.9M,
					"Grouped X-Chain" => 4.7M,
					"Grouped Fishy Cycle" => 4.7M,
					"Grouped XY-X-Chain" => 5.1M,
					"Grouped Discontinuous Nice Loop" => 5.1M,
					"Grouped Alternating Inference Chain" => 5M,
					"Grouped Continuous Nice Loop" => 5M,
					"Grouped XY-Wing" => 4.4M,
					"Grouped XY-Chain" => 5M,
					"Grouped XY-Cycle" => 4.9M,
					_ => 4.9M
				} + ChainingDifficultyRatingUtils.GetExtraDifficultyByLength(Length);
			}
		}

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;


		/// <inheritdoc/>
		public override string ToString()
		{
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			string nodesStr = NodeCollection.ToString(Nodes);
			return $"{Name}: {nodesStr} => {elimStr}";
		}

		/// <inheritdoc/>
		public override bool Equals(object? obj) =>
			obj is GroupedAicTechniqueInfo comparer && Equals(comparer);

		/// <inheritdoc/>
		public override bool Equals(TechniqueInfo obj) =>
			obj is GroupedAicTechniqueInfo comparer && Equals(comparer);

		/// <inheritdoc/>
		public bool Equals(GroupedAicTechniqueInfo other)
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
					thisMap.AddRange(Nodes[i].Candidates);
					otherMap.AddRange(other.Nodes[i].Candidates);
				}

				return thisMap == otherMap;
			}
			else
			{
				return GetHashCode() == other.GetHashCode();
			}
		}

		/// <inheritdoc/>
		public override int GetHashCode() =>
			Nodes[0].CandidatesMap.GetHashCode() << 20 + Nodes[LastIndex].GetHashCode();

		/// <summary>
		/// Indicates whether the chain is grouped chain.
		/// </summary>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool IsGroupedChain() => Nodes.Any(node => node.NodeType != NodeType.Candidate);

		/// <summary>
		/// Indicates whether the chain is X-Chain.
		/// </summary>
		/// <returns>A <see cref="bool"/> value.</returns>
		private bool IsXChain()
		{
			int i = 0, cand = default;
			bool isX = true;
			foreach (var node in Nodes)
			{
				foreach (int candidate in node.Candidates)
				{
					if (i++ == 0)
					{
						cand = candidate % 9;
					}
					else if (cand != candidate % 9)
					{
						isX = false;
						break;
					}
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
			if (IsGroupedChain())
			{
				return false;
			}

			for (int i = 0; i < Nodes.Count; i += 2)
			{
				if (Nodes[i][0] / 9 != Nodes[i + 1][0] / 9)
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
			if (IsGroupedChain())
			{
				return false;
			}

			int a = Nodes[0][0], b = Nodes[1][0];
			int c = Nodes[2][0], d = Nodes[3][0];
			int e = Nodes[4][0], f = Nodes[5][0];
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
			if (IsGroupedChain())
			{
				return false;
			}

			int a = Nodes[0][0], b = Nodes[1][0];
			int c = Nodes[2][0], d = Nodes[3][0];
			int e = Nodes[4][0], f = Nodes[5][0];
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
			if (IsGroupedChain())
			{
				return false;
			}

			int a = Nodes[0][0], b = Nodes[1][0];
			int c = Nodes[2][0], d = Nodes[3][0];
			int e = Nodes[4][0], f = Nodes[5][0];
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
			if (IsGroupedChain())
			{
				return false;
			}

			int a = Nodes[0][0], b = Nodes[1][0];
			int c = Nodes[2][0], d = Nodes[3][0];
			int e = Nodes[4][0], f = Nodes[5][0];
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
			if (IsGroupedChain())
			{
				return false;
			}

			int a = Nodes[0][0], b = Nodes[1][0];
			int c = Nodes[2][0], d = Nodes[3][0];
			int e = Nodes[4][0], f = Nodes[5][0];
			return b / 9 == c / 9 && d / 9 == e / 9
				&& a % 9 == b % 9 && c % 9 == d % 9 && e % 9 == f % 9;
		}

		/// <summary>
		/// Indicates whether the chain is discontinuous nice loop whose head and tail
		/// is a same node.
		/// </summary>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool IsHeadCollisionChain() => Nodes[0][0] == Nodes[LastIndex][0];

		/// <summary>
		/// Keep the head node is lower than tail node.
		/// </summary>
		/// <param name="nodes">The nodes.</param>
		/// <returns>The list.</returns>
		private IReadOnlyList<Node> KeepNodeMinimum(IReadOnlyList<Node> nodes)
		{
			if (nodes[0][0] > nodes[LastIndex][0])
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
		private bool IsHeadTailSameDigit() => Nodes[0][0] % 9 == Nodes[LastIndex][0] % 9;

		/// <summary>
		/// Indicates whether the head and tail node are in different cells.
		/// </summary>
		/// <returns>
		/// <see langword="true"/> is for same digit; otherwise, <see langword="false"/>.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool IsHeadTailSameCell() => Nodes[0][0] / 9 == Nodes[LastIndex][0] / 9;

		/// <summary>
		/// Indicates whether the head and tail node are difference values.
		/// </summary>
		/// <returns>
		/// <see langword="true"/> is for same digit; otherwise, <see langword="false"/>.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool IsHeadTailSame() => Nodes[0][0] == Nodes[LastIndex][0];


		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(GroupedAicTechniqueInfo left, GroupedAicTechniqueInfo right) =>
			left.Equals(right);

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(GroupedAicTechniqueInfo left, GroupedAicTechniqueInfo right) =>
			!(left == right);
	}
}
