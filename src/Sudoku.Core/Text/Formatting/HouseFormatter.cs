namespace Sudoku.Text.Formatting;

/// <summary>
/// Provides with a formatter that can format a mask that represents for a list of houses.
/// </summary>
public abstract class HouseFormatter : ICollectionFormatter<House>
{
	/// <summary>
	/// Gets a <see cref="string"/> value that can describes for a list of houses that is represented
	/// as an <see cref="int"/> mask.
	/// </summary>
	/// <param name="housesMask">The houses mask.</param>
	/// <returns>The <see cref="string"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Format(int housesMask) => Format(housesMask, FormattingMode.Normal);

	/// <inheritdoc cref="Format(int)"/>
	/// <param name="houseMask"><inheritdoc/></param>
	/// <param name="formattingMode">The formatting mode.</param>
	/// <returns><inheritdoc/></returns>
	/// <exception cref="NotSupportedException">
	/// Throws when the argument <paramref name="formattingMode"/> is <see cref="FormattingMode.Full"/>.
	/// </exception>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="formattingMode"/> is not defined.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Format(HouseMask houseMask, FormattingMode formattingMode)
	{
		return formattingMode switch
		{
			FormattingMode.Simple => formatSimple(houseMask),
			FormattingMode.Normal => formatNormal(houseMask),
			FormattingMode.Full => throw new NotSupportedException("The full-formatting mode is not supported on digit collections."),
			_ => throw new ArgumentOutOfRangeException(nameof(formattingMode))
		};


		static string formatSimple(HouseMask houseMask)
		{
			scoped var sb = new StringHandler(27);
			for (var (houseIndex, i) = (9, 0); i < 27; i++, houseIndex = (houseIndex + 1) % 27)
			{
				if ((houseMask >> houseIndex & 1) != 0)
				{
					sb.Append(GetLabel(houseIndex / 9));
				}
			}

			return sb.ToStringAndClear();
		}

		static string formatNormal(HouseMask housesMask)
		{
			return PopCount((uint)housesMask) switch
			{
				0 => string.Empty,
				1 when Log2((uint)housesMask) is var r => $"{GetLabel(r / 9)}{r % 9 + 1}",
				_ => f(housesMask)
			};


			static string f(HouseMask housesMask)
			{
				var dic = new Dictionary<HouseType, List<int>>();
				foreach (var house in housesMask)
				{
					var houseType = house.ToHouseType();
					if (!dic.TryAdd(houseType, new() { house }))
					{
						dic[houseType].Add(house);
					}
				}

				scoped var sb = new StringHandler(30);
				foreach (var (houseType, houses) in
					from kvp in dic
					let key = kvp.Key
					orderby key switch { HouseType.Block => 2, HouseType.Row => 0, HouseType.Column => 1 }
					select kvp)
				{
					sb.Append(houseType.GetLabel());
					sb.AppendRange(from house in houses select house % 9 + 1);
				}

				return sb.ToStringAndClear();
			}
		}
	}

	/// <summary>
	/// <para>
	/// Gets a <see cref="string"/> value that can describes for a list of houses that is represented
	/// as a <see cref="ReadOnlySpan{T}"/> of <see cref="House"/> elements.
	/// </para>
	/// <para>
	/// Although the argument <paramref name="houses"/> is of type <see cref="ReadOnlySpan{T}"/>, you can still
	/// pass an array of <see cref="House"/> elements as the value.
	/// </para>
	/// </summary>
	/// <param name="houses">The houses.</param>
	/// <returns>The <see cref="string"/> result.</returns>
	public static string Format(scoped ReadOnlySpan<House> houses)
	{
		var targetMask = 0;
		foreach (var house in houses)
		{
			targetMask |= 1 << house;
		}

		return Format(targetMask);
	}

	/// <inheritdoc/>
	static string ICollectionFormatter<House>.Format(IEnumerable<House> elements, string separator)
	{
		var targetMask = 0;
		foreach (var element in elements)
		{
			targetMask |= 1 << element;
		}

		return Format(targetMask);
	}

	/// <inheritdoc/>
	static string ICollectionFormatter<House>.Format(IEnumerable<House> elements, FormattingMode formattingMode)
	{
		var targetMask = 0;
		foreach (var element in elements)
		{
			targetMask |= 1 << element;
		}

		return Format(targetMask, formattingMode);
	}

	/// <summary>
	/// Get the label of each house.
	/// </summary>
	/// <param name="houseIndex">The house index.</param>
	/// <returns>The label.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static char GetLabel(House houseIndex) => houseIndex switch { 0 => 'b', 1 => 'r', 2 => 'c' };
}
