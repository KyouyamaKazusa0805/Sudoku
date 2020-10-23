using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Sudoku.Constants;
using Sudoku.DocComments;
using Sudoku.Extensions;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Data.Collections
{
	/// <summary>
	/// Provides a collection that contains the conclusions.
	/// </summary>
	public readonly ref struct ConclusionCollection
	{
		/// <summary>
		/// The pointer to point <see cref="_collection"/>.
		/// If the constructor isn't <see cref="ConclusionCollection(Conclusion)"/>,
		/// the field is keep the value <see cref="IntPtr.Zero"/>.
		/// </summary>
		/// <seealso cref="_collection"/>
		/// <seealso cref="ConclusionCollection(Conclusion)"/>
		/// <seealso cref="IntPtr.Zero"/>
		private readonly IntPtr _ptr;

		/// <summary>
		/// The internal collection.
		/// </summary>
		private readonly Span<Conclusion> _collection;


		/// <summary>
		/// Initializes an instance with one conclusion.
		/// </summary>
		/// <param name="conclusion">The conclusion.</param>
		public unsafe ConclusionCollection(Conclusion conclusion)
		{
			var tempSpan = new Span<Conclusion>((_ptr = Marshal.AllocHGlobal(sizeof(Conclusion))).ToPointer(), 1);
			tempSpan[0] = conclusion;
			_collection = tempSpan;
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
		/// To dispose this instance (frees the unmanaged memory).
		/// </summary>
		public void Dispose()
		{
			if (_ptr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(_ptr);
			}
		}

		/// <inheritdoc cref="object.Equals(object?)"/>
		/// <exception cref="NotSupportedException">Always throws.</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DoesNotReturn]
		public override bool Equals(object? obj) => throw Throwings.RefStructNotSupported;

		/// <inheritdoc cref="IValueEquatable{TStruct}.Equals(in TStruct)"/>
		public bool Equals(in ConclusionCollection other) => _collection == other._collection;

		/// <inheritdoc cref="object.GetHashCode"/>
		/// <exception cref="NotSupportedException">Always throws.</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DoesNotReturn]
		public override int GetHashCode() => throw Throwings.RefStructNotSupported;

		/// <inheritdoc cref="object.ToString"/>
		public override string ToString() => ToString(true, ", ");

		/// <summary>
		/// Converts the current instance to <see cref="string"/> with the specified separator.
		/// </summary>
		/// <param name="shouldSort">Indicates whether the specified collection should be sorted first.</param>
		/// <param name="separator">The separator.</param>
		/// <returns>The string result.</returns>
		public unsafe string ToString(bool shouldSort, string separator)
		{
			return _collection.Length switch
			{
				0 => string.Empty,
				1 => _collection[0].ToString(),
				_ => internalToString(_collection)
			};

			unsafe string internalToString(in Span<Conclusion> collection)
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
					conclusions.Sort(&cmp);

					var selection = from conclusion in conclusions
									group conclusion by conclusion.ConclusionType;
					bool hasOnlyOneType = selection.HasOnlyOneElement();
					foreach (var typeGroup in selection)
					{
						string op = typeGroup.Key == Assignment ? " = " : " <> ";
						foreach (var digitGroup in
							from conclusion in typeGroup group conclusion by conclusion.Digit)
						{
							sb
								.Append(new GridMap(from conclusion in digitGroup select conclusion.Cell))
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
					static string? converter(in Conclusion conc, in string? separator) => $"{conc}{separator}";

					sb
						.AppendRange<Conclusion, string?, string?>(conclusions, &converter, separator)
						.RemoveFromEnd(separator.Length);
				}

				return sb.ToString();
			}
		}


		/// <inheritdoc cref="Operators.operator =="/>
		public static bool operator ==(in ConclusionCollection left, in ConclusionCollection right) => left.Equals(right);

		/// <inheritdoc cref="Operators.operator !="/>
		public static bool operator !=(in ConclusionCollection left, in ConclusionCollection right) => !(left == right);
	}
}
