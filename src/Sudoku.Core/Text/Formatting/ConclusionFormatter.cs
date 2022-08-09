namespace Sudoku.Text.Formatting;

/// <summary>
/// Provides with a formatter that can format a list of <see cref="Conclusion"/>s,
/// represented as a <see cref="string"/> value.
/// </summary>
public abstract class ConclusionFormatter : ICollectionFormatter<Conclusion>
{
	/// <exception cref="NotSupportedException"/>
	[Obsolete("Do not use this constructor", true)]
	private protected ConclusionFormatter() => throw new NotSupportedException();


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


		string f(scoped in Span<Conclusion> c)
		{
			var conclusions = c.ToArray(); // This operation will copy the inner data.
			scoped var sb = new StringHandler(50);
			if (shouldSort)
			{
				unsafe { conclusions.Sort(&cmp); }

				var selection =
					from conclusion in conclusions
					orderby conclusion.Digit
					group conclusion by conclusion.ConclusionType;
				bool hasOnlyOneType = selection.HasOnlyOneElement();
				foreach (var typeGroup in selection)
				{
					string op = typeGroup.Key == ConclusionType.Assignment ? " = " : " <> ";
					foreach (var digitGroup in from conclusion in typeGroup group conclusion by conclusion.Digit)
					{
						sb.Append(Cells.Empty + from conclusion in digitGroup select conclusion.Cell);
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
			FormattingMode.Simple
				=> EliminationNotation.ToCandidatesString(
					new(from conclusion in conclusions select (conclusion.Cell * 9 + conclusion.Digit))
				),
			FormattingMode.Normal => Format(conclusions, R["Comma"]!, true),
			FormattingMode.Full => throw new NotSupportedException("The full-formatting mode is not supported on conclusion collections."),
			_ => throw new ArgumentOutOfRangeException(nameof(formattingMode))
		};


	static string ICollectionFormatter<Conclusion>.Format(IEnumerable<Conclusion> elements, string separator)
		=> Format(elements.ToArray(), separator, false);

	static string ICollectionFormatter<Conclusion>.Format(IEnumerable<Conclusion> elements, FormattingMode formattingMode)
		=> Format(elements.ToArray(), formattingMode);
}
