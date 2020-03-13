using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Encapsulates an alternating inference chain (AIC) technique searcher.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This technique searcher may use the basic searching way to find all AICs.
	/// For example, this searcher will try to search for all strong inferences firstly,
	/// and then search a weak inference that the candidate is in the same region or cell
	/// with a node in the strong inference in order to link them.
	/// </para>
	/// <para>
	/// Note that AIC may be static chains, which means that the searcher may just use
	/// static analysis is fine, which is different with dynamic chains.
	/// </para>
	/// </remarks>
	[Slow(SlowButNecessary = true)]
	public sealed class AlternatingInferenceChainTechniqueSearcher : ChainTechniqueSearcher
	{
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
		private readonly bool _addHeadCollision;

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
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="searchX">Indicates searching X-Chains or not.</param>
		/// <param name="searchY">Indicates searching Y-Chains or not.</param>
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
		/// <param name="addHeadCollision">
		/// Indicates whether the searcher will store the discontinuous nice loop
		/// whose head and tail is a same node.
		/// </param>
		/// <param name="checkContinuousNiceLoop">
		/// Indicates whether the searcher will check the chain forms a continuous
		/// nice loop.
		/// </param>
		public AlternatingInferenceChainTechniqueSearcher(
			bool searchX, bool searchY, int maxLength, bool reductDifferentPathAic,
			bool onlySaveShortestPathAic, bool addHeadCollision, bool checkContinuousNiceLoop)
		{
			_searchX = searchX;
			_searchY = searchY;
			_maxLength = maxLength;
			_reductDifferentPathAic = reductDifferentPathAic;
			_onlySaveShortestPathAic = onlySaveShortestPathAic;
			_addHeadCollision = addHeadCollision;
			_checkContinuousNiceLoop = checkContinuousNiceLoop;
		}


		/// <inheritdoc/>
		public override int Priority { get; set; } = 45;


		/// <inheritdoc/>
		public override void AccumulateAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			// Iterate on each strong relation, and search for weak relations.
			var candidateList = FullGridMap.Empty;
			var stack = new List<int>();
			var strongInferences = GetAllStrongInferences(grid);
			foreach (var (start, end) in strongInferences)
			{
				foreach (var (startCandidate, endCandidate) in stackalloc[] { (start, end), (end, start) })
				{
					int endCell = endCandidate / 9;
					int endDigit = endCandidate % 9;
					candidateList[startCandidate] = true;
					candidateList[endCandidate] = true;
					stack.Add(startCandidate);
					stack.Add(endCandidate);

					// Get 'on' to 'off' nodes and 'off' to 'on' nodes recursively.
					GetOnToOffRecursively(
						accumulator, grid, candidateList, endCell, endDigit,
						strongInferences, stack, _maxLength - 2);

					// Undo the step to recover the candidate status.
					candidateList[startCandidate] = false;
					candidateList[endCandidate] = false;
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
		/// <param name="candidateList">The candidate list.</param>
		/// <param name="currentCell">The current cell.</param>
		/// <param name="currentDigit">The current digit.</param>
		/// <param name="strongRelations">The strong relations.</param>
		/// <param name="stack">The stack.</param>
		/// <param name="length">The last length to search.</param>
		private void GetOnToOffRecursively(
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, FullGridMap candidateList,
			int currentCell, int currentDigit, IReadOnlyList<(int, int)> strongRelations,
			IList<int> stack, int length)
		{
			if (length < 0)
			{
				return;
			}

			// Search for same regions.
			if (_searchX)
			{
				foreach (int nextCell in new GridMap(currentCell, false).Offsets)
				{
					if (!grid.CandidateExists(nextCell, currentDigit))
					{
						continue;
					}

					int nextCandidate = nextCell * 9 + currentDigit;
					if (candidateList[nextCandidate])
					{
						continue;
					}

					candidateList[nextCandidate] = true;
					stack.Add(nextCandidate);

					GetOffToOnRecursively(
						accumulator, grid, candidateList, nextCell, currentDigit,
						strongRelations, stack, length - 1);

					candidateList[nextCandidate] = false;
					stack.RemoveLastElement();
				}
			}

			// Search for the cells.
			if (_searchY)
			{
				foreach (int nextDigit in grid.GetCandidatesReversal(currentCell).GetAllSets())
				{
					if (nextDigit == currentDigit)
					{
						continue;
					}

					int nextCandidate = currentCell * 9 + nextDigit;
					if (candidateList[nextCandidate])
					{
						continue;
					}

					candidateList[nextCandidate] = true;
					stack.Add(nextCandidate);

					GetOffToOnRecursively(
						accumulator, grid, candidateList, currentCell, nextDigit,
						strongRelations, stack, length - 1);

					candidateList[nextCandidate] = false;
					stack.RemoveLastElement();
				}
			}
		}

		/// <summary>
		/// Get 'off' nodes to 'on' nodes recursively (Searching for strong links).
		/// </summary>
		/// <param name="accumulator">The accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="candidateList">The candidate list.</param>
		/// <param name="currentCell">The current cell.</param>
		/// <param name="currentDigit">The current digit.</param>
		/// <param name="strongRelations">All strong relations.</param>
		/// <param name="stack">The stack.</param>
		/// <param name="length">The last length to search.</param>
		private void GetOffToOnRecursively(
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, FullGridMap candidateList,
			int currentCell, int currentDigit, IReadOnlyList<(int, int)> strongRelations,
			IList<int> stack, int length)
		{
			if (length < 0)
			{
				return;
			}

			// Search for same regions.
			bool checkCollision(int next) => !_addHeadCollision && candidateList[next];
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
				if (checkCollision(nextCandidate))
				{
					continue;
				}

				candidateList[nextCandidate] = true;
				stack.Add(nextCandidate);

				// Now check elimination.
				// If the elimination exists, the chain will be added to the accumulator.
				CheckElimination(accumulator, grid, candidateList, stack);

				GetOnToOffRecursively(
					accumulator, grid, candidateList, nextCell, currentDigit,
					strongRelations, stack, length - 1);

				candidateList[nextCandidate] = false;
				stack.RemoveLastElement();
			}

			// Search for cell.
			if (_searchY)
			{
				if (grid.IsBivalueCell(currentCell, out short mask))
				{
					mask &= (short)~(1 << currentDigit);
					int nextDigit = mask.FindFirstSet();
					int nextCandidate = currentCell * 9 + nextDigit;
					if (checkCollision(nextCandidate))
					{
						return;
					}

					candidateList[nextCandidate] = true;
					stack.Add(nextCandidate);

					// Now check elimination.
					// If the elimination exists, the chain will be added to the accumulator.
					CheckElimination(accumulator, grid, candidateList, stack);

					GetOnToOffRecursively(
						accumulator, grid, candidateList, currentCell, nextDigit,
						strongRelations, stack, length - 1);

					candidateList[nextCandidate] = false;
					stack.RemoveLastElement();
				}
			}
		}

		/// <summary>
		/// Get all strong relations.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <returns>All strong relations.</returns>
		private IReadOnlyList<(int, int)> GetAllStrongInferences(IReadOnlyGrid grid)
		{
			var result = new List<(int, int)>();
			for (int region = 0; region < 27; region++)
			{
				for (int digit = 0; digit < 9; digit++)
				{
					if (!grid.IsBilocationRegion(digit, region, out short mask))
					{
						continue;
					}

					int pos1 = mask.FindFirstSet();
					result.Add((
						RegionUtils.GetCellOffset(region, pos1) * 9 + digit,
						RegionUtils.GetCellOffset(region, mask.GetNextSetBit(pos1)) * 9 + digit));
				}
			}

			if (_searchY)
			{
				for (int cell = 0; cell < 81; cell++)
				{
					if (!grid.IsBivalueCell(cell, out short mask))
					{
						continue;
					}

					int digit1 = mask.FindFirstSet();
					result.Add((cell * 9 + digit1, cell * 9 + mask.GetNextSetBit(digit1)));
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
			IList<int> stack)
		{
			if (_checkContinuousNiceLoop && IsContinuousNiceLoop(stack))
			{
				// The structure is a continuous nice loop!
				// Now we should get all weak inferences to search all eliminations.
				// Step 1: save all weak inferences.
				var weakInferences = new List<(int, int)>();
				for (int i = 1; i < stack.Count - 1; i += 2)
				{
					weakInferences.Add((stack[i], stack[i + 1]));
				}

				// Step 2: Check elimination sets.
				var eliminationSets = new List<FullGridMap>();
				foreach (var (start, end) in weakInferences)
				{
					eliminationSets.Add(FullGridMap.CreateInstance(new[] { start, end }));
				}
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
				var links = new List<ChainInference>();
				var nodes = new List<ChainNode>();
				int index = 0;
				int last = default;
				foreach (int node in stack)
				{
					int isOn = index & 1, isOff = (index + 1) & 1;
					candidateOffsets.Add((isOff, node));
					nodes.Add(new ChainNode(node, isOff == 0));

					if (index > 0)
					{
						links.Add(new ChainInference(last, isOn == 0, node, isOff == 0));
					}

					last = node;
					index++;
				}

				// Continuous nice loop should be a loop.
				links.Add(new ChainInference(stack[LastIndex], true, stack[0], false));

				SumUpResult(
					accumulator,
					new AlternatingInferenceChainTechniqueInfo(
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
				int startCandidate = stack[0], endCandidate = stack[LastIndex];
				var elimMap = FullGridMap.CreateInstance(new[] { startCandidate, endCandidate });
				if (!elimMap.IsNotEmpty)
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
				int lastCand = default;
				var candidateOffsets = new List<(int, int)>();
				var nodes = new List<ChainNode>();
				var links = new List<ChainInference>();
				bool @switch = false;
				int i = 0;
				foreach (int candidate in stack)
				{
					nodes.Add(new ChainNode(candidate, @switch));
					candidateOffsets.Add((@switch ? 1 : 0, candidate));

					// To ensure this loop has the predecessor.
					if (i++ > 0)
					{
						links.Add(new ChainInference(lastCand, !@switch, candidate, @switch));
					}

					lastCand = candidate;
					@switch = !@switch;
				}

				// Step 3: Record it into the result accumulator.
				SumUpResult(
					accumulator,
					new AlternatingInferenceChainTechniqueInfo(
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
			IBag<TechniqueInfo> accumulator, AlternatingInferenceChainTechniqueInfo resultInfo)
		{
			if (_onlySaveShortestPathAic)
			{
				bool hasSameAic = false;
				int sameAicIndex = default;
				for (int i = 0; i < accumulator.Count; i++)
				{
					if (accumulator[i] is AlternatingInferenceChainTechniqueInfo comparer
						&& comparer == resultInfo)
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
		private bool IsContinuousNiceLoop(IList<int> stack)
		{
			int head = stack[0], tail = stack[LastIndex];
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

		/// <summary>
		/// Bound with <see cref="CheckElimination(IBag{TechniqueInfo}, IReadOnlyGrid, FullGridMap, IList{int})"/>.
		/// </summary>
		/// <param name="accumulator">The accumulator.</param>
		/// <returns>The result action.</returns>
		/// <seealso cref="CheckElimination(IBag{TechniqueInfo}, IReadOnlyGrid, FullGridMap, IList{int})"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Action<AlternatingInferenceChainTechniqueInfo> GetAct(IBag<TechniqueInfo> accumulator)
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
	}
}
