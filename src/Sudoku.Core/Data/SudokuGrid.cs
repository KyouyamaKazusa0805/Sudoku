using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Extensions;
using System.Runtime.CompilerServices;
using System.Text;
using Sudoku.DocComments;
using static System.Numerics.BitOperations;
using static Sudoku.Constants;
using static Sudoku.Constants.Tables;
using CreatingOption = Sudoku.Data.GridCreatingOption;
using ParsingOption = Sudoku.Data.GridParsingOption;

namespace Sudoku.Data
{
	/// <summary>
	/// Encapsulates a sudoku grid using value type instead of reference type.
	/// </summary>
#if DEBUG
	[DebuggerDisplay("{" + nameof(ToString) + "(\".+:\"),nq}")]
#endif
	[DisableParameterlessConstructor(nameof(Empty))]
	public unsafe partial struct SudokuGrid : IValueEquatable<SudokuGrid>, IFormattable
	{
		/// <summary>
		/// Indicates the default mask of a cell (an empty cell, with all 9 candidates left).
		/// </summary>
		public const short DefaultMask = EmptyMask | MaxCandidatesMask;

		/// <summary>
		/// Indicates the maximum candidate mask that used.
		/// </summary>
		public const short MaxCandidatesMask = 0b111_111_111;

		/// <summary>
		/// Indicates the empty mask.
		/// </summary>
		public const short EmptyMask = (int)CellStatus.Empty << 9;

		/// <summary>
		/// Indicates the modifiable mask.
		/// </summary>
		public const short ModifiableMask = (int)CellStatus.Modifiable << 9;

		/// <summary>
		/// Indicates the given mask.
		/// </summary>
		public const short GivenMask = (int)CellStatus.Given << 9;

		/// <summary>
		/// Indicates the size of each grid.
		/// </summary>
		private const byte Length = 81;


		/// <summary>
		/// Indicates the empty grid string.
		/// </summary>
		public static readonly string EmptyString = new('0', Length);

		/// <summary>
		/// Indicates the event triggered when the value is changed.
		/// </summary>
		[CLSCompliant(false)]
		public static readonly delegate* managed<ref SudokuGrid, in ValueChangedArgs, void> ValueChanged;

		/// <summary>
		/// Indicates the event triggered when should re-compute candidates.
		/// </summary>
		[CLSCompliant(false)]
		public static readonly delegate* managed<ref SudokuGrid, void> RefreshingCandidates;

		/// <summary>
		/// Indicates the default grid that all values are initialized 0, which is same as
		/// <see cref="SudokuGrid()"/>.
		/// </summary>
		/// <remarks>
		/// We recommend you should use this static field instead of the default constructor
		/// to reduce object creation.
		/// </remarks>
		/// <seealso cref="SudokuGrid()"/>
		public static readonly SudokuGrid Undefined;

		/// <summary>
		/// The empty grid that is valid during implementation or running the program
		/// (all values are <see cref="DefaultMask"/>, i.e. empty cells).
		/// </summary>
		/// <remarks>
		/// This field is initialized by the static constructor of this structure.
		/// </remarks>
		/// <seealso cref="DefaultMask"/>
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
		/// Creates an instance using grid values.
		/// </summary>
		/// <param name="gridValues">The array of grid values.</param>
		public SudokuGrid(int[] gridValues) : this(gridValues, CreatingOption.None)
		{
		}

		/// <summary>
		/// Creates an instance using grid values.
		/// </summary>
		/// <param name="gridValues">The array of grid values.</param>
		/// <param name="creatingOption">The grid creating option.</param>
		public SudokuGrid(int[] gridValues, CreatingOption creatingOption)
		{
			var result = Empty;
			for (int i = 0; i < Length; i++)
			{
				if (gridValues[i] is var value and not 0)
				{
					// Calls the indexer to trigger the event
					// (Clear the candidates in peer cells).
					result[i] = creatingOption == CreatingOption.MinusOne ? value - 1 : value;

					// Set the status to 'CellStatus.Given'.
					result.SetStatus(i, CellStatus.Given);
				}
			}

			this = result;
		}

		/// <summary>
		/// Initializes an instance with the specified mask array.
		/// </summary>
		/// <param name="masks">The masks.</param>
		/// <exception cref="ArgumentException">Throws when <see cref="Array.Length"/> is not 81.</exception>
		internal SudokuGrid(short[] masks)
		{
			if (masks.Length != Length)
			{
				throw new ArgumentException($"The length of the array argument should be {Length.ToString()}.", nameof(masks));
			}

			fixed (short* pArray = masks, pValues = _values, pInitialValues = _initialValues)
			{
				Unsafe.CopyBlock(pValues, pArray, sizeof(short) * 81);
				Unsafe.CopyBlock(pInitialValues, pArray, sizeof(short) * 81);
			}
		}


		/// <inheritdoc cref="StaticConstructor"/>
		static SudokuGrid()
		{
			// Initializes the empty grid.
			Empty = new();
			fixed (short* p = Empty._values, q = Empty._initialValues)
			{
				int i = 0;
				for (short* ptrP = p, ptrQ = q; i < Length; *ptrP++ = *ptrQ++ = DefaultMask, i++) ;
			}

			// Initializes events.
			ValueChanged = &OnValueChanged;
			RefreshingCandidates = &OnRefreshingCandidates;
		}


		/// <summary>
		/// Indicates the grid has already solved. If the value is <see langword="true"/>,
		/// the grid is solved; otherwise, <see langword="false"/>.
		/// </summary>
		public readonly bool IsSolved
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
						count += PopCount((uint)GetCandidates(i));
					}
				}

				return count;
			}
		}

		/// <summary>
		/// Indicates the total number of given cells.
		/// </summary>
		public readonly int GivensCount => Triplet.C;

		/// <summary>
		/// Indicates the total number of modifiable cells.
		/// </summary>
		public readonly int ModifiablesCount => Triplet.B;

		/// <summary>
		/// Indicates the total number of empty cells.
		/// </summary>
		public readonly int EmptiesCount => Triplet.A;

		/// <summary>
		/// Indicates the cells that correspoinding position in this grid is empty.
		/// </summary>
		/// <remarks>
		/// Note that this property isn't a promptly one because the value won't be calculated
		/// until this property called.
		/// </remarks>
		public readonly Cells EmptyCells
		{
			get
			{
				return GetCells(&p);

				static bool p(in SudokuGrid @this, int cell) => @this.GetStatus(cell) == CellStatus.Empty;
			}
		}

		/// <summary>
		/// Indicates the cells that corresponding position in this grid contain two candidates.
		/// </summary>
		/// <remarks>
		/// Note that this property isn't a promptly one because the value won't be calculated
		/// until this property called.
		/// </remarks>
		public readonly Cells BivalueCells
		{
			get
			{
				return GetCells(&p);

				static bool p(in SudokuGrid @this, int cell) => PopCount((uint)@this.GetCandidates(cell)) == 2;
			}
		}

		/// <summary>
		/// Indicates the map of possible positions of the existence of the candidate value for each digit.
		/// The return value will be an array of 9 elements, which stands for the statuses of 9 digits.
		/// </summary>
		/// <remarks>
		/// Note that this property isn't a promptly one because the value won't be calculated
		/// until this property called.
		/// </remarks>
		public readonly Cells[] CandidateMap
		{
			get
			{
				return GetMap(&p);

				static bool p(in SudokuGrid @this, int cell, int digit) => @this.Exists(cell, digit) is true;
			}
		}

		/// <summary>
		/// <para>
		/// Indicates the map of possible positions of the existence of each digit. The return value will
		/// be an array of 9 elements, which stands for the statuses of 9 digits.
		/// </para>
		/// <para>
		/// Different with <see cref="CandidateMap"/>, this property contains all givens, modifiables and
		/// empty cells only if it contains the digit in the mask.
		/// </para>
		/// </summary>
		/// <remarks>
		/// Note that this property isn't a promptly one because the value won't be calculated
		/// until this property called.
		/// </remarks>
		/// <seealso cref="CandidateMap"/>
		public readonly Cells[] DigitsMap
		{
			get
			{
				return GetMap(&p);

				static bool p(in SudokuGrid @this, int cell, int digit) =>
					(@this.GetCandidates(cell) >> digit & 1) != 0;
			}
		}

		/// <summary>
		/// <para>
		/// Indicates the map of possible positions of the existence of that value of each digit.
		/// The return value will be an array of 9 elements, which stands for the statuses of 9 digits.
		/// </para>
		/// <para>
		/// Different with <see cref="CandidateMap"/>, the value only contains the given or modifiable
		/// cells whose mask contain the set bit of that digit.
		/// </para>
		/// </summary>
		/// <remarks>
		/// Note that this property isn't a promptly one because the value won't be calculated
		/// until this property called.
		/// </remarks>
		/// <seealso cref="CandidateMap"/>
		public readonly Cells[] ValuesMap
		{
			get
			{
				return GetMap(&p);

				static bool p(in SudokuGrid @this, int cell, int digit) => @this[cell] == digit;
			}
		}

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
		/// <value>
		/// The value you want to set. The value should be between 0 and 8. If assigning -1,
		/// that means to re-compute all candidates.
		/// </value>
		/// <returns>The value that the cell filled with.</returns>
		[IndexerName("Value")]
		public int this[int cell]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get => GetStatus(cell) switch
			{
				CellStatus.Undefined => -2,
				CellStatus.Empty => -1,
				CellStatus.Modifiable or CellStatus.Given => TrailingZeroCount(_values[cell])
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
						result = (short)(ModifiableMask | 1 << value);

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
		/// The case you want to set. <see langword="false"/> means that this candidate
		/// doesn't exist in this current sudoku grid; otherwise, <see langword="true"/>.
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
				if (cell is >= 0 and < 81 && digit is >= 0 and < 9)
				{
					short copied = _values[cell];
					if (value)
					{
						_values[cell] |= (short)(1 << digit);
					}
					else
					{
						_values[cell] &= (short)~(1 << digit);
					}

					// To trigger the event.
					ValueChanged(ref this, new(cell, copied, _values[cell], -1));
				}
			}
		}

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="empty">(<see langword="out"/> parameter) The map of all empty cells.</param>
		/// <param name="bivalue">(<see langword="out"/> parameter) The map of all bi-value cells.</param>
		/// <param name="candidates">
		/// (<see langword="out"/> parameter) The map of all cells that contain the candidate of that digit.
		/// </param>
		/// <param name="digits">
		/// (<see langword="out"/> parameter) The map of all cells that contain the candidate of that digit
		/// or that value in given or modifiable.
		/// </param>
		/// <param name="values">
		/// (<see langword="out"/> parameter) The map of all cells that is the given or modifiable value,
		/// and the digit is the specified one.
		/// </param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly void Deconstruct(
			out Cells empty, out Cells bivalue, out Cells[] candidates, out Cells[] digits, out Cells[] values)
		{
			empty = EmptyCells;
			bivalue = BivalueCells;
			candidates = CandidateMap;
			digits = DigitsMap;
			values = ValuesMap;
		}

		/// <summary>
		/// Check whether the current grid is valid (no duplicate values on same row, column or block).
		/// </summary>
		/// <returns>The <see cref="bool"/> result.</returns>
		public readonly bool SimplyValidate()
		{
			for (int i = 0; i < Length; i++)
			{
				switch (GetStatus(i))
				{
					case CellStatus.Given:
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
					case CellStatus.Empty:
					{
						continue;
					}
					default:
					{
						return false;
					}
				}
			}

			return true;
		}

		/// <inheritdoc cref="object.Equals(object?)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly bool Equals(object? obj) => obj is SudokuGrid other && Equals(other);

		/// <inheritdoc/>
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(in SudokuGrid other) => Equals(this, other);

		/// <summary>
		/// Indicates whether the current grid contains the digit in the specified cell.
		/// </summary>
		/// <param name="cell">The cell offset.</param>
		/// <param name="digit">The digit.</param>
		/// <returns>
		/// The method will return a <see cref="bool"/>? value (contains three possible cases:
		/// <see langword="true"/>, <see langword="false"/> and <see langword="null"/>).
		/// All values corresponding to the cases are below:
		/// <list type="table">
		/// <item>
		/// <term><c><see langword="true"/></c></term>
		/// <description>
		/// The cell is an empty cell <b>and</b> contains the specified digit.
		/// </description>
		/// </item>
		/// <item>
		/// <term><c><see langword="false"/></c></term>
		/// <description>
		/// The cell is an empty cell <b>but doesn't</b> contain the specified digit.
		/// </description>
		/// </item>
		/// <item>
		/// <term><c><see langword="null"/></c></term>
		/// <description>The cell is <b>not</b> an empty cell.</description>
		/// </item>
		/// </list>
		/// </returns>
		/// <remarks>
		/// Note that the method will return a <see cref="bool"/>?, so you should use the code
		/// '<c>grid.Exists(candidate) is true</c>' or '<c>grid.Exists(candidate) == true</c>'
		/// to decide whether a condition is true.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool? Exists(int cell, int digit) =>
			GetStatus(cell) == CellStatus.Empty ? this[cell, digit] : null;

		/// <inheritdoc cref="object.GetHashCode"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly int GetHashCode() =>
			this == Undefined ? 0 : this == Empty ? 1 : ToString("#").GetHashCode();

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
		/// <para>
		/// The candidate mask. The return value is a 9-bit <see cref="short"/>
		/// value, where each bit will be:
		/// <list type="table">
		/// <item>
		/// <term><c>0</c></term>
		/// <description>The cell <b>doesn't contain</b> the possibility of the digit.</description>
		/// </item>
		/// <item>
		/// <term><c>1</c></term>
		/// <description>The cell <b>contains</b> the possibility of the digit.</description>
		/// </item>
		/// </list>
		/// </para>
		/// <para>
		/// For example, if the result mask is 266 (i.e. <c>0b<b>1</b>00_00<b>1</b>_0<b>1</b>0</c> in binary),
		/// the value will indicate the cell contains the digit 2, 4 and 9.
		/// </para>
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly short GetCandidates(int cell) => (short)(_values[cell] & MaxCandidatesMask);

		/// <summary>
		/// Returns a reference to the element of the <see cref="SudokuGrid"/> at index zero.
		/// </summary>
		/// <returns>A reference to the element of the <see cref="SudokuGrid"/> at index zero.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public readonly ref readonly short GetPinnableReference()
		{
			fixed (SudokuGrid* @this = &this)
			{
				return ref @this->_values[0];
			}
		}

		/// <summary>
		/// Returns a reference to the element of the <see cref="SudokuGrid"/> at index zero.
		/// </summary>
		/// <param name="pinnedItem">The item you want to fix. If </param>
		/// <returns>A reference to the element of the <see cref="SudokuGrid"/> at index zero.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly ref readonly short GetPinnableReference(PinnedItem pinnedItem)
		{
			switch (pinnedItem)
			{
				case PinnedItem.CurrentGrid:
				{
					return ref GetPinnableReference();
				}
				case PinnedItem.InitialGrid:
				{
					fixed (SudokuGrid* @this = &this)
					{
						return ref @this->_initialValues[0];
					}
				}
				default:
				{
					throw new ArgumentException(
						$"The argument '{nameof(pinnedItem)}' holds invalid value: " +
						$"The value must be {nameof(PinnedItem.CurrentGrid)} or {nameof(PinnedItem.InitialGrid)}.",
						nameof(pinnedItem)
					);
				}
			}
		}

		/// <summary>
		/// Get all masks and print them.
		/// </summary>
		/// <returns>The result.</returns>
		/// <remarks>
		/// Please note that the method cannot be called with a correct behavior using
		/// <see cref="DebuggerDisplayAttribute"/> to output. It seems that Visual Studio
		/// doesn't print correct values when indices of this grid aren't 0. In other words,
		/// when we call this method using <see cref="DebuggerDisplayAttribute"/>, only <c>grid[0]</c>
		/// can be output correctly, and other values will be incorrect: they're always 0.
		/// </remarks>
		public readonly unsafe string ToMaskString()
		{
			const string separator = ", ";
			fixed (short* pArr = _values)
			{
				var sb = new ValueStringBuilder(400);
				sb.AppendRange(pArr, 81, &p, separator);
				sb.RemoveFromEnd(separator.Length);
				return sb.ToString();
			}

			static string p(short v) => v.ToString();
		}

		/// <inheritdoc cref="object.ToString"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly string ToString() => ToString(null, null);

		/// <inheritdoc cref="Formattable.ToString(string?)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly string ToString(string? format) => ToString(format, null);

		/// <inheritdoc/>
		public readonly string ToString(string? format, IFormatProvider? formatProvider)
		{
			if (this == Empty)
			{
				return "<Empty>";
			}

			if (this == Undefined)
			{
				return "<Undefined>";
			}

#if DEBUG
			// The debugger can't recognize fixed buffer.
			// The fixed buffer whose code is like:
			//
			// private fixed short _values[81];
			//
			// However, internally, the field '_values' is implemented
			// with a fixed buffer using a inner struct, which is just like:
			//
			// [StructLayout(LayoutKind.Explicit, Size = 81 * sizeof(short))]
			// private struct FixedBuffer
			// {
			//     public short _internalValue;
			// }
			//
			// And that field:
			//
			// private FixedBuffer _fixedField;
			//
			// From the code we can learn that only 2 bytes of the inner struct can be detected,
			// because the buffer struct only contains 2 bytes data.
			if (debuggerUndefined(this))
			{
				return "<Debugger can't recognize the fixed buffer>";
			}
#endif

			if (formatProvider.HasFormatted(this, format, out string? result))
			{
				return result;
			}

			var f = Formatter.Create(format);
			return format switch
			{
				":" => f.ToString(this).Match(RegularExpressions.ExtendedSusserEliminations) ?? string.Empty,
				"!" => f.ToString(this).Replace("+", string.Empty),
				".!" or "!." or "0!" or "!0" => f.ToString(this).Replace("+", string.Empty),
				".!:" or "!.:" or "0!:" => f.ToString(this).Replace("+", string.Empty),
				_ => f.ToString(this)
			};

#if DEBUG
			static bool debuggerUndefined(in SudokuGrid grid)
			{
				fixed (short* pGrid = grid)
				{
					int i = 1;
					short* p = pGrid + 1;
					while (i < Length)
					{
						if (p++[i++] != 0)
						{
							return false;
						}
					}

					return true;
				}
			}
#endif
		}

		/// <summary>
		/// Get the cell status at the specified cell.
		/// </summary>
		/// <param name="cell">The cell.</param>
		/// <returns>The cell status.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly CellStatus GetStatus(int cell) => MaskGetStatus(_values[cell]);

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Enumerator GetEnumerator()
		{
			fixed (short* arr = _values)
			{
				return new Enumerator(arr);
			}
		}

		/// <summary>
		/// To fix the current grid (all modifiable values will be changed to given ones).
		/// </summary>
		public void Fix()
		{
			for (int i = 0; i < Length; i++)
			{
				if (GetStatus(i) == CellStatus.Modifiable)
				{
					SetStatus(i, CellStatus.Given);
				}
			}

			UpdateInitialMasks();
		}

		/// <summary>
		/// To unfix the current grid (all given values will be chanegd to modifiable ones).
		/// </summary>
		public void Unfix()
		{
			for (int i = 0; i < Length; i++)
			{
				if (GetStatus(i) == CellStatus.Given)
				{
					SetStatus(i, CellStatus.Modifiable);
				}
			}
		}

		/// <summary>
		/// To reset the grid to initial status.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Reset()
		{
			fixed (short* pValues = _values, pInitialValues = _initialValues)
			{
				Unsafe.CopyBlock(pValues, pInitialValues, sizeof(short) * 81);
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
		/// To update initial masks.
		/// </summary>
		internal void UpdateInitialMasks()
		{
			fixed (short* pValues = _values, pInitialValues = _initialValues)
			{
				Unsafe.CopyBlock(pInitialValues, pValues, sizeof(short) * 81);
			}
		}

		/// <summary>
		/// Called by properties <see cref="CandidateMap"/>, <see cref="DigitsMap"/>
		/// and <see cref="ValuesMap"/>.
		/// </summary>
		/// <param name="predicate">The predicate.</param>
		/// <returns>The map of digits.</returns>
		/// <seealso cref="CandidateMap"/>
		/// <seealso cref="DigitsMap"/>
		/// <seealso cref="ValuesMap"/>
		private readonly Cells[] GetMap(delegate* managed<in SudokuGrid, int, int, bool> predicate)
		{
			var result = new Cells[9];
			for (int digit = 0; digit < 9; digit++)
			{
				ref var map = ref result[digit];
				for (int cell = 0; cell < 81; cell++)
				{
					if (predicate(this, cell, digit))
					{
						map.AddAnyway(cell);
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Called by properties <see cref="EmptyCells"/> and <see cref="BivalueCells"/>.
		/// </summary>
		/// <param name="predicate">The predicate.</param>
		/// <returns>The cells.</returns>
		/// <seealso cref="EmptyCells"/>
		/// <seealso cref="BivalueCells"/>
		private readonly Cells GetCells(delegate* managed<in SudokuGrid, int, bool> predicate)
		{
			var result = Cells.Empty;
			for (int cell = 0; cell < 81; cell++)
			{
				if (predicate(this, cell))
				{
					result.AddAnyway(cell);
				}
			}

			return result;
		}


		/// <summary>
		/// To determine whether two sudoku grid is totally same.
		/// </summary>
		/// <param name="left">The left one.</param>
		/// <param name="right">The right one.</param>
		/// <returns>The <see cref="bool"/> result indicating that.</returns>
		public static bool Equals(in SudokuGrid left, in SudokuGrid right)
		{
			fixed (short* pThis = left, pOther = right)
			{
				int i = 0;
				for (short* l = pThis, r = pOther; i < Length; i++, l++, r++)
				{
					if (*l != *r)
					{
						return false;
					}
				}

				return true;
			}
		}

		/// <summary>
		/// <para>Parses a string value and converts to this type.</para>
		/// <para>
		/// If you want to parse a PM grid, we recommend you use the method
		/// <see cref="Parse(string, ParsingOption)"/> instead of this method.
		/// </para>
		/// </summary>
		/// <param name="str">(<see langword="in"/> parameter) The string.</param>
		/// <returns>The result instance had converted.</returns>
		/// <seealso cref="Parse(string, ParsingOption)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SudokuGrid Parse(in ReadOnlySpan<char> str) => new Parser(str.ToString()).Parse();

		/// <summary>
		/// <para>
		/// Parses a string value and converts to this type.
		/// </para>
		/// <para>
		/// If you want to parse a PM grid, we recommend you use the method
		/// <see cref="Parse(string, ParsingOption)"/> instead of this method.
		/// </para>
		/// </summary>
		/// <param name="str">The string.</param>
		/// <returns>The result instance had converted.</returns>
		/// <seealso cref="Parse(string, ParsingOption)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SudokuGrid Parse(string str) => new Parser(str).Parse();

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
		/// PM grid. See <see cref="Parser.CompatibleFirst"/> to learn more.
		/// </param>
		/// <returns>The result instance had converted.</returns>
		/// <seealso cref="Parser.CompatibleFirst"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SudokuGrid Parse(string str, bool compatibleFirst) =>
			new Parser(str, compatibleFirst).Parse();

		/// <summary>
		/// Parses a string value and converts to this type,
		/// using a specified grid parsing type.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <param name="gridParsingOption">The grid parsing type.</param>
		/// <returns>The result instance had converted.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SudokuGrid Parse(string str, ParsingOption gridParsingOption) =>
			new Parser(str).Parse(gridParsingOption);

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
		/// <param name="option">The grid parsing type.</param>
		/// <param name="result">
		/// (<see langword="out"/> parameter) The result parsed. If the conversion is failed,
		/// this argument will be <see cref="Undefined"/>.
		/// </param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		/// <seealso cref="Undefined"/>
		public static bool TryParse(string str, ParsingOption option, out SudokuGrid result)
		{
			try
			{
				result = Parse(str, option);
				return true;
			}
			catch
			{
				result = Undefined;
				return false;
			}
		}

		/// <summary>
		/// To get the cell status through a mask.
		/// </summary>
		/// <param name="mask">The mask.</param>
		/// <returns>The cell status.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static CellStatus MaskGetStatus(short mask) => (CellStatus)(mask >> 9 & (int)CellStatus.All);

		/// <summary>
		/// The method that is pointed by the function pointer <see cref="ValueChanged"/>.
		/// </summary>
		/// <param name="this">(<see langword="ref"/> parameter) The sudoku grid.</param>
		/// <param name="e">(<see langword="in"/> parameter) The event arguments.</param>
		/// <seealso cref="ValueChanged"/>
		private static void OnValueChanged(ref SudokuGrid @this, in ValueChangedArgs e)
		{
			if (e is { Cell: var cell, SetValue: var setValue and not -1 })
			{
				foreach (int peerCell in PeerMaps[cell])
				{
					if (@this.GetStatus(peerCell) == CellStatus.Empty)
					{
						// You can't do this because of being invoked recursively.
						//@this[peerCell, setValue] = false;

						@this._values[peerCell] &= (short)~(1 << setValue);
					}
				}
			}
		}

		/// <summary>
		/// The method that is pointed by the function pointer <see cref="RefreshingCandidates"/>.
		/// </summary>
		/// <param name="this">(<see langword="ref"/> parameter) The sudoku grid.</param>
		/// <seealso cref="RefreshingCandidates"/>
		private static void OnRefreshingCandidates(ref SudokuGrid @this)
		{
			for (int i = 0; i < Length; i++)
			{
				if (@this.GetStatus(i) == CellStatus.Empty)
				{
					// Remove all appeared digits.
					short mask = MaxCandidatesMask;
					foreach (int cell in PeerMaps[i])
					{
						if (@this[cell] is var digit and not -1)
						{
							mask &= (short)~(1 << digit);
						}
					}

					@this._values[i] = (short)(EmptyMask | mask);
				}
			}
		}


		/// <inheritdoc cref="Operators.operator =="/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(in SudokuGrid left, in SudokuGrid right) => Equals(left, right);

		/// <inheritdoc cref="Operators.operator !="/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(in SudokuGrid left, in SudokuGrid right) => !(left == right);
	}
}
