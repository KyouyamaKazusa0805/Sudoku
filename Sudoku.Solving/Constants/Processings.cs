using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Extensions;
using static Sudoku.Data.LinkType;

namespace Sudoku.Solving.Constants
{
	/// <summary>
	/// Provides the constants and read-only values in the current project.
	/// Of course, the class will also provides you with some method to get the results
	/// such as the chain length rating calculation.
	/// </summary>
	public static class Processings
	{
		/// <summary>
		/// The names of all subsets by their sizes.
		/// </summary>
		public static readonly string[] SubsetNames =
		{
			string.Empty, "Single", "Pair", "Triple", "Quadruple",
			"Quintuple", "Sextuple", "Septuple", "Octuple", "Nonuple"
		};

		/// <summary>
		/// The names of all fishes by their sizes.
		/// </summary>
		public static readonly string[] FishNames =
		{
			string.Empty, "Cyclopsfish", "X-Wing", "Swordfish", "Jellyfish",
			"Squirmbag", "Whale", "Leviathan", "Octopus", "Dragon"
		};

		/// <summary>
		/// The names of all regular wings by their sizes.
		/// </summary>
		public static readonly string[] RegularWingNames =
		{
			string.Empty, string.Empty, string.Empty, string.Empty, "WXYZ-Wing", "VWXYZ-Wing",
			"UVWXYZ-Wing", "TUVWXYZ-Wing", "STUVWXYZ-Wing", "RSTUVWXYZ-Wing"
		};


		/// <summary>
		/// Get the mask that is a result after the bitwise or operation processed all cells
		/// in the specified map.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="map">The map.</param>
		/// <returns>The result.</returns>
		public static short BitwiseOrMasks(IReadOnlyGrid grid, GridMap map)
		{
			short mask = 0;
			foreach (int cell in map)
			{
				mask |= grid.GetCandidateMask(cell);
			}

			return mask;
		}

		/// <summary>
		/// Get extra difficulty rating for a chain node sequence.
		/// </summary>
		/// <param name="length">The length.</param>
		/// <returns>The difficulty.</returns>
		public static decimal GetExtraDifficultyByLength(int length)
		{
			decimal added = 0;
			int ceil = 4;
			for (bool isOdd = false; length > ceil; isOdd.Flip())
			{
				added += .1M;
				ceil = isOdd ? ceil * 4 / 3 : ceil * 3 / 2;
			}
			return added;

			#region Obsolete code
			// I have seen the code of Sudoku Explainer.
			// The calculation formula (older one) is:
			//int[] steps =
			//{
			//	4, 6, 8, 12, 16, 24, 32, 48, 64, 96, 128,
			//	192, 256, 384, 512, 768, 1024, 1536, 2048,
			//	3072, 4096, 6144, 8192
			//};
			//decimal added = 0;
			//for (int index = 0; index < steps.Length && length > steps[index]; index++)
			//{
			//	added += .1M;
			//}
			//return added;
			#endregion
		}

		/// <summary>
		/// Get highlight candidate offsets through the specified target node.
		/// </summary>
		/// <param name="target">The target node.</param>
		/// <returns>The candidate offsets.</returns>
		public static IReadOnlyList<(int, int)> GetCandidateOffsets(Node target)
		{
			bool?[] map = new bool?[729];
			var result = new List<(int, int)>();
			foreach (var p in target.Chain)
			{
				if (p.ParentsCount <= 6)
				{
					// Add candidate offsets from all parents of p to p.
					for (int i = 0; i < p.ParentsCount; i++)
					{
						var pr = p[i];
						map[pr.Cells.Count == 1 ? pr._cell : pr.Cells.SetAt(0)] = pr.IsOn;
					}
				}
			}

			for (int i = 0; i < 729; i++)
			{
				bool? value = map[i];
				if (value.HasValue)
				{
					result.Add((i, value.Value ? 1 : 0));
				}
			}

			return result;
		}

		/// <summary>
		/// Get the links through the specified target node.
		/// </summary>
		/// <returns>The link.</returns>
		public static IReadOnlyList<Link> GetLinks(Node target)
		{
			var result = new List<Link>();
			foreach (var p in target.Chain)
			{
				if (p.ParentsCount <= 6)
				{
					// Add links from all parents of p to p.
					for (int i = 0; i < p.ParentsCount; i++)
					{
						var pr = p[i];
						result.Add(
							new Link(
								startCandidate: pr.Cells.Count == 1 ? pr._cell : pr.Cells.SetAt(0),
								endCandidate: p.Cells.Count == 1 ? p._cell : p.Cells.SetAt(0),
								linkType: (p.IsOn, pr.IsOn) switch
								{
									(false, true) => Strong,
									(true, false) => Weak,
									_ => Default
								}));
					}
				}
			}

			return result;
		}
	}
}
