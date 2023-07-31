namespace Sudoku.Text.Notation;

/// <summary>
/// Represents a notation that represents for a <see cref="CellMap"/> instance.
/// </summary>
/// <seealso cref="CellMap"/>
public sealed class CellMapNotation : INotation<CellMapNotation, CellMap, CellMapNotationKind>
{
	/// <inheritdoc cref="CellNotation.Parse(string)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap Parse(string text) => Parse(text, CellMapNotationKind.RxCy);

	/// <inheritdoc/>
	/// <exception cref="NotSupportedException">Throws when the argument <paramref name="notation"/> is not supported.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap Parse(string text, CellMapNotationKind notation)
		=> notation switch
		{
			CellMapNotationKind.RxCy => CellNotation.ParseCollection(text, CellNotationKind.RxCy),
			CellMapNotationKind.K9 => CellNotation.ParseCollection(text, CellNotationKind.K9),
			CellMapNotationKind.Binary => throw new NotSupportedException("The binary format is not supported for parsing."),
			CellMapNotationKind.Table => throw new NotSupportedException("The table format is not supported for parsing."),
			_ => throw new ArgumentOutOfRangeException(nameof(notation))
		};

	/// <inheritdoc cref="CellNotation.ToCollectionString(in CellMap)"/>
	/// <param name="value"><inheritdoc cref="CellNotation.ToCollectionString(in CellMap)" path="/param[@name='collection']"/></param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToString(scoped in CellMap value) => CellNotation.ToCollectionString(value);

	/// <inheritdoc cref="INotation{TSelf, TElement, TConceptKindPresenter}.ToString(TElement, TConceptKindPresenter)"/>
	public static string ToString(scoped in CellMap value, CellMapNotationKind notation)
	{
		switch (notation)
		{
			case CellMapNotationKind.RxCy:
			{
				return CellNotation.ToCollectionString(value, CellNotationKind.RxCy);
			}
			case CellMapNotationKind.K9:
			{
				return CellNotation.ToCollectionString(value, CellNotationKind.K9);
			}
			case CellMapNotationKind.Binary:
			{
				var newLine = Environment.NewLine;
				scoped var sb = new StringHandler((3 * 7 + newLine.Length) * 13 - newLine.Length);
				for (var i = 0; i < 3; i++)
				{
					for (var bandLn = 0; bandLn < 3; bandLn++)
					{
						for (var j = 0; j < 3; j++)
						{
							for (var columnLn = 0; columnLn < 3; columnLn++)
							{
								sb.Append(value.Contains((i * 3 + bandLn) * 9 + j * 3 + columnLn) ? '*' : '.');
								sb.Append(' ');
							}

							if (j != 2)
							{
								sb.Append("| ");
							}
							else
							{
								sb.AppendLine();
							}
						}
					}

					if (i != 2)
					{
						sb.Append("------+-------+------");
						sb.AppendLine();
					}
				}

				sb.RemoveFromEnd(newLine.Length);
				return sb.ToStringAndClear();
			}
			case CellMapNotationKind.Table:
			{
				scoped var sb = new StringHandler(81);
				var (low, high) = (value._low, value._high);

				var v = low;
				Cell i;
				for (i = 0; i < 27; i++, v >>= 1)
				{
					sb.Append(v & 1);
				}
				sb.Append(' ');
				for (; i < 41; i++, v >>= 1)
				{
					sb.Append(v & 1);
				}
				for (v = high; i < 54; i++, v >>= 1)
				{
					sb.Append(v & 1);
				}
				sb.Append(' ');
				for (; i < 81; i++, v >>= 1)
				{
					sb.Append(v & 1);
				}

				sb.Reverse();
				return sb.ToStringAndClear();
			}
			default:
			{
				throw new ArgumentOutOfRangeException(nameof(notation));
			}
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static string INotation<CellMapNotation, CellMap, CellMapNotationKind>.ToString(CellMap value, CellMapNotationKind notation)
		=> ToString(value, notation);
}
