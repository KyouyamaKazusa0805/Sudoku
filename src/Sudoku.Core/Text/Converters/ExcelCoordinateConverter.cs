namespace Sudoku.Text.Converters;

/// <summary>
/// Represents for Excel coordinate-based converter.
/// </summary>
/// <param name="MakeLettersUpperCase">
/// <para>Indicates whether we make the letters be upper-casing.</para>
/// <para>The value is <see langword="false"/> by default.</para>
/// </param>
/// <param name="DefaultSeparator"><inheritdoc/></param>
/// <param name="DigitsSeparator"><inheritdoc/></param>
/// <param name="CurrentCulture"><inheritdoc/></param>
public sealed record ExcelCoordinateConverter(
	bool MakeLettersUpperCase = false,
	string DefaultSeparator = ", ",
	string? DigitsSeparator = null,
	CultureInfo? CurrentCulture = null
) : CoordinateConverter(DefaultSeparator, DigitsSeparator, CurrentCulture)
{
	/// <inheritdoc/>
	public override CellNotationConverter CellConverter
		=> (scoped ref readonly CellMap cells) =>
		{
			switch (cells)
			{
				case []: { return string.Empty; }
				case [var p]:
				{
					var row = p / 9;
					var column = p % 9;
					var columnCharacter = (char)((MakeLettersUpperCase ? 'A' : 'a') + column);
					return $"{DigitConverter((Mask)(1 << row))}{columnCharacter}";
				}
				default: { return r(in cells) is var a && c(in cells) is var b && a.Length <= b.Length ? a : b; }
			}


			string r(scoped ref readonly CellMap cells)
			{
				var sbRow = new StringBuilder(18);
				var dic = new Dictionary<Cell, List<ColumnIndex>>(9);
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
					sbRow.AppendRange(dic[row].AsReadOnlySpan(), column => ((MakeLettersUpperCase ? 'A' : 'a') + column).ToString());
					sbRow.Append(row + 1);
					sbRow.Append(DefaultSeparator);
				}
				return sbRow.RemoveFrom(^DefaultSeparator.Length).ToString();
			}

			string c(scoped ref readonly CellMap cells)
			{
				var dic = new Dictionary<Digit, List<RowIndex>>(9);
				var sbColumn = new StringBuilder(18);
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
					sbColumn.Append((char)((MakeLettersUpperCase ? 'A' : 'a') + column));
					sbColumn.AppendRange(dic[column].AsReadOnlySpan(), static row => (row + 1).ToString());
					sbColumn.Append(DefaultSeparator);
				}
				return sbColumn.RemoveFrom(^DefaultSeparator.Length).ToString();
			}
		};

	/// <inheritdoc/>
	public override CandidateNotationConverter CandidateConverter
		=> (scoped ref readonly CandidateMap candidates) =>
		{
			var sb = new StringBuilder(50);
			foreach (var digitGroup in
				from candidate in candidates
				group candidate by candidate % 9 into digitGroups
				orderby digitGroups.Key
				select digitGroups)
			{
				var cells = (CellMap)[];
				foreach (var candidate in digitGroup)
				{
					cells.Add(candidate / 9);
				}

				sb.Append(CellConverter(in cells));
				sb.Append('.');
				sb.Append(digitGroup.Key + 1);

				sb.Append(DefaultSeparator);
			}
			return sb.RemoveFrom(^DefaultSeparator.Length).ToString();
		};

	/// <inheritdoc/>
	public override HouseNotationConverter HouseConverter
		=> new K9Converter(MakeLettersUpperCase, DefaultSeparator: DefaultSeparator, DigitsSeparator: DigitsSeparator).HouseConverter;

	/// <inheritdoc/>
	public override ConclusionNotationConverter ConclusionConverter
		=> (scoped ReadOnlySpan<Conclusion> conclusions) =>
		{
			return conclusions switch
			{
			[] => string.Empty,
			[(var t, var c, var d)] => $"{CellConverter([c])}{t.Notation()}{DigitConverter((Mask)(1 << d))}",
				_ => toString(conclusions)
			};


			static int cmp(scoped ref readonly Conclusion left, scoped ref readonly Conclusion right) => left.CompareTo(right);

			unsafe string toString(scoped ReadOnlySpan<Conclusion> c)
			{
				var conclusions = new Conclusion[c.Length];
				Unsafe.CopyBlock(
					ref Ref.AsByteRef(ref conclusions[0]),
					in Ref.AsReadOnlyByteRef(in c[0]),
					(uint)(sizeof(Conclusion) * c.Length)
				);

				var sb = new StringBuilder(50);
				conclusions.SortUnsafe(&cmp);

				var selection = from conclusion in conclusions orderby conclusion.Digit group conclusion by conclusion.ConclusionType;
				var hasOnlyOneType = selection.HasOnlyOneElement();
				foreach (var typeGroup in selection)
				{
					var op = typeGroup.Key.Notation();
					foreach (var digitGroup in from conclusion in typeGroup group conclusion by conclusion.Digit)
					{
						sb.Append(CellConverter([.. from conclusion in digitGroup select conclusion.Cell]));
						sb.Append(op);
						sb.Append(digitGroup.Key + 1);
						sb.Append(DefaultSeparator);
					}

					sb.RemoveFrom(^DefaultSeparator.Length);
					if (!hasOnlyOneType)
					{
						sb.Append(DefaultSeparator);
					}
				}

				if (!hasOnlyOneType)
				{
					sb.RemoveFrom(^DefaultSeparator.Length);
				}

				return sb.ToString();
			}
		};

	/// <inheritdoc/>
	public override DigitNotationConverter DigitConverter
		=> new LiteralCoordinateConverter(DigitsSeparator: DigitsSeparator).DigitConverter;

	/// <inheritdoc/>
	public override IntersectionNotationConverter IntersectionConverter
		=> new K9Converter(MakeLettersUpperCase, DefaultSeparator: DefaultSeparator, DigitsSeparator: DigitsSeparator).IntersectionConverter;

	/// <inheritdoc/>
	public override ChuteNotationConverter ChuteConverter
		=> new K9Converter(MakeLettersUpperCase, DefaultSeparator: DefaultSeparator, DigitsSeparator: DigitsSeparator).ChuteConverter;

	/// <inheritdoc/>
	public override ConjugateNotationConverter ConjugateConverter
		=> (scoped ReadOnlySpan<Conjugate> conjugatePairs) =>
		{
			if (conjugatePairs.Length == 0)
			{
				return string.Empty;
			}

			var sb = new StringBuilder(20);
			foreach (var conjugatePair in conjugatePairs)
			{
				var fromCellString = CellConverter([conjugatePair.From]);
				var toCellString = CellConverter([conjugatePair.To]);
				sb.Append($"{fromCellString} == {toCellString}.{DigitConverter((Mask)(1 << conjugatePair.Digit))}");
				sb.Append(DefaultSeparator);
			}
			return sb.RemoveFrom(^DefaultSeparator.Length).ToString();
		};
}
