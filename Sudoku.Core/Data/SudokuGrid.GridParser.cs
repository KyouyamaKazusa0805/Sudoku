using System;
using System.Linq;
using System.Text;
using Sudoku.Constants;
using Sudoku.DocComments;
using Sudoku.Extensions;

namespace Sudoku.Data
{
	partial struct SudokuGrid
	{
		/// <summary>
		/// Encapsulates a grid parser.
		/// </summary>
		public unsafe ref struct GridParser
		{
			/// <summary>
			/// The list of all methods to parse.
			/// </summary>
			private static readonly delegate*<ref GridParser, SudokuGrid>[] ParseFunctions;


			/// <summary>
			/// Initializes an instance with parsing data.
			/// </summary>
			/// <param name="parsingValue">The string to parse.</param>
			public GridParser(string parsingValue) : this(parsingValue, false)
			{
			}

			/// <summary>
			/// Initializes an instance with parsing data and a bool value
			/// indicating whether the parsing operation should use compatible mode.
			/// </summary>
			/// <param name="parsingValue">The string to parse.</param>
			/// <param name="compatibleFirst">
			/// Indicates whether the parsing operation should use compatible mode to check
			/// PM grid. See <see cref="CompatibleFirst"/> to learn more.
			/// </param>
			/// <seealso cref="CompatibleFirst"/>
			public GridParser(string parsingValue, bool compatibleFirst)
			{
				ParsingValue = parsingValue;
				CompatibleFirst = compatibleFirst;
			}


			/// <inheritdoc cref="StaticConstructor"/>
			static GridParser()
			{
				ParseFunctions = new delegate*<ref GridParser, SudokuGrid>[]
				{
					&OnParsingSimpleTable,
					&OnParsingSimpleMultilineGrid,
					&OnParsingPencilMarked_1,
					&OnParsingPencilMarked_2,
					&OnParsingSusser,
					&OnParsingExcel,
					&OnParsingSukaku_1,
					&OnParsingSukaku_2
				};

				static SudokuGrid OnParsingPencilMarked_1(ref GridParser @this) =>
					OnParsingPencilMarked(ref @this, @this.CompatibleFirst);
				static SudokuGrid OnParsingPencilMarked_2(ref GridParser @this) =>
					OnParsingPencilMarked(ref @this, !@this.CompatibleFirst);
				static SudokuGrid OnParsingSukaku_1(ref GridParser @this) =>
					OnParsingSukaku(ref @this, @this.CompatibleFirst);
				static SudokuGrid OnParsingSukaku_2(ref GridParser @this) =>
					OnParsingSukaku(ref @this, !@this.CompatibleFirst);
			}


			/// <summary>
			/// The string value to parse.
			/// </summary>
			public string ParsingValue { readonly get; private set; }

			/// <summary>
			/// Indicates whether the parser will change the execution order of PM grid.
			/// If the value is <see langword="true"/>, the parser will check compatible one
			/// first, and then check recommended parsing plan ('<c>&lt;d&gt;</c>' and '<c>*d*</c>').
			/// </summary>
			public readonly bool CompatibleFirst { get; }


			/// <summary>
			/// To parse the value.
			/// </summary>
			/// <returns>The grid.</returns>
			/// <exception cref="ArgumentException">Throws when failed to parse.</exception>
			public SudokuGrid Parse()
			{
				for (int trial = 0; trial < 8; trial++)
				{
					if (ParseFunctions[trial](ref this) is var grid && grid != Undefined)
					{
						return grid;
					}
				}

				return Undefined;
			}

			/// <summary>
			/// To parse the value with a specified grid parsing type.
			/// </summary>
			/// <param name="gridParsingOption">A specified parsing type.</param>
			/// <returns>The grid.</returns>
			public SudokuGrid Parse(GridParsingOption gridParsingOption) =>
				gridParsingOption switch
				{
					GridParsingOption.Susser => OnParsingSusser(ref this),
					GridParsingOption.Table => OnParsingSimpleMultilineGrid(ref this),
					GridParsingOption.PencilMarked => OnParsingPencilMarked(ref this, false),
					GridParsingOption.PencilMarkedTreatSingleAsGiven => OnParsingPencilMarked(ref this, true),
					GridParsingOption.SimpleTable => OnParsingSimpleTable(ref this),
					GridParsingOption.Sukaku => OnParsingSukaku(ref this, false),
					GridParsingOption.SukakuSingleLine => OnParsingSukaku(ref this, true),
					GridParsingOption.Excel => OnParsingExcel(ref this)
				};


			/// <summary>
			/// Parse the value using multi-line simple grid (without any candidates).
			/// </summary>
			/// <param name="parser">(<see langword="ref"/> parameter) The parser.</param>
			/// <returns>The result.</returns>
			private static SudokuGrid OnParsingSimpleMultilineGrid(ref GridParser parser)
			{
				string[] matches = parser.ParsingValue.MatchAll(RegularExpressions.DigitOrEmptyCell);
				if (matches.Length is var length && length is not (81 or 85))
				{
					// Subtle grid outline will bring 2 '.'s on first line of the grid.
					return Undefined;
				}

				var result = Empty;
				for (int i = 0; i < 81; i++)
				{
					string currentMatch = matches[length - 81 + i];
					switch (currentMatch.Length)
					{
						case 1 when currentMatch[0] is var match and not ('.' or '0'):
						{
							result[i] = match - '1';
							result.SetStatus(i, CellStatus.Given);

							break;
						}
						case 2 when currentMatch[1] is var match:
						{
							if (match is '.' or '0')
							{
								// '+0' or '+.'? Invalid combination.
								return Undefined;
							}
							else
							{
								result[i] = match - '1';
								result.SetStatus(i, CellStatus.Modifiable);
							}

							break;
						}
						case 1:
						{
							continue;
						}
						default:
						{
							// The sub-match contains more than 2 characters or empty string,
							// which is invalid.
							return Undefined;
						}
					}
				}

				result.UpdateInitialMasks();
				return result;
			}

			/// <summary>
			/// Parse the Excel format.
			/// </summary>
			/// <param name="parser">(<see langword="ref"/> parameter) The parser.</param>
			/// <returns>The result.</returns>
			private static SudokuGrid OnParsingExcel(ref GridParser parser)
			{
				string parsingValue = parser.ParsingValue;
				if (!parsingValue.Contains('\t'))
				{
					return Undefined;
				}

				string[] values = parsingValue.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
				if (values.Length != 9)
				{
					return Undefined;
				}

				var sb = new StringBuilder();
				foreach (string value in values)
				{
					foreach (string digitString in value.Split(new[] { '\t' }))
					{
						sb.Append(string.IsNullOrEmpty(digitString) ? '.' : digitString[0]);
					}
				}

				return SudokuGrid.Parse(sb.ToString());
			}

			/// <summary>
			/// Parse the PM grid.
			/// </summary>
			/// <param name="parser">(<see langword="ref"/> parameter) The parser.</param>
			/// <param name="treatSingleValueAsGiven">
			/// The value indicating whether the parsing should treat
			/// the modifiable values as given ones.
			/// </param>
			/// <returns>The result.</returns>
			private static SudokuGrid OnParsingPencilMarked(ref GridParser parser, bool treatSingleValueAsGiven)
			{
				// Older regular expression pattern:
				// string[] matches = ParsingValue.MatchAll(RegularExpressions.PmGridUnit_Old);
				string[] matches = parser.ParsingValue.MatchAll(RegularExpressions.PmGridUnit);
				if (matches.Length != 81)
				{
					return Undefined;
				}

				var series = (stackalloc bool[9]);
				var result = Empty;
				for (int cell = 0; cell < 81; cell++)
				{
					string s = matches[cell].Reserve(RegularExpressions.Digit);
					int length = s.Length;
					if (length > 9)
					{
						// More than 9 characters.
						return Undefined;
					}

					if (treatSingleValueAsGiven)
					{
						// This options means that all characters matched will
						// contain only digit characters.
						// Check the length is 1 or not.
						// The string has only one character, which means that
						// the digit character is the given of the cell.
						if (length == 1)
						{
							// To assign the value, and to trigger the event
							// to modify all information of peers.
							result[cell] = s[0] - '1';
							result.SetStatus(cell, CellStatus.Given);
						}
						else
						{
							series.Fill(false);
							foreach (char c in s)
							{
								series[c - '1'] = true;
							}
							for (int digit = 0; digit < 9; digit++)
							{
								result[cell, digit] = series[digit];
							}
						}
					}
					else if (s.Contains('<'))
					{
						// All values will be treated as normal characters:
						// '<digit>', '*digit*' and 'candidates'.

						// Givens.
						if (length == 3)
						{
							if (s[1] is var c and >= '1' and <= '9')
							{
								result[cell] = c - '1';
								result.SetStatus(cell, CellStatus.Given);
							}
							else
							{
								// Illegal characters found.
								return Undefined;
							}
						}
						else
						{
							// The length is not 3.
							return Undefined;
						}
					}
					else if (s.Contains('*'))
					{
						// Modifiables.
						if (length == 3)
						{
							if (s[1] is var c and >= '1' and <= '9')
							{
								result[cell] = c - '1';
								result.SetStatus(cell, CellStatus.Modifiable);
							}
							else
							{
								// Illegal characters found.
								return Undefined;
							}
						}
						else
						{
							// The length is not 3.
							return Undefined;
						}
					}
					else if (s.SatisfyPattern(RegularExpressions.PmGridCandidates))
					{
						// Candidates.
						// Here don't need to check the length of the string,
						// and also all characters are digit characters.
						series.Fill(false);
						foreach (char c in s)
						{
							series[c - '1'] = true;
						}
						for (int digit = 0; digit < 9; digit++)
						{
							result[cell, digit] = series[digit];
						}
					}
					else
					{
						// All conditions can't match.
						return Undefined;
					}
				}

				result.UpdateInitialMasks();
				return result;
			}

			/// <summary>
			/// Parse the simple table format string (Sudoku explainer format).
			/// </summary>
			/// <param name="parser">(<see langword="ref"/> parameter) The parser.</param>
			/// <returns>The grid.</returns>
			private static SudokuGrid OnParsingSimpleTable(ref GridParser parser)
			{
				if (parser.ParsingValue.Match(RegularExpressions.SimpleTable) is var match && match is null)
				{
					return Undefined;
				}

				// Remove all '\r' and '\n'-s.
				var sb = new StringBuilder();
				foreach (char c in from @char in match where @char is not ('\r' or '\n') select @char)
				{
					sb.Append(c);
				}

				parser.ParsingValue = sb.ToString();
				return OnParsingSusser(ref parser);
			}

			/// <summary>
			/// Parse the susser format string.
			/// </summary>
			/// <param name="parser">(<see langword="ref"/> parameter) The parser.</param>
			/// <returns>The result.</returns>
			private static SudokuGrid OnParsingSusser(ref GridParser parser)
			{
				var match = parser.ParsingValue.Match(RegularExpressions.Susser);
				if (match is not { Length: <= 405 })
				{
					return Undefined;
				}

				// Step 1: fills all digits.
				var result = Empty;
				int i = 0, length = match.Length;
				for (int realPos = 0; i < length && match[i] != ':'; realPos++)
				{
					char c = match[i];
					switch (c)
					{
						case '+':
						{
							// Plus sign means the character after it is a digit,
							// which is modifiable value in the grid in its corresponding position.
							if (i < length - 1)
							{
								if (match[i + 1] is var nextChar and >= '1' and <= '9')
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
									//throw new ArgumentException(
									//	message: $"Argument can't be parsed and converted to target type {typeof(Grid)}.",
									//	innerException: new ArgumentException(
									//		message: "The value after the specified argument is not a digit.",
									//		paramName: nameof(i)));
									return Undefined;
								}
							}
							else
							{
								return Undefined;
							}

							break;
						}
						case '.' or '0':
						{
							// A placeholder.
							// Do nothing but only move 1 step forward.
							i++;

							break;
						}
						case >= '1' and <= '9':
						{
							// Is a digit character.
							// Digits are representing given values in the grid.
							// Not the plus sign, but a placeholder '0' or '.'.
							// Set value.
							result[realPos] = c - '1';

							// Set the cell status as 'CellStatus.Given'.
							// If the code below doesn't make sense to you,
							// you can see the comments in method 'OnParsingSusser(string)'
							// to know the meaning also.
							result.SetStatus(realPos, CellStatus.Given);

							// Finally moves 1 step forward.
							i++;

							break;
						}
						default:
						{
							// Other invalid characters. Throws an exception.
							//throw Throwing.ParsingError<Grid>(nameof(ParsingValue));
							return Undefined;
						}
					}
				}

				// Step 2: eliminates candidates if exist.
				// If we have met the colon sign ':', this loop would not be executed.
				var elimMatch = match.Match(RegularExpressions.ExtendedSusserEliminations);
				if (elimMatch is not null)
				{
					foreach (string elimBlock in elimMatch.MatchAll(RegularExpressions.ThreeDigitsCandidate))
					{
						// Set the candidate true value to eliminate the candidate.
						result[(elimBlock[1] - '1') * 9 + elimBlock[2] - '1', elimBlock[0] - '1'] = false;
					}
				}

				result.UpdateInitialMasks();
				return result;
			}

			/// <summary>
			/// Parse the sukaku format string.
			/// </summary>
			/// <param name="parser">(<see langword="ref"/> parameter) The parser.</param>
			/// <returns>The result.</returns>
			private static SudokuGrid OnParsingSukaku(ref GridParser parser, bool compatibleFirst)
			{
				if (compatibleFirst)
				{
					if (parser.ParsingValue is var parsingValue && parsingValue.Length < 729)
					{
						return Undefined;
					}

					var result = Empty;
					fixed (char* pStr = parsingValue)
					{
						int i = 0;
						for (char* p = pStr; i < 729; p++, i++)
						{
							if (*p is var c && c is not (>= '0' and <= '9' or '.'))
							{
								return Undefined;
							}

							if (c is '0' or '.')
							{
								result[i / 9, i % 9] = false;
							}
						}
					}

					result.UpdateInitialMasks();
					return result;
				}
				else
				{
					var matches = parser.ParsingValue.MatchAll(RegularExpressions.PmGridCandidatesUnit);
					if (matches is { Length: not 81 })
					{
						return Undefined;
					}

					var series = (stackalloc bool[9]);
					var result = Empty;
					for (int offset = 0; offset < 81; offset++)
					{
						if (matches[offset].Reserve(RegularExpressions.Digit) is var s && s.Length > 9)
						{
							// More than 9 characters.
							return Undefined;
						}

						series.Fill(false);
						foreach (char c in s)
						{
							series[c - '1'] = true;
						}
						for (int digit = 0; digit < 9; digit++)
						{
							result[offset, digit] = series[digit];
						}
					}

					result.UpdateInitialMasks();
					return result;
				}
			}
		}
	}
}
