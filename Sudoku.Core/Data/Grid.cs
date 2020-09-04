using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Security;
using Sudoku.Constants;
using Sudoku.Extensions;
using static Sudoku.Constants.Processings;
using S = Sudoku.Data.CellStatus;
using T = Sudoku.Constants.Throwings;

namespace Sudoku.Data
{
	/// <summary>
	/// Encapsulates a basic sudoku grid, which uses mask table to store all information for 81 cells.
	/// </summary>
	[DebuggerStepThrough]
	public class Grid : ICloneable<Grid>, IEnumerable, IEnumerable<short>, IEquatable<Grid?>, IReadOnlyGrid
	{
		/// <summary>
		/// Indicates the empty grid string.
		/// </summary>
		public static readonly string EmptyString = new('0', 81);

		/// <summary>
		/// Indicates an empty grid, where all values are zero.
		/// </summary>
		public static readonly IReadOnlyGrid Empty = new Grid();


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
		/// <permission cref="PermissionSet">
		/// The current project or the derived class can access this field.
		/// </permission>
		/// <seealso cref="S"/>
		protected internal readonly short[] _masks;

		/// <summary>
		/// Same as <see cref="_masks"/>, but this field stores the all masks at
		/// the initial grid. The field will not be modified until this instance
		/// destructs.
		/// </summary>
		/// <permission cref="PermissionSet">
		/// The current project or the derived class can access this field.
		/// </permission>
		/// <seealso cref="_masks"/>
		protected internal readonly short[] _initialMasks;


		/// <summary>
		/// Initializes an instance with the binary mask array.
		/// </summary>
		/// <param name="masks">The mask array.</param>
		/// <exception cref="ArgumentException">
		/// Throws when the length of the specified argument is not 81.
		/// </exception>
		public Grid(short[] masks)
		{
			if (masks.Length != 81)
			{
				throw new ArgumentException(
					message: "The specified argument is invalid, because the length of this argument is not 81.",
					paramName: nameof(masks));
			}

			_masks = masks;
			_initialMasks = (short[])masks.Clone();

			ValueChanged += OnValueChanged;
		}

		/// <include file='..\GlobalDocComments.xml' path='comments/defaultConstructor'/>
		private Grid()
		{
			// 512 is equivalent to value '0b001_000_000_000', where the higher 3 bits
			// can be combined a binary number of cell status.
			_masks = new short[81];
			for (int i = 0; i < 81; i++)
			{
				_masks[i] = DefaultMask;
			}
			_initialMasks = (short[])_masks.Clone();

			// Initializes the event handler.
			// Note that the default event initialization hides the back delegate field,
			// so we should use this field-style event to trigger the event by
			// 'Event.Invoke(objectToTrigger, eventArg)', where the variable
			// 'objectToTrigger' is always 'this'.
			ValueChanged += OnValueChanged;
		}


		/// <inheritdoc/>
		public bool HasSolved
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

		/// <inheritdoc/>
		public int CandidatesCount
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

		/// <inheritdoc/>
		public int GivensCount => Triplet.C;

		/// <inheritdoc/>
		public int ModifiablesCount => Triplet.B;

		/// <inheritdoc/>
		public int EmptiesCount => Triplet.A;

		/// <summary>
		/// The triplet of three main information.
		/// </summary>
		private unsafe (int A, int B, int C) Triplet
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


		/// <inheritdoc/>
		public virtual int this[int offset]
		{
			get => GetStatus(offset) switch
			{
				S.Empty => -1,
				S.Modifiable => (~_masks[offset]).FindFirstSet(),
				S.Given => (~_masks[offset]).FindFirstSet(),
				_ => throw T.ImpossibleCase
			};
			set
			{
				switch (value)
				{
					case -1 when GetStatus(offset) == S.Modifiable:
					{
						// If 'value' is -1, we should reset the grid.
						// Note that reset candidates may not trigger the event.
						_masks[offset] = (short)S.Empty << 9;
						RecomputeCandidates();

						break;
					}
					case >= 0 and < 9:
					{
						ref short result = ref _masks[offset];
						short copy = result;

						// Set cell status to 'CellStatus.Modifiable'.
						result = (short)((short)S.Modifiable << 9 | MaxCandidatesMask & ~(1 << value));

						// To trigger the event, which is used for eliminate
						// all same candidates in peer cells.
						ValueChanged.Invoke(this, new(offset, copy, result, value));

						break;
					}
				}
			}
		}

		/// <inheritdoc/>
		public virtual bool this[int offset, int digit]
		{
			get => (_masks[offset] >> digit & 1) != 0;
			set
			{
				ref short result = ref _masks[offset];
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
				ValueChanged.Invoke(this, new(offset, copy, result, -1));
			}
		}


		/// <summary>
		/// Indicates the event when the mask in a certain cell has changed.
		/// </summary>
		public event ValueChangedEventHandler ValueChanged;


		/// <summary>
		/// To fix a grid, which means all modifiable values will be changed
		/// to given ones.
		/// </summary>
		public virtual void Fix()
		{
			for (int i = 0; i < 81; i++)
			{
				if (GetStatus(i) == S.Modifiable)
				{
					SetStatus(i, S.Given);
				}
			}

			Array.Copy(_masks, _initialMasks, 81);
		}

		/// <summary>
		/// To unfix a grid, which means all given values will be changed
		/// to modifiable ones.
		/// </summary>
		public virtual void Unfix()
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
		public virtual void Reset() => Array.Copy(_initialMasks, _masks, 81);

		/// <summary>
		/// Set the status in a cell.
		/// </summary>
		/// <param name="offset">The cell offset you want to change.</param>
		/// <param name="cellStatus">The cell status you want to set.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual void SetStatus(int offset, S cellStatus)
		{
			ref short mask = ref _masks[offset];
			short copy = mask;
			mask = (short)((int)cellStatus << 9 | mask & MaxCandidatesMask);

			ValueChanged.Invoke(this, new(offset, copy, mask, -1));
		}

		/// <summary>
		/// Set a mask in a cell.
		/// </summary>
		/// <param name="offset">The cell offset you want to change.</param>
		/// <param name="value">The cell mask you want to set.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual void SetMask(int offset, short value)
		{
			ref short mask = ref _masks[offset];
			short copy = mask;
			mask = value;

			ValueChanged.Invoke(this, new(offset, copy, mask, -1));
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
		public override bool Equals(object? obj) => Equals(obj as Grid);

		/// <inheritdoc/>
		public bool Equals(Grid? other) => Equals(this, other);

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

		/// <inheritdoc/>
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

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public short GetMask(int offset) => _masks[offset];

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public short GetCandidateMask(int offset) => (short)(~_masks[offset] & MaxCandidatesMask);

		/// <inheritdoc/>
		public sealed override string ToString() => ToString(null, null);

		/// <summary>
		/// Returns a string that represents the current object with the grid output option.
		/// </summary>
		/// <param name="gridOutputOption">The grid output option.</param>
		/// <returns>The string.</returns>
		public string ToString(GridOutputOptions gridOutputOption) =>
			GridFormatFactory.Create(gridOutputOption).ToString(this);

		/// <include file='..\GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="string"]'/>
		public string ToString(string format) => ToString(format, null);

		/// <inheritdoc/>
		public string ToString(string? format, IFormatProvider? formatProvider)
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

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public S GetStatus(int offset) => (S)(_masks[offset] >> 9 & (int)S.All);

		/// <inheritdoc/>
		public Grid Clone() => new((short[])_masks.Clone());

		/// <inheritdoc/>
		public IEnumerator<short> GetEnumerator() => ((IEnumerable<short>)_masks).GetEnumerator();

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		/// <summary>
		/// The method, which will be invoked when the mask has changed.
		/// </summary>
		/// <param name="sender">The instance triggering the event.</param>
		/// <param name="e">The data.</param>
		private void OnValueChanged(object sender, ValueChangedEventArgs e)
		{
			var (offset, _, _, setValue) = e;
			if (setValue == -1)
			{
				return;
			}

			foreach (int cell in PeerMaps[offset])
			{
				if (GetStatus(cell) == S.Empty)
				{
					_masks[cell] |= (short)(1 << setValue);
				}
			}
		}

		/// <summary>
		/// Simply validate.
		/// </summary>
		/// <returns>The <see cref="bool"/> result.</returns>
		private bool SimplyValidate()
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
		public static Grid Parse(ReadOnlySpan<char> str) => new GridParser(str.ToString()).Parse();

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
		public static Grid Parse(string str) => new GridParser(str).Parse();

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
		public static Grid Parse(string str, bool compatibleFirst) => new GridParser(str, compatibleFirst).Parse();

		/// <summary>
		/// Parses a string value and converts to this type,
		/// using a specified grid parsing type.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <param name="gridParsingOption">The grid parsing type.</param>
		/// <returns>The result instance had converted.</returns>
		public static Grid Parse(string str, GridParsingOption gridParsingOption) =>
			new GridParser(str).Parse(gridParsingOption);

		/// <summary>
		/// Try to parse a string and converts to this type, and returns a
		/// <see cref="bool"/> value indicating the result of the conversion.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <param name="result">
		/// (<see langword="out"/> parameter) The result parsed. If the conversion is failed,
		/// this argument will be <see langword="null"/>.
		/// </param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		public static bool TryParse(string str, [NotNullWhen(true)] out Grid? result)
		{
			try
			{
				result = Parse(str);
				return true;
			}
			catch
			{
				result = null;
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
		/// this argument will be <see langword="null"/>.
		/// </param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		public static bool TryParse(string str, GridParsingOption gridParsingOption, [NotNullWhen(true)] out Grid? result)
		{
			try
			{
				result = Parse(str, gridParsingOption);
				return true;
			}
			catch
			{
				result = null;
				return false;
			}
		}

		/// <summary>
		/// Creates an instance using grid values.
		/// </summary>
		/// <param name="gridValues">The array of grid values.</param>
		/// <returns>The result instance.</returns>
		public static Grid CreateInstance(int[] gridValues)
		{
			var result = Empty.Clone();
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
		private static bool Equals(Grid? left, Grid? right)
		{
			switch ((left, right))
			{
				case (null, null):
				{
					return true;
				}
				case (not null, not null):
				{
					// Both not null.
					for (int i = 0; i < 81; i++)
					{
						if (left!._masks[i] != right!._masks[i])
						{
							return false;
						}
					}

					return true;
				}
				default:
				{
					return false;
				}
			}
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


		/// <include file='..\GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(Grid? left, Grid? right) => Equals(left, right);

		/// <include file='..\GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(Grid? left, Grid? right) => !(left == right);
	}
}
