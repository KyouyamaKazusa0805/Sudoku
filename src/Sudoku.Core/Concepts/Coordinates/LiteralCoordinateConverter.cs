namespace Sudoku.Concepts.Coordinates;

/// <summary>
/// Represents a converter that outputs coordinates as literally-speaking representation.
/// </summary>
/// <param name="DefaultSeparator"><inheritdoc/></param>
/// <param name="DigitsSeparator"><inheritdoc/></param>
/// <param name="CurrentCulture"><inheritdoc/></param>
public sealed record LiteralCoordinateConverter(string DefaultSeparator = ", ", string? DigitsSeparator = null, CultureInfo? CurrentCulture = null) :
	CoordinateConverter(DefaultSeparator, DigitsSeparator, CurrentCulture)
{
	/// <inheritdoc/>
	public override CellNotationConverter CellConverter
		=> cells => cells switch
		{
			[] => string.Empty,
			[var p] => string.Format(
				ResourceDictionary.Get("CellLabel", TargetCurrentCulture),
#if NET9_0_OR_GREATER
				[
#endif
				(p / 9 + 1).ToString(),
				(p % 9 + 1).ToString()
#if NET9_0_OR_GREATER
				]
#endif
			),
			_ => string.Format(
				ResourceDictionary.Get("CellsLabel", TargetCurrentCulture),
				string.Join(
					DefaultSeparator,
#if !NET9_0_OR_GREATER
					[
					..
#endif
					from cell in cells
					select string.Format(
						ResourceDictionary.Get("CellLabel", TargetCurrentCulture),
#if NET9_0_OR_GREATER
						[
#endif
						cell / 9 + 1,
						cell % 9 + 1
#if NET9_0_OR_GREATER
						]
#endif
					)
#if !NET9_0_OR_GREATER
					]
#endif
				)
			)
		};

	/// <inheritdoc/>
	public override CandidateNotationConverter CandidateConverter
		=> candidates =>
		{
			var snippets = new List<string>();
			foreach (var candidate in candidates)
			{
				var cellString = CellConverter(candidate / 9);
				var digitString = DigitConverter((Mask)(1 << candidate % 9));
				snippets.Add(
					string.Format(
						ResourceDictionary.Get("CandidateLabel", TargetCurrentCulture),
#if NET9_0_OR_GREATER
						[
#endif
						cellString,
						digitString
#if NET9_0_OR_GREATER
						]
#endif
					)
				);
			}

			return string.Join(DefaultSeparator, snippets);
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
				return string.Format(
					ResourceDictionary.Get(
						houseType switch
						{
							HouseType.Row => "RowLabel",
							HouseType.Column => "ColumnLabel",
							HouseType.Block => "BlockLabel",
							_ => throw new InvalidOperationException($"The specified house value '{nameof(house)}' is invalid.")
						},
						TargetCurrentCulture
					),
					house % 9 + 1
				);
			}

			var snippets = new List<string>(PopCount((uint)housesMask));
			foreach (var house in housesMask)
			{
				var houseType = house.ToHouseType();
				snippets.Add(
					string.Format(
						ResourceDictionary.Get(
							houseType switch
							{
								HouseType.Row => "RowLabel",
								HouseType.Column => "ColumnLabel",
								HouseType.Block => "BlockLabel",
								_ => throw new InvalidOperationException($"The specified house value '{nameof(house)}' is invalid.")
							},
							TargetCurrentCulture
						),
						house % 9 + 1
					)
				);
			}

			return string.Format(ResourceDictionary.Get("HousesLabel", TargetCurrentCulture), string.Join(DefaultSeparator, snippets));
		};

	/// <inheritdoc/>
	public override ConclusionNotationConverter ConclusionConverter
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

				Array.Sort(conclusions, static (left, right) => left.CompareTo(right));
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
	public override DigitNotationConverter DigitConverter
		=> mask => DigitsSeparator switch
		{
			null or [] => string.Concat(
#if !NET9_0_OR_GREATER
				[
				..
#endif
				from digit in mask select (digit + 1).ToString()
#if !NET9_0_OR_GREATER
				]
#endif
			),
			_ => string.Join(
				DigitsSeparator,
#if !NET9_0_OR_GREATER
				[
				..
#endif
				from digit in mask select (digit + 1).ToString()
#if !NET9_0_OR_GREATER
				]
#endif
			)
		};

	/// <inheritdoc/>
	public override IntersectionNotationConverter IntersectionConverter
		=> intersections =>
		{
			return string.Join(
				DefaultSeparator,
#if !NET9_0_OR_GREATER
				[
				..
#endif
				from intersection in intersections
				let baseSet = intersection.Base.Line
				let coverSet = intersection.Base.Block
				select string.Format(
					ResourceDictionary.Get("LockedCandidatesLabel", TargetCurrentCulture),
#if NET9_0_OR_GREATER
					[
#endif
					labelKey(baseSet),
					labelKey(coverSet)
#if NET9_0_OR_GREATER
					]
#endif
				)
#if !NET9_0_OR_GREATER
				]
#endif
			);


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			string labelKey(byte house)
				=> ((House)house).ToHouseType() switch
				{
					HouseType.Block => string.Format(ResourceDictionary.Get("BlockLabel", TargetCurrentCulture), house % 9 + 1),
					HouseType.Row => string.Format(ResourceDictionary.Get("RowLabel", TargetCurrentCulture), house % 9 + 1),
					HouseType.Column => string.Format(ResourceDictionary.Get("ColumnLabel", TargetCurrentCulture), house % 9 + 1),
					_ => throw new ArgumentOutOfRangeException(nameof(house))
				};
		};

	/// <inheritdoc/>
	public override ChuteNotationConverter ChuteConverter
		=> chutes =>
		{
			var snippets = new List<string>(6);
			foreach (var (index, _, isRow, _) in chutes)
			{
				snippets.Add(string.Format(ResourceDictionary.Get("MegaRowLabel", TargetCurrentCulture), index % 3 + 1));
			}

			return string.Format(ResourceDictionary.Get("MegaLinesLabel", TargetCurrentCulture), string.Join(DefaultSeparator, snippets));
		};

	/// <inheritdoc/>
	public override ConjugateNotationConverter ConjugateConverter
		=> conjugatePairs =>
		{
			if (conjugatePairs.Length == 0)
			{
				return string.Empty;
			}

			var snippets = new List<string>();
			foreach (var conjugatePair in conjugatePairs)
			{
				var fromCellString = CellConverter(conjugatePair.From);
				var toCellString = CellConverter(conjugatePair.To);
				var digitString = DigitConverter((Mask)(1 << conjugatePair.Digit));
				snippets.Add(
					string.Format(
						ResourceDictionary.Get("ConjugatePairWith", TargetCurrentCulture),
#if NET9_0_OR_GREATER
						[
#endif
						fromCellString,
						toCellString,
						digitString
#if NET9_0_OR_GREATER
						]
#endif
					)
				);
			}

			return string.Join(DefaultSeparator, snippets);
		};


	/// <inheritdoc/>
	[return: NotNullIfNotNull(nameof(formatType))]
	public override object? GetFormat(Type? formatType) => formatType == typeof(CoordinateConverter) ? this : null;
}
