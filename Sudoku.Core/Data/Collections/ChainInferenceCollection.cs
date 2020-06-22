using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Sudoku.Constants;
using Sudoku.Solving.Manual.Chaining;

namespace Sudoku.Data.Collections
{
	/// <summary>
	/// Provides a collection that contains the chain inferences.
	/// </summary>
	public readonly ref struct ChainInferenceCollection
	{
		/// <summary>
		/// The pointer to point <see cref="_collection"/>.
		/// If the constructor isn't <see cref="ChainInferenceCollection(ChainInference)"/>,
		/// the field is keep the value <see cref="IntPtr.Zero"/>.
		/// </summary>
		/// <seealso cref="_collection"/>
		/// <seealso cref="ChainInferenceCollection(ChainInference)"/>
		/// <seealso cref="IntPtr.Zero"/>
		private readonly IntPtr _ptr;

		/// <summary>
		/// The internal collection.
		/// </summary>
		private readonly Span<ChainInference> _collection;


		/// <summary>
		/// Initializes an instance with one inference.
		/// </summary>
		/// <param name="chainInference">The chain inference.</param>
		public unsafe ChainInferenceCollection(ChainInference chainInference)
		{
			var tempSpan = new Span<ChainInference>((_ptr = Marshal.AllocHGlobal(sizeof(ChainInference))).ToPointer(), 1);
			tempSpan[0] = chainInference;
			_collection = tempSpan;
		}

		/// <summary>
		/// Initializes an instance with the specified collection.
		/// </summary>
		/// <param name="collection">The collection.</param>
		public ChainInferenceCollection(Span<ChainInference> collection) : this() => _collection = collection;

		/// <summary>
		/// Initializes an instance with the specified collection.
		/// </summary>
		/// <param name="collection">The collection.</param>
		public ChainInferenceCollection(IEnumerable<ChainInference> collection) : this() =>
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

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="Equals" and @paramType="object"]'/>
		/// <exception cref="NotSupportedException">Always throws.</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DoesNotReturn]
		public override bool Equals(object? obj) => throw Throwings.RefStructNotSupported;

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="Equals" and @paramType="__any"]'/>
		public bool Equals(ChainInferenceCollection other) => _collection == other._collection;

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
					var (digit, startMap) = inferences[0].Start;
					var sb = new StringBuilder(new CellCollection(startMap).ToString());
					for (int i = 0, length = inferences.Length; i < length; i++)
					{
						var (_, startIsOn, (endDigit, endMap), endIsOn) = inferences[i];
						if (digit != endDigit)
						{
							sb.Append($"({digit + 1})");
						}

						sb
							.Append(
								(startIsOn, endIsOn) switch
								{
									(true, true) => " => ",
									(true, false) => " -- ",
									(false, true) => " == ",
									(false, false) => " -> "
								})
							.Append(new CellCollection(endMap).ToString());

						digit = endDigit; // Replacement.
					}

					return sb.Append($"({inferences[^1].End.Digit + 1})").ToString();
				}
			}
		}


		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(ChainInferenceCollection left, ChainInferenceCollection right) => left.Equals(right);

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(ChainInferenceCollection left, ChainInferenceCollection right) => !(left == right);
	}
}
