using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Constants;
using Sudoku.DocComments;
using Sudoku.Extensions;
using static Sudoku.Constants.Processings;
using System.ComponentModel;
#if DEBUG
using System.Diagnostics;
#endif

namespace Sudoku.Data
{
	/// <summary>
	/// Encapsulates a sudoku grid using value type instead of reference type.
	/// </summary>
#if DEBUG
	[DebuggerDisplay("{ToString(\"0+:\")}")]
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
		/// Indicates the inner array.
		/// </summary>
		private fixed short _values[Length];

		/// <summary>
		/// Indicates the inner array suggests the initial grid.
		/// </summary>
		private fixed short _initialValues[Length];


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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public SudokuGrid(short* masks, int length)
		{
			_ = masks == null ? throw new ArgumentNullException(nameof(masks)) : masks;
			_ = length != Length ? throw new ArgumentException($"The specified argument should be {Length}.", nameof(length)) : length;

			fixed (short* pValues = _values, pInitialValues = _initialValues)
			{
				InternalCopy(pValues, masks);
				InternalCopy(pInitialValues, masks);
			}
		}


		/// <inheritdoc cref="StaticConstructor"/>
		static SudokuGrid()
		{
			// Initializes the empty grid.
			Empty = new();
			fixed (short* p = Empty._values, q = Empty._initialValues)
			{
				InternalInitialize(p, DefaultMask);
				InternalInitialize(q, DefaultMask);
			}

			// Initializes events.
			ValueChanged = &OnValueChanged;
			RefreshingCandidates = &OnRefreshingCandidates;
		}


		/// <summary>
		/// Indicates the grid has already solved. If the value is <see langword="true"/>,
		/// the grid is solved; otherwise, <see langword="false"/>.
		/// </summary>
		public readonly bool HasSolved
		{
			get
			{
				for (int i = 0; i < Length; i++)
				{
					if (GetStatus(i) == CellStatus.Empty)
					{
						return false;
					}
				}

				return SimplyValidate();
			}
		}

		/// <summary>
		/// Indicates the number of total candidates.
		/// </summary>
		public readonly int CandidatesCount
		{
			get
			{
				int count = 0;
				for (int i = 0; i < Length; i++)
				{
					if (GetStatus(i) == CellStatus.Empty)
					{
						count += GetCandidateMask(i).PopCount();
					}
				}

				return count;
			}
		}

		/// <summary>
		/// Indicates the total number of given cells.
		/// </summary>
		public readonly int GivensCount { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Triplet.C; }

		/// <summary>
		/// Indicates the total number of modifiable cells.
		/// </summary>
		public readonly int ModifiablesCount { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Triplet.B; }

		/// <summary>
		/// Indicates the total number of empty cells.
		/// </summary>
		public readonly int EmptiesCount { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Triplet.A; }

		/// <summary>
		/// The triplet of three main information.
		/// </summary>
		private readonly (int A, int B, int C) Triplet
		{
			get
			{
				int a = 0, b = 0, c = 0;
				for (int i = 0; i < Length; i++)
				{
					var s = GetStatus(i);
					(s == CellStatus.Empty ? ref a : ref s == CellStatus.Modifiable ? ref b : ref c)++;
				}

				return (a, b, c);
			}
		}


		/// <summary>
		/// Gets or sets the value in the specified cell.
		/// </summary>
		/// <param name="cell">The cell you want to get or set a value.</param>
		/// <value>The value you want to set.</value>
		/// <returns>The value that the cell filled with.</returns>
		[IndexerName("Value")]
		public int this[int cell]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get => GetStatus(cell) switch
			{
				CellStatus.Empty => -1,
				CellStatus.Modifiable or CellStatus.Given => (~_values[cell]).FindFirstSet(),
				_ => throw Throwings.ImpossibleCase
			};

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				switch (value)
				{
					case -1 when GetStatus(cell) == CellStatus.Modifiable:
					{
						// If 'value' is -1, we should reset the grid.
						// Note that reset candidates may not trigger the event.
						_values[cell] = DefaultMask;
						RefreshingCandidates(ref this);

						break;
					}
					case >= 0 and < 9:
					{
						ref short result = ref _values[cell];
						short copy = result;

						// Set cell status to 'CellStatus.Modifiable'.
						result = (short)((short)CellStatus.Modifiable << 9 | MaxCandidatesMask & ~(1 << value));

						// To trigger the event, which is used for eliminate
						// all same candidates in peer cells.
						ValueChanged(ref this, new(cell, copy, result, value));

						break;
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets a candidate existence case with a <see cref="bool"/> value.
		/// </summary>
		/// <param name="cell">The cell offset between 0 and 80.</param>
		/// <param name="digit">The digit between 0 and 8.</param>
		/// <value>
		/// The case you want to set. <see langword="true"/> means that this candidate
		/// doesn't exist in this current sudoku grid; otherwise, <see langword="false"/>.
		/// </value>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		[IndexerName("Value")]
		public bool this[int cell, int digit]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get => (_values[cell] >> digit & 1) != 0;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				ref short result = ref _values[cell];
				short copy = result;
				if (value)
				{
					result |= (short)(1 << digit);
				}
				else
				{
					result &= (short)~(1 << digit);
				}

				// To trigger the event.
				ValueChanged(ref this, new(cell, copy, result, -1));
			}
		}


		/// <summary>
		/// Check whether the current grid is valid (no duplicate values on same row, column or block).
		/// </summary>
		/// <returns>The <see cref="bool"/> result.</returns>
		public readonly bool SimplyValidate()
		{
			for (int i = 0, count = 0; i < Length; i++)
			{
				switch (GetStatus(i))
				{
					case CellStatus.Given: /*fallthrough*/
					{
						count++;
						goto case CellStatus.Modifiable;
					}
					case CellStatus.Modifiable:
					{
						int curDigit = this[i];
						foreach (int cell in PeerMaps[i])
						{
							if (curDigit == this[cell])
							{
								return false;
							}
						}

						break;
					}
				}
			}

			return true;
		}

		/// <inheritdoc cref="object.Equals(object?)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly bool Equals(object? obj) => obj is SudokuGrid other && Equals(in other);

		/// <inheritdoc/>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(SudokuGrid other) => Equals(in other);

		/// <inheritdoc cref="Equals(SudokuGrid)"/>
		public readonly bool Equals(in SudokuGrid other)
		{
			fixed (short* pThis = _values, pOther = other._values)
			{
				for (short* l = pThis, r = pOther, i = (short*)0; (int)i < Length; i = (short*)((int)i + 1), l++, r++)
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly int GetHashCode() =>
			true switch
			{
				_ when this == Undefined => 0,
				_ when this == Empty => 1,
				_ => ToString(".+:").GetHashCode()
			};

		/// <summary>
		/// Serializes this instance to an array, where all digit value will be stored.
		/// </summary>
		/// <returns>
		/// This array. All elements are between 0 to 9, where 0 means the
		/// cell is <see cref="CellStatus.Empty"/> now.
		/// </returns>
		public readonly int[] ToArray()
		{
			var span = (stackalloc int[Length]);
			for (int i = 0; i < Length; i++)
			{
				// 'this[i]' is always in range -1 to 8 (-1 is empty, and 0 to 8 is 1 to 9 for
				// mankind representation).
				span[i] = this[i] + 1;
			}

			return span.ToArray();
		}

		/// <summary>
		/// Get a mask at the specified cell.
		/// </summary>
		/// <param name="offset">The cell offset you want to get.</param>
		/// <returns>The mask.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)] 
		public readonly short GetMask(int offset) => _values[offset];

		/// <summary>
		/// Get the candidate mask part of the specified cell.
		/// </summary>
		/// <param name="cell">The cell offset you want to get.</param>
		/// <returns>
		/// <para>The candidate mask.</para>
		/// <para>
		/// The return value is a 9-bit <see cref="short"/>
		/// value, where the bit will be <c>0</c> if the corresponding digit <b>doesn't exist</b> in the cell,
		/// and will be <c>1</c> if the corresponding contains this digit (either the cell
		/// is filled with this digit or the cell is an empty cell, whose candidates contains the digit).
		/// </para>
		/// </returns>
		/// <remarks>
		/// Please note that the grid masks is represented with bits, where 0 is for the digit containing in a
		/// cell, 1 is for the digit <b>not</b> containing. However, here the return mask is the reversal:
		/// 1 is for containing and 0 is for <b>not</b>.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)] 
		public readonly short GetCandidateMask(int cell) => (short)(~_values[cell] & MaxCandidatesMask);

		/// <inheritdoc cref="object.ToString"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)] 
		public override readonly string ToString() => ToString(null, null);

		/// <inheritdoc cref="Formattable.ToString(string?)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)] 
		public readonly string ToString(string? format) => ToString(format, null);

		/// <inheritdoc/>
		public readonly string ToString(string? format, IFormatProvider? formatProvider)
		{
			if (formatProvider.HasFormatted(this, format, out string? result))
			{
				return result;
			}

			var formatter = GridFormatter.Create(format);
			return format switch
			{
				":" => formatter.ToString(this).Match(RegularExpressions.ExtendedSusserEliminations).NullableToString(),
				"!" => formatter.ToString(this).Replace("+", string.Empty),
				".!" or "!." or "0!" or "!0" => formatter.ToString(this).Replace("+", string.Empty),
				".!:" or "!.:" or "0!:" => formatter.ToString(this).Replace("+", string.Empty),
				_ => formatter.ToString(this)
			};
		}

		/// <summary>
		/// Get the cell status at the specified cell.
		/// </summary>
		/// <param name="cell">The cell.</param>
		/// <returns>The cell status.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)] 
		public readonly CellStatus GetStatus(int cell) => (CellStatus)(_values[cell] >> 9 & (int)CellStatus.All);

		/// <summary>
		/// Get all candidates containing in the specified cell.
		/// </summary>
		/// <param name="cell">The cell you want to get.</param>
		/// <returns>All candidates.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)] 
		public readonly IEnumerable<int> GetCandidates(int cell) => GetCandidateMask(cell).GetAllSets();

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly IEnumerator<short> GetEnumerator()
		{
			fixed (short* arr = _values)
			{
				return new Enumerator(arr);
			}
		}

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)] 
		readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		/// <summary>
		/// To fix the current grid (all modifiable values will be changed to given ones).
		/// </summary>
		public void Fix()
		{
			for (int i = 0; i < 81; i++)
			{
				if (GetStatus(i) == CellStatus.Modifiable)
				{
					SetStatus(i, CellStatus.Given);
				}
			}

			fixed (short* pValues = _values, pInitialValues = _initialValues)
			{
				InternalCopy(pInitialValues, pValues);
			}
		}

		/// <summary>
		/// To unfix the current grid (all given values will be chanegd to modifiable ones).
		/// </summary>
		public void Unfix()
		{
			for (int i = 0; i < 81; i++)
			{
				if (GetStatus(i) == CellStatus.Given)
				{
					SetStatus(i, CellStatus.Modifiable);
				}
			}
		}

		/// <summary>
		/// To reset the grid to iniatial status.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Reset()
		{
			fixed (short* pValues = _values, pInitialValues = _initialValues)
			{
				InternalCopy(pValues, pInitialValues);
			}
		}

		/// <summary>
		/// Set the specified cell to the specified status.
		/// </summary>
		/// <param name="cell">The cell.</param>
		/// <param name="status">The status.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetStatus(int cell, CellStatus status)
		{
			ref short mask = ref _values[cell];
			short copy = mask;
			mask = (short)((int)status << 9 | mask & MaxCandidatesMask);

			ValueChanged(ref this, new(cell, copy, mask, -1));
		}

		/// <summary>
		/// Set the specified cell to the specified mask.
		/// </summary>
		/// <param name="cell">The cell.</param>
		/// <param name="mask">The mask to set.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetMask(int cell, short mask)
		{
			ref short m = ref _values[cell];
			short copy = m;
			m = mask;

			ValueChanged(ref this, new(cell, copy, m, -1));
		}


		/// <summary>
		/// <para>
		/// Parses a string value and converts to this type.
		/// </para>
		/// <para>
		/// If you want to parse a PM grid, we recommend you use the method
		/// <see cref="Parse(string, GridParsingOption)"/> instead of this method.
		/// </para>
		/// </summary>
		/// <param name="str">The string.</param>
		/// <returns>The result instance had converted.</returns>
		/// <seealso cref="Parse(string, GridParsingOption)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)] 
		public static SudokuGrid Parse(ReadOnlySpan<char> str) => new GridParser(str.ToString()).Parse();

		/// <summary>
		/// <para>
		/// Parses a string value and converts to this type.
		/// </para>
		/// <para>
		/// If you want to parse a PM grid, we recommend you use the method
		/// <see cref="Parse(string, GridParsingOption)"/> instead of this method.
		/// </para>
		/// </summary>
		/// <param name="str">The string.</param>
		/// <returns>The result instance had converted.</returns>
		/// <seealso cref="Parse(string, GridParsingOption)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)] 
		public static SudokuGrid Parse(string str) => new GridParser(str).Parse();

		/// <summary>
		/// <para>
		/// Parses a string value and converts to this type.
		/// </para>
		/// <para>
		/// If you want to parse a PM grid, you should decide the mode to parse.
		/// If you use compatible mode to parse, all single values will be treated as
		/// given values; otherwise, recommended mode, which uses '<c>&lt;d&gt;</c>'
		/// or '<c>*d*</c>' to represent a value be a given or modifiable one. The decision
		/// will be indicated and passed by the second parameter <paramref name="compatibleFirst"/>.
		/// </para>
		/// </summary>
		/// <param name="str">The string.</param>
		/// <param name="compatibleFirst">
		/// Indicates whether the parsing operation should use compatible mode to check
		/// PM grid. See <see cref="GridParser.CompatibleFirst"/> to learn more.
		/// </param>
		/// <returns>The result instance had converted.</returns>
		/// <seealso cref="GridParser.CompatibleFirst"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)] 
		public static SudokuGrid Parse(string str, bool compatibleFirst) => new GridParser(str, compatibleFirst).Parse();

		/// <summary>
		/// Parses a string value and converts to this type,
		/// using a specified grid parsing type.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <param name="gridParsingOption">The grid parsing type.</param>
		/// <returns>The result instance had converted.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SudokuGrid Parse(string str, GridParsingOption gridParsingOption) =>
			new GridParser(str).Parse(gridParsingOption);

		/// <summary>
		/// Try to parse a string and converts to this type, and returns a
		/// <see cref="bool"/> value indicating the result of the conversion.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <param name="result">
		/// (<see langword="out"/> parameter) The result parsed. If the conversion is failed,
		/// this argument will be <see cref="Undefined"/>.
		/// </param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		/// <seealso cref="Undefined"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryParse(string str, out SudokuGrid result)
		{
			try
			{
				result = Parse(str);
				return true;
			}
			catch
			{
				result = Undefined;
				return false;
			}
		}

		/// <summary>
		/// Try to parse a string and converts to this type, and returns a
		/// <see cref="bool"/> value indicating the result of the conversion.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <param name="gridParsingOption">The grid parsing type.</param>
		/// <param name="result">
		/// (<see langword="out"/> parameter) The result parsed. If the conversion is failed,
		/// this argument will be <see cref="Undefined"/>.
		/// </param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		/// <seealso cref="Undefined"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryParse(string str, GridParsingOption gridParsingOption, out SudokuGrid result)
		{
			try
			{
				result = Parse(str, gridParsingOption);
				return true;
			}
			catch
			{
				result = Undefined;
				return false;
			}
		}

		/// <summary>
		/// Creates an instance using grid values.
		/// </summary>
		/// <param name="gridValues">The array of grid values.</param>
		/// <returns>The result instance.</returns>
		public static SudokuGrid CreateInstance(int[] gridValues)
		{
			var result = Empty;
			for (int i = 0; i < Length; i++)
			{
				if (gridValues[i] is var value and not 0)
				{
					// Calls the indexer to trigger the event
					// (Clear the candidates in peer cells).
					result[i] = value - 1;

					// Set the status to 'CellStatus.Given'.
					result.SetStatus(i, CellStatus.Given);
				}
			}

			return result;
		}

		/// <summary>
		/// Delete or set a value on the specified grid.
		/// </summary>
		/// <param name="this">(<see langword="ref"/> parameter) The grid.</param>
		/// <param name="e">(<see langword="in"/> parameter) The event arguments.</param>
		private static void OnValueChanged(ref SudokuGrid @this, in ValueChangedArgs e)
		{
			if ((e.Cell, e.SetValue) is (var cell, var setValue and not -1))
			{
				foreach (int peerCell in PeerMaps[cell])
				{
					if (@this.GetStatus(peerCell) == CellStatus.Empty)
					{
						@this._values[peerCell] |= (short)(1 << setValue);
					}
				}
			}
		}

		/// <summary>
		/// Re-compute candidates.
		/// </summary>
		/// <param name="this">The grid.</param>
		public static void OnRefreshingCandidates(ref SudokuGrid @this)
		{
			for (int i = 0; i < Length; i++)
			{
				if (@this.GetStatus(i) == CellStatus.Empty)
				{
					short mask = 0;
					foreach (int cell in PeerMaps[i])
					{
						if (@this[cell] is var digit and not -1)
						{
							mask |= (short)(1 << digit);
						}
					}

					@this._values[i] = (short)(DefaultMask | mask);
				}
			}
		}

#if DEBUG
		/// <summary>
		/// Internal copy.
		/// </summary>
		/// <param name="dest">The destination pointer.</param>
		/// <param name="src">The source pointer.</param>
		/// <exception cref="ArgumentNullException">
		/// Throws when <paramref name="dest"/> or <paramref name="src"/> is <see langword="null"/>.
		/// </exception>
#else
		/// <summary>
		/// Internal copy.
		/// </summary>
		/// <param name="dest">The destination pointer.</param>
		/// <param name="src">The source pointer.</param>
#endif
		private static void InternalCopy(short* dest, short* src)
		{
#if DEBUG
			_ = dest == null ? throw new ArgumentNullException(nameof(dest)) : dest;
			_ = src == null ? throw new ArgumentNullException(nameof(src)) : src;
#endif

			for (short* p = dest, q = src, i = (short*)0; (int)i < Length; i = (short*)((int)i + 1), *p++ = *q++) ;
		}

#if DEBUG
		/// <summary>
		/// Internal initialize.
		/// </summary>
		/// <param name="dest">The destination pointer.</param>
		/// <param name="value">The value.</param>
		/// <exception cref="ArgumentNullException">
		/// Throws when <paramref name="dest"/> is <see langword="null"/>.
		/// </exception>
#else
		/// <summary>
		/// Internal initialize.
		/// </summary>
		/// <param name="dest">The destination pointer.</param>
		/// <param name="value">The value.</param>
#endif
		private static void InternalInitialize(short* dest, short value)
		{
#if DEBUG
			_ = dest == null ? throw new ArgumentNullException(nameof(dest)) : dest;
#endif

			for (short* p = dest, i = (short*)0; (int)i < Length; i = (short*)((int)i + 1), *p++ = value) ;
		}


		/// <inheritdoc cref="Operators.operator =="/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)] 
		public static bool operator ==(in SudokuGrid left, in SudokuGrid right) => left.Equals(in right);

		/// <inheritdoc cref="Operators.operator !="/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)] 
		public static bool operator !=(in SudokuGrid left, in SudokuGrid right) => !(left == right);
	}
}
