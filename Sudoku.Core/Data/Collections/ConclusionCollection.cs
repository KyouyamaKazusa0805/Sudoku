using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Extensions;
using System.Linq;
using System.Text;
using Sudoku.DocComments;
using static Sudoku.Data.ConclusionType;

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
		/// Initializes an instance with one conclusion.
		/// </summary>
		/// <param name="conclusion">(<see langword="in"/> parameter) The conclusion.</param>
		public unsafe ConclusionCollection(in Conclusion conclusion)
		{
			void* ptr = stackalloc[] { conclusion };
			_collection = new(ptr, 1);
		}

		/// <summary>
		/// Initializes an instance with the specified collection.
		/// </summary>
		/// <param name="collection">(<see langword="in"/> parameter) The collection.</param>
		public ConclusionCollection(in Span<Conclusion> collection) : this() => _collection = collection;

		/// <summary>
		/// Initializes an instance with the specified collection.
		/// </summary>
		/// <param name="collection">The collection.</param>
		public ConclusionCollection(IEnumerable<Conclusion> collection) : this() =>
			_collection = collection.ToArray().AsSpan();


		/// <summary>
		/// Indicates all cells used in this conclusions list.
		/// </summary>
		public Cells Cells
		{
			get
			{
				var result = Cells.Empty;
				foreach (var conclusion in _collection)
				{
					result.AddAnyway(conclusion.Cell);
				}

				return result;
			}
		}

		/// <summary>
		/// Indicates all digits used in this conclusions list, represented as a mask.
		/// </summary>
		public short Digits
		{
			get
			{
				short result = 0;
				foreach (var conclusion in _collection)
				{
					result |= (short)(1 << conclusion.Digit);
				}

				return result;
			}
		}


		/// <inheritdoc cref="object.Equals(object?)"/>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Equals(object? obj) => false;

		/// <inheritdoc cref="IValueEquatable{TStruct}.Equals(in TStruct)"/>
		public bool Equals(in ConclusionCollection other) => _collection == other._collection;

		/// <inheritdoc cref="object.GetHashCode"/>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override int GetHashCode()
		{
			int result = 0;
			foreach (var conclusion in _collection)
			{
				result ^= 123574689 ^ conclusion.GetHashCode();
			}

			return result;
		}

		/// <inheritdoc cref="object.ToString"/>
		public override string ToString() => ToString(true, ", ");

		/// <summary>
		/// Converts the current instance to <see cref="string"/> with the specified separator.
		/// </summary>
		/// <param name="shouldSort">Indicates whether the specified collection should be sorted first.</param>
		/// <param name="separator">The separator.</param>
		/// <returns>The string result.</returns>
		public string ToString(bool shouldSort, string separator)
		{
			return _collection.Length switch
			{
				0 => string.Empty,
				1 => _collection[0].ToString(),
				_ => internalToString(_collection)
			};

			string internalToString(in Span<Conclusion> collection)
			{
				var conclusions = collection.ToArray();
				var sb = new StringBuilder();
				if (shouldSort)
				{
					static int cmp(in Conclusion left, in Conclusion right)
					{
						var (t1, c1, d1) = left;
						var (t2, c2, d2) = right;
						if (t1 > t2) return 1;
						if (t1 < t2) return -1;
						if (d1 > d2) return 1;
						if (d1 < d2) return -1;
						return 0;
					}

					unsafe
					{
						conclusions.Sort(&cmp);
					}

					var selection = from conclusion in conclusions
									group conclusion by conclusion.ConclusionType;
					bool hasOnlyOneType = selection.HasOnlyOneElement();
					foreach (var typeGroup in selection)
					{
						string op = typeGroup.Key == Assignment ? " = " : " <> ";
						foreach (var digitGroup in
							from conclusion in typeGroup
							group conclusion by conclusion.Digit)
						{
							sb
								.Append(new Cells(from conclusion in digitGroup select conclusion.Cell).ToString())
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

					if (!hasOnlyOneType)
					{
						sb.RemoveFromEnd(separator.Length);
					}
				}
				else
				{
					static string? converter(in Conclusion conc, in string? separator) => $"{conc}{separator}";
					unsafe
					{
						sb
							.AppendRange<Conclusion, string?, string?>(conclusions, &converter, separator)
							.RemoveFromEnd(separator.Length);
					}
				}

				return sb.ToString();
			}
		}


		/// <inheritdoc cref="Operators.operator =="/>
		public static bool operator ==(in ConclusionCollection left, in ConclusionCollection right) =>
			left.Equals(right);

		/// <inheritdoc cref="Operators.operator !="/>
		public static bool operator !=(in ConclusionCollection left, in ConclusionCollection right) =>
			!(left == right);
	}
}
