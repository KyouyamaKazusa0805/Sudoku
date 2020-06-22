using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Collections;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Indicates a candidate pair used in chains.
	/// </summary>
	public readonly struct CandidatePair : IEnumerable<int>, IEquatable<CandidatePair>
	{
		/// <summary>
		/// Initializes an instance with the specified digit and the map.
		/// </summary>
		/// <param name="digit">The digit.</param>
		/// <param name="map">The map.</param>
		public CandidatePair(byte digit, GridMap map) => (Digit, Map) = (digit, map);


		/// <summary>
		/// Indicates the digit used.
		/// </summary>
		public byte Digit { get; }

		/// <summary>
		/// The map of all cells used.
		/// </summary>
		public GridMap Map { get; }


		/// <include file='../../../GlobalDocComments.xml' path='comments/method[@name="Equals" and @paramType="object"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object? obj) => obj is CandidatePair comparer && Equals(comparer);

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(CandidatePair other) => Digit == other.Digit && Map == other.Map;

		/// <include file='../../../GlobalDocComments.xml' path='comments/method[@name="GetHashCode"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode() => HashCode.Combine(Digit, Map);

		/// <include file='../../../GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="__noparam"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString() => $"{new CellCollection(Map).ToString()}({Digit + 1})";

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IEnumerator<int> GetEnumerator() => Map.GetEnumerator();

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


		/// <include file='../../../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(CandidatePair left, CandidatePair right) => left.Equals(right);

		/// <include file='../../../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(CandidatePair left, CandidatePair right) => !(left == right);
	}
}
