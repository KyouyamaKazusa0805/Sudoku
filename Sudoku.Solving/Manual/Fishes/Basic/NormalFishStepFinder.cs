using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Sudoku.Data.Extensions;
using Sudoku.Data.Meta;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Fishes.Basic
{
	/// <summary>
	/// Encapsulates a normal fish technique step finder in solving
	/// in <see cref="ManualSolver"/>.
	/// </summary>
	public sealed class NormalFishStepFinder : FishStepFinder
	{
		/// <inheritdoc/>
		public override IReadOnlyList<TechniqueInfo> TakeAll(Grid grid)
		{
			var result = new List<TechniqueInfo>();

			result.AddRange(TakeAllBySizeAndFinChecks(grid, 2, searchRow: false, searchFin: false));
			result.AddRange(TakeAllBySizeAndFinChecks(grid, 2, searchRow: false, searchFin: true));
			result.AddRange(TakeAllBySizeAndFinChecks(grid, 2, searchRow: true, searchFin: false));
			result.AddRange(TakeAllBySizeAndFinChecks(grid, 2, searchRow: true, searchFin: true));
			result.AddRange(TakeAllBySizeAndFinChecks(grid, 3, searchRow: false, searchFin: false));
			result.AddRange(TakeAllBySizeAndFinChecks(grid, 3, searchRow: false, searchFin: true));
			result.AddRange(TakeAllBySizeAndFinChecks(grid, 3, searchRow: true, searchFin: false));
			result.AddRange(TakeAllBySizeAndFinChecks(grid, 3, searchRow: true, searchFin: true));
			result.AddRange(TakeAllBySizeAndFinChecks(grid, 4, searchRow: false, searchFin: false));
			result.AddRange(TakeAllBySizeAndFinChecks(grid, 4, searchRow: false, searchFin: true));
			result.AddRange(TakeAllBySizeAndFinChecks(grid, 4, searchRow: true, searchFin: false));
			result.AddRange(TakeAllBySizeAndFinChecks(grid, 4, searchRow: true, searchFin: true));

			return result;
		}


		/// <summary>
		/// Searches all basic fish of the specified size and
		/// fin checking <see cref="bool"/> value.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="size">The size.</param>
		/// <param name="searchRow">
		/// Indicates the solver will searching rows or columns.
		/// </param>
		/// <param name="searchFin">Indicates the solver will searching fins.</param>
		/// <returns>The result.</returns>
		private static IReadOnlyList<NormalFishTechniqueInfo> TakeAllBySizeAndFinChecks(
			Grid grid, int size, bool searchRow, bool searchFin)
		{
			Contract.Requires(size >= 2 && size <= 4);

			var result = new List<NormalFishTechniqueInfo>();

			int regionStart = searchRow ? 9 : 18;
			for (int digit = 0; digit < 9; digit++)
			{
				for (int bs1 = regionStart; bs1 < regionStart + 10 - size; bs1++)
				{
					// Get the appearing mask of 'digit' in 'bs1'.
					short mask1 = grid.GetDigitAppearingMask(digit, bs1);
					if (mask1.CountSet() == 0)
					{
						continue;
					}

					for (int bs2 = bs1 + 1; bs2 < regionStart + 11 - size; bs2++)
					{
						// Get the appearing mask of 'digit' in 'bs2'.
						short mask2 = grid.GetDigitAppearingMask(digit, bs2);
						if (mask2.CountSet() == 0)
						{
							continue;
						}

						if (size == 2)
						{
							// TODO: Search (Finned) (Sashimi) X-Wing.
						}
						else // size > 2
						{
							for (int bs3 = 0; bs3 < regionStart + 12 - size; bs3++)
							{
								// Get the appearing mask of 'digit' in 'bs3'.
								short mask3 = grid.GetDigitAppearingMask(digit, bs3);
								if (mask3.CountSet() == 0)
								{
									continue;
								}

								if (size == 3)
								{
									// TODO: Search (Finned) (Sashimi) Swordfish.
								}
								else // size == 4
								{
									for (int bs4 = 0; bs4 < regionStart + 9; bs4++)
									{
										// Get the appearing mask of 'digit' in 'bs4'.
										short mask4 = grid.GetDigitAppearingMask(digit, bs4);
										if (mask4.CountSet() == 0)
										{
											continue;
										}

										// TODO: Search (Finned) (Sashimi) Jellyfish.
									}
								}
							}
						}
					}
				}
			}

			return result;
		}
	}
}
