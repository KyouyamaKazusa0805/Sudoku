namespace Sudoku.Concepts.Solving.ChainNodes;

/// <summary>
/// Defines a chain node that provides with the data for an almost locked set.
/// </summary>
public sealed class AlmostLockedSetNode : Node
{
	/// <summary>
	/// Indicates the shifting bits that each cell stores in the <see cref="Node._other"/> mask.
	/// </summary>
	/// <seealso cref="Node._other"/>
	private const int Shifting = 7;


	/// <summary>
	/// Initializes an <see cref="AlmostLockedSetNode"/> instance via the digit used
	/// and the cells used.
	/// </summary>
	/// <param name="digit">The digit used.</param>
	/// <param name="cells">The cells used.</param>
	/// <param name="extraCells">
	/// Indicates the extra cells that the current ALS used, but the current digit doesn't used.
	/// </param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public AlmostLockedSetNode(byte digit, in Cells cells, in Cells extraCells) :
		base(NodeType.AlmostLockedSets, digit, cells, InitOtherMask(extraCells))
	{
	}


	/// <summary>
	/// Indicates the whole ALS map.
	/// </summary>
	public Cells FullCells
	{
		get
		{
			var result = Cells;
			long otherCells = _other >> Shifting & (1L << (sizeof(long) << 3) - Shifting) - 1;
			while (otherCells != 0)
			{
				int cell = (int)(otherCells >> Shifting & 127);
				result.Add(cell);

				otherCells >>= Shifting;
			}

			return result;
		}
	}


	/// <inheritdoc/>
	public override string ToSimpleString() => $"{Digit + 1}{FullCells}";


	/// <summary>
	/// Gets the part of the other mask value that represents the extra cells used.
	/// </summary>
	/// <param name="extraCells">The extra cells used.</param>
	/// <returns>The mask of type <see cref="long"/>.</returns>
	/// <exception cref="ArgumentException">
	/// Throws when the number of elements in the argument <paramref name="extraCells"/>
	/// is greater than 8.
	/// </exception>
	private static long InitOtherMask(in Cells extraCells)
	{
		if (extraCells.Count > 8)
		{
			// 8 * 7 <= 64 - 7
			throw new ArgumentException(
				$"The length of the argument '{nameof(extraCells)}' must be lower than 8.",
				nameof(extraCells));
		}

		long finalMask = 0;
		int i = 0;
		foreach (int cell in extraCells)
		{
			finalMask |= (long)(cell << Shifting * i++);
		}

		// Preserve the specified bits being zeroed.
		return finalMask << PreservedBitsCount;
	}
}
