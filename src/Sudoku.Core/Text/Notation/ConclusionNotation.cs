using Sudoku.Analytics;

namespace Sudoku.Text.Notation;

/// <summary>
/// Represents a notation that formats for a <see cref="Conclusion"/> or <see cref="Conclusion"/> list.
/// </summary>
/// <seealso cref="Conclusion"/>
public sealed partial class ConclusionNotation : INotation<ConclusionNotation, Conclusion[], Conclusion, ConclusionNotation.Kind>
{
	/// <inheritdoc cref="INotation{TSelf, TElement, TConceptKindPresenter}.Parse(string, TConceptKindPresenter)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Conclusion Parse(string text) => Parse(text, Kind.RxCy);

	/// <inheritdoc cref="INotation{TSelf, TCollection, TElement, TConceptKindPresenter}.ParseCollection(string, TConceptKindPresenter)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Conclusion[] ParseCollection(string text) => ParseCollection(text, Kind.RxCy);

	/// <inheritdoc cref="INotation{TSelf, TElement, TConceptKindPresenter}.ToString(TElement, TConceptKindPresenter)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToString(Conclusion value) => ToString(value, Kind.RxCy);

	/// <inheritdoc cref="INotation{TSelf, TCollection, TElement, TConceptKindPresenter}.ToCollectionString(TCollection, TConceptKindPresenter)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToCollectionString(Conclusion[] collection) => ToCollectionString(collection, Kind.RxCy);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Conclusion Parse(string text, Kind notation)
		=> ParseCollection(text, notation) is [var r] ? r : throw new InvalidOperationException("The multiple valid values parsed.");

	/// <inheritdoc/>
	public static Conclusion[] ParseCollection(string text, Kind notation)
	{
		switch (notation)
		{
			case Kind.RxCy
			when NotationPatterns.ConclusionPattern_RxCy().Match(text) is
			{
				Success: true,
				Captures: [{ Value: var r }, { Value: var c }, { Value: var k }, { Value: var d }]
			}:
			{
				scoped var rows = from row in r select row - '1';
				scoped var columns = from column in c select column - '1';
				scoped var digits = from digit in d select digit - '1';
				var resultLength = rows.Length * columns.Length * digits.Length;
				switch (k switch { "=" => Assignment, "<>" or "!=" => Elimination })
				{
					case Assignment when resultLength == 1:
					{
						return [new(Assignment, rows[0] * 9 + columns[0], digits[0])];
					}
					case Assignment:
					{
						throw new InvalidOperationException("Assignment conclusions must set only one digit into only one cell.");
					}
					case Elimination:
					{
						var conclusionList = new List<Conclusion>(resultLength);
						foreach (var row in rows)
						{
							foreach (var column in columns)
							{
								foreach (var digit in digits)
								{
									conclusionList.Add(new(Elimination, row * 9 + column, digit));
								}
							}
						}

						return [.. conclusionList];
					}
					default:
					{
						throw new InvalidOperationException("The target conclusion kind is invalid.");
					}
				}
			}
			case Kind.RxCy:
			{
				goto ThrowInvalidOperationException;
			}
			case Kind.K9
			when NotationPatterns.ConclusionPattern_K9().Match(text) is
			{
				Success: true,
				Captures: [{ Value: var r }, { Value: var c }, { Value: var k }, { Value: var d }]
			}:
			{
				scoped var rows = from row in r select row is 'K' or 'k' ? 8 : char.ToUpper(row) - 'A';
				scoped var columns = from column in c select column - '1';
				scoped var digits = from digit in d select digit - '1';
				var resultLength = rows.Length * columns.Length * digits.Length;
				switch (k switch { "=" => Assignment, "<>" or "!=" => Elimination })
				{
					case Assignment when resultLength == 1:
					{
						return [new(Assignment, rows[0] * 9 + columns[0], digits[0])];
					}
					case Assignment:
					{
						throw new InvalidOperationException("Assignment conclusions must set only one digit into only one cell.");
					}
					case Elimination:
					{
						var conclusionList = new List<Conclusion>(resultLength);
						foreach (var row in rows)
						{
							foreach (var column in columns)
							{
								foreach (var digit in digits)
								{
									conclusionList.Add(new(Elimination, row * 9 + column, digit));
								}
							}
						}

						return [.. conclusionList];
					}
					default:
					{
						throw new InvalidOperationException("The target conclusion kind is invalid.");
					}
				}
			}
			case Kind.K9:
			{
				goto ThrowInvalidOperationException;
			}
			default:
			{
				throw new ArgumentOutOfRangeException(nameof(notation));
			}
		}

	ThrowInvalidOperationException:
		throw new InvalidOperationException("The text cannot be parsed into a valid conclusion instance.");
	}

	/// <inheritdoc/>
	public static string ToString(Conclusion value, Kind notation) => ToCollectionString([value], notation);

	/// <inheritdoc/>
	public static string ToCollectionString(Conclusion[] collection, Kind notation)
	{
		return collection switch
		{
			[] => string.Empty,
			[var value] => $"{CellsMap[value.Cell]}{value.ConclusionType.Notation()}{value.Digit + 1}",
			_ => toString(collection)
		};


		static int cmp(scoped in Conclusion left, scoped in Conclusion right) => left.CompareTo(right);
		static string toString(Conclusion[] c)
		{
			var conclusions = new Conclusion[c.Length];
			Array.Copy(c, 0, conclusions, 0, c.Length);

			scoped var sb = new StringHandler(50);
			unsafe { conclusions.Sort(&cmp); }

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
					sb.Append(", ");
				}

				sb.RemoveFromEnd(2);
				if (!hasOnlyOneType)
				{
					sb.Append(", ");
				}
			}

			if (!hasOnlyOneType)
			{
				sb.RemoveFromEnd(2);
			}

			return sb.ToStringAndClear();
		}
	}
}
