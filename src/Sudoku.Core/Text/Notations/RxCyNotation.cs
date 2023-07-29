namespace Sudoku.Text.Notations;

/// <summary>
/// Encapsulates a set of methods that handles a variety of instances, using RxCy notation
/// to output the <see cref="string"/> representation, or parse a <see cref="string"/> value to convert
/// it to the suitable-typed instance.
/// </summary>
/// <remarks>
/// <para>
/// The <b>RxCy notation</b> is a notation to describe a set of cells that uses letter
/// <c>R</c> (or its lower case <c>r</c>) to describe a row label, and uses the other letter
/// <c>C</c> (or its lower case <c>c</c>) to describe a column label. For example,
/// <c>R4C2</c> means the cell at row 4 and column 2.
/// </para>
/// <para>
/// For more information about this concept, please visit
/// <see href="http://sudopedia.enjoysudoku.com/Rncn.html">this link</see>.
/// </para>
/// </remarks>
public sealed partial class RxCyNotation : ICellNotation<RxCyNotation, RxCyNotationOptions>, ICandidateNotation<RxCyNotation, RxCyNotationOptions>
{
	/// <inheritdoc/>
	public static CellNotation CellNotation => CellNotation.RxCy;

	/// <inheritdoc/>
	public static CandidateNotation CandidateNotation => CandidateNotation.RxCy;


	/// <summary>
	/// Try to parse the specified <see cref="string"/> value, and convert it into the <see cref="Cell"/>
	/// instance, as the cell value.
	/// </summary>
	/// <param name="str">The <see cref="string"/> value.</param>
	/// <param name="result">
	/// The <see cref="Cell"/> result. If the return value is <see langword="false"/>,
	/// this argument will be a discard and cannot be used.
	/// </param>
	/// <returns>A <see cref="bool"/> value indicating whether the parsing operation is successful.</returns>
	public static bool TryParseCell(string str, out Cell result)
	{
		(result, var @return) = str.Trim() switch
		{
			['R' or 'r', var r and >= '1' and <= '9', 'C' or 'c', var c and >= '1' and <= '9'] => ((r - '1') * 9 + (c - '1'), true),
			_ => (0, false)
		};

		return @return;
	}

	/// <inheritdoc/>
	public static bool TryParseCells(string str, out CellMap result)
	{
		try
		{
			result = ParseCells(str);
			return true;
		}
		catch (FormatException)
		{
			SkipInit(out result);
			return false;
		}
	}

	/// <summary>
	/// Gets the <see cref="string"/> representation of a cell.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <returns>The <see cref="string"/> representation of a cell.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToCellString(Cell cell) => ToCellsString(CellsMap[cell]);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToCellsString(scoped in CellMap cells) => ToCellsString(cells, RxCyNotationOptions.Default);

	/// <inheritdoc/>
	public static string ToCellsString(scoped in CellMap cells, scoped in RxCyNotationOptions options)
	{
		var upperCasing = options.UpperCasing;
		return cells switch
		{
			[] => string.Empty,
			[var p] => $"{rowLabel(upperCasing)}{p / 9 + 1}{columnLabel(upperCasing)}{p % 9 + 1}",
			_ => r(cells, options) is var a && c(cells, options) is var b && a.Length <= b.Length ? a : b
		};


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static char rowLabel(bool upperCasing) => upperCasing ? 'R' : 'r';

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static char columnLabel(bool upperCasing) => upperCasing ? 'C' : 'c';

		static string i(Digit v) => (v + 1).ToString();

		static unsafe string r(scoped in CellMap cells, scoped in RxCyNotationOptions options)
		{
			scoped var sbRow = new StringHandler(50);
			var dic = new Dictionary<int, List<int>>(9);
			var (upperCasing, separator) = options;
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
				sbRow.Append(rowLabel(upperCasing));
				sbRow.Append(row + 1);
				sbRow.Append(columnLabel(upperCasing));
				sbRow.AppendRange(dic[row], &i);
				sbRow.Append(separator);
			}
			sbRow.RemoveFromEnd(options.Separator.Length);

			return sbRow.ToStringAndClear();
		}

		static unsafe string c(scoped in CellMap cells, scoped in RxCyNotationOptions options)
		{
			var dic = new Dictionary<int, List<int>>(9);
			scoped var sbColumn = new StringHandler(50);
			var (upperCasing, separator) = options;
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
				sbColumn.Append(rowLabel(upperCasing));
				sbColumn.AppendRange(dic[column], &i);
				sbColumn.Append(columnLabel(upperCasing));
				sbColumn.Append(column + 1);
				sbColumn.Append(separator);
			}
			sbColumn.RemoveFromEnd(options.Separator.Length);

			return sbColumn.ToStringAndClear();
		}
	}

	/// <inheritdoc/>
	public static CellMap ParseCells(string str)
	{
		if (simpleForm(str, out var r))
		{
			return r;
		}
		if (complexForm(str, out r))
		{
			return r;
		}

		throw new FormatException($"The specified cannot be parsed as a valid '{nameof(CellMap)}' instance.");


		static unsafe bool simpleForm(string s, out CellMap result)
		{
			if (CellOrCellListPattern().Matches(s) is not { Count: not 0 } matches)
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
				var rIndex = 0;
				var cIndex = 0;
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
			if (ComplexCellOrCellListPattern().Match(s) is not { Success: true, Value: [_, .. var str, _] })
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

	/// <inheritdoc/>
	public static bool TryParseCandidates(string str, out CandidateMap result)
	{
		try
		{
			result = ParseCandidates(str);
			return true;
		}
		catch (FormatException)
		{
			SkipInit(out result);
			return false;
		}
	}

	/// <summary>
	/// Gets the <see cref="string"/> representation of a candidate.
	/// </summary>
	/// <param name="candidate">The candidate.</param>
	/// <returns>The <see cref="string"/> representation of a candidate.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToCandidateString(Candidate candidate) => $"{ToCellString(candidate / 9)}({candidate % 9 + 1})";

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToCandidatesString(scoped in CandidateMap candidates) => ToCandidatesString(candidates, RxCyNotationOptions.Default);

	/// <inheritdoc/>
	public static string ToCandidatesString(scoped in CandidateMap candidates, scoped in RxCyNotationOptions options)
	{
		return candidates switch
		{
			[] => "{ }",
			[var a] when (a / 9, a % 9) is var (c, d) => $"r{c / 9 + 1}c{c % 9 + 1}({d + 1})",
			_ => f([.. candidates], options)
		};


		static string f(Candidate[] offsets, scoped in RxCyNotationOptions options)
		{
			scoped var sb = new StringHandler(50);

			foreach (var digitGroup in
				from candidate in offsets
				group candidate by candidate % 9 into digitGroups
				orderby digitGroups.Key
				select digitGroups)
			{
				var cells = CellMap.Empty;
				foreach (var candidate in digitGroup)
				{
					cells.Add(candidate / 9);
				}

				sb.Append(ToCellsString(cells, options));
				sb.Append('(');
				sb.Append(digitGroup.Key + 1);
				sb.Append(')');
				sb.Append(options.Separator);
			}

			sb.RemoveFromEnd(options.Separator.Length);
			return sb.ToStringAndClear();
		}
	}

	/// <inheritdoc/>
	public static CandidateMap ParseCandidates(string str)
	{
		if (prepositionalForm(str, out var r))
		{
			return r;
		}
		if (postpositionalForm(str, out r))
		{
			return r;
		}
		if (complexPrepositionalForm(str, out r))
		{
			return r;
		}
		if (complexPostpositionalForm(str, out r))
		{
			return r;
		}

		throw new FormatException("The target string cannot be parsed as a valid candidates collection.");


		static bool prepositionalForm(string str, out CandidateMap result)
		{
			if (Candidates_PrepositionalFormPattern().Match(str) is not { Success: true, Value: var s })
			{
				goto ReturnInvalid;
			}

			if (s.Split(['R', 'r', 'C', 'c']) is not [var digits, var rows, var columns])
			{
				goto ReturnInvalid;
			}

			var r = CandidateMap.Empty;
			foreach (var row in rows)
			{
				foreach (var column in columns)
				{
					foreach (var digit in digits)
					{
						r.Add(((row - '1') * 9 + (column - '1')) * 9 + (digit - '1'));
					}
				}
			}

			result = r;
			return true;

		ReturnInvalid:
			SkipInit(out result);
			return false;
		}

		static bool postpositionalForm(string str, out CandidateMap result)
		{
			if (Candidates_PostpositionalFormPattern().Match(str) is not { Success: true, Value: [_, .. var s, _] })
			{
				goto ReturnInvalid;
			}

			if (s.Split(['C', 'c', '(']) is not [var rows, var columns, var digits])
			{
				goto ReturnInvalid;
			}

			var r = CandidateMap.Empty;
			foreach (var row in rows)
			{
				foreach (var column in columns)
				{
					foreach (var digit in digits)
					{
						r.Add(((row - '1') * 9 + (column - '1')) * 9 + (digit - '1'));
					}
				}
			}

			result = r;
			return true;

		ReturnInvalid:
			SkipInit(out result);
			return false;
		}

		static bool complexPrepositionalForm(string str, out CandidateMap result)
		{
			if (Candidates_ComplexPrepositionalFormPattern().Match(str) is not { Success: true, Value: var s })
			{
				goto ReturnInvalid;
			}

			var cells = CellMap.Empty;
			foreach (var match in CellOrCellListPattern().Matches(s).Cast<Match>())
			{
				cells |= ParseCells(match.Value);
			}

			var digits = s[..s.IndexOf('{')];
			var r = CandidateMap.Empty;
			foreach (var cell in cells)
			{
				foreach (var digit in digits)
				{
					r.Add(cell * 9 + (digit - '1'));
				}
			}

			result = r;
			return true;

		ReturnInvalid:
			SkipInit(out result);
			return false;
		}

		static bool complexPostpositionalForm(string str, out CandidateMap result)
		{
			if (Candidates_ComplexPostpositionalFormPattern().Match(str) is not { Success: true, Value: var s })
			{
				goto ReturnInvalid;
			}

			var cells = CellMap.Empty;
			foreach (var match in CellOrCellListPattern().Matches(s).Cast<Match>())
			{
				cells |= ParseCells(match.Value);
			}

			var digits = s[(s.IndexOf('(') + 1)..s.IndexOf(')')];
			var r = CandidateMap.Empty;
			foreach (var cell in cells)
			{
				foreach (var digit in digits)
				{
					r.Add(cell * 9 + (digit - '1'));
				}
			}

			result = r;
			return true;

		ReturnInvalid:
			SkipInit(out result);
			return false;
		}
	}

	[GeneratedRegex("""(R[1-9]{1,9}C[1-9]{1,9}|r[1-9]{1,9}c[1-9]{1,9})""", RegexOptions.Compiled, 5000)]
	private static partial Regex CellOrCellListPattern();

	[GeneratedRegex("""\{\s*(R[1-9]{1,9}C[1-9]{1,9}|r[1-9]{1,9}c[1-9]{1,9})(,\s*(R[1-9]{1,9}C[1-9]{1,9}|r[1-9]{1,9}c[1-9]{1,9}))?\s*\}""", RegexOptions.Compiled, 5000)]
	private static partial Regex ComplexCellOrCellListPattern();

	[GeneratedRegex("""[1-9]{1,9}(R[1-9]{1,9}C[1-9]{1,9}|r[1-9]{1,9}c[1-9]{1,9})""", RegexOptions.Compiled, 5000)]
	private static partial Regex Candidates_PrepositionalFormPattern();

	[GeneratedRegex("""(R[1-9]{1,9}C[1-9]{1,9}|r[1-9]{1,9}c[1-9]{1,9})\([1-9]{1,9}\)""", RegexOptions.Compiled, 5000)]
	private static partial Regex Candidates_PostpositionalFormPattern();

	[GeneratedRegex("""[1-9]{1,9}\{\s*(R[1-9]{1,9}C[1-9]{1,9}|r[1-9]{1,9}c[1-9]{1,9})(,\s*(R[1-9]{1,9}C[1-9]{1,9}|r[1-9]{1,9}c[1-9]{1,9}))?\s*\}""", RegexOptions.Compiled, 5000)]
	private static partial Regex Candidates_ComplexPrepositionalFormPattern();

	[GeneratedRegex("""\{\s*(R[1-9]{1,9}C[1-9]{1,9}|r[1-9]{1,9}c[1-9]{1,9})(,\s*(R[1-9]{1,9}C[1-9]{1,9}|r[1-9]{1,9}c[1-9]{1,9}))?\s*\}\([1-9]{1,9}\)""", RegexOptions.Compiled, 5000)]
	private static partial Regex Candidates_ComplexPostpositionalFormPattern();
}
