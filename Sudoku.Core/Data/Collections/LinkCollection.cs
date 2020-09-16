using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
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
		/// The pointer to point <see cref="_collection"/>.
		/// If the constructor isn't <see cref="LinkCollection(Link)"/>,
		/// the field is keep the value <see cref="IntPtr.Zero"/>.
		/// </summary>
		/// <seealso cref="_collection"/>
		/// <seealso cref="LinkCollection(Link)"/>
		/// <seealso cref="IntPtr.Zero"/>
		private readonly IntPtr _ptr;

		/// <summary>
		/// The internal collection.
		/// </summary>
		private readonly Span<Link> _collection;


		/// <summary>
		/// Initializes an instance with one link.
		/// </summary>
		/// <param name="link">The chain link.</param>
		public unsafe LinkCollection(Link link)
		{
			var tempSpan = new Span<Link>((_ptr = Marshal.AllocHGlobal(sizeof(Link))).ToPointer(), 1);
			tempSpan[0] = link;
			_collection = tempSpan;
		}

		/// <summary>
		/// Initializes an instance with the specified collection.
		/// </summary>
		/// <param name="collection">The collection.</param>
		public LinkCollection(Span<Link> collection) : this() => _collection = collection;

		/// <summary>
		/// Initializes an instance with the specified collection.
		/// </summary>
		/// <param name="collection">The collection.</param>
		public LinkCollection(IEnumerable<Link> collection) : this() => _collection = collection.ToArray().AsSpan();

		/// <summary>
		/// To dispose this link (frees the unmanaged memory).
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

		/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
		public bool Equals(LinkCollection other) => _collection == other._collection;

		/// <inheritdoc cref="object.GetHashCode"/>
		/// <exception cref="NotSupportedException">Always throws.</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DoesNotReturn]
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
		public static bool operator ==(LinkCollection left, LinkCollection right) => left.Equals(right);

		/// <inheritdoc cref="Operators.operator !="/>
		public static bool operator !=(LinkCollection left, LinkCollection right) => !(left == right);
	}
}
