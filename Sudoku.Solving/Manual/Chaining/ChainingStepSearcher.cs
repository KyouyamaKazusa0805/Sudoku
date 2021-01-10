using Sudoku.Data;
using Sudoku.Data.Extensions;
using System.Collections.Generic;
using System.Extensions;
using System.Runtime.CompilerServices;
using static Sudoku.Constants.Tables;
using static Sudoku.Solving.Manual.FastProperties;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Encapsulates a <b>chain</b> technique searcher.
	/// </summary>
	public abstract class ChainingStepSearcher : StepSearcher
	{
		/// <summary>
		/// Get all available weak links.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="p">(<see langword="in"/> parameter) The current node.</param>
		/// <param name="yEnabled">Indicates whether the Y-Chains are enabled.</param>
		/// <returns>All possible weak links.</returns>
		protected internal static ISet<Node> GetOnToOff(in SudokuGrid grid, in Node p, bool yEnabled)
		{
			var result = new Set<Node>();

			if (yEnabled)
			{
				// First rule: Other candidates for this cell get off.
				for (int digit = 0; digit < 9; digit++)
				{
					if (digit != p.Digit && grid.Exists(p.Cell, digit) is true)
					{
						result.Add(
							new(p.Cell, digit, false, p)
#if DOUBLE_LAYERED_ASSUMPTION
							{
								Cause = Cause.NakedSingle
							}
#endif
							);
					}
				}
			}

			// Second rule: Other positions for this digit get off.
			for (var label = RegionLabel.Block; label <= RegionLabel.Column; label++)
			{
				int region = label.ToRegion(p.Cell);
				for (int pos = 0; pos < 9; pos++)
				{
					int cell = RegionCells[region][pos];
					if (cell != p.Cell && grid.Exists(cell, p.Digit) is true)
					{
						result.Add(
							new(cell, p.Digit, false, p)
#if DOUBLE_LAYERED_ASSUMPTION
							{
								Cause = label.GetRegionCause()
							}
#endif
							);
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Get all available strong links.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="p">(<see langword="in"/> parameter) The current node.</param>
		/// <param name="xEnabled">Indicates whether the X-Chains are enabled.</param>
		/// <param name="yEnabled">Indicates whether the Y-Chains are enabled.</param>
		/// <param name="isDynamic">
		/// Indicates whether the current searcher is searching for dynamic chains. If so,
		/// we can't use those static properties to optimize the performance.
		/// </param>
		/// <returns>All possible strong links.</returns>
		protected internal static ISet<Node> GetOffToOn(
			in SudokuGrid grid, in Node p, bool xEnabled, bool yEnabled,
			in SudokuGrid? source = null, ISet<Node>? offNodes = null, bool isDynamic = false)
		{
			var result = new Set<Node>();
			if (yEnabled)
			{
				// First rule: If there's only two candidates in this cell, the other one gets on.
				short mask = (short)(grid.GetCandidates(p.Cell) & ~(1 << p.Digit));
				if (g(grid, p.Cell, isDynamic) && mask.IsPowerOfTwo())
				{
					var pOn = new Node(p.Cell, mask.FindFirstSet(), true, p)
#if DOUBLE_LAYERED_ASSUMPTION
					{
						Cause = Cause.NakedSingle
					}
#endif
					;

					if (source.HasValue && offNodes is not null)
					{
						AddHiddenParentsOfCell(ref pOn, grid, source.Value, offNodes);
					}

					result.Add(pOn);
				}
			}

			if (xEnabled)
			{
				// Second rule: If there's only two positions for this candidate, the other ont gets on.
				for (var label = RegionLabel.Block; label <= RegionLabel.Column; label++)
				{
					int region = label.ToRegion(p.Cell);
					var cells = (h(grid, p.Digit, region, isDynamic) & RegionMaps[region]) - p.Cell;
					if (cells.Count == 1)
					{
						var pOn = new Node(cells[0], p.Digit, true, p)
#if DOUBLE_LAYERED_ASSUMPTION
						{
							Cause = label.GetRegionCause()
						}
#endif
						;

						if (source.HasValue && offNodes is not null)
						{
							AddHiddenParentsOfRegion(ref pOn, grid, source.Value, label, offNodes);
						}

						result.Add(pOn);
					}
				}
			}

			return result;


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static bool g(in SudokuGrid grid, int cell, bool isDynamic) =>
				isDynamic ? grid.GetCandidates(cell).PopCount() == 2 : BivalueMap.Contains(cell);

			static Cells h(in SudokuGrid grid, int digit, int region, bool isDynamic)
			{
				if (!isDynamic)
				{
					// If not dynamic chains, we can use this property to optimize performance.
					return CandMaps[digit];
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
		/// <param name="p">(<see langword="ref"/> parameter) The node.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="source">(<see langword="in"/> parameter) The source grid.</param>
		/// <param name="offNodes">All off nodes.</param>
		/// <exception cref="SudokuHandlingException">
		/// Throws when the parent node of the specified node cannot be found.
		/// </exception>
		private static void AddHiddenParentsOfCell(
			ref Node p, in SudokuGrid grid, in SudokuGrid source, ISet<Node> offNodes)
		{
			foreach (int digit in (short)(source.GetCandidates(p.Cell) & ~grid.GetCandidates(p.Cell)))
			{
				// Add a hidden parent.
				var parent = new Node(p.Cell, digit, false);
				(p.Parents ??= new List<Node>()).Add(
					offNodes.Contains(parent)
					? parent
					: throw new SudokuHandlingException(errorCode: 501));
			}
		}

		/// <summary>
		/// Add hidden parents of a region.
		/// </summary>
		/// <param name="p">(<see langword="ref"/> parameter) The node.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="source">(<see langword="in"/> parameter) The source grid.</param>
		/// <param name="currRegion">The current region label.</param>
		/// <param name="offNodes">All off nodes.</param>
		/// <exception cref="SudokuHandlingException">
		/// Throws when the parent node of the specified node cannot be found.
		/// </exception>
		private static void AddHiddenParentsOfRegion(
			ref Node p, in SudokuGrid grid, in SudokuGrid source, RegionLabel currRegion, ISet<Node> offNodes)
		{
			int region = currRegion.ToRegion(p.Cell);
			foreach (int pos in (short)(m(source, p.Digit, region) & ~m(grid, p.Digit, region)))
			{
				// Add a hidden parent.
				var parent = new Node(RegionCells[region][pos], p.Digit, false);
				(p.Parents ??= new List<Node>()).Add(
					offNodes.Contains(parent)
					? parent
					: throw new SudokuHandlingException(errorCode: 501));
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
}
