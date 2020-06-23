using System;
using System.Collections.Generic;
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
		/// The cache of nice loops.
		/// </summary>
		private readonly GridMap[] _nl = new GridMap[1000];

		/// <summary>
		/// The cache of alternating inference chains.
		/// </summary>
		private readonly GridMap[] _aic = new GridMap[1000];

		/// <summary>
		/// The current number of nice loops.
		/// </summary>
		private int _nlcnt;

		/// <summary>
		/// The current number of alternating inference chains.
		/// </summary>
		private int _aiccnt;


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
			var candMaps = (GridMap[])CandMaps.Clone();
			GetAll(accumulator, grid, true, false, candMaps);
			GetAll(accumulator, grid, false, true, candMaps);
			GetAll(accumulator, grid, true, true, candMaps);
		}

		private bool GetAll(
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, bool isXEnabled, bool isYEnabled, GridMap[] maps)
		{
			foreach (byte cell in EmptyMap)
			{
				if (isYEnabled && !isXEnabled && !BivalueMap[cell])
				{
					continue;
				}

				foreach (byte digit in grid.GetCandidates(cell))
				{
					using var p = new ChainNode(cell, digit, true);
					if (GetUnaryChain(accumulator, grid, p, maps, isXEnabled, isYEnabled))
					{
						return true;
					}

					using var q = new ChainNode(cell, digit, false);
					if (GetUnaryChain(accumulator, grid, q, maps, isXEnabled, isYEnabled))
					{
						return true;
					}
				}
			}

			return false;
		}


		private unsafe bool GetUnaryChain(
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, ChainNode src, GridMap[] maps,
			bool isXEnabled, bool isYEnabled)
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

					GetOnToOff(onP, makeOff, grid, maps, isYEnabled);

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
								if (!srcIsOn && !haveShortestPath && CacheDoesNotContainLoop(&offNode))
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

						if (!toOff.Contains(offNode, out _))
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
					tempOff--;
					var offP = &toOff.Nodes[--parent];

					GetOffToOn(offP, toOff, makeOn, grid, maps, isXEnabled, isYEnabled);

					for (int i = 1; i < makeOn.Count; i++)
					{
						using var pOn = *makeOn.Remove();
						var (tempCell, tempDigit) = pOn;
						if (length > 3 && pOn != *offP->Predecessors[0])
						{
							var nodeToCheck = &pOn;
							bool haveShortestPath = CheckShortestPath(nodeToCheck);
							if (!haveShortestPath)
							{
								bool isContinuous;
								if (srcIsOn && pOn.Similar(src) && CacheDoesNotContainLoop(&pOn))
								{
									isContinuous = true;
									// TODO: Make hint.
								}
								else
								if (pOn.Similar(src) && !srcIsOn
									|| tempCell == srcCell && tempDigit != srcDigit && srcIsOn)
								{
									// Discontinuous nice loop.
								}
								else
								if (tempCell != srcCell && tempDigit == srcDigit && !srcIsOn && !toOn.Contains(pOn, out _)
									&& CacheDoesNotContainAic(&pOn))
								{
									// Alternating Inference Chain Type 1.
								}
								else
								if (tempCell != srcCell && tempDigit != srcDigit && !srcIsOn
									&& new GridMap { tempCell, srcCell }.AllSetsAreInOneRegion(out _)
									&& ((1 << srcDigit) & grid.GetCandidateMask(tempCell)) != 0
									|| ((1 << tempDigit) & grid.GetCandidateMask(srcCell)) != 0
									&& CacheDoesNotContainAic(&pOn))
								{
									// XY-X-Chain, XY-Chain or X-Chain.

								}
							}
						}
					}
				}
			}

			src.Dispose();
		}

		private unsafe void GetOffToOn(
			ChainNode* pOff, Table toOff, Table outTable, IReadOnlyGrid grid, GridMap[] maps,
			bool isXEnabled, bool isYEnabled)
		{
			var alreadyMap = GridMap.Empty;
			var (rootCell, rootDigit) = *pOff;
			if (isYEnabled && BivalueMap[rootCell])
			{
				short mask = grid.GetCandidateMask(rootCell);
				byte digit = (byte)mask.SetAt(0);
				if (digit == rootDigit)
				{
					digit = (byte)mask.GetNextSet(digit);
				}

				outTable.Add(new ChainNode(rootCell, digit, true, pOff));
			}

			if (isXEnabled)
			{
				for (var label = Block; label < UpperLimit; label++)
				{
					int region = GetRegion(rootCell, label);
					var tempMap = RegionMaps[region] & maps[rootDigit];
					if (tempMap.Count != 2)
					{
						continue;
					}

					byte cell = (byte)tempMap.SetAt(0);
					if (cell == rootCell)
					{
						cell = (byte)tempMap.SetAt(1);
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

		private unsafe void GetOnToOff(
			ChainNode* pOn, Table outTable, IReadOnlyGrid grid, GridMap[] maps, bool isYEnabled)
		{
			var (rootCell, rootDigit) = *pOn;
			if (isYEnabled)
			{
				short mask = (short)(grid.GetCandidateMask(rootCell) & ~(1 << rootDigit));
				if (mask != 0)
				{
					foreach (byte digit in mask.GetAllSets())
					{
						outTable.Add(new ChainNode(rootCell, digit, false, pOn));
					}
				}
			}

			var tempMap = maps[rootDigit] & PeerMaps[rootCell];
			if (tempMap.IsNotEmpty)
			{
				foreach (byte cell in tempMap)
				{
					outTable.Add(new ChainNode(cell, rootDigit, false, pOn));
				}
			}
		}

		private unsafe void GatherHint(ChainNode endPoint, GridMap[] maps, bool isContinuous, IReadOnlyGrid grid)
		{
			var nodes = new List<Node>();

			var forwardPtr = &endPoint;
			for (var ptr = forwardPtr; ptr->PredecessorsCount != 0; ptr = ptr->Predecessors[0])
			{
				nodes.Add(new Node(ptr->Cell, ptr->Digit));
			}


		}

		private unsafe bool CacheDoesNotContainLoop(ChainNode* endPoint)
		{
			var tempMap = new GridMap { endPoint->Cell };
			for (var p = endPoint->Predecessors[0]; p->PredecessorsCount != 0; p = p->Predecessors[0])
			{
				tempMap.Add(p->Cell);
			}

			if (tempMap.AllSetsAreInOneRegion(out _))
			{
				return false;
			}
			for (int i = 0; i < _nlcnt; i++)
			{
				if (_nl[i] == tempMap)
				{
					return false;
				}
			}

			if (_nlcnt < _nl.Length)
			{
				_nl[++_nlcnt] = tempMap;
			}
			else
			{
				throw new SudokuRuntimeException("The number of found loops is greater than the maximum value.");
			}

			return true;
		}

		private unsafe bool CacheDoesNotContainAic(ChainNode* endPoint)
		{
			var tempMap = new GridMap { endPoint->Cell };
			for (var p = endPoint->Predecessors[0]; p->PredecessorsCount != 0; p = p->Predecessors[0])
			{
				tempMap.Add(p->Cell);
			}

			if (tempMap.AllSetsAreInOneRegion(out _))
			{
				return false;
			}
			for (int i = 0; i < _aiccnt; i++)
			{
				if (_aic[i] == tempMap)
				{
					return false;
				}
			}

			if (_aiccnt < _aic.Length)
			{
				_aic[++_aiccnt] = tempMap;
			}
			else
			{
				throw new SudokuRuntimeException("The number of found loops is greater than the maximum value.");
			}

			return true;
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
