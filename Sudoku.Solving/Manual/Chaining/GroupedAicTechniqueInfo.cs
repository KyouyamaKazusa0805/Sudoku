using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;
using Sudoku.Windows;
using static Sudoku.Solving.Constants.Processings;
using static Sudoku.Solving.Manual.TechniqueCode;

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


		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="nodes">All nodes.</param>
		/// <param name="isContinuousNiceLoop">
		/// Indicates whether the structure is a continuous nice loop.
		/// </param>
		public GroupedAicTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			IReadOnlyList<Node> nodes, bool isContinuousNiceLoop) : base(conclusions, views) =>
			(Nodes, IsContinuousNiceLoop) = (KeepNodeMinimum(nodes), isContinuousNiceLoop);


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
		public override decimal Difficulty =>
			TechniqueCode switch
			{
				XyWing => 4.2M,
				WWing => 4.4M,
				MWing => 4.5M,
				LocalWing => 4.7M,
				SplitWing => 4.7M,
				HybridWing => 4.7M,
				XChain => 4.5M,
				FishyCycle => 4.5M,
				XyChain => 4.8M,
				XyCycle => 4.7M,
				ContinuousNiceLoop => 4.8M,
				XyXChain => 4.9M,
				DiscontinuousNiceLoop => 4.9M,
				Aic => 4.9M,
				GroupedWWing => 4.6M,
				GroupedMWing => 4.7M,
				GroupedLocalWing => 4.9M,
				GroupedSplitWing => 4.9M,
				GroupedHybridWing => 4.9M,
				GroupedXChain => 4.7M,
				GroupedFishyCycle => 4.7M,
				GroupedXyXChain => 5.1M,
				GroupedDiscontinuousNiceLoop => 5.1M,
				GroupedAic => 5M,
				GroupedContinuousNiceLoop => 5M,
				GroupedXyWing => 4.4M, // Impossible case.
				GroupedXyChain => 5M, // Impossible case.
				GroupedXyCycle => 4.9M, // Impossible case.
				_ => 4.9M
			} + GetExtraDifficultyByLength(Length);

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode =>
			IsContinuousNiceLoop switch
			{
				true => true switch
				{
					_ when IsXChain() => IsGrouped() ? GroupedFishyCycle : FishyCycle,
					_ when IsXyChain() => IsGrouped() ? GroupedXyCycle : XyCycle,
					_ => IsGrouped() ? GroupedContinuousNiceLoop : ContinuousNiceLoop
				},
				false => Length switch
				{
					5 => true switch
					{
						_ when IsXChain() => IsGrouped() ? GroupedXChain : XChain,
						_ when IsXyChain() => IsGrouped() ? GroupedXyWing : XyWing,
						_ when IsWWing() => IsGrouped() ? GroupedWWing : WWing,
						_ when IsMWing() => IsGrouped() ? GroupedMWing : MWing,
						_ when IsHybridWing() => IsGrouped() ? GroupedHybridWing : HybridWing,
						_ when IsLocalWing() => IsGrouped() ? GroupedLocalWing : LocalWing,
						_ when IsSplitWing() => IsGrouped() ? GroupedSplitWing : SplitWing,
						_ => IsGrouped() ? GroupedPurpleCow : PurpleCow,
					},
					_ => true switch
					{
						_ when IsXChain() => IsGrouped() ? GroupedXChain : XChain,
						_ when IsXyChain() => IsGrouped() ? GroupedXyChain : XyChain,
						_ when IsHeadTailSame() => IsGrouped() ? GroupedDiscontinuousNiceLoop : DiscontinuousNiceLoop,
						_ when !IsHeadTailSameDigit() => Conclusions.Count switch
						{
							1 => IsGrouped() ? GroupedDiscontinuousNiceLoop : DiscontinuousNiceLoop,
							2 => IsGrouped() ? GroupedXyXChain : XyXChain,
							_ => IsGrouped() ? GroupedAic : Aic
						},
						_ when IsHeadCollisionChain() => IsGrouped() ? GroupedDiscontinuousNiceLoop : DiscontinuousNiceLoop,
						_ => IsGrouped() ? GroupedAic : Aic
					}
				}
			};
		//Name switch
		//{
		//	"XY-Wing" => TechniqueCode.XyWing,
		//	"W-Wing" => TechniqueCode.WWing,
		//	"M-Wing" => TechniqueCode.MWing,
		//	"Local-Wing" => TechniqueCode.LocalWing,
		//	"Split-Wing" => TechniqueCode.SplitWing,
		//	"Hybrid-Wing" => TechniqueCode.HybridWing,
		//	"Purple Cow" => TechniqueCode.PurpleCow,
		//	"X-Chain" => TechniqueCode.XChain,
		//	"Fishy Cycle" => TechniqueCode.XChain,
		//	"XY-Chain" => TechniqueCode.XyChain,
		//	"XY-Cycle" => TechniqueCode.XyChain,
		//	"Continuous Nice Loop" => TechniqueCode.ContinuousNiceLoop,
		//	"XY-X-Chain" => TechniqueCode.XyXChain,
		//	"Discontinuous Nice Loop" => TechniqueCode.DiscontinuousNiceLoop,
		//	"Alternating Inference Chain" => TechniqueCode.Aic,
		//	"Grouped W-Wing" => TechniqueCode.GroupedWWing,
		//	"Grouped M-Wing" => TechniqueCode.GroupedMWing,
		//	"Grouped Local-Wing" => TechniqueCode.GroupedLocalWing,
		//	"Grouped Split-Wing" => TechniqueCode.GroupedSplitWing,
		//	"Grouped Hybrid-Wing" => TechniqueCode.GroupedHybridWing,
		//	"Grouped Purple Cow" => TechniqueCode.GroupedPurpleCow,
		//	"Grouped X-Chain" => TechniqueCode.GroupedXChain,
		//	"Grouped Fishy Cycle" => TechniqueCode.GroupedXChain,
		//	"Grouped XY-X-Chain" => TechniqueCode.GroupedXyXChain,
		//	"Grouped Discontinuous Nice Loop" => TechniqueCode.GroupedDiscontinuousNiceLoop,
		//	"Grouped Alternating Inference Chain" => TechniqueCode.GroupedAic,
		//	"Grouped Continuous Nice Loop" => TechniqueCode.GroupedContinuousNiceLoop,
		//	"Grouped XY-Wing" => TechniqueCode.GroupedXyWing,
		//	"Grouped XY-Chain" => TechniqueCode.GroupedXyChain,
		//	"Grouped XY-Cycle" => TechniqueCode.GroupedXyChain,
		//	_ => throw Throwings.ImpossibleCase
		//};


		/// <inheritdoc/>
		public override string ToString()
		{
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			string nodesStr = NodeCollection.ToString(Nodes);
			return $"{Name}: {nodesStr} => {elimStr}";
		}

		/// <inheritdoc/>
		public override bool Equals(object? obj) => Equals(obj as TechniqueInfo);

		/// <inheritdoc/>
		public override bool Equals(TechniqueInfo? obj) => obj is GroupedAicTechniqueInfo comparer && Equals(comparer);

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
		private bool IsGrouped() => Nodes.Any(node => node.NodeType != NodeType.Candidate);

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
			if (IsGrouped())
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
			if (IsGrouped())
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
			if (IsGrouped())
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
			if (IsGrouped())
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
			if (IsGrouped())
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
			if (IsGrouped())
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
		/// Get the grouped prefix.
		/// </summary>
		/// <returns>The prefix.</returns>
		private string GroupedPrefix() => IsGrouped() ? $"{Resources.GetValue("Grouped")}" : string.Empty;

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
		public static bool operator ==(GroupedAicTechniqueInfo left, GroupedAicTechniqueInfo right) => left.Equals(right);

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(GroupedAicTechniqueInfo left, GroupedAicTechniqueInfo right) => !(left == right);
	}
}
