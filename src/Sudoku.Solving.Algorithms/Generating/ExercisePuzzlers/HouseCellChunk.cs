namespace Sudoku.Generating.ExercisePuzzlers;

/// <summary>
/// Defines a chunk that stores the cells in a house.
/// </summary>
[GeneratedOverloadingOperator(GeneratedOperator.EqualityOperators)]
[GeneratedOverloadingOperator(GeneratedOperator.ComparisonOperators)]
public readonly partial struct HouseCellChunk :
	IComparable<HouseCellChunk>,
	IComparisonOperators<HouseCellChunk, HouseCellChunk, bool>,
	IEquatable<HouseCellChunk>,
	IEqualityOperators<HouseCellChunk, HouseCellChunk, bool>
{
	/// <summary>
	/// Indicates the mask.
	/// </summary>
	private readonly long _mask;


	/// <summary>
	/// Initializes a <see cref="HouseCellChunk"/> instance via the chunk of 9 values. 0 is for empty cell.
	/// </summary>
	/// <param name="house">Indicates the house used.</param>
	/// <param name="chunkValues">A chunk of values. This argument must hold 9 elements.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe HouseCellChunk(int house, int* chunkValues)
		=> _mask = (long)house << 36
			| (long)chunkValues[0] | (long)chunkValues[1] << 4 | (long)chunkValues[2] << 8
			| (long)chunkValues[3] << 12 | (long)chunkValues[4] << 16 | (long)chunkValues[5] << 20
			| (long)chunkValues[6] << 24 | (long)chunkValues[7] << 28 | (long)chunkValues[8] << 32;


	/// <summary>
	/// Indicates the house used.
	/// </summary>
	public int House
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (int)(_mask >> 36) & 15;
	}

	/// <summary>
	/// Indicates the result candidate.
	/// </summary>
	public int ResultCandidate
	{
		get
		{
			var (cells, digits) = this;
			for (var i = 0; i < 9; i++)
			{
				if ((cells[i], digits[i]) is (var cell, 0xF))
				{
					short mask = 511;
					for (var j = 0; j < 9; j++)
					{
						if (digits[j] != 0xF)
						{
							mask |= (short)(1 << digits[j]);
						}
					}

					return cell * 9 + TrailingZeroCount(mask);
				}
			}

			return -1;
		}
	}

	/// <summary>
	/// Indicates the cells used in this structure.
	/// </summary>
	public CellMap Cells
	{
		get
		{
			var digits = Digits;
			var houseCells = HouseCells[House];
			var result = CellMap.Empty;
			for (var i = 0; i < 9; i++)
			{
				if (digits[i] == 0xF)
				{
					result.Add(houseCells[i]);
				}
			}

			return result;
		}
	}

	/// <summary>
	/// Indicates the digits used.
	/// </summary>
	private int[] Digits
	{
		get
		{
			var digits = new int[9];
			for (byte i = 0; i < 9; i++)
			{
				digits[i] = (byte)(_mask >> (i << 2) & 15);
			}

			return digits;
		}
	}


	[GeneratedDeconstruction]
	public partial void Deconstruct(out CellMap cells, out int[] digits);

	[GeneratedOverriddingMember(GeneratedEqualsBehavior.TypeCheckingAndCallingOverloading)]
	public override partial bool Equals(object? obj);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(HouseCellChunk other) => _mask == other._mask;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.SimpleField, nameof(_mask))]
	public override partial int GetHashCode();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(object? obj)
	{
		ArgumentNullException.ThrowIfNull(obj);
		Argument.ThrowIfFalse(obj is HouseCellChunk, $"The type must be '{nameof(HouseCellChunk)}'.");

		return CompareTo((HouseCellChunk)obj);
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(HouseCellChunk other) => _mask.CompareTo(other._mask);

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
	{
		var resultCand = ResultCandidate;
		int cell = resultCand / 9, digit = resultCand % 9;
		var houseStr = HouseFormatter.Format(1 << House);
		var cellStr = RxCyNotation.ToCellString(cell);
		var digitStr = (digit + 1).ToString();
		return $$"""{{nameof(HouseCellChunk)}} { {{nameof(House)}} = {{houseStr}}, {{nameof(ResultCandidate)}} = {{cellStr}}({{digitStr}}) }""";
	}


	/// <summary>
	/// Converts the current <see cref="HouseCellChunk"/> instance into a <see cref="Grid"/>.
	/// </summary>
	/// <param name="chunk">The <see cref="HouseCellChunk"/> instance.</param>
	public static implicit operator Grid(HouseCellChunk chunk)
	{
		var (cells, digits) = chunk;
		var result = Grid.Empty;
		for (var i = 0; i < 9; i++)
		{
			if (digits[i] is var digit and not 0xF)
			{
				result[cells[i]] = digit;
			}
		}

		return result;
	}
}
