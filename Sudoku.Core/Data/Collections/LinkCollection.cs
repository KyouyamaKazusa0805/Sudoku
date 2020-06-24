using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Sudoku.Constants;
using Sudoku.Extensions;

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

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="Equals" and @paramType="object"]'/>
		/// <exception cref="NotSupportedException">Always throws.</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DoesNotReturn]
		public override bool Equals(object? obj) => throw Throwings.RefStructNotSupported;

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="Equals" and @paramType="__any"]'/>
		public bool Equals(LinkCollection other) => _collection == other._collection;

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="GetHashCode"]'/>
		/// <exception cref="NotSupportedException">Always throws.</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DoesNotReturn]
		public override int GetHashCode() => throw Throwings.RefStructNotSupported;

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="__noparam"]'/>
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
					var inferences = _collection.ToArray();
					int start = inferences[0].StartCandidate, startCell = start / 9, digit = start % 9;
					var sb = new StringBuilder(new CellCollection(startCell).ToString());
					for (int i = 0, length = inferences.Length; i < length; i++)
					{
						var (_, end, type) = inferences[i];
						int endCell = end / 9, endDigit = end % 9;
						if (digit != endDigit)
						{
							sb.Append($"({digit + 1})");
						}

						sb.NullableAppend(NameAttribute.GetName(type)).Append(new CellCollection(endCell).ToString());

						digit = endDigit; // Replacement.
					}

					return sb.Append($"({inferences[^1].EndCandidate % 9 + 1})").ToString();
				}
			}
		}


		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(LinkCollection left, LinkCollection right) => left.Equals(right);

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(LinkCollection left, LinkCollection right) => !(left == right);
	}
}
