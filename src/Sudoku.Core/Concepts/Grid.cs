#define TARGET_64BIT
namespace Sudoku.Concepts;

/// <summary>
/// Represents a sudoku grid that uses the mask list to construct the data structure.
/// </summary>
/// <remarks>
/// <para><include file="../../global-doc-comments.xml" path="/g/large-structure"/></para>
/// <para>
/// Begin with C# 12, we can use feature "Inline Arrays" to access internal masks as raw values via indexers.
/// For example, You can use <c>grid[cellIndex]</c> to get the raw mask at the index <c>cellIndex</c>, whose value is between 0 and 81
/// (include 0 but not include 81).
/// </para>
/// </remarks>
[JsonConverter(typeof(Converter))]
[DebuggerDisplay($$"""{{{nameof(ToString)}}("#")}""")]
[InlineArray(81)]
[LargeStructure]
[Equals]
[ToString]
[EqualityOperators]
public unsafe partial struct Grid :
	IEqualityOperators<Grid, Grid, bool>,
	IFormattable,
	IMinMaxValue<Grid>,
	IParsable<Grid>,
	IReadOnlyCollection<Digit>,
	ISimpleFormattable,
	ISimpleParsable<Grid>
{
	/// <summary>
	/// Indicates the default mask of a cell (an empty cell, with all 9 candidates left).
	/// </summary>
	public const Mask DefaultMask = EmptyMask | MaxCandidatesMask;

	/// <summary>
	/// Indicates the maximum candidate mask that used.
	/// </summary>
	public const Mask MaxCandidatesMask = (1 << 9) - 1;

	/// <summary>
	/// Indicates the empty mask, modifiable mask and given mask.
	/// </summary>
	public const Mask EmptyMask = (Mask)CellStatus.Empty << 9;

	/// <summary>
	/// Indicates the modifiable mask.
	/// </summary>
	public const Mask ModifiableMask = (Mask)CellStatus.Modifiable << 9;

	/// <summary>
	/// Indicates the given mask.
	/// </summary>
	public const Mask GivenMask = (Mask)CellStatus.Given << 9;


	/// <summary>
	/// Indicates the empty grid string.
	/// </summary>
	public static readonly string EmptyString = new('0', 81);

	/// <summary>
	/// Indicates the event triggered when the value is changed.
	/// </summary>
	/// <remarks>
	/// <include file="../../global-doc-comments.xml" path="/g/csharp9/feature[@name='function-pointer']"/>
	/// </remarks>
	public static readonly ValueChangedMethodPtr ValueChanged;

	/// <summary>
	/// Indicates the event triggered when should re-compute candidates.
	/// </summary>
	/// <remarks>
	/// <include file="../../global-doc-comments.xml" path="/g/csharp9/feature[@name='function-pointer']"/>
	/// </remarks>
	public static readonly RefreshingCandidatesMethodPtr RefreshingCandidates;

	/// <summary>
	/// The empty grid that is valid during implementation or running the program (all values are <see cref="DefaultMask"/>, i.e. empty cells).
	/// </summary>
	/// <remarks>
	/// This field is initialized by the static constructor of this structure.
	/// </remarks>
	/// <seealso cref="DefaultMask"/>
	public static readonly Grid Empty;

	/// <summary>
	/// Indicates the default grid that all values are initialized 0. This value is equivalent to <see langword="default"/>(<see cref="Grid"/>).
	/// </summary>
	/// <remarks>
	/// This value can be used for non-candidate-based sudoku operations, e.g. a sudoku grid canvas.
	/// </remarks>
	public static readonly Grid Undefined;

	/// <summary>
	/// Indicates the backing solver.
	/// </summary>
	private static readonly BitwiseSolver BackingSolver = new();


	/// <summary>
	/// Indicates the inner array that stores the masks of the sudoku grid, which stores the in-time sudoku grid inner information.
	/// </summary>
	/// <remarks>
	/// The field uses the mask table of length 81 to indicate the status and all possible candidates
	/// holding for each cell. Each mask uses a <see cref="Mask"/> value, but only uses 11 of 16 bits.
	/// <code>
	/// | 16  15  14  13  12  11  10  9   8   7   6   5   4   3   2   1   0 |
	/// |-------------------|-----------|-----------------------------------|
	/// |   |   |   |   |   | 0 | 0 | 1 | 1 | 1 | 1 | 1 | 1 | 1 | 1 | 1 | 1 |
	/// '-------------------|-----------|-----------------------------------'
	///                      \_________/ \_________________________________/
	///                          (2)                     (1)
	/// </code>
	/// Here the 9 bits in (1) indicate whether each digit is possible candidate in the current cell for each bit respectively,
	/// and the higher 3 bits in (2) indicate the cell status. The possible cell status are:
	/// <list type="table">
	/// <listheader>
	/// <term>Status name</term>
	/// <description>Description</description>
	/// </listheader>
	/// <item>
	/// <term>Empty cell (i.e. <see cref="CellStatus.Empty"/>)</term>
	/// <description>The cell is currently empty, and wait for being filled.</description>
	/// </item>
	/// <item>
	/// <term>Modifiable cell (i.e. <see cref="CellStatus.Modifiable"/>)</term>
	/// <description>The cell is filled by a digit, but the digit isn't the given by the initial grid.</description>
	/// </item>
	/// <item>
	/// <term>Given cell (i.e. <see cref="CellStatus.Given"/>)</term>
	/// <description>The cell is filled by a digit, which is given by the initial grid and can't be modified.</description>
	/// </item>
	/// </list>
	/// </remarks>
	/// <seealso cref="CellStatus"/>
	private Mask _values;


	/// <summary>
	/// Creates a <see cref="Grid"/> instance via the pointer of the first element of the cell digit,
	/// and the creating option.
	/// </summary>
	/// <param name="firstElement">
	/// <para>The reference of the first element.</para>
	/// <para>
	/// <include file='../../global-doc-comments.xml' path='g/csharp7/feature[@name="ref-returns"]/target[@name="in-parameter"]'/>
	/// </para>
	/// </param>
	/// <param name="creatingOption">The creating option.</param>
	/// <remarks>
	/// <include file='../../global-doc-comments.xml' path='g/csharp7/feature[@name="ref-returns"]/target[@name="method"]'/>
	/// </remarks>
	/// <exception cref="ArgumentNullRefException">
	/// Throws when the argument <paramref name="firstElement"/> is <see langword="null"/> reference.
	/// </exception>
	private Grid(scoped in Digit firstElement, GridCreatingOption creatingOption = GridCreatingOption.None)
	{
		ArgumentNullRefException.ThrowIfNullRef(ref AsRef(firstElement));

		// Firstly we should initialize the inner values.
		this = Empty;

		// Then traverse the array (span, pointer or etc.), to get refresh the values.
		var minusOneEnabled = creatingOption == GridCreatingOption.MinusOne;
		for (var i = 0; i < 81; i++)
		{
			var value = AddByteOffset(ref AsRef(firstElement), (nuint)(i * sizeof(Digit)));
			if ((minusOneEnabled ? value - 1 : value) is var realValue and not -1)
			{
				// Calls the indexer to trigger the event (Clear the candidates in peer cells).
				SetDigit(i, realValue);

				// Set the status to 'CellStatus.Given'.
				SetStatus(i, CellStatus.Given);
			}
		}
	}


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static Grid()
	{
		// Initializes the empty grid.
		Empty = default;
		scoped ref var firstElement = ref Empty[0];
		for (var i = 0; i < 81; i++)
		{
			AddByteOffset(ref firstElement, (nuint)(i * sizeof(Mask))) = DefaultMask;
		}

		// Initializes events.
		ValueChanged = &onValueChanged;
		RefreshingCandidates = &onRefreshingCandidates;

		// Initializes special fields.
		Undefined = default;


		static void onRefreshingCandidates(scoped ref Grid @this)
		{
			for (var i = 0; i < 81; i++)
			{
				if (@this.GetStatus(i) == CellStatus.Empty)
				{
					// Remove all appeared digits.
					var mask = MaxCandidatesMask;
					foreach (var cell in Peers[i])
					{
						if (@this.GetDigit(cell) is var digit and not -1)
						{
							mask &= (Mask)~(1 << digit);
						}
					}

					@this[i] = (Mask)(EmptyMask | mask);
				}
			}
		}

		static void onValueChanged(scoped ref Grid @this, Cell cell, Mask oldMask, Mask newMask, Digit setValue)
		{
			if (setValue != -1)
			{
				foreach (var peerCell in Peers[cell])
				{
					if (@this.GetStatus(peerCell) == CellStatus.Empty)
					{
						// You can't do this because of being invoked recursively.
						//@this.SetCandidateIsOn(peerCell, setValue, false);

						@this[peerCell] &= (Mask)~(1 << setValue);
					}
				}
			}
		}
	}


	/// <summary>
	/// Indicates the grid has already solved. If the value is <see langword="true"/>,
	/// the grid is solved; otherwise, <see langword="false"/>.
	/// </summary>
	public readonly bool IsSolved
	{
		get
		{
			for (var i = 0; i < 81; i++)
			{
				if (GetStatus(i) == CellStatus.Empty)
				{
					return false;
				}
			}

			for (var i = 0; i < 81; i++)
			{
				switch (GetStatus(i))
				{
					case CellStatus.Given or CellStatus.Modifiable:
					{
						var curDigit = GetDigit(i);
						foreach (var cell in Peers[i])
						{
							if (curDigit == GetDigit(cell))
							{
								return false;
							}
						}

						break;
					}
					case CellStatus.Empty:
					{
						continue;
					}
					default:
					{
						return false;
					}
				}
			}

			return true;
		}
	}

	/// <summary>
	/// Indicates whether the grid is <see cref="Undefined"/>, which means the grid
	/// holds totally same value with <see cref="Undefined"/>.
	/// </summary>
	/// <seealso cref="Undefined"/>
	public readonly bool IsUndefined
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => this == Undefined;
	}

	/// <summary>
	/// Indicates whether the grid is <see cref="Empty"/>, which means the grid
	/// holds totally same value with <see cref="Empty"/>.
	/// </summary>
	/// <seealso cref="Empty"/>
	public readonly bool IsEmpty
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => this == Empty;
	}

	/// <summary>
	/// Indicates whether the puzzle has a unique solution.
	/// </summary>
	public readonly bool IsValid => BackingSolver.CheckValidity(ToString());

	/// <summary>
	/// Determines whether the puzzle is a minimal puzzle, which means the puzzle will become multiple solution
	/// if arbitrary one given digit will be removed from the grid.
	/// </summary>
	public readonly bool IsMinimal => CheckMinimal(out _);

	/// <summary>
	/// Indicates the number of total candidates.
	/// </summary>
	public readonly int CandidatesCount
	{
		get
		{
			var count = 0;
			for (var i = 0; i < 81; i++)
			{
				if (GetStatus(i) == CellStatus.Empty)
				{
					count += PopCount((uint)GetCandidates(i));
				}
			}

			return count;
		}
	}

	/// <summary>
	/// Indicates the total number of given cells.
	/// </summary>
	public readonly int GivensCount
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => GivenCells.Count;
	}

	/// <summary>
	/// Indicates the total number of modifiable cells.
	/// </summary>
	public readonly int ModifiablesCount
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ModifiableCells.Count;
	}

	/// <summary>
	/// Indicates the total number of empty cells.
	/// </summary>
	public readonly int EmptiesCount
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => EmptyCells.Count;
	}

	/// <summary>
	/// <para>Indicates which houses are null houses.</para>
	/// <para>A <b>Null House</b> is a house whose hold cells are all empty cells.</para>
	/// <para>
	/// The property returns a <see cref="HouseMask"/> value as a mask that contains all possible house indices.
	/// For example, if the row 5, column 5 and block 5 (1-9) are null houses, the property will return
	/// the result <see cref="HouseMask"/> value, <c>000010000_000010000_000010000</c> as binary.
	/// </para>
	/// </summary>
	public readonly HouseMask NullHouses
	{
		get
		{
			var result = 0;
			for (var (houseIndex, valueCells) = (0, ~EmptyCells); houseIndex < 27; houseIndex++)
			{
				if (valueCells / houseIndex == 0)
				{
					result |= 1 << houseIndex;
				}
			}

			return result;
		}
	}

	/// <summary>
	/// Gets a cell list that only contains the given cells.
	/// </summary>
	public readonly CellMap GivenCells => GetMap(&CellFilteringMethods.GivenCells);

	/// <summary>
	/// Gets a cell list that only contains the modifiable cells.
	/// </summary>
	public readonly CellMap ModifiableCells => GetMap(&CellFilteringMethods.ModifiableCells);

	/// <summary>
	/// Indicates a cell list whose corresponding position in this grid is empty.
	/// </summary>
	public readonly CellMap EmptyCells => GetMap(&CellFilteringMethods.EmptyCells);

	/// <summary>
	/// Indicates a cell list whose corresponding position in this grid contain two candidates.
	/// </summary>
	public readonly CellMap BivalueCells => GetMap(&CellFilteringMethods.BivalueCells);

	/// <summary>
	/// Indicates the map of possible positions of the existence of the candidate value for each digit.
	/// The return value will be an array of 9 elements, which stands for the statuses of 9 digits.
	/// </summary>
	public readonly CellMap[] CandidatesMap => GetMaps(&CellFilteringMethods.CandidatesMap);

	/// <summary>
	/// <para>
	/// Indicates the map of possible positions of the existence of each digit. The return value will
	/// be an array of 9 elements, which stands for the statuses of 9 digits.
	/// </para>
	/// <para>
	/// Different with <see cref="CandidatesMap"/>, this property contains all givens, modifiables and
	/// empty cells only if it contains the digit in the mask.
	/// </para>
	/// </summary>
	/// <seealso cref="CandidatesMap"/>
	public readonly CellMap[] DigitsMap => GetMaps(&CellFilteringMethods.DigitsMap);

	/// <summary>
	/// <para>
	/// Indicates the map of possible positions of the existence of that value of each digit.
	/// The return value will be an array of 9 elements, which stands for the statuses of 9 digits.
	/// </para>
	/// <para>
	/// Different with <see cref="CandidatesMap"/>, the value only contains the given or modifiable
	/// cells whose mask contain the set bit of that digit.
	/// </para>
	/// </summary>
	/// <seealso cref="CandidatesMap"/>
	public readonly CellMap[] ValuesMap => GetMaps(&CellFilteringMethods.ValuesMap);

	/// <summary>
	/// Indicates all possible conjugate pairs appeared in this grid.
	/// </summary>
	public readonly Conjugate[] ConjugatePairs
	{
		get
		{
			var conjugatePairs = new List<Conjugate>();
			for (var (digit, candidatesMap) = (0, CandidatesMap); digit < 9; digit++)
			{
				scoped ref readonly var cellsMap = ref candidatesMap[digit];
				foreach (var houseMap in HousesMap)
				{
					if ((houseMap & cellsMap) is { Count: 2 } temp)
					{
						conjugatePairs.Add(new(temp, digit));
					}
				}
			}

			return conjugatePairs.ToArray();
		}
	}

	/// <summary>
	/// Gets the grid where all modifiable cells are empty cells (i.e. the initial one).
	/// </summary>
	public readonly Grid ResetGrid => Preserve(GivenCells);

	/// <summary>
	/// Indicates the solution of the current grid. If the puzzle has no solution or multiple solutions,
	/// this property will return <see cref="Undefined"/>.
	/// </summary>
	/// <seealso cref="Undefined"/>
	public readonly Grid SolutionGrid
	{
		get
		{
			return BackingSolver.Solve(this) is { IsUndefined: false } solution ? unfix(solution, GivenCells) : Undefined;

			static Grid unfix(scoped in Grid solution, scoped in CellMap pattern)
			{
				var result = solution;
				foreach (var cell in ~pattern)
				{
					if (result.GetStatus(cell) == CellStatus.Given)
					{
						result.SetStatus(cell, CellStatus.Modifiable);
					}
				}

				return result;
			}
		}
	}

	/// <inheritdoc/>
	readonly int IReadOnlyCollection<Digit>.Count => 81;

	/// <summary>
	/// Indicates the minimum possible grid value that the current type can reach.
	/// </summary>
	/// <remarks>
	/// This value is found out via backtracking algorithm. For more information, please visit type <see cref="BacktrackingSolver"/>.
	/// </remarks>
	/// <seealso cref="BacktrackingSolver"/>
	static Grid IMinMaxValue<Grid>.MinValue => (Grid)"123456789456789123789123456214365897365897214897214365531642978642978531978531642";

	/// <summary>
	/// Indicates the maximum possible grid value that the current type can reach.
	/// </summary>
	/// <remarks>
	/// This value is found out via backtracking algorithm. For more information, please visit type <see cref="BacktrackingSolver"/>.
	/// </remarks>
	/// <seealso cref="BacktrackingSolver"/>
	static Grid IMinMaxValue<Grid>.MaxValue => (Grid)"987654321654321987321987654896745213745213896213896745579468132468132579132579468";


	/// <inheritdoc cref="this[in CellMap]"/>
	public readonly Mask this[Cell[] cells]
	{
		get
		{
			var result = (Mask)0;
			foreach (var cell in cells)
			{
				result |= this[cell];
			}

			return (Mask)(result & MaxCandidatesMask);
		}
	}

	/// <summary>
	/// Creates a mask of type <see cref="Mask"/> that represents the usages of digits 1 to 9,
	/// ranged in a specified list of cells in the current sudoku grid.
	/// </summary>
	/// <param name="cells">The list of cells to gather the usages on all digits.</param>
	/// <returns>A mask of type <see cref="Mask"/> that represents the usages of digits 1 to 9.</returns>
	public readonly Mask this[scoped in CellMap cells]
	{
		get
		{
			var result = (Mask)0;
			foreach (var cell in cells)
			{
				result |= this[cell];
			}

			return (Mask)(result & MaxCandidatesMask);
		}
	}

	/// <summary>
	/// <inheritdoc cref="this[in CellMap]" path="/summary"/>
	/// </summary>
	/// <param name="cells"><inheritdoc cref="this[in CellMap]" path="/param[@name='cells']"/></param>
	/// <param name="withValueCells">
	/// Indicates whether the value cells (given or modifiable ones) will be included to be gathered.
	/// If <see langword="true"/>, all value cells (no matter what kind of cell) will be summed up.
	/// </param>
	/// <returns><inheritdoc cref="this[in CellMap]" path="/returns"/></returns>
	public readonly Mask this[scoped in CellMap cells, bool withValueCells]
	{
		get
		{
			var result = (Mask)0;
			foreach (var cell in cells)
			{
				if (!withValueCells && GetStatus(cell) != CellStatus.Empty || withValueCells)
				{
					result |= this[cell];
				}
			}

			return (Mask)(result & MaxCandidatesMask);
		}
	}

	/// <summary>
	/// <inheritdoc cref="this[in CellMap]" path="/summary"/>
	/// </summary>
	/// <param name="cells"><inheritdoc cref="this[in CellMap]" path="/param[@name='cells']"/></param>
	/// <param name="withValueCells">
	/// <inheritdoc cref="this[in CellMap, bool]" path="/param[@name='withValueCells']"/>
	/// </param>
	/// <param name="mergingMethod">
	/// </param>
	/// <returns><inheritdoc cref="this[in CellMap]" path="/returns"/></returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when <paramref name="mergingMethod"/> is not defined.</exception>
	public readonly Mask this[scoped in CellMap cells, bool withValueCells, GridMaskMergingMethod mergingMethod]
	{
		get
		{
			switch (mergingMethod)
			{
				case GridMaskMergingMethod.AndNot:
				{
					var result = MaxCandidatesMask;
					foreach (var cell in cells)
					{
						if (!withValueCells && GetStatus(cell) != CellStatus.Empty || withValueCells)
						{
							result &= (Mask)~this[cell];
						}
					}

					return result;
				}
				case GridMaskMergingMethod.Or:
				{
					var result = (Mask)0;
					foreach (var cell in cells)
					{
						if (!withValueCells && GetStatus(cell) != CellStatus.Empty || withValueCells)
						{
							result |= this[cell];
						}
					}

					return (Mask)(result & MaxCandidatesMask);
				}
				default:
				{
					throw new ArgumentOutOfRangeException(nameof(mergingMethod));
				}
			}
		}
	}


	/// <summary>
	/// Determine whether the specified <see cref="Grid"/> instance hold the same values as the current instance.
	/// </summary>
	/// <param name="other">The instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(scoped in Grid other)
	{
		return e(ref AsByteRef(ref AsRef(this[0])), ref AsByteRef(ref AsRef(other[0])), sizeof(Mask) * 81);

#pragma warning disable CS1587
		/// <summary>
		/// Determines whether two sequences are considered equal on respective bits.
		/// </summary>
		/// <param name="first">The first sequence.</param>
		/// <param name="second">The second sequence.</param>
		/// <param name="length">
		/// The total bits of the sequence to be compared. Please note that two sequences
		/// <paramref name="first"/> and <paramref name="second"/> must hold a same length.
		/// </param>
		/// <returns>A <see cref="bool"/> result indicating whether they are considered equal.</returns>
		/// <remarks>
		/// Optimized byte-based <c>SequenceEquals</c>.
		/// The <paramref name="length"/> parameter for this one is declared a <see langword="nuint"/> rather than <see cref="int"/>
		/// as we also use it for types other than <see cref="byte"/> where the length can exceed 2Gb once scaled by <see langword="sizeof"/>(T).
		/// </remarks>
		/// <!--
		/// Licensed to the .NET Foundation under one or more agreements.
		/// The .NET Foundation licenses this file to you under the MIT license.
		/// https://source.dot.net/#System.Private.CoreLib/src/libraries/System.Private.CoreLib/src/System/SpanHelpers.Byte.cs,998a36a55f580ab1
		/// -->
#pragma warning restore CS1587
		static bool e(scoped ref byte first, scoped ref byte second, nuint length)
		{
			bool result;

			// Use nint for arithmetic to avoid unnecessary 64->32->64 truncations.
			if (length >= (nuint)sizeof(nuint))
			{
				// Conditional jmp forward to favor shorter lengths. (See comment at "Equal:" label)
				// The longer lengths can make back the time due to branch misprediction better than shorter lengths.
				goto Longer;
			}

#if TARGET_64BIT
			// On 32-bit, this will always be true since sizeof(nuint) == 4.
			if (length < sizeof(uint))
#endif
			{
				var differentBits = 0U;
				var offset = length & 2;
				if (offset != 0)
				{
					differentBits = load_ushort(ref first);
					differentBits -= load_ushort(ref second);
				}
				if ((length & 1) != 0)
				{
					differentBits |= (uint)AddByteOffset(ref first, offset) - AddByteOffset(ref second, offset);
				}

				result = differentBits == 0;

				goto Result;
			}
#if TARGET_64BIT
			else
			{
				var offset = length - sizeof(uint);
				var differentBits = load_uint(ref first) - load_uint(ref second);
				differentBits |= load_uint2(ref first, offset) - load_uint2(ref second, offset);
				result = differentBits == 0;

				goto Result;
			}
#endif
		Longer:
			// Only check that the ref is the same if buffers are large, and hence its worth avoiding doing unnecessary comparisons.
			if (!AreSame(ref first, ref second))
			{
				// C# compiler inverts this test, making the outer goto the conditional jmp.
				goto Vector;
			}

			// This becomes a conditional jmp forward to not favor it.
			goto Equal;

		Result:
			return result;

		Equal:
			// When the sequence is equal; which is the longest execution, we want it to determine that
			// as fast as possible so we do not want the early outs to be "predicted not taken" branches.
			return true;

		Vector:
			if (Vector128.IsHardwareAccelerated)
			{
				if (Vector256.IsHardwareAccelerated && length >= (nuint)Vector256<byte>.Count)
				{
					var offset = (nuint)0;
					var lengthToExamine = length - (nuint)Vector256<byte>.Count;

					// Unsigned, so it shouldn't have overflowed larger than length (rather than negative).
					Debug.Assert(lengthToExamine < length);
					if (lengthToExamine != 0)
					{
						do
						{
							if (Vector256.LoadUnsafe(ref first, offset) != Vector256.LoadUnsafe(ref second, offset))
							{
								goto NotEqual;
							}

							offset += (nuint)Vector256<byte>.Count;
						} while (lengthToExamine > offset);
					}

					// Do final compare as Vector256<byte>.Count from end rather than start.
					if (Vector256.LoadUnsafe(ref first, lengthToExamine) == Vector256.LoadUnsafe(ref second, lengthToExamine))
					{
						// C# compiler inverts this test, making the outer goto the conditional jmp.
						goto Equal;
					}

					// This becomes a conditional jmp forward to not favor it.
					goto NotEqual;
				}

				if (length >= (nuint)Vector128<byte>.Count)
				{
					var offset = (nuint)0;
					var lengthToExamine = length - (nuint)Vector128<byte>.Count;

					// Unsigned, so it shouldn't have overflowed larger than length (rather than negative).
					Debug.Assert(lengthToExamine < length);
					if (lengthToExamine != 0)
					{
						do
						{
							if (Vector128.LoadUnsafe(ref first, offset) != Vector128.LoadUnsafe(ref second, offset))
							{
								goto NotEqual;
							}

							offset += (nuint)Vector128<byte>.Count;
						} while (lengthToExamine > offset);
					}

					// Do final compare as Vector128<byte>.Count from end rather than start.
					if (Vector128.LoadUnsafe(ref first, lengthToExamine) == Vector128.LoadUnsafe(ref second, lengthToExamine))
					{
						// C# compiler inverts this test, making the outer goto the conditional jmp.
						goto Equal;
					}

					// This becomes a conditional jmp forward to not favor it.
					goto NotEqual;
				}
			}
			else if (Vector.IsHardwareAccelerated && length >= (nuint)Vector<byte>.Count)
			{
				var offset = (nuint)0;
				var lengthToExamine = length - (nuint)Vector<byte>.Count;

				// Unsigned, so it shouldn't have overflowed larger than length (rather than negative).
				Debug.Assert(lengthToExamine < length);
				if (lengthToExamine > 0)
				{
					do
					{
						if (load_Vector(ref first, offset) != load_Vector(ref second, offset))
						{
							goto NotEqual;
						}

						offset += (nuint)Vector<byte>.Count;
					} while (lengthToExamine > offset);
				}

				// Do final compare as Vector<byte>.Count from end rather than start.
				if (load_Vector(ref first, lengthToExamine) == load_Vector(ref second, lengthToExamine))
				{
					// C# compiler inverts this test, making the outer goto the conditional jmp.
					goto Equal;
				}

				// This becomes a conditional jmp forward to not favor it.
				goto NotEqual;
			}

#if TARGET_64BIT
			if (Vector128.IsHardwareAccelerated)
			{
				Debug.Assert(length <= (nuint)sizeof(nuint) * 2);

				var offset = length - (nuint)sizeof(nuint);
				var differentBits = load_nuint(ref first) - load_nuint(ref second);
				differentBits |= load_uint2(ref first, offset) - load_uint2(ref second, offset);
				result = differentBits == 0;
				goto Result;
			}
			else
#endif
			{
				Debug.Assert(length >= (nuint)sizeof(nuint));

				var offset = (nuint)0;
				var lengthToExamine = length - (nuint)sizeof(nuint);
				// Unsigned, so it shouldn't have overflowed larger than length (rather than negative).
				Debug.Assert(lengthToExamine < length);
				if (lengthToExamine > 0)
				{
					do
					{
						// Compare unsigned so not do a sign extend mov on 64 bit.
						if (load_nuint2(ref first, offset) != load_nuint2(ref second, offset))
						{
							goto NotEqual;
						}
						offset += (nuint)sizeof(nuint);
					} while (lengthToExamine > offset);
				}

				// Do final compare as sizeof(nuint) from end rather than start.
				result = load_nuint2(ref first, lengthToExamine) == load_nuint2(ref second, lengthToExamine);
				goto Result;
			}

		NotEqual:
			// As there are so many true/false exit points the JIT will coalesce them to one location.
			// We want them at the end so the conditional early exit jmps are all jmp forwards
			// so the branch predictor in a uninitialized state will not take them e.g.
			// - loops are conditional jmps backwards and predicted.
			// - exceptions are conditional forwards jmps and not predicted.
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static ushort load_ushort(scoped ref byte start) => ReadUnaligned<ushort>(ref start);

#if TARGET_64BIT
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static uint load_uint(scoped ref byte start) => ReadUnaligned<uint>(ref start);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static uint load_uint2(scoped ref byte start, nuint offset) => ReadUnaligned<uint>(ref AddByteOffset(ref start, offset));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static nuint load_nuint(scoped ref byte start) => ReadUnaligned<nuint>(ref start);
#endif

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static nuint load_nuint2(scoped ref byte start, nuint offset) => ReadUnaligned<nuint>(ref AddByteOffset(ref start, offset));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static Vector<byte> load_Vector(scoped ref byte start, nuint offset) => ReadUnaligned<Vector<byte>>(ref AddByteOffset(ref start, offset));
	}

	/// <summary>
	/// Determine whether the digit in the target cell may be duplicated with a certain cell in the peers of the current cell,
	/// if the digit is filled into the cell.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public readonly bool DuplicateWith(Cell cell, Digit digit)
	{
		foreach (var tempCell in Peers[cell])
		{
			if (GetDigit(tempCell) == digit)
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// <para>
	/// Determines whether the current grid is valid, checking on both normal and sukaku cases
	/// and returning a <see cref="bool"/>? value indicating whether the current sudoku grid is valid
	/// only on sukaku case.
	/// </para>
	/// <para>
	/// For more information, please see the introduction about the parameter
	/// <paramref name="sukaku"/>.
	/// </para>
	/// </summary>
	/// <param name="solutionIfValid">
	/// The solution if the puzzle is valid; otherwise, <see cref="Undefined"/>.
	/// </param>
	/// <param name="sukaku">Indicates whether the current mode is sukaku mode.<list type="table">
	/// <item>
	/// <term><see langword="true"/></term>
	/// <description>The puzzle is a sukaku puzzle.</description>
	/// </item>
	/// <item>
	/// <term><see langword="false"/></term>
	/// <description>The puzzle is a normal sudoku puzzle.</description>
	/// </item>
	/// <item>
	/// <term><see langword="null"/></term>
	/// <description>The puzzle is invalid.</description>
	/// </item>
	/// </list>
	/// </param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	/// <seealso cref="Undefined"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool ExactlyValidate(out Grid solutionIfValid, [NotNullWhen(true)] out bool? sukaku)
	{
		SkipInit(out solutionIfValid);
		if (BackingSolver.CheckValidity(ToString(), out var solution))
		{
			solutionIfValid = Parse(solution);
			sukaku = false;
			return true;
		}

		if (BackingSolver.CheckValidity(ToString("~"), out solution))
		{
			solutionIfValid = Parse(solution);
			sukaku = true;
			return true;
		}
		sukaku = null;
		return false;
	}

	/// <summary>
	/// Determines whether the puzzle is a minimal puzzle, which means the puzzle will become multiple solution
	/// if arbitrary one given digit will be removed from the grid.
	/// </summary>
	/// <param name="firstCandidateMakePuzzleNotMinimal">
	/// <para>
	/// Indicates the first found candidate that can make the puzzle not minimal, which means
	/// if we remove the digit in the cell, the puzzle will still keep unique.
	/// </para>
	/// <para>If the return value is <see langword="true"/>, this argument will be -1.</para>
	/// </param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	/// <exception cref="InvalidOperationException">Throws when the puzzle is invalid (i.e. not unique).</exception>
	public readonly bool CheckMinimal(out Candidate firstCandidateMakePuzzleNotMinimal)
	{
		switch (this)
		{
			case { IsValid: false }:
			{
				throw new InvalidOperationException("The puzzle is not unique.");
			}
			case { IsSolved: true, GivenCells.Count: 81 }:
			{
				// Very special case: all cells are givens.
				// The puzzle is considered not a minimal puzzle, because any digit in the grid can be removed.
				firstCandidateMakePuzzleNotMinimal = GetDigit(0);
				return false;
			}
			default:
			{
				var gridCopied = this;
				gridCopied.Unfix();

				foreach (var cell in gridCopied.ModifiableCells)
				{
					var newGrid = gridCopied;
					newGrid.SetDigit(cell, -1);
					newGrid.Fix();

					if (newGrid.IsValid)
					{
						firstCandidateMakePuzzleNotMinimal = cell * 9 + GetDigit(cell);
						return false;
					}
				}

				firstCandidateMakePuzzleNotMinimal = -1;
				return true;
			}
		}
	}

	/// <summary>
	/// Sets a candidate existence case with a <see cref="bool"/> value.
	/// </summary>
	/// <param name="cell"><inheritdoc cref="SetCandidateIsOn(int, int, bool)" path="/param[@name='cell']"/></param>
	/// <param name="digit"><inheritdoc cref="SetCandidateIsOn(int, int, bool)" path="/param[@name='digit']"/></param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool GetCandidateIsOn(Cell cell, Digit digit) => (this[cell] >> digit & 1) != 0;

	/// <summary>
	/// Indicates whether the current grid contains the specified candidate offset.
	/// </summary>
	/// <param name="candidate">The candidate offset.</param>
	/// <returns><inheritdoc cref="Exists(Cell, Digit)" path="/returns"/></returns>
	/// <remarks><inheritdoc cref="Exists(Cell, Digit)" path="/remarks"/></remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool? Exists(Candidate candidate) => Exists(candidate / 9, candidate % 9);

	/// <summary>
	/// Indicates whether the current grid contains the digit in the specified cell.
	/// </summary>
	/// <param name="cell">The cell offset.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>
	/// The method will return a <see cref="bool"/>? value
	/// (containing three possible cases: <see langword="true"/>, <see langword="false"/> and <see langword="null"/>).
	/// All values corresponding to the cases are below:
	/// <list type="table">
	/// <listheader>
	/// <term>Value</term>
	/// <description>Case description on this value</description>
	/// </listheader>
	/// <item>
	/// <term><see langword="true"/></term>
	/// <description>
	/// The cell is an empty cell <b>and</b> contains the specified digit.
	/// </description>
	/// </item>
	/// <item>
	/// <term><see langword="false"/></term>
	/// <description>
	/// The cell is an empty cell <b>but doesn't</b> contain the specified digit.
	/// </description>
	/// </item>
	/// <item>
	/// <term><see langword="null"/></term>
	/// <description>The cell is <b>not</b> an empty cell.</description>
	/// </item>
	/// </list>
	/// </returns>
	/// <remarks>
	/// <para>
	/// Note that the method will return a <see cref="bool"/>?, so you should use the code
	/// '<c>grid.Exists(cell, digit) is true</c>' or '<c>grid.Exists(cell, digit) == true</c>'
	/// to decide whether a condition is true.
	/// </para>
	/// <para>
	/// In addition, because the type is <see cref="bool"/>? rather than <see cref="bool"/>,
	/// the result case will be more precisely than the indexer <see cref="GetCandidateIsOn(Cell, Digit)"/>,
	/// which is the main difference between this method and that indexer.
	/// </para>
	/// </remarks>
	/// <seealso cref="GetCandidateIsOn(Cell, Digit)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool? Exists(Cell cell, Digit digit) => GetStatus(cell) == CellStatus.Empty ? GetCandidateIsOn(cell, digit) : null;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly int GetHashCode()
		=> this switch { { IsUndefined: true } => 0, { IsEmpty: true } => 1, _ => ToString("#").GetHashCode() };

	/// <summary>
	/// Serializes this instance to an array, where all digit value will be stored.
	/// </summary>
	/// <returns>
	/// This array. All elements are between 0 and 9, where 0 means the cell is <see cref="CellStatus.Empty"/> now.
	/// </returns>
	public readonly Digit[] ToArray()
	{
		var result = new Digit[81];
		for (var i = 0; i < 81; i++)
		{
			// -1..8 -> 0..9
			result[i] = GetDigit(i) + 1;
		}

		return result;
	}

	/// <summary>
	/// Get the candidate mask part of the specified cell.
	/// </summary>
	/// <param name="cell">The cell offset you want to get.</param>
	/// <returns>
	/// <para>
	/// The candidate mask. The return value is a 9-bit <see cref="Mask"/>
	/// value, where each bit will be:
	/// <list type="table">
	/// <item>
	/// <term><c>0</c></term>
	/// <description>The cell <b>doesn't contain</b> the possibility of the digit.</description>
	/// </item>
	/// <item>
	/// <term><c>1</c></term>
	/// <description>The cell <b>contains</b> the possibility of the digit.</description>
	/// </item>
	/// </list>
	/// </para>
	/// <para>
	/// For example, if the result mask is 266 (i.e. <c>0b<b>1</b>00_00<b>1</b>_0<b>1</b>0</c> in binary),
	/// the value will indicate the cell contains the digit 2, 4 and 9.
	/// </para>
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Mask GetCandidates(Cell cell) => (Mask)(this[cell] & MaxCandidatesMask);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(string? format)
		=> this switch
		{
			{ IsEmpty: true } => $"<{nameof(Empty)}>",
			{ IsUndefined: true } => $"<{nameof(Undefined)}>",
			_ => GridFormatterFactory.GetBuiltInFormatter(format)?.ToString(this) ?? throw new FormatException("The specified format is invalid.")
		};

	/// <summary>
	/// Gets <see cref="string"/> representation of the current grid, using pre-defined grid formatters.
	/// </summary>
	/// <param name="gridFormatter">
	/// The grid formatter instance to format the current grid.
	/// </param>
	/// <returns>The <see cref="string"/> result.</returns>
	/// <remarks>
	/// <para>
	/// The target and supported types are stored in namespace <see cref="Text.Formatting"/>.
	/// If you don't remember the full format strings, you can try this method instead by passing
	/// actual <see cref="IGridFormatter"/> instances.
	/// </para>
	/// <para>
	/// For example, by using Susser formatter <see cref="SusserFormat"/> instances:
	/// <code><![CDATA[
	/// // Suppose the variable is of type 'Grid'.
	/// var grid = ...;
	/// 
	/// // Creates a Susser-based formatter, with placeholder text as '0',
	/// // missing candidates output and modifiable distinction.
	/// var formatter = SusserFormat.Default with
	/// {
	///     Placeholder = '0',
	///     WithCandidates = true,
	///     WithModifiables = true
	/// };
	/// 
	/// // Using this method to get the target string representation.
	/// string targetStr = grid.ToString(formatter);
	/// 
	/// // Output the result.
	/// Console.WriteLine(targetStr);
	/// ]]></code>
	/// </para>
	/// <para>
	/// In some cases we suggest you use this method instead of calling <see cref="ToString(string?)"/>
	/// and <see cref="ToString(string?, IFormatProvider?)"/> because you may not remember all possible string formats.
	/// </para>
	/// <para>
	/// In addition, the method <see cref="ToString(string?, IFormatProvider?)"/> is also compatible with this method.
	/// If you forget to call this one, you can also use that method to get the same target result by passing first argument
	/// named <c>format</c> with <see langword="null"/> value:
	/// <code><![CDATA[
	/// string targetStr = grid.ToString(null, formatter);
	/// ]]></code>
	/// </para>
	/// </remarks>
	/// <seealso cref="Text.Formatting"/>
	/// <seealso cref="IGridFormatter"/>
	/// <seealso cref="SusserFormat"/>
	/// <seealso cref="ToString(string?)"/>
	/// <seealso cref="ToString(string?, IFormatProvider?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(IGridFormatter gridFormatter)
		=> this switch
		{
			{ IsUndefined: true } => $"<{nameof(Undefined)}>",
			{ IsEmpty: true } => $"<{nameof(Empty)}>",
			_ => gridFormatter.ToString(this)
		};

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(string? format, IFormatProvider? formatProvider)
		=> (this, format, formatProvider) switch
		{
			({ IsUndefined: true }, _, _) => $"<{nameof(Undefined)}>",
			({ IsEmpty: true }, _, _) => $"<{nameof(Empty)}>",
			(_, null, null) => ToString(SusserFormat.Default),
			(_, not null, _) => ToString(format),
			(_, _, IGridFormatter formatter) => formatter.ToString(this),
			(_, _, ICustomFormatter formatter) => formatter.Format(format, this, formatProvider),
			(_, _, CultureInfo { Name: "zh-CN" }) => ToString(SusserFormat.Full),
			(_, _, CultureInfo { Name: ['e', 'n', '-', >= 'A' and <= 'Z', >= 'A' and <= 'Z'] }) => ToString(MultipleLineFormat.Default),
			_ => ToString(SusserFormat.Default)
		};

	/// <summary>
	/// Get the cell status at the specified cell.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <returns>The cell status.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly CellStatus GetStatus(Cell cell) => MaskToStatus(this[cell]);

	/// <summary>
	/// Try to get the digit filled in the specified cell.
	/// </summary>
	/// <param name="cell">The cell used.</param>
	/// <returns>The digit that the current cell filled. If the cell is empty, return -1.</returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when the specified cell keeps a wrong cell status value. For example, <see cref="CellStatus.Undefined"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Digit GetDigit(Cell cell)
		=> GetStatus(cell) switch
		{
			CellStatus.Empty => -1,
			CellStatus.Modifiable or CellStatus.Given => TrailingZeroCount(this[cell]),
			_ => throw new InvalidOperationException("The grid cannot keep invalid cell status value.")
		};

	/// <summary>
	/// Gets the enumerator of the current instance in order to use <see langword="foreach"/> loop.
	/// </summary>
	/// <returns>The enumerator instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly CandidateEnumerator EnumerateCandidates() => new(ref AsRef(this[0]));

	/// <summary>
	/// Projects each element of a sequence into a new form.
	/// </summary>
	/// <typeparam name="TResult">
	/// The type of the value returned by <paramref name="selector"/>.
	/// This type must be an <see langword="unmanaged"/> type in order to make optimization
	/// in the future release of C# versions.
	/// </typeparam>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <returns>
	/// An array of <typeparamref name="TResult"/> elements converted.
	/// </returns>
	public readonly TResult[] Select<TResult>(Func<Candidate, TResult> selector)
	{
		var (result, i) = (new TResult[81], 0);
		foreach (var candidate in EnumerateCandidates())
		{
			result[i++] = selector(candidate);
		}

		return result;
	}

	/// <summary>
	/// Reset the sudoku grid, to set all modifiable values to empty ones.
	/// </summary>
	public void Reset()
	{
		for (var i = 0; i < 81; i++)
		{
			if (GetStatus(i) == CellStatus.Modifiable)
			{
				SetDigit(i, -1); // Reset the cell, and then re-compute all candidates.
			}
		}
	}

	/// <summary>
	/// To fix the current grid (all modifiable values will be changed to given ones).
	/// </summary>
	public void Fix()
	{
		for (var i = 0; i < 81; i++)
		{
			if (GetStatus(i) == CellStatus.Modifiable)
			{
				SetStatus(i, CellStatus.Given);
			}
		}
	}

	/// <summary>
	/// To unfix the current grid (all given values will be changed to modifiable ones).
	/// </summary>
	public void Unfix()
	{
		for (var i = 0; i < 81; i++)
		{
			if (GetStatus(i) == CellStatus.Given)
			{
				SetStatus(i, CellStatus.Modifiable);
			}
		}
	}

	/// <summary>
	/// Try to apply the specified conclusion.
	/// </summary>
	/// <param name="conclusion">The conclusion to be applied.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Apply(Conclusion conclusion)
	{
		_ = conclusion is { Cell: var cell, Digit: var digit, ConclusionType: var type };
		switch (type)
		{
			case Assignment:
			{
				SetDigit(cell, digit);
				break;
			}
			case Elimination:
			{
				SetCandidateIsOn(cell, digit, false);
				break;
			}
		}
	}

	/// <summary>
	/// Try to apply the specified array of conclusions.
	/// </summary>
	/// <param name="conclusions">The conclusions to be applied.</param>
	public void Apply(Conclusion[] conclusions)
	{
		foreach (var conclusion in conclusions)
		{
			Apply(conclusion);
		}
	}

	/// <summary>
	/// Set the specified cell to the specified status.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="status">The status.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetStatus(Cell cell, CellStatus status)
	{
		scoped ref var mask = ref this[cell];
		var copied = mask;
		mask = (Mask)((int)status << 9 | mask & MaxCandidatesMask);

		ValueChanged(ref this, cell, copied, mask, -1);
	}

	/// <summary>
	/// Set the specified cell to the specified mask.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="mask">The mask to set.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetMask(Cell cell, Mask mask)
	{
		scoped ref var m = ref this[cell];
		var copied = m;
		m = mask;

		ValueChanged(ref this, cell, copied, m, -1);
	}

	/// <summary>
	/// Set the specified digit into the specified cell.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">
	/// <para>
	/// The value you want to set. The value should be between 0 and 8.
	/// If assigning -1, the grid will execute an implicit behavior that candidates in <b>all</b> empty cells will be re-computed.
	/// </para>
	/// <para>
	/// The values set into the grid will be regarded as the modifiable values.
	/// If the cell contains a digit, it will be covered when it is a modifiable value.
	/// If the cell is a given cell, the setter will do nothing.
	/// </para>
	/// </param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetDigit(Cell cell, Digit digit)
	{
		switch (digit)
		{
			case -1 when GetStatus(cell) == CellStatus.Modifiable:
			{
				// If 'value' is -1, we should reset the grid.
				// Note that reset candidates may not trigger the event.
				this[cell] = DefaultMask;

				RefreshingCandidates(ref this);

				break;
			}
			case >= 0 and < 9:
			{
				scoped ref var result = ref this[cell];
				var copied = result;

				// Set cell status to 'CellStatus.Modifiable'.
				result = (Mask)(ModifiableMask | 1 << digit);

				// To trigger the event, which is used for eliminate all same candidates in peer cells.
				ValueChanged(ref this, cell, copied, result, digit);

				break;
			}
		}
	}

	/// <summary>
	/// Sets the target candidate status.
	/// </summary>
	/// <param name="cell">The cell offset between 0 and 80.</param>
	/// <param name="digit">The digit between 0 and 8.</param>
	/// <param name="isOn">
	/// The case you want to set. <see langword="false"/> means that this candidate
	/// doesn't exist in this current sudoku grid; otherwise, <see langword="true"/>.
	/// </param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetCandidateIsOn(Cell cell, Digit digit, bool isOn)
	{
		if (cell is >= 0 and < 81 && digit is >= 0 and < 9)
		{
			var copied = this[cell];
			if (isOn)
			{
				this[cell] |= (Mask)(1 << digit);
			}
			else
			{
				this[cell] &= (Mask)~(1 << digit);
			}

			// To trigger the event.
			ValueChanged(ref this, cell, copied, this[cell], -1);
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly IEnumerator IEnumerable.GetEnumerator() => ToArray().GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly IEnumerator<Digit> IEnumerable<Digit>.GetEnumerator() => ((IEnumerable<Digit>)ToArray()).GetEnumerator();

	/// <summary>
	/// Called by properties <see cref="EmptyCells"/> and <see cref="BivalueCells"/>.
	/// </summary>
	/// <param name="predicate">The predicate.</param>
	/// <returns>The map.</returns>
	/// <seealso cref="EmptyCells"/>
	/// <seealso cref="BivalueCells"/>
	private readonly CellMap GetMap(GridCellFilter predicate)
	{
		var result = CellMap.Empty;
		for (var cell = 0; cell < 81; cell++)
		{
			if (predicate(this, cell))
			{
				result.Add(cell);
			}
		}

		return result;
	}

	/// <summary>
	/// Called by properties <see cref="CandidatesMap"/>, <see cref="DigitsMap"/> and <see cref="ValuesMap"/>.
	/// </summary>
	/// <param name="predicate">The predicate.</param>
	/// <returns>The map indexed by each digit.</returns>
	/// <seealso cref="CandidatesMap"/>
	/// <seealso cref="DigitsMap"/>
	/// <seealso cref="ValuesMap"/>
	private readonly CellMap[] GetMaps(GridCellDigitFilter predicate)
	{
		var result = new CellMap[9];
		for (var digit = 0; digit < 9; digit++)
		{
			scoped ref var map = ref result[digit];
			for (var cell = 0; cell < 81; cell++)
			{
				if (predicate(this, cell, digit))
				{
					map.Add(cell);
				}
			}
		}

		return result;
	}

	/// <summary>
	/// Gets a sudoku grid, removing all value digits not appearing in the specified <paramref name="pattern"/>.
	/// </summary>
	/// <param name="pattern">The pattern.</param>
	/// <returns>The result grid.</returns>
	private readonly Grid Preserve(scoped in CellMap pattern)
	{
		var result = this;
		foreach (var cell in ~pattern)
		{
			result.SetDigit(cell, -1);
		}

		return result;
	}


	/// <summary>
	/// Creates a <see cref="Grid"/> instance using grid values.
	/// </summary>
	/// <param name="gridValues">The array of grid values.</param>
	/// <param name="creatingOption">The grid creating option.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Create(Digit[] gridValues, GridCreatingOption creatingOption = 0) => new(gridValues[0], creatingOption);

	/// <summary>
	/// Creates a <see cref="Grid"/> instance with the specified mask array.
	/// </summary>
	/// <param name="masks">The masks.</param>
	/// <exception cref="ArgumentException">Throws when <see cref="Array.Length"/> is not 81.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Create(Mask[] masks) => checked((Grid)masks);

	/// <summary>
	/// Creates a <see cref="Grid"/> instance via the array of cell digits
	/// of type <see cref="ReadOnlySpan{T}"/>.
	/// </summary>
	/// <param name="gridValues">The list of cell digits.</param>
	/// <param name="creatingOption">The grid creating option.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Create(scoped ReadOnlySpan<Digit> gridValues, GridCreatingOption creatingOption = 0) => new(gridValues[0], creatingOption);

	/// <inheritdoc/>
	/// <remarks>
	/// We suggest you use <see cref="op_Explicit(string)"/> to achieve same goal if the passing argument is a constant.
	/// For example:
	/// <code><![CDATA[
	/// var grid1 = (Grid)"123456789456789123789123456214365897365897214897214365531642978642978531978531642";
	/// var grid2 = (Grid)"987654321654321987321987654896745213745213896213896745579468132468132579132579468";
	/// var grid3 = Grid.Parse(stringCode); // 'stringCode' is a string, not null.
	/// ]]></code>
	/// </remarks>
	/// <seealso cref="op_Explicit(string)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Parse(string str) => new GridParser(str).Parse();

	/// <summary>
	/// <para>
	/// Parses a string value and converts to this type.
	/// </para>
	/// <para>
	/// If you want to parse a PM grid, you should decide the mode to parse.
	/// If you use compatible mode to parse, all single values will be treated as
	/// given values; otherwise, recommended mode, which uses '<c><![CDATA[<d>]]></c>'
	/// or '<c>*d*</c>' to represent a value be a given or modifiable one. The decision
	/// will be indicated and passed by the second parameter <paramref name="compatibleFirst"/>.
	/// </para>
	/// </summary>
	/// <param name="str">The string.</param>
	/// <param name="compatibleFirst">
	/// Indicates whether the parsing operation should use compatible mode to check PM grid.
	/// </param>
	/// <returns>The result instance had converted.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Parse(string str, bool compatibleFirst) => new GridParser(str, compatibleFirst).Parse();

	/// <summary>
	/// Parses a string value and converts to this type, using a specified grid parsing type.
	/// </summary>
	/// <param name="str">The string.</param>
	/// <param name="gridParsingOption">The grid parsing type.</param>
	/// <returns>The result instance had converted.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Parse(string str, GridParsingOption gridParsingOption) => new GridParser(str).Parse(gridParsingOption);

	/// <summary>
	/// <para>Parses a string value and converts to this type.</para>
	/// <para>
	/// If you want to parse a PM grid, we recommend you use the method
	/// <see cref="Parse(string, GridParsingOption)"/> instead of this method.
	/// </para>
	/// </summary>
	/// <param name="str">The string.</param>
	/// <returns>The result instance had converted.</returns>
	/// <seealso cref="Parse(string, GridParsingOption)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Parse(scoped ReadOnlySpan<char> str) => new GridParser(str.ToString()).Parse();

	/// <inheritdoc/>
	public static bool TryParse(string str, out Grid result)
	{
		try
		{
			result = Parse(str);
			return !result.IsUndefined;
		}
		catch (FormatException)
		{
			result = Undefined;
			return false;
		}
	}

	/// <summary>
	/// Try to parse a string and converts to this type, and returns a
	/// <see cref="bool"/> value indicating the result of the conversion.
	/// </summary>
	/// <param name="str">The string.</param>
	/// <param name="option">The grid parsing type.</param>
	/// <param name="result">
	/// The result parsed. If the conversion is failed, this argument will be <see cref="Undefined"/>.
	/// </param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	/// <seealso cref="Undefined"/>
	public static bool TryParse(string str, GridParsingOption option, out Grid result)
	{
		try
		{
			result = Parse(str, option);
			return true;
		}
		catch (FormatException)
		{
			result = Undefined;
			return false;
		}
	}

	/// <inheritdoc cref="TryParse(string, out Grid)"/>
	public static bool TryParse(Utf8String str, out Grid result)
	{
		try
		{
			result = Parse(str);
			return !result.IsUndefined;
		}
		catch (FormatException)
		{
			result = Undefined;
			return false;
		}
	}

	/// <inheritdoc cref="TryParse(string, GridParsingOption, out Grid)"/>
	public static bool TryParse(Utf8String str, GridParsingOption option, out Grid result)
	{
		try
		{
			result = Parse(str, option);
			return true;
		}
		catch (FormatException)
		{
			result = Undefined;
			return false;
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Grid IParsable<Grid>.Parse(string s, IFormatProvider? provider) => Parse(s);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IParsable<Grid>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Grid result)
	{
		result = Undefined;
		return s is not null && TryParse(s, out result);
	}


	/// <summary>
	/// Converts the specified array elements into the target <see cref="Grid"/> instance, without any value boundary checking.
	/// </summary>
	/// <param name="maskArray">An array of the target mask. The array must be of length 81.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator Grid(Mask[] maskArray)
	{
		var result = Empty;
		CopyBlock(ref AsByteRef(ref result[0]), ref AsByteRef(ref maskArray[0]), (uint)(sizeof(Mask) * maskArray.Length));

		return result;
	}

	/// <summary>
	/// Converts the specified array elements into the target <see cref="Grid"/> instance, with value boundary checking.
	/// </summary>
	/// <param name="maskArray">
	/// <inheritdoc cref="op_Explicit(Mask[])" path="/param[@name='maskArray']"/>
	/// </param>
	/// <exception cref="ArgumentException">
	/// Throws when at least one element in the mask array is greater than 0b100__111_111_111 (i.e. 2559) or less than 0.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator checked Grid(Mask[] maskArray)
	{
		static bool maskMatcher(Mask element) => element >> 9 is 0 or 1 or 2 or 4;
		Argument.ThrowIfNotEqual(maskArray.Length, 81, nameof(maskArray));
		Argument.ThrowIfFalse(Array.TrueForAll(maskArray, maskMatcher), "Each element in this array must contain a valid cell status.");

		var result = Empty;
		CopyBlock(ref AsByteRef(ref result[0]), ref AsByteRef(ref maskArray[0]), sizeof(Mask) * 81);

		return result;
	}

	/// <summary>
	/// Implicit cast from <see cref="string"/> code to its equivalent <see cref="Grid"/> instance representation.
	/// </summary>
	/// <param name="gridCode">The grid code.</param>
	/// <remarks>
	/// <para>
	/// This explicit operator has same meaning for method <see cref="Parse(string)"/>. You can also use
	/// <see cref="Parse(string)"/> to get the same result as this operator.
	/// </para>
	/// <para>
	/// If the argument being passed is <see langword="null"/>, this operator will return <see cref="Undefined"/>
	/// as the final result, whose behavior is the only one that is different with method <see cref="Parse(string)"/>.
	/// That method will throw a <see cref="FormatException"/> instance to report the invalid argument being passed.
	/// </para>
	/// </remarks>
	/// <exception cref="FormatException">
	/// See exception thrown cases for method <see cref="ISimpleParsable{TSimpleParseable}.Parse(string)"/>.
	/// </exception>
	/// <seealso cref="Undefined"/>
	/// <seealso cref="Parse(string)"/>
	/// <seealso cref="ISimpleParsable{TSimpleParseable}.Parse(string)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator Grid([ConstantExpected] string? gridCode) => gridCode is null ? Undefined : Parse(gridCode);
}

/// <summary>
/// Indicates the JSON converter of the current type.
/// </summary>
file sealed class Converter : JsonConverter<Grid>
{
	/// <inheritdoc/>
	public override bool HandleNull => true;


	/// <inheritdoc/>
	/// <exception cref="InvalidOperationException">Throws when the target text is <see langword="null"/>.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override Grid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> Grid.Parse(reader.GetString() ?? throw new InvalidOperationException("The target value text cannot be null."));

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override void Write(Utf8JsonWriter writer, Grid value, JsonSerializerOptions options)
		=> writer.WriteStringValue(value.ToString(SusserFormat.Full));
}

/// <summary>
/// Represents a list of methods to filter the cells, used by <see cref="Grid.GetMap(GridCellFilter)"/>
/// or <see cref="Grid.GetMaps(GridCellDigitFilter)"/>.
/// </summary>
/// <seealso cref="Grid.GetMap(GridCellFilter)"/>
/// <seealso cref="Grid.GetMaps(GridCellDigitFilter)"/>
file static class CellFilteringMethods
{
	public static bool GivenCells(scoped in Grid g, Cell cell) => g.GetStatus(cell) == CellStatus.Given;

	public static bool ModifiableCells(scoped in Grid g, Cell cell) => g.GetStatus(cell) == CellStatus.Modifiable;

	public static bool EmptyCells(scoped in Grid g, Cell cell) => g.GetStatus(cell) == CellStatus.Empty;

	public static bool BivalueCells(scoped in Grid g, Cell cell) => PopCount((uint)g.GetCandidates(cell)) == 2;

	public static bool CandidatesMap(scoped in Grid g, Cell cell, Digit digit) => g.Exists(cell, digit) is true;

	public static bool DigitsMap(scoped in Grid g, Cell cell, Digit digit) => (g.GetCandidates(cell) >> digit & 1) != 0;

	public static bool ValuesMap(scoped in Grid g, Cell cell, Digit digit) => g.GetDigit(cell) == digit;
}
