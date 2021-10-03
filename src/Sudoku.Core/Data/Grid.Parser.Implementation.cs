namespace Sudoku.Data;

partial struct Grid
{
	partial struct Parser
	{
		/// <summary>
		/// Parse the value using multi-line simple grid (without any candidates).
		/// </summary>
		/// <param name="parser">The parser.</param>
		/// <returns>The result.</returns>
		private static partial Grid OnParsingSimpleMultilineGrid(ref Parser parser)
		{
			string[] matches = parser.ParsingValue.MatchAll(RegularExpressions.DigitOrEmptyCell);
			int length = matches.Length;
			if (length is not (81 or 85))
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
					case 1:
					{
						continue;
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
					default:
					{
						// The sub-match contains more than 2 characters or empty string,
						// which is invalid.
						return Undefined;
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Parse the Excel format.
		/// </summary>
		/// <param name="parser">The parser.</param>
		/// <returns>The result.</returns>
		private static partial Grid OnParsingExcel(ref Parser parser)
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

			var sb = new ValueStringBuilder(stackalloc char[81]);
			foreach (string value in values)
			{
				foreach (string digitString in value.Split(new[] { '\t' }))
				{
					unsafe
					{
						fixed (char* c = digitString)
						{
							sb.Append(string.IsNullOrEmpty(digitString) ? '.' : *c);
						}
					}
				}
			}

			return Grid.Parse(sb.ToStringAndClear());
		}

		/// <summary>
		/// Parse the open sudoku format grid.
		/// </summary>
		/// <param name="parser">The parser.</param>
		/// <returns>The result.</returns>
		private static partial Grid OnParsingOpenSudoku(ref Parser parser)
		{
			if (parser.ParsingValue.Match(RegularExpressions.OpenSudoku) is not { } match)
			{
				return Undefined;
			}

			var result = Empty;
			for (int i = 0; i < 81; i++)
			{
				switch (match[i * 6])
				{
					case '0' when whenClause(i * 6, match, "|0|1", "|0|1|"):
					{
						continue;
					}
					case not '0' and var ch when whenClause(i * 6, match, "|0|0", "|0|0|"):
					{
						result[i] = ch - '1';
						result.SetStatus(i, CellStatus.Given);

						break;
					}
					default:
					{
						// Invalid string status.
						return Undefined;
					}
				}
			}

			return result;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static bool whenClause(int i, string match, string pattern1, string pattern2) =>
				i == 80 * 6 ? match[(i + 1)..(i + 5)] == pattern1 : match[(i + 1)..(i + 6)] == pattern2;
		}

		/// <summary>
		/// Parse the PM grid.
		/// </summary>
		/// <param name="parser">The parser.</param>
		/// <returns>The result.</returns>
		private static partial Grid OnParsingPencilMarked(ref Parser parser)
		{
			// Older regular expression pattern:
			if (parser.ParsingValue.MatchAll(RegularExpressions.PmGridUnit) is not { Length: 81 } matches)
			{
				return Undefined;
			}

			var result = Empty;
			for (int cell = 0; cell < 81; cell++)
			{
				if (matches[cell] is not { Length: var length and <= 9 } s)
				{
					// More than 9 characters.
					return Undefined;
				}

				if (s.Contains('<'))
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
					short mask = 0;
					foreach (char c in s)
					{
						mask |= (short)(1 << c - '1');
					}

					if (mask == 0)
					{
						return Undefined;
					}

					if ((mask & mask - 1) == 0)
					{
						result[cell] = TrailingZeroCount(mask);
						result.SetStatus(cell, CellStatus.Given);
					}
					else
					{
						for (int digit = 0; digit < 9; digit++)
						{
							result[cell, digit] = (mask >> digit & 1) != 0;
						}
					}
				}
				else
				{
					// All conditions can't match.
					return Undefined;
				}
			}

			return result;
		}

		/// <summary>
		/// Parse the simple table format string (Sudoku explainer format).
		/// </summary>
		/// <param name="parser">The parser.</param>
		/// <returns>The grid.</returns>
		private static partial Grid OnParsingSimpleTable(ref Parser parser)
		{
			if (parser.ParsingValue.Match(RegularExpressions.SimpleTable) is not { } match)
			{
				return Undefined;
			}

			// Remove all '\r's and '\n's.
			var sb = new ValueStringBuilder(stackalloc char[81 + (9 << 1)]);
			foreach (char c in from @char in match where @char is not ('\r' or '\n') select @char)
			{
				sb.Append(c);
			}

			parser.ParsingValue = sb.ToStringAndClear();
			return OnParsingSusser(ref parser);
		}

		/// <summary>
		/// Parse the susser format string.
		/// </summary>
		/// <param name="parser">The parser.</param>
		/// <returns>The result.</returns>
		private static partial Grid OnParsingSusser(ref Parser parser)
		{
			string? match = parser.ParsingValue.Match(RegularExpressions.Susser);
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
								// Note that the subtractor is character '1', not '0'.
								result[realPos] = nextChar - '1';

								// Add 2 on iteration variable to skip 2 characters
								// (A plus sign '+' and a digit).
								i += 2;
							}
							else
							{
								// Why isn't the character a digit character?
								return Undefined;
							}
						}
						else
						{
							return Undefined;
						}

						break;
					}
					case '.':
					case '0':
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
			if (match.Match(RegularExpressions.ExtendedSusserEliminations) is { } elimMatch)
			{
				foreach (string elimBlock in elimMatch.MatchAll(RegularExpressions.ThreeDigitsCandidate))
				{
					// Set the candidate true value to eliminate the candidate.
					result[(elimBlock[1] - '1') * 9 + elimBlock[2] - '1', elimBlock[0] - '1'] = false;
				}
			}

			return result;
		}

		/// <summary>
		/// Parse the sukaku format string.
		/// </summary>
		/// <param name="parser">The parser.</param>
		/// <param name="compatibleFirst">
		/// Indicates whether the algorithm uses compatibility mode to check and parse sudoku grid.
		/// </param>
		/// <returns>The result.</returns>
		private static partial Grid OnParsingSukaku(ref Parser parser, bool compatibleFirst)
		{
			if (compatibleFirst)
			{
				string parsingValue = parser.ParsingValue;
				if (parsingValue.Length < 729)
				{
					return Undefined;
				}

				var result = Empty;
				unsafe
				{
					fixed (char* pStr = parsingValue)
					{
						int i = 0;
						for (char* p = pStr; i < 729; p++, i++)
						{
							char c = *p;
							if (c is not (>= '0' and <= '9' or '.'))
							{
								return Undefined;
							}

							if (c is '0' or '.')
							{
								result[i / 9, i % 9] = false;
							}
						}
					}
				}

				return result;
			}
			else
			{
				string[] matches = parser.ParsingValue.MatchAll(RegularExpressions.PmGridCandidatesUnit);
				if (matches is { Length: not 81 })
				{
					return Undefined;
				}

				var result = Empty;
				for (int offset = 0; offset < 81; offset++)
				{
					string s = matches[offset].Reserve(RegularExpressions.Digit);
					if (s.Length > 9)
					{
						// More than 9 characters.
						return Undefined;
					}

					short mask = 0;
					foreach (char c in s)
					{
						mask |= (short)(1 << c - '1');
					}

					if (mask == 0)
					{
						return Undefined;
					}

					// We don't need to set the value as a given because the current parsing
					// if for sukakus, rather than normal sudokus.
					//if ((mask & mask - 1) == 0)
					//{
					//	result[offset] = TrailingZeroCount(mask);
					//	result.SetStatus(offset, CellStatus.Given);
					//}

					for (int digit = 0; digit < 9; digit++)
					{
						result[offset, digit] = (mask >> digit & 1) != 0;
					}
				}

				return result;
			}
		}
	}
}
