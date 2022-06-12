namespace Sudoku.Concepts.Collections.Handlers;

/// <summary>
/// Indicates a house collection.
/// </summary>
public readonly ref partial struct HouseCollection
{
	/// <summary>
	/// Initializes an empty collection and add one house into the list.
	/// </summary>
	/// <param name="houseIndex">The house index.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public HouseCollection(int houseIndex) => Mask |= 1 << houseIndex;

	/// <summary>
	/// Initializes an instance with the specified houses.
	/// </summary>
	/// <param name="houseIndices">The house indices.</param>
	public HouseCollection(in ReadOnlySpan<int> houseIndices)
	{
		foreach (int houseIndex in houseIndices)
		{
			Mask |= 1 << houseIndex;
		}
	}

	/// <summary>
	/// Initializes an instance with the specified houses.
	/// </summary>
	/// <param name="houseIndices">The house indices.</param>
	public HouseCollection(IEnumerable<int> houseIndices)
	{
		foreach (int houseIndex in houseIndices)
		{
			Mask |= 1 << houseIndex;
		}
	}


	/// <summary>
	/// Indicates the number of houses that contain in this collection.
	/// </summary>
	public int Count
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => PopCount((uint)Mask);
	}

	/// <summary>
	/// Indicates the mask used.
	/// </summary>
	public int Mask { get; } = 0;


	/// <summary>
	/// Gets a <see cref="bool"/> value indicating whether the bit
	/// of the corresponding specified house is set <see langword="true"/>.
	/// </summary>
	/// <param name="houseIndex">The house index.</param>
	/// <returns>A <see cref="bool"/> value.</returns>
	public bool this[int houseIndex]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Mask >> houseIndex & 1) != 0;
	}


	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(HouseCollection other) => Mask == other.Mask;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => Mask;

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
	{
		return Count switch
		{
			0 => string.Empty,
			1 when Log2((uint)Mask) is var r => $"{GetLabel(r / 9)}{r % 9 + 1}",
			_ => f(this)
		};


		static string f(HouseCollection @this)
		{
			var dic = new Dictionary<int, ICollection<int>>();
			foreach (int houseIndex in @this)
			{
				if (!dic.ContainsKey(houseIndex / 9))
				{
					dic.Add(houseIndex / 9, new List<int>());
				}

				dic[houseIndex / 9].Add(houseIndex % 9);
			}

			var sb = new StringHandler(30);
			for (int i = 1, j = 0; j < 3; i = (i + 1) % 3, j++)
			{
				if (!dic.ContainsKey(i))
				{
					continue;
				}

				sb.Append(@this.GetLabel(i));
				foreach (int z in dic[i])
				{
					sb.Append(z + 1);
				}
			}

			return sb.ToStringAndClear();
		}
	}

	/// <summary>
	/// To string but only output the labels ('r', 'c' or 'b').
	/// </summary>
	/// <returns>The labels.</returns>
	public string ToSimpleString()
	{
		var sb = new StringHandler(27);
		for (int houseIndex = 9, i = 0; i < 27; i++, houseIndex = (houseIndex + 1) % 27)
		{
			if (this[houseIndex])
			{
				sb.Append(GetLabel(houseIndex / 9));
			}
		}

		return sb.ToStringAndClear();
	}

	/// <summary>
	/// Gets the enumerator of the instance in order to use <see langword="foreach"/> loop.
	/// </summary>
	/// <returns>The enumerator instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ReadOnlySpan<int>.Enumerator GetEnumerator() => Mask.GetEnumerator();

	/// <summary>
	/// Get the label of each house.
	/// </summary>
	/// <param name="houseIndex">The house index.</param>
	/// <returns>The label.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private char GetLabel(int houseIndex) => houseIndex switch { 0 => 'b', 1 => 'r', 2 => 'c', _ => default };
}
