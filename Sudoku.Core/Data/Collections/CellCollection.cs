using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Sudoku.Constants;
using Sudoku.Extensions;

namespace Sudoku.Data.Collections
{
	/// <summary>
	/// Indicates the cell collection.
	/// </summary>
	/// <remarks>
	/// Different with <see cref="GridMap"/>, the collection is immutable, which means you cannot
	/// modify any values in this collection after initialized. In addition, the output text will
	/// be different with <see cref="GridMap"/> (<see cref="GridMap"/>s use digit characters
	/// <c>0</c> or <c>1</c> to represent a bit is set or not set; however, this collection will
	/// output all cells using RCB-notation, for example, <c>r2c4</c>).
	/// </remarks>
	/// <seealso cref="GridMap"/>
	public readonly ref struct CellCollection
	{
		/// <summary>
		/// The inner map.
		/// </summary>
		private readonly GridMap _map;


		/// <summary>
		/// Initializes an instance with the specified cell.
		/// </summary>
		/// <param name="cell">The cell.</param>
		public CellCollection(int cell) : this() => _map.Add(cell);

		/// <summary>
		/// Initializes an instance with the specified cells.
		/// </summary>
		/// <param name="cells">The cells.</param>
		public CellCollection(int[] cells) : this((IEnumerable<int>)cells) { }

		/// <summary>
		/// Initializes an instance with the specified cells.
		/// </summary>
		/// <param name="cells">The cells.</param>
		public CellCollection(ReadOnlySpan<int> cells) : this() => _map.AddRange(cells);

		/// <summary>
		/// Initializes an instance with the specified cells.
		/// </summary>
		/// <param name="cells">The cells.</param>
		public CellCollection(IEnumerable<int> cells) : this() => _map.AddRange(cells);


		/// <summary>
		/// Indicates the number of cells in this collection.
		/// </summary>
		public int Count => _map.Count;


		/// <inheritdoc/>
		/// <exception cref="NotSupportedException">Always throws.</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DoesNotReturn]
		public override bool Equals(object? obj) => throw Throwings.RefStructNotSupported;

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="Equals" and @paramType="__any"]'/>
		public bool Equals(CellCollection other) => _map == other._map;

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="GetHashCode"]'/>
		/// <exception cref="NotSupportedException">Always throws.</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DoesNotReturn]
		public override int GetHashCode() => throw Throwings.RefStructNotSupported;

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="__noparam"]'/>
		public override string ToString()
		{
			if (Count == 0)
			{
				return string.Empty;
			}

			if (Count == 1)
			{
				int cell = _map.SetAt(0);
				return $"r{cell / 9 + 1}c{cell % 9 + 1}";
			}

			const string separator = ", ";
			var sbRow = new StringBuilder();
			var dic = new Dictionary<int, ICollection<int>>();
			foreach (int cell in this)
			{
				if (!dic.ContainsKey(cell / 9))
				{
					dic.Add(cell / 9, new List<int>());
				}

				dic[cell / 9].Add(cell % 9);
			}
			bool addCurlyBraces = dic.Count > 1;
			if (addCurlyBraces)
			{
				sbRow.Append("{ ");
			}
			foreach (int row in dic.Keys)
			{
				sbRow.Append($"r{row + 1}c");
				foreach (int z in dic[row])
				{
					sbRow.Append(z + 1);
				}
				sbRow.Append(separator);
			}
			sbRow.RemoveFromEnd(separator.Length);
			if (addCurlyBraces)
			{
				sbRow.Append(" }");
			}

			dic.Clear();
			var sbColumn = new StringBuilder();
			foreach (int cell in this)
			{
				if (!dic.ContainsKey(cell % 9))
				{
					dic.Add(cell % 9, new List<int>());
				}

				dic[cell % 9].Add(cell / 9);
			}
			addCurlyBraces = dic.Count > 1;
			if (addCurlyBraces)
			{
				sbColumn.Append("{ ");
			}
			foreach (int column in dic.Keys)
			{
				sbColumn.Append("r");
				foreach (int z in dic[column])
				{
					sbColumn.Append(z + 1);
				}
				sbColumn.Append($"c{column + 1}{separator}");
			}
			sbColumn.RemoveFromEnd(separator.Length);
			if (addCurlyBraces)
			{
				sbColumn.Append(" }");
			}

			return (sbRow.Length > sbColumn.Length ? sbColumn : sbRow).ToString();
		}

		/// <summary>
		/// Get the enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		public IEnumerator<int> GetEnumerator() => _map.GetEnumerator();


		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(CellCollection left, CellCollection right) => left.Equals(right);

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(CellCollection left, CellCollection right) => !(left == right);
	}
}
