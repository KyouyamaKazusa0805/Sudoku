using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.Extensions;
using Grid = Sudoku.Data.SudokuGrid;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.ConclusionType;
using System.Extensions;
using Sudoku.Data.Extensions;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Encapsulates an <b>forcing chains</b> (<b>FCs</b>) technique searcher.
	/// </summary>
	public class FcStepSearcher : ChainingStepSearcher
	{
		/// <summary>
		/// Indicates the information.
		/// </summary>
		protected readonly bool IsNishio, IsMultiple, IsDynamic;


		/// <summary>
		/// Indicates the level of the searching depth.
		/// </summary>
		protected int Level;

		/// <summary>
		/// Indicates the grid that is used in processing.
		/// </summary>
		private Grid _savedGrid;


		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="nishio">Indicates whether the searcher should search for nishio chains.</param>
		/// <param name="multiple">Indicates whether the searcher should search for multiple chains.</param>
		/// <param name="dynamic">Indicates whether the searcher should search for dynamic chains.</param>
		public FcStepSearcher(bool nishio, bool multiple, bool dynamic)
		{
			IsNishio = nishio;
			IsMultiple = multiple;
			IsDynamic = dynamic;
			Level = 0;
		}


		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(80, nameof(TechniqueCode.RegionFc))
		{
			DisplayLevel = 3
		};


		/// <inheritdoc/>
		public sealed override void GetAll(IList<StepInfo> accumulator, in Grid grid)
		{
			var tempGrid = grid;
			var tempAccumulator = new List<ChainingStepInfo>();
			GetAll(tempAccumulator, ref tempGrid);

			if (tempAccumulator.Count == 0)
			{
				return;
			}

			accumulator.AddRange(SortInfo(tempAccumulator));
		}

		/// <summary>
		/// Get all advanced nodes.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="source">(<see langword="in"/> parameter) The source.</param>
		/// <param name="offNodes">All nodes that is off.</param>
		/// <returns>All nodes.</returns>
		protected virtual IEnumerable<Node>? Advanced(in Grid grid, in Grid source, Set<Node> offNodes) => null;

		/// <summary>
		/// Search for chains of each type.
		/// </summary>
		/// <param name="accumulator">The accumulator.</param>
		/// <param name="grid">(<see langword="ref"/> parameter) The grid.</param>
		private void GetAll(IList<ChainingStepInfo> accumulator, ref Grid grid)
		{
			// Iterate on empty cells.
			foreach (int cell in EmptyMap)
			{
				short mask = grid.GetCandidates(cell);
				int count = mask.PopCount();
				switch (count)
				{
					case > 2:
					case > 1 when IsDynamic:
					{
						// Prepare storage and accumulator for cell eliminations.
						Dictionary<int, Set<Node>> valueToOn = new(), valueToOff = new();
						Set<Node>? cellToOn = null, cellToOff = null;

						// Iterate on all candidates that aren't alone.
						foreach (int digit in mask)
						{
							Node pOn = new(cell, digit, true), pOff = new(cell, digit, false);
							Set<Node> onToOn = new(), onToOff = new();

							bool doDouble = count >= 3 && !IsNishio && IsDynamic;
							bool doContradiction = IsDynamic || IsNishio;

							DoBinaryChaining(
								accumulator, ref grid, pOn, pOff, onToOn, onToOff, doDouble, doContradiction);

							if (!IsNishio)
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
						if (!IsNishio && (count == 2 || IsMultiple))
						{
							if (cellToOn is not null)
							{
								foreach (var p in cellToOn)
								{
									var hint = CreateCellFcHint(grid, cell, p, valueToOn);
									accumulator.Add(hint);
								}
							}
							if (cellToOff is not null)
							{
								foreach (var p in cellToOff)
								{
									var hint = CreateCellFcHint(grid, cell, p, valueToOff);
									accumulator.Add(hint);
								}
							}
						}
						break;
					}
				}
			}
		}

		/// <summary>
		/// Do binary chaining.
		/// </summary>
		/// <param name="accumulator">The current accumulator.</param>
		/// <param name="grid">(<see langword="ref"/> parameter) The grid.</param>
		/// <param name="pOn">(<see langword="in"/> parameter) The on node.</param>
		/// <param name="pOff">(<see langword="in"/> parameter) The off node.</param>
		/// <param name="onToOn">The list for <c>on</c> nodes to <c>on</c> nodes.</param>
		/// <param name="onToOff">The list for <c>on</c> nodes to <c>off</c> nodes.</param>
		/// <param name="doReduction">Indicates whether the method executes double chaining.</param>
		/// <param name="doContradiction">Indicates whether the method executes contradiction chaining.</param>
		private void DoBinaryChaining(
			IList<ChainingStepInfo> accumulator, ref Grid grid, in Node pOn, in Node pOff,
			Set<Node> onToOn, Set<Node> onToOff, bool doReduction, bool doContradiction)
		{
			Set<Node> offToOn = new(), offToOff = new();

			// Circular Forcing Chains (hypothesis implying its negation)
			// are already covered by Cell Forcing Chains, and are therefore
			// not checked for.

			// Test 'p' is on.
			onToOn.Add(pOn);
			var absurdNodes = DoChaining(ref grid, onToOn, onToOff);
			if (doContradiction && absurdNodes is { On: var on1, Off: var off1 })
			{
				// 'p' can't hold its value, otherwise it'd lead to a contradiction.
				var hint = CreateChainingOffHint(on1, off1, pOn, pOn, true);
				accumulator.Add(hint);
			}

			// Test 'p' is off.
			offToOff.Add(pOff);
			absurdNodes = DoChaining(ref grid, offToOn, offToOff);
			if (doContradiction && absurdNodes is { On: var on2, Off: var off2 })
			{
				// 'p' must hold its value, otherwise it'd lead to a contradiction.
				var hint = CreateChainingOnHint(on2, off2, pOff, pOff, true);
				accumulator.Add(hint);
			}

			if (doReduction)
			{
				// Check candidates that must be on in both cases.
				foreach (var pFromOn in onToOn)
				{
					if (offToOn.TryGetValue(pFromOn, out var pFromOff))
					{
						var hint = CreateChainingOnHint(pFromOn, pFromOff, pOn, pFromOn, false);
						accumulator.Add(hint);
					}
				}

				// Check candidates that must be off in both cases.
				foreach (var pFromOn in onToOff)
				{
					if (offToOff.TryGetValue(pFromOn, out var pFromOff))
					{
						var hint = CreateChainingOffHint(pFromOn, pFromOff, pOff, pFromOff, false);
						accumulator.Add(hint);
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
			IList<ChainingStepInfo> accumulator, ref Grid grid, int cell, int digit,
			Set<Node> onToOn, Set<Node> onToOff)
		{
			for (var label = RegionLabel.Block; label <= RegionLabel.Column; label++)
			{
				int region = label.ToRegion(cell);
				var worthMap = CandMaps[digit] & RegionMaps[region];
				switch (worthMap.Count)
				{
					case 2:
					case > 2 when IsMultiple:
					{
						// Determine whether we meet this region for the first time.
						if (worthMap.Offsets[0] == cell)
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
								var hint = CreateRegionFcHint(region, digit, p, posToOn);
								accumulator.Add(hint);
							}
							foreach (var p in regionToOff)
							{
								var hint = CreateRegionFcHint(region, digit, p, posToOff);
								accumulator.Add(hint);
							}
						}
						break;
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
		private (Node On, Node Off)? DoChaining(ref Grid grid, Set<Node> toOn, Set<Node> toOff)
		{
			_savedGrid = grid;

			try
			{
				Set<Node> pendingOn = new(toOn), pendingOff = new(toOff);
				while (pendingOn.Count != 0 || pendingOff.Count != 0)
				{
					if (pendingOn.Count != 0)
					{
						var p = pendingOn.RemoveAt(0);
						var makeOff = GetOnToOff(grid, p, !IsNishio);

						foreach (var pOff in makeOff)
						{
							var pOn = new Node(pOff.Cell, pOff.Digit, true); // Conjugate
							if (toOn.TryGetValue(pOn, out pOn))
							{
								// Contradiction found.
								return (pOn, pOff); // Cannot be both on and off at the same time.
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
						var p = pendingOff.RemoveAt(0);
						var makeOn = GetOffToOn(grid, p, true, !IsNishio, _savedGrid, toOff, IsDynamic);

						if (IsDynamic)
						{
							// Remove that digit.
							grid[p.Cell, p.Digit] = false;
						}

						foreach (var pOn in makeOn)
						{
							var pOff = new Node(pOn.Cell, pOn.Digit, false); // Conjugate.
							if (toOff.TryGetValue(pOff, out pOff))
							{
								// Contradiction found.
								return (pOn, pOff); // Cannot be both on and off at the same time.
							}
							else if (!toOn.Contains(pOn))
							{
								// Not processed yet.
								toOn.Add(pOn);
								pendingOn.Add(pOn);
							}
						}
					}

					// Get advanced relations (i.e. FCs (+) in Sudoku Explainer).
					if (Level > 0 && pendingOn.Count == 0 && pendingOff.Count == 0
						&& Advanced(grid, _savedGrid, toOff) is var list and not null)
					{
						foreach (var pOff in list)
						{
							if (!toOff.Contains(pOff))
							{
								// Not processed yet.
								toOff.Add(pOff);
								pendingOff.Add(pOff);
							}
						}
					}
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
		private BinaryChainingStepInfo CreateChainingOnHint(
			in Node destOn, in Node destOff, in Node source, in Node target, bool isAbsurd)
		{
			// Get views.
			var appendedInfo = new DrawingInfo(0, target.Cell * 9 + target.Digit);
			var cellOffset = new DrawingInfo[] { new(0, destOn.Cell) };
			var views = new List<View>();
			var globalCandidates = new List<DrawingInfo> { appendedInfo };
			var globalLinks = new List<Link>();

			var candidateOffsets = new List<DrawingInfo>(destOn.GetCandidateOffsets())
			{
				appendedInfo
			};
			var links = destOn.GetLinks(true);
			globalCandidates.AddRange(candidateOffsets, true);
			globalLinks.AddRange(links);
			views.Add(new()
			{
				Cells = cellOffset,
				Candidates = candidateOffsets,
				Links  = links
			});

			candidateOffsets = new List<DrawingInfo>(destOff.GetCandidateOffsets())
			{
				appendedInfo
			};
			links = destOff.GetLinks(true);
			globalCandidates.AddRange(candidateOffsets, true);
			globalLinks.AddRange(links);
			views.Add(new()
			{
				Cells = cellOffset,
				Candidates = candidateOffsets,
				Links = links
			});

			// Insert the global view at head.
			views.Insert(0, new()
			{
				Cells = cellOffset,
				Candidates = globalCandidates,
				Links = globalLinks
			});

			return new BinaryChainingStepInfo(
				new Conclusion[] { new(Assignment, target.Cell, target.Digit) },
				views,
				source,
				destOn,
				destOff,
				isAbsurd,
				IsMultiple,
				IsNishio,
				Level);
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
		private BinaryChainingStepInfo CreateChainingOffHint(
			in Node destOn, in Node destOff, in Node source, in Node target, bool isAbsurd)
		{
			// Get views.
			var cellOffset = new DrawingInfo[] { new(0, destOn.Cell) };
			var views = new List<View>();
			var globalCandidates = new List<DrawingInfo>();
			var globalLinks = new List<Link>();

			var candidateOffsets = destOn.GetCandidateOffsets();
			var links = destOn.GetLinks(true);
			globalCandidates.AddRange(candidateOffsets, true);
			globalLinks.AddRange(links);
			views.Add(new()
			{
				Cells = cellOffset,
				Candidates  = candidateOffsets.AsReadOnlyList(),
				Links = links
			});

			candidateOffsets = destOff.GetCandidateOffsets();
			links = destOff.GetLinks(true);
			globalCandidates.AddRange(candidateOffsets, true);
			globalLinks.AddRange(links);
			views.Add(new()
			{
				Cells = cellOffset,
				Candidates = candidateOffsets.AsReadOnlyList(),
				Links = links
			});

			// Insert the global view at head.
			views.Insert(0, new()
			{
				Cells = cellOffset,
				Candidates = globalCandidates,
				Links = globalLinks
			});

			return new BinaryChainingStepInfo(
				new Conclusion[] { new(Elimination, target.Cell, target.Digit) },
				views,
				source,
				destOn,
				destOff,
				isAbsurd,
				IsMultiple,
				IsNishio,
				Level);
		}

		/// <summary>
		/// Create the hint for cell forcing chains.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="sourceCell">The source cell.</param>
		/// <param name="target">(<see langword="in"/> parameter) The target elimination node.</param>
		/// <param name="outcomes">All outcomes (conclusions).</param>
		/// <returns>The information instance.</returns>
		private CellChainingStepInfo CreateCellFcHint(
			in Grid grid, int sourceCell, in Node target, IReadOnlyDictionary<int, Set<Node>> outcomes)
		{
			var (targetCandidate, targetIsOn) = target;

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
			var globalCandidates = new List<DrawingInfo> { new(0, targetCandidate) };
			var globalLinks = new List<Link>();
			foreach (var (digit, node) in chains)
			{
				var candidateOffsets = new List<DrawingInfo>(node.GetCandidateOffsets())
				{
					new(2, sourceCell * 9 + digit),
					new(0, targetCandidate)
				};

				var links = node.GetLinks(true);
				views.Add(new()
				{
					Cells = new DrawingInfo[] { new(0, sourceCell) },
					Candidates = candidateOffsets,
					Links = links
				});
				candidateOffsets.RemoveLastElement();
				globalCandidates.AddRange(candidateOffsets, true);
				globalLinks.AddRange(links);
			}

			// Insert the global view at head.
			views.Insert(0, new()
			{
				Cells  = new DrawingInfo[] { new(0, sourceCell) },
				Candidates = globalCandidates,
				Links = globalLinks
			});

			return new CellChainingStepInfo(
				new Conclusion[] { new(targetIsOn ? Assignment : Elimination, target.Cell, target.Digit) },
				views,
				sourceCell,
				chains,
				IsDynamic,
				Level);
		}

		/// <summary>
		/// Create a hint of region forcing chains.
		/// </summary>
		/// <param name="region">The region.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="target">(<see langword="in"/> parameter) The target node.</param>
		/// <param name="outcomes">All outcomes (conclusions).</param>
		/// <returns>The technique information instance.</returns>
		private RegionChainingStepInfo CreateRegionFcHint(
			int region, int digit, in Node target, IDictionary<int, Set<Node>> outcomes)
		{
			var (targetCandidate, targetIsOn) = target;

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
			var globalCandidates = new List<DrawingInfo> { new(0, targetCandidate) };
			var globalLinks = new List<Link>();
			foreach (var (cell, node) in chains)
			{
				var candidateOffsets = new List<DrawingInfo>(node.GetCandidateOffsets())
				{
					new(2, cell * 9 + digit),
					new(0, targetCandidate)
				};

				var links = node.GetLinks(true);
				views.Add(new()
				{
					Candidates = candidateOffsets,
					Regions = new DrawingInfo[] { new(0, region) },
					Links = links
				});
				candidateOffsets.RemoveLastElement();
				globalCandidates.AddRange(candidateOffsets, true);
				globalLinks.AddRange(links);
			}

			views.Insert(0, new()
			{
				Candidates = globalCandidates,
				Regions = new DrawingInfo[] { new(0, region) },
				Links = globalLinks
			});

			return new RegionChainingStepInfo(
				new Conclusion[] { new(targetIsOn ? Assignment : Elimination, target.Cell, target.Digit) },
				views,
				region,
				digit,
				chains,
				IsDynamic,
				Level);
		}
	}
}
