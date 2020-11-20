using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Sudoku.Constants;
using Sudoku.DocComments;

namespace Sudoku.Data.Collections
{
	/// <summary>
	/// Provides a collection that contains the chain links.
	/// </summary>
	public readonly ref struct LinkCollection
	{
		/// <summary>
		/// The internal collection.
		/// </summary>
		private readonly Span<Link> _collection;


		/// <summary>
		/// Initializes an instance with one link.
		/// </summary>
		/// <param name="link">(<see langword="in"/> parameter) The chain link.</param>
		public unsafe LinkCollection(in Link link)
		{
			void* ptr = stackalloc[] { link };
			_collection = new(ptr, 1);
		}

		/// <summary>
		/// Initializes an instance with the specified collection.
		/// </summary>
		/// <param name="collection">(<see langword="in"/> parameter) The collection.</param>
		public LinkCollection(in Span<Link> collection) : this() => _collection = collection;

		/// <summary>
		/// Initializes an instance with the specified collection.
		/// </summary>
		/// <param name="collection">The collection.</param>
		public LinkCollection(IEnumerable<Link> collection) : this() =>
			_collection = collection.ToArray().AsSpan();


		/// <inheritdoc cref="object.Equals(object?)"/>
		/// <exception cref="NotSupportedException">Always throws.</exception>
		[EditorBrowsable(EditorBrowsableState.Never), DoesNotReturn]
		public override bool Equals(object? obj) => throw Throwings.RefStructNotSupported;

		/// <inheritdoc cref="IValueEquatable{TStruct}.Equals(in TStruct)"/>
		public bool Equals(in LinkCollection other) => _collection == other._collection;

		/// <inheritdoc cref="object.GetHashCode"/>
		/// <exception cref="NotSupportedException">Always throws.</exception>
		[EditorBrowsable(EditorBrowsableState.Never), DoesNotReturn]
		public override int GetHashCode() => throw Throwings.RefStructNotSupported;

		/// <inheritdoc cref="object.ToString"/>
		public override string ToString()
		{
			switch (_collection.Length)
			{
				case 0:
				{
					return string.Empty;
				}
				case 1:
				{
					return _collection[0].ToString();
				}
				default:
				{
					var links = _collection.ToArray();
					var sb = new StringBuilder();
					for (int i = 0, length = links.Length; i < length; i++)
					{
						var (start, _, type) = links[i];
						sb
							.Append(new SudokuMap { start })
							.Append(NameAttribute.GetName(type));
					}
					sb.Append(new SudokuMap { links[^1].EndCandidate });

					// Remove redundant digit labels:
					// r1c1(1) == r1c2(1) --> r1c1 == r1c2(1).
					var list = new List<(int Pos, char Value)>();
					for (int i = 0; i < sb.Length; i++)
					{
						if (sb[i] == '(')
						{
							list.Add((i, sb[i + 1]));
							i += 2;
						}
					}

					char digit = list[^1].Value;
					for (int i = list.Count - 1; i >= 1; i--)
					{
						if (list[i - 1].Value == digit)
						{
							sb.Remove(list[i - 1].Pos, 3);
						}

						digit = list[i - 1].Value;
					}

					return sb.ToString();
				}
			}
		}


		/// <inheritdoc cref="Operators.operator =="/>
		public static bool operator ==(in LinkCollection left, in LinkCollection right) => left.Equals(right);

		/// <inheritdoc cref="Operators.operator !="/>
		public static bool operator !=(in LinkCollection left, in LinkCollection right) => !(left == right);
	}
}
