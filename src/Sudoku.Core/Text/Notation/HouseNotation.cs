namespace Sudoku.Text.Notation;

/// <summary>
/// Represents a type that can output notations for <see cref="House"/> values.
/// </summary>
public sealed partial class HouseNotation : INotation<HouseNotation, House[], House, HouseNotation.Kind>
{
	/// <inheritdoc cref="ToString(House, Kind)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToString(House value) => ToCollectionString([value], Kind.Normal);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToString(House value, Kind notation) => ToCollectionString([value], notation);

	/// <inheritdoc cref="ToCollectionString(in House[], Kind)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToMaskString(HouseMask value) => ToCollectionString([.. value.GetAllSets()], Kind.Normal);

	/// <inheritdoc cref="ToCollectionString(in House[], Kind)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToMaskString(HouseMask value, Kind notation) => ToCollectionString([.. value.GetAllSets()], Kind.Normal);

	/// <inheritdoc cref="ToCollectionString(in House[], Kind)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToCollectionString(scoped in House[] collection) => ToCollectionString(collection, Kind.Normal);

	/// <inheritdoc/>
	public static string ToCollectionString(scoped in House[] collection, Kind notation)
	{
		return notation switch
		{
			Kind.Normal => toNormalString(collection),
			Kind.CapitalOnly => formatSimple(collection),
			_ => throw new ArgumentOutOfRangeException(nameof(notation))
		};


		static string formatSimple(House[] collection)
		{
			var houseMask = collection.Aggregate(static (interim, next) => interim | (1 << next));
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

		static string toNormalString(House[] collection)
		{
			return collection switch
			{
				[] => string.Empty,
				[var r] => $"{GetLabel(r / 9)}{r % 9 + 1}",
				_ => f(collection)
			};


			static string f(House[] collection)
			{
				var dic = new Dictionary<HouseType, List<int>>(3);
				foreach (var house in collection)
				{
					var houseType = house.ToHouseType();
					if (!dic.TryAdd(houseType, [house]))
					{
						dic[houseType].Add(house);
					}
				}

				scoped var sb = new StringHandler(30);
				foreach (var (houseType, houses) in from kvp in dic orderby kvp.Key.GetProgramOrder() select kvp)
				{
					sb.Append(houseType.GetLabel());
					sb.AppendRange(from house in houses select house % 9 + 1);
				}

				return sb.ToStringAndClear();
			}
		}
	}

	/// <inheritdoc/>
	/// <exception cref="NotSupportedException">Throws always.</exception>
	[DoesNotReturn]
	static int INotation<HouseNotation, House, Kind>.Parse(string text, Kind notation)
		=> throw new NotSupportedException("This method may not be implemented.");

	/// <inheritdoc/>
	[DoesNotReturn]
	static int[] INotation<HouseNotation, House[], House, Kind>.ParseCollection(string text, Kind notation)
		=> throw new NotSupportedException("This method may not be implemented.");

	/// <summary>
	/// Get the label of each house.
	/// </summary>
	/// <param name="houseIndex">The house index.</param>
	/// <returns>The label.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static char GetLabel(House houseIndex) => houseIndex switch { 0 => 'b', 1 => 'r', 2 => 'c' };
}
