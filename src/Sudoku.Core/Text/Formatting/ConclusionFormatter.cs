namespace Sudoku.Text.Formatting;

/// <summary>
/// Provides with a formatter that can format a list of <see cref="Conclusion"/>s,
/// represented as a <see cref="string"/> value.
/// </summary>
public abstract class ConclusionFormatter : ICollectionFormatter<Conclusion>
{
	/// <summary>
	/// Formats a list of conclusions as the string representation.
	/// </summary>
	/// <param name="conclusions">The list of conclusions to be formatted.</param>
	/// <param name="separator">The separator.</param>
	/// <param name="shouldSort">
	/// Indicates whether the list of conclusions should be sorted before formatting.
	/// </param>
	/// <returns>The <see cref="string"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Format(Conclusion[] conclusions, string separator, bool shouldSort)
	{
		return conclusions switch
		{
			[] => string.Empty,
			[var conclusion] => conclusion.ToString(),
			_ => f(conclusions)
		};


		string f(scoped Span<Conclusion> c)
		{
			var conclusions = (Conclusion[])[.. c]; // This operation will copy the inner data.
			scoped var sb = new StringHandler(50);
			if (shouldSort)
			{
				unsafe { conclusions.Sort(&cmp); }

				var selection =
					from conclusion in conclusions
					orderby conclusion.Digit
					group conclusion by conclusion.ConclusionType;
				var hasOnlyOneType = selection.HasOnlyOneElement();
				foreach (var typeGroup in selection)
				{
					var op = typeGroup.Key.Notation();
					foreach (var digitGroup in from conclusion in typeGroup group conclusion by conclusion.Digit)
					{
						sb.Append(CellMap.Empty + from conclusion in digitGroup select conclusion.Cell);
						sb.Append(op);
						sb.Append(digitGroup.Key + 1);
						sb.Append(separator);
					}

					sb.RemoveFromEnd(separator.Length);
					if (!hasOnlyOneType)
					{
						sb.Append(separator);
					}
				}

				if (!hasOnlyOneType)
				{
					sb.RemoveFromEnd(separator.Length);
				}
			}
			else
			{
				sb.AppendRangeWithSeparator(conclusions, StringHandler.ElementToStringConverter, separator);
			}

			return sb.ToStringAndClear();
		}


		static int cmp(in Conclusion left, in Conclusion right) => left.CompareTo(right);
	}

	/// <inheritdoc cref="Format(Conclusion[], string, bool)"/>
	/// <param name="conclusions"><inheritdoc/></param>
	/// <param name="formattingMode">The formatting mode.</param>
	/// <returns><inheritdoc/></returns>
	/// <exception cref="NotSupportedException">
	/// Throws when the argument <paramref name="formattingMode"/> is <see cref="FormattingMode.Full"/>.
	/// </exception>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="formattingMode"/> is not defined.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Format(Conclusion[] conclusions, FormattingMode formattingMode)
		=> formattingMode switch
		{
			FormattingMode.Simple => CandidateConceptNotation.ToCollectionString([.. from c in conclusions select c.Cell * 9 + c.Digit], CandidateNotationKind.HodokuTriplet),
			FormattingMode.Normal => Format(conclusions, ", ", true),
			FormattingMode.Full => throw new NotSupportedException("The full-formatting mode is not supported on conclusion collections."),
			_ => throw new ArgumentOutOfRangeException(nameof(formattingMode))
		};

	/// <inheritdoc/>
	static string ICollectionFormatter<Conclusion>.Format(IEnumerable<Conclusion> elements, string separator)
		=> Format([.. elements], separator, false);

	/// <inheritdoc/>
	static string ICollectionFormatter<Conclusion>.Format(IEnumerable<Conclusion> elements, FormattingMode formattingMode)
		=> Format([.. elements], formattingMode);
}
