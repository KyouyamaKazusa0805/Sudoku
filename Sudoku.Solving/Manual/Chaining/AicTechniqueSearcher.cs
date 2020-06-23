using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static Sudoku.Constants.Processings;
using static Sudoku.Constants.RegionLabel;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Encapsulates an <b>alternating inference chain</b> (AIC) technique searcher.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This technique searcher may use the basic searching way to find all AICs and
	/// grouped AICs. For example, this searcher will try to search for all strong
	/// inferences firstly, and then search a weak inference that the candidate is
	/// in the same region or cell with a node in the strong inference in order to
	/// link them.
	/// </para>
	/// <para>
	/// Note that AIC may be static chains, which means that the searcher may just use
	/// static analysis is fine, which is different with dynamic chains.
	/// </para>
	/// <para>By the way, the searching method is <b>BFS</b> (breadth-first searching).</para>
	/// </remarks>
	[TechniqueDisplay(nameof(TechniqueCode.Aic))]
	public sealed class AicTechniqueSearcher : ChainTechniqueSearcher
	{
		/// <summary>
		/// Indicates the maximum length to search.
		/// </summary>
		private readonly int _maxLength;

		/// <summary>
		/// Indicates whether the X-Chain is enabled.
		/// </summary>
		private readonly bool _isXEnabled;

		/// <summary>
		/// Indicates whether the Y-Chain is enabled.
		/// </summary>
		private readonly bool _isYEnabled;


		/// <summary>
		/// Initializes an instance with the specified length.
		/// </summary>
		/// <param name="maxLength">The max length.</param>
		public AicTechniqueSearcher(int maxLength) => _maxLength = maxLength;


		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 50;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override unsafe void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{

		}


		private unsafe void GetUnaryChain(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, ChainNode src, GridMap[] maps, int aa = 0)
		{
			using Table toOn = Table.Empty, toOff = Table.Empty, makeOn = Table.Empty, makeOff = Table.Empty;
			int tempOn = 0, tempOff = 0;
			var (srcCell, srcDigit, srcIsOn) = src;
			if (srcIsOn)
			{
				tempOn++;
				toOn.Add(src);
			}
			else
			{
				tempOff++;
				toOff.Add(src);
			}

			int length = 0, parent;
			while (tempOn != 0 || tempOff != 0)
			{
				length++;
				parent = toOn.Count;
				while (tempOn != 0)
				{
					tempOn--;
					var onP = &toOn.Nodes[--parent];

					GetOnToOff(onP, makeOff, grid, maps);

					for (int i = 1; i <= makeOff.Count; i++)
					{
						using var offNode = *makeOff.Remove();
						var (tempCell, tempDigit) = offNode;
						if (length > 3 && src.Similar(offNode) && offNode != *onP->Predecessors[0])
						{
							// Loop.
							var ptrToCheck = &offNode;
							bool haveShortestPath = CheckShortestPath(ptrToCheck);
							bool isContinuous;
							if (!haveShortestPath)
							{
								if (!srcIsOn && !haveShortestPath && (aa == 0 || IsNotHave(&offNode)))
								{
									isContinuous = true;
									// TODO: Make hint.
								}
								else
								{
									isContinuous = false;
									// TODO: Make hint.
								}
							}
						}

						if (toOff.Contains(offNode, out _))
						{
							toOff.Add(offNode);
							tempOff--;
						}
					}
				}

				length++;
				parent = toOff.Count;
				while (tempOff != 0)
				{

				}
			}

			src.Dispose();
		}

		private unsafe void GetOffToOn(ChainNode* pOff, Table toOff, Table outTable, IReadOnlyGrid grid, GridMap[] maps)
		{
			var alreadyMap = GridMap.Empty;
			var (rootCell, rootDigit) = *pOff;
			if (_isYEnabled && BivalueMap[rootCell])
			{
				short mask = grid.GetCandidateMask(rootCell);
				int digit = mask.SetAt(0);
				if (digit == rootDigit)
				{
					digit = mask.GetNextSet(digit);
				}

				outTable.Add(new ChainNode(rootCell, digit, true, pOff));
			}

			if (_isXEnabled)
			{
				for (var label = Block; label < UpperLimit; label++)
				{
					int region = GetRegion(rootCell, label);
					var tempMap = RegionMaps[region] & maps[rootDigit];
					if (tempMap.Count != 2)
					{
						continue;
					}

					int cell = tempMap.SetAt(0);
					if (cell == rootCell)
					{
						cell = tempMap.SetAt(1);
					}

					if (alreadyMap[cell])
					{
						continue;
					}

					alreadyMap.Add(cell);
					outTable.Add(new ChainNode(cell, rootDigit, true, pOff));
				}
			}
		}

		private unsafe void GetOnToOff(ChainNode* pOn, Table outTable, IReadOnlyGrid grid, GridMap[] maps)
		{
			var (rootCell, rootDigit) = *pOn;
			if (_isYEnabled)
			{
				short mask = (short)(grid.GetCandidateMask(rootCell) & ~(1 << rootDigit));
				if (mask != 0)
				{
					foreach (int digit in mask.GetAllSets())
					{
						outTable.Add(new ChainNode(rootCell, digit, false, pOn));
					}
				}
			}

			var tempMap = maps[rootDigit] & PeerMaps[rootCell];
			if (tempMap.IsNotEmpty)
			{
				foreach (int cell in tempMap)
				{
					outTable.Add(new ChainNode(cell, rootDigit, false, pOn));
				}
			}
		}


		private static unsafe bool CheckShortestPath(ChainNode* start)
		{
			var src = *start;
			var p = start->Predecessors[0];
			while (p->PredecessorsCount != 0)
			{
				var (cell, digit, isOn) = *p;
				if (new ChainNode(cell, digit, !isOn).IsParentOf(*p, out _))
				{
					return true;
				}

				p = p->Predecessors[0];
			}

			return false;
		}
	}
}
