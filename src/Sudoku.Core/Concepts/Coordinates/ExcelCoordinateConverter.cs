namespace Sudoku.Concepts.Coordinates;

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
		=> cells =>
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


			string r(ref readonly CellMap cells)
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
					sbRow.AppendRange(
						column => ((MakeLettersUpperCase ? 'A' : 'a') + column).ToString(),
						elements: dic[row].AsReadOnlySpan()
					);
					sbRow.Append(row + 1);
					sbRow.Append(DefaultSeparator);
				}
				return sbRow.RemoveFrom(^DefaultSeparator.Length).ToString();
			}

			string c(ref readonly CellMap cells)
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
					sbColumn.AppendRange(static row => (row + 1).ToString(), elements: dic[column].AsReadOnlySpan());
					sbColumn.Append(DefaultSeparator);
				}
				return sbColumn.RemoveFrom(^DefaultSeparator.Length).ToString();
			}
		};

	/// <inheritdoc/>
	public override CandidateNotationConverter CandidateConverter
		=> candidates =>
		{
			var sb = new StringBuilder(50);
			foreach (var digitGroup in
				from candidate in candidates
				group candidate by candidate % 9 into digitGroups
				orderby digitGroups.Key
				select digitGroups)
			{
				sb.Append(CellConverter([.. from candidate in digitGroup select candidate / 9]));
				sb.Append('.');
				sb.Append(digitGroup.Key + 1);
				sb.Append(DefaultSeparator);
			}
			return sb.RemoveFrom(^DefaultSeparator.Length).ToString();
		};

	/// <inheritdoc/>
	public override Func<HouseMask, string> HouseConverter
		=> new K9Converter(MakeLettersUpperCase, DefaultSeparator: DefaultSeparator, DigitsSeparator: DigitsSeparator).HouseConverter;

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<Conclusion>, string> ConclusionConverter
		=> conclusions =>
		{
			return conclusions switch
			{
			[] => string.Empty,
			[(var t, var c, var d)] => $"{CellConverter(c)}{t.GetNotation()}{DigitConverter((Mask)(1 << d))}",
				_ => toString(conclusions)
			};


			unsafe string toString(ReadOnlySpan<Conclusion> c)
			{
				var conclusions = new Conclusion[c.Length];
				Unsafe.CopyBlock(
					ref @ref.ByteRef(ref conclusions[0]),
					in @ref.ReadOnlyByteRef(in c[0]),
					(uint)(sizeof(Conclusion) * c.Length)
				);

				var sb = new StringBuilder(50);

				Array.Sort(conclusions);
				var selection = from conclusion in conclusions orderby conclusion.Digit group conclusion by conclusion.ConclusionType;
				var hasOnlyOneType = selection.Length == 1;
				foreach (var typeGroup in selection)
				{
					var op = typeGroup.Key.GetNotation();
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
	public override Func<Mask, string> DigitConverter
		=> new LiteralCoordinateConverter(DigitsSeparator: DigitsSeparator).DigitConverter;

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<Miniline>, string> IntersectionConverter
		=> new K9Converter(MakeLettersUpperCase, DefaultSeparator: DefaultSeparator, DigitsSeparator: DigitsSeparator).IntersectionConverter;

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<Chute>, string> ChuteConverter
		=> new K9Converter(MakeLettersUpperCase, DefaultSeparator: DefaultSeparator, DigitsSeparator: DigitsSeparator).ChuteConverter;

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<Conjugate>, string> ConjugateConverter
		=> conjugatePairs =>
		{
			if (conjugatePairs.Length == 0)
			{
				return string.Empty;
			}

			var sb = new StringBuilder(20);
			foreach (var conjugatePair in conjugatePairs)
			{
				var fromCellString = CellConverter(conjugatePair.From);
				var toCellString = CellConverter(conjugatePair.To);
				sb.Append($"{fromCellString} == {toCellString}.{DigitConverter((Mask)(1 << conjugatePair.Digit))}");
				sb.Append(DefaultSeparator);
			}
			return sb.RemoveFrom(^DefaultSeparator.Length).ToString();
		};


	/// <inheritdoc/>
	[return: NotNullIfNotNull(nameof(formatType))]
	public override object? GetFormat(Type? formatType) => formatType == typeof(CoordinateConverter) ? this : null;
}
