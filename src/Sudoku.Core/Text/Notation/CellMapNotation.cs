using System.SourceGeneration;

namespace Sudoku.Text.Notation;

/// <summary>
/// Represents a notation that represents for a <see cref="CellMap"/> instance.
/// </summary>
/// <seealso cref="CellMap"/>
public sealed partial class CellMapNotation : INotation<CellMapNotation, CellMap, CellMapNotation.Kind>
{
	/// <inheritdoc cref="CellNotation.Parse(string)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap Parse(string text) => Parse(text, Kind.RxCy);

	/// <inheritdoc/>
	/// <exception cref="NotSupportedException">Throws when the argument <paramref name="notation"/> is not supported.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap Parse(string text, Kind notation)
		=> notation switch
		{
			Kind.RxCy => CellNotation.ParseCollection(text, CellNotation.Kind.RxCy),
			Kind.K9 => CellNotation.ParseCollection(text, CellNotation.Kind.K9),
			Kind.Binary => throw new NotSupportedException("The binary format is not supported for parsing."),
			Kind.Table => throw new NotSupportedException("The table format is not supported for parsing."),
			_ => throw new ArgumentOutOfRangeException(nameof(notation))
		};

	/// <inheritdoc cref="CellNotation.ToCollectionString(in CellMap)"/>
	/// <param name="value"><inheritdoc cref="CellNotation.ToCollectionString(in CellMap)" path="/param[@name='collection']"/></param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToString(scoped in CellMap value) => CellNotation.ToCollectionString(value);

	/// <inheritdoc cref="INotation{TSelf, TElement, TConceptKindPresenter}.ToString(TElement, TConceptKindPresenter)"/>
	[ExplicitInterfaceImpl(typeof(INotation<,,>))]
	public static string ToString(scoped in CellMap value, Kind notation)
	{
		switch (notation)
		{
			case Kind.RxCy:
			{
				return CellNotation.ToCollectionString(value, CellNotation.Kind.RxCy);
			}
			case Kind.K9:
			{
				return CellNotation.ToCollectionString(value, CellNotation.Kind.K9);
			}
			case Kind.Binary:
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
			case Kind.Table:
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
}
