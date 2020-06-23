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
	public readonly struct Node : IEnumerable<int>, IEquatable<Node>
	{
		/// <summary>
		/// Initializes an instance with the specified candidate.
		/// </summary>
		/// <param name="candidate">The candidate.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Node(int candidate) : this((byte)(candidate % 9), (byte)(candidate / 9))
		{
		}

		/// <summary>
		/// Initializes an instance with the specified digit and a cell.
		/// </summary>
		/// <param name="digit">The digit.</param>
		/// <param name="cell">A cell.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Node(byte digit, byte cell) : this(digit, new GridMap { cell })
		{
		}

		/// <summary>
		/// Initializes an instance with the specified digit and the map.
		/// </summary>
		/// <param name="digit">The digit.</param>
		/// <param name="map">The map.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Node(byte digit, GridMap map) : this(digit, map, NodeType.Candidate)
		{
		}

		/// <summary>
		/// Initializes an instance with the specified digit, the map and the node type.
		/// </summary>
		/// <param name="digit">The digit.</param>
		/// <param name="map">The map.</param>
		/// <param name="nodeType">The node type.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Node(byte digit, GridMap map, NodeType nodeType) => (Digit, Map, NodeType) = (digit, map, nodeType);


		/// <summary>
		/// Indicates the digit used.
		/// </summary>
		public byte Digit { get; }

		/// <summary>
		/// Indicates the node type.
		/// </summary>
		public NodeType NodeType { get; }

		/// <summary>
		/// The map of all cells used.
		/// </summary>
		public GridMap Map { get; }

		/// <summary>
		/// Indictaes the candidates map.
		/// </summary>
		public SudokuMap CandidatesMap => new SudokuMap(Map, Digit);


		/// <include file='../../../../../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="digit">(<see langword="out"/> parameter) The digit.</param>
		/// <param name="map">(<see langword="out"/> parameter) The map.</param>
		public void Deconstruct(out byte digit, out GridMap map) => (digit, map) = (Digit, Map);

		/// <include file='../../../../../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="digit">(<see langword="out"/> parameter) The digit.</param>
		/// <param name="map">(<see langword="out"/> parameter) The map.</param>
		/// <param name="fullMap">(<see langword="out"/> parameter) The candidates map.</param>
		public void Deconstruct(out byte digit, out GridMap map, out SudokuMap fullMap) =>
			(digit, map, fullMap) = (Digit, Map, CandidatesMap);

		/// <include file='../../../../../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="digit">(<see langword="out"/> parameter) The digit.</param>
		/// <param name="map">(<see langword="out"/> parameter) The map.</param>
		/// <param name="fullMap">(<see langword="out"/> parameter) The candidates map.</param>
		/// <param name="nodeType">(<see langword="out"/> parameter) The node type.</param>
		public void Deconstruct(out byte digit, out GridMap map, out SudokuMap fullMap, out NodeType nodeType) =>
			(digit, map, fullMap, nodeType) = (Digit, Map, CandidatesMap, NodeType);

		/// <include file='../../../../../GlobalDocComments.xml' path='comments/method[@name="Equals" and @paramType="object"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object? obj) => obj is Node comparer && Equals(comparer);

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(Node other) => Digit == other.Digit && Map == other.Map && NodeType == other.NodeType;

		/// <include file='../../../../../GlobalDocComments.xml' path='comments/method[@name="GetHashCode"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode() => HashCode.Combine(Digit, Map, NodeType);

		/// <include file='../../../../../GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="__noparam"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString() => $"{new CellCollection(Map).ToString()}({Digit + 1})";

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IEnumerator<int> GetEnumerator() => Map.GetEnumerator();

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


		/// <include file='../../../../../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(Node left, Node right) => left.Equals(right);

		/// <include file='../../../../../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(Node left, Node right) => !(left == right);
	}
}
