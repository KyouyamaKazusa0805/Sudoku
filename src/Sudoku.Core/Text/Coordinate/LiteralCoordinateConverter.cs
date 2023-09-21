using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Sudoku.Analytics;
using Sudoku.Concepts;
using static System.Numerics.BitOperations;
using static Sudoku.Strings.InternalStringAccessor;

namespace Sudoku.Text.Coordinate;

/// <summary>
/// Represents a converter that outputs coordinates as literally-speaking representation.
/// </summary>
/// <param name="DefaultSeparator">
/// <para>Indicates the default separator. The value will be inserted into two non-digit-kind instances.</para>
/// <para>The value is <c>", "</c> by default.</para>
/// </param>
/// <param name="DigitsSeparator">
/// <para>Indicates the digits separator.</para>
/// <para>The value is <see langword="null"/> by default, meaning no separators will be inserted between 2 digits.</para>
/// </param>
public sealed record LiteralCoordinateConverter(string DefaultSeparator = ", ", string? DigitsSeparator = null) : CoordinateConverter
{
	/// <inheritdoc/>
	public override CellNotationConverter CellNotationConverter
		=> (scoped ref readonly CellMap cells) => cells switch
		{
			[] => string.Empty,
			[var p] => string.Format(GetString("CellLabel"), (p / 9 + 1).ToString(), (p % 9 + 1).ToString()),
			_ => string.Format(
				GetString("CellsLabel"),
				string.Join(DefaultSeparator, [.. from cell in cells select string.Format(GetString("CellLabel"), cell / 9 + 1, cell % 9 + 1)])
			)
		};

	/// <inheritdoc/>
	public override CandidateNotationConverter CandidateNotationConverter
		=> (scoped ref readonly CandidateMap candidates) =>
		{
			var snippets = new List<string>();
			foreach (var candidate in candidates)
			{
				var cellString = CellNotationConverter([candidate / 9]);
				var digitString = DigitNotationConverter((Mask)(1 << candidate % 9));
				snippets.Add(string.Format(GetString("CandidateLabel"), cellString, digitString));
			}

			return string.Join(DefaultSeparator, snippets);
		};

	/// <inheritdoc/>
	public override HouseNotationConverter HouseNotationConverter
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

			var snippets = new List<string>(PopCount((uint)housesMask));
			foreach (var house in housesMask)
			{
				var houseType = house.ToHouseType();
				snippets.Add(string.Format(GetString(houseType switch
				{
					HouseType.Row => "RowLabel",
					HouseType.Column => "ColumnLabel",
					HouseType.Block => "BlockLabel",
					_ => throw new InvalidOperationException($"The specified house value '{nameof(house)}' is invalid.")
				}), house % 9 + 1));
			}

			return string.Format(GetString("HousesLabel"), string.Join(DefaultSeparator, snippets));
		};

	/// <inheritdoc/>
	public override ConclusionNotationConverter ConclusionNotationConverter
		=> (scoped ReadOnlySpan<Conclusion> conclusions) =>
		{
			return conclusions switch
			{
				[] => string.Empty,
				[(var t, var c, var d)] => $"{CellNotationConverter([c])}{t.Notation()}{DigitNotationConverter((Mask)(1 << d))}",
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
				conclusions.Sort(&cmp);

				var selection = from conclusion in conclusions orderby conclusion.Digit group conclusion by conclusion.ConclusionType;
				var hasOnlyOneType = selection.HasOnlyOneElement();
				foreach (var typeGroup in selection)
				{
					var op = typeGroup.Key.Notation();
					foreach (var digitGroup in from conclusion in typeGroup group conclusion by conclusion.Digit)
					{
						sb.Append(CellMap.Empty + from conclusion in digitGroup select conclusion.Cell);
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
	public override DigitNotationConverter DigitNotationConverter
		=> mask => DigitsSeparator switch
		{
			null or [] => string.Concat([.. from digit in mask select (digit + 1).ToString()]),
			_ => string.Join(DigitsSeparator, [.. from digit in mask select (digit + 1).ToString()])
		};

	/// <inheritdoc/>
	public override IntersectionNotationConverter IntersectionNotationConverter
		=> (scoped ReadOnlySpan<(IntersectionBase Base, IntersectionResult Result)> intersections) =>
		{
			return string.Join(
				DefaultSeparator,
				[
					..
					from intersection in intersections
					let baseSet = intersection.Base.Line
					let coverSet = intersection.Base.Block
					select string.Format(GetString("LockedCandidatesLabel"), labelKey(baseSet), labelKey(coverSet))
				]
			);


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static string labelKey(byte house)
				=> ((House)house).ToHouseType() switch
				{
					HouseType.Block => string.Format(GetString("BlockLabel"), house % 9 + 1),
					HouseType.Row => string.Format(GetString("RowLabel"), house % 9 + 1),
					HouseType.Column => string.Format(GetString("ColumnLabel", house % 9 + 1)),
					_ => throw new ArgumentOutOfRangeException(nameof(house))
				};
		};

	/// <inheritdoc/>
	public override ChuteNotationConverter ChuteNotationConverter
		=> (scoped ReadOnlySpan<Chute> chutes) =>
		{
			var snippets = new List<string>(6);
			foreach (var (index, _, isRow, _) in chutes)
			{
				snippets.Add(string.Format(GetString("MegaRowLabel"), index % 3 + 1));
			}

			return string.Format(GetString("MegaLinesLabel"), string.Join(DefaultSeparator, snippets));
		};

	/// <inheritdoc/>
	public override ConjugateNotationConverter ConjugateNotationConverter
		=> (scoped ReadOnlySpan<Conjugate> conjugatePairs) =>
		{
			if (conjugatePairs.Length == 0)
			{
				return string.Empty;
			}

			var snippets = new List<string>();
			foreach (var conjugatePair in conjugatePairs)
			{
				var fromCellString = CellNotationConverter([conjugatePair.From]);
				var toCellString = CellNotationConverter([conjugatePair.To]);
				var digitString = DigitNotationConverter((Mask)(1 << conjugatePair.Digit));
				snippets.Add(string.Format(GetString("ConjugatePairWith"), fromCellString, toCellString, digitString));
			}

			return string.Join(DefaultSeparator, snippets);
		};
}
