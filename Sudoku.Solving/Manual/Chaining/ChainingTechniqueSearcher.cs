using System;
using System.Collections.Generic;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Data.Extensions;
using Sudoku.Extensions;
using static Sudoku.Constants.Processings;
using static Sudoku.Constants.RegionLabel;
using static Sudoku.Solving.Manual.Chaining.Node.Cause;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Encapsulates a <b>chain</b> technique searcher.
	/// </summary>
	public sealed class ChainingTechniqueSearcher : TechniqueSearcher
	{
		/// <summary>
		/// Indicats whether the searcher will search for nishio forcing chains.
		/// </summary>
		private readonly bool _withNishio;

		/// <summary>
		/// Indicating whether the searcher will search for multiple forcing chains.
		/// </summary>
		private readonly bool _withMultiple;

		/// <summary>
		/// Indicating whether the searcher will search for dynamic forcing chains.
		/// </summary>
		private readonly bool _withDynamic;

		/// <summary>
		/// The dynamic level. <c>0</c> is for no dynamic.
		/// </summary>
		private readonly int _level;

		/// <summary>
		/// The saved grid.
		/// </summary>
		private Grid? _saveGrid;

		/// <summary>
		/// The last grid used.
		/// </summary>
		private Grid? _lastGrid;

		/// <summary>
		/// The searchers used in advanced relations searching.
		/// </summary>
		private IList<TechniqueSearcher>? _searchers;

		/// <summary>
		/// The last hints stored.
		/// </summary>
		private ICollection<ChainingTechniqueInfo>? _lastHints;


		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="withNishio">
		/// A <see cref="bool"/> value indicating whether the searcher will search for nishio forcing chains.
		/// </param>
		/// <param name="withMultiple">
		/// A <see cref="bool"/> value indicating whether the searcher will search for multiple forcing chains.
		/// </param>
		/// <param name="withDynamic">
		/// A <see cref="bool"/> value indicating whether the searcher will search for dynamic forcing chains.
		/// </param>
		/// <param name="level">The dynamic level. <c>0</c> is for no dynamic.</param>
		public ChainingTechniqueSearcher(bool withNishio, bool withMultiple, bool withDynamic, int level) =>
			(_withNishio, _withMultiple, _withDynamic, _level) = (withNishio, withMultiple, withDynamic, level);


		/// <summary>
		/// The name if worth.
		/// </summary>
		public string Name =>
			true switch
			{
				_ when _withNishio => "Nishio Forcing Chains",
				_ when _withMultiple => "Multiple Forcing Chains",
				_ when _withDynamic =>
					_level == 0 ? "Dynamic Forcing Chains" : $"Dynamic Forcing Chains{GetNestedSuffix(_level)}",
				_ => "Forcing Chains / Cycles"
			};

		/// <summary>
		/// Get the difficulty with the current searcher's state.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// throws when the state is invalid.
		/// </exception>
		public decimal Difficulty =>
			true switch
			{
				_ when _level >= 2 => 9.5M + .5M * (_level - 2),
				_ when _level > 0 => 8.5M + .5M * _level,
				_ when _withNishio => 7.5M,
				_ when _withMultiple => 8M,
				_ when _withDynamic => 8.5M,
				_ => throw new InvalidOperationException()
			};


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			if (grid.ToMutable() == _lastGrid)
			{
				GetPreviousHints(accumulator);
				return;
			}

			var result = GetHintList(grid);
			_lastGrid = grid.Clone();

			// This filters hints that are equal:
			accumulator.AddRange(_lastHints = new List<ChainingTechniqueInfo>(result));
		}

		/// <summary>
		/// Get hint list.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <returns>The technique information.</returns>
		private ICollection<ChainingTechniqueInfo> GetHintList(IReadOnlyGrid grid)
		{
			List<ChainingTechniqueInfo> result;
			if (_withMultiple || _withDynamic)
			{
				result = GetMultipleChainsHintList(grid);
			}
			else
			{
				var xLoops = GetLoopHintList(grid, true, false);
				var yLoops = GetLoopHintList(grid, false, true);
				var xyLoops = GetLoopHintList(grid, true, true);

				result = xLoops;
				result.AddRange(yLoops);
				result.AddRange(xyLoops);
			}

			// Sort the result. The hints with the shortest path are returned first.
			result.Sort((h1, h2) =>
			{
				decimal d1 = h1.Difficulty, d2 = h2.Difficulty;
				if (d1 < d2) return -1;
				else if (d1 > d2) return 1;

				int l1 = h1.Complexity, l2 = h2.Complexity;
				return l1 == l2 ? h1.SortKey - h2.SortKey : l1.CompareTo(l2);
			});

			return result;
		}

		/// <summary>
		/// Get loop hints.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="xEnabled">Indicates whether the searcher enables X-Chains searching.</param>
		/// <param name="yEnabled">Indicates whether the searcher enables Y-Chains searching.</param>
		/// <returns>The list.</returns>
		private List<ChainingTechniqueInfo> GetLoopHintList(IReadOnlyGrid grid, bool xEnabled, bool yEnabled)
		{
			var result = new List<ChainingTechniqueInfo>();

			// Iterate on each empty cell.
			foreach (int cell in EmptyMap)
			{
				short mask = grid.GetCandidateMask(cell);
				if (mask != 0 && !mask.IsPowerOfTwo())
				{
					// Iterate on all candidates that are not alone.
					foreach (int digit in mask.GetAllSets())
					{
						var pOn = new Node(cell * 9 + digit, true);
						DoUnaryChaining(grid, pOn, result, xEnabled, yEnabled);
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Search for hints on the given grid.
		/// </summary>
		/// <param name="grid">The grid on which to search for hints.</param>
		/// <returns>The hints found.</returns>
		private List<ChainingTechniqueInfo> GetMultipleChainsHintList(IReadOnlyGrid grid)
		{
			var result = new List<ChainingTechniqueInfo>();

			// Iterate on each empty cell.
			foreach (int cell in EmptyMap)
			{
				short mask = grid.GetCandidateMask(cell);
				int maskCount = mask.CountSet();
				if (maskCount > 2 || maskCount > 1 && _withDynamic)
				{
					// Prepare storage and accumulator for cell eliminations.
					var valueToOn = new Dictionary<int, HashSet<Node>>();
					var valueToOff = new Dictionary<int, HashSet<Node>>();
					HashSet<Node>? cellToOn = null, cellToOff = null;

					// Iterate on all candidates that are not alone.
					foreach (int digit in mask.GetAllSets())
					{
						// Do Binary chaining (same candidates either on or off).
						var pOn = new Node(cell * 9 + digit, true);
						var pOff = new Node(cell * 9 + digit, false);
						var onToOn = new HashSet<Node>();
						var onToOff = new HashSet<Node>();
						bool doDouble = maskCount >= 3 && !_withNishio && _withDynamic;
						bool doContradiction = _withDynamic || _withNishio;

						DoBinaryChaining(grid, pOn, pOff, result, onToOn, onToOff, doDouble, doContradiction);

						if (_withNishio)
						{
							// Do region chaining.
							DoRegionChaining(grid, result, cell, digit, onToOn, onToOff);
						}

						// Collect results for cell chaining.
						valueToOn.Add(digit, onToOn);
						valueToOff.Add(digit, onToOff);

						if (!(cellToOn is null))
						{
							cellToOn.IntersectWith(onToOn);
						}
						else
						{
							cellToOn = new HashSet<Node>(onToOn);
						}

						if (!(cellToOff is null))
						{
							cellToOff.IntersectWith(onToOff);
						}
						else
						{
							cellToOff = new HashSet<Node>(onToOff);
						}
					}

					if (!_withNishio)
					{
						// Do cell elimination.
						if (maskCount == 2 || _withMultiple/* && maskCount > 2*/)
						{
							if (!(cellToOn is null))
							{
								foreach (var p in cellToOn)
								{
									var hint = CreateCellReductionHint(cell, p, valueToOn);
									result.Add(hint);
								}
							}

							if (!(cellToOff is null))
							{
								foreach (var p in cellToOff)
								{
									var hint = CreateCellReductionHint(cell, p, valueToOff);
									result.Add(hint);
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Get the reversed loop with the specified org.
		/// </summary>
		/// <param name="org">The org.</param>
		/// <returns>The reversed node.</returns>
		private Node GetReversedLoop(Node? org)
		{
			var result = new HashSet<Node>();
			string? explanations = null;
			while (!(org is null))
			{
				var rev = new Node(org.Candidate, !org.IsOn, org.NodeCause, explanations);
				explanations = org.Explanation;
				result.Insert(0, rev);

				org = org.Parents.Count != 0 ? org.Parents[0] : null;
			}

			Node? prev = null;
			foreach (var rev in result)
			{
				if (!(prev is null))
				{
					prev.Parents.Add(rev);
				}

				prev = rev;
			}

			return result.Get(0);
		}

		/// <summary>
		/// Look for, and add single forcing chains, and continuous nice loops.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="pOn">The start node.</param>
		/// <param name="result">Filled with the hints found.</param>
		/// <param name="xEnabled">Whether X-Chains are enabled.</param>
		/// <param name="yEnabled">Whether Y-Chains are enabled.</param>
		private void DoUnaryChaining(
			IReadOnlyGrid grid, Node pOn, IList<ChainingTechniqueInfo> result, bool xEnabled, bool yEnabled)
		{
			var loops = new List<Node>();
			var chains = new List<Node>();
			var onToOn = new HashSet<Node> { pOn };
			var onToOff = new HashSet<Node>();

			DoLoops(grid, onToOn, onToOff, xEnabled, yEnabled, loops, pOn);

			if (xEnabled)
			{
				// Y-Chains don't exist (length must be both odd and even).

				// Chain with 'off' implication.
				onToOn = new HashSet<Node> { pOn };
				onToOff = new HashSet<Node>();

				DoForcingChains(grid, onToOn, onToOff, yEnabled, chains, pOn);

				var pOff = new Node(pOn.Candidate, false);
				onToOn = new HashSet<Node>();
				onToOff = new HashSet<Node> { pOff };

				DoForcingChains(grid, onToOn, onToOff, yEnabled, chains, pOff);
			}

			foreach (var destOn in loops)
			{
				// Continuous nice loops found.
				var destOff = GetReversedLoop(destOn);
				var hint = CreateLoopHint(grid, destOn, destOff, xEnabled, yEnabled);
				result.Add(hint);
			}

			foreach (var target in chains)
			{
				var hint = CreateForcingChainHint(grid, target, xEnabled, yEnabled);
				result.Add(hint);
			}
		}

		/// <summary>
		/// To check the binary chaining.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="pOn">An empty set, filled with nodes that get on if the given node is on.</param>
		/// <param name="pOff">An empty set, filled with nodes that get off if the given node is on.</param>
		/// <param name="result">The result accumulator.</param>
		/// <param name="onToOn">The on-to-on list.</param>
		/// <param name="onToOff">The on-to-off list.</param>
		/// <param name="doReduction">Indicates whether the method should check double forcing chains.</param>
		/// <param name="doContradiction">
		/// Indicates whether the method should check contradiction forcing chains.
		/// </param>
		/// <remarks>
		/// <para>From the node <c>p</c>, compute the consequences from both states.</para>
		/// <para>
		/// More precisely, <c>p</c> is first assumed to be correct (<c>on</c>), and then to be
		/// incorrect (<c>off</c>); and the following sets are created:
		/// <list type="table">
		/// <item>
		/// <term><c>onToOn</c></term>
		/// <description>The set of nodes that must be <c>on</c> when <c>p</c> is <c>on</c>.</description>
		/// </item>
		/// <item>
		/// <term><c>onToOff</c></term>
		/// <description>The set of nodes that must be <c>off</c> when <c>p</c> is <c>on</c>.</description>
		/// </item>
		/// <item>
		/// <term><c>offToOn</c></term>
		/// <description>The set of nodes that must be <c>on</c> when <c>p</c> is <c>off</c>.</description>
		/// </item>
		/// <item>
		/// <term><c>offToOff</c></term>
		/// <description>The set of nodes that must be <c>off</c> when <c>p</c> is <c>off</c>.</description>
		/// </item>
		/// </list>
		/// </para>
		/// <para>
		/// Then the following rules are applied:
		/// <list type="bullet">
		/// <item>
		/// If a node belongs to both <c>onToOn</c> and <c>onToOff</c>, the node <c>p</c> cannot be <c>on</c>
		/// because it would imply a node to be both <c>on</c> and <c>off</c>, which is an absurd.
		/// </item>
		/// <item>
		/// If a node belongs to both <c>offToOn</c> and <c>offToOff</c>, the node <c>p</c> cannot be <c>off</c>
		/// because it would imply a node to be both <c>on</c> and <c>off</c>, which is an absurd.
		/// </item>
		/// <item>
		/// If a node belongs to both <c>onToOn</c> and <c>offToOn</c>, this node must be <c>on</c>, because
		/// it is implied to be <c>on</c> by the two possible states of <c>p</c>.
		/// </item>
		/// <item>
		/// If a node belongs to both <c>onToOff</c> and <c>offtoOff</c>, this node must be <c>off</c>, because
		/// it is implied to be <c>off</c> by the two possible states of <c>p</c>.
		/// </item>
		/// </list>
		/// </para>
		/// <para>
		/// Note that if a node belongs to all the four sets, the sudoku has no solution. This is not checked here.
		/// </para>
		/// </remarks>
		private void DoBinaryChaining(
			IReadOnlyGrid grid, Node pOn, Node pOff, IList<ChainingTechniqueInfo> result, ISet<Node> onToOn,
			ISet<Node> onToOff, bool doReduction, bool doContradiction)
		{
			Node[] absurdNodes;
			var offToOn = new HashSet<Node>();
			var offToOff = new HashSet<Node>();

			// Circular forcing chains (hypothesis implying its negation)
			// are already covered by cell forcing chains, and are therefore
			// not checked for.
			onToOn.Add(pOn);

			// Test p is on.
			absurdNodes = DoChaining(grid, onToOn, onToOff);
			if (doContradiction && !(absurdNodes is null))
			{
				// 'p' cannot hold its value, otherwise it would lead to a contradiction.
				var hint = CreateChainingOffHint(absurdNodes[0], absurdNodes[1], pOn, pOn, true);
				result.Add(hint);
			}

			// Test p is off.
			offToOff.Add(pOff);
			absurdNodes = DoChaining(grid, offToOn, offToOff);
			if (doContradiction && !(absurdNodes is null))
			{
				// 'p' must hold its value, otherwise it would lead to a contradiction.
				var hint = CreateChainingOnHint(absurdNodes[0], absurdNodes[1], pOff, pOff, true);
				result.Add(hint);
			}

			if (doReduction)
			{
				// Check nodes that must be on in both case.
				foreach (var pFromOn in onToOn)
				{
					if (offToOn.TryGetValue(pFromOn, out var pFromOff))
					{
						var hint = CreateChainingOnHint(pFromOn, pFromOff, pOn, pFromOn, false);
						result.Add(hint);
					}
				}

				// Check nodes that must be off in both case.
				foreach (var pFromOn in onToOff)
				{
					if (offToOff.TryGetValue(pFromOn, out var pFromOff))
					{
						var hint = CreateChainingOffHint(pFromOn, pFromOff, pOff, pFromOff, false);
						result.Add(hint);
					}
				}
			}
		}

		/// <summary>
		/// Do region chaining.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="result">The result accumulator.</param>
		/// <param name="cell">The current cell.</param>
		/// <param name="digit">The current digit.</param>
		/// <param name="onToOn">The on-to-on list.</param>
		/// <param name="onToOff">The on-to-off list.</param>
		private void DoRegionChaining(
			IReadOnlyGrid grid, IList<ChainingTechniqueInfo> result, int cell, int digit,
			ISet<Node> onToOn, ISet<Node> onToOff)
		{
			for (var label = Block; label < UpperLimit; label++)
			{
				int region = GetRegion(cell, label);
				var map = CandMaps[digit] & RegionMaps[region];

				// To determine whether the region is worth to search for hints.
				int mapCount = map.Count;
				if (mapCount == 2 || _withMultiple && mapCount > 2)
				{
					int firstPos = map.SetAt(0);
					if (firstPos == cell)
					{
						// We meet the region for the first time.
						var posToOn = new Dictionary<int, ISet<Node>>();
						var posToOff = new Dictionary<int, ISet<Node>>();
						var regionToOn = new HashSet<Node>();
						var regionToOff = new HashSet<Node>();

						// Iterate on node positions within the region.
						foreach (int pos in map)
						{
							if (pos == cell)
							{
								posToOn.Add(pos, onToOn);
								posToOff.Add(pos, onToOff);
								regionToOn.AddRange(onToOn);
								regionToOff.AddRange(onToOff);
							}
							else
							{
								var other = new Node(pos * 9 + digit, true);
								var otherToOn = new HashSet<Node> { other };
								var otherToOff = new HashSet<Node>();

								DoChaining(grid, otherToOn, otherToOff);

								posToOn.Add(pos, otherToOn);
								posToOff.Add(pos, otherToOff);
								regionToOn.IntersectWith(otherToOn);
								regionToOff.IntersectWith(otherToOff);
							}
						}

						// Gather results.
						foreach (var p in regionToOn)
						{
							var hint = CreateRegionReductionHint(region, digit, p, posToOn);
							result.Add(hint);
						}
						foreach (var p in regionToOff)
						{
							var hint = CreateRegionReductionHint(region, digit, p, posToOff);
							result.Add(hint);
						}
					}
				}
			}
		}

		/// <summary>
		/// Get on to off (weak links).
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="p">The current node.</param>
		/// <param name="yEnabled">Indicates whether the current Y-Chains is enabled.</param>
		/// <returns>The nodes.</returns>
		private ISet<Node> GetOnToOff(IReadOnlyGrid grid, Node p, bool yEnabled)
		{
			var result = new HashSet<Node>();
			int cell = p.Candidate / 9, digit = p.Candidate % 9;

			// This rule is not used with X-Chains.
			if (yEnabled)
			{
				foreach (int d in grid.GetCandidates(cell))
				{
					if (d != digit)
					{
						result.Add(
							new Node(
								cell * 9 + d, false, NakedSingle, p,
								"the cell can contain only one value"));
					}
				}
			}

			for (var label = Block; label < UpperLimit; label++)
			{
				int region = GetRegion(cell, label);
				foreach (int c in RegionMaps[region] & CandMaps[digit])
				{
					if (c != cell)
					{
						result.Add(
							new Node(
								c * 9 + digit, false, GetRegionCause(label), p,
								$"the value can occur only once in the {new RegionCollection(region).ToString()}"));
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Add hidden parents of cell.
		/// </summary>
		/// <param name="p">The current node.</param>
		/// <param name="grid">The initial grid.</param>
		/// <param name="source">The source.</param>
		/// <param name="offNodes">The off nodes.</param>
		/// <exception cref="SudokuRuntimeException">
		/// Throws when the parent node cannot be found.
		/// </exception>
		private void AddHiddenParentsOfCell(Node p, IReadOnlyGrid grid, IReadOnlyGrid source, HashSet<Node> offNodes)
		{
			int cell = p.Candidate / 9;
			short currentCell = grid.GetCandidateMask(cell);
			short sourceCell = source.GetCandidateMask(cell);
			for (int digit = 0; digit < 9; digit++)
			{
				if (source.Exists(sourceCell, digit) is true && !grid.Exists(currentCell, digit) is true)
				{
					// Add a hidden parent.
					var parent = new Node(currentCell * 9 + digit, false);
					if (offNodes.TryGetValue(parent, out parent))
					{
						p.Parents.Add(parent);
					}
					else
					{
						throw new SudokuRuntimeException("Parent node cannot be found.");
					}
				}
			}
		}

		/// <summary>
		/// Add hidden parents of region.
		/// </summary>
		/// <param name="p">The node.</param>
		/// <param name="grid">The initial grid.</param>
		/// <param name="source">The source.</param>
		/// <param name="label">The region label.</param>
		/// <param name="region">the region index.</param>
		/// <param name="offNodes">The off nodes.</param>
		/// <exception cref="SudokuRuntimeException">
		/// Throws when parent node cannot be found.
		/// </exception>
		private void AddHiddenParentsOfRegion(
			Node p, IReadOnlyGrid grid, IReadOnlyGrid source, RegionLabel label, int region,
			HashSet<Node> offNodes)
		{
			int cell = p.Candidate / 9, digit = p.Candidate % 9;
			int sourceRegion = GetRegion(cell, label);
			var currentMap = RegionMaps[region] & CandMaps[digit];
			var sourceMap = RegionMaps[sourceRegion];
			foreach (int c in RegionCells[sourceRegion])
			{
				if (!(source.Exists(c, digit) is true))
				{
					sourceMap.Remove(c);
				}
			}
			sourceMap -= currentMap;

			foreach (int c in sourceMap)
			{
				// Add a hidden parent.
				var parent = new Node(c * 9 + digit, false);
				if (offNodes.TryGetValue(parent, out parent))
				{
					p.Parents.Add(parent);
				}
				else
				{
					throw new SudokuRuntimeException("Parent node cannot be found.");
				}
			}
		}

		/// <summary>
		/// Get off to on (strong links).
		/// </summary>
		/// <param name="grid">The initial grid.</param>
		/// <param name="p">The current node.</param>
		/// <param name="source">The source grid.</param>
		/// <param name="offNodes">The off nodes.</param>
		/// <param name="xEnabled">Whether the X-Chains are enabled.</param>
		/// <param name="yEnabled">Whether the Y-Chains are enabled.</param>
		/// <returns>The nodes.</returns>
		private ISet<Node> GetOffToOn(
			IReadOnlyGrid grid, Node p, IReadOnlyGrid source, HashSet<Node> offNodes, bool xEnabled, bool yEnabled)
		{
			var result = new HashSet<Node>();
			int cell = p.Candidate / 9, digit = p.Candidate % 9;

			if (yEnabled)
			{
				// First rule: If there is only two nodes in this cell, the other one will be got on.
				if (BivalueMap[cell])
				{
					short mask = grid.GetCandidateMask(cell);
					int otherDigit = mask.FindFirstSet();
					if (otherDigit == digit)
					{
						otherDigit = mask.GetNextSet(otherDigit);
					}

					var pOn = new Node(
						cell * 9 + otherDigit, true, NakedSingle, p, "only remaining possible value in the cell");

					AddHiddenParentsOfCell(pOn, grid, source, offNodes);

					result.Add(pOn);
				}
			}

			if (xEnabled)
			{
				// Second rule: If there is only two positions for this node, the other one will be got on.
				for (var label = Block; label < UpperLimit; label++)
				{
					int region = GetRegion(cell, label);
					var map = RegionMaps[region] & CandMaps[digit];
					if (map.Count == 2)
					{
						int otherPos = map.SetAt(0);
						if (otherPos == cell)
						{
							otherPos = map.SetAt(1);
						}

						var pOn = new Node(
							otherPos * 9 + digit, true, GetRegionCause(label), p,
							$"only remaining possible position in the {new RegionCollection(region).ToString()}");

						AddHiddenParentsOfRegion(pOn, grid, source, label, region, offNodes);

						result.Add(pOn);
					}
				}
			}
		}

		/// <summary>
		/// Get previous hints.
		/// </summary>
		/// <param name="accumulator">The accumulator.</param>
		private void GetPreviousHints(IBag<TechniqueInfo> accumulator)
		{
			if (!(_lastHints is null))
			{
				accumulator.AddRange(_lastHints);
			}
		}


		/// <summary>
		/// Get nested suffix using the specified level.
		/// </summary>
		/// <param name="level">The level.</param>
		/// <returns>The suffix.</returns>
		public static string GetNestedSuffix(int level) =>
			level switch
			{
				1 => " (+)",
				2 => " (+ Forcing Chains)",
				3 => " (+ Multiple Forcing Chains)",
				4 => " (+ Dynamic Forcing Chains)",
				_ when level >= 5 => $" (+ Dynamic Forcing Chains{GetNestedSuffix(level - 3)})",
				_ => string.Empty
			};

		/// <summary>
		/// To check whether the current <paramref name="parent"/> is the parent of
		/// the child node <paramref name="child"/>.
		/// </summary>
		/// <param name="child">The child node to check.</param>
		/// <param name="parent">The parent node to check.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		private static bool IsParentOf(Node child, Node parent)
		{
			var p = child;
			while (p.Parents.Count != 0)
			{
				p = p.Parents[0];
				if (p == parent)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Get the region cause with the specified region label.
		/// </summary>
		/// <param name="regionLabel">The region label.</param>
		/// <returns>The region cause.</returns>
		public static Node.Cause GetRegionCause(RegionLabel regionLabel) =>
			regionLabel switch
			{
				Block => HiddenSingleBlock,
				Column => HiddenSingleColumn,
				_ => HiddenSingleRow,
			};
	}
}
