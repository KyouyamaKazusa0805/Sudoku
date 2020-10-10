using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Sudoku.Constants;
using Sudoku.DocComments;
using Sudoku.Extensions;
using static Sudoku.Constants.Processings;
using S = Sudoku.Data.CellStatus;
using T = Sudoku.Constants.Throwings;

namespace Sudoku.Data
{
	/// <summary>
	/// Encapsulates a grid same as <see cref="Grid"/>
	/// but use <see langword="struct"/> instead of <see langword="class"/>.
	/// </summary>
	/// <remarks>
	/// Please do <b>not</b> use the default constructor <see cref="ValueGrid()"/>.
	/// </remarks>
	/// <seealso cref="Grid"/>
	/// <seealso cref="ValueGrid()"/>
	public unsafe partial struct ValueGrid : IEnumerable<short>, IEquatable<ValueGrid>, IFormattable
	{
		/// <summary>
		/// Indicates the empty grid string.
		/// </summary>
		public static readonly string EmptyString = new('0', 81);

		/// <summary>
		/// Indicates an empty grid, where all values are zero.
		/// </summary>
		public static readonly ValueGrid Empty;

		/// <summary>
		/// Indicates the undefined grid (All values are 0, uninitialized,
		/// i.e. <see langword="default"/>(<see cref="ValueGrid"/>)).
		/// </summary>
		/// <remarks>
		/// Please don't use the default constructor <see cref="ValueGrid()"/>.
		/// </remarks>
		/// <seealso cref="ValueGrid()"/>
		public static readonly ValueGrid Undefined;


		/// <summary>
		/// Indicates the default mask of a cell (an empty cell, with all 9 candidates left).
		/// </summary>
		public const short DefaultMask = (short)S.Empty << 9;

		/// <summary>
		/// Indicates the maximum candidate mask that used.
		/// </summary>
		public const short MaxCandidatesMask = 0b111_111_111;


		/// <summary>
		/// Binary masks of all cells.
		/// </summary>
		/// <remarks>
		/// <para>This array stores binary representation of decimals.</para>
		/// <para>
		/// There are 81 cells in a sudoku grid, so this data structure uses
		/// an array of size 81. Each element is a <see cref="short"/> value
		/// (but only use 12 bits), where the lower 9 bits indicates whether
		/// the digit 1 to 9 exists or not. If the corresponding value is
		/// <see langword="true"/>, or in other words, the binary representation
		/// is 1, this digit will <b>not</b> exist.
		/// </para>
		/// <para>
		/// The higher 3 bits indicates the cell status. The
		/// cases are below:
		/// <list type="table">
		/// <item>
		/// <term><c>0b001</c> (1)</term>
		/// <description>The cell is <see cref="S.Empty"/>.</description>
		/// </item>
		/// <item>
		/// <term><c>0b010</c> (2)</term>
		/// <description>The cell is <see cref="S.Modifiable"/>.</description>
		/// </item>
		/// <item>
		/// <term><c>0b100</c> (4)</term>
		/// <description>The cell is <see cref="S.Given"/>.</description>
		/// </item>
		/// </list>
		/// </para>
		/// </remarks>
		/// <seealso cref="S"/>
		internal fixed short _masks[81];

		/// <summary>
		/// Same as <see cref="_masks"/>, but this field stores the all masks at
		/// the initial grid. The field will not be modified until this instance
		/// destructs.
		/// </summary>
		/// <seealso cref="_masks"/>
		internal fixed short _initialMasks[81];

		/// <summary>
		/// The event handler triggering when the value changed.
		/// </summary>
		internal delegate* managed<ValueGrid, ValueChangedValues, void> _valueChangedEventHandler;


		/// <summary>
		/// Initializes an instance with the binary mask array (represented by a pointer) and a value indicating
		/// the length of this array.
		/// </summary>
		/// <param name="masks">The masks.</param>
		/// <param name="length">The length of the mask.</param>
		/// <exception cref="ArgumentException">
		/// Throws when the length of the specified argument is not 81.
		/// </exception>
		public ValueGrid(short* masks, int length) : this()
		{
			if (length != 81)
			{
				throw new ArgumentException(
					message: "The specified argument is invalid, because the length of this argument is not 81.",
					paramName: nameof(masks));
			}

			for (int i = 0; i < 81; i++)
			{
				_masks[i] = _initialMasks[i] = masks[i];
			}

			_valueChangedEventHandler = &OnValueChanged;
		}

		/// <summary>
		/// Initializes an instance with the binary mask array.
		/// </summary>
		/// <param name="masks">The mask array.</param>
		/// <exception cref="ArgumentException">
		/// Throws when the length of the specified argument is not 81.
		/// </exception>
		public ValueGrid(short[] masks) : this()
		{
			if (masks.Length != 81)
			{
				throw new ArgumentException(
					message: "The specified argument is invalid, because the length of this argument is not 81.",
					paramName: nameof(masks));
			}

			for (int i = 0; i < 81; i++)
			{
				_masks[i] = _initialMasks[i] = masks[i];
			}

			_valueChangedEventHandler = &OnValueChanged;
		}


		/// <inheritdoc cref="StaticConstructor"/>
		static ValueGrid()
		{
			// 512 is equivalent to value '0b001_000_000_000', where the higher 3 bits
			// can be combined a binary number of cell status.
			for (int i = 0; i < 81; i++)
			{
				Empty._masks[i] = Empty._initialMasks[i] = DefaultMask;
			}

			// Initializes the event handler.
			// Note that the default event initialization hides the back delegate field,
			// so we should use this field-style event to trigger the event by
			// 'Event(objectToTrigger, eventArg)', where the variable
			// 'objectToTrigger' is always 'this'.
			Empty._valueChangedEventHandler = &OnValueChanged;
		}


		/// <summary>
		/// Indicates the grid has already solved. If the value is <see langword="true"/>,
		/// the grid is solved; otherwise, <see langword="false"/>.
		/// </summary>
		public readonly bool HasSolved
		{
			get
			{
				for (int i = 0; i < 81; i++)
				{
					if (GetStatus(i) == S.Empty)
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
				for (int i = 0; i < 81; i++)
				{
					if (GetStatus(i) == S.Empty)
					{
						count += GetCandidateMask(i).CountSet();
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
		/// The triplet of three main information.
		/// </summary>
		private readonly (int A, int B, int C) Triplet
		{
			get
			{
				int a, b, c;
				a = b = c = 0;
				for (int i = 0; i < 81; i++)
				{
					(*(GetStatus(i) switch { S.Empty => &a, S.Modifiable => &b, S.Given => &c, _ => throw T.ImpossibleCase }))++;
				}

				return (a, b, c);
			}
		}


		/// <summary>
		/// Gets or sets a digit into a cell.
		/// </summary>
		/// <param name="cell">The cell offset you want to get or set.</param>
		/// <value>
		/// The digit you want to set. This value should be between 0 and 8.
		/// In addition, if your input is -1, the candidate mask in this cell
		/// will be re-computed. If your input is none of them above, this indexer
		/// will do nothing.
		/// </value>
		/// <returns>
		/// An <see cref="int"/> value indicating the result.
		/// If the current cell doesn't have a digit
		/// (i.e. The cell is <see cref="S.Empty"/>),
		/// The value will be -1.
		/// </returns>
		[IndexerName("Value")]
		public int this[int cell]
		{
			readonly get => GetStatus(cell) switch
			{
				S.Empty => -1,
				S.Modifiable => (~_masks[cell]).FindFirstSet(),
				S.Given => (~_masks[cell]).FindFirstSet(),
				_ => throw T.ImpossibleCase
			};
			set
			{
				switch (value)
				{
					case -1 when GetStatus(cell) == S.Modifiable:
					{
						// If 'value' is -1, we should reset the grid.
						// Note that reset candidates may not trigger the event.
						_masks[cell] = (short)S.Empty << 9;
						RecomputeCandidates();

						break;
					}
					case >= 0 and < 9:
					{
						ref short result = ref _masks[cell];
						short copy = result;

						// Set cell status to 'CellStatus.Modifiable'.
						result = (short)((short)S.Modifiable << 9 | MaxCandidatesMask & ~(1 << value));

						// To trigger the event, which is used for eliminate
						// all same candidates in peer cells.
						_valueChangedEventHandler(this, new(cell, copy, result, value));

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
			readonly get => (_masks[cell] >> digit & 1) != 0;
			set
			{
				ref short result = ref _masks[cell];
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
				_valueChangedEventHandler(this, new(cell, copy, result, -1));
			}
		}


		/// <summary>
		/// To fix a grid, which means all modifiable values will be changed
		/// to given ones.
		/// </summary>
		public void Fix()
		{
			for (int i = 0; i < 81; i++)
			{
				if (GetStatus(i) == S.Modifiable)
				{
					SetStatus(i, S.Given);
				}
			}

			fixed (short* pTarget = _masks, pBase = _initialMasks)
			{
				Memcpy(pTarget, pBase, sizeof(short));
			}
		}

		/// <summary>
		/// To unfix a grid, which means all given values will be changed
		/// to modifiable ones.
		/// </summary>
		public void Unfix()
		{
			for (int i = 0; i < 81; i++)
			{
				if (GetStatus(i) == S.Given)
				{
					SetStatus(i, S.Modifiable);
				}
			}
		}

		/// <summary>
		/// To reset the grid.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Reset()
		{
			fixed (short* pTarget = _initialMasks, pBase = _masks)
			{
				Memcpy(pTarget, pBase, sizeof(short));
			}
		}

		/// <summary>
		/// Set the status in a cell.
		/// </summary>
		/// <param name="offset">The cell offset you want to change.</param>
		/// <param name="cellStatus">The cell status you want to set.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetStatus(int offset, S cellStatus)
		{
			ref short mask = ref _masks[offset];
			short copy = mask;
			mask = (short)((int)cellStatus << 9 | mask & MaxCandidatesMask);

			_valueChangedEventHandler(this, new(offset, copy, mask, -1));
		}

		/// <summary>
		/// Set a mask in a cell.
		/// </summary>
		/// <param name="offset">The cell offset you want to change.</param>
		/// <param name="value">The cell mask you want to set.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetMask(int offset, short value)
		{
			ref short mask = ref _masks[offset];
			short copy = mask;
			mask = value;

			_valueChangedEventHandler(this, new(offset, copy, mask, -1));
		}

		/// <summary>
		/// Re-compute candidates.
		/// </summary>
		public void RecomputeCandidates()
		{
			for (int i = 0; i < 81; i++)
			{
				if (GetStatus(i) == S.Empty)
				{
					short mask = 0;
					foreach (int cell in PeerMaps[i])
					{
						int digit = this[cell];
						if (digit != -1)
						{
							mask |= (short)(1 << digit);
						}
					}

					_masks[i] = (short)((int)S.Empty << 9 | mask);
				}
			}
		}

		/// <inheritdoc/>
		public override bool Equals(object? obj) => Equals(obj is ValueGrid comparer ? comparer : Undefined);

		/// <inheritdoc/>
		public bool Equals(ValueGrid other) => Equals(this, other);

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			int result = GetType().GetHashCode() ^ nameof(_masks).GetHashCode();

			for (int i = 0; i < 81; i++)
			{
				result ^= (i + 1) * _masks[i];
			}

			return result;
		}

		/// <summary>
		/// Serializes this instance to an array, where all digit value will be stored.
		/// </summary>
		/// <returns>
		/// This array. All elements are between 0 to 9, where 0 means the
		/// cell is <see cref="S.Empty"/> now.
		/// </returns>
		public int[] ToArray()
		{
			var span = (stackalloc int[81]);
			for (int i = 0; i < 81; i++)
			{
				// 'this[i]' is always in range -1 to 8 (-1 is empty, and 0 to 8 is 1 to 9 for
				// mankind representation).
				span[i] = this[i] + 1;
			}

			return span.ToArray();
		}

		/// <summary>
		/// Get a mask of the specified cell.
		/// </summary>
		/// <param name="offset">The cell offset you want to get.</param>
		/// <returns>The mask.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public short GetMask(int offset) => _masks[offset];

		/// <summary>
		/// Get the candidate mask part of the specified cell.
		/// </summary>
		/// <param name="cell">The cell offset you want to get.</param>
		/// <returns>
		/// The candidate mask. The return value is a 9-bit <see cref="short"/>
		/// value, where the bit will be <c>0</c> if the corresponding digit <b>doesn't exist</b> in the cell,
		/// and will be <c>1</c> if the corresponding contains this digit (either the cell
		/// is filled with this digit or the cell is an empty cell, whose candidates contains the digit).
		/// </returns>
		/// <remarks>
		/// Please note that the grid masks is represented with bits, where 0 is for the digit containing in a
		/// cell, 1 is for the digit <b>not</b> containing. However, here the return mask is the reversal:
		/// 1 is for containing and 0 is for <b>not</b>.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly short GetCandidateMask(int cell) => (short)(~_masks[cell] & MaxCandidatesMask);

		/// <inheritdoc/>
		public override readonly string ToString() => ToString(null, null);

		/// <summary>
		/// Returns a string that represents the current object with the grid output option.
		/// </summary>
		/// <param name="gridOutputOption">The grid output option.</param>
		/// <returns>The string.</returns>
		public readonly string ToString(GridOutputOptions gridOutputOption) =>
			GridFormatFactory.Create(gridOutputOption).ToString(this);

		/// <inheritdoc cref="Formattable.ToString(string?)"/>
		public readonly string ToString(string format) => ToString(format, null);

		/// <inheritdoc/>
		public readonly string ToString(string? format, IFormatProvider? formatProvider)
		{
			if (formatProvider.HasFormatted(this, format, out string? result))
			{
				return result;
			}

			if (format is not null)
			{
				// Format checking.
				CheckFormatString(format);
			}

			// Returns the grid string.
			// Here you can also use switch expression to return.
			var formatter = GridFormatFactory.Create(format);
			return format switch
			{
				":" => formatter.ToString(this).Match(RegularExpressions.ExtendedSusserEliminations).NullableToString(),
				"!" => formatter.ToString(this).ToString().Replace("+", string.Empty),
				".!" or "!." or "0!" or "!0" => formatter.ToString(this).ToString().Replace("+", string.Empty),
				".!:" or "!.:" or "0!:" => formatter.ToString(this).ToString().Replace("+", string.Empty),
				_ => formatter.ToString(this)
			};
		}

		/// <summary>
		/// Get the current status for the specified cell.
		/// </summary>
		/// <param name="cell">The cell offset you want to get.</param>
		/// <returns>The cell status.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly S GetStatus(int cell) => (S)(_masks[cell] >> 9 & (int)S.All);

		/// <summary>
		/// Get all candidates containing in the specified cell.
		/// </summary>
		/// <param name="cell">The cell you want to get.</param>
		/// <returns>All candidates.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly IEnumerable<int> GetCandidates(int cell) => GetCandidateMask(cell).GetAllSets();

		/// <inheritdoc/>
		public readonly IEnumerator<short> GetEnumerator() => new Enumerator(this);

		/// <inheritdoc/>
		readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		/// <summary>
		/// The method, which will be invoked when the mask has changed.
		/// </summary>
		/// <param name="sender">The instance triggering the event.</param>
		/// <param name="e">The data.</param>
		private static void OnValueChanged(ValueGrid sender, ValueChangedValues e)
		{
			if (e is { SetValue: not -1 } and var (offset, _, _, setValue))
			{
				foreach (int cell in PeerMaps[offset])
				{
					if (sender.GetStatus(cell) == S.Empty)
					{
						sender._masks[cell] |= (short)(1 << setValue);
					}
				}
			}
		}

		/// <summary>
		/// Simply validate.
		/// </summary>
		/// <returns>The <see cref="bool"/> result.</returns>
		private readonly bool SimplyValidate()
		{
			int count = 0;
			for (int i = 0; i < 81; i++)
			{
				switch (GetStatus(i))
				{
					case S.Given:
					{
						count++;
						goto case S.Modifiable;
					}
					case S.Modifiable:
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

		/// <summary>
		/// Parses a string value and converts to this type.
		/// </summary>
		/// <param name="str">The char pointer to a string.</param>
		/// <param name="length">The length.</param>
		/// <returns>The grid result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ValueGrid Parse(char* str, int length) => Parse(new ReadOnlySpan<char>(str, length));

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
		public static ValueGrid Parse(ReadOnlySpan<char> str) => (ValueGrid)new GridParser(str.ToString()).Parse();

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
		public static ValueGrid Parse(string str) => (ValueGrid)new GridParser(str).Parse();

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
		public static ValueGrid Parse(string str, bool compatibleFirst) =>
			(ValueGrid)new GridParser(str, compatibleFirst).Parse();

		/// <summary>
		/// Parses a string value and converts to this type,
		/// using a specified grid parsing type.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <param name="gridParsingOption">The grid parsing type.</param>
		/// <returns>The result instance had converted.</returns>
		public static ValueGrid Parse(string str, GridParsingOption gridParsingOption) =>
			(ValueGrid)new GridParser(str).Parse(gridParsingOption);

		/// <summary>
		/// Try to parse a string and converts to this type, and returns a
		/// <see cref="bool"/> value indicating the result of the conversion.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <param name="result">
		/// (<see langword="out"/> parameter) The result parsed. If the conversion is failed,
		/// this argument will be <see langword="default"/>.
		/// </param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		public static bool TryParse(string str, [NotNullWhen(true)] out ValueGrid result)
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
		/// this argument will be <see langword="default"/>.
		/// </param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		public static bool TryParse(
			string str, GridParsingOption gridParsingOption, [NotNullWhen(true)] out ValueGrid result)
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
		public static ValueGrid CreateInstance(int[] gridValues)
		{
			var result = Empty;
			for (int i = 0; i < 81; i++)
			{
				if (gridValues[i] is var value and not 0)
				{
					// Calls the indexer to trigger the event
					// (Clear the candidates in peer cells).
					result[i] = value - 1;

					// Set the status to 'CellStatus.Given'.
					result.SetStatus(i, S.Given);
				}
			}

			return result;
		}

		/// <summary>
		/// To determine whether two grids are equal.
		/// </summary>
		/// <param name="left">The left grid.</param>
		/// <param name="right">The right grid.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		private static bool Equals(ValueGrid left, ValueGrid right)
		{
			for (int i = 0; i < 81; i++)
			{
				if (left._masks[i] != right._masks[i])
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// To check the format string, delegated from the method
		/// <see cref="ToString(string, IFormatProvider)"/>.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <seealso cref="ToString(string, IFormatProvider)"/>
		private static void CheckFormatString(string format)
		{
			if (format.Contains('@'))
			{
				if (!format.StartsWith('@'))
				{
					throw T.FormatErrorWithMessage(
						"Multi-line identifier '@' must be at the first place.",
						nameof(format));
				}
				else if ((format.Contains('0') || format.Contains('.')) && format.Contains(':'))
				{
					throw T.FormatErrorWithMessage(
						"In multi-line environment, '0' and '.' cannot appear with ':' together.",
						nameof(format));
				}
				else if (format.IsMatch(@"\@[^0\!\~\*\.\:]+"))
				{
					throw T.FormatErrorWithMessage(
						"Multi-line identifier '@' must follow only character '!', '*', '0', '.' or ':'.",
						nameof(format));
				}
			}
			else if (format.Contains('#'))
			{
				if (!format.StartsWith('#'))
				{
					throw T.FormatErrorWithMessage(
						"Intelligence option character '#' must be at the first place.",
						nameof(format));
				}
				else if (format.IsMatch(@"\#[^\.0]+"))
				{
					throw T.FormatErrorWithMessage(
						"Intelligence option character '#' must be with placeholder '0' or '.'.",
						nameof(format));
				}
				else if (format.Contains('0') && format.Contains('.'))
				{
					throw T.FormatErrorWithMessage(
						"Placeholder character '0' and '.' cannot appear both.",
						nameof(format));
				}
				else if (format.Contains('+') && format.Contains('!'))
				{
					throw T.FormatErrorWithMessage(
						"Cell status character '+' and '!' cannot appear both.",
						nameof(format));
				}
				else if (format.Contains(':') && !format.EndsWith(':'))
				{
					throw T.FormatErrorWithMessage(
						"Candidate leading character ':' must be at the last place.",
						nameof(format));
				}
			}
			else if (format.Contains('~'))
			{
				if (format.IsMatch(@"(\~[^\@\.0]|[^\@0\.]\~)"))
				{
					throw T.FormatErrorWithMessage(
						"Sukaku character '~' can only be together with the characters '0', '.' or '@'.",
						nameof(format));
				}
			}
			else if (format.Contains('%'))
			{
				if (format.Length > 1)
				{
					throw T.FormatErrorWithMessage(
						"Excel option character '%' cannot be used with other characters together.",
						nameof(format));
				}
			}
		}

		/// <summary>
		/// Copies the specified memory to the destination memory block,
		/// with the specified value indicating the size unit.
		/// </summary>
		/// <param name="dest">The pointer to the destination block.</param>
		/// <param name="src">The pointer to the destination block.</param>
		/// <param name="size">The number of the size unit.</param>
		private static void Memcpy(void* dest, void* src, int size)
		{
			if (src != null && dest != null && size >= 0)
			{
				byte* tempDest = (byte*)dest, tempSrc = (byte*)src;

				for (int n = size; n-- > 0; *tempDest++ = *tempSrc++) ;
			}
		}


		/// <inheritdoc cref="Operators.operator =="/>
		public static bool operator ==(ValueGrid left, ValueGrid right) => Equals(left, right);

		/// <inheritdoc cref="Operators.operator !="/>
		public static bool operator !=(ValueGrid left, ValueGrid right) => !(left == right);


		/// <summary>
		/// Implicit cast from <see cref="ValueGrid"/> to <see cref="Grid"/>.
		/// </summary>
		/// <param name="grid">The base grid.</param>
		public static implicit operator Grid(ValueGrid grid)
		{
			var result = Grid.Empty.Clone();
			for (int i = 0; i < 81; i++)
			{
				result._masks[i] = grid._masks[i];
			}

			return result;
		}

		/// <summary>
		/// Explicit cast from <see cref="Grid"/> to <see cref="ValueGrid"/>.
		/// </summary>
		/// <param name="grid">The base grid.</param>
		public static explicit operator ValueGrid(Grid grid)
		{
			var result = Empty;
			for (int i = 0; i < 81; i++)
			{
				result._masks[i] = grid._masks[i];
			}

			return result;
		}
	}
}
