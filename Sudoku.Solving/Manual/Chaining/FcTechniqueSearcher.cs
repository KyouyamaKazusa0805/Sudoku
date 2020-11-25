using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.Extensions;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Encapsulates an <b>forcing chains</b> (<b>FCs</b>) technique searcher.
	/// </summary>
	[TechniqueDisplay(nameof(TechniqueCode.RegionFc))]
	public sealed class FcTechniqueSearcher : ChainingTechniqueSearcher
	{
		/// <summary>
		/// Indicates the information.
		/// </summary>
		private readonly bool _nishio, _multiple, _dynamic;

#pragma warning disable IDE0052
		/// <summary>
		/// Indicates the level of the searching depth.
		/// </summary>
		private readonly int _level;
#pragma warning restore IDE0052


		/// <summary>
		/// Indicates the grid that is used in processing.
		/// </summary>
		private SudokuGrid _savedGrid;


		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="nishio">Indicates whether the searcher should search for nishio chains.</param>
		/// <param name="multiple">Indicates whether the searcher should search for multiple chains.</param>
		/// <param name="dynamic">Indicates whether the searcher should search for dynamic chains.</param>
		public FcTechniqueSearcher(bool nishio, bool multiple, bool dynamic, int level)
		{
			_nishio = nishio;
			_multiple = multiple;
			_dynamic = dynamic;
			_level = level;
		}


		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(80);


		/// <inheritdoc/>
		public override void GetAll(IList<TechniqueInfo> accumulator, in SudokuGrid grid)
		{
			var tempGrid = grid;
			var tempAccumulator = new List<ChainingTechniqueInfo>();
			GetAll(tempAccumulator, ref tempGrid);

			if (tempAccumulator.Count == 0)
			{
				return;
			}

			accumulator.AddRange(SortInfo(tempAccumulator));
		}

		/// <summary>
		/// Search for chains of each type.
		/// </summary>
		/// <param name="accumulator">The accumulator.</param>
		/// <param name="grid">(<see langword="ref"/> parameter) The grid.</param>
		private void GetAll(IList<ChainingTechniqueInfo> accumulator, ref SudokuGrid grid)
		{
			// Iterate on all empty cells.
			foreach (int cell in EmptyMap)
			{
				short mask = grid.GetCandidateMask(cell);
				int count = mask.PopCount();
				if (count > 2 || _dynamic && count > 1)
				{
					// Prepare storage and accumulator for cell eliminations.
					Dictionary<int, Set<Node>> valueToOn = new(), valueToOff = new();
					Set<Node>? cellToOn = null, cellToOff = null;

					// Iterate on all candidates that aren't alone.
					foreach (int digit in mask)
					{
						var pOn = new Node(cell, digit, true);
						var pOff = new Node(cell, digit, false);
						Set<Node> onToOn = new(), onToOff = new();

						bool doDouble = count >= 3 && !_nishio && _dynamic;
						bool doContradiction = _dynamic || _nishio;

						DoBinaryChaining(
							ref grid, pOn, pOff, accumulator, onToOn, onToOff, doDouble, doContradiction);

						if (!_nishio)
						{
							DoRegionChaining(accumulator, ref grid, cell, digit, onToOn, onToOff);
						}

						// Collect results for cell chaining.
						valueToOn.Add(digit, onToOn);
						valueToOff.Add(digit, onToOff);
						if (cellToOn is null || cellToOff is null)
						{
							cellToOn = new(onToOn);
							cellToOff = new(onToOff);
						}
						else
						{
							cellToOn &= onToOn;
							cellToOff &= onToOff;
						}
					}

					// Do cell eliminations.
					if (!_nishio && (count == 2 || _multiple))
					{
						if (cellToOn is not null)
						{
							foreach (var p in cellToOn)
							{
								var hint = CreateCellEliminationHint(grid, cell, p, valueToOn);
								if (hint is not null)
								{
									accumulator.Add(hint);
								}
							}
						}
						if (cellToOff is not null)
						{
							foreach (var p in cellToOff)
							{
								var hint = CreateCellEliminationHint(grid, cell, p, valueToOff);
								if (hint is not null)
								{
									accumulator.Add(hint);
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Do binary chaining.
		/// </summary>
		/// <param name="grid">(<see langword="ref"/> parameter) The grid.</param>
		/// <param name="pOn">(<see langword="in"/> parameter) The on node.</param>
		/// <param name="pOff">(<see langword="in"/> parameter) The off node.</param>
		/// <param name="accumulator">The current accumulator.</param>
		/// <param name="onToOn">The list for <c>on</c> nodes to <c>on</c> nodes.</param>
		/// <param name="onToOff">The list for <c>on</c> nodes to <c>off</c> nodes.</param>
		/// <param name="doReduction">Indicates whether the method executes double chaining.</param>
		/// <param name="doContradiction">Indicates whether the method executes contradiction chaining.</param>
		private void DoBinaryChaining(
			ref SudokuGrid grid, in Node pOn, in Node pOff, IList<ChainingTechniqueInfo> accumulator,
			ISet<Node> onToOn, ISet<Node> onToOff, bool doReduction, bool doContradiction)
		{
			Set<Node> offToOn = new(), offToOff = new();

			// Circular Forcing Chains (hypothesis implying its negation)
			// are already covered by Cell Forcing Chains, and are therefore
			// not checked for.

			// Test 'p' is on.
			onToOn.Add(pOn);
			var absurdNodes = DoChaining(ref grid, onToOn, onToOff);
			if (doContradiction && absurdNodes is not null)
			{
				// 'p' can't hold its value, otherwise it'd lead to a contradiction.
				var hint = CreateChainingOffHint(absurdNodes[0], absurdNodes[1], pOn, pOn, true);
				if (hint is not null)
				{
					accumulator.Add(hint);
				}
			}

			// Test 'p' is off.
			offToOff.Add(pOff);
			absurdNodes = DoChaining(ref grid, offToOn, offToOff);
			if (doContradiction && absurdNodes is not null)
			{
				// 'p' must hold its value, otherwise it'd lead to a contradiction.
				var hint = CreateChainingOnHint(absurdNodes[0], absurdNodes[1], pOff, pOff, true);
				if (hint is not null)
				{
					accumulator.Add(hint);
				}
			}

			if (doReduction)
			{
				// Check candidates that must be on in both cases.
				foreach (var pFromOn in onToOn)
				{
					if (offToOn.Contains(pFromOn))
					{
						//                                       pFromOff
						var hint = CreateChainingOnHint(pFromOn, pFromOn, pOn, pFromOn, false);
						if (hint is not null)
						{
							accumulator.Add(hint);
						}
					}
				}

				// Check candidates that must be off in both cases.
				foreach (var pFromOn in onToOff)
				{
					if (offToOff.Contains(pFromOn))
					{
						//                                        pFromOff       pFromOff
						var hint = CreateChainingOffHint(pFromOn, pFromOn, pOff, pFromOn, false);
						if (hint is not null)
						{
							accumulator.Add(hint);
						}
					}
				}
			}
		}

		/// <summary>
		/// Do region chaining.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">(<see langword="ref"/> parameter) </param>
		/// <param name="cell">The cell.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="onToOn">The list for <c>on</c> nodes to <c>on</c> nodes.</param>
		/// <param name="onToOff">The list for <c>on</c> nodes to <c>off</c> nodes.</param>
		private void DoRegionChaining(
			IList<ChainingTechniqueInfo> accumulator, ref SudokuGrid grid, int cell, int digit,
			Set<Node> onToOn, Set<Node> onToOff)
		{
			for (var label = RegionLabel.Block; label <= RegionLabel.Column; label++)
			{
				int region = GetRegion(cell, label);
				var worthMap = CandMaps[digit] & RegionMaps[region];
				if (worthMap.Count == 2 || _multiple && worthMap.Count > 2)
				{
					// Determine whether we meet this region for the first time.
					if (worthMap.First == cell)
					{
						Dictionary<int, Set<Node>> posToOn = new(), posToOff = new();
						Set<Node> regionToOn = new(), regionToOff = new();

						// Iterate on node positions within the region.
						foreach (int otherCell in worthMap)
						{
							if (otherCell == cell)
							{
								posToOn.Add(otherCell, onToOn);
								posToOff.Add(otherCell, onToOff);
								regionToOn |= onToOn;
								regionToOff |= onToOff;
							}
							else
							{
								var other = new Node(otherCell, digit, true);
								Set<Node> otherToOn = new() { other }, otherToOff = new();

								DoChaining(ref grid, otherToOn, otherToOff);

								posToOn.Add(otherCell, otherToOn);
								posToOff.Add(otherCell, otherToOff);
								regionToOn &= otherToOn;
								regionToOff &= otherToOff;
							}
						}

						// Gather results.
						foreach (var p in regionToOn)
						{
							var hint = CreateRegionEliminationHint(region, digit, p, posToOn);
							if (hint is not null)
							{
								accumulator.Add(hint);
							}
						}
						foreach (var p in regionToOff)
						{
							var hint = CreateRegionEliminationHint(region, digit, p, posToOff);
							if (hint is not null)
							{
								accumulator.Add(hint);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Do chaining (i.e. multiple chaining).
		/// </summary>
		/// <param name="grid">(<see langword="ref"/> parameter) The grid.</param>
		/// <param name="toOn">The list to <c>on</c> nodes.</param>
		/// <param name="toOff">The list to <c>off</c> nodes.</param>
		/// <returns>The result.</returns>
		private Node[]? DoChaining(ref SudokuGrid grid, ISet<Node> toOn, ISet<Node> toOff)
		{
			_savedGrid = grid;

			try
			{
				Set<Node> pendingOn = new(toOn), pendingOff = new(toOff);
				while (pendingOn.Count != 0 || pendingOff.Count != 0)
				{
					if (pendingOn.Count != 0)
					{
						var p = pendingOn.Remove();
						var makeOff = GetOnToOff(grid, p, !_nishio);

						foreach (var pOff in makeOff)
						{
							var pOn = new Node(pOff.Cell, pOff.Digit, true); // Conjugate
							if (toOn.Contains(pOn))
							{
								// Contradiction found.
								return new[] { pOn, pOff }; // Cannot be both on and off at the same time.
							}
							else if (!toOff.Contains(pOff))
							{
								// Not processed yet.
								toOff.Add(pOff);
								pendingOff.Add(pOff);
							}
						}
					}
					else
					{
						var p = pendingOff.Remove();
						var makeOn = GetOffToOn(grid, p, true, !_nishio, _savedGrid, toOff);

						if (_dynamic)
						{
							p.OffTo(ref grid);
						}

						foreach (var pOn in makeOn)
						{
							var pOff = new Node(pOn.Cell, pOn.Digit, false); // Conjugate.
							if (toOff.Contains(pOff))
							{
								// Contradiction found.
								return new[] { pOn, pOff }; // Cannot be both on and off at the same time.
							}
							else if (!toOn.Contains(pOn))
							{
								// Not processed yet.
								toOn.Add(pOn);
								pendingOn.Add(pOn);
							}
						}
					}

					// TODO: Get advanced relations (i.e. FCs (+) in Sudoku Explainer).
				}

				return null;
			}
			finally
			{
				grid = _savedGrid;
			}
		}

		/// <summary>
		/// Create a chaining hint whose conclusion is an on result.
		/// </summary>
		/// <param name="destOn">(<see langword="in"/> parameter) The destination on node.</param>
		/// <param name="destOff">(<see langword="in"/> parameter) The destination off node.</param>
		/// <param name="source">(<see langword="in"/> parameter) The source node.</param>
		/// <param name="target">(<see langword="in"/> parameter) The target node.</param>
		/// <param name="isAbsurd">Indicates whether the chain is absurd.</param>
		/// <returns>The hint.</returns>
		private BinaryChainingTechniqueInfo CreateChainingOnHint(
			in Node destOn, in Node destOff, in Node source, in Node target, bool isAbsurd)
		{
			// Get views.
			var cellOffset = new DrawingInfo[] { new(0, source.Cell) };
			var views = new List<View>();
			var globalCandidates = new List<DrawingInfo>();
			var globalLinks = new List<Link>();
			foreach (var node in destOn.Chain)
			{
				var candidateOffsets = new List<DrawingInfo>(node.GetCandidateOffsets());
				var links = node.GetLinks(true);

				views.Add(new(cellOffset, candidateOffsets, null, links));

				globalCandidates.AddRange(candidateOffsets);
				globalLinks.AddRange(links);
			}
			foreach (var node in destOff.Chain)
			{
				var candidateOffsets = new List<DrawingInfo>(node.GetCandidateOffsets());
				var links = node.GetLinks(true);
				views.Add(new(cellOffset, candidateOffsets, null, links));

				globalCandidates.AddRange(candidateOffsets);
				globalLinks.AddRange(links);
			}

			// Insert the global view at head.
			views.Insert(0, new(cellOffset, globalCandidates, null, globalLinks));

			return new BinaryChainingTechniqueInfo(
				new Conclusion[] { new(Assignment, target.Cell, target.Digit) },
				views,
				source,
				destOn,
				destOff,
				isAbsurd,
				_nishio);
		}

		/// <summary>
		/// Create a chaining hint whose conclusion is an off result.
		/// </summary>
		/// <param name="destOn">(<see langword="in"/> parameter) The destination on node.</param>
		/// <param name="destOff">(<see langword="in"/> parameter) The destination off node.</param>
		/// <param name="source">(<see langword="in"/> parameter) The source node.</param>
		/// <param name="target">(<see langword="in"/> parameter) The target node.</param>
		/// <param name="isAbsurd">Indicates whether the chain is absurd.</param>
		/// <returns>The hint.</returns>
		private BinaryChainingTechniqueInfo CreateChainingOffHint(
			in Node destOn, in Node destOff, in Node source, in Node target, bool isAbsurd)
		{
			// Get views.
			var cellOffset = new DrawingInfo[] { new(0, source.Cell) };
			var views = new List<View>();
			var globalCandidates = new List<DrawingInfo>();
			var globalLinks = new List<Link>();
			foreach (var node in destOn.Chain)
			{
				var candidateOffsets = new List<DrawingInfo>(node.GetCandidateOffsets());
				var links = node.GetLinks(true);

				views.Add(new(cellOffset, candidateOffsets, null, links));

				globalCandidates.AddRange(candidateOffsets);
				globalLinks.AddRange(links);
			}
			foreach (var node in destOff.Chain)
			{
				var candidateOffsets = new List<DrawingInfo>(node.GetCandidateOffsets());
				var links = node.GetLinks(true);
				views.Add(new(cellOffset, candidateOffsets, null, links));

				globalCandidates.AddRange(candidateOffsets);
				globalLinks.AddRange(links);
			}

			// Insert the global view at head.
			views.Insert(0, new(cellOffset, globalCandidates, null, globalLinks));

			return new BinaryChainingTechniqueInfo(
				new Conclusion[] { new(Elimination, target.Cell, target.Digit) },
				views,
				source,
				destOn,
				destOff,
				isAbsurd,
				_nishio);
		}

		/// <summary>
		/// Create the hint for cell forcing chains.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="sourceCell">The source cell.</param>
		/// <param name="target">(<see langword="in"/> parameter) The target elimination node.</param>
		/// <param name="outcomes">All outcomes (conclusions).</param>
		/// <returns>The information instance.</returns>
		private ChainingTechniqueInfo? CreateCellEliminationHint(
			in SudokuGrid grid, int sourceCell, in Node target, IReadOnlyDictionary<int, Set<Node>> outcomes)
		{
			// Build removable nodes.
			var conclusions = new List<Conclusion>
			{
				new(target.IsOn ? Assignment : Elimination, target.Cell, target.Digit)
			};

			// Build chains.
			var chains = new Dictionary<int, Node>();
			foreach (int digit in grid.GetCandidateMask(sourceCell))
			{
				// Get the node that contains the same cell, digit and isOn property.
				var valueTarget = outcomes[digit][target];
				chains.Add(digit, valueTarget);
			}

			// Get views.
			var views = new List<View>();
			var globalCandidates = new List<DrawingInfo>();
			var globalLinks = new List<Link>();
			foreach (var (digit, node) in chains)
			{
				var candidateOffsets = new List<DrawingInfo>(node.GetCandidateOffsets()) { new(2, sourceCell * 9 + digit) };
				var links = node.GetLinks(true);
				views.Add(new(new DrawingInfo[] { new(0, sourceCell) }, candidateOffsets, null, links));
				globalCandidates.AddRange(candidateOffsets);
				globalLinks.AddRange(links);
			}

			// Insert the global view at head.
			views.Insert(0, new(new DrawingInfo[] { new(0, sourceCell) }, globalCandidates, null, globalLinks));

			return new CellChainingTechniqueInfo(conclusions, views, sourceCell, chains);
		}

		/// <summary>
		/// Create a hint of region forcing chains.
		/// </summary>
		/// <param name="region">The region.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="target">(<see langword="in"/> parameter) The target node.</param>
		/// <param name="outcomes">All outcomes (conclusions).</param>
		/// <returns>The technique information instance.</returns>
		private ChainingTechniqueInfo? CreateRegionEliminationHint(
			int region, int digit, in Node target, IDictionary<int, Set<Node>> outcomes)
		{
			// Build removable nodes.
			var conclusions = new List<Conclusion>
			{
				new(target.IsOn ? Assignment : Elimination, target.Cell, target.Digit)
			};

			// Build chains.
			var chains = new Dictionary<int, Node>();
			var map = RegionMaps[region] & CandMaps[digit];
			foreach (int cell in map)
			{
				// Get the node that contains the same cell, digit and isOn property.
				var posTarget = outcomes[cell][target];
				chains.Add(cell, posTarget);
			}

			// Get views.
			var views = new List<View>();
			var globalCandidates = new List<DrawingInfo>();
			var globalLinks = new List<Link>();
			foreach (var (cell, node) in chains)
			{
				var candidateOffsets = new List<DrawingInfo>(node.GetCandidateOffsets()) { new(2, cell * 9 + digit) };
				var links = node.GetLinks(true);
				views.Add(new(null, candidateOffsets, new DrawingInfo[] { new(0, region) }, links));
				globalCandidates.AddRange(candidateOffsets);
				globalLinks.AddRange(links);
			}

			views.Insert(0, new(null, globalCandidates, new DrawingInfo[] { new(0, region) }, globalLinks));

			return new RegionChainingTechniqueInfo(conclusions, views, region, digit, chains);
		}
	}
}
