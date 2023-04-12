namespace Sudoku.Text.Formatting;

/// <summary>
/// Provides with a formatter that can format a mask that represents for a list of houses.
/// </summary>
public abstract class HouseFormatter : ICollectionFormatter<int>
{
	/// <exception cref="NotSupportedException"/>
	[Obsolete("Do not use this constructor", true)]
	private protected HouseFormatter() => throw new NotSupportedException();


	/// <summary>
	/// Gets a <see cref="string"/> value that can describes for a list of houses that is represented
	/// as an <see cref="int"/> mask.
	/// </summary>
	/// <param name="housesMask">The houses mask.</param>
	/// <returns>The <see cref="string"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Format(int housesMask) => Format(housesMask, FormattingMode.Normal);

	/// <inheritdoc cref="Format(int)"/>
	/// <param name="housesMask"><inheritdoc/></param>
	/// <param name="formattingMode">The formatting mode.</param>
	/// <returns><inheritdoc/></returns>
	/// <exception cref="NotSupportedException">
	/// Throws when the argument <paramref name="formattingMode"/> is <see cref="FormattingMode.Full"/>.
	/// </exception>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="formattingMode"/> is not defined.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Format(int housesMask, FormattingMode formattingMode)
	{
		return formattingMode switch
		{
			FormattingMode.Simple => formatSimple(housesMask),
			FormattingMode.Normal => formatNormal(housesMask),
			FormattingMode.Full => throw new NotSupportedException("The full-formatting mode is not supported on digit collections."),
			_ => throw new ArgumentOutOfRangeException(nameof(formattingMode))
		};


		static string formatSimple(int housesMask)
		{
			scoped var sb = new StringHandler(27);
			for (int houseIndex = 9, i = 0; i < 27; i++, houseIndex = (houseIndex + 1) % 27)
			{
				if ((housesMask >> houseIndex & 1) != 0)
				{
					sb.Append(GetLabel(houseIndex / 9));
				}
			}

			return sb.ToStringAndClear();
		}

		static string formatNormal(int housesMask)
		{
			return PopCount((uint)housesMask) switch
			{
				0 => string.Empty,
				1 when Log2((uint)housesMask) is var r => $"{GetLabel(r / 9)}{r % 9 + 1}",
				_ => f(housesMask)
			};


			static string f(int housesMask)
			{
				var dic = new Dictionary<int, ICollection<int>>();
				foreach (var houseIndex in housesMask)
				{
					if (!dic.ContainsKey(houseIndex / 9))
					{
						dic.Add(houseIndex / 9, new List<int>());
					}

					dic[houseIndex / 9].Add(houseIndex % 9);
				}

				scoped var sb = new StringHandler(30);
				for (int i = 1, j = 0; j < 3; i = (i + 1) % 3, j++)
				{
					if (!dic.ContainsKey(i))
					{
						continue;
					}

					sb.Append(GetLabel(i));
					foreach (var z in dic[i])
					{
						sb.Append(z + 1);
					}
				}

				return sb.ToStringAndClear();
			}
		}
	}

	/// <summary>
	/// <para>
	/// Gets a <see cref="string"/> value that can describes for a list of houses that is represented
	/// as a <see cref="ReadOnlySpan{T}"/> of <see cref="int"/> elements.
	/// </para>
	/// <para>
	/// Although the argument <paramref name="houses"/> is of type <see cref="ReadOnlySpan{T}"/>, you can still
	/// pass an array of <see cref="int"/> elements as the value.
	/// </para>
	/// </summary>
	/// <param name="houses">The houses.</param>
	/// <returns>The <see cref="string"/> result.</returns>
	public static string Format(scoped ReadOnlySpan<int> houses)
	{
		var targetMask = (Mask)0;
		foreach (var house in houses)
		{
			targetMask |= (Mask)(1 << house);
		}

		return Format(targetMask);
	}

	/// <inheritdoc/>
	static string ICollectionFormatter<int>.Format(IEnumerable<int> elements, string separator)
	{
		var targetMask = (Mask)0;
		foreach (var element in elements)
		{
			targetMask |= (Mask)(1 << element);
		}

		return Format(targetMask);
	}

	/// <inheritdoc/>
	static string ICollectionFormatter<int>.Format(IEnumerable<int> elements, FormattingMode formattingMode)
	{
		var targetMask = (Mask)0;
		foreach (var element in elements)
		{
			targetMask |= (Mask)(1 << element);
		}

		return Format(targetMask, formattingMode);
	}

	/// <summary>
	/// Get the label of each house.
	/// </summary>
	/// <param name="houseIndex">The house index.</param>
	/// <returns>The label.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static char GetLabel(int houseIndex) => houseIndex switch { 0 => 'b', 1 => 'r', 2 => 'c' };
}
