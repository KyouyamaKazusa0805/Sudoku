namespace Sudoku.Text.Notation;

/// <summary>
/// Represents a notation that represents for a cell or a list of cells.
/// </summary>
public sealed class CellNotation : INotation<CellNotation, CellMap, Cell, CellNotationKind>
{
	/// <summary>
	/// Try to parse the specified text, converting it into the target cell value via RxCy Notation rule.
	/// </summary>
	/// <param name="text"><inheritdoc cref="Parse(string, CellNotationKind)" path="/param[@name='text']"/></param>
	/// <returns><inheritdoc cref="Parse(string, CellNotationKind)" path="/returns"/></returns>
	/// <seealso cref="CellNotationKind.RxCy"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Cell Parse(string text) => Parse(text, CellNotationKind.RxCy);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Cell Parse(string text, CellNotationKind notation)
		=> (notation, text) switch
		{
			(CellNotationKind.RxCy, ['R' or 'r', var r and >= '1' and <= '9', 'C' or 'c', var c and >= '1' and <= '9']) => (r - '1') * 9 + c - '1',
			(CellNotationKind.RxCy, _) => throw new InvalidOperationException(),
			(CellNotationKind.K9, [var r and (>= 'A' and <= 'I' or 'K'), var c and >= '1' and <= '9']) => (r == 'K' ? 8 : r - 'A') * 9 + c - '1',
			(CellNotationKind.K9, _) => throw new InvalidOperationException(),
			_ => throw new ArgumentOutOfRangeException(nameof(notation))
		};

	/// <summary>
	/// Try to parse the specified text using basic parsing rule, converting it into a collection of type <see cref="CellMap"/>.
	/// </summary>
	/// <param name="text">The text to be parsed.</param>
	/// <returns>The target result instance of type <see cref="CellMap"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap ParseCollection(string text) => ParseCollection(text, CellNotationKind.RxCy);

	/// <inheritdoc/>
	public static unsafe CellMap ParseCollection(string text, CellNotationKind notation)
	{
		switch (notation)
		{
			case CellNotationKind.RxCy:
			{
				return simpleForm(text, out var r) || complexForm(text, out r)
					? r
					: throw new FormatException($"The specified cannot be parsed as a valid '{nameof(CellMap)}' instance.");


				static unsafe bool simpleForm(string s, out CellMap result)
				{
					if (NotationPatterns.CellOrCellListPattern_RxCy().Matches(s) is not { Count: not 0 } matches)
					{
						SkipInit(out result);
						return false;
					}

					scoped var bufferRows = (stackalloc int[9]);
					scoped var bufferColumns = (stackalloc int[9]);
					result = CellMap.Empty;

					foreach (var match in matches.Cast<Match>())
					{
						var value = match.Value;
						char* anchorR, anchorC;
						fixed (char* pValue = value)
						{
							anchorR = anchorC = pValue;

							// Find the index of the character 'C'.
							// The regular expression guaranteed the string must contain the character 'C' or 'c',
							// so we don't need to check '*p != '\0''.
							while (*anchorC is not ('C' or 'c'))
							{
								anchorC++;
							}
						}

						// Stores the possible values into the buffer.
						var (rIndex, cIndex) = (0, 0);
						for (var p = anchorR + 1; *p is not ('C' or 'c'); p++, rIndex++)
						{
							bufferRows[rIndex] = *p - '1';
						}
						for (var p = anchorC + 1; *p != '\0'; p++, cIndex++)
						{
							bufferColumns[cIndex] = *p - '1';
						}

						for (var i = 0; i < rIndex; i++)
						{
							for (var j = 0; j < cIndex; j++)
							{
								result.Add(bufferRows[i] * 9 + bufferColumns[j]);
							}
						}
					}

					return true;
				}

				static bool complexForm(string s, out CellMap result)
				{
					if (NotationPatterns.ComplexCellOrCellListPattern_RxCy().Match(s) is not { Success: true, Value: [_, .. var str, _] })
					{
						goto ReturnInvalid;
					}

					result = CellMap.Empty;
					foreach (var part in str.SplitBy([',']))
					{
						var cCharacterIndex = part.IndexOf('C', StringComparison.OrdinalIgnoreCase);
						var rows = part[1..cCharacterIndex];
						var columns = part[(cCharacterIndex + 1)..];

						foreach (var row in rows)
						{
							foreach (var column in columns)
							{
								result.Add((row - '1') * 9 + column - '1');
							}
						}
					}

					return true;

				ReturnInvalid:
					SkipInit(out result);
					return false;
				}
			}
			case CellNotationKind.K9:
			{
				// Check whether the match is successful.
				if (NotationPatterns.CellOrCellListPattern_K9().Matches(text) is not { Count: not 0 } matches)
				{
					throw new FormatException("The specified string can't match any cell instance.");
				}

				// Declare the buffer.
				var bufferRows = stackalloc int[9];
				var bufferColumns = stackalloc int[9];

				// Declare the result variable.
				var result = CellMap.Empty;

				// Iterate on each match instance.
				foreach (var match in matches.Cast<Match>())
				{
					var value = match.Value;
					char* anchorR, anchorC;
					fixed (char* pValue = value)
					{
						anchorR = anchorC = pValue;

						while (!char.IsDigit(*anchorC))
						{
							anchorC++;
						}
					}

					// Stores the possible values into the buffer.
					var rIndex = 0;
					var cIndex = 0;
					for (var p = anchorR; !char.IsDigit(*p); p++, rIndex++)
					{
						bufferRows[rIndex] = *p switch
						{
							'K' or 'k' or 'I' or 'i' => 8,
							>= 'A' and <= 'H' => *p - 'A',
							_ => *p - 'a'
						};
					}
					for (var p = anchorC; *p != '\0'; p++, cIndex++)
					{
						bufferColumns[cIndex] = *p - '1';
					}

					// Now combine two buffers.
					for (var i = 0; i < rIndex; i++)
					{
						for (var j = 0; j < cIndex; j++)
						{
							result.Add(bufferRows[i] * 9 + bufferColumns[j]);
						}
					}
				}

				// Returns the result.
				return result;
			}
			default:
			{
				throw new ArgumentOutOfRangeException(nameof(notation));
			}
		}
	}

	/// <summary>
	/// Gets the text notation that can represent the specified value.
	/// </summary>
	/// <param name="value">The value to be output and converted into a <see cref="string"/> representation.</param>
	/// <returns><inheritdoc cref="ToString(Cell, CellNotationKind)" path="/returns"/></returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToString(Cell value) => ToString(value, CellNotationKind.RxCy);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToString(Cell value, CellNotationKind notation)
		=> value switch
		{
			>= 0 and < 81 => notation switch
			{
				CellNotationKind.RxCy => $"r{value / 9 + 1}c{value % 9 + 1}",
				CellNotationKind.K9 => $"{value / 9 + 'A'}{value % 9 + 1}",
				_ => throw new ArgumentOutOfRangeException(nameof(notation))
			},
			_ => throw new ArgumentOutOfRangeException(nameof(value))
		};

	/// <summary>
	/// Gets the text notation that can represent the specified collection.
	/// </summary>
	/// <param name="collection"><inheritdoc cref="ToString(Cell)" path="/param[@name='value']"/></param>
	/// <returns><inheritdoc cref="ToString(Cell)" path="/returns"/></returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToCollectionString(scoped in CellMap collection) => ToCollectionString(collection, CellNotationKind.RxCy);

	/// <inheritdoc/>
	public static string ToCollectionString(scoped in CellMap collection, CellNotationKind notation)
	{
		static string i(Digit v) => (v + 1).ToString();
		switch (notation)
		{
			case CellNotationKind.RxCy:
			{
				return collection switch
				{
					[] => string.Empty,
					[var p] => ToString(p),
					_ => r(collection) is var a && c(collection) is var b && a.Length <= b.Length ? a : b
				};


				static unsafe string r(scoped in CellMap cells)
				{
					scoped var sbRow = new StringHandler(50);
					var dic = new Dictionary<int, List<int>>(9);
					foreach (var cell in cells)
					{
						if (!dic.ContainsKey(cell / 9))
						{
							dic.Add(cell / 9, new(9));
						}

						dic[cell / 9].Add(cell % 9);
					}
					foreach (var row in dic.Keys)
					{
						sbRow.Append('r');
						sbRow.Append(row + 1);
						sbRow.Append('c');
						sbRow.AppendRange(dic[row], &i);
						sbRow.Append(", ");
					}
					sbRow.RemoveFromEnd(2);

					return sbRow.ToStringAndClear();
				}

				static unsafe string c(scoped in CellMap cells)
				{
					var dic = new Dictionary<int, List<int>>(9);
					scoped var sbColumn = new StringHandler(50);
					foreach (var cell in cells)
					{
						if (!dic.ContainsKey(cell % 9))
						{
							dic.Add(cell % 9, new(9));
						}

						dic[cell % 9].Add(cell / 9);
					}

					foreach (var column in dic.Keys)
					{
						sbColumn.Append('r');
						sbColumn.AppendRange(dic[column], &i);
						sbColumn.Append('c');
						sbColumn.Append(column + 1);
						sbColumn.Append(", ");
					}
					sbColumn.RemoveFromEnd(2);

					return sbColumn.ToStringAndClear();
				}
			}
			case CellNotationKind.K9:
			{
				return collection switch
				{
					[] => string.Empty,
					[var p] => ToString(p, CellNotationKind.K9),
					_ => r(collection) is var a && c(collection) is var b && a.Length <= b.Length ? a : b
				};


				static unsafe string r(scoped in CellMap cells)
				{
					scoped var sbRow = new StringHandler(18);
					var dic = new Dictionary<int, List<int>>(9);
					foreach (var cell in cells)
					{
						if (!dic.ContainsKey(cell / 9))
						{
							dic.Add(cell / 9, new(9));
						}

						dic[cell / 9].Add(cell % 9);
					}
					foreach (var row in dic.Keys)
					{
						sbRow.Append((char)('A' + row));
						sbRow.AppendRange(dic[row], &i);
						sbRow.Append(", ");
					}
					sbRow.RemoveFromEnd(2);

					return sbRow.ToStringAndClear();
				}

				static unsafe string c(scoped in CellMap cells)
				{
					var dic = new Dictionary<int, List<int>>(9);
					scoped var sbColumn = new StringHandler(18);
					foreach (var cell in cells)
					{
						if (!dic.ContainsKey(cell % 9))
						{
							dic.Add(cell % 9, new(9));
						}

						dic[cell % 9].Add(cell / 9);
					}

					foreach (var column in dic.Keys)
					{
						foreach (var row in dic[column])
						{
							sbColumn.Append((char)('A' + row));
						}

						sbColumn.Append(column + 1);
						sbColumn.Append(", ");
					}
					sbColumn.RemoveFromEnd(2);

					return sbColumn.ToStringAndClear();
				}
			}
			default:
			{
				throw new ArgumentOutOfRangeException(nameof(notation));
			}
		}
	}
}
