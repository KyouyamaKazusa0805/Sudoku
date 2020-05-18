using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Sudoku.Data.Collections;
using Sudoku.Extensions;

namespace Sudoku.Solving.Manual.Fishes
{
	/// <summary>
	/// Provides a region map that contains 27 bits indicating whether the corresponding region is used now.
	/// </summary>
	public ref struct RegionMap
	{
		/// <summary>
		/// Initializes an instance with the specified mask.
		/// </summary>
		/// <param name="mask">The specified mask.</param>
		public RegionMap(int mask) => Mask = mask;

		/// <summary>
		/// Initializes a map with the specified regions.
		/// </summary>
		/// <param name="regions">The regions.</param>
		public RegionMap(int[] regions) : this((IEnumerable<int>)regions)
		{
		}

		/// <summary>
		/// Initializes a map with the specified regions.
		/// </summary>
		/// <param name="regions">The regions.</param>
		public RegionMap(ReadOnlySpan<int> regions) : this()
		{
			foreach (int region in regions)
			{
				Mask |= 1 << region;
			}
		}

		/// <summary>
		/// Initializes a map with the specified regions.
		/// </summary>
		/// <param name="regions">The regions.</param>
		public RegionMap(IEnumerable<int> regions) : this()
		{
			foreach (int region in regions)
			{
				Mask |= 1 << region;
			}
		}


		/// <summary>
		/// The mask.
		/// </summary>
		public int Mask { readonly get; private set; }

		/// <summary>
		/// Indicates how many regions are used now.
		/// </summary>
		public readonly int Count => Mask.CountSet();

		/// <summary>
		/// Indicates all regions used.
		/// </summary>
		public readonly IEnumerable<int> Regions => Mask.GetAllSets();


		/// <summary>
		/// Get whether the specified region is in this collection.
		/// </summary>
		/// <param name="region">The region.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		public readonly bool this[int region] => (Mask >> region & 1) != 0;


		/// <inheritdoc/>
		public override readonly bool Equals(object? obj) => throw Throwing.RefStructNotSupported;

		/// <include file='../../../GlobalDocComments.xml' path='comments/method[@name="Equals" and @paramType="__any"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(RegionMap other) => Mask == other.Mask;

		/// <include file='../../../GlobalDocComments.xml' path='comments/method[@name="GetHashCode"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode() => Mask;

		/// <summary>
		/// Get all regions, and copy them into an array.
		/// </summary>
		/// <returns>The array.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int[] ToArray() => Mask.GetAllSets().ToArray();

		/// <include file='../../../GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="__noparam"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly string ToString() => new RegionCollection(Regions).ToString();

		/// <summary>
		/// Get the enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly IEnumerator<int> GetEnumerator() => Mask.GetAllSets().GetEnumerator();

		/// <summary>
		/// Add a region.
		/// </summary>
		/// <param name="region">The region.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(int region) => Mask |= 1 << region;

		/// <summary>
		/// Add regions.
		/// </summary>
		/// <param name="regions">The regions.</param>
		public void AddRange(IEnumerable<int> regions)
		{
			foreach (int region in regions)
			{
				Add(region);
			}
		}

		/// <summary>
		/// Remove a region.
		/// </summary>
		/// <param name="region">The region.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Remove(int region) => Mask &= ~(1 << region);


		/// <include file='../../../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(RegionMap left, RegionMap right) => left.Equals(right);

		/// <include file='../../../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(RegionMap left, RegionMap right) => !(left == right);

		/// <summary>
		/// Negate the map.
		/// </summary>
		/// <param name="map"> The map.</param>
		/// <returns>The map after being negated.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RegionMap operator ~(RegionMap map) => new RegionMap(~map.Mask);

		/// <summary>
		/// Get the regions that two maps both contain.
		/// </summary>
		/// <param name="left">The left map.</param>
		/// <param name="right">The right map.</param>
		/// <returns>The result map.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RegionMap operator &(RegionMap left, RegionMap right) => new RegionMap(left.Mask & right.Mask);

		/// <summary>
		/// Get all regions that comes from two maps.
		/// </summary>
		/// <param name="left">The left map.</param>
		/// <param name="right">The right map.</param>
		/// <returns>The result map.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RegionMap operator |(RegionMap left, RegionMap right) => new RegionMap(left.Mask | right.Mask);

		/// <summary>
		/// Get the regions that two maps contain but don't overlap with each other.
		/// </summary>
		/// <param name="left">The left map.</param>
		/// <param name="right">The right map.</param>
		/// <returns>The result map.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RegionMap operator ^(RegionMap left, RegionMap right) => new RegionMap(left.Mask ^ right.Mask);

		/// <summary>
		/// Get the regions that <paramref name="left"/> contains but the <paramref name="right"/> does not contain.
		/// </summary>
		/// <param name="left">The left map.</param>
		/// <param name="right">The right map.</param>
		/// <returns>The result map.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RegionMap operator -(RegionMap left, RegionMap right) => new RegionMap(left.Mask & ~right.Mask);
	}
}
