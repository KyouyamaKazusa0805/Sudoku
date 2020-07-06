using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static Sudoku.Constants.Processings;
using static Sudoku.Constants.RegionLabel;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Encapsulates an <b>forcing chains</b> (<b>FCs</b>) technique searcher.
	/// </summary>
	[TechniqueDisplay(nameof(TechniqueCode.MultipleFc))]
	public sealed class FcTechniqueSearcher : ChainingTechniqueSearcher
	{
		/// <summary>
		/// Indicates the information.
		/// </summary>
		private readonly bool _nishio, _multiple/*, _dynamic*/;

		///// <summary>
		///// Indicates the level of the dynamic searching.
		///// </summary>
		//private readonly int _level;


		/// <summary>
		/// The temporary grid for handling dynamic and multiple forcing chains.
		/// </summary>
		private Grid _tempGrid = null!;


		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="nishio">Indicates whether the current searcher searches nishio forcing chains.</param>
		/// <param name="multiple">Indicates whether the current searcher searches multiple forcing chains.</param>
		///// <param name="dynamic">Indicates whether the current searcher searches dynamic forcing chains.</param>
		///// <param name="level">Indicates the level of the dynamic searching.</param>
		public FcTechniqueSearcher(bool nishio, bool multiple/*, bool dynamic, int level*/) =>
			(_nishio, _multiple/*, _dynamic, _level*/) = (nishio, multiple/*, dynamic, level*/);


		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 80;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			var tempAccumulator = new Bag<ChainingTechniqueInfo>();
			GetAll(tempAccumulator, grid);

			if (tempAccumulator.Count == 0)
			{
				return;
			}

			var set = new Set<ChainingTechniqueInfo>(tempAccumulator);
			set.Sort((i1, i2) =>
			{
				decimal d1 = i1.Difficulty, d2 = i2.Difficulty;
				if (d1 < d2)
				{
					return -1;
				}
				else if (d1 > d2)
				{
					return 1;
				}
				else
				{
					int l1 = i1.Complexity;
					int l2 = i2.Complexity;
					return l1 == l2 ? i1.SortKey - i2.SortKey : l1 - l2;
				}
			});

			accumulator.AddRange(set);
		}

		/// <summary>
		/// Search for chains of each type.
		/// </summary>
		/// <param name="accumulator">The accumulator.</param>
		/// <param name="grid">Thr grid.</param>
		private void GetAll(IBag<ChainingTechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			// Iterate on all empty cells.
			foreach (int cell in EmptyMap)
			{
				short mask = grid.GetCandidateMask(cell);
				int count = mask.CountSet();
				if (count > 2)
				{
					// Prepare storage and accumulator for cell eliminations.
					var valueToOn = new SudokuMap();
					var valueToOff = new SudokuMap();
					Set<Node>? cellToOn = null, cellToOff = null;

					// Iterate on all candidates that aren't alone.
					foreach (int digit in mask.GetAllSets())
					{
						// Do binary chaining (same candidate either on or off).
						var pOn = new Node(cell, digit, true);
						var pOff = new Node(cell, digit, false);
						var onToOn = new Set<Node>();
						var onToOff = new Set<Node>();
						//bool doDouble = count >= 3 && !_nishio && _dynamic, doContradiction = _dynamic || _nishio;
						//DoBinaryChaining(accumulator, grid, pOn, pOff, onToOn, onToOff, doDouble, doContradiction);

						if (_nishio)
						{
							// Do region chaining.
							DoRegionChaining(accumulator, grid, cell, digit, onToOn, onToOff);
						}

						// Collect results for cell chaining.
						foreach (var node in onToOn)
						{
							valueToOn.Add(node._cell * 9 + digit);
						}
						foreach (var node in onToOff)
						{
							valueToOff.Add(node._cell * 9 + digit);
						}
						if (cellToOn is null || cellToOff is null)
						{
							cellToOn = new Set<Node>(onToOn);
							cellToOff = new Set<Node>(onToOff);
						}
						else
						{
							cellToOn &= onToOn;
							cellToOff &= onToOff;
						}
					}

					if (_nishio)
					{
						// Do cell eliminations.
						if (count == 2 || _multiple)
						{
							if (!(cellToOn is null))
							{
								foreach (var p in cellToOn)
								{
									var hint = CreateCellEliminationHint(cell, p, valueToOn);
									if (!(hint is null))
									{
										accumulator.Add(hint);
									}
								}
							}
							if (!(cellToOff is null))
							{
								foreach (var p in cellToOff)
								{
									var hint = CreateCellEliminationHint(cell, p, valueToOff);
									if (!(hint is null))
									{
										accumulator.Add(hint);
									}
								}
							}
						}
					}
				}
			}
		}

		//private void DoBinaryChaining(
		//	IBag<ChainingTechniqueInfo> accumulator, IReadOnlyGrid grid, Node pOn, Node pOff,
		//	ISet<Node> onToOn, ISet<Node> onToOff)
		//{
		//	Node[] absurdNodes;
		//	Set<Node> offToOn = new Set<Node>(), offToOff = new Set<Node>();
		//
		//	// Circular forcing chains (hypothesis implying its negation)
		//	// are already covered by cell forcing chains, and are therefore not checked for.
		//
		//	// Test o is currectly on.
		//	onToOn.Add(pOn);
		//	absurdNodes = DoChaining(ref grid, onToOn, onToOff);
		//	if (doContradiction && !(absurdNodes is null))
		//	{
		//		// 'p' cannot hold its value, otherwise it would lead to a contradiction.
		//		var hint = CreateChainingOffHint(absurdNodes[0], absurdNodes[1], pOn, pOn, true);
		//		if (!(hint is null))
		//		{
		//			accumulator.Add(hint);
		//		}
		//	}
		//
		//	// Test p is currently off.
		//	offToOff.Add(pOff);
		//	if (doContradiction && !(absurdNodes is null))
		//	{
		//		// 'p' must hold its value otherwise it would lead to a contradiction.
		//		var hint = CreateChainingOnHint(absurdNodes[0], absurdNodes[1], pOff, pOff, true);
		//		if (!(hint is null))
		//		{
		//			accumulator.Add(hint);
		//		}
		//	}
		//
		//	if (doDouble)
		//	{
		//		// Check nodes that must be on in both case.
		//		foreach (var pFromOn in onToOn)
		//		{
		//			if (offToOn.Contains(pFromOn))
		//			{
		//				var hint = CreateChainingOnHint(pFromOn, pOn, pFromOn, false);
		//				if (!(hint is null))
		//				{
		//					accumulator.Add(hint);
		//				}
		//			}
		//		}
		//
		//		// Check nodes that must be off in both case.
		//		foreach (var pFromOn in onToOff)
		//		{
		//			if (offToOff.Contains(pFromOn))
		//			{
		//				var hint = CreateChainingOffHint(pFromOn, pFromOn, pOff, pFromOn, false);
		//				if (!(hint is null))
		//				{
		//					accumulator.Add(hint);
		//				}
		//			}
		//		}
		//	}
		//}

		private void DoRegionChaining(
			IBag<ChainingTechniqueInfo> accumulator, IReadOnlyGrid grid, int cell, int digit,
			ISet<Node> onToOn, ISet<Node> onToOff)
		{
			for (var label = Block; label <= Column; label++)
			{
				int region = GetRegion(cell, label);
				var worthMap = CandMaps[digit] & RegionMaps[region];
				if (worthMap.Count == 2 || _multiple && worthMap.Count > 2)
				{
					int firstCell = worthMap.SetAt(0);

					// Determine whether we meet this region for the first time.
					if (firstCell == cell)
					{
						SudokuMap posToOn = new SudokuMap(), posToOff = new SudokuMap();
						Set<Node> regionToOn = new Set<Node>(), regionToOff = new Set<Node>();

						// Iterate on node positions within the region.
						foreach (int otherCell in worthMap)
						{
							if (otherCell == cell)
							{
								foreach (var node in onToOn)
								{
									posToOn.Add(otherCell * 9 + node.Digit);
								}
								foreach (var node in onToOff)
								{
									posToOff.Add(otherCell * 9 + node.Digit);
								}
								regionToOn |= onToOn;
								regionToOff |= onToOff;
							}
							else
							{
								var other = new Node(otherCell, digit, true);
								Set<Node> otherToOn = new Set<Node> { other }, otherToOff = new Set<Node>();
								DoChaining(ref grid, otherToOn, otherToOff);
								foreach (var node in otherToOn)
								{
									posToOn.Add(otherCell * 9 + node.Digit);
								}
								foreach (var node in otherToOff)
								{
									posToOff.Add(otherCell * 9 + node.Digit);
								}

								regionToOn &= otherToOn;
								regionToOff &= otherToOff;
							}
						}

						// Gather results.
						foreach (var p in regionToOn)
						{
							var hint = CreateRegionEliminationHint(region, digit, p, posToOn);
							if (!(hint is null))
							{
								accumulator.Add(hint);
							}
						}
						foreach (var p in regionToOff)
						{
							var hint = CreateRegionEliminationHint(region, digit, p, posToOff);
							if (!(hint is null))
							{
								accumulator.Add(hint);
							}
						}
					}
				}
			}
		}

		private Node[]? DoChaining(ref IReadOnlyGrid grid, ISet<Node> toOn, ISet<Node> toOff)
		{
			_tempGrid = grid.Clone();
			try
			{
				var pendingOn = new Set<Node>(toOn);
				var pendingOff = new Set<Node>(toOff);
				while (pendingOn.Count != 0 || pendingOff.Count != 0)
				{
					if (pendingOn.Count != 0)
					{
						var p = pendingOn.Remove();

						var makeOff = GetOnToOff(grid, p, !_nishio);
						foreach (var pOff in makeOff)
						{
							var pOn = new Node(pOff._cell, pOff.Digit, true); // Conjugate
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

						var makeOn = GetOffToOn(grid, p/*, _tempGrid, toOff*/, !_nishio, true);
						//if (_dynamic)
						//{
						//	p.Off(grid);
						//}

						foreach (var pOn in makeOn)
						{
							var pOff = new Node(pOn._cell, pOn.Digit, false); // Conjugate.
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

					//if (pendingOn.Count != 0 && pendingOff.Count != 0 && _level > 0)
					//{
					//	foreach (var pOff in GetAdvanceedNodes(grid, _tempGrid, toOff))
					//	{
					//		if (!toOff.Contains(pOff))
					//		{
					//			// Not processed yet.
					//			toOff.Add(pOff);
					//			pendingOff.Add(pOff);
					//		}
					//	}
					//}
				}

				return null;
			}
			//catch (SudokuRuntimeException)
			//{
			//}
			finally
			{
				grid = _tempGrid;
			}
		}

		private ChainingTechniqueInfo? CreateCellEliminationHint(int cell, Node p, SudokuMap valueToOn)
		{
		}

		private ChainingTechniqueInfo? CreateRegionEliminationHint(int region, int digit, Node p, SudokuMap posToOn)
		{
		}
	}
}
