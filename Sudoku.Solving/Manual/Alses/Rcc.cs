using System;
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
		public Rcc(Als als1, Als als2, int commonDigit) =>
			(Als1, Als2, CommonDigit) = (als1, als2, commonDigit);


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


		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="als1">(<see langword="out"/> parameter) The ALS 1.</param>
		/// <param name="als2">(<see langword="out"/> parameter) The ALS 2.</param>
		/// <param name="commonDigit">
		/// (<see langword="out"/> parameter) The common digit.
		/// </param>
		public void Deconstruct(out Als als1, out Als als2, out int commonDigit) =>
			(als1, als2, commonDigit) = (Als1, Als2, CommonDigit);

		/// <inheritdoc/>
		public override bool Equals(object? obj) => obj is Rcc comparer && Equals(comparer);

		/// <inheritdoc/>
		public bool Equals(Rcc other) =>
			Als1 == other.Als1 && Als2 == other.Als2 && CommonDigit == other.CommonDigit;

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="GetHashCode"]'/>
		public override int GetHashCode() => base.GetHashCode();

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="__noparam"]'/>
		public override string ToString() => $"{CommonDigit + 1} in {Als1} and {Als2}";


		/// <summary>
		/// Get the common digit that two ALSes share.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="als1">The ALS 1.</param>
		/// <param name="als2">The ALS 2.</param>
		/// <param name="digitsMask">
		/// (<see langword="out"/> parameter) The mask of appearing digits.
		/// </param>
		/// <param name="region">
		/// (<see langword="out"/> parameter) The region of the common digit shares in.
		/// </param>
		/// <returns>
		/// The digit. If the method cannot find out a digit,
		/// it will return <see langword="null"/>.
		/// </returns>
		public static int? GetCommonDigit(
			IReadOnlyGrid grid, Als als1, Als als2, out short digitsMask,
			[NotNullWhen(true)] out int? region)
		{
			foreach (int digit in (digitsMask = (short)(als1.DigitsMask | als2.DigitsMask)).GetAllSets())
			{
				if (!DigitAppears(grid, als1, digit, out var map1)
					|| !DigitAppears(grid, als2, digit, out var map2)
					|| !((GridMap)(map1 | map2)).AllSetsAreInOneRegion(out region))
				{
					continue;
				}

				return digit;
			}

			return region = null;
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
			var (region, relativePos, _) = als;

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
