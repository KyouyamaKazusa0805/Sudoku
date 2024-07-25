namespace Sudoku.Runtime.CoordinateServices;

/// <summary>
/// Represents a coordinate converter using RxCy notation.
/// </summary>
/// <param name="MakeLettersUpperCase">
/// <para>Indicates whether we make the letters <c>'r'</c>, <c>'c'</c> and <c>'b'</c> to be upper-casing.</para>
/// <para>The value is <see langword="false"/> by default.</para>
/// </param>
/// <param name="MakeDigitBeforeCell">
/// <para>Indicates whether digits will be displayed before the cell coordinates.</para>
/// <para>The value is <see langword="false"/> by default.</para>
/// </param>
/// <param name="HouseNotationOnlyDisplayCapitals">
/// <para>Indicates whether the houses will be displayed its capitals only.</para>
/// <para>The value is <see langword="false"/> by default.</para>
/// </param>
/// <param name="DefaultSeparator"><inheritdoc/></param>
/// <param name="DigitsSeparator"><inheritdoc/></param>
/// <param name="CurrentCulture"><inheritdoc/></param>
public sealed record RxCyConverter(
	bool MakeLettersUpperCase = false,
	bool MakeDigitBeforeCell = false,
	bool HouseNotationOnlyDisplayCapitals = false,
	string DefaultSeparator = ", ",
	string? DigitsSeparator = null,
	CultureInfo? CurrentCulture = null
) : CoordinateConverter(DefaultSeparator, DigitsSeparator, CurrentCulture)
{
	/// <inheritdoc/>
	public override FuncRefReadOnly<CellMap, string> CellConverter
		=> (ref readonly CellMap cells) =>
		{
			return cells switch
			{
				[] => string.Empty,
				[var p] => MakeLettersUpperCase switch { true => $"R{p / 9 + 1}C{p % 9 + 1}", _ => $"r{p / 9 + 1}c{p % 9 + 1}" },
				_ => r(in cells) is var a && c(in cells) is var b && a.Length <= b.Length ? a : b
			};


			string r(ref readonly CellMap cells)
			{
				var sbRow = new StringBuilder(50);
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
					sbRow.Append(MakeLettersUpperCase ? 'R' : 'r');
					sbRow.Append(row + 1);
					sbRow.Append(MakeLettersUpperCase ? 'C' : 'c');
					sbRow.AppendRange(d => DigitConverter((Mask)(1 << d)), elements: dic[row].AsReadOnlySpan());
					sbRow.Append(DefaultSeparator);
				}
				return sbRow.RemoveFrom(^DefaultSeparator.Length).ToString();
			}

			string c(ref readonly CellMap cells)
			{
				var dic = new Dictionary<Digit, List<RowIndex>>(9);
				var sbColumn = new StringBuilder(50);
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
					sbColumn.Append(MakeLettersUpperCase ? 'R' : 'r');
					sbColumn.AppendRange(d => DigitConverter((Mask)(1 << d)), elements: dic[column].AsReadOnlySpan());
					sbColumn.Append(MakeLettersUpperCase ? 'C' : 'c');
					sbColumn.Append(column + 1);
					sbColumn.Append(DefaultSeparator);
				}
				return sbColumn.RemoveFrom(^DefaultSeparator.Length).ToString();
			}
		};

	/// <inheritdoc/>
	public override FuncRefReadOnly<CandidateMap, string> CandidateConverter
		=> (ref readonly CandidateMap candidates) =>
		{
			if (!candidates)
			{
				return string.Empty;
			}

			var sb = new StringBuilder(50);
			foreach (var digitGroup in
				from candidate in candidates
				group candidate by candidate % 9 into digitGroups
				orderby digitGroups.Key
				select digitGroups)
			{
				var cells = (CellMap)([.. from candidate in digitGroup select candidate / 9]);
				if (MakeDigitBeforeCell)
				{
					sb.Append(digitGroup.Key + 1);
					sb.Append(CellConverter(in cells));
				}
				else
				{
					sb.Append(CellConverter(in cells));
					sb.Append('(');
					sb.Append(digitGroup.Key + 1);
					sb.Append(')');
				}

				sb.Append(DefaultSeparator);
			}
			return sb.RemoveFrom(^DefaultSeparator.Length).ToString();
		};

	/// <inheritdoc/>
	public override Func<HouseMask, string> HouseConverter
		=> housesMask =>
		{
			if (housesMask == 0)
			{
				return string.Empty;
			}

			if (HouseNotationOnlyDisplayCapitals)
			{
				var sb = new StringBuilder(27);
				for (var (houseIndex, i) = (9, 0); i < 27; i++, houseIndex = (houseIndex + 1) % 27)
				{
					if ((housesMask >> houseIndex & 1) != 0)
					{
						sb.Append(getChar(houseIndex / 9));
					}
				}
				return sb.ToString();
			}

			if (IsPow2((uint)housesMask))
			{
				var house = Log2((uint)housesMask);
				return $"{getChar(house)}{house % 9 + 1}";
			}

			{
				var dic = new Dictionary<HouseType, List<House>>(3);
				foreach (var house in housesMask)
				{
					var houseType = house.ToHouseType();
					if (!dic.TryAdd(houseType, [house]))
					{
						dic[houseType].Add(house);
					}
				}

				var sb = new StringBuilder(30);
				foreach (var (houseType, h) in from kvp in dic orderby kvp.Key.GetProgramOrder() select kvp)
				{
					sb.Append(houseType.GetLabel());
					sb.AppendRange(static integer => integer.ToString(), elements: from house in h select house % 9 + 1);
				}

				return sb.ToString();
			}


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static char getChar(House house)
				=> house switch
				{
					>= 0 and < 9 => 'b',
					>= 9 and < 18 => 'r',
					>= 18 and < 27 => 'c',
					_ => throw new ArgumentOutOfRangeException(nameof(house))
				};
		};

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<Conclusion>, string> ConclusionConverter
		=> conclusions =>
		{
			return conclusions switch
			{
				[] => string.Empty,
				[(var t, var c, var d)] => $"{CellConverter(in c.AsCellMap())}{t.GetNotation()}{DigitConverter((Mask)(1 << d))}",
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
		=> new LiteralCoordinateConverter(DefaultSeparator: DefaultSeparator).DigitConverter;

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<Miniline>, string> IntersectionConverter
		=> intersections => DefaultSeparator switch
		{
			null or [] => string.Concat(
				from intersection in intersections
				let baseSet = intersection.Base.Line
				let coverSet = intersection.Base.Block
				select $"{GetLabel((byte)(baseSet / 9))}{baseSet % 9 + 1}{GetLabel((byte)(coverSet / 9))}{coverSet % 9 + 1}"
			),
			_ => string.Join(
				DefaultSeparator,
				from intersection in intersections
				let baseSet = intersection.Base.Line
				let coverSet = intersection.Base.Block
				select $"{GetLabel((byte)(baseSet / 9))}{baseSet % 9 + 1}{GetLabel((byte)(coverSet / 9))}{coverSet % 9 + 1}"
			)
		};

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<Chute>, string> ChuteConverter
		=> chutes =>
		{
			var megalines = new Dictionary<bool, byte>(2);
			foreach (var (index, _, isRow, _) in chutes)
			{
				if (!megalines.TryAdd(isRow, (byte)(1 << index % 3)))
				{
					megalines[isRow] |= (byte)(1 << index % 3);
				}
			}

			var sb = new StringBuilder(12);
			if (megalines.TryGetValue(true, out var megaRows))
			{
				sb.Append(MakeLettersUpperCase ? "MR" : "mr");
				foreach (var megaRow in megaRows)
				{
					sb.Append(megaRow + 1);
				}

				sb.Append(DefaultSeparator);
			}
			if (megalines.TryGetValue(false, out var megaColumns))
			{
				sb.Append(MakeLettersUpperCase ? "MC" : "mc");
				foreach (var megaColumn in megaColumns)
				{
					sb.Append(megaColumn + 1);
				}
			}

			return sb.ToString();
		};

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
				var fromCellString = CellConverter(in conjugatePair.From.AsCellMap());
				var toCellString = CellConverter(in conjugatePair.To.AsCellMap());
				sb.Append(
					MakeDigitBeforeCell
						? $"{DigitConverter((Mask)(1 << conjugatePair.Digit))}{fromCellString} == {toCellString}"
						: $"{fromCellString} == {toCellString}({DigitConverter((Mask)(1 << conjugatePair.Digit))})"
				);
				sb.Append(DefaultSeparator);
			}
			return sb.RemoveFrom(^DefaultSeparator.Length).ToString();
		};


	/// <inheritdoc/>
	[return: NotNullIfNotNull(nameof(formatType))]
	public override object? GetFormat(Type? formatType) => formatType == typeof(CoordinateConverter) ? this : null;

	/// <summary>
	/// Get the label of each house.
	/// </summary>
	/// <param name="houseIndex">The house index.</param>
	/// <returns>The label.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private char GetLabel(byte houseIndex)
		=> (houseIndex, MakeLettersUpperCase) switch
		{
			(0, true) => 'B',
			(0, _) => 'b',
			(1, true) => 'R',
			(1, _) => 'r',
			(2, true) => 'C',
			_ => 'c'
		};
}
