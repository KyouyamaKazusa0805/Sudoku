using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Sudoku.Data.Extensions;
using Sudoku.Solving.Checking;

namespace Sudoku.Data.Meta
{
	public sealed class Grid : ICloneable<Grid>, IEnumerable<short>, IEquatable<Grid>, IFormattable
	{
		public static readonly Grid Empty = new Grid();


		private readonly short[] _masks;


		public Grid(short[] masks)
		{
			if (masks.Length != 81)
			{
				throw new ArgumentException(
					message: "The specified argument is invalid, because the length of this argument is not 81.",
					paramName: nameof(masks));
			}

			_masks = masks;

			ValueChanged += OnValueChanged;
		}

		private Grid()
		{
			// 512 is equal to binary number '0b01_000_000_000', where the higher 2 bits
			// can be combined a binary number of cell status.
			_masks = new short[81];
			for (int i = 0; i < 81; i++)
			{
				_masks[i] = (int)CellStatus.Empty << 9;
			}

			// Initializes the event handler.
			// Note that the default event initialization hides the back delegate field,
			// so we should use 'fake' event field to trigger the event by
			// 'Event.Invoke(objectToTrigger, eventArg)', where the variable
			// 'objectToTrigger' is always 'this'.
			ValueChanged += OnValueChanged;
		}


		public bool HasSolved
		{
			get
			{
				for (int i = 0; i < 81; i++)
				{
					if (GetCellStatus(i) == CellStatus.Empty)
					{
						return false;
					}
				}

				bool SimplyValidate()
				{
					int count = 0;
					for (int i = 0; i < 81; i++)
					{
						if (GetCellStatus(i) == CellStatus.Given)
						{
							count++;
						}

						int curDigit, peerDigit;
						if (GetCellStatus(i) != CellStatus.Empty)
						{
							curDigit = this[i];
							foreach (int peerOffset in new GridMap(i).Offsets)
							{
								if (peerOffset == i)
								{
									continue;
								}

								if ((peerDigit = this[peerOffset]) != -1 && curDigit == peerDigit)
								{
									return false;
								}
							}
						}
					}

					return count >= 17;
				}
				return SimplyValidate();
			}
		}

		public bool HasUniqueSolution => this.IsUnique(out _);


		public int this[int offset]
		{
			get
			{
				if (GetCellStatus(offset) == CellStatus.Empty)
				{
					// Empty cells does not have a fixed value.
					return -1;
				}
				else
				{
					short mask = _masks[offset];
					for (int i = 0; i < 9; i++, mask >>= 1)
					{
						if ((mask & 1) == 0)
						{
							return i;
						}
					}

					// Modifiables and givens contain no fixed digit? What the hell?
					return -1;
				}
			}
			set
			{
				if (value >= 0 && value < 9)
				{
					ref short result = ref _masks[offset];
					short copy = result;

					// Set cell status to 'CellStatus.Modifiable'.
					result = (short)((int)CellStatus.Modifiable << 9 | 511 & ~(1 << value));

					// To trigger the event, which is used for eliminate
					// all same candidates in peer cells.
					ValueChanged.Invoke(this, new ValueChangedEventArgs(offset, copy, result, value));
				}
				else if (value == -1)
				{
					// If 'value' is -1, we should re-compute all candidates.
					// Note that reset candidates may not trigger the event.
#if I_DONT_KNOW_WHY_GENERATING_BUG
					if (GetCellStatus(offset) == CellStatus.Modifiable)
					{
						short resultMask = (int)CellStatus.Empty << 9;
						foreach (int peerOffset in new GridMap(offset).Offsets)
						{
							if (peerOffset == offset)
							{
								continue;
							}
					
							// Check the digit in its peer cells aiming to re-computing
							// the candidates in the cell with offset 'offset'.
							if (GetCellStatus(peerOffset) != CellStatus.Empty)
							{
								resultMask |= (short)(1 << this[peerOffset]);
								continue;
							}
					
							// Then modify peer cells mask.
							int digit = this[offset];
							if (new GridMap(peerOffset).Offsets.All(
								o => o == peerOffset || GetCellStatus(o) == CellStatus.Empty || this[o] != digit))
							{
								_masks[peerOffset] &= (short)~(1 << digit);
							}
						}
					
						_masks[offset] = resultMask;
					}
#else
					if (GetCellStatus(offset) == CellStatus.Modifiable)
					{
						var tempGrid = Parse(ToString("."));
						for (int i = 0; i < 81; i++)
						{
							_masks[i] = tempGrid.GetMask(i);
						}
					}
#endif
				}
			}
		}

		public bool this[int offset, int digit]
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
				ValueChanged.Invoke(this, new ValueChangedEventArgs(offset, copy, result, -1));
			}
		}


		public event ValueChangedEventHandler ValueChanged;


		public void FixGrid()
		{
			for (int i = 0; i < 81; i++)
			{
				if (GetCellStatus(i) == CellStatus.Modifiable)
				{
					SetCellStatus(i, CellStatus.Given);
				}
			}
		}

		public void UnfixGrid()
		{
			for (int i = 0; i < 81; i++)
			{
				if (GetCellStatus(i) == CellStatus.Given)
				{
					SetCellStatus(i, CellStatus.Modifiable);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetCellStatus(int offset, CellStatus cellStatus)
		{
			ref short mask = ref _masks[offset];
			short copy = mask;
			mask = (short)((int)cellStatus << 9 | mask & 511);

			ValueChanged.Invoke(this, new ValueChangedEventArgs(offset, copy, mask, -1));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetMask(int offset, short value)
		{
			ref short mask = ref _masks[offset];
			short copy = mask;
			mask = value;

			ValueChanged.Invoke(this, new ValueChangedEventArgs(offset, copy, mask, -1));
		}

		public override bool Equals(object? obj) =>
			obj is Grid comparer && Equals(comparer);

		public bool Equals(Grid other) => GetHashCode() == other.GetHashCode();

		public override int GetHashCode()
		{
			int result = GetType().GetHashCode() ^ nameof(_masks).GetHashCode();

			for (int i = 0; i < 81; i++)
			{
				result ^= (i + 1) * _masks[i];
			}

			return result;
		}

		public int[] ToArray()
		{
			int[] result = new int[81];
			for (int i = 0; i < 81; i++)
			{
				// 'this[i]' is always in range -1 to 8 (-1 is empty, and 0 to 8 is 1 to 9 for
				// mankind representation).
				result[i] = this[i] + 1;
			}

			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public short GetMask(int offset) => _masks[offset];

		public override string ToString() => ToString(null, null);

		public string ToString(string format) => ToString(format, null);

		public string ToString(string? format, IFormatProvider? formatProvider)
		{
			if (formatProvider?.GetFormat(GetType()) is ICustomFormatter customFormatter)
			{
				return customFormatter.Format(format, this, formatProvider);
			}

			// Format checking.
			if (!(format is null))
			{
				if (format.Contains('#'))
				{
					if (!format.StartsWith('#'))
					{
						throw new FormatException(
							message: "The specified format is invalid.",
							innerException: new Exception(
								message: "Intelligence option character '#' must be at the first place."));
					}
					else if (format.IsMatch(@"\#[^\.0]+"))
					{
						throw new FormatException(
							message: "The specified format is invalid.",
							innerException: new Exception(
								message: "Intelligence option character '#' must be with placeholder '0' or '.'."));
					}
				}
				else if (format.Contains('0') && format.Contains('.'))
				{
					throw new FormatException(
						message: "The specified format is invalid.",
						innerException: new Exception(
							message: "Placeholder character '0' and '.' cannot appear both."));
				}
				else if (format.Contains('+') && format.Contains('!'))
				{
					throw new FormatException(
						message: "The specified format is invalid.",
						innerException: new Exception(
							message: "Cell status character '+' and '!' cannot appear both."));
				}
				else if (format.Contains(':') && !format.EndsWith(':'))
				{
					throw new FormatException(
						message: "The specified format is invalid.",
						innerException: new Exception(
							message: "Candidate leading character ':' must be at the last place."));
				}
				else if (format.Contains('@'))
				{
					if (!format.StartsWith('@'))
					{
						throw new FormatException(
							message: "The specified format is invalid.",
							innerException: new Exception(
								message: "Multiline identifier '@' must be at the first place."));
					}
					else if (format.IsMatch(@"\@[^\!\*]+"))
					{
						throw new FormatException(
							message: "The specified format is invalid.",
							innerException: new Exception(
								message: "Multiline identifier '@' must follow only character '!' or '*'."));
					}
				}
			}

			switch (format)
			{
				case null:
				case ".":
				{
					return new GridFormatter(this, false).ToString();
				}
				case "+":
				case ".+":
				case "+.":
				{
					return new GridFormatter(this, false)
					{
						WithModifiables = true
					}.ToString();
				}
				case "0":
				{
					return new GridFormatter(this, false)
					{
						Placeholder = '0'
					}.ToString();
				}
				case ":":
				{
					return new GridFormatter(this, false)
					{
						WithEliminations = true
					}.ToString().Match(@"(?<=\:)(\d{3}\s+)*\d{3}").NullableToString();
				}
				case "!":
				case ".!":
				case "!.":
				{
					return new GridFormatter(this, false)
					{
						WithModifiables = true
					}.ToString().Replace("+", string.Empty);
				}
				case "0!":
				case "!0":
				{
					return new GridFormatter(this, false)
					{
						Placeholder = '0',
						WithModifiables = true
					}.ToString().Replace("+", string.Empty);
				}
				case ".:":
				{
					return new GridFormatter(this, false)
					{
						WithEliminations = true
					}.ToString();
				}
				case "0:":
				{
					return new GridFormatter(this, false)
					{
						Placeholder = '0',
						WithEliminations = true
					}.ToString();
				}
				case "0+":
				case "+0":
				{
					return new GridFormatter(this, false)
					{
						Placeholder = '0',
						WithModifiables = true
					}.ToString();
				}
				case "+:":
				case "+.:":
				case ".+:":
				{
					return new GridFormatter(this, false)
					{
						WithModifiables = true,
						WithEliminations = true
					}.ToString();
				}
				case "0+:":
				case "+0:":
				{
					return new GridFormatter(this, false)
					{
						Placeholder = '0',
						WithModifiables = true,
						WithEliminations = true
					}.ToString();
				}
				case ".!:":
				case "!.:":
				{
					return new GridFormatter(this, false)
					{
						WithModifiables = true
					}.ToString().Replace("+", string.Empty);
				}
				case "0!:":
				case "!0:":
				{
					return new GridFormatter(this, false)
					{
						Placeholder = '0',
						WithModifiables = true
					}.ToString().Replace("+", string.Empty);
				}
				case "#":
				{
					// Formats representing 'intelligence processor' is equal to
					// format '.+:' and '0+:'.
					goto case ".+:";
				}
				case "#0":
				{
					goto case ".+:";
				}
				case "#.":
				{
					goto case "0+:";
				}
				case "@":
				{
					return new GridFormatter(this, true).ToString();
				}
				case "@!":
				{
					return new GridFormatter(this, true)
					{
						TreatValueAsGiven = true
					}.ToString();
				}
				case "@*":
				{
					return new GridFormatter(this, true)
					{
						SimpleOutputMode = true
					}.ToString();
				}
				case "@!*":
				case "@*!":
				{
					return new GridFormatter(this, true)
					{
						SimpleOutputMode = true,
						TreatValueAsGiven = true
					}.ToString();
				}
				default:
				{
					throw new FormatException("The specified format is invalid.");
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CellStatus GetCellStatus(int offset) =>
			(CellStatus)(_masks[offset] >> 9 & (int)CellStatus.All);

		public Grid Clone() => new Grid((short[])_masks.Clone());

		public IEnumerator<short> GetEnumerator() =>
			((IEnumerable<short>)_masks).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		private void OnValueChanged(object sender, ValueChangedEventArgs e)
		{
			var (offset, _, _, setValue) = e;
			if (setValue != -1)
			{
				foreach (int peerOffset in new GridMap(offset).Offsets)
				{
					if (peerOffset == offset)
					{
						// Same cell.
						continue;
					}

					// To check if the peer cell is empty or not.
					if (GetCellStatus(peerOffset) == CellStatus.Empty)
					{
						ref short peerValue = ref _masks[peerOffset];
						peerValue |= (short)(1 << setValue);
					}
				}
			}
		}


		public static Grid Parse(string str) => new GridParser(str).Parse();

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

		public static Grid CreateInstance(int[] gridValues)
		{
			var result = Empty.Clone();
			for (int i = 0; i < 81; i++)
			{
				int value = gridValues[i];
				if (value != 0)
				{
					// Calls the indexer to trigger the event
					// (Clear the candidates in peer cells).
					result[i] = value - 1;

					// Set the status to 'CellStatus.Given'.
					result.SetCellStatus(i, CellStatus.Given);
				}
			}

			return result;
		}

		public static Grid CreateInstance(int[,] gridValues)
		{
			var result = Empty.Clone();
			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					int value = gridValues[i, j];
					if (value != 0)
					{
						int pos = i * 9 + j;
						result[pos] = value - 1;
						result.SetCellStatus(pos, CellStatus.Given);
					}
				}
			}

			return result;
		}


		public static bool operator ==(Grid left, Grid right) => left.Equals(right);

		public static bool operator !=(Grid left, Grid right) => !(left == right);
	}
}
