namespace Sudoku.Solving.Manual;

/// <summary>
/// Provides methods that do checking during chain searching.
/// </summary>
public static class ChainingHelper
{
	/// <summary>
	/// Get extra difficulty rating for a chain node sequence.
	/// </summary>
	/// <param name="length">The length.</param>
	/// <returns>The difficulty.</returns>
	public static decimal GetExtraDifficultyByLength(this int length)
	{
		decimal added = 0;
		int ceil = 4;
		for (bool isOdd = false; length > ceil; isOdd = !isOdd)
		{
			added += .1M;
			ceil = isOdd ? ceil * 4 / 3 : ceil * 3 / 2;
		}

		return added;
	}

	/// <summary>
	/// Converts all cells to the links that is used in drawing ULs or Reverse BUGs.
	/// </summary>
	/// <param name="this">The list of cells.</param>
	/// <param name="offset">The offset. The default value is 4.</param>
	/// <returns>All links.</returns>
	public static IReadOnlyList<Link> GetLinks(this IReadOnlyList<int> @this, int offset = 4)
	{
		var result = new List<Link>();

		for (int i = 0, length = @this.Count - 1; i < length; i++)
		{
			result.Add(new(@this[i] * 9 + offset, @this[i + 1] * 9 + offset, LinkType.Line));
		}

		result.Add(new(@this[^1] * 9 + offset, @this[0] * 9 + offset, LinkType.Line));

		return result;
	}

	/// <summary>
	/// Get all available weak links.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="p">The current node.</param>
	/// <param name="yEnabled">Indicates whether the Y-Chains are enabled.</param>
	/// <returns>All possible weak links.</returns>
	public static ISet<Node> GetOnToOff(in SudokuGrid grid, in Node p, bool yEnabled)
	{
		var result = new Set<Node>();

		if (yEnabled)
		{
			// First rule: Other candidates for this cell get off.
			for (int digit = 0; digit < 9; digit++)
			{
				if (digit != p.Digit && grid.Exists(p.Cell, digit) is true)
				{
					result.Add(new(p.Cell, digit, false, p));
				}
			}
		}

		// Second rule: Other positions for this digit get off.
		for (var label = RegionLabel.Block; label <= RegionLabel.Column; label++)
		{
			int region = p.Cell.ToRegion(label);
			for (int pos = 0; pos < 9; pos++)
			{
				int cell = RegionCells[region][pos];
				if (cell != p.Cell && grid.Exists(cell, p.Digit) is true)
				{
					result.Add(new(cell, p.Digit, false, p));
				}
			}
		}

		return result;
	}

	/// <summary>
	/// Get all available strong links.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="p">The current node.</param>
	/// <param name="xEnabled">Indicates whether the X-Chains are enabled.</param>
	/// <param name="yEnabled">Indicates whether the Y-Chains are enabled.</param>
	/// <param name="enableFastProperties">
	/// Indicates whether the caller has enabled <see cref="FastProperties"/>.
	/// </param>
	/// <param name="source">The source grid.</param>
	/// <param name="offNodes">All off nodes.</param>
	/// <param name="isDynamic">
	/// Indicates whether the current searcher is searching for dynamic chains. If so,
	/// we can't use those static properties to optimize the performance.
	/// </param>
	/// <returns>All possible strong links.</returns>
	public static ISet<Node> GetOffToOn(
		in SudokuGrid grid,
		in Node p,
		bool xEnabled,
		bool yEnabled,
		bool enableFastProperties,
		in SudokuGrid? source = null,
		ISet<Node>? offNodes = null,
		bool isDynamic = false
	)
	{
		var result = new Set<Node>();
		if (yEnabled)
		{
			// First rule: If there's only two candidates in this cell, the other one gets on.
			short mask = (short)(grid.GetCandidates(p.Cell) & ~(1 << p.Digit));
			if (g(grid, p.Cell, isDynamic, enableFastProperties) && IsPow2(mask))
			{
				var pOn = new Node(p.Cell, TrailingZeroCount(mask), true, p);

				if (source is { } sourceCell && offNodes is not null)
				{
					AddHiddenParentsOfCell(ref pOn, grid, sourceCell, offNodes);
				}

				result.Add(pOn);
			}
		}

		if (xEnabled)
		{
			// Second rule: If there's only two positions for this candidate, the other ont gets on.
			for (var label = RegionLabel.Block; label <= RegionLabel.Column; label++)
			{
				int region = p.Cell.ToRegion(label);
				var cells = new Cells(
					h(grid, p.Digit, region, isDynamic, enableFastProperties) & RegionMaps[region]
				) { ~p.Cell };
				if (cells.Count == 1)
				{
					var pOn = new Node(cells[0], p.Digit, true, p);

					if (source is { } sourceCell && offNodes is not null)
					{
						AddHiddenParentsOfRegion(ref pOn, grid, sourceCell, label, offNodes);
					}

					result.Add(pOn);
				}
			}
		}

		return result;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool g(in SudokuGrid grid, int cell, bool isDynamic, bool enableFastProperties) =>
			isDynamic
			? PopCount((uint)grid.GetCandidates(cell)) == 2
			: enableFastProperties ? BivalueMap.Contains(cell) : grid.BivalueCells.Contains(cell);

		static Cells h(in SudokuGrid grid, int digit, int region, bool isDynamic, bool enableFastProperties)
		{
			if (!isDynamic)
			{
				// If not dynamic chains, we can use this property to optimize performance.
				return enableFastProperties ? CandMaps[digit] : grid.CandidateMap[digit];
			}

			var result = Cells.Empty;
			for (int i = 0; i < 9; i++)
			{
				int cell = RegionCells[region][i];
				if (grid.Exists(cell, digit) is true)
				{
					result.AddAnyway(cell);
				}
			}

			return result;
		}
	}

	/// <summary>
	/// Add hidden parents of a cell.
	/// </summary>
	/// <param name="p">The node.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="source">The source grid.</param>
	/// <param name="offNodes">All off nodes.</param>
	/// <exception cref="Exception">
	/// Throws when the parent node of the specified node cannot be found.
	/// </exception>
	private static void AddHiddenParentsOfCell(
		ref Node p,
		in SudokuGrid grid,
		in SudokuGrid source,
		ISet<Node> offNodes
	)
	{
		foreach (int digit in (short)(source.GetCandidates(p.Cell) & ~grid.GetCandidates(p.Cell)))
		{
			// Add a hidden parent.
			var parent = new Node(p.Cell, digit, false);
			(p.Parents ??= new List<Node>()).Add(
				offNodes.Contains(parent) ? parent : throw new("Parent node can't be found.")
			);
		}
	}

	/// <summary>
	/// Add hidden parents of a region.
	/// </summary>
	/// <param name="p">The node.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="source">The source grid.</param>
	/// <param name="currRegion">The current region label.</param>
	/// <param name="offNodes">All off nodes.</param>
	/// <exception cref="Exception">
	/// Throws when the parent node of the specified node cannot be found.
	/// </exception>
	private static void AddHiddenParentsOfRegion(
		ref Node p, in SudokuGrid grid,
		in SudokuGrid source,
		RegionLabel currRegion,
		ISet<Node> offNodes
	)
	{
		int region = p.Cell.ToRegion(currRegion);
		foreach (int pos in (short)(m(source, p.Digit, region) & ~m(grid, p.Digit, region)))
		{
			// Add a hidden parent.
			var parent = new Node(RegionCells[region][pos], p.Digit, false);
			(p.Parents ??= new List<Node>()).Add(
				offNodes.Contains(parent) ? parent : throw new("Parent node can't be found.")
			);
		}

		static short m(in SudokuGrid grid, int digit, int region)
		{
			short result = 0;
			for (int i = 0; i < 9; i++)
			{
				if (grid.Exists(RegionCells[region][i], digit) is true)
				{
					result |= (short)(1 << i);
				}
			}

			return result;
		}
	}
}
