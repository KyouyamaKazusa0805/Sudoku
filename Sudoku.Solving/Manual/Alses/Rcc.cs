using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Alses
{
	/// <summary>
	/// Encapsulates a data structure for restricted common candidates (RCC).
	/// </summary>
	public readonly struct Rcc : IEquatable<Rcc>
	{
		/// <summary>
		/// Initializes an instance with two ALSes and their common digit.
		/// </summary>
		/// <param name="als1">The ALS 1.</param>
		/// <param name="als2">The ALS 2.</param>
		/// <param name="commonDigit">The common digit.</param>
		/// <param name="commonRegion">The common region.</param>
		public Rcc(Als als1, Als als2, int commonDigit, int commonRegion) =>
			(Als1, Als2, CommonDigit, CommonRegion) = (als1, als2, commonDigit, commonRegion);


		/// <summary>
		/// Indicates the ALS 1.
		/// </summary>
		public Als Als1 { get; }

		/// <summary>
		/// Indicates the ALS 2.
		/// </summary>
		public Als Als2 { get; }

		/// <summary>
		/// Indicates the digit that two ALSes share.
		/// </summary>
		public int CommonDigit { get; }

		/// <summary>
		/// Indicates the common region.
		/// </summary>
		public int CommonRegion { get; }


		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="als1">(<see langword="out"/> parameter) The ALS 1.</param>
		/// <param name="als2">(<see langword="out"/> parameter) The ALS 2.</param>
		/// <param name="commonDigit">
		/// (<see langword="out"/> parameter) The common digit.
		/// </param>
		/// <param name="commonRegion">
		/// (<see langword="out"/> parameter) The common region.
		/// </param>
		public void Deconstruct(
			out Als als1, out Als als2, out int commonDigit, out int commonRegion) =>
			(als1, als2, commonDigit, commonRegion) = (Als1, Als2, CommonDigit, CommonRegion);

		/// <inheritdoc/>
		public override bool Equals(object? obj) => obj is Rcc comparer && Equals(comparer);

		/// <inheritdoc/>
		public bool Equals(Rcc other)
		{
			return Als1 == other.Als1
				&& Als2 == other.Als2
				&& CommonDigit == other.CommonDigit
				&& CommonRegion == other.CommonRegion;
		}

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="GetHashCode"]'/>
		public override int GetHashCode() =>
			Als1.GetHashCode() ^ Als2.GetHashCode() ^ CommonDigit ^ CommonRegion;

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="__noparam"]'/>
		public override string ToString() => $"{CommonDigit + 1} in {Als1} and {Als2}";

		
		/// <summary>
		/// Get all RCCs in the specified grid.
		/// </summary>
		/// <param name="grid">The grid to check.</param>
		/// <param name="allowOverlap">
		/// Indicates whether the specified searcher allows ALSes overlapping.
		/// </param>
		/// <returns>All RCCs searched.</returns>
		public static IEnumerable<Rcc> GetAllRccs(IReadOnlyGrid grid, bool allowOverlap)
		{
			for (int r1 = 0; r1 < 26; r1++)
			{
				for (int r2 = r1 + 1; r2 < 27; r2++)
				{
					var alses1 = Als.GetAllAlses(grid, r1);
					if (!alses1.Any())
					{
						continue;
					}

					foreach (var als1 in alses1)
					{
						var alses2 = Als.GetAllAlses(grid, r2);
						if (!alses2.Any())
						{
							continue;
						}

						foreach (var als2 in alses2)
						{
							// Check whether two ALSes hold same cells.
							foreach (var (commonDigit, region) in
								GetCommonDigits(grid, als1, als2, out short digitsMask))
							{
								var overlapMap = als1.Map & als2.Map;
								if (allowOverlap && overlapMap.IsNotEmpty)
								{
									// Check overlap regions contain common digits or not.
									if (overlapMap
										.Offsets
										.Any(cell => grid.CandidateExists(cell, commonDigit)))
									{
										continue;
									}
								}

								// Now we should check elimination.
								// But firstly, we should check all digits appearing
								// in two ALSes.
								foreach (int elimDigit in digitsMask.GetAllSets())
								{
									if (elimDigit == commonDigit)
									{
										continue;
									}

									// To check whether both ALSes contain this digit.
									// If not (either containing), continue to next iteration.
									if (((als1.DigitsMask ^ als2.DigitsMask) >> elimDigit & 1) != 0)
									{
										continue;
									}

									yield return new Rcc(als1, als2, commonDigit, region);
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Get the common digit that two ALSes share.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="als1">The ALS 1.</param>
		/// <param name="als2">The ALS 2.</param>
		/// <param name="digitsMask">
		/// (<see langword="out"/> parameter) The mask of appearing digits.
		/// </param>
		/// <returns>
		/// The digit. If the method cannot find out a digit,
		/// it will return <see langword="null"/>.
		/// </returns>
		public static IEnumerable<(int _digit, int _region)> GetCommonDigits(
			IReadOnlyGrid grid, Als als1, Als als2, out short digitsMask)
		{
			var result = new List<(int, int)>();
			foreach (int digit in (digitsMask = (short)(als1.DigitsMask | als2.DigitsMask)).GetAllSets())
			{
				if (!DigitAppears(grid, als1, digit, out var map1)
					|| !DigitAppears(grid, als2, digit, out var map2)
					|| !((GridMap)(map1 | map2)).AllSetsAreInOneRegion(out int? region))
				{
					continue;
				}

				result.Add((digit, (int)region));
			}

			return result;
		}

		/// <summary>
		/// Check whether the digit appears at least once in the specified ALS.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="als">The ALS.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="cellsIfUnique">
		/// (<see langword="out"/> parameter) The cells if not empty.
		/// </param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		private static bool DigitAppears(
			IReadOnlyGrid grid, Als als, int digit, [NotNullWhen(true)] out GridMap? cellsIfUnique)
		{
			var (region, relativePos, _, _) = als;

			var map = GridMap.Empty;
			foreach (int cell in from pos in relativePos
								 select RegionUtils.GetCellOffset(region, pos))
			{
				if (grid.CandidateExists(cell, digit))
				{
					map.Add(cell);
				}
			}

			if (map.IsEmpty)
			{
				cellsIfUnique = null;
				return false;
			}
			else
			{
				cellsIfUnique = map;
				return true;
			}
		}


		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(Rcc left, Rcc right) => left.Equals(right);

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(Rcc left, Rcc right) => !(left == right);
	}
}
