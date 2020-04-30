using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Sudoku.Extensions;

namespace Sudoku.Data.Collections
{
	/// <summary>
	/// Indicates the cell collection.
	/// </summary>
	public readonly ref struct CellCollection
	{
		/// <summary>
		/// The inner map.
		/// </summary>
		private readonly GridMap _map;


		/// <summary>
		/// Initializes an instance with the specified cells.
		/// </summary>
		/// <param name="cells">The cells.</param>
		public CellCollection(params int[] cells) : this((IEnumerable<int>)cells) { }

		/// <summary>
		/// Initializes an instance with the specified cells.
		/// </summary>
		/// <param name="cells">The cells.</param>
		public CellCollection(ReadOnlySpan<int> cells)
		{
			_map = GridMap.Empty;
			foreach (int cell in cells)
			{
				_map.Add(cell);
			}
		}

		/// <summary>
		/// Initializes an instance with the specified cells.
		/// </summary>
		/// <param name="cells">The cells.</param>
		public CellCollection(IEnumerable<int> cells)
		{
			_map = GridMap.Empty;
			foreach (int cell in cells)
			{
				_map.Add(cell);
			}
		}


		/// <summary>
		/// Indicates the number of cells in this collection.
		/// </summary>
		public int Count => _map.Count;

		/// <summary>
		/// Indicates all the cells in this collection.
		/// </summary>
		public IEnumerable<int> Cells => _map.Offsets;


		/// <inheritdoc/>
		/// <exception cref="NotSupportedException">Always throws.</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Equals(object? obj) => throw Throwing.RefStructNotSupported;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(CellCollection other) => _map == other._map;

		/// <inheritdoc/>
		/// <exception cref="NotSupportedException">Always throws.</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override int GetHashCode() => throw Throwing.RefStructNotSupported;

		/// <inheritdoc/>
		public override string ToString()
		{
			string separator = ", ";
			var sb = new StringBuilder();
			foreach (int cell in Cells)
			{
				sb.Append($"{cell}{separator}");
			}

			return sb.RemoveFromEnd(separator.Length).ToString();
		}

		/// <summary>
		/// Get the enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		public IEnumerator<int> GetEnumerator() => Cells.GetEnumerator();


		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(CellCollection left, CellCollection right) => left.Equals(right);

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(CellCollection left, CellCollection right) => !(left == right);
	}
}
