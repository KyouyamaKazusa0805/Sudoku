namespace Sudoku.Data
{
	/// <summary>
	/// Defines a segment that stores a part of a sudoku grid.
	/// </summary>
	public unsafe ref partial struct SudokuGridSegment
	{
		/// <summary>
		/// Indicates the mask list and the candidates list.
		/// </summary>
		private fixed short _maskList[9], _candidatesList[9];


		/// <summary>
		/// Initializes a <see cref="SudokuGridSegment"/> with the specified region,
		/// and what sudoku grid filters.
		/// </summary>
		/// <param name="grid">The sudoku grid.</param>
		/// <param name="region">The region to project.</param>
		/// <exception cref="IndexOutOfRangeException">
		/// Throws when the argument <paramref name="region"/> isn't between 0 and 26.
		/// </exception>
		public SudokuGridSegment(in SudokuGrid grid, int region)
		{
			int[] cells = RegionCells[region];

			fixed (short* pGrid = grid)
			fixed (int* pCells = cells)
			{
				for (int index = 0; index < 9; index++)
				{
					short mask = pGrid[pCells[index]];
					_maskList[index] = mask;
					_candidatesList[index] = (short)(mask & SudokuGrid.MaxCandidatesMask);
				}
			}

			RegionIndex = region;
			Cells = cells;
		}

		/// <summary>
		/// Initialzes a <see cref="SudokuGridSegment"/> with the specified region label,
		/// the index and a sudoku grid that filters.
		/// </summary>
		/// <param name="grid">The sudoku grid.</param>
		/// <param name="regionLabel">The region label.</param>
		/// <param name="index">The index.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public SudokuGridSegment(in SudokuGrid grid, RegionLabel regionLabel, int index)
			: this(grid, (int)regionLabel * 9 + index)
		{
		}


		/// <summary>
		/// Indicates the inner masks.
		/// </summary>
		public readonly short* Masks
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				fixed (short* s = _maskList)
				{
					return s;
				}
			}
		}

		/// <summary>
		/// Indicates the candicates calculated.
		/// </summary>
		public readonly short* Candidates
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				fixed (short* s = _candidatesList)
				{
					return s;
				}
			}
		}

		/// <summary>
		/// Indicates the region index.
		/// </summary>
		public int RegionIndex { get; }

		/// <summary>
		/// Indicates the cells covered.
		/// </summary>
		public Cells Cells { get; }


		/// <summary>
		/// Indicates the empty segment value that holds the undefined value.
		/// </summary>
		public static SudokuGridSegment Undefined => default;


		/// <summary>
		/// Returns a reference to the element of the <see cref="SudokuGridSegment"/> at index zero.
		/// </summary>
		/// <returns>A reference to the element of the <see cref="SudokuGridSegment"/> at index zero.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public readonly ref readonly short GetPinnableReference() => ref _maskList[0];

		/// <summary>
		/// Returns a reference to the element of the <see cref="SudokuGridSegment"/> at index zero.
		/// </summary>
		/// <param name="pinnedItem">
		/// The item you want to fix. If the value is
		/// <list type="table">
		/// <item>
		/// <term><see cref="PinnedItem.Masks"/></term>
		/// <description>The original mask list of pointer value will be returned.</description>
		/// </item>
		/// <item>
		/// <term><see cref="PinnedItem.CandidateMasks"/></term>
		/// <description>The candidate mask list of pointer value will be returned.</description>
		/// </item>
		/// </list>
		/// </param>
		/// <returns>A reference to the element of the <see cref="SudokuGridSegment"/> at index zero.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[return: MaybeNullWhenNotDefined("pinnedItem")]
		public readonly ref readonly short GetPinnableReference(PinnedItem pinnedItem) =>
			ref pinnedItem == PinnedItem.Masks
			? ref _maskList[0]
			: ref pinnedItem == PinnedItem.CandidateMasks
			? ref _candidatesList[0]
			: ref *(short*)null;

		/// <summary>
		/// Converts the collection into an array of type <see cref="short"/>.
		/// </summary>
		/// <returns>The array.</returns>
		public readonly short[] ToArray()
		{
			short[] result = new short[9];
			fixed (short* pResult = result, pMaskList = _maskList)
			{
				Unsafe.CopyBlock(pResult, pMaskList, sizeof(short) * 9);
			}

			return result;
		}

		/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Enumerator GetEnumerator()
		{
			fixed (short* arr = _maskList)
			{
				return new Enumerator(arr);
			}
		}


		/// <summary>
		/// Determine whether two <see cref="SudokuGridSegment"/>s are equal.
		/// </summary>
		/// <param name="left">The left instance to check.</param>
		/// <param name="right">The right instance to check.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		[ProxyEquality]
		public static bool Equals(in SudokuGridSegment left, in SudokuGridSegment right)
		{
			fixed (short* l = left._maskList, r = right._maskList)
			{
				short* a = l, b = r;
				for (int i = 0; i < 9; i++)
				{
					if (*a++ != *b++)
					{
						return false;
					}
				}
			}

			return true;
		}
	}
}
