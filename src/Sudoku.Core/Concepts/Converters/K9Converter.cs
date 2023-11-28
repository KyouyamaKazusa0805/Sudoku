using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Sudoku.Analytics;
using static System.Numerics.BitOperations;
using static Sudoku.Strings.InternalStringAccessor;

namespace Sudoku.Concepts.Converters;

/// <summary>
/// Represents a coordinate converter using K9 notation.
/// </summary>
/// <param name="MakeLettersUpperCase">
/// <para>Indicates whether we make the letters be upper-casing.</para>
/// <para>The value is <see langword="false"/> by default.</para>
/// </param>
/// <param name="FinalRowLetter">
/// <para>
/// Indicates the character that displays for the last row. Generally it uses <c>'I'</c> to be the last row,
/// but sometimes it may produce much difficulty on distincting with digit 1 and i (They are nearly same by its shape).
/// This option will change the last row letter if you want to change it.
/// </para>
/// <para>The value is <c>'I'</c> by default. You can set the value to <c>'J'</c> or <c>'K'</c>; other letters are not suggested.</para>
/// </param>
/// <param name="DefaultSeparator"><inheritdoc/></param>
/// <param name="DigitsSeparator"><inheritdoc/></param>
public sealed record K9Converter(
	bool MakeLettersUpperCase = false,
	char FinalRowLetter = 'I',
	string DefaultSeparator = ", ",
	string? DigitsSeparator = null
) : CoordinateConverter(DefaultSeparator, DigitsSeparator)
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
					var rowCharacter = row == 8
						? MakeLettersUpperCase ? char.ToUpper(FinalRowLetter) : char.ToLower(FinalRowLetter)
						: (char)((MakeLettersUpperCase ? 'A' : 'a') + row);
					return $"{rowCharacter}{DigitConverter((Mask)(1 << column))}";
				}
				default: { return r(in cells) is var a && c(in cells) is var b && a.Length <= b.Length ? a : b; }
			}


			string r(scoped ref readonly CellMap cells)
			{
				scoped var sbRow = new StringHandler(18);
				var dic = new Dictionary<Cell, List<Digit>>(9);
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
					sbRow.Append(
						row == 8
							? MakeLettersUpperCase ? char.ToUpper(FinalRowLetter) : char.ToLower(FinalRowLetter)
							: (char)((MakeLettersUpperCase ? 'A' : 'a') + row)
					);
					sbRow.AppendRange(dic[row], d => DigitConverter((Mask)(1 << d)));
					sbRow.Append(DefaultSeparator);
				}
				sbRow.RemoveFromEnd(DefaultSeparator.Length);

				return sbRow.ToStringAndClear();
			}

			string c(scoped ref readonly CellMap cells)
			{
				var dic = new Dictionary<Digit, List<Cell>>(9);
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
						sbColumn.Append(
							row == 8
								? MakeLettersUpperCase ? char.ToUpper(FinalRowLetter) : char.ToLower(FinalRowLetter)
								: (char)((MakeLettersUpperCase ? 'A' : 'a') + row)
						);
					}

					sbColumn.Append(column + 1);
					sbColumn.Append(DefaultSeparator);
				}
				sbColumn.RemoveFromEnd(DefaultSeparator.Length);

				return sbColumn.ToStringAndClear();
			}
		};

	/// <inheritdoc/>
	public override CandidateNotationConverter CandidateConverter
		=> (scoped ref readonly CandidateMap candidates) =>
		{
			scoped var sb = new StringHandler(50);
			foreach (var digitGroup in
				from candidate in candidates
				group candidate by candidate % 9 into digitGroups
				orderby digitGroups.Key
				select digitGroups)
			{
				var cells = CellMap.Empty;
				foreach (var candidate in digitGroup)
				{
					cells.Add(candidate / 9);
				}

				sb.Append(CellConverter(in cells));
				sb.Append('.');
				sb.Append(digitGroup.Key + 1);

				sb.Append(DefaultSeparator);
			}

			sb.RemoveFromEnd(DefaultSeparator.Length);
			return sb.ToStringAndClear();
		};

	/// <inheritdoc/>
	public override HouseNotationConverter HouseConverter
		=> housesMask =>
		{
			if (housesMask == 0)
			{
				return string.Empty;
			}

			if (IsPow2((uint)housesMask))
			{
				var house = Log2((uint)housesMask);
				var houseType = house.ToHouseType();
				return string.Format(GetString(houseType switch
				{
					HouseType.Row => "RowLabel",
					HouseType.Column => "ColumnLabel",
					HouseType.Block => "BlockLabel",
					_ => throw new InvalidOperationException($"The specified house value '{nameof(house)}' is invalid.")
				}), house % 9 + 1);
			}

			var dic = new Dictionary<HouseType, List<House>>(3);
			foreach (var house in housesMask)
			{
				var houseType = house.ToHouseType();
				if (!dic.TryAdd(houseType, [house]))
				{
					dic[houseType].Add(house);
				}
			}

			scoped var sb = new StringHandler(30);
			foreach (var (houseType, h) in from kvp in dic orderby kvp.Key.GetProgramOrder() select kvp)
			{
				sb.Append(
					string.Format(
						GetString(
							houseType switch
							{
								HouseType.Row => "RowLabel",
								HouseType.Column => "ColumnLabel",
								HouseType.Block => "BlockLabel",
								_ => throw new InvalidOperationException($"The specified house value '{nameof(houseType)}' is invalid.")
							}
						), string.Concat([.. from house in h select (house % 9 + 1).ToString()])
					)
				);
			}

			return sb.ToStringAndClear();
		};

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
					ref Unsafe.As<Conclusion, byte>(ref conclusions[0]),
					in Unsafe.As<Conclusion, byte>(ref Unsafe.AsRef(in c[0])),
					(uint)(sizeof(Conclusion) * c.Length)
				);

				scoped var sb = new StringHandler(50);
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

					sb.RemoveFromEnd(DefaultSeparator.Length);
					if (!hasOnlyOneType)
					{
						sb.Append(DefaultSeparator);
					}
				}

				if (!hasOnlyOneType)
				{
					sb.RemoveFromEnd(DefaultSeparator.Length);
				}

				return sb.ToStringAndClear();
			}
		};

	/// <inheritdoc/>
	public override DigitNotationConverter DigitConverter
		=> new LiteralCoordinateConverter(DigitsSeparator: DigitsSeparator).DigitConverter;

	/// <inheritdoc/>
	public override IntersectionNotationConverter IntersectionConverter
		=> (scoped ReadOnlySpan<(IntersectionBase Base, IntersectionResult Result)> intersections) => DefaultSeparator switch
		{
			null or [] => string.Concat([
				..
				from intersection in intersections
				let baseSet = intersection.Base.Line
				let coverSet = intersection.Base.Block
				select string.Format(
					GetString("LockedCandidatesLabel"),
					((House)baseSet).ToHouseType() switch
					{
						HouseType.Block => string.Format(GetString("BlockLabel"), (baseSet % 9 + 1).ToString()),
						HouseType.Row => string.Format(GetString("RowLabel"), (baseSet % 9 + 1).ToString()),
						HouseType.Column => string.Format(GetString("ColumnLabel"), (baseSet % 9 + 1).ToString())
					},
					((House)coverSet).ToHouseType() switch
					{
						HouseType.Block => string.Format(GetString("BlockLabel"), (coverSet % 9 + 1).ToString()),
						HouseType.Row => string.Format(GetString("RowLabel"), (coverSet % 9 + 1).ToString()),
						HouseType.Column => string.Format(GetString("ColumnLabel"), (coverSet % 9 + 1).ToString())
					}
				)
			]),
			_ => string.Join(
				DefaultSeparator,
				[
					..
					from intersection in intersections
					let baseSet = intersection.Base.Line
					let coverSet = intersection.Base.Block
					select string.Format(
						GetString("LockedCandidatesLabel"),
						((House)baseSet).ToHouseType() switch
						{
							HouseType.Block => string.Format(GetString("BlockLabel"), (baseSet % 9 + 1).ToString()),
							HouseType.Row => string.Format(GetString("RowLabel"), (baseSet % 9 + 1).ToString()),
							HouseType.Column => string.Format(GetString("ColumnLabel"), (baseSet % 9 + 1).ToString())
						},
						((House)coverSet).ToHouseType() switch
						{
							HouseType.Block => string.Format(GetString("BlockLabel"), (coverSet % 9 + 1).ToString()),
							HouseType.Row => string.Format(GetString("RowLabel"), (coverSet % 9 + 1).ToString()),
							HouseType.Column => string.Format(GetString("ColumnLabel"), (coverSet % 9 + 1).ToString())
						}
					)
				]
			)
		};

	/// <inheritdoc/>
	public override ChuteNotationConverter ChuteConverter
		=> (scoped ReadOnlySpan<Chute> chutes) =>
		{
			var megalines = new Dictionary<bool, byte>(2);
			foreach (var (index, _, isRow, _) in chutes)
			{
				if (!megalines.TryAdd(isRow, (byte)(1 << index % 3)))
				{
					megalines[isRow] |= (byte)(1 << index % 3);
				}
			}

			var sb = new StringHandler(12);
			if (megalines.TryGetValue(true, out var megaRows))
			{
				sb.Append(MakeLettersUpperCase ? "Mega Row" : "mega row");
				foreach (var megaRow in megaRows)
				{
					sb.Append(megaRow + 1);
				}

				sb.Append(DefaultSeparator);
			}
			if (megalines.TryGetValue(false, out var megaColumns))
			{
				sb.Append(MakeLettersUpperCase ? "Mega Column" : "mega column");
				foreach (var megaColumn in megaColumns)
				{
					sb.Append(megaColumn + 1);
				}
			}

			return sb.ToStringAndClear();
		};

	/// <inheritdoc/>
	public override ConjugateNotationConverter ConjugateConverter
		=> (scoped ReadOnlySpan<Conjugate> conjugatePairs) =>
		{
			if (conjugatePairs.Length == 0)
			{
				return string.Empty;
			}

			var sb = new StringHandler(20);
			foreach (var conjugatePair in conjugatePairs)
			{
				var fromCellString = CellConverter([conjugatePair.From]);
				var toCellString = CellConverter([conjugatePair.To]);
				sb.Append($"{fromCellString} == {toCellString}.{DigitConverter((Mask)(1 << conjugatePair.Digit))}");
				sb.Append(DefaultSeparator);
			}
			sb.RemoveFromEnd(DefaultSeparator.Length);

			return sb.ToStringAndClear();
		};
}
