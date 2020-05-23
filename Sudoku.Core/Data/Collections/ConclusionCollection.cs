using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Sudoku.Extensions;

namespace Sudoku.Data.Collections
{
	/// <summary>
	/// Provides a collection that contains the conclusions.
	/// </summary>
	public readonly ref struct ConclusionCollection
	{
		/// <summary>
		/// The internal collection.
		/// </summary>
		private readonly Span<Conclusion> _collection;


		/// <summary>
		/// Initializes an instance with the specified collection.
		/// </summary>
		/// <param name="collection">The collection.</param>
		public ConclusionCollection(Span<Conclusion> collection) => _collection = collection;

		/// <summary>
		/// Initializes an instance with the specified collection.
		/// </summary>
		/// <param name="collection">The collection.</param>
		public ConclusionCollection(IEnumerable<Conclusion> collection) => _collection = collection.ToArray().AsSpan();


		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="Equals" and @paramType="object"]'/>
		/// <exception cref="NotSupportedException">Always throws.</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DoesNotReturn]
		public override bool Equals(object? obj) => throw Throwing.RefStructNotSupported;

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="Equals" and @paramType="__any"]'/>
		public bool Equals(ConclusionCollection other) => _collection == other._collection;

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="GetHashCode"]'/>
		/// <exception cref="NotSupportedException">Always throws.</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DoesNotReturn]
		public override int GetHashCode() => throw Throwing.RefStructNotSupported;

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="__noparam"]'/>
		public override string ToString() => ToString(true, ", ");

		/// <summary>
		/// Converts the current instance to <see cref="string"/> with the specified separator.
		/// </summary>
		/// <param name="shouldSort">Indicates whether the specified collection should be sorted first.</param>
		/// <param name="separator">The separator.</param>
		/// <returns>The string result.</returns>
		public string ToString(bool shouldSort, string separator)
		{
			if (_collection.Length == 0)
			{
				return string.Empty;
			}

			if (_collection.Length == 1)
			{
				return _collection[0].ToString();
			}

			var concs = _collection.ToArray();
			var sb = new StringBuilder();
			if (shouldSort)
			{
				Array.Sort(
					concs,
					(a, b) =>
					{
						var (t1, c1, d1) = a;
						var (t2, c2, d2) = b;
						return true switch
						{
							_ when t1 > t2 => 1,
							_ when t1 < t2 => -1,
							_ when d1 > d2 => 1,
							_ when d1 < d2 => -1,
							_ => 0
						};
					});

				var selection = from conc in concs group conc by conc.ConclusionType;
				bool hasOnlyOneType = selection.HasOnlyOneElement();
				foreach (var typeGroup in selection)
				{
					string op = typeGroup.Key == ConclusionType.Assignment ? " = " : " <> ";
					foreach (var digitGroup in from conclusion in typeGroup group conclusion by conclusion.Digit)
					{
						sb
							.Append(new CellCollection(from conc in digitGroup select conc.CellOffset).ToString())
							.Append(op)
							.Append(digitGroup.Key + 1)
							.Append(separator);
					}

					sb.RemoveFromEnd(separator.Length);
					if (!hasOnlyOneType)
					{
						sb.Append(separator);
					}
				}
			}
			else
			{
				foreach (var conc in concs)
				{
					sb.Append($"{conc}{separator}");
				}

				sb.RemoveFromEnd(separator.Length);
			}

			return sb.ToString();
		}


		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(ConclusionCollection left, ConclusionCollection right) => left.Equals(right);

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(ConclusionCollection left, ConclusionCollection right) => !(left == right);
	}
}
