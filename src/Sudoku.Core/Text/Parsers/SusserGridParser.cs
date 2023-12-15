using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Sudoku.Concepts;
using Sudoku.Runtime.MaskServices;
using Sudoku.Text.Converters;

namespace Sudoku.Text.Parsers;

/// <summary>
/// Represents a Susser format parser.
/// </summary>
/// <param name="ShortenSusserFormat">
/// Indicates whether the parser will use shorten mode to parse a susser format grid.
/// If the value is <see langword="true"/>, the parser will omit the continuous empty notation
/// <c>.</c>s or <c>0</c>s to a <c>*</c>.
/// </param>
/// <param name="NegateEliminationsTripletRule">
/// Indicates whether the parser will negate the rule, treating all digits as candidates existing in the grid instead of removed ones.
/// The default value is <see langword="false"/>.
/// </param>
public sealed partial record SusserGridParser(bool ShortenSusserFormat = false, bool NegateEliminationsTripletRule = false) : ISpecifiedConceptParser<Grid>
{
	/// <inheritdoc/>
	public Func<string, Grid> Parser
		=> str =>
		{
			var match = (ShortenSusserFormat ? GridShortenedSusserPattern() : GridSusserPattern()).Match(str).Value;

			if (!ShortenSusserFormat && match is not { Length: <= 405 }
				|| ShortenSusserFormat && (match is not { Length: <= 81 } || !expandCode(match, out match)))
			{
				return Grid.Undefined;
			}

			// Step 1: fills all digits.
			var (result, i) = (Grid.Empty, 0);
			if (match.Length is not (var length and not 0))
			{
				return Grid.Undefined;
			}

			for (var realPos = 0; i < length && match[i] != ':'; realPos++)
			{
				switch (match[i])
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
								result.SetDigit(realPos, nextChar - '1');

								// Add 2 on iteration variable to skip 2 characters
								// (A plus sign '+' and a digit).
								i += 2;
							}
							else
							{
								// Why isn't the character a digit character?
								return Grid.Undefined;
							}
						}
						else
						{
							return Grid.Undefined;
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
					case var c and >= '1' and <= '9':
					{
						// Is a digit character.
						// Digits are representing given values in the grid.
						// Not the plus sign, but a placeholder '0' or '.'.
						// Set value.
						result.SetDigit(realPos, c - '1');

						// Set the cell state as 'CellState.Given'.
						// If the code below doesn't make sense to you,
						// you can see the comments in method 'OnParsingSusser(string)'
						// to know the meaning also.
						result.SetState(realPos, CellState.Given);

						// Finally moves 1 step forward.
						i++;

						break;
					}
					default:
					{
						// Other invalid characters. Throws an exception.
						//throw Throwing.ParsingError<Grid>(nameof(ParsingValue));
						return Grid.Undefined;
					}
				}
			}

			// Step 2: eliminates candidates if exist.
			// If we have met the colon sign ':', this loop would not be executed.
			if (SusserEliminationsGridConverter.EliminationPattern().Match(match) is { Success: true, Value: var elimMatch })
			{
				var candidates = new HodokuTripletParser().Parser(elimMatch);
				if (!NegateEliminationsTripletRule)
				{
					// This applies for normal rule - removing candidates marked.
					foreach (var candidate in candidates)
					{
						// Set the candidate with false to eliminate the candidate.
						result.SetExistence(candidate / 9, candidate % 9, false);
					}
				}
				else
				{
					// If negate candidates, we should remove all possible candidates from all empty cells, making the grid invalid firstly.
					// Then we should add candidates onto the grid to make the grid valid.
					var distribution = candidates.CellDistribution;
					for (var cell = 0; cell < 81; cell++)
					{
						scoped ref var mask = ref result[cell];
						if (MaskOperations.MaskToCellState(mask) == CellState.Empty && distribution.TryGetValue(cell, out var digitsMask))
						{
							mask &= ~Grid.MaxCandidatesMask; // Remove all possible candidates in this cell.
							mask |= digitsMask; // Set candidates via found digits.
						}
					}
				}
			}

			return result;


			static bool expandCode(string? original, [NotNullWhen(true)] out string? result)
			{
				// We must the string code holds 8 ','s and is with no ':' or '+'.
				if (original is null || original.Contains(':') || original.Contains('+') || original.AsSpan().Count(',') != 8)
				{
					result = null;
					return false;
				}

				scoped var resultSpan = (stackalloc char[81]);
				var lines = original.Split(',');
				if (lines.Length != 9)
				{
					result = null;
					return false;
				}

				// Check per line, and expand it.
				var placeholder = original.Contains('0') ? '0' : '.';
				for (var i = 0; i < 9; i++)
				{
					var line = lines[i];
					switch (line.AsSpan().Count('*'))
					{
						case 1 when (9 + 1 - line.Length, 0, 0) is var (empties, j, k):
						{
							foreach (var c in line)
							{
								if (c == '*')
								{
									resultSpan.Slice(i * 9 + k, empties).Fill(placeholder);

									j++;
									k += empties;
								}
								else
								{
									resultSpan[i * 9 + k] = line[j];

									j++;
									k++;
								}
							}

							break;
						}

						case var n when (9 + n - line.Length, 0, 0) is var (empties, j, k):
						{
							var emptiesPerStar = empties / n;
							foreach (var c in line)
							{
								if (c == '*')
								{
									resultSpan.Slice(i * 9 + k, emptiesPerStar).Fill(placeholder);

									j++;
									k += emptiesPerStar;
								}
								else
								{
									resultSpan[i * 9 + k] = line[j];

									j++;
									k++;
								}
							}

							break;
						}
					}
				}

				result = resultSpan.ToString();
				return true;
			}
		};


	[GeneratedRegex("""[\d\.\+]{80,}(\:(\d{3}\s+)*\d{3})?""", RegexOptions.Compiled, 5000)]
	public static partial Regex GridSusserPattern();

	[GeneratedRegex("""[\d\.\*]{1,9}(,[\d\.\*]{1,9}){8}""", RegexOptions.Compiled, 5000)]
	public static partial Regex GridShortenedSusserPattern();
}
