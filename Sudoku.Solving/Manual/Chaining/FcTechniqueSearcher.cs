using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.Extensions;
using static Sudoku.Constants.Processings;
using static Sudoku.Constants.RegionLabel;
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
		private readonly bool _multiple;


		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		public FcTechniqueSearcher() => _multiple = true;


		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(80);


		/// <inheritdoc/>
		public override void GetAll(IList<TechniqueInfo> accumulator, Grid grid)
		{
			var tempAccumulator = new List<ChainingTechniqueInfo>();
			GetAll(tempAccumulator, grid);

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
		/// <param name="grid">Thr grid.</param>
		private void GetAll(IList<ChainingTechniqueInfo> accumulator, Grid grid)
		{
			// Iterate on all empty cells.
			foreach (int cell in EmptyMap)
			{
				short mask = grid.GetCandidateMask(cell);
				int count = mask.CountSet();
				if (count > 2)
				{
					// Prepare storage and accumulator for cell eliminations.
					var valueToOn = new Dictionary<int, Set<Node>>();
					var valueToOff = new Dictionary<int, Set<Node>>();
					Set<Node>? cellToOn = null, cellToOff = null;

					// Iterate on all candidates that aren't alone.
					foreach (int digit in mask)
					{
						var pOn = new Node(cell, digit, true);
						var pOff = new Node(cell, digit, false);
						var onToOn = new Set<Node>();
						var onToOff = new Set<Node>();

						// Do region chaining.
						onToOn.Add(pOn);
						DoChaining(grid, onToOn, onToOff);

						DoRegionChaining(accumulator, grid, cell, digit, onToOn, onToOff);

						// Collect results for cell chaining.
						valueToOn.Add(digit, onToOn);
						valueToOff.Add(digit, onToOff);
						if (cellToOn is null/* || cellToOff is null*/)
						{
							cellToOn = new(onToOn);
							cellToOff = new(onToOff);
						}
						else
						{
							cellToOn &= onToOn;
							cellToOff = cellToOff! & onToOff;
						}
					}

					// Do cell eliminations.
					if (count == 2 || _multiple)
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

		private void DoRegionChaining(
			IList<ChainingTechniqueInfo> accumulator, Grid grid, int cell, int digit,
			Set<Node> onToOn, Set<Node> onToOff)
		{
			for (var label = Block; label <= Column; label++)
			{
				int region = GetRegion(cell, label);
				var worthMap = CandMaps[digit] & RegionMaps[region];
				if (worthMap.Count == 2 || _multiple && worthMap.Count > 2)
				{
					// Determine whether we meet this region for the first time.
					if (worthMap.First == cell)
					{
						var posToOn = new Dictionary<int, Set<Node>>();
						var posToOff = new Dictionary<int, Set<Node>>();
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

								DoChaining(grid, otherToOn, otherToOff);

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

		private Node[]? DoChaining(Grid grid, ISet<Node> toOn, ISet<Node> toOff)
		{
			var pendingOn = new Set<Node>(toOn);
			var pendingOff = new Set<Node>(toOff);
			while (pendingOn.Count != 0 || pendingOff.Count != 0)
			{
				if (pendingOn.Count != 0)
				{
					var p = pendingOn.Remove();
					var makeOff = GetOnToOff(grid, p, true);

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

					var makeOn = GetOffToOn(grid, p, true, true);

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
			}

			return null;
		}

		private ChainingTechniqueInfo? CreateCellEliminationHint(
			Grid grid, int sourceCell, Node target, IReadOnlyDictionary<int, Set<Node>> outcomes)
		{
			// Build removable nodes.
			var conclusions = new List<Conclusion>
			{
				new(target.IsOn ? Assignment : Elimination, target.Cell, target.Digit)
			};

			// Build chains.
			var chains = new Dictionary<int, Node>();
			foreach (int digit in grid.GetCandidates(sourceCell))
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

		private ChainingTechniqueInfo? CreateRegionEliminationHint(
			int region, int digit, Node target, IDictionary<int, Set<Node>> outcomes)
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
