using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Solving.Extensions;
using Sudoku.Solving.Utils;
using static Sudoku.Solving.Utils.RegionUtils;
using GaicInfo = Sudoku.Solving.Manual.Chaining.GroupedAlternatingInferenceChainTechniqueInfo;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Encapsulates a/an (grouped) alternating inference chain (AIC) technique searcher.
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
	/// </remarks>
	[Slow(SlowButNecessary = true)]
	public sealed class GroupedAlternatingInferenceChainTechniqueSearcher : ChainTechniqueSearcher
	{
		/// <summary>
		/// The empty array.
		/// </summary>
		private static readonly int[] EmptyArray = Array.Empty<int>();

		/// <summary>
		/// The table of blocks.
		/// </summary>
		private static readonly int[][] BlockTable = new int[27][]
		{
			EmptyArray, EmptyArray, EmptyArray,
			EmptyArray, EmptyArray, EmptyArray,
			EmptyArray, EmptyArray, EmptyArray,
			new[] { 0, 1, 2 }, new[] { 0, 1, 2 }, new[] { 0, 1, 2 },
			new[] { 3, 4, 5 }, new[] { 3, 4, 5 }, new[] { 3, 4, 5 },
			new[] { 6, 7, 8 }, new[] { 6, 7, 8 }, new[] { 6, 7, 8 },
			new[] { 0, 3, 6 }, new[] { 0, 3, 6 }, new[] { 0, 3, 6 },
			new[] { 1, 4, 7 }, new[] { 1, 4, 7 }, new[] { 1, 4, 7 },
			new[] { 2, 5, 8 }, new[] { 2, 5, 8 }, new[] { 2, 5, 8 }
		};

		/// <summary>
		/// Indicates the last index of the collection.
		/// </summary>
		private static readonly Index LastIndex = ^1;


		/// <summary>
		/// Indicates whether the searcher will search for X-Chains.
		/// </summary>
		private readonly bool _searchX;

		/// <summary>
		/// Indicates whether the searcher will search for Y-Chains.
		/// Here Y-Chains means for multi-digit AICs.
		/// </summary>
		private readonly bool _searchY;

		/// <summary>
		/// Indicates whether the searcher will check locked candidates nodes.
		/// </summary>
		private readonly bool _searchLcNodes;

		/// <summary>
		/// Indicates whether the searcher will reduct same AICs
		/// which has same head and tail nodes.
		/// </summary>
		private readonly bool _reductDifferentPathAic;

		/// <summary>
		/// Indicates whether the searcher will store the shortest path only.
		/// </summary>
		private readonly bool _onlySaveShortestPathAic;

		/// <summary>
		/// Indicates whether the searcher will store the discontinuous nice loop
		/// whose head and tail is a same node.
		/// </summary>
		private readonly bool _checkHeadCollision;

		/// <summary>
		/// Indicates whether the searcher will check the chain forms a continuous
		/// nice loop.
		/// </summary>
		private readonly bool _checkContinuousNiceLoop;

		/// <summary>
		/// Indicates the maximum length to search.
		/// </summary>
		private readonly int _maxLength;

		/// <summary>
		/// Indicates all region maps.
		/// </summary>
		private readonly GridMap[] _regionMaps;


		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="searchX">Indicates searching X-Chains or not.</param>
		/// <param name="searchY">Indicates searching Y-Chains or not.</param>
		/// <param name="searchLockedCandidatesNodes">
		/// Indicates whether the searcher will check the locked candidates nodes.
		/// </param>
		/// <param name="maxLength">
		/// Indicates the maximum length of a chain to search.
		/// </param>
		/// <param name="reductDifferentPathAic">
		/// Indicates whether the searcher will reduct same AICs
		/// which has same head and tail nodes.
		/// </param>
		/// <param name="onlySaveShortestPathAic">
		/// Indicates whether the searcher will store the shortest path
		/// only.
		/// </param>
		/// <param name="checkHeadCollision">
		/// Indicates whether the searcher will store the discontinuous nice loop
		/// whose head and tail is a same node.
		/// </param>
		/// <param name="checkContinuousNiceLoop">
		/// Indicates whether the searcher will check the chain forms a continuous
		/// nice loop.
		/// </param>
		/// <param name="regionMaps">All region maps.</param>
		public GroupedAlternatingInferenceChainTechniqueSearcher(
			bool searchX, bool searchY, bool searchLockedCandidatesNodes,
			int maxLength, bool reductDifferentPathAic, bool onlySaveShortestPathAic,
			bool checkHeadCollision, bool checkContinuousNiceLoop,
			GridMap[] regionMaps)
		{
			_searchX = searchX;
			_searchY = searchY;
			_searchLcNodes = searchLockedCandidatesNodes;
			_maxLength = maxLength;
			_reductDifferentPathAic = reductDifferentPathAic;
			_onlySaveShortestPathAic = onlySaveShortestPathAic;
			_checkHeadCollision = checkHeadCollision;
			_checkContinuousNiceLoop = checkContinuousNiceLoop;
			_regionMaps = regionMaps;
		}


		/// <inheritdoc/>
		public override int Priority { get; set; } = 45;


		/// <inheritdoc/>
		public override void AccumulateAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			(_, _, var digitDistributions) = grid;

			var candidatesUsed = FullGridMap.Empty;
			var stack = new List<Node>();
			var strongInferences = GetAllStrongInferences(grid, digitDistributions);

			// Iterate on each strong relation, and search for weak relations.
			foreach (var (start, end) in strongInferences)
			{
				// Iterate on two cases.
				foreach (var (startNode, endNode) in new[] { (start, end), (end, start) })
				{
					// Add the start and end node to the used list.
					AddNode(start, ref candidatesUsed);
					AddNode(end, ref candidatesUsed);
					stack.Add(startNode);
					stack.Add(endNode);

					// Get 'on' to 'off' nodes and 'off' to 'on' nodes recursively.
					GetOnToOffRecursively(
						accumulator, grid, candidatesUsed, endNode, strongInferences,
						digitDistributions, stack, _maxLength - 2);

					// Undo the step to recover the candidate status.
					RemoveNode(start, ref candidatesUsed);
					RemoveNode(end, ref candidatesUsed);
					stack.RemoveLastElement();
					stack.RemoveLastElement();
				}
			}
		}

		/// <summary>
		/// Get 'on' nodes to 'off' nodes recursively (Searching for weak links).
		/// </summary>
		/// <param name="accumulator">The accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="candidatesUsed">The candidate used.</param>
		/// <param name="currentNode">The current node.</param>
		/// <param name="strongInferences">The strong inferences.</param>
		/// <param name="digitDistributions">All digits' distributions.</param>
		/// <param name="stack">The stack.</param>
		/// <param name="length">The last length to search.</param>
		private void GetOnToOffRecursively(
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, FullGridMap candidatesUsed,
			Node currentNode, IReadOnlyList<Inference> strongInferences,
			GridMap[] digitDistributions, IList<Node> stack, int length)
		{
			if (length < 0)
			{
				return;
			}

			switch (currentNode.NodeType)
			{
				case NodeType.Candidate:
				{
					int currentCandidate = currentNode[0];
					int currentCell = currentCandidate / 9, currentDigit = currentCandidate % 9;

					// Search for same regions.
					foreach (int nextCell in new GridMap(currentCell, false).Offsets)
					{
						if (!grid.CandidateExists(nextCell, currentDigit))
						{
							continue;
						}

						int nextCandidate = nextCell * 9 + currentDigit;
						if (candidatesUsed[nextCandidate])
						{
							continue;
						}

						candidatesUsed[nextCandidate] = true;
						var nextNode = new Node(nextCandidate, NodeType.Candidate);
						stack.Add(nextNode);

						GetOffToOnRecursively(
							accumulator, grid, candidatesUsed, nextNode, strongInferences,
							digitDistributions, stack, length - 1);

						candidatesUsed[nextCandidate] = false;
						stack.RemoveLastElement();
					}

					// Search for the cells.
					if (_searchY || _searchLcNodes)
					{
						foreach (int nextDigit in grid.GetCandidatesReversal(currentCell).GetAllSets())
						{
							if (nextDigit == currentDigit)
							{
								continue;
							}

							int nextCandidate = currentCell * 9 + nextDigit;
							if (candidatesUsed[nextCandidate])
							{
								continue;
							}

							candidatesUsed[nextCandidate] = true;
							var nextNode = new Node(nextCandidate, NodeType.Candidate);
							stack.Add(nextNode);

							GetOffToOnRecursively(
								accumulator, grid, candidatesUsed, nextNode, strongInferences,
								digitDistributions, stack, length - 1);

							candidatesUsed[nextCandidate] = false;
							stack.RemoveLastElement();
						}
					}

					if (_searchLcNodes)
					{
						// Check locked candidate nodes.
						foreach (var nextNode in GetLcNodes(digitDistributions, currentCell, currentDigit))
						{
							if (nextNode.FullCovered(currentNode))
							{
								continue;
							}

							candidatesUsed.AddRange(nextNode.Candidates);
							stack.Add(nextNode);

							GetOffToOnRecursively(
								accumulator, grid, candidatesUsed, nextNode, strongInferences,
								digitDistributions, stack, length - 1);

							stack.RemoveLastElement();
							candidatesUsed.RemoveRange(nextNode.Candidates);
						}
					}

					break;
				}
				case NodeType.LockedCandidates:
				{
					int currentDigit = currentNode[0] % 9;
					var currentCells = from cand in currentNode.Candidates
									   where cand % 9 == currentDigit
									   select cand / 9;

					// Search for same regions.
					foreach (int nextCell in GridMap.CreateInstance(currentCells, false).Offsets)
					{
						if (!grid.CandidateExists(nextCell, currentDigit))
						{
							continue;
						}

						int nextCandidate = nextCell * 9 + currentDigit;
						if (candidatesUsed[nextCandidate])
						{
							continue;
						}

						candidatesUsed[nextCandidate] = true;
						var nextNode = new Node(nextCandidate, NodeType.Candidate);
						stack.Add(nextNode);

						GetOffToOnRecursively(
							accumulator, grid, candidatesUsed, nextNode, strongInferences,
							digitDistributions, stack, length - 1);

						candidatesUsed[nextCandidate] = false;
						stack.RemoveLastElement();
					}

					// In a grouped AIC, a locked candidate node cannot link with a
					// candidate node with different value.
					// In contrast, this one is an advanced link (such as using
					// an almost UR, or an ALS structure).
					// Search for the cells.
					//if (_searchY || _searchLcNodes)
					//{
					//	// Do nothing.
					//}

					if (_searchLcNodes)
					{
						// Check locked candidate nodes.
						foreach (var nextNode in GetLcNodes(digitDistributions, currentCells, currentDigit))
						{
							if ((nextNode.CandidatesMap | candidatesUsed) == candidatesUsed)
							{
								// The current node is fully covered by 'candidatesUsed'.
								continue;
							}

							candidatesUsed.AddRange(nextNode.Candidates);
							stack.Add(nextNode);

							GetOffToOnRecursively(
								accumulator, grid, candidatesUsed, nextNode, strongInferences,
								digitDistributions, stack, length - 1);

							stack.RemoveLastElement();
							candidatesUsed.RemoveRange(nextNode.Candidates);
						}
					}

					break;
				}
			}
		}

		/// <summary>
		/// Get 'off' nodes to 'on' nodes recursively (Searching for strong links).
		/// </summary>
		/// <param name="accumulator">The accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="candidatesUsed">The candidates used.</param>
		/// <param name="currentNode">The current node.</param>
		/// <param name="strongInferences">All strong inferences.</param>
		/// <param name="digitDistributions">All digits' distributions.</param>
		/// <param name="stack">The stack.</param>
		/// <param name="length">The last length to search.</param>
		private void GetOffToOnRecursively(
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, FullGridMap candidatesUsed,
			Node currentNode, IReadOnlyList<Inference> strongInferences,
			GridMap[] digitDistributions, IList<Node> stack, int length)
		{
			if (length < 0)
			{
				return;
			}

			switch (currentNode.NodeType)
			{
				case NodeType.Candidate:
				{
					int currentCandidate = currentNode[0];
					int currentCell = currentCandidate / 9, currentDigit = currentCandidate % 9;

					// Search for same regions.
					var (r, c, b) = CellUtils.GetRegion(currentCell);
					foreach (int region in stackalloc[] { r + 9, c + 18, b })
					{
						var map = grid.GetDigitAppearingCells(currentDigit, region);
						if (map.Count != 2)
						{
							continue;
						}

						map[currentCell] = false;
						int nextCell = map.SetAt(0);
						int nextCandidate = nextCell * 9 + currentDigit;
						if (candidatesUsed[nextCandidate])
						{
							continue;
						}

						candidatesUsed[nextCandidate] = true;
						var nextNode = new Node(nextCandidate, NodeType.Candidate);
						stack.Add(nextNode);

						// Now check elimination.
						// If the elimination exists, the chain will be added to the accumulator.
						CheckElimination(accumulator, grid, candidatesUsed, stack);

						GetOnToOffRecursively(
							accumulator, grid, candidatesUsed, nextNode, strongInferences,
							digitDistributions, stack, length - 1);

						candidatesUsed[nextCandidate] = false;
						stack.RemoveLastElement();
					}

					// Search for cell.
					if (_searchY || _searchLcNodes)
					{
						if (grid.IsBivalueCell(currentCell, out short mask))
						{
							mask &= (short)~(1 << currentDigit);
							int nextDigit = mask.FindFirstSet();
							int nextCandidate = currentCell * 9 + nextDigit;
							if (candidatesUsed[nextCandidate])
							{
								return;
							}

							candidatesUsed[nextCandidate] = true;
							var nextNode = new Node(nextCandidate, NodeType.Candidate);
							stack.Add(nextNode);

							// Now check elimination.
							// If the elimination exists, the chain will be added to the accumulator.
							CheckElimination(accumulator, grid, candidatesUsed, stack);

							GetOnToOffRecursively(
								accumulator, grid, candidatesUsed, nextNode, strongInferences,
								digitDistributions, stack, length - 1);

							candidatesUsed[nextCandidate] = false;
							stack.RemoveLastElement();
						}
					}

					if (_searchLcNodes)
					{
						foreach (var nextNode in GetLcNodes(digitDistributions, currentCell, currentDigit))
						{
							var tempMap = (nextNode.CandidatesMap | currentNode.CandidatesMap)
								.Reduct(currentDigit);
							if ((tempMap - digitDistributions[currentDigit]).IsNotEmpty
									|| nextNode.FullCovered(currentNode))
							{
								continue;
							}

							foreach (int coveredRegion in tempMap.CoveredRegions)
							{
								// Strong inference found.
								candidatesUsed.AddRange(nextNode.Candidates);
								stack.Add(nextNode);

								// Now check elimination.
								// If the elimination exists, the chain will be added to the accumulator.
								CheckElimination(accumulator, grid, candidatesUsed, stack);

								GetOnToOffRecursively(
									accumulator, grid, candidatesUsed, nextNode, strongInferences,
									digitDistributions, stack, length - 1);

								candidatesUsed.RemoveRange(nextNode.Candidates);
								stack.RemoveLastElement();
							}
						}
					}

					break;
				}
				case NodeType.LockedCandidates:
				{
					int currentDigit = currentNode[0] % 9;
					var currentCells = from cand in currentNode.Candidates
									   where cand % 9 == currentDigit
									   select cand / 9;

					// Search for same regions.
					foreach (int region in GridMap.CreateInstance(currentCells, false).CoveredRegions)
					{
						var map = grid.GetDigitAppearingCells(currentDigit, region);
						if (map.Count != 1)
						{
							continue;
						}

						int nextCell = map.SetAt(0);
						int nextCandidate = nextCell * 9 + currentDigit;
						if (candidatesUsed[nextCandidate])
						{
							continue;
						}

						candidatesUsed[nextCandidate] = true;
						var nextNode = new Node(nextCandidate, NodeType.Candidate);
						stack.Add(nextNode);

						// Now check elimination.
						// If the elimination exists, the chain will be added to the accumulator.
						CheckElimination(accumulator, grid, candidatesUsed, stack);

						GetOnToOffRecursively(
							accumulator, grid, candidatesUsed, nextNode, strongInferences,
							digitDistributions, stack, length - 1);

						candidatesUsed[nextCandidate] = false;
						stack.RemoveLastElement();
					}

					// In a grouped AIC, a locked candidate node cannot link with a
					// candidate node with different value.
					// In contrast, this one is an advanced link (such as using
					// an almost UR, or an ALS structure).
					// Search for the cells.
					//if (_searchY || _searchLcNodes)
					//{
					//	// Do nothing.
					//}

					if (_searchLcNodes)
					{
						foreach (var nextNode in GetLcNodes(digitDistributions, currentCells, currentDigit))
						{
							var tempMap = (nextNode.CandidatesMap | currentNode.CandidatesMap)
								.Reduct(currentDigit);
							if ((tempMap - digitDistributions[currentDigit]).IsNotEmpty
								|| nextNode == currentNode)
							{
								continue;
							}

							foreach (int coveredRegion in tempMap.CoveredRegions)
							{
								// Strong inference found.
								candidatesUsed.AddRange(nextNode.Candidates);
								stack.Add(nextNode);

								// Now check elimination.
								// If the elimination exists, the chain will be added to the accumulator.
								CheckElimination(accumulator, grid, candidatesUsed, stack);

								GetOnToOffRecursively(
									accumulator, grid, candidatesUsed, nextNode, strongInferences,
									digitDistributions, stack, length - 1);

								candidatesUsed.RemoveRange(nextNode.Candidates);
								stack.RemoveLastElement();
							}
						}
					}

					break;
				}
			}
		}

		/// <summary>
		/// Get all locked candidates nodes at current cell and digit.
		/// </summary>
		/// <param name="digitDistributions">The digit distributions.</param>
		/// <param name="currentCell">The current cell.</param>
		/// <param name="currentDigit">The current digit.</param>
		/// <returns>All locked candidates nodes.</returns>
		private IEnumerable<Node> GetLcNodes(
			GridMap[] digitDistributions, int currentCell, int currentDigit)
		{
			var result = new List<Node>();
			var (r, c, b) = CellUtils.GetRegion(currentCell);
			foreach (int nonblock in stackalloc[] { r + 9, c + 18 })
			{
				var tempMap = _regionMaps[nonblock] & digitDistributions[currentDigit];
				if (tempMap.IsEmpty)
				{
					// No candidates left in this region.
					continue;
				}

				var anotherMap = tempMap & _regionMaps[b];
				if (anotherMap.IsEmpty || anotherMap.Count < 2)
				{
					// No candidates in this region
					// or the node is not a locked candidates node.
					continue;
				}

				// Group node found.
				result.Add(
					new Node(
						from cell in anotherMap.Offsets select cell * 9 + currentDigit,
						NodeType.LockedCandidates));
			}

			return result;
		}

		private IEnumerable<Node> GetLcNodes(
			GridMap[] digitDistributions, IEnumerable<int> currentCells, int currentDigit)
		{
			var result = new List<Node>();
			foreach (int nonblock in from region in new GridMap(currentCells).CoveredRegions
									 where region >= 9
									 select region)
			{
				var tempMap = _regionMaps[nonblock] & digitDistributions[currentDigit];
				if (tempMap.IsEmpty)
				{
					// No candidates left in this region.
					continue;
				}

				foreach (int block in BlockTable[nonblock])
				{
					var anotherMap = tempMap & _regionMaps[block];
					if (anotherMap.IsEmpty || anotherMap.Count < 2)
					{
						// No candidates in this region
						// or the node is not a locked candidates node.
						continue;
					}

					// Group node found.
					result.Add(
						new Node(
							from cell in anotherMap.Offsets select cell * 9 + currentDigit,
							NodeType.LockedCandidates));
				}
			}

			return result;
		}

		/// <summary>
		/// Check the elimination, and save the chain into the accumulator
		/// when the chain is valid and worth.
		/// </summary>
		/// <param name="accumulator">The accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="candidateList">The candidate list.</param>
		/// <param name="stack">The stack.</param>
		private void CheckElimination(
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, FullGridMap candidateList,
			IList<Node> stack)
		{
			if (_checkContinuousNiceLoop && IsContinuousNiceLoop(stack))
			{
				// The structure is a continuous nice loop!
				// Now we should get all weak inferences to search all eliminations.
				// Step 1: save all weak inferences.
				var weakInferences = new List<Inference>();
				for (int i = 1; i < stack.Count - 1; i += 2)
				{
					weakInferences.Add(new Inference(stack[i], true, stack[i + 1], false));
				}

				// Step 2: Check elimination sets.
				var eliminationSets = new List<FullGridMap>(
					from weakInference in weakInferences
					select weakInference.Intersection);
				if (eliminationSets.Count == 0)
				{
					return;
				}

				// Step 3: Record eliminations if exists.
				var conclusions = new List<Conclusion>();
				foreach (var eliminationSet in eliminationSets)
				{
					foreach (int candidate in eliminationSet.Offsets)
					{
						if (grid.CandidateExists(candidate / 9, candidate % 9))
						{
							conclusions.Add(new Conclusion(ConclusionType.Elimination, candidate));
						}
					}
				}
				if (conclusions.Count == 0)
				{
					return;
				}

				// Step 4: Get all highlight candidates.
				var candidateOffsets = new List<(int, int)>();
				var links = new List<Inference>();
				var nodes = new List<Node>();
				int index = 0;
				var last = (Node?)default;
				foreach (var node in stack)
				{
					int isOn = index & 1, isOff = (index + 1) & 1;
					nodes.Add(node);
					switch (node.NodeType)
					{
						case NodeType.Candidate:
						{
							candidateOffsets.Add((isOff, node[0]));

							break;
						}
						case NodeType.LockedCandidates:
						{
							candidateOffsets.AddRange(
								from candidate in node.Candidates
								select (isOff, candidate));

							break;
						}
					}

					if (index > 0)
					{
						links.Add(new Inference(last!, isOn == 0, node, isOff == 0));
					}

					last = node;
					index++;
				}

				// Continuous nice loop should be a loop.
				links.Add(new Inference(stack[LastIndex], true, stack[0], false));

				SumUpResult(
					accumulator,
					new GaicInfo(
						conclusions,
						views: new[]
						{
							new View(
								cellOffsets: null,
								candidateOffsets,
								regionOffsets: null,
								links)
						},
						nodes,
						isContinuousNiceLoop: true));
			}
			else
			{
				// Is a normal chain.
				// Step 1: Check eliminations.
				var startNode = stack[0];
				var endNode = stack[LastIndex];
				if (_checkHeadCollision && startNode == endNode)
				{
					return;
				}

				var elimMap = new Inference(startNode, false, endNode, true).Intersection;
				if (elimMap.IsEmpty)
				{
					return;
				}

				var conclusions = new List<Conclusion>();
				foreach (int candidate in elimMap.Offsets)
				{
					if (grid.CandidateExists(candidate / 9, candidate % 9))
					{
						conclusions.Add(new Conclusion(ConclusionType.Elimination, candidate));
					}
				}

				if (conclusions.Count == 0)
				{
					return;
				}

				// Step 2: if the chain is worth, we will construct a node list.
				// Record all highlight candidates.
				var lastCand = (Node?)default;
				var candidateOffsets = new List<(int, int)>();
				var nodes = new List<Node>();
				var links = new List<Inference>();
				bool @switch = false;
				int index = 0;
				foreach (var node in stack)
				{
					nodes.Add(node);
					switch (node.NodeType)
					{
						case NodeType.Candidate:
						{
							candidateOffsets.Add((@switch ? 1 : 0, node[0]));

							break;
						}
						case NodeType.LockedCandidates:
						{
							candidateOffsets.AddRange(
								from candidate in node.Candidates
								select (@switch ? 1 : 0, candidate));

							break;
						}
					}

					// To ensure this loop has the predecessor.
					if (index++ > 0)
					{
						links.Add(new Inference(lastCand!, !@switch, node, @switch));
					}

					lastCand = node;
					@switch = !@switch;
				}

				// Step 3: Record it into the result accumulator.
				SumUpResult(
					accumulator,
					new GaicInfo(
						conclusions,
						views: new[]
						{
							new View(
								cellOffsets: null,
								candidateOffsets,
								regionOffsets: null,
								links)
						},
						nodes,
						isContinuousNiceLoop: false));
			}
		}

		/// <summary>
		/// Sum up the result.
		/// </summary>
		/// <param name="accumulator">The accumulator list.</param>
		/// <param name="resultInfo">The result information instance.</param>
		private void SumUpResult(
			IBag<TechniqueInfo> accumulator, GaicInfo resultInfo)
		{
			if (_onlySaveShortestPathAic)
			{
				bool hasSameAic = false;
				int sameAicIndex = default;
				for (int i = 0; i < accumulator.Count; i++)
				{
					if (accumulator[i] is GaicInfo comparer && comparer == resultInfo)
					{
						hasSameAic = true;
						sameAicIndex = i;
					}
				}
				if (hasSameAic)
				{
					var list = new List<TechniqueInfo>(accumulator) { [sameAicIndex] = resultInfo };
					accumulator.Clear();
					accumulator.AddRange(list);
				}
				else
				{
					GetAct(accumulator)(resultInfo);
				}
			}
			else
			{
				GetAct(accumulator)(resultInfo);
			}
		}

		/// <summary>
		/// To check whether the nodes form a loop.
		/// </summary>
		/// <param name="stack">The stack.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		/// <remarks>
		/// If the nodes form a continuous nice loop, the head and tail node should be
		/// in a same region, and hold a same digit; or else the head and the tail is in
		/// a same cell, but the digits are different; otherwise, the nodes forms a normal
		/// AIC.
		/// </remarks>
		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool IsContinuousNiceLoop(IList<Node> stack)
		{
			var startNode = stack[0];
			var endNode = stack[LastIndex];
			switch ((startNode.NodeType, endNode.NodeType))
			{
				case (NodeType.Candidate, _):
				case (NodeType.LockedCandidates, _):
				{
					int head = startNode[0], tail = endNode[0];
					int headCell = head / 9, headDigit = head % 9;
					int tailCell = tail / 9, tailDigit = tail % 9;
					if (headCell == tailCell)
					{
						return headDigit != tailDigit;
					}

					// If the cell are not same, we will check the cells are in a same region
					// and the digits are same.
					return headDigit == tailDigit
						&& new GridMap(new[] { headCell, tailCell }).AllSetsAreInOneRegion(out _);
				}
				default:
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Bound with <see cref="CheckElimination(IBag{TechniqueInfo}, IReadOnlyGrid, FullGridMap, IList{Node})"/>.
		/// </summary>
		/// <param name="accumulator">The accumulator.</param>
		/// <returns>The result action.</returns>
		/// <seealso cref="CheckElimination(IBag{TechniqueInfo}, IReadOnlyGrid, FullGridMap, IList{Node})"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Action<GaicInfo> GetAct(IBag<TechniqueInfo> accumulator)
		{
			// Here may use conditional operator '?:' to decide the result.
			// However, this operator cannot tell with the type of the result
			// due to the delegate type (return a method call rather than a normal object),
			// so you should add the type name explicitly at the either a branch,
			// but the code is so ugly...
			return _reductDifferentPathAic switch
			{
				true => accumulator.Add,
				false => accumulator.AddIfDoesNotContain
			};
		}

		/// <summary>
		/// Get all strong relations.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="digitDistributions">All digits' distributions.</param>
		/// <returns>All strong relations.</returns>
		private IReadOnlyList<Inference> GetAllStrongInferences(
			IReadOnlyGrid grid, GridMap[] digitDistributions)
		{
			var result = new List<Inference>();

			// Search for each region to get all strong inferences (basic strong inference).
			for (int region = 0; region < 27; region++)
			{
				for (int digit = 0; digit < 9; digit++)
				{
					if (!grid.IsBilocationRegion(digit, region, out short mask))
					{
						continue;
					}

					int pos1 = mask.FindFirstSet();
					result.Add(
						new Inference(
							new Node(
								GetCellOffset(region, pos1) * 9 + digit,
								NodeType.Candidate),
							false,
							new Node(
								GetCellOffset(
									region, mask.GetNextSetBit(pos1)) * 9 + digit,
								NodeType.Candidate),
							true));
				}
			}

			if (_searchY || _searchLcNodes)
			{
				// Search for each bivalue cells.
				for (int cell = 0; cell < 81; cell++)
				{
					if (!grid.IsBivalueCell(cell, out short mask))
					{
						continue;
					}

					int digit1 = mask.FindFirstSet();
					result.Add(
						new Inference(
							new Node(cell * 9 + digit1, NodeType.Candidate),
							false,
							new Node(cell * 9 + mask.GetNextSetBit(digit1), NodeType.Candidate),
							true));
				}
			}

			if (_searchLcNodes)
			{
				// Search for each digit and each regions.
				for (int digit = 0; digit < 9; digit++)
				{
					for (int region = 9; region < 27; region++)
					{
						var map = digitDistributions[digit] & _regionMaps[region];
						int[] coveredBlocks = (
							from coveredRegion in map.CoveredRegions
							where coveredRegion < 9
							select coveredRegion).ToArray();
						if (coveredBlocks.Length != 2)
						{
							continue;
						}

						var startCandidates = (map & _regionMaps[coveredBlocks[0]]).Offsets;
						var endCandidates = (map & _regionMaps[coveredBlocks[1]]).Offsets;
						result.Add(
							new Inference(
								new Node(
									startCandidates, startCandidates.HasOnlyOneElement()
										? NodeType.Candidate
										: NodeType.LockedCandidates),
								false,
								new Node(
									endCandidates, endCandidates.HasOnlyOneElement()
										? NodeType.Candidate
										: NodeType.LockedCandidates),
								true));
					}
				}
			}

			return result;
		}


		/// <summary>
		/// Add the node to the map.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <param name="map">The map.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void AddNode(Node node, ref FullGridMap map)
		{
			switch (node.NodeType)
			{
				case NodeType.Candidate:
				{
					map[node[0]] = true;
					break;
				}
				case NodeType.LockedCandidates:
				{
					foreach (int cand in node.Candidates)
					{
						map[cand] = true;
					}

					break;
				}
			}
		}

		/// <summary>
		/// Remove the node from the map.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <param name="map">The map.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void RemoveNode(Node node, ref FullGridMap map)
		{
			switch (node.NodeType)
			{
				case NodeType.Candidate:
				{
					map[node[0]] = false;
					break;
				}
				case NodeType.LockedCandidates:
				{
					foreach (int cand in node.Candidates)
					{
						map[cand] = false;
					}

					break;
				}
			}
		}
	}
}
