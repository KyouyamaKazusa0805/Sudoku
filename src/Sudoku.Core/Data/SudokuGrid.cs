#pragma warning disable CS1591 // Because of the false-positive of the source generator

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Extensions;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Sudoku.CodeGenerating;
using Sudoku.Data.Extensions;
using Sudoku.Versioning;
using static System.Numerics.BitOperations;
using static Sudoku.Constants;
using static Sudoku.Constants.Tables;

namespace Sudoku.Data
{
	/// <summary>
	/// Encapsulates a sudoku grid using value type instead of reference type.
	/// </summary>
#if DEBUG
	[DebuggerDisplay("{" + nameof(ToString) + "(\".+:\"),nq}")]
#endif
	[AutoDeconstruct(nameof(EmptyCells), nameof(BivalueCells), nameof(CandidateMap), nameof(DigitsMap), nameof(ValuesMap))]
	[AutoFormattable]
	public unsafe partial struct SudokuGrid : IValueEquatable<SudokuGrid>, IFormattable
	{
		/// <summary>
		/// Indicates the default mask of a cell (an empty cell, with all 9 candidates left).
		/// </summary>
		public const short DefaultMask = EmptyMask | MaxCandidatesMask;

		/// <summary>
		/// Indicates the maximum candidate mask that used.
		/// </summary>
		public const short MaxCandidatesMask = (1 << RegionCellsCount) - 1;

		/// <summary>
		/// Indicates the empty mask, modifiable mask and given mask.
		/// </summary>
		public const short
			EmptyMask = (int)CellStatus.Empty << RegionCellsCount,
			ModifiableMask = (int)CellStatus.Modifiable << RegionCellsCount,
			GivenMask = (int)CellStatus.Given << RegionCellsCount;

		/// <summary>
		/// The list of 64-based characters.
		/// </summary>
		private const string Base64List = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ,.";

		/// <summary>
		/// Indicates the size of each region.
		/// </summary>
		private const byte RegionCellsCount = 9;

		/// <summary>
		/// Indicates the size of each grid.
		/// </summary>
		private const byte Length = 81;

		/// <summary>
		/// Indicates the length of the string <see cref="Base64List"/>.
		/// </summary>
		/// <seealso cref="Base64List"/>
		private const byte Base64Length = 64;


		/// <summary>
		/// Indicates the empty grid string.
		/// </summary>
		public static readonly string EmptyString = new('0', Length);

		/// <summary>
		/// Indicates the event triggered when the value is changed.
		/// </summary>
		public static readonly delegate*<ref SudokuGrid, in ValueChangedArgs, void> ValueChanged;

		/// <summary>
		/// Indicates the event triggered when should re-compute candidates.
		/// </summary>
		public static readonly delegate*<ref SudokuGrid, void> RefreshingCandidates;

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
		/// The lookup table.
		/// </summary>
		private static readonly IReadOnlyDictionary<char, int> Lookup;


		/// <summary>
		/// Indicates the inner array that stores the masks of the sudoku grid, where:
		/// <list type="table">
		/// <item>
		/// <term><c>_values</c></term>
		/// <description>Stores the in-time sudoku grid inner information.</description>
		/// </item>
		/// <item>
		/// <term><c>_initialValues</c></term>
		/// <description>Stores the initial information of a sudoku grid.</description>
		/// </item>
		/// </list>
		/// </summary>
		private fixed short _values[Length], _initialValues[Length];


		/// <summary>
		/// Creates an instance using grid values.
		/// </summary>
		/// <param name="gridValues">The array of grid values.</param>
		public SudokuGrid(int[] gridValues) : this(gridValues, GridCreatingOption.None)
		{
		}

		/// <summary>
		/// Creates an instance using grid values.
		/// </summary>
		/// <param name="gridValues">The array of grid values.</param>
		/// <param name="creatingOption">The grid creating option.</param>
		public SudokuGrid(int[] gridValues, GridCreatingOption creatingOption)
		{
			this = Empty;
			for (int i = 0; i < Length; i++)
			{
				if (gridValues[i] is var value and not 0)
				{
					// Calls the indexer to trigger the event
					// (Clear the candidates in peer cells).
					this[i] = creatingOption == GridCreatingOption.MinusOne ? value - 1 : value;

					// Set the status to 'CellStatus.Given'.
					SetStatus(i, CellStatus.Given);
				}
			}
		}

		/// <summary>
		/// Try to parse a token, and converts the token to the sudoku grid instance.
		/// </summary>
		/// <param name="token">The token.</param>
		public SudokuGrid(string token)
		{
			var bi = BigInteger.Zero;
			for (int i = 0, length = token.Length; i < length; i++)
			{
				bi += Lookup[token[i]] * BigInteger.Pow(Base64Length, i);
			}

			this = Parse(bi.ToString().PadLeft(Length, '0'));
		}

		/// <summary>
		/// Initializes an instance with the specified mask array.
		/// </summary>
		/// <param name="masks">The masks.</param>
		/// <exception cref="ArgumentException">Throws when <see cref="Array.Length"/> is not 81.</exception>
		internal SudokuGrid(short[] masks)
		{
#if DEBUG
			Debug.Assert(
				masks.Length == Length,
				$"The length of the array argument should be {Length.ToString()}."
			);
#endif

			fixed (short* pArray = masks, pValues = _values, pInitialValues = _initialValues)
			{
				Unsafe.CopyBlock(pValues, pArray, sizeof(short) * Length);
				Unsafe.CopyBlock(pInitialValues, pArray, sizeof(short) * Length);
			}
		}


		static SudokuGrid()
		{
			// Initializes the empty grid.
#pragma warning disable SD0303
			Empty = default;
#pragma warning restore SD0303
			fixed (short* p = Empty._values, q = Empty._initialValues)
			{
				int i = 0;
				for (short* ptrP = p, ptrQ = q; i < Length; *ptrP++ = *ptrQ++ = DefaultMask, i++) ;
			}

			// Lookup table.
			Lookup = new Dictionary<char, int>(
				from i in Enumerable.Range(0, Base64Length)
				select new KeyValuePair<char, int>(Base64List[i], i)
			);

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
		/// Indicates whether the grid is <see cref="Undefined"/>, which means the grid
		/// holds totally same value with <see cref="Undefined"/>.
		/// </summary>
		/// <seealso cref="Undefined"/>
		public readonly bool IsUndefined
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => this == Undefined;
		}

		/// <summary>
		/// Indicates whether the grid is <see cref="Empty"/>, which means the grid
		/// holds totally same value with <see cref="Empty"/>.
		/// </summary>
		/// <seealso cref="Empty"/>
		public readonly bool IsEmpty
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => this == Empty;
		}

#if DEBUG
		/// <summary>
		/// Indicates whether the grid is as same behaviors as <see cref="Undefined"/>
		/// in debugging mode.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This property checks whether all non-first masks are all 0. This checking behavior
		/// is aiming to the debugger because the debugger can't recognize the fixed buffer.
		/// </para>
		/// <para>
		/// The debugger can't recognize fixed buffer.
		/// The fixed buffer whose code is like:
		/// <code>
		/// private fixed short _values[81];
		/// </code>
		/// However, internally, the field <c>_values</c> is implemented
		/// with a fixed buffer using a inner struct, which is just like:
		/// <code>
		/// [StructLayout(LayoutKind.Explicit, Size = 81 * sizeof(short))]
		/// private struct FixedBuffer
		/// {
		///     public short _internalValue;
		/// }
		/// </code>
		/// And that field:
		/// <code>
		/// private FixedBuffer _fixedField;
		/// </code>
		/// From the code we can learn that only 2 bytes of the inner struct can be detected,
		/// because the buffer struct only contains 2 bytes data.
		/// </para>
		/// </remarks>
		/// <see cref="Undefined"/>
		public readonly bool IsDebuggerUndefined
		{
			get
			{
				fixed (short* pGrid = _values)
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
		}
#endif

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
		/// Gets the token of this sudoku grid.
		/// </summary>
		public readonly string Token
		{
			get
			{
				// The maximum grid as the base 64 is of length 45.
				var sb = new ValueStringBuilder(stackalloc char[45]);
				for (var temp = BigInteger.Parse(EigenString); temp > 0; temp /= Base64Length)
				{
					sb.Append(Base64List[(int)(temp % Base64Length)]);
				}

				return sb.ToString();
			}
		}

		/// <summary>
		/// Indicates the eigen string value that can introduce the current sudoku grid.
		/// </summary>
		public readonly string EigenString => ToString("0").TrimStart('0');

		/// <summary>
		/// Indicates the cells that corresponding position in this grid is empty.
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

				static bool p(in SudokuGrid g, int cell) => g.GetStatus(cell) == CellStatus.Empty;
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

				static bool p(in SudokuGrid g, int cell) => PopCount((uint)g.GetCandidates(cell)) == 2;
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

				static bool p(in SudokuGrid g, int cell, int digit) => g.Exists(cell, digit) is true;
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

				static bool p(in SudokuGrid g, int cell, int digit) => (g.GetCandidates(cell) >> digit & 1) != 0;
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

				static bool p(in SudokuGrid g, int cell, int digit) => g[cell] == digit;
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
		/// <returns>
		/// The value that the cell filled with. The possible values are:
		/// <list type="table">
		/// <item>
		/// <term>-2</term>
		/// <description>The status of the specified cell is <see cref="CellStatus.Undefined"/>.</description>
		/// </item>
		/// <item>
		/// <term>-1</term>
		/// <description>The status of the specified cell is <see cref="CellStatus.Empty"/>.</description>
		/// </item>
		/// <item>
		/// <term>0 to 8</term>
		/// <description>
		/// The actual value that the cell filled with. 0 is for the digit 1, 1 is for the digit 2, etc..
		/// </description>
		/// </item>
		/// </list>
		/// </returns>
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
					case >= 0 and < RegionCellsCount:
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
		public bool this[int cell, int digit]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get => (_values[cell] >> digit & 1) != 0;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				if ((Cell: cell, Digit: digit) is (Cell: >= 0 and < Length, Digit: >= 0 and < RegionCellsCount))
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
		/// <term><see langword="true"/></term>
		/// <description>
		/// The cell is an empty cell <b>and</b> contains the specified digit.
		/// </description>
		/// </item>
		/// <item>
		/// <term><see langword="false"/></term>
		/// <description>
		/// The cell is an empty cell <b>but doesn't</b> contain the specified digit.
		/// </description>
		/// </item>
		/// <item>
		/// <term><see langword="null"/></term>
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
		public override readonly int GetHashCode() => this switch
		{
			{ IsUndefined: true } => 0,
#if DEBUG
			{ IsDebuggerUndefined: true } => 0,
#endif
			{ IsEmpty: true } => 1,
			_ => ToString("#").GetHashCode()
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
		public readonly ref readonly short GetPinnableReference() => ref _values[0];

		/// <summary>
		/// Returns a reference to the element of the <see cref="SudokuGrid"/> at index zero.
		/// </summary>
		/// <param name="pinnedItem">
		/// The item you want to fix. If the value is
		/// <list type="table">
		/// <item>
		/// <term><see cref="PinnedItem.CurrentGrid"/></term>
		/// <description>The current grid mask list of pointer value will be returned.</description>
		/// </item>
		/// <item>
		/// <term><see cref="PinnedItem.InitialGrid"/></term>
		/// <description>The initial grid mask list of pointer value will be returned.</description>
		/// </item>
		/// <item>
		/// <term>Otherwise</term>
		/// <description>The reference of <see langword="null"/>.</description>
		/// </item>
		/// </list>
		/// </param>
		/// <returns>A reference to the element of the <see cref="SudokuGrid"/> at index zero.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[return: MaybeNullWhenNotDefined("pinnedItem")]
		public readonly ref readonly short GetPinnableReference(PinnedItem pinnedItem) =>
			ref pinnedItem == PinnedItem.CurrentGrid
			? ref GetPinnableReference()
			: ref pinnedItem == PinnedItem.InitialGrid
			? ref _initialValues[0]
			: ref *(short*)null;

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
		public readonly string ToMaskString()
		{
			const string separator = ", ";
			fixed (short* pArr = _values)
			{
				var sb = new ValueStringBuilder(400);
				sb.AppendRange(pArr, Length, &p, separator);
				return sb.ToString();
			}

			static string p(short v) => v.ToString();
		}

		[NonVersionable]
		public override readonly partial string ToString();

		[NonVersionable]
		public readonly partial string ToString(string? format);

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[NonVersionable]
		public readonly string ToString(string? format, IFormatProvider? formatProvider) => this switch
		{
			{ IsEmpty: true } => "<Empty>",
			{ IsUndefined: true } => "<Undefined>",
#if DEBUG
			{ IsDebuggerUndefined: true } => "<Debugger can't recognize the fixed buffer>",
#endif
			_ when formatProvider.HasFormatted(this, format, out string? result) => result,
			_ when Formatter.Create(format) is var f => format switch
			{
				":" => f.ToString(this).Match(RegularExpressions.ExtendedSusserEliminations) ?? string.Empty,
				"!" => f.ToString(this).Replace("+", string.Empty),
				".!" or "!." or "0!" or "!0" => f.ToString(this).Replace("+", string.Empty),
				".!:" or "!.:" or "0!:" => f.ToString(this).Replace("+", string.Empty),
				_ => f.ToString(this)
			}
		};

		/// <summary>
		/// Get the cell status at the specified cell.
		/// </summary>
		/// <param name="cell">The cell.</param>
		/// <returns>The cell status.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly CellStatus GetStatus(int cell) => _values[cell].MaskToStatus();

		/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
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
		/// To unfix the current grid (all given values will be changed to modifiable ones).
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
				Unsafe.CopyBlock(pValues, pInitialValues, sizeof(short) * Length);
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
			mask = (short)((int)status << RegionCellsCount | mask & MaxCandidatesMask);

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
				Unsafe.CopyBlock(pInitialValues, pValues, sizeof(short) * Length);
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
		private readonly Cells[] GetMap(delegate*<in SudokuGrid, int, int, bool> predicate)
		{
			var result = new Cells[RegionCellsCount];
			for (int digit = 0; digit < RegionCellsCount; digit++)
			{
				ref var map = ref result[digit];
				for (int cell = 0; cell < Length; cell++)
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
		private readonly Cells GetCells(delegate*<in SudokuGrid, int, bool> predicate)
		{
			var result = Cells.Empty;
			for (int cell = 0; cell < Length; cell++)
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
		[ProxyEquality]
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
		/// <see cref="Parse(string, GridParsingOption)"/> instead of this method.
		/// </para>
		/// </summary>
		/// <param name="str">The string.</param>
		/// <returns>The result instance had converted.</returns>
		/// <seealso cref="Parse(string, GridParsingOption)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SudokuGrid Parse(in ReadOnlySpan<char> str) => new Parser(str.ToString()).Parse();

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
		public static SudokuGrid Parse(string str) => new Parser(str).Parse();

		/// <summary>
		/// <para>
		/// Parses a string value and converts to this type.
		/// </para>
		/// <para>
		/// If you want to parse a PM grid, you should decide the mode to parse.
		/// If you use compatible mode to parse, all single values will be treated as
		/// given values; otherwise, recommended mode, which uses '<c><![CDATA[<d>]]></c>'
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
		/// Parses a string value and converts to this type, using a specified grid parsing type.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <param name="gridParsingOption">The grid parsing type.</param>
		/// <returns>The result instance had converted.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SudokuGrid Parse(string str, GridParsingOption gridParsingOption) =>
			new Parser(str).Parse(gridParsingOption);

		/// <summary>
		/// Try to parse a string and converts to this type, and returns a
		/// <see cref="bool"/> value indicating the result of the conversion.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <param name="result">
		/// The result parsed. If the conversion is failed,
		/// this argument will be <see cref="Undefined"/>.
		/// </param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		/// <seealso cref="Undefined"/>
		public static bool TryParse(string str, out SudokuGrid result)
		{
			try
			{
				result = Parse(str);
				return !result.IsUndefined;
			}
			catch (FormatException)
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
		/// The result parsed. If the conversion is failed,
		/// this argument will be <see cref="Undefined"/>.
		/// </param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		/// <seealso cref="Undefined"/>
		public static bool TryParse(string str, GridParsingOption option, out SudokuGrid result)
		{
			try
			{
				result = Parse(str, option);
				return true;
			}
			catch (FormatException)
			{
				result = Undefined;
				return false;
			}
		}

		/// <summary>
		/// The method that is pointed by the function pointer <see cref="ValueChanged"/>.
		/// </summary>
		/// <param name="this">The sudoku grid.</param>
		/// <param name="e">The event arguments.</param>
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
		/// <param name="this">The sudoku grid.</param>
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


		/// <summary>
		/// Returns the segment via the specified region and the sudoku grid to filter.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="region">The region.</param>
		/// <returns>The segment.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SudokuGridSegment operator /(in SudokuGrid grid, int region) => new(grid, region);
	}
}
