namespace Sudoku.Text.Formatting;

/// <summary>
/// Provides with a formatter that can format a mask that represents for a list of digits.
/// </summary>
public abstract class DigitMaskFormatter : ICollectionFormatter<Digit>
{
	/// <exception cref="NotSupportedException"/>
	[Obsolete("Do not use this constructor", true)]
	private protected DigitMaskFormatter() => throw new NotSupportedException();


	/// <summary>
	/// Gets a <see cref="string"/> value that can describes for a list of digits that is represented as a <see cref="Mask"/> mask.
	/// </summary>
	/// <param name="digitsMask">The digits mask.</param>
	/// <param name="separator">The separator used.</param>
	/// <returns>The <see cref="string"/> result.</returns>
	public static string Format(Mask digitsMask, string separator)
	{
		return digitsMask switch
		{
			0 => "{ }",
			_ when IsPow2(digitsMask) => (TrailingZeroCount(digitsMask) + 1).ToString(),
			_ => defaultToString(digitsMask, separator)
		};


		static string defaultToString(Mask digitsMask, string separator)
		{
			scoped var sb = new StringHandler(9);
			foreach (var digit in digitsMask)
			{
				sb.Append(digit + 1);
				sb.Append(separator);
			}

			sb.RemoveFromEnd(separator.Length);
			return sb.ToStringAndClear();
		}
	}

	/// <inheritdoc cref="Format(Mask, string)"/>
	/// <param name="digitsMask"><inheritdoc/></param>
	/// <param name="formattingMode">The formatting mode.</param>
	/// <returns><inheritdoc/></returns>
	/// <exception cref="NotSupportedException">
	/// Throws when the argument <paramref name="formattingMode"/> is <see cref="FormattingMode.Full"/>.
	/// </exception>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="formattingMode"/> is not defined.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Format(Mask digitsMask, FormattingMode formattingMode = FormattingMode.Simple)
		=> formattingMode switch
		{
			FormattingMode.Simple => Format(digitsMask, string.Empty),
			FormattingMode.Normal => Format(digitsMask, ", "),
			FormattingMode.Full => throw new NotSupportedException("The full-formatting mode is not supported on digit collections."),
			_ => throw new ArgumentOutOfRangeException(nameof(formattingMode))
		};

	/// <inheritdoc/>
	static string ICollectionFormatter<Digit>.Format(IEnumerable<Digit> elements, string separator)
	{
		var targetMask = (Mask)0;
		foreach (var element in elements)
		{
			targetMask |= (Mask)(1 << element);
		}

		return Format(targetMask, separator);
	}

	/// <inheritdoc/>
	static string ICollectionFormatter<Digit>.Format(IEnumerable<Digit> elements, FormattingMode formattingMode)
	{
		var targetMask = (Mask)0;
		foreach (var element in elements)
		{
			targetMask |= (Mask)(1 << element);
		}

		return Format(targetMask, formattingMode);
	}
}
