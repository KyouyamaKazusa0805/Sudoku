using System;
using System.Collections;
using System.Collections.Generic;
using Sudoku.DocComments;
using Sudoku.Extensions;
#if DEBUG && false
using System.Diagnostics;
#endif

namespace Sudoku.Data
{
	/// <summary>
	/// Encapsulates a sudoku grid using value type instead of reference type.
	/// </summary>
#if DEBUG && false
	[DebuggerDisplay("{ToString(\"#0\")}")]
#endif
	public unsafe partial struct SudokuGrid : IEnumerable<short>, IEquatable<SudokuGrid>, IFormattable
	{
		/// <summary>
		/// Indicates the default mask of a cell (an empty cell, with all 9 candidates left).
		/// </summary>
		public const short DefaultMask = (short)CellStatus.Empty << 9;

		/// <summary>
		/// Indicates the maximum candidate mask that used.
		/// </summary>
		public const short MaxCandidatesMask = 0b111_111_111;

		/// <summary>
		/// Indicates the size of each grid.
		/// </summary>
		private const byte Length = 81;


		/// <summary>
		/// Indicates the empty grid string.
		/// </summary>
		public static readonly string EmptyString = new('0', Length);

		/// <summary>
		/// Indicates the default grid that all values are initialized 0, which is same as <see cref="SudokuGrid()"/>.
		/// </summary>
		/// <remarks>
		/// We recommend you should use <see cref="Undefined"/> instead of the default constructor to reduce object
		/// creation.
		/// </remarks>
		/// <seealso cref="SudokuGrid()"/>
		public static readonly SudokuGrid Undefined = new();

		/// <summary>
		/// The empty grid that is valid during implementation or running the program
		/// (all values are 511, i.e. empty cells).
		/// </summary>
		/// <remarks>
		/// This field is initialized by the static constructor of this structure.
		/// </remarks>
		public static readonly SudokuGrid Empty;

		/// <summary>
		/// Indicates the event triggered when the value is changed.
		/// </summary>
		public static readonly delegate* managed<ref SudokuGrid, in ValueChangedArgs, void> ValueChanged;


		/// <summary>
		/// Indicates the inner array.
		/// </summary>
		private fixed short _values[Length];


		/// <summary>
		/// Initializes an instance with the specified mask list and the length.
		/// </summary>
		/// <param name="masks">The masks.</param>
		/// <param name="length">The length of the <paramref name="masks"/>. The value should be 81.</param>
		/// <exception cref="ArgumentNullException">
		/// Throws when <paramref name="masks"/> is <see langword="null"/>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Throws when <paramref name="length"/> is not 81.
		/// </exception>
		public SudokuGrid(short* masks, int length)
		{
			_ = masks == null ? throw new ArgumentNullException(nameof(masks)) : masks;
			_ = length != Length ? throw new ArgumentException($"The specified argument should be {Length}.", nameof(length)) : length;

			fixed (short* arr = _values)
			{
				InternalCopy(arr, masks);
			}
		}


		/// <inheritdoc cref="StaticConstructor"/>
		static SudokuGrid()
		{
			Empty = new();
			fixed (short* p = Empty._values)
			{
				InternalInitialize(p, DefaultMask);
			}
		}


		/// <inheritdoc cref="object.Equals(object?)"/>
		public override readonly bool Equals(object? obj) => obj is SudokuGrid other && Equals(other);

		/// <inheritdoc/>
		public readonly bool Equals(SudokuGrid other)
		{
			fixed (short* arr = _values)
			{
				for (short* l = arr, r = other._values, i = (short*)0; (int)i < Length; i = (short*)((int)i + 1), l++, r++)
				{
					if (*l != *r)
					{
						return false;
					}
				}

				return true;
			}
		}

		/// <inheritdoc cref="object.GetHashCode"/>
		public override readonly int GetHashCode() =>
			true switch
			{
				_ when this == Undefined => 0,
				_ when this == Empty => 1,
				_ => ToString(".+:").GetHashCode()
			};

		/// <inheritdoc cref="object.ToString"/>
		public override readonly string ToString() => ToString(null, null);

		/// <inheritdoc cref="Formattable.ToString(string?)"/>
		public readonly string ToString(string? format) => ToString(format, null);

		/// <inheritdoc/>
		public readonly string ToString(string? format, IFormatProvider? formatProvider)
		{
			if (formatProvider.HasFormatted(this, format, out string? result))
			{
				return result;
			}

			// TODO: Implement this.
			throw new NotImplementedException();
		}

		/// <inheritdoc/>
		public readonly IEnumerator<short> GetEnumerator()
		{
			fixed (short* arr = _values)
			{
				return new Enumerator(arr);
			}
		}

		/// <inheritdoc/>
		readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


		/// <summary>
		/// Internal copy.
		/// </summary>
		/// <param name="dest">The destination pointer.</param>
		/// <param name="src">The source pointer.</param>
		private static void InternalCopy(short* dest, short* src)
		{
#if DEBUG
			_ = dest == null ? throw new ArgumentNullException(nameof(dest)) : dest;
			_ = src == null ? throw new ArgumentNullException(nameof(src)) : src;
#endif

			for (short* p = dest, q = src, i = (short*)0; (int)i < Length; i = (short*)((int)i + 1), *p++ = *q++) ;
		}

		/// <summary>
		/// Internal initialize.
		/// </summary>
		/// <param name="dest">The destination pointer.</param>
		/// <param name="value">The value.</param>
		private static void InternalInitialize(short* dest, short value)
		{
#if DEBUG
			_ = dest == null ? throw new ArgumentNullException(nameof(dest)) : dest;
#endif

			for (short* p = dest, i = (short*)0; (int)i < Length; i = (short*)((int)i + 1), *p++ = value) ;
		}


		/// <inheritdoc cref="Operators.operator =="/>
		public static bool operator ==(SudokuGrid left, SudokuGrid right) => left.Equals(right);

		/// <inheritdoc cref="Operators.operator !="/>
		public static bool operator !=(SudokuGrid left, SudokuGrid right) => !(left == right);
	}
}
