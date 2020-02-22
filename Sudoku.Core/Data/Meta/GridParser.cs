using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sudoku.Data.Extensions;

namespace Sudoku.Data.Meta
{
	/// <summary>
	/// Encapsulates a grid parser.
	/// </summary>
	[DebuggerStepThrough]
	internal sealed class GridParser
	{
		/// <summary>
		/// Initializes an instance with parsing data.
		/// </summary>
		/// <param name="parsingValue">The string to parse.</param>
		public GridParser(string parsingValue) => ParsingValue = parsingValue;


		/// <summary>
		/// The value to parse.
		/// </summary>
		public string ParsingValue { get; }


		/// <summary>
		/// To parse the value.
		/// </summary>
		/// <returns>The grid.</returns>
		/// <exception cref="ArgumentException">
		/// Throws when failed to parse.
		/// </exception>
		public Grid Parse()
		{
			Grid? grid;
			if (!((grid = OnParsingSusser()) is null)
				|| !((grid = OnParsingSimpleMultilineGrid()) is null)
				|| !((grid = OnParsingPencilMarked(false)) is null)
				|| !((grid = OnParsingPencilMarked(true)) is null))
			{
				return grid;
			}

			// All ways are failed, therefore we cannot find a way to parse.
			// Throw an exception to report this error.
			throw new ArgumentException(
				message: $"Argument cannot be parsed and converted to target type {typeof(Grid)}.",
				paramName: nameof(ParsingValue));
		}

		/// <summary>
		/// To parse the value with a specified grid parsing type.
		/// </summary>
		/// <param name="gridParsingType">A specified parsing type.</param>
		/// <returns>The grid.</returns>
		/// <exception cref="ArgumentException">
		/// Throws when failed to parse.
		/// </exception>
		public Grid Parse(GridParsingType gridParsingType)
		{
			Grid? grid;
			if (!((grid = new Dictionary<GridParsingType, Func<Grid?>>
			{
				[GridParsingType.Susser] = OnParsingSusser,
				[GridParsingType.Table] = OnParsingSimpleMultilineGrid,
				[GridParsingType.PencilMarked] = () => OnParsingPencilMarked(false),
				[GridParsingType.PencilMarkedTreatSingleAsGiven] = () => OnParsingPencilMarked(true)
			}[gridParsingType]()) is null))
			{
				return grid;
			}
			else
			{
				throw new ArgumentException(
					message: $"Argument cannot be parsed and converted to target type {typeof(Grid)}.",
					paramName: nameof(ParsingValue));
			}
		}

		/// <summary>
		/// Parse the value using multi-line simple grid (without any candidates).
		/// </summary>
		/// <returns>The result.</returns>
		private Grid? OnParsingSimpleMultilineGrid()
		{
			string[] matches = ParsingValue.MatchAll(@"[\d\.]");
			int length = matches.Length;
			if (length != 81 && length != 83)
			{
				// Subtle grid outline will bring 2 '.'s on first line of the grid.
				return null;
			}

			var result = Grid.Empty.Clone();
			for (int i = 0; i < 81; i++)
			{
				char match = matches[length == 81 ? i : i + 2][0];
				if (match != '.' && match != '0')
				{
					result[i] = match - '1';
				}
			}

			return result;
		}

		/// <summary>
		/// Parse the PM grid.
		/// </summary>
		/// <param name="treatSingleValueAsGiven">
		/// The value indicating whether the parsing should treat
		/// the modifiable values as given ones.
		/// </param>
		/// <returns>The result.</returns>
		private Grid? OnParsingPencilMarked(bool treatSingleValueAsGiven)
		{
			string[] matches = ParsingValue.MatchAll(@"(\<\d\>|\*\d\*|\d{1,9})");
			if (matches.Length != 81)
			{
				return null;
			}

			var result = Grid.Empty.Clone();
			for (int offset = 0; offset < 81; offset++)
			{
				string s = matches[offset];
				if (treatSingleValueAsGiven)
				{
					// This options means that all characters matched will
					// contain only digit characters.
					// If the match has only one digit character, this character
					// will be treated as given at once.
					if (s.IsMatch(@"[^1-9]"))
					{
						// Matches some invalid characters,
						// which means that the parsing failed.
						return null;
					}
					else
					{
						// Check the length is 1 or not.
						// The string has only one character, which means that
						// the digit character is the given of the cell.
						int length = s.Length;
						if (length == 1)
						{
							// To assign the value, and to trigger the event
							// to modify all information of peers.
							result[offset] = s[0] - '1';
							result.SetCellStatus(offset, CellStatus.Given);
						}
						else if (length > 9)
						{
							// Greater than 9 characters is also invalid.
							return null;
						}
						else
						{
							bool[] series = new[] { true, true, true, true, true, true, true, true, true };
							foreach (char c in s)
							{
								series[c - '1'] = false;
							}
							for (int digit = 0; digit < 9; digit++)
							{
								result[offset, digit] = series[digit];
							}
						}
					}
				}
				else
				{
					// All values will be treated as normal characters:
					// '<digit>', '*digit*' and 'candidates'.
					if (s.Contains('<'))
					{
						// Givens.
						if (s.Length == 3)
						{
							char c = s[1];
							if (c >= '1' && c <= '9')
							{
								result[offset] = c - '1';
								result.SetCellStatus(offset, CellStatus.Given);
							}
							else
							{
								// Illegal characters found.
								return null;
							}
						}
						else
						{
							// The length is not 3.
							return null;
						}
					}
					else if (s.Contains('*'))
					{
						// Modifiables.
						if (s.Length == 3)
						{
							char c = s[1];
							if (c >= '1' && c <= '9')
							{
								result[offset] = c - '1';
								result.SetCellStatus(offset, CellStatus.Modifiable);
							}
							else
							{
								// Illegal characters found.
								return null;
							}
						}
						else
						{
							// The length is not 3.
							return null;
						}
					}
					else if (s.SatisfyPattern(@"[1-9]{1,9}"))
					{
						// Candidates.
						// Here do not need to check the length of the string,
						// and also all characters are digit characters.
						bool[] series = new[] { true, true, true, true, true, true, true, true, true };
						foreach (char c in s)
						{
							series[c - '1'] = false;
						}
						for (int digit = 0; digit < 9; digit++)
						{
							result[offset, digit] = series[digit];
						}
					}
					else
					{
						// All conditions cannot match.
						return null;
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Parse the susser format string.
		/// </summary>
		/// <returns>The result.</returns>
		private Grid? OnParsingSusser()
		{
			string? match = ParsingValue.Match(@"[\d\.\+]{81,}(\:(\d{3}\s+)*\d{3})?");
			if (match is null)
			{
				return null;
			}

			#region Step 1
			// Step 1: fills all digits.
			var result = Grid.Empty.Clone();
			int i = 0, length = match.Length;
			for (int realPos = 0; i < length && match[i] != ':'; realPos++)
			{
				char c = match[i];
				if (c == '+')
				{
					// Plus sign means the character after it is a digit,
					// which is modifiable value in the grid in its corresponding position.
					if (i < length - 1)
					{
						char nextChar = match[i + 1];
						if (nextChar >= '1' && nextChar <= '9')
						{
							// Set value.
							// Note that the subtracter is character '1', not '0'.
							result[realPos] = nextChar - '1';

							// Add 2 on iteration variable to skip 2 characters
							// (A plus sign '+' and a digit).
							i += 2;
						}
						else
						{
							// Why isn't the character a digit character?
							// Throws an exception to report this case.
							throw new ArgumentException(
								message: $"Argument cannot be parsed and converted to target type {typeof(Grid)}.",
								innerException: new ArgumentException(
									message: "The value after the specified argument is not a digit.",
									paramName: nameof(i)));
						}
					}
					else
					{
						throw new ArgumentException(
							message: $"Argument cannot be parsed and converted to target type {typeof(Grid)}.",
							innerException: new ArgumentOutOfRangeException(
								paramName: nameof(i),
								message: "The specified iteration argument is out of range due to invalid grid string."));
					}
				}
				else if (c == '.' || c == '0')
				{
					// A placeholder.
					// Do nothing but only move 1 step forward.
					i++;
				}
				else if (c >= '1' && c <= '9')
				{
					// Is a digit character.
					// Digits are representing given values in the grid.
					// Not the plus sign, but a placeholder '0' or '.'.
					// Set value.
					result[realPos] = c - '1';

					// Set the cell status as 'CellStatus.Given'.
					// If the code below does not make sense to you,
					// you can see the comments in method 'OnParsingSusser(string)'
					// to know the meaning also.
					result.SetCellStatus(realPos, CellStatus.Given);

					// Finally moves 1 step forward.
					i++;
				}
				else
				{
					// Other invalid characters. Throws an exception.
					throw new ArgumentException(
						message: $"Argument cannot be parsed and converted to target type {typeof(Grid)}.",
						paramName: nameof(ParsingValue));
				}
			}
			#endregion

			#region Step 2
			// Step 2: eliminates candidates if exist.
			// If we have met the colon sign ':', this loop would not be executed. 
			string? elimMatch = ParsingValue.Match(@"(?<=\:)(\d{3}\s+)*\d{3}");
			if (!(elimMatch is null))
			{
				string[] eliminationBlocks = elimMatch.MatchAll(@"\d{3}");
				foreach (string eliminationBlock in eliminationBlocks)
				{
					// Set the candidate true value to eliminate the candidate.
					result[
						offset: (eliminationBlock[1] - '1') * 9 + eliminationBlock[2] - '1',
						digit: eliminationBlock[0] - '1'] = true;
				}
			}
			#endregion

			return result;
		}
	}
}
