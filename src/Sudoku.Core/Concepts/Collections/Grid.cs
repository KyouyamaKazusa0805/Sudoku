#define SOLUTION_DISPLAY_MODIFIABLES


namespace Sudoku.Concepts.Collections;

/// <summary>
/// Represents a sudoku grid that uses the mask list to construct the data structure.
/// </summary>
[DebuggerDisplay($$"""{{{nameof(ToString)}}("#")}""")]
[JsonConverter(typeof(GridJsonConverter))]
public unsafe partial struct Grid :
	ISimpleFormattable,
	ISimpleParseable<Grid>,
	IEqualityOperators<Grid, Grid>
{
	/// <summary>
	/// Indicates the default mask of a cell (an empty cell, with all 9 candidates left).
	/// </summary>
	public const short DefaultMask = EmptyMask | MaxCandidatesMask;

	/// <summary>
	/// Indicates the maximum candidate mask that used.
	/// </summary>
	public const short MaxCandidatesMask = (1 << 9) - 1;

	/// <summary>
	/// Indicates the empty mask, modifiable mask and given mask.
	/// </summary>
	public const short EmptyMask = (int)CellStatus.Empty << 9;

	/// <summary>
	/// Indicates the modifiable mask.
	/// </summary>
	public const short ModifiableMask = (int)CellStatus.Modifiable << 9;

	/// <summary>
	/// Indicates the given mask.
	/// </summary>
	public const short GivenMask = (int)CellStatus.Given << 9;


	/// <summary>
	/// Indicates the empty grid string.
	/// </summary>
	public static readonly string EmptyString = new('0', 81);

	/// <summary>
	/// Indicates the event triggered when the value is changed.
	/// </summary>
	public static readonly void* ValueChanged;

	/// <summary>
	/// Indicates the event triggered when should re-compute candidates.
	/// </summary>
	public static readonly void* RefreshingCandidates;

	/// <summary>
	/// The empty grid that is valid during implementation or running the program
	/// (all values are <see cref="DefaultMask"/>, i.e. empty cells).
	/// </summary>
	/// <remarks>
	/// This field is initialized by the static constructor of this structure.
	/// </remarks>
	/// <seealso cref="DefaultMask"/>
	public static readonly Grid Empty;

	/// <summary>
	/// Indicates the default grid that all values are initialized 0.
	/// </summary>
	public static readonly Grid Undefined;

	/// <summary>
	/// Indicates the solver that uses the bitwise algorithm to solve a puzzle as fast as possible.
	/// </summary>
	private static readonly BitwiseSolver Solver = new();


	/// <summary>
	/// Indicates the inner array that stores the masks of the sudoku grid, which
	/// stores the in-time sudoku grid inner information.
	/// </summary>
	/// <remarks>
	/// The field uses the mask table of length 81 to indicate the status and all possible candidates
	/// holding for each cell. Each mask uses a <see cref="short"/> value, but only uses 11 of 16 bits.
	/// <code>
	/// 16       8       0
	///  |-------|-------|
	///  |   |--|--------|
	/// 16  12  9        0
	/// </code>
	/// Here the first-nine bits indicate whether the digit 1-9 is possible candidate in the current cell respectively,
	/// and the higher 3 bits indicate the cell status. The possible cell status are:
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
	/// <description>
	/// The cell is filled by a digit, but the digit isn't the given by the initial grid.
	/// </description>
	/// </item>
	/// <item>
	/// <term>Given cell (i.e. <see cref="CellStatus.Given"/>)</term>
	/// <description>
	/// The cell is filled by a digit, which is given by the initial grid and can't be modified.
	/// </description>
	/// </item>
	/// </list>
	/// </remarks>
	/// <seealso cref="CellStatus"/>
	private fixed short _values[81];


	/// <summary>
	/// Throws a <see cref="NotSupportedException"/>.
	/// </summary>
	/// <exception cref="NotSupportedException">
	/// The exception will always be thrown.
	/// </exception>
	/// <remarks>
	/// The main idea of the parameterless constructor is to create a new instance
	/// without any extra information, but the current type is special:
	/// the author wants to make you use another member instead of it to get a better experience.
	/// Therefore, the parameterless constructor is disallowed to be invoked
	/// no matter what kind of invocation, reflection or strongly reference.
	/// </remarks>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete($"Please use the member '{nameof(Empty)}' or '{nameof(Undefined)}' instead.", true)]
	public Grid() => throw new NotSupportedException();

	/// <summary>
	/// Creates an instance using grid values.
	/// </summary>
	/// <param name="gridValues">The array of grid values.</param>
	/// <param name="creatingOption">The grid creating option.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Grid(int[] gridValues, GridCreatingOption creatingOption = GridCreatingOption.None) :
		this(gridValues[0], creatingOption)
	{
	}

	/// <summary>
	/// Initializes a <see cref="Grid"/> instance via the array of cell digits of type <see cref="int"/>*.
	/// </summary>
	/// <param name="pGridValues">The pointer parameter indicating the array of cell digits.</param>
	/// <param name="creatingOption">The grid creating option.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Grid(int* pGridValues, GridCreatingOption creatingOption = GridCreatingOption.None) :
		this(*pGridValues, creatingOption)
	{
	}

	/// <summary>
	/// Initializes a <see cref="Grid"/> instance via the array of cell digits
	/// of type <see cref="ReadOnlySpan{T}"/>.
	/// </summary>
	/// <param name="gridValues">The list of cell digits.</param>
	/// <param name="creatingOption">The grid creating option.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Grid(scoped ReadOnlySpan<int> gridValues, GridCreatingOption creatingOption = GridCreatingOption.None) :
		this(gridValues[0], creatingOption)
	{
	}

	/// <summary>
	/// Initializes an instance with the specified mask array.
	/// </summary>
	/// <param name="masks">The masks.</param>
	/// <remarks>
	/// In order to decrease the memory allocation, you can use the system buffer,
	/// whose corresponding code will be implemented like this:
	/// <code><![CDATA[
	/// // Rents the buffer memory.
	/// short[] buffer = ArrayPool<short>.Shared.Rent(81);
	/// 
	/// // Initialize the memory in order to be used later.
	/// fixed (short* pBuffer = buffer, pGrid = this)
	/// {
	///     Unsafe.CopyBlock(pBuffer, pGrid, sizeof(short) * 81);
	/// }
	///
	/// // Gets the result sudoku grid instance.
	/// try
	/// {
	///     var targetGrid = new Grid(buffer); // Now the result grid is created here.
	///
	///     // Do something to use 'targetGrid'.
	/// }
	/// finally
	/// {
	///     // Returns the buffer memory to system.
	///     ArrayPool<short>.Shared.Return(buffer, false);
	/// }
	/// ]]></code>
	/// In this way we can get the sudoku grid without any allocations.
	/// </remarks>
	/// <exception cref="ArgumentException">Throws when <see cref="Array.Length"/> is not 81.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Grid(short[] masks)
	{
		Argument.ThrowIfNotEqual(masks.Length, 81, nameof(masks));

		Unsafe.CopyBlock(
			ref Unsafe.As<short, byte>(ref _values[0]),
			ref Unsafe.As<short, byte>(ref MemoryMarshal.GetArrayDataReference(masks)),
			sizeof(short) * 81
		);
	}

	/// <summary>
	/// Creates a <see cref="Grid"/> instance via the pointer of the first element of the cell digit,
	/// and the creating option.
	/// </summary>
	/// <param name="firstElement">
	/// <para>The reference of the first element.</para>
	/// <para>
	/// <include
	///     file='../../global-doc-comments.xml'
	///     path='g/csharp7/feature[@name="ref-returns"]/target[@name="in-parameter"]'/>
	/// </para>
	/// </param>
	/// <param name="creatingOption">The creating option.</param>
	/// <remarks>
	/// <include
	///     file='../../global-doc-comments.xml'
	///     path='g/csharp7/feature[@name="ref-returns"]/target[@name="method"]'/>
	/// </remarks>
	/// <exception cref="ArgumentNullException">
	/// Throws when the argument <paramref name="firstElement"/> is <see langword="null"/> reference.
	/// </exception>
	private Grid(scoped in int firstElement, GridCreatingOption creatingOption = GridCreatingOption.None)
	{
		Argument.ThrowIfNullRef(firstElement);

		// Firstly we should initialize the inner values.
		this = Empty;

		// Then traverse the array (span, pointer or etc.), to get refresh the values.
		bool minusOneEnabled = creatingOption == GridCreatingOption.MinusOne;
		for (int i = 0; i < 81; i++)
		{
			int value = Unsafe.AddByteOffset(ref Unsafe.AsRef(firstElement), (nuint)(i * sizeof(int)));
			if ((minusOneEnabled ? value - 1 : value) is var realValue and not -1)
			{
				// Calls the indexer to trigger the event (Clear the candidates in peer cells).
				this[i] = realValue;

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
		scoped ref short firstElement = ref Empty._values[0];
		for (int i = 0; i < 81; i++)
		{
			Unsafe.AddByteOffset(ref firstElement, (nuint)(i * sizeof(short))) = DefaultMask;
		}

		// Initializes events.
		ValueChanged = (delegate*<ref Grid, int, short, short, int, void>)&onValueChanged;
		RefreshingCandidates = (delegate*<ref Grid, void>)&onRefreshingCandidates;


		static void onValueChanged(ref Grid @this, int cell, short oldMask, short newMask, int setValue)
		{
			if (setValue != -1)
			{
				foreach (int peerCell in PeerMaps[cell])
				{
					if (@this.GetStatus(peerCell) == CellStatus.Empty)
					{
						// You can't do this because of being invoked recursively.
						//@this[peerCell, setValue] = false;

						@this._values[peerCell] &= (short)~(1 << setValue);
					}
				}
			}
		}

		static void onRefreshingCandidates(ref Grid @this)
		{
			for (int i = 0; i < 81; i++)
			{
				if (@this.GetStatus(i) == CellStatus.Empty)
				{
					// Remove all appeared digits.
					short mask = MaxCandidatesMask;
					foreach (int cell in PeerMaps[i])
					{
						if (@this[cell] is var digit and not -1)
						{
							mask &= (short)~(1 << digit);
						}
					}

					@this._values[i] = (short)(EmptyMask | mask);
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
			for (int i = 0; i < 81; i++)
			{
				if (GetStatus(i) == CellStatus.Empty)
				{
					return false;
				}
			}

			for (int i = 0; i < 81; i++)
			{
				switch (GetStatus(i))
				{
					case CellStatus.Given:
					case CellStatus.Modifiable:
					{
						int curDigit = this[i];
						foreach (int cell in PeerMaps[i])
						{
							if (curDigit == this[cell])
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
	/// Indicates whether the puzzle is valid, which means the puzzle contains a unique solution.
	/// </summary>
	public readonly bool IsValid
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Solver.CheckValidity(ToString());
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
	/// Indicates the number of total candidates.
	/// </summary>
	public readonly int CandidatesCount
	{
		get
		{
			int count = 0;
			for (int i = 0; i < 81; i++)
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
	/// The property returns an <see cref="int"/> value as a mask that contains all possible house indices.
	/// For example, if the row 5, column 5 and block 5 (1-9) are null houses, the property will return
	/// the result <see cref="int"/> value, <c>000010000_000010000_000010000</c> as binary.
	/// </para>
	/// </summary>
	public readonly int NullHouses
	{
		get
		{
			int maskResult = 0;
			for (int houseIndex = 0; houseIndex < 27; houseIndex++)
			{
				if ((EmptyCells & HouseMaps[houseIndex]).Count == 9)
				{
					maskResult |= 1 << houseIndex;
				}
			}

			return maskResult;
		}
	}

	/// <summary>
	/// Gets the cell template that only contains the given cells.
	/// </summary>
	public readonly Cells GivenCells
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return GetCells(&p);


			static bool p(in Grid g, int cell) => g.GetStatus(cell) == CellStatus.Given;
		}
	}

	/// <summary>
	/// Gets the cell template that only contains the modifiable cells.
	/// </summary>
	public readonly Cells ModifiableCells
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return GetCells(&p);


			static bool p(in Grid g, int cell) => g.GetStatus(cell) == CellStatus.Modifiable;
		}
	}

	/// <summary>
	/// Indicates the cells that corresponding position in this grid is empty.
	/// </summary>
	public readonly Cells EmptyCells
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return GetCells(&p);


			static bool p(in Grid g, int cell) => g.GetStatus(cell) == CellStatus.Empty;
		}
	}

	/// <summary>
	/// Indicates the cells that corresponding position in this grid contain two candidates.
	/// </summary>
	public readonly Cells BivalueCells
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return GetCells(&p);


			static bool p(in Grid g, int cell) => PopCount((uint)g.GetCandidates(cell)) == 2;
		}
	}

	/// <summary>
	/// Indicates the map of possible positions of the existence of the candidate value for each digit.
	/// The return value will be an array of 9 elements, which stands for the statuses of 9 digits.
	/// </summary>
	public readonly Cells[] CandidatesMap
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return GetMap(&p);


			static bool p(in Grid g, int cell, int digit) => g.Exists(cell, digit) is true;
		}
	}

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
	public readonly Cells[] DigitsMap
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return GetMap(&p);


			static bool p(in Grid g, int cell, int digit) => (g.GetCandidates(cell) >> digit & 1) != 0;
		}
	}

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
	public readonly Cells[] ValuesMap
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return GetMap(&p);


			static bool p(in Grid g, int cell, int digit) => g[cell] == digit;
		}
	}

	/// <summary>
	/// Gets the grid where all modifiable cells are empty cells (i.e. the initial one).
	/// </summary>
	public readonly Grid ResetGrid
	{
		get
		{
			scoped var arr = (stackalloc int[81]);
			arr.Fill(-1);

			foreach (int cell in GivenCells)
			{
				arr[cell] = this[cell];
			}

			return new(arr);
		}
	}

	/// <summary>
	/// Indicates the solution of the grid. If failed to solve (for example,
	/// the puzzle contains multiple solutions), the property will return <see cref="Undefined"/>.
	/// </summary>
	/// <see cref="Undefined"/>
	public readonly Grid Solution
	{
#if !SOLUTION_DISPLAY_MODIFIABLES
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		get
		{
			// Gets the solution grid. The current result grid may be undefined if the grid is invalid.
			var solution = Solver.Solve(this);

#if SOLUTION_DISPLAY_MODIFIABLES
			// Checks for the grid validity.
			if (solution.IsUndefined)
			{
				return solution;
			}

			// Displays modifiable values if worth.
			var result = Empty;
			for (int i = 0; i < 81; i++)
			{
				result[i] = solution[i];
				if (GivenCells.Contains(i))
				{
					result.SetStatus(i, CellStatus.Given);
				}
			}
#endif

			// Return the result.
			return result;
		}
	}


	/// <summary>
	/// Gets or sets the digit that has been filled in the specified cell.
	/// </summary>
	/// <param name="cell">The cell you want to get or set a value.</param>
	/// <value>
	/// <para>
	/// The value you want to set. The value should be between 0 and 8. If assigning -1,
	/// that means to re-compute all candidates.
	/// </para>
	/// <para>
	/// The values set into the grid will be regarded as the modifiable values.
	/// If the cell contains a digit, it will be covered when it is a modifiable value.
	/// If the cell is a given cell, the setter will do nothing.
	/// </para>
	/// </value>
	/// <returns>
	/// The value that the cell filled with. The possible values are:
	/// <list type="table">
	/// <item>
	/// <term>-1</term>
	/// <description>The status of the specified cell is <see cref="CellStatus.Empty"/>.</description>
	/// </item>
	/// <item>
	/// <term><![CDATA[>= 0 and < 9]]></term>
	/// <description>
	/// The actual value that the cell filled with. 0 is for the digit 1, 1 is for the digit 2, etc..
	/// </description>
	/// </item>
	/// </list>
	/// </returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when the specified cell keeps a wrong cell status value.
	/// For example, <see cref="CellStatus.Undefined"/>.
	/// </exception>
	public int this[int cell]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get => GetStatus(cell) switch
		{
			CellStatus.Empty => -1,
			CellStatus.Modifiable or CellStatus.Given => TrailingZeroCount(_values[cell]),
			_ => throw new InvalidOperationException("The grid cannot keep invalid cell status value.")
		};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			switch (value)
			{
				case -1 when GetStatus(cell) == CellStatus.Modifiable:
				{
					// If 'value' is -1, we should reset the grid.
					// Note that reset candidates may not trigger the event.
					_values[cell] = DefaultMask;

					((delegate*<ref Grid, void>)RefreshingCandidates)(ref this);

					break;
				}
				case >= 0 and < 9:
				{
					scoped ref short result = ref _values[cell];
					short copied = result;

					// Set cell status to 'CellStatus.Modifiable'.
					result = (short)(ModifiableMask | 1 << value);

					// To trigger the event, which is used for eliminate all same candidates in peer cells.
					var f = (delegate*<ref Grid, int, short, short, int, void>)ValueChanged;
					f(ref this, cell, copied, result, value);

					break;
				}
			}
		}
	}

	/// <summary>
	/// Gets or sets a candidate existence case with a <see cref="bool"/> value.
	/// </summary>
	/// <param name="cell">The cell offset between 0 and 80.</param>
	/// <param name="digit">The digit between 0 and 8.</param>
	/// <value>
	/// The case you want to set. <see langword="false"/> means that this candidate
	/// doesn't exist in this current sudoku grid; otherwise, <see langword="true"/>.
	/// </value>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	public bool this[int cell, int digit]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get => (_values[cell] >> digit & 1) != 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (cell is >= 0 and < 81 && digit is >= 0 and < 9)
			{
				short copied = _values[cell];
				if (value)
				{
					_values[cell] |= (short)(1 << digit);
				}
				else
				{
					_values[cell] &= (short)~(1 << digit);
				}

				// To trigger the event.
				var f = (delegate*<ref Grid, int, short, short, int, void>)ValueChanged;
				f(ref this, cell, copied, _values[cell], -1);
			}
		}
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly bool Equals([NotNullWhen(true)] object? obj)
		=> obj is Grid comparer && Equals(this, comparer);

	/// <summary>
	/// Determine whether the specified <see cref="Grid"/> instance hold the same values
	/// as the current instance.
	/// </summary>
	/// <param name="other">The instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(scoped in Grid other) => Equals(this, other);

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
		Unsafe.SkipInit(out solutionIfValid);
		if (Solver.CheckValidity(ToString(null), out string? solution))
		{
			solutionIfValid = Parse(solution);
			sukaku = false;
			return true;
		}
		else if (Solver.CheckValidity(ToString("~"), out solution))
		{
			solutionIfValid = Parse(solution);
			sukaku = true;
			return true;
		}
		else
		{
			sukaku = null;
			return false;
		}
	}

	/// <summary>
	/// Indicates whether the current grid contains the specified candidate offset.
	/// </summary>
	/// <param name="candidate">The candidate offset.</param>
	/// <returns>
	/// The method will return a <see cref="bool"/>? value (contains three possible cases:
	/// <see langword="true"/>, <see langword="false"/> and <see langword="null"/>).
	/// All values corresponding to the cases are below:
	/// <list type="table">
	/// <item>
	/// <term><see langword="true"/></term>
	/// <description>
	/// The cell that the candidate specified is an empty cell <b>and</b> contains the specified digit
	/// that the candidate specified.
	/// </description>
	/// </item>
	/// <item>
	/// <term><see langword="false"/></term>
	/// <description>
	/// The cell that the candidate specified is an empty cell <b>but doesn't</b> contain the specified digit
	/// that the candidate specified.
	/// </description>
	/// </item>
	/// <item>
	/// <term><see langword="null"/></term>
	/// <description>
	/// The cell that the candidate specified is <b>not</b> an empty cell that the candidate specified.
	/// </description>
	/// </item>
	/// </list>
	/// </returns>
	/// <remarks>
	/// Note that the method will return a <see cref="bool"/>?, so you should use the code
	/// '<c>grid.Exists(candidate) is true</c>' or '<c>grid.Exists(candidate) == true</c>'
	/// to decide whether a condition is true.
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool? Exists(int candidate) => Exists(candidate / 9, candidate % 9);

	/// <summary>
	/// Indicates whether the current grid contains the digit in the specified cell.
	/// </summary>
	/// <param name="cell">The cell offset.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>
	/// The method will return a <see cref="bool"/>? value (contains three possible cases:
	/// <see langword="true"/>, <see langword="false"/> and <see langword="null"/>).
	/// All values corresponding to the cases are below:
	/// <list type="table">
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
	/// the result case will be more precisely than the indexer <see cref="this[int, int]"/>,
	/// which is the main difference between this method and that indexer.
	/// </para>
	/// </remarks>
	/// <seealso cref="this[int, int]"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool? Exists(int cell, int digit) => GetStatus(cell) == CellStatus.Empty ? this[cell, digit] : null;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly int GetHashCode()
		=> this switch { { IsUndefined: true } => 0, { IsEmpty: true } => 1, _ => ToString("#").GetHashCode() };

	/// <summary>
	/// Serializes this instance to an array, where all digit value will be stored.
	/// </summary>
	/// <returns>
	/// This array. All elements are between 0 to 9, where 0 means the
	/// cell is <see cref="CellStatus.Empty"/> now.
	/// </returns>
	public readonly int[] ToArray()
	{
		int[] result = new int[81];
		for (int i = 0; i < 81; i++)
		{
			// 'this[i]' is always between -1 and 8 (-1 is empty, and 0 to 8 is 1 to 9 for human representation).
			result[i] = this[i] + 1;
		}

		return result;
	}

	/// <summary>
	/// Get a mask at the specified cell.
	/// </summary>
	/// <param name="offset">The cell offset you want to get.</param>
	/// <returns>The mask.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly short GetMask(int offset) => _values[offset];

	/// <summary>
	/// Get the candidate mask part of the specified cell.
	/// </summary>
	/// <param name="cell">The cell offset you want to get.</param>
	/// <returns>
	/// <para>
	/// The candidate mask. The return value is a 9-bit <see cref="short"/>
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
	public readonly short GetCandidates(int cell) => (short)(_values[cell] & MaxCandidatesMask);

	/// <summary>
	/// Creates a mask of type <see cref="short"/> that represents the usages of digits 1 to 9,
	/// ranged in a specified list of cells in the current sudoku grid.
	/// </summary>
	/// <param name="cells">The list of cells to gather the usages on all digits.</param>
	/// <returns>A mask of type <see cref="short"/> that represents the usages of digits 1 to 9.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly short GetDigitsUnion(int[] cells)
	{
		short result = 0;
		for (int i = 0, length = cells.Length; i < length; i++)
		{
			result |= _values[cells[i]];
		}

		return (short)(result & MaxCandidatesMask);
	}

	/// <summary>
	/// Creates a mask of type <see cref="short"/> that represents the usages of digits 1 to 9,
	/// ranged in a specified list of cells in the current sudoku grid.
	/// </summary>
	/// <param name="cells">The list of cells to gather the usages on all digits.</param>
	/// <returns>A mask of type <see cref="short"/> that represents the usages of digits 1 to 9.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly short GetDigitsUnion(scoped in Cells cells)
	{
		short result = 0;
		foreach (int cell in cells)
		{
			result |= _values[cell];
		}

		return (short)(result & MaxCandidatesMask);
	}

	/// <summary>
	/// Creates a mask of type <see cref="short"/> that represents the usages of digits 1 to 9,
	/// ranged in a specified list of cells in the current sudoku grid.
	/// </summary>
	/// <param name="cells">The list of cells to gather the usages on all digits.</param>
	/// <param name="withValueCells">
	/// Indicates whether the value cells (given or modifiable ones) will be included to be gathered.
	/// If <see langword="true"/>, all value cells (no matter what kind of cell) will be summed up.
	/// </param>
	/// <returns>A mask of type <see cref="short"/> that represents the usages of digits 1 to 9.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly short GetDigitsUnion(scoped in Cells cells, bool withValueCells)
	{
		short result = 0;
		foreach (int cell in cells)
		{
			if (!withValueCells && GetStatus(cell) != CellStatus.Empty || withValueCells)
			{
				result |= _values[cell];
			}
		}

		return (short)(result & MaxCandidatesMask);
	}

	/// <summary>
	/// Creates a mask of type <see cref="short"/> that represents the usages of digits 1 to 9,
	/// ranged in a specified list of cells in the current sudoku grid,
	/// to determine which digits are not used.
	/// </summary>
	/// <param name="cells">The list of cells to gather the usages on all digits.</param>
	/// <returns>A mask of type <see cref="short"/> that represents the usages of digits 1 to 9.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly short GetDigitsIntersection(scoped in Cells cells)
	{
		short result = MaxCandidatesMask;
		foreach (int cell in cells)
		{
			result &= (short)~_values[cell];
		}

		return result;
	}

	/// <include
	///	    file="../../global-doc-comments.xml"
	///	    path="g/csharp7/feature[@name='custom-fixed']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public readonly ref readonly short GetPinnableReference() => ref _values[0];

	/// <summary>
	/// Gets the mask at the specified position. This method returns the reference of the mask.
	/// </summary>
	/// <param name="index">The desired index.</param>
	/// <returns>The reference to the mask that you want to get.</returns>
	/// <remarks>
	/// <para>
	/// This method returns the reference, which means you can use this method as an lvalue.
	/// For example, if you want to use bitwise-or operator to update the value, you can use:
	/// <code><![CDATA[
	/// // Update the mask.
	/// short mask = ...;
	/// grid.GetMaskRefAt(0) |= mask;
	/// ]]></code>
	/// The expression <c>grid.GetMaskRefAt(0) |= mask</c> is equivalent to
	/// <c>grid.GetMaskRefAt(0) = grid.GetMaskRefAt(0) | mask</c>, and it can be replaced
	/// with the expression <c>grid._values[0] = grid._values[0] | mask</c>,
	/// meaning we update the mask at the first place (i.e. <c>r1c1</c>).
	/// </para>
	/// <para>
	/// This method is a little bit different with <see cref="GetPinnableReference"/>.
	/// This method returns an modifiable reference, therefore the return value is
	/// <see langword="ref"/> <see cref="short"/> instead of <see langword="ref readonly"/> <see cref="short"/>
	/// being used by that method, which means you cannot use it as an lvalue to update the mask.
	/// In addition, that method always returns the reference to the <b>first</b> element.
	/// </para>
	/// </remarks>
	/// <seealso cref="GetPinnableReference"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ref short GetMaskRefAt(int index) => ref _values[index];

	/// <summary>
	/// Get all masks and print them.
	/// </summary>
	/// <returns>The result.</returns>
	/// <remarks>
	/// Please note that the method cannot be called with a correct behavior using
	/// <see cref="DebuggerDisplayAttribute"/> to output. It seems that Visual Studio
	/// doesn't print correct values when indices of this grid aren't 0. In other words,
	/// when we call this method using <see cref="DebuggerDisplayAttribute"/>, only <c>grid[0]</c>
	/// can be output correctly, and other values will be incorrect: they're always 0.
	/// </remarks>
	public readonly string ToMaskString()
	{
		const string separator = ", ";
		scoped var sb = new StringHandler(400);
		sb.AppendRangeWithSeparatorUnsafe(
			(short*)Unsafe.AsPointer(ref Unsafe.AsRef(_values[0])),
			81,
			&StringHandler.ElementToStringConverter,
			separator
		);

		return sb.ToStringAndClear();
	}

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly string ToString() => ToString(null);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(string? format)
		=> this switch
		{
			{ IsEmpty: true } => "<Empty>",
			{ IsUndefined: true } => "<Undefined>",
			_ when FileLocalType_GridFormatterFactory.Create(format) is var f
				=> format switch
				{
					":" => ExtendedSusserEliminationsRegex().Match(f.ToString(this)) switch
					{
						{ Success: true, Value: var value } => value,
						_ => string.Empty
					},
					"!" => f.ToString(this).RemoveAll('+'),
					".!" or "!." or "0!" or "!0" => f.ToString(this).RemoveAll('+'),
					".!:" or "!.:" or "0!:" => f.ToString(this).RemoveAll('+'),
					_ => f.ToString(this)
				},
			_ => throw new FormatException("The current status is invalid.")
		};

	/// <summary>
	/// Get the cell status at the specified cell.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <returns>The cell status.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly CellStatus GetStatus(int cell) => MaskToStatus(_values[cell]);

	/// <summary>
	/// Gets the enumerator of the current instance in order to use <see langword="foreach"/> loop.
	/// </summary>
	/// <returns>The enumerator instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly CandidateCollectionEnumerator GetEnumerator() => EnumerateCandidates();

	/// <summary>
	/// Try to enumerate all possible candidates in the current grid.
	/// </summary>
	/// <returns>
	/// An enumerator that allows us using <see langword="foreach"/> statement
	/// to iterate all possible candidates in the current grid.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly CandidateCollectionEnumerator EnumerateCandidates() => new(_values[0]);

	/// <summary>
	/// Try to enumerate the mask table of the current grid.
	/// </summary>
	/// <returns>
	/// An enumerator that allows us using <see langword="foreach"/> statement
	/// to iterate all masks in the current grid. The mask list must contain 81 masks.
	/// </returns>
	/// <remarks>
	/// Please note that the iterator will iterate all masks by reference, which means
	/// you can apply <see langword="ref"/> and <see langword="ref readonly"/> modifier
	/// onto the iteration variable:
	/// <code>
	/// foreach (ref readonly short mask in grid)
	/// {
	///     // Do something.
	/// }
	/// </code>
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly MaskCollectionEnumerator EnumerateMasks() => new(_values[0]);

	/// <summary>
	/// Reset the sudoku grid, to set all modifiable values to empty ones.
	/// </summary>
	public void Reset()
	{
		for (int i = 0; i < 81; i++)
		{
			if (GetStatus(i) == CellStatus.Modifiable)
			{
				this[i] = -1; // Reset the cell, and then re-compute all candidates.
			}
		}
	}

	/// <summary>
	/// To fix the current grid (all modifiable values will be changed to given ones).
	/// </summary>
	public void Fix()
	{
		for (int i = 0; i < 81; i++)
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
		for (int i = 0; i < 81; i++)
		{
			if (GetStatus(i) == CellStatus.Given)
			{
				SetStatus(i, CellStatus.Modifiable);
			}
		}
	}

	/// <summary>
	/// Set the specified cell to the specified status.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="status">The status.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetStatus(int cell, CellStatus status)
	{
		scoped ref short mask = ref _values[cell];
		short copied = mask;
		mask = (short)((int)status << 9 | mask & MaxCandidatesMask);

		((delegate*<ref Grid, int, short, short, int, void>)ValueChanged)(ref this, cell, copied, mask, -1);
	}

	/// <summary>
	/// Set the specified cell to the specified mask.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="mask">The mask to set.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetMask(int cell, short mask)
	{
		scoped ref short m = ref _values[cell];
		short copied = m;
		m = mask;

		((delegate*<ref Grid, int, short, short, int, void>)ValueChanged)(ref this, cell, copied, m, -1);
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly bool IEquatable<Grid>.Equals(Grid other) => Equals(this, other);

	/// <summary>
	/// Called by properties <see cref="CandidatesMap"/>, <see cref="DigitsMap"/> and <see cref="ValuesMap"/>.
	/// </summary>
	/// <param name="predicate">The predicate.</param>
	/// <returns>The map of digits.</returns>
	/// <seealso cref="CandidatesMap"/>
	/// <seealso cref="DigitsMap"/>
	/// <seealso cref="ValuesMap"/>
	private readonly Cells[] GetMap(delegate*<in Grid, int, int, bool> predicate)
	{
		var result = new Cells[9];
		for (int digit = 0; digit < 9; digit++)
		{
			scoped ref var map = ref result[digit];
			for (int cell = 0; cell < 81; cell++)
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
	/// Called by properties <see cref="EmptyCells"/> and <see cref="BivalueCells"/>.
	/// </summary>
	/// <param name="predicate">The predicate.</param>
	/// <returns>The cells.</returns>
	/// <seealso cref="EmptyCells"/>
	/// <seealso cref="BivalueCells"/>
	private readonly Cells GetCells(delegate*<in Grid, int, bool> predicate)
	{
		var result = Cells.Empty;
		for (int cell = 0; cell < 81; cell++)
		{
			if (predicate(this, cell))
			{
				result.Add(cell);
			}
		}

		return result;
	}


	/// <summary>
	/// To determine whether two sudoku grid is totally same.
	/// </summary>
	/// <param name="left">The left one.</param>
	/// <param name="right">The right one.</param>
	/// <returns>The <see cref="bool"/> result indicating that.</returns>
	public static bool Equals(scoped in Grid left, scoped in Grid right)
	{
		fixed (short* pThis = left, pOther = right)
		{
			int i = 0;
			for (short* l = pThis, r = pOther; i < 81; i++, l++, r++)
			{
				if (*l != *r)
				{
					return false;
				}
			}

			return true;
		}
	}

	/// <summary>
	/// Parses a pointer that points to a string value and converts to this type.
	/// </summary>
	/// <param name="ptrStr">The pointer that points to string.</param>
	/// <returns>The result instance.</returns>
	/// <exception cref="ArgumentNullException">
	/// Throws when the only argument is <see langword="null"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Parse(char* ptrStr)
	{
		Argument.ThrowIfNull(ptrStr);

		return Parse(new string(ptrStr));
	}

	/// <inheritdoc/>
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
	public static Grid Parse(string str, GridParsingOption gridParsingOption)
		=> new GridParser(str).Parse(gridParsingOption);

	/// <inheritdoc cref="Parse(string)"/>
	/// <param name="handler">The string handler.</param>
	public static Grid Parse([InterpolatedStringHandlerArgument] ref StringHandler handler)
		=> Parse(handler.ToStringAndClear());

	/// <summary>
	/// Parses a pointer that points to a <see cref="Utf8String"/> value and converts to this type.
	/// </summary>
	/// <param name="ptrStr">The pointer that points to string.</param>
	/// <returns>The result instance.</returns>
	/// <exception cref="ArgumentNullException">
	/// Throws when the only argument is <see langword="null"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Parse(Utf8Char* ptrStr)
	{
		Argument.ThrowIfNull(ptrStr);

		return Parse(new Utf8String(ptrStr));
	}

	/// <inheritdoc cref="Parse(string)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Parse(Utf8String str) => new FileLocalType_Utf8GridParser(str).Parse();

	/// <inheritdoc cref="Parse(string, bool)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Parse(Utf8String str, bool compatibleFirst) => new FileLocalType_Utf8GridParser(str, compatibleFirst).Parse();

	/// <inheritdoc cref="Parse(string, GridParsingOption)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Parse(Utf8String str, GridParsingOption gridParsingOption)
		=> new FileLocalType_Utf8GridParser(str).Parse(gridParsingOption);

	/// <summary>
	/// <para>Parses a string value and converts to this type.</para>
	/// <para>
	/// If you want to parse a PM grid, we recommend you use the method
	/// <see cref="Parse(Utf8String, GridParsingOption)"/> instead of this method.
	/// </para>
	/// </summary>
	/// <param name="str">The string.</param>
	/// <returns>The result instance had converted.</returns>
	/// <seealso cref="Parse(Utf8String, GridParsingOption)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Parse(scoped ReadOnlySpan<byte> str) => new FileLocalType_Utf8GridParser(str.ToArray()).Parse();

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
	/// The result parsed. If the conversion is failed,
	/// this argument will be <see cref="Undefined"/>.
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

	/// <summary>
	/// To get the cell status through a mask.
	/// </summary>
	/// <param name="mask">The mask.</param>
	/// <returns>The cell status.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static CellStatus MaskToStatus(short mask) => (CellStatus)(mask >> 9 & 7);

	/// <summary>
	/// Indicates the eliminations in the extended susser format.
	/// </summary>
	[RegexGenerator("""(?<=\:)(\d{3}\s+)*\d{3}""", RegexOptions.Compiled, 5000)]
	internal static partial Regex ExtendedSusserEliminationsRegex();


	/// <summary>
	/// Gets a sudoku grid, removing all digits filled in the cells
	/// that doesn't appear in the specified <paramref name="pattern"/>.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="pattern">The pattern.</param>
	/// <returns>The result grid.</returns>
	public static Grid operator &(scoped in Grid grid, scoped in Cells pattern)
	{
		var result = grid;
		foreach (int cell in ~pattern)
		{
			result[cell] = -1;
		}

		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(scoped in Grid left, scoped in Grid right) => left.Equals(right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(scoped in Grid left, scoped in Grid right) => !(left == right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEqualityOperators<Grid, Grid>.operator ==(Grid left, Grid right) => left == right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEqualityOperators<Grid, Grid>.operator !=(Grid left, Grid right) => left != right;
}

/// <summary>
/// Provides a formatter that gathers the main information for a <see cref="Grid"/> instance,
/// and convert it to a <see cref="string"/> value as the result.
/// </summary>
internal readonly ref partial struct FileLocalType_GridFormatter
{
	/// <summary>
	/// Indicates the inner mask that stores the flags.
	/// </summary>
	private readonly short _flags;


	/// <summary>
	/// Initializes an instance with a <see cref="bool"/> value
	/// indicating multi-line.
	/// </summary>
	/// <param name="multiline">
	/// The multi-line identifier. If the value is <see langword="true"/>, the output will
	/// be multi-line.
	/// </param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public FileLocalType_GridFormatter(bool multiline) : this(
		placeholder: '.', multiline: multiline, withModifiables: false,
		withCandidates: false, treatValueAsGiven: false, subtleGridLines: false,
		hodokuCompatible: false, sukaku: false, excel: false, openSudoku: false,
		shortenSusser: false)
	{
	}

	/// <summary>
	/// Initializes a <see cref="FileLocalType_GridFormatter"/> instance using the specified mask storing all possible flags.
	/// </summary>
	/// <param name="flags">The flags.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public FileLocalType_GridFormatter(short flags) => _flags = flags;

	/// <summary>
	/// Initialize an instance with the specified information.
	/// </summary>
	/// <param name="placeholder">The placeholder.</param>
	/// <param name="multiline">Indicates whether the formatter will use multiple lines mode.</param>
	/// <param name="withModifiables">Indicates whether the formatter will output modifiables.</param>
	/// <param name="withCandidates">
	/// Indicates whether the formatter will output candidates list.
	/// </param>
	/// <param name="treatValueAsGiven">
	/// Indicates whether the formatter will treat values as givens always.
	/// </param>
	/// <param name="subtleGridLines">
	/// Indicates whether the formatter will process outline corner of the multi-line grid.
	/// </param>
	/// <param name="hodokuCompatible">
	/// Indicates whether the formatter will use hodoku library mode to output.
	/// </param>
	/// <param name="sukaku">Indicates whether the formatter will output as sukaku.</param>
	/// <param name="excel">Indicates whether the formatter will output as excel.</param>
	/// <param name="openSudoku">
	/// Indicates whether the formatter will output as open sudoku format.
	/// </param>
	/// <param name="shortenSusser">
	/// Indicates whether the formatter will shorten the susser format.
	/// </param>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="placeholder"/> is not supported.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private FileLocalType_GridFormatter(
		char placeholder, bool multiline, bool withModifiables, bool withCandidates,
		bool treatValueAsGiven, bool subtleGridLines, bool hodokuCompatible,
		bool sukaku, bool excel, bool openSudoku, bool shortenSusser)
	{
		_flags = placeholder switch
		{
			'.' => 0,
			'0' => 1024,
			_ => throw new ArgumentOutOfRangeException(nameof(placeholder))
		};
		_flags |= (short)(multiline ? 512 : 0);
		_flags |= (short)(withModifiables ? 256 : 0);
		_flags |= (short)(withCandidates ? 128 : 0);
		_flags |= (short)(treatValueAsGiven ? 64 : 0);
		_flags |= (short)(subtleGridLines ? 32 : 0);
		_flags |= (short)(hodokuCompatible ? 16 : 0);
		_flags |= (short)(sukaku ? 8 : 0);
		_flags |= (short)(excel ? 4 : 0);
		_flags |= (short)(openSudoku ? 2 : 0);
		_flags |= (short)(shortenSusser ? 1 : 0);
	}


	/// <summary>
	/// The place holder.
	/// </summary>
	/// <returns>The result placeholder text.</returns>
	/// <value>The value to assign. The value must be 46 (<c>'.'</c>) or 48 (<c>'0'</c>).</value>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <see langword="value"/> is not supported.
	/// </exception>
	public char Placeholder
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (_flags >> 10 & 1) != 0 ? '.' : '0';

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		init => _flags = value switch
		{
			'.' => (short)(_flags & 1023 | 1024),
			'0' => (short)(_flags & 1023),
			_ => throw new ArgumentOutOfRangeException(nameof(value))
		};
	}

	/// <summary>
	/// Indicates whether the output should be multi-line.
	/// </summary>
	public bool Multiline
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (_flags >> 9 & 1) != 0;

#if false
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		init => _flags |= (short)(value ? 512 : 0);
#endif
	}

	/// <summary>
	/// Indicates the output should be with modifiable values.
	/// </summary>
	/// <returns>The output should be with modifiable values.</returns>
	/// <value>A <see cref="bool"/> value to set.</value>
	public bool WithModifiables
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (_flags >> 8 & 1) != 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		init => _flags |= (short)(value ? 256 : 0);
	}

	/// <summary>
	/// <para>
	/// Indicates the output should be with candidates.
	/// If the output is single line, the candidates will indicate
	/// the candidates-have-eliminated before the current grid status;
	/// if the output is multi-line, the candidates will indicate
	/// the real candidate at the current grid status.
	/// </para>
	/// <para>
	/// If the output is single line, the output will append the candidates
	/// value at the tail of the string in '<c>:candidate list</c>'. In addition,
	/// candidates will be represented as 'digit', 'row offset' and
	/// 'column offset' in order.
	/// </para>
	/// </summary>
	/// <returns>The output should be with candidates.</returns>
	/// <value>A <see cref="bool"/> value to set.</value>
	public bool WithCandidates
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (_flags >> 7 & 1) != 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		init => _flags |= (short)(value ? 128 : 0);
	}

	/// <summary>
	/// Indicates the output will treat modifiable values as given ones.
	/// If the output is single line, the output will remove all plus marks '+'.
	/// If the output is multi-line, the output will use '<c><![CDATA[<digit>]]></c>' instead
	/// of '<c>*digit*</c>'.
	/// </summary>
	/// <returns>The output will treat modifiable values as given ones.</returns>
	/// <value>A <see cref="bool"/> value to set.</value>
	public bool TreatValueAsGiven
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (_flags >> 6 & 1) != 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		init => _flags |= (short)(value ? 64 : 0);
	}

	/// <summary>
	/// Indicates whether need to handle all grid outlines while outputting.
	/// </summary>
	/// <returns>
	/// The <see cref="bool"/> result indicating whether need to handle all grid outlines while outputting.
	/// </returns>
	/// <value>A <see cref="bool"/> value to set.</value>
	public bool SubtleGridLines
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (_flags >> 5 & 1) != 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		init => _flags |= (short)(value ? 32 : 0);
	}

	/// <summary>
	/// Indicates whether the output will be compatible with Hodoku library format.
	/// </summary>
	/// <returns>
	/// The <see cref="bool"/> result indicating whether the output will be compatible
	/// with Hodoku library format.
	/// </returns>
	/// <value>A <see cref="bool"/> value to set.</value>
	public bool HodokuCompatible
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (_flags >> 4 & 1) != 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		init => _flags |= (short)(value ? 16 : 0);
	}

	/// <summary>
	/// Indicates the output will be sukaku format (all single-valued digit will
	/// be all treated as candidates).
	/// </summary>
	/// <returns>
	/// The output will be sukaku format (all single-valued digit will
	/// be all treated as candidates).
	/// </returns>
	/// <value>A <see cref="bool"/> value to set.</value>
	public bool Sukaku
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (_flags >> 3 & 1) != 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		init => _flags |= (short)(value ? 8 : 0);
	}

	/// <summary>
	/// Indicates the output will be Excel format.
	/// </summary>
	/// <returns>The output will be Excel format.</returns>
	/// <value>A <see cref="bool"/> value to set.</value>
	public bool Excel
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (_flags >> 2 & 1) != 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		init => _flags |= (short)(value ? 4 : 0);
	}

	/// <summary>
	/// Indicates whether the current output mode is aiming to open sudoku format.
	/// </summary>
	/// <returns>
	/// The <see cref="bool"/> result indicating whether the current output mode
	/// is aiming to open sudoku format.
	/// </returns>
	/// <value>A <see cref="bool"/> value to set.</value>
	public bool OpenSudoku
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (_flags >> 1 & 1) != 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		init => _flags |= (short)(value ? 2 : 0);
	}

	/// <summary>
	/// Indicates whether the current output mode will shorten the susser format.
	/// </summary>
	/// <returns>
	/// The <see cref="bool"/> result indicating whether the current output mode
	/// will shorten the susser format.
	/// </returns>
	/// <value>A <see cref="bool"/> value to set.</value>
	public bool ShortenSusser
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (_flags & 1) != 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		init => _flags |= (short)(value ? 1 : 0);
	}


	/// <summary>
	/// Represents a string value indicating this instance.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>The string.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(scoped in Grid grid)
		=> Sukaku
			? ToSukakuString(grid)
			: Multiline
				? WithCandidates
					? ToMultiLineStringCore(grid)
					: Excel
						? ToExcelString(grid)
						: ToMultiLineSimpleGridCore(grid)
				: HodokuCompatible
					? ToHodokuLibraryFormatString(grid)
					: OpenSudoku
						? ToOpenSudokuString(grid)
						: ToSingleLineStringCore(grid);

	/// <summary>
	/// Represents a string value indicating this instance, with the specified format string.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="format">The string format.</param>
	/// <returns>The string.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(scoped in Grid grid, string? format) => FileLocalType_GridFormatterFactory.Create(format).ToString(grid);

	/// <summary>
	/// To Excel format string.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>The string.</returns>
	private string ToExcelString(scoped in Grid grid)
	{
		scoped ReadOnlySpan<char> span = grid.ToString("0");
		scoped var sb = new StringHandler(81 + 72 + 9);
		for (int i = 0; i < 9; i++)
		{
			for (int j = 0; j < 9; j++)
			{
				if (span[i * 9 + j] - '0' is var digit and not 0)
				{
					sb.Append(digit);
				}

				sb.Append('\t');
			}

			sb.RemoveFromEnd(1);
			sb.AppendLine();
		}

		return sb.ToStringAndClear();
	}

	/// <summary>
	/// To open sudoku format string.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>The string.</returns>
	/// <exception cref="FormatException">Throws when the specified grid is invalid.</exception>
	private unsafe string ToOpenSudokuString(scoped in Grid grid)
	{
		// Calculates the length of the result string.
		const int length = 1 + (81 * 3 - 1 << 1);

		// Creates a string instance as a buffer.
		string result = new('\0', length);

		// Modify the string value via pointers.
		fixed (char* pResult = result)
		{
			// Replace the base character with the separator.
			for (int pos = 1; pos < length; pos += 2)
			{
				pResult[pos] = '|';
			}

			// Now replace some positions with the specified values.
			for (int i = 0, pos = 0; i < 81; i++, pos += 6)
			{
				switch (grid.GetStatus(i))
				{
					case CellStatus.Empty:
					{
						pResult[pos] = '0';
						pResult[pos + 2] = '0';
						pResult[pos + 4] = '1';

						break;
					}
					case CellStatus.Modifiable:
					case CellStatus.Given:
					{
						pResult[pos] = (char)(grid[i] + '1');
						pResult[pos + 2] = '0';
						pResult[pos + 4] = '0';

						break;
					}
					default:
					{
						throw new FormatException("The specified grid is invalid.");
					}
				}
			}
		}

		// Returns the result.
		return result;
	}

	/// <summary>
	/// To string with Hodoku library format compatible string.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>The string.</returns>
	private string ToHodokuLibraryFormatString(scoped in Grid grid) => $":0000:x:{ToSingleLineStringCore(grid)}:::";

	/// <summary>
	/// To string with the sukaku format.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>The string.</returns>
	/// <exception cref="ArgumentException">
	/// Throws when the puzzle is an invalid sukaku puzzle (at least one cell is given or modifiable).
	/// </exception>
	private string ToSukakuString(scoped in Grid grid)
	{
		if (Multiline)
		{
			// Append all digits.
			var builders = new StringBuilder[81];
			for (int i = 0; i < 81; i++)
			{
				builders[i] = new();
				foreach (int digit in grid.GetCandidates(i))
				{
					builders[i].Append(digit + 1);
				}
			}

			// Now consider the alignment for each column of output text.
			var sb = new StringBuilder();
			scoped var span = (stackalloc int[9]);
			for (int column = 0; column < 9; column++)
			{
				int maxLength = 0;
				for (int p = 0; p < 9; p++)
				{
					maxLength = Max(maxLength, builders[p * 9 + column].Length);
				}

				span[column] = maxLength;
			}
			for (int row = 0; row < 9; row++)
			{
				for (int column = 0; column < 9; column++)
				{
					int cell = row * 9 + column;
					sb.Append(builders[cell].ToString().PadLeft(span[column])).Append(' ');
				}
				sb.RemoveFrom(^1).AppendLine(); // Remove last whitespace.
			}

			return sb.ToString();
		}
		else
		{
			var sb = new StringBuilder();
			for (int i = 0; i < 81; i++)
			{
				sb.Append("123456789");
			}

			for (int i = 0; i < 729; i++)
			{
				if (!grid[i / 9, i % 9])
				{
					sb[i] = Placeholder;
				}
			}

			return sb.ToString();
		}
	}

	/// <summary>
	/// To single line string.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>The result.</returns>
	private string ToSingleLineStringCore(scoped in Grid grid)
	{
		scoped var sb = new StringHandler(162);
		var originalGrid = WithCandidates && !ShortenSusser ? Grid.Parse($"{grid:.+}") : Grid.Undefined;

		var eliminatedCandidates = Candidates.Empty;
		for (int c = 0; c < 81; c++)
		{
			var status = grid.GetStatus(c);
			if (status == CellStatus.Empty && !originalGrid.IsUndefined && WithCandidates)
			{
				// Check if the value has been set 'true'
				// and the value has already deleted at the grid
				// with only givens and modifiables.
				foreach (int i in originalGrid.GetMask(c) & Grid.MaxCandidatesMask)
				{
					if (!grid[c, i])
					{
						// The value is 'false', which means the digit has already been deleted.
						eliminatedCandidates.Add(c * 9 + i);
					}
				}
			}

			switch (status)
			{
				case CellStatus.Empty:
				{
					sb.Append(Placeholder);
					break;
				}
				case CellStatus.Modifiable:
				{
					if (WithModifiables && !ShortenSusser)
					{
						sb.Append('+');
						sb.Append(grid[c] + 1);
					}
					else
					{
						sb.Append(Placeholder);
					}

					break;
				}
				case CellStatus.Given:
				{
					sb.Append(grid[c] + 1);
					break;
				}
				default:
				{
					throw new InvalidOperationException("The specified status is invalid.");
				}
			}
		}

		string elimsStr = EliminationNotation.ToCandidatesString(eliminatedCandidates);
		string @base = sb.ToStringAndClear();
		return ShortenSusser
			? shorten(@base, Placeholder)
			: $"{@base}{(string.IsNullOrEmpty(elimsStr) ? string.Empty : $":{elimsStr}")}";


		static unsafe string shorten(string @base, char placeholder)
		{
			scoped var resultSpan = (stackalloc char[81]);
			int index = 0;
			for (int i = 0; i < 9; i++)
			{
				string sliced = @base.Substring(i * 9, 9);
				var collection = Regex.Matches(sliced, $"{(placeholder == '.' ? @"\." : "0")}+");
				if (collection.Count == 0)
				{
					// Can't find any simplifications.
					fixed (char* p = resultSpan.Slice(i * 9, 9), q = sliced)
					{
						Unsafe.CopyBlock(p, q, sizeof(char) * 9);
					}

					index += 9;
				}
				else
				{
					var set = new HashSet<Match>(collection, new FileLocalType_MatchLengthComparer());
					if (set.Count == 1)
					{
						// All matches are same-length.
						int j = 0;
						while (j < 9)
						{
							if (sliced[j] == placeholder)
							{
								resultSpan[index++] = '*';
								j += set.First().Length;
							}
							else
							{
								resultSpan[index++] = sliced[j];
								j++;
							}
						}
					}
					else
					{
						string match = set.MaxBy(static m => m.Length)!.Value;
						int pos = sliced.IndexOf(match);
						int j = 0;
						while (j < 9)
						{
							if (j == pos)
							{
								resultSpan[index++] = '*';
								j += match.Length;
							}
							else
							{
								resultSpan[index++] = sliced[j];
								j++;
							}
						}
					}
				}

				if (i != 8)
				{
					resultSpan[index++] = ',';
				}
			}

			return resultSpan[..index].ToString();
		}
	}

	/// <summary>
	/// To multi-line string with candidates.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>The result.</returns>
	private unsafe string ToMultiLineStringCore(scoped in Grid grid)
	{
		// Step 1: gets the candidates information grouped by columns.
		Dictionary<int, List<short>> valuesByColumn = new()
		{
			{ 0, new() },
			{ 1, new() },
			{ 2, new() },
			{ 3, new() },
			{ 4, new() },
			{ 5, new() },
			{ 6, new() },
			{ 7, new() },
			{ 8, new() }
		}, valuesByRow = new()
		{
			{ 0, new() },
			{ 1, new() },
			{ 2, new() },
			{ 3, new() },
			{ 4, new() },
			{ 5, new() },
			{ 6, new() },
			{ 7, new() },
			{ 8, new() }
		};

		for (int i = 0; i < 81; i++)
		{
			short value = grid.GetMask(i);
			valuesByRow[i / 9].Add(value);
			valuesByColumn[i % 9].Add(value);
		}

		// Step 2: gets the maximal number of candidates in a cell,
		// which is used for aligning by columns.
		const int bufferLength = 9;
		int* maxLengths = stackalloc int[bufferLength];
		Unsafe.InitBlock(maxLengths, 0, sizeof(int) * bufferLength);

		foreach (var (i, _) in valuesByColumn)
		{
			int* maxLength = maxLengths + i;

			// Iteration on row index.
			for (int j = 0; j < 9; j++)
			{
				// Gets the number of candidates.
				int candidatesCount = 0;
				short value = valuesByColumn[i][j];

				// Iteration on each candidate.
				// Counts the number of candidates.
				candidatesCount += PopCount((uint)value);

				// Compares the values.
				int comparer = Max(
					candidatesCount,
					Grid.MaskToStatus(value) switch
					{
						// The output will be '<digit>' and consist of 3 characters.
						CellStatus.Given => Max(candidatesCount, 3),
						// The output will be '*digit*' and consist of 3 characters.
						CellStatus.Modifiable => Max(candidatesCount, 3),
						// Normal output: 'series' (at least 1 character).
						_ => candidatesCount,
					}
				);
				if (comparer > *maxLength)
				{
					*maxLength = comparer;
				}
			}
		}

		// Step 3: outputs all characters.
		scoped var sb = new StringHandler();
		for (int i = 0; i < 13; i++)
		{
			switch (i)
			{
				case 0: // Print tabs of the first line.
				{
					if (SubtleGridLines)
					{
						printTabLines(ref sb, '.', '.', '-', maxLengths);
					}
					else
					{
						printTabLines(ref sb, '+', '+', '-', maxLengths);
					}
					break;
				}
				case 4:
				case 8: // Print tabs of mediate lines.
				{
					if (SubtleGridLines)
					{
						printTabLines(ref sb, ':', '+', '-', maxLengths);
					}
					else
					{
						printTabLines(ref sb, '+', '+', '-', maxLengths);
					}
					break;
				}
				case 12: // Print tabs of the foot line.
				{
					if (SubtleGridLines)
					{
						printTabLines(ref sb, '\'', '\'', '-', maxLengths);
					}
					else
					{
						printTabLines(ref sb, '+', '+', '-', maxLengths);
					}
					break;
				}
				default: // Print values and tabs.
				{
					p(this, ref sb, valuesByRow[A057353(i)], '|', '|', maxLengths);

					break;


					static void p(
						scoped in FileLocalType_GridFormatter formatter, scoped ref scoped StringHandler sb,
						IList<short> valuesByRow, char c1, char c2, int* maxLengths)
					{
						sb.Append(c1);
						printValues(formatter, ref sb, valuesByRow, 0, 2, maxLengths);
						sb.Append(c2);
						printValues(formatter, ref sb, valuesByRow, 3, 5, maxLengths);
						sb.Append(c2);
						printValues(formatter, ref sb, valuesByRow, 6, 8, maxLengths);
						sb.Append(c1);
						sb.AppendLine();


						static void printValues(
							scoped in FileLocalType_GridFormatter formatter,
							scoped ref scoped StringHandler sb, IList<short> valuesByRow,
							int start, int end, int* maxLengths)
						{
							sb.Append(' ');
							for (int i = start; i <= end; i++)
							{
								// Get digit.
								short value = valuesByRow[i];
								var status = Grid.MaskToStatus(value);

								value &= Grid.MaxCandidatesMask;
								int d = value == 0
									? -1
									: (status != CellStatus.Empty ? TrailingZeroCount(value) : -1) + 1;
								string s;
								switch (status)
								{
									case CellStatus.Given:
									case CellStatus.Modifiable when formatter.TreatValueAsGiven:
									{
										s = $"<{d}>";
										break;
									}
									case CellStatus.Modifiable:
									{
										s = $"*{d}*";
										break;
									}
									default:
									{
										var innerSb = new StringHandler(9);
										foreach (int z in value)
										{
											innerSb.Append(z + 1);
										}

										s = innerSb.ToStringAndClear();

										break;
									}
								}

								sb.Append(s.PadRight(maxLengths[i]));
								sb.Append(i != end ? "  " : " ");
							}
						}
					}
				}
			}
		}

		// The last step: returns the value.
		return sb.ToString();


		static void printTabLines(scoped ref scoped StringHandler sb, char c1, char c2, char fillingChar, int* m)
		{
			sb.Append(c1);
			sb.Append(string.Empty.PadRight(m[0] + m[1] + m[2] + 6, fillingChar));
			sb.Append(c2);
			sb.Append(string.Empty.PadRight(m[3] + m[4] + m[5] + 6, fillingChar));
			sb.Append(c2);
			sb.Append(string.Empty.PadRight(m[6] + m[7] + m[8] + 6, fillingChar));
			sb.Append(c1);
			sb.AppendLine();
		}
	}

	/// <summary>
	/// To multi-line normal grid string without any candidates.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>The result.</returns>
	private string ToMultiLineSimpleGridCore(scoped in Grid grid)
	{
		string t = grid.ToString(TreatValueAsGiven ? $"{Placeholder}!" : Placeholder.ToString());
		return new StringBuilder()
			.AppendLine(SubtleGridLines ? ".-------+-------+-------." : "+-------+-------+-------+")
			.Append("| ").Append(t[0]).Append(' ').Append(t[1]).Append(' ').Append(t[2])
			.Append(" | ").Append(t[3]).Append(' ').Append(t[4]).Append(' ').Append(t[5])
			.Append(" | ").Append(t[6]).Append(' ').Append(t[7]).Append(' ').Append(t[8])
			.AppendLine(" |")
			.Append("| ").Append(t[9]).Append(' ').Append(t[10]).Append(' ').Append(t[11])
			.Append(" | ").Append(t[12]).Append(' ').Append(t[13]).Append(' ').Append(t[14])
			.Append(" | ").Append(t[15]).Append(' ').Append(t[16]).Append(' ').Append(t[17])
			.AppendLine(" |")
			.Append("| ").Append(t[18]).Append(' ').Append(t[19]).Append(' ').Append(t[20])
			.Append(" | ").Append(t[21]).Append(' ').Append(t[22]).Append(' ').Append(t[23])
			.Append(" | ").Append(t[24]).Append(' ').Append(t[25]).Append(' ').Append(t[26])
			.AppendLine(" |")
			.AppendLine(SubtleGridLines ? ":-------+-------+-------:" : "+-------+-------+-------+")
			.Append("| ").Append(t[27]).Append(' ').Append(t[28]).Append(' ').Append(t[29])
			.Append(" | ").Append(t[30]).Append(' ').Append(t[31]).Append(' ').Append(t[32])
			.Append(" | ").Append(t[33]).Append(' ').Append(t[34]).Append(' ').Append(t[35])
			.AppendLine(" |")
			.Append("| ").Append(t[36]).Append(' ').Append(t[37]).Append(' ').Append(t[38])
			.Append(" | ").Append(t[39]).Append(' ').Append(t[40]).Append(' ').Append(t[41])
			.Append(" | ").Append(t[42]).Append(' ').Append(t[43]).Append(' ').Append(t[44])
			.AppendLine(" |")
			.Append("| ").Append(t[45]).Append(' ').Append(t[46]).Append(' ').Append(t[47])
			.Append(" | ").Append(t[48]).Append(' ').Append(t[49]).Append(' ').Append(t[50])
			.Append(" | ").Append(t[51]).Append(' ').Append(t[52]).Append(' ').Append(t[53])
			.AppendLine(" |")
			.AppendLine(SubtleGridLines ? ":-------+-------+-------:" : "+-------+-------+-------+")
			.Append("| ").Append(t[54]).Append(' ').Append(t[55]).Append(' ').Append(t[56])
			.Append(" | ").Append(t[57]).Append(' ').Append(t[58]).Append(' ').Append(t[59])
			.Append(" | ").Append(t[60]).Append(' ').Append(t[61]).Append(' ').Append(t[62])
			.AppendLine(" |")
			.Append("| ").Append(t[63]).Append(' ').Append(t[64]).Append(' ').Append(t[65])
			.Append(" | ").Append(t[66]).Append(' ').Append(t[67]).Append(' ').Append(t[68])
			.Append(" | ").Append(t[69]).Append(' ').Append(t[70]).Append(' ').Append(t[71])
			.AppendLine(" |")
			.Append("| ").Append(t[72]).Append(' ').Append(t[73]).Append(' ').Append(t[74])
			.Append(" | ").Append(t[75]).Append(' ').Append(t[76]).Append(' ').Append(t[77])
			.Append(" | ").Append(t[78]).Append(' ').Append(t[79]).Append(' ').Append(t[80])
			.AppendLine(" |")
			.AppendLine(SubtleGridLines ? "'-------+-------+-------'" : "+-------+-------+-------+")
			.ToString();
	}


	/// <summary>
	/// Create a <see cref="FileLocalType_GridFormatter"/> according to the specified grid output options.
	/// </summary>
	/// <param name="gridOutputOption">The grid output options.</param>
	/// <returns>The grid formatter.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static FileLocalType_GridFormatter Create(GridFormattingOptions gridOutputOption)
		=> gridOutputOption switch
		{
			GridFormattingOptions.Excel => new(true) { Excel = true },
			GridFormattingOptions.OpenSudoku => new(false) { OpenSudoku = true },
			_ => new(gridOutputOption.Flags(GridFormattingOptions.Multiline))
			{
				WithModifiables = gridOutputOption.Flags(GridFormattingOptions.WithModifiers),
				WithCandidates = gridOutputOption.Flags(GridFormattingOptions.WithCandidates),
				ShortenSusser = gridOutputOption.Flags(GridFormattingOptions.Shorten),
				TreatValueAsGiven = gridOutputOption.Flags(GridFormattingOptions.TreatValueAsGiven),
				SubtleGridLines = gridOutputOption.Flags(GridFormattingOptions.SubtleGridLines),
				HodokuCompatible = gridOutputOption.Flags(GridFormattingOptions.HodokuCompatible),
				Sukaku = gridOutputOption == GridFormattingOptions.Sukaku,
				Placeholder = gridOutputOption.Flags(GridFormattingOptions.DotPlaceholder) ? '.' : '0'
			}
		};
}

/// <summary>
/// Encapsulates a grid parser that can parse a string value and convert it
/// into a valid <see cref="Grid"/> instance as the result.
/// </summary>
public unsafe ref partial struct GridParser
{
	/// <summary>
	/// The list of all methods to parse.
	/// </summary>
	private static readonly delegate*<ref GridParser, Grid>[] ParseFunctions;

	/// <summary>
	/// The list of all methods to parse multiple-line grid.
	/// </summary>
	private static readonly delegate*<ref GridParser, Grid>[] MultilineParseFunctions;


	/// <summary>
	/// Initializes an instance with parsing data.
	/// </summary>
	/// <param name="parsingValue">The string to parse.</param>
	public GridParser(string parsingValue) : this(parsingValue, false, false)
	{
	}

	/// <summary>
	/// Initializes an instance with parsing data and a bool value
	/// indicating whether the parsing operation should use compatible mode.
	/// </summary>
	/// <param name="parsingValue">The string to parse.</param>
	/// <param name="compatibleFirst">
	/// Indicates whether the parsing operation should use compatible mode to check
	/// PM grid. See <see cref="CompatibleFirst"/> to learn more.
	/// </param>
	/// <seealso cref="CompatibleFirst"/>
	public GridParser(string parsingValue, bool compatibleFirst) : this(parsingValue, compatibleFirst, false)
	{
	}

	/// <summary>
	/// Initializes an instance with parsing data and a bool value
	/// indicating whether the parsing operation should use compatible mode.
	/// </summary>
	/// <param name="parsingValue">The string to parse.</param>
	/// <param name="compatibleFirst">
	/// Indicates whether the parsing operation should use compatible mode to check
	/// PM grid. See <see cref="CompatibleFirst"/> to learn more.
	/// </param>
	/// <param name="shortenSusser">Indicates the parser will shorten the susser format result.</param>
	/// <seealso cref="CompatibleFirst"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public GridParser(string parsingValue, bool compatibleFirst, bool shortenSusser)
		=> (ParsingValue, CompatibleFirst, ShortenSusserFormat) = (parsingValue, compatibleFirst, shortenSusser);


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static GridParser()
	{
		ParseFunctions = new delegate*<ref GridParser, Grid>[]
		{
			&OnParsingSimpleTable,
			&OnParsingSimpleMultilineGrid,
			&OnParsingPencilMarked,
			&onParsingSusser_1,
			&onParsingSusser_2,
			&OnParsingExcel,
			&OnParsingOpenSudoku,
			&onParsingSukaku_1,
			&onParsingSukaku_2
		};

		// Bug fix for GitHub issue #216: Cannot apply Range syntax '1..3' onto pointer-typed array.
		// In other words, the following code will always cause an error on AnyCPU.
		//MultilineParseFunctions = ParseFunctions[1..3];
		MultilineParseFunctions = new delegate*<ref GridParser, Grid>[]
		{
			&OnParsingSimpleMultilineGrid,
			&OnParsingPencilMarked
		};

		static Grid onParsingSukaku_1(ref GridParser @this) => OnParsingSukaku(ref @this, @this.CompatibleFirst);
		static Grid onParsingSukaku_2(ref GridParser @this) => OnParsingSukaku(ref @this, !@this.CompatibleFirst);
		static Grid onParsingSusser_1(ref GridParser @this) => OnParsingSusser(ref @this, !@this.ShortenSusserFormat);
		static Grid onParsingSusser_2(ref GridParser @this) => OnParsingSusser(ref @this, @this.ShortenSusserFormat);
	}


	/// <summary>
	/// The string value to parse.
	/// </summary>
	public string ParsingValue { get; private set; }

	/// <summary>
	/// Indicates whether the parser will change the execution order of PM grid.
	/// If the value is <see langword="true"/>, the parser will check compatible one
	/// first, and then check recommended parsing plan ('<c><![CDATA[<d>]]></c>' and '<c>*d*</c>').
	/// </summary>
	public bool CompatibleFirst { get; }

	/// <summary>
	/// Indicates whether the parser will use shorten mode to parse a susser format grid.
	/// If the value is <see langword="true"/>, the parser will omit the continuous empty notation
	/// <c>.</c>s or <c>0</c>s to a <c>*</c>.
	/// </summary>
	public bool ShortenSusserFormat { get; private set; }


	/// <summary>
	/// To parse the value.
	/// </summary>
	/// <returns>The grid.</returns>
	public Grid Parse()
	{
		if (ParsingValue.Length == 729)
		{
			if (OnParsingExcel(ref this) is { IsUndefined: false } grid)
			{
				return grid;
			}
		}
		else if (ParsingValue.Contains("-+-"))
		{
			foreach (var parseMethod in MultilineParseFunctions)
			{
				if (parseMethod(ref this) is { IsUndefined: false } grid)
				{
					return grid;
				}
			}
		}
		else if (ParsingValue.Contains('\t'))
		{
			if (OnParsingExcel(ref this) is { IsUndefined: false } grid)
			{
				return grid;
			}
		}
		else
		{
			for (int trial = 0, length = ParseFunctions.Length; trial < length; trial++)
			{
				if (ParseFunctions[trial](ref this) is { IsUndefined: false } grid)
				{
					return grid;
				}
			}
		}

		return Grid.Undefined;
	}

	/// <summary>
	/// To parse the value with a specified grid parsing type.
	/// </summary>
	/// <param name="gridParsingOption">A specified parsing type.</param>
	/// <returns>The grid.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="gridParsingOption"/> is not defined.
	/// </exception>
	public Grid Parse(GridParsingOption gridParsingOption)
		=> gridParsingOption switch
		{
			GridParsingOption.Susser => OnParsingSusser(ref this, false),
			GridParsingOption.ShortenSusser => OnParsingSusser(ref this, true),
			GridParsingOption.Table => OnParsingSimpleMultilineGrid(ref this),
			GridParsingOption.PencilMarked => OnParsingPencilMarked(ref this),
			GridParsingOption.SimpleTable => OnParsingSimpleTable(ref this),
			GridParsingOption.Sukaku => OnParsingSukaku(ref this, compatibleFirst: false),
			GridParsingOption.SukakuSingleLine => OnParsingSukaku(ref this, compatibleFirst: true),
			GridParsingOption.Excel => OnParsingExcel(ref this),
			GridParsingOption.OpenSudoku => OnParsingOpenSudoku(ref this),
			_ => throw new ArgumentOutOfRangeException(nameof(gridParsingOption))
		};


	/// <summary>
	/// Parse the value using multi-line simple grid (without any candidates).
	/// </summary>
	/// <param name="parser">The parser.</param>
	/// <returns>The result.</returns>
	private static Grid OnParsingSimpleMultilineGrid(ref GridParser parser)
	{
		string[] matches = parser.ParsingValue.MatchAll("""(\+?\d|\.)""");
		int length = matches.Length;
		if (length is not (81 or 85))
		{
			// Subtle grid outline will bring 2 '.'s on first line of the grid.
			return Grid.Undefined;
		}

		var result = Grid.Empty;
		for (int i = 0; i < 81; i++)
		{
			string currentMatch = matches[length - 81 + i];
			switch (currentMatch)
			{
				case [var match and not ('.' or '0')]:
				{
					result[i] = match - '1';
					result.SetStatus(i, CellStatus.Given);

					break;
				}
				case [_]:
				{
					continue;
				}
				case [_, var match]:
				{
					if (match is '.' or '0')
					{
						// '+0' or '+.'? Invalid combination.
						return Grid.Undefined;
					}
					else
					{
						result[i] = match - '1';
						result.SetStatus(i, CellStatus.Modifiable);
					}

					break;
				}
				default:
				{
					// The sub-match contains more than 2 characters or empty string,
					// which is invalid.
					return Grid.Undefined;
				}
			}
		}

		return result;
	}

	/// <summary>
	/// Parse the Excel format.
	/// </summary>
	/// <param name="parser">The parser.</param>
	/// <returns>The result.</returns>
	private static Grid OnParsingExcel(ref GridParser parser)
	{
		string parsingValue = parser.ParsingValue;
		if (!parsingValue.Contains('\t'))
		{
			return Grid.Undefined;
		}

		string[] values = parsingValue.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
		if (values.Length != 9)
		{
			return Grid.Undefined;
		}

		scoped var sb = new StringHandler(81);
		foreach (string value in values)
		{
			foreach (string digitString in value.Split(new[] { '\t' }))
			{
				sb.Append(string.IsNullOrEmpty(digitString) ? '.' : digitString[0]);
			}
		}

		return Grid.Parse(sb.ToStringAndClear());
	}

	/// <summary>
	/// Parse the open sudoku format grid.
	/// </summary>
	/// <param name="parser">The parser.</param>
	/// <returns>The result.</returns>
	private static Grid OnParsingOpenSudoku(ref GridParser parser)
	{
		if (parser.ParsingValue.Match("""\d(\|\d){242}""") is not { } match)
		{
			return Grid.Undefined;
		}

		var result = Grid.Empty;
		for (int i = 0; i < 81; i++)
		{
			switch (match[i * 6])
			{
				case '0' when whenClause(i * 6, match, "|0|1", "|0|1|"):
				{
					continue;
				}
				case not '0' and var ch when whenClause(i * 6, match, "|0|0", "|0|0|"):
				{
					result[i] = ch - '1';
					result.SetStatus(i, CellStatus.Given);

					break;
				}
				default:
				{
					// Invalid string status.
					return Grid.Undefined;
				}
			}
		}

		return result;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool whenClause(int i, string match, string pattern1, string pattern2)
			=> i == 80 * 6 ? match[(i + 1)..(i + 5)] == pattern1 : match[(i + 1)..(i + 6)] == pattern2;
	}

	/// <summary>
	/// Parse the PM grid.
	/// </summary>
	/// <param name="parser">The parser.</param>
	/// <returns>The result.</returns>
	private static Grid OnParsingPencilMarked(ref GridParser parser)
	{
		// Older regular expression pattern:
		if (parser.ParsingValue.MatchAll("""(\<\d\>|\*\d\*|\d*[\+\-]?\d+)""") is not { Length: 81 } matches)
		{
			return Grid.Undefined;
		}

		var result = Grid.Empty;
		for (int cell = 0; cell < 81; cell++)
		{
			if (matches[cell] is not { Length: var length and <= 9 } s)
			{
				// More than 9 characters.
				return Grid.Undefined;
			}

			if (s.Contains('<'))
			{
				// All values will be treated as normal characters:
				// '<digit>', '*digit*' and 'candidates'.

				// Givens.
				if (length == 3)
				{
					if (s[1] is var c and >= '1' and <= '9')
					{
						result[cell] = c - '1';
						result.SetStatus(cell, CellStatus.Given);
					}
					else
					{
						// Illegal characters found.
						return Grid.Undefined;
					}
				}
				else
				{
					// The length is not 3.
					return Grid.Undefined;
				}
			}
			else if (s.Contains('*'))
			{
				// Modifiables.
				if (length == 3)
				{
					if (s[1] is var c and >= '1' and <= '9')
					{
						result[cell] = c - '1';
						result.SetStatus(cell, CellStatus.Modifiable);
					}
					else
					{
						// Illegal characters found.
						return Grid.Undefined;
					}
				}
				else
				{
					// The length is not 3.
					return Grid.Undefined;
				}
			}
			else if (s.SatisfyPattern("""[1-9]{1,9}"""))
			{
				// Candidates.
				// Here don't need to check the length of the string,
				// and also all characters are digit characters.
				short mask = 0;
				foreach (char c in s)
				{
					mask |= (short)(1 << c - '1');
				}

				if (mask == 0)
				{
					return Grid.Undefined;
				}

				if ((mask & mask - 1) == 0)
				{
					result[cell] = TrailingZeroCount(mask);
					result.SetStatus(cell, CellStatus.Given);
				}
				else
				{
					for (int digit = 0; digit < 9; digit++)
					{
						result[cell, digit] = (mask >> digit & 1) != 0;
					}
				}
			}
			else
			{
				// All conditions can't match.
				return Grid.Undefined;
			}
		}

		return result;
	}

	/// <summary>
	/// Parse the simple table format string (Sudoku explainer format).
	/// </summary>
	/// <param name="parser">The parser.</param>
	/// <returns>The grid.</returns>
	private static Grid OnParsingSimpleTable(ref GridParser parser)
	{
		if (parser.ParsingValue.Match("""([\d\.\+]{9}(\r|\n|\r\n)){8}[\d\.\+]{9}""") is not { } match)
		{
			return Grid.Undefined;
		}

		// Remove all '\r's and '\n's.
		scoped var sb = new StringHandler(81 + (9 << 1));
		sb.AppendCharacters(from @char in match where @char is not ('\r' or '\n') select @char);
		parser.ParsingValue = sb.ToStringAndClear();
		return OnParsingSusser(ref parser, false);
	}

	/// <summary>
	/// Parse the susser format string.
	/// </summary>
	/// <param name="parser">The parser.</param>
	/// <param name="shortenSusser">Indicates whether the parser will shorten the susser format.</param>
	/// <returns>The result.</returns>
	private static Grid OnParsingSusser(ref GridParser parser, bool shortenSusser)
	{
		string? match = shortenSusser
			? parser.ParsingValue.Match("""[\d\.\*]{1,9}(,[\d\.\*]{1,9}){8}""")
			: parser.ParsingValue.Match("""[\d\.\+]{80,}(\:(\d{3}\s+)*\d{3})?""");

		switch (shortenSusser)
		{
			case false when match is not { Length: <= 405 }:
			case true when match is not { Length: <= 81 } || !expandCode(match, out match):
			{
				return Grid.Undefined;
			}
		}

		// Step 1: fills all digits.
		var result = Grid.Empty;
		int i = 0, length = match.Length;
		for (int realPos = 0; i < length && match[i] != ':'; realPos++)
		{
			char c = match[i];
			switch (c)
			{
				case '+':
				{
					// Plus sign means the character after it is a digit,
					// which is modifiable value in the grid in its corresponding position.
					if (i < length - 1)
					{
						if (match[i + 1] is var nextChar and >= '1' and <= '9')
						{
							// Set value.
							// Note that the subtractor is character '1', not '0'.
							result[realPos] = nextChar - '1';

							// Add 2 on iteration variable to skip 2 characters
							// (A plus sign '+' and a digit).
							i += 2;
						}
						else
						{
							// Why isn't the character a digit character?
							return Grid.Undefined;
						}
					}
					else
					{
						return Grid.Undefined;
					}

					break;
				}
				case '.':
				case '0':
				{
					// A placeholder.
					// Do nothing but only move 1 step forward.
					i++;

					break;
				}
				case >= '1' and <= '9':
				{
					// Is a digit character.
					// Digits are representing given values in the grid.
					// Not the plus sign, but a placeholder '0' or '.'.
					// Set value.
					result[realPos] = c - '1';

					// Set the cell status as 'CellStatus.Given'.
					// If the code below doesn't make sense to you,
					// you can see the comments in method 'OnParsingSusser(string)'
					// to know the meaning also.
					result.SetStatus(realPos, CellStatus.Given);

					// Finally moves 1 step forward.
					i++;

					break;
				}
				default:
				{
					// Other invalid characters. Throws an exception.
					//throw Throwing.ParsingError<Grid>(nameof(ParsingValue));
					return Grid.Undefined;
				}
			}
		}

		// Step 2: eliminates candidates if exist.
		// If we have met the colon sign ':', this loop would not be executed.
		if (Grid.ExtendedSusserEliminationsRegex().Match(match) is { Success: true, Value: var elimMatch })
		{
			foreach (int candidate in EliminationNotation.ParseCandidates(elimMatch))
			{
				// Set the candidate with false to eliminate the candidate.
				result[candidate / 9, candidate % 9] = false;
			}
		}

		return result;


		static bool expandCode(string? original, [NotNullWhen(true)] out string? result)
		{
			// We must the string code holds 8 ','s and is with no ':' or '+'.
			if (original is null || original.Contains(':') || original.Contains('+') || original.CountOf(',') != 8)
			{
				result = null;
				return false;
			}

			scoped var resultSpan = (stackalloc char[81]);
			string[] lines = original.Split(',');
			if (lines.Length != 9)
			{
				result = null;
				return false;
			}

			// Check per line, and expand it.
			char placeholder = original.IndexOf('0') == -1 ? '.' : '0';
			for (int i = 0; i < 9; i++)
			{
				string line = lines[i];
				switch (line.CountOf('*'))
				{
					case 1 when (9 + 1 - line.Length, 0, 0) is var (empties, j, k):
					{
						foreach (char c in line)
						{
							if (c == '*')
							{
								resultSpan.Slice(i * 9 + k, empties).Fill(placeholder);

								j++;
								k += empties;
							}
							else
							{
								resultSpan[i * 9 + k] = line[j];

								j++;
								k++;
							}
						}

						break;
					}

					case var n when (9 + n - line.Length, 0, 0) is var (empties, j, k):
					{
						int emptiesPerStar = empties / n;
						foreach (char c in line)
						{
							if (c == '*')
							{
								resultSpan.Slice(i * 9 + k, emptiesPerStar).Fill(placeholder);

								j++;
								k += emptiesPerStar;
							}
							else
							{
								resultSpan[i * 9 + k] = line[j];

								j++;
								k++;
							}
						}

						break;
					}
				}
			}

			result = resultSpan.ToString();
			return true;
		}
	}

	/// <summary>
	/// Parse the sukaku format string.
	/// </summary>
	/// <param name="parser">The parser.</param>
	/// <param name="compatibleFirst">
	/// Indicates whether the algorithm uses compatibility mode to check and parse sudoku grid.
	/// </param>
	/// <returns>The result.</returns>
	private static Grid OnParsingSukaku(ref GridParser parser, bool compatibleFirst)
	{
		const int candidatesCount = 729;
		if (compatibleFirst)
		{
			string parsingValue = parser.ParsingValue;
			if (parsingValue.Length < candidatesCount)
			{
				return Grid.Undefined;
			}

			var result = Grid.Empty;
			for (int i = 0; i < candidatesCount; i++)
			{
				char c = parsingValue[i];
				if (c is not (>= '0' and <= '9' or '.'))
				{
					return Grid.Undefined;
				}

				if (c is '0' or '.')
				{
					result[i / 9, i % 9] = false;
				}
			}

			return result;
		}
		else
		{
			string[] matches = parser.ParsingValue.MatchAll("""\d*[\-\+]?\d+""");
			if (matches is { Length: not 81 })
			{
				return Grid.Undefined;
			}

			var result = Grid.Empty;
			for (int offset = 0; offset < 81; offset++)
			{
				string s = matches[offset].Reserve(@"\d");
				if (s.Length > 9)
				{
					// More than 9 characters.
					return Grid.Undefined;
				}

				short mask = 0;
				foreach (char c in s)
				{
					mask |= (short)(1 << c - '1');
				}

				if (mask == 0)
				{
					return Grid.Undefined;
				}

				// We don't need to set the value as a given because the current parsing
				// if for sukakus, rather than normal sudokus.
				//if (IsPow2(mask))
				//{
				//	result[offset] = TrailingZeroCount(mask);
				//	result.SetStatus(offset, CellStatus.Given);
				//}

				for (int digit = 0; digit < 9; digit++)
				{
					result[offset, digit] = (mask >> digit & 1) != 0;
				}
			}

			return result;
		}
	}
}

/// <summary>
/// Encapsulates a grid parser that can parse a <see cref="Utf8String"/> value and convert it
/// into a valid <see cref="Grid"/> instance as the result.
/// </summary>
internal unsafe ref partial struct FileLocalType_Utf8GridParser
{
	/// <summary>
	/// The list of all methods to parse.
	/// </summary>
	private static readonly delegate*<ref FileLocalType_Utf8GridParser, Grid>[] ParseFunctions;

	/// <summary>
	/// The list of all methods to parse multiple-line grid.
	/// </summary>
	private static readonly delegate*<ref FileLocalType_Utf8GridParser, Grid>[] MultilineParseFunctions;


	/// <summary>
	/// Initializes an instance with parsing data.
	/// </summary>
	/// <param name="parsingValue">The string to parse.</param>
	public FileLocalType_Utf8GridParser(Utf8String parsingValue) : this(parsingValue, false, false)
	{
	}

	/// <summary>
	/// Initializes an instance with parsing data and a bool value
	/// indicating whether the parsing operation should use compatible mode.
	/// </summary>
	/// <param name="parsingValue">The string to parse.</param>
	/// <param name="compatibleFirst">
	/// Indicates whether the parsing operation should use compatible mode to check
	/// PM grid. See <see cref="CompatibleFirst"/> to learn more.
	/// </param>
	/// <seealso cref="CompatibleFirst"/>
	public FileLocalType_Utf8GridParser(Utf8String parsingValue, bool compatibleFirst) : this(parsingValue, compatibleFirst, false)
	{
	}

	/// <summary>
	/// Initializes an instance with parsing data and a bool value
	/// indicating whether the parsing operation should use compatible mode.
	/// </summary>
	/// <param name="parsingValue">The string to parse.</param>
	/// <param name="compatibleFirst">
	/// Indicates whether the parsing operation should use compatible mode to check
	/// PM grid. See <see cref="CompatibleFirst"/> to learn more.
	/// </param>
	/// <param name="shortenSusser">Indicates the parser will shorten the susser format result.</param>
	/// <seealso cref="CompatibleFirst"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public FileLocalType_Utf8GridParser(Utf8String parsingValue, bool compatibleFirst, bool shortenSusser)
		=> (ParsingValue, CompatibleFirst, ShortenSusserFormat) = (parsingValue, compatibleFirst, shortenSusser);


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static FileLocalType_Utf8GridParser()
	{
		ParseFunctions = new delegate*<ref FileLocalType_Utf8GridParser, Grid>[]
		{
			&OnParsingSimpleTable,
			&OnParsingSimpleMultilineGrid,
			&OnParsingPencilMarked,
			&onParsingSusser_1,
			&onParsingSusser_2,
			&OnParsingExcel,
			&OnParsingOpenSudoku,
			&onParsingSukaku_1,
			&onParsingSukaku_2
		};

		// Bug fix for GitHub issue #216: Cannot apply Range syntax '1..3' onto pointer-typed array.
		// In other words, the following code will always cause an error on AnyCPU.
		//MultilineParseFunctions = ParseFunctions[1..3];
		MultilineParseFunctions = new delegate*<ref FileLocalType_Utf8GridParser, Grid>[]
		{
			&OnParsingSimpleMultilineGrid,
			&OnParsingPencilMarked
		};

		static Grid onParsingSukaku_1(ref FileLocalType_Utf8GridParser @this) => OnParsingSukaku(ref @this, @this.CompatibleFirst);
		static Grid onParsingSukaku_2(ref FileLocalType_Utf8GridParser @this) => OnParsingSukaku(ref @this, !@this.CompatibleFirst);
		static Grid onParsingSusser_1(ref FileLocalType_Utf8GridParser @this) => OnParsingSusser(ref @this, !@this.ShortenSusserFormat);
		static Grid onParsingSusser_2(ref FileLocalType_Utf8GridParser @this) => OnParsingSusser(ref @this, @this.ShortenSusserFormat);
	}


	/// <summary>
	/// The string value to parse.
	/// </summary>
	public Utf8String ParsingValue { get; private set; }

	/// <summary>
	/// Indicates whether the parser will change the execution order of PM grid.
	/// If the value is <see langword="true"/>, the parser will check compatible one
	/// first, and then check recommended parsing plan ('<c><![CDATA[<d>]]></c>' and '<c>*d*</c>').
	/// </summary>
	public bool CompatibleFirst { get; }

	/// <summary>
	/// Indicates whether the parser will use shorten mode to parse a susser format grid.
	/// If the value is <see langword="true"/>, the parser will omit the continuous empty notation
	/// <c>.</c>s or <c>0</c>s to a <c>*</c>.
	/// </summary>
	public bool ShortenSusserFormat { get; private set; }


	/// <summary>
	/// To parse the value.
	/// </summary>
	/// <returns>The grid.</returns>
	public Grid Parse()
	{
		if (ParsingValue.Length == 729)
		{
			if (OnParsingExcel(ref this) is { IsUndefined: false } grid)
			{
				return grid;
			}
		}
		else if (ParsingValue.Contains("-+-"U8))
		{
			foreach (var parseMethod in MultilineParseFunctions)
			{
				if (parseMethod(ref this) is { IsUndefined: false } grid)
				{
					return grid;
				}
			}
		}
		else if (ParsingValue.Contains((Utf8Char)'\t'))
		{
			if (OnParsingExcel(ref this) is { IsUndefined: false } grid)
			{
				return grid;
			}
		}
		else
		{
			for (int trial = 0, length = ParseFunctions.Length; trial < length; trial++)
			{
				if (ParseFunctions[trial](ref this) is { IsUndefined: false } grid)
				{
					return grid;
				}
			}
		}

		return Grid.Undefined;
	}

	/// <summary>
	/// To parse the value with a specified grid parsing type.
	/// </summary>
	/// <param name="gridParsingOption">A specified parsing type.</param>
	/// <returns>The grid.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="gridParsingOption"/> is not defined.
	/// </exception>
	public Grid Parse(GridParsingOption gridParsingOption)
		=> gridParsingOption switch
		{
			GridParsingOption.Susser => OnParsingSusser(ref this, false),
			GridParsingOption.ShortenSusser => OnParsingSusser(ref this, true),
			GridParsingOption.Table => OnParsingSimpleMultilineGrid(ref this),
			GridParsingOption.PencilMarked => OnParsingPencilMarked(ref this),
			GridParsingOption.SimpleTable => OnParsingSimpleTable(ref this),
			GridParsingOption.Sukaku => OnParsingSukaku(ref this, compatibleFirst: false),
			GridParsingOption.SukakuSingleLine => OnParsingSukaku(ref this, compatibleFirst: true),
			GridParsingOption.Excel => OnParsingExcel(ref this),
			GridParsingOption.OpenSudoku => OnParsingOpenSudoku(ref this),
			_ => throw new ArgumentOutOfRangeException(nameof(gridParsingOption))
		};


	/// <summary>
	/// Parse the value using multi-line simple grid (without any candidates).
	/// </summary>
	/// <param name="parser">The parser.</param>
	/// <returns>The result.</returns>
	private static Grid OnParsingSimpleMultilineGrid(ref FileLocalType_Utf8GridParser parser)
	{
		string[] matches = parser.ParsingValue.MatchAll("""(\+?\d|\.)"""U8);
		int length = matches.Length;
		if (length is not (81 or 85))
		{
			// Subtle grid outline will bring 2 '.'s on first line of the grid.
			return Grid.Undefined;
		}

		var result = Grid.Empty;
		for (int i = 0; i < 81; i++)
		{
			string currentMatch = matches[length - 81 + i];
			switch (currentMatch)
			{
				case [var match and not ('.' or '0')]:
				{
					result[i] = match - '1';
					result.SetStatus(i, CellStatus.Given);

					break;
				}
				case [_]:
				{
					continue;
				}
				case [_, var match]:
				{
					if (match is '.' or '0')
					{
						// '+0' or '+.'? Invalid combination.
						return Grid.Undefined;
					}
					else
					{
						result[i] = match - '1';
						result.SetStatus(i, CellStatus.Modifiable);
					}

					break;
				}
				default:
				{
					// The sub-match contains more than 2 characters or empty string,
					// which is invalid.
					return Grid.Undefined;
				}
			}
		}

		return result;
	}

	/// <summary>
	/// Parse the Excel format.
	/// </summary>
	/// <param name="parser">The parser.</param>
	/// <returns>The result.</returns>
	private static Grid OnParsingExcel(ref FileLocalType_Utf8GridParser parser)
	{
		var parsingValue = parser.ParsingValue;
		if (!parsingValue.Contains((Utf8Char)'\t'))
		{
			return Grid.Undefined;
		}

		string[] values = ((string)parsingValue).Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
		if (values.Length != 9)
		{
			return Grid.Undefined;
		}

		scoped var sb = new StringHandler(81);
		foreach (string value in values)
		{
			foreach (string digitString in value.Split(new[] { '\t' }))
			{
				sb.Append(string.IsNullOrEmpty(digitString) ? '.' : digitString[0]);
			}
		}

		return Grid.Parse(sb.ToStringAndClear());
	}

	/// <summary>
	/// Parse the open sudoku format grid.
	/// </summary>
	/// <param name="parser">The parser.</param>
	/// <returns>The result.</returns>
	private static Grid OnParsingOpenSudoku(ref FileLocalType_Utf8GridParser parser)
	{
		if (parser.ParsingValue.Match("""\d(\|\d){242}"""U8) is not { } match)
		{
			return Grid.Undefined;
		}

		var result = Grid.Empty;
		for (int i = 0; i < 81; i++)
		{
			switch (match[i * 6])
			{
				case '0' when whenClause(i * 6, match, "|0|1", "|0|1|"):
				{
					continue;
				}
				case not '0' and var ch when whenClause(i * 6, match, "|0|0", "|0|0|"):
				{
					result[i] = ch - '1';
					result.SetStatus(i, CellStatus.Given);

					break;
				}
				default:
				{
					// Invalid string status.
					return Grid.Undefined;
				}
			}
		}

		return result;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool whenClause(int i, string match, string pattern1, string pattern2)
			=> i == 80 * 6 ? match[(i + 1)..(i + 5)] == pattern1 : match[(i + 1)..(i + 6)] == pattern2;
	}

	/// <summary>
	/// Parse the PM grid.
	/// </summary>
	/// <param name="parser">The parser.</param>
	/// <returns>The result.</returns>
	private static Grid OnParsingPencilMarked(ref FileLocalType_Utf8GridParser parser)
	{
		// Older regular expression pattern:
		if (parser.ParsingValue.MatchAll("""(\<\d\>|\*\d\*|\d*[\+\-]?\d+)"""U8) is not { Length: 81 } matches)
		{
			return Grid.Undefined;
		}

		var result = Grid.Empty;
		for (int cell = 0; cell < 81; cell++)
		{
			if (matches[cell] is not { Length: var length and <= 9 } s)
			{
				// More than 9 characters.
				return Grid.Undefined;
			}

			if (s.Contains('<'))
			{
				// All values will be treated as normal characters:
				// '<digit>', '*digit*' and 'candidates'.

				// Givens.
				if (length == 3)
				{
					if (s[1] is var c and >= '1' and <= '9')
					{
						result[cell] = c - '1';
						result.SetStatus(cell, CellStatus.Given);
					}
					else
					{
						// Illegal characters found.
						return Grid.Undefined;
					}
				}
				else
				{
					// The length is not 3.
					return Grid.Undefined;
				}
			}
			else if (s.Contains('*'))
			{
				// Modifiables.
				if (length == 3)
				{
					if (s[1] is var c and >= '1' and <= '9')
					{
						result[cell] = c - '1';
						result.SetStatus(cell, CellStatus.Modifiable);
					}
					else
					{
						// Illegal characters found.
						return Grid.Undefined;
					}
				}
				else
				{
					// The length is not 3.
					return Grid.Undefined;
				}
			}
			else if (s.SatisfyPattern("""[1-9]{1,9}"""))
			{
				// Candidates.
				// Here don't need to check the length of the string,
				// and also all characters are digit characters.
				short mask = 0;
				foreach (char c in s)
				{
					mask |= (short)(1 << c - '1');
				}

				if (mask == 0)
				{
					return Grid.Undefined;
				}

				if ((mask & mask - 1) == 0)
				{
					result[cell] = TrailingZeroCount(mask);
					result.SetStatus(cell, CellStatus.Given);
				}
				else
				{
					for (int digit = 0; digit < 9; digit++)
					{
						result[cell, digit] = (mask >> digit & 1) != 0;
					}
				}
			}
			else
			{
				// All conditions can't match.
				return Grid.Undefined;
			}
		}

		return result;
	}

	/// <summary>
	/// Parse the simple table format string (Sudoku explainer format).
	/// </summary>
	/// <param name="parser">The parser.</param>
	/// <returns>The grid.</returns>
	private static Grid OnParsingSimpleTable(ref FileLocalType_Utf8GridParser parser)
	{
		if (parser.ParsingValue.Match("""([\d\.\+]{9}(\r|\n|\r\n)){8}[\d\.\+]{9}"""U8) is not { } match)
		{
			return Grid.Undefined;
		}

		// Remove all '\r's and '\n's.
		scoped var sb = new StringHandler(81 + (9 << 1));
		sb.AppendCharacters(from @char in match where @char is not ('\r' or '\n') select @char);
		parser.ParsingValue = (Utf8String)sb.ToStringAndClear();
		return OnParsingSusser(ref parser, false);
	}

	/// <summary>
	/// Parse the susser format string.
	/// </summary>
	/// <param name="parser">The parser.</param>
	/// <param name="shortenSusser">Indicates whether the parser will shorten the susser format.</param>
	/// <returns>The result.</returns>
	private static Grid OnParsingSusser(ref FileLocalType_Utf8GridParser parser, bool shortenSusser)
	{
		string? match = shortenSusser
			? parser.ParsingValue.Match("""[\d\.\*]{1,9}(,[\d\.\*]{1,9}){8}"""U8)
			: parser.ParsingValue.Match("""[\d\.\+]{80,}(\:(\d{3}\s+)*\d{3})?"""U8);

		switch (shortenSusser)
		{
			case false when match is not { Length: <= 405 }:
			case true when match is not { Length: <= 81 } || !expandCode(match, out match):
			{
				return Grid.Undefined;
			}
		}

		// Step 1: fills all digits.
		var result = Grid.Empty;
		int i = 0, length = match.Length;
		for (int realPos = 0; i < length && match[i] != ':'; realPos++)
		{
			char c = match[i];
			switch (c)
			{
				case '+':
				{
					// Plus sign means the character after it is a digit,
					// which is modifiable value in the grid in its corresponding position.
					if (i < length - 1)
					{
						if (match[i + 1] is var nextChar and >= '1' and <= '9')
						{
							// Set value.
							// Note that the subtractor is character '1', not '0'.
							result[realPos] = nextChar - '1';

							// Add 2 on iteration variable to skip 2 characters
							// (A plus sign '+' and a digit).
							i += 2;
						}
						else
						{
							// Why isn't the character a digit character?
							return Grid.Undefined;
						}
					}
					else
					{
						return Grid.Undefined;
					}

					break;
				}
				case '.':
				case '0':
				{
					// A placeholder.
					// Do nothing but only move 1 step forward.
					i++;

					break;
				}
				case >= '1' and <= '9':
				{
					// Is a digit character.
					// Digits are representing given values in the grid.
					// Not the plus sign, but a placeholder '0' or '.'.
					// Set value.
					result[realPos] = c - '1';

					// Set the cell status as 'CellStatus.Given'.
					// If the code below doesn't make sense to you,
					// you can see the comments in method 'OnParsingSusser(string)'
					// to know the meaning also.
					result.SetStatus(realPos, CellStatus.Given);

					// Finally moves 1 step forward.
					i++;

					break;
				}
				default:
				{
					// Other invalid characters. Throws an exception.
					//throw Throwing.ParsingError<Grid>(nameof(ParsingValue));
					return Grid.Undefined;
				}
			}
		}

		// Step 2: eliminates candidates if exist.
		// If we have met the colon sign ':', this loop would not be executed.
		if (Grid.ExtendedSusserEliminationsRegex().Match(match) is { Success: true, Value: var elimMatch })
		{
			foreach (int candidate in EliminationNotation.ParseCandidates(elimMatch))
			{
				// Set the candidate with false to eliminate the candidate.
				result[candidate / 9, candidate % 9] = false;
			}
		}

		return result;


		static bool expandCode(string? original, [NotNullWhen(true)] out string? result)
		{
			// We must the string code holds 8 ','s and is with no ':' or '+'.
			if (original is null || original.Contains(':') || original.Contains('+') || original.CountOf(',') != 8)
			{
				result = null;
				return false;
			}

			scoped var resultSpan = (stackalloc char[81]);
			string[] lines = original.Split(',');
			if (lines.Length != 9)
			{
				result = null;
				return false;
			}

			// Check per line, and expand it.
			char placeholder = original.IndexOf('0') == -1 ? '.' : '0';
			for (int i = 0; i < 9; i++)
			{
				string line = lines[i];
				switch (line.CountOf('*'))
				{
					case 1 when (9 + 1 - line.Length, 0, 0) is var (empties, j, k):
					{
						foreach (char c in line)
						{
							if (c == '*')
							{
								resultSpan.Slice(i * 9 + k, empties).Fill(placeholder);

								j++;
								k += empties;
							}
							else
							{
								resultSpan[i * 9 + k] = line[j];

								j++;
								k++;
							}
						}

						break;
					}

					case var n when (9 + n - line.Length, 0, 0) is var (empties, j, k):
					{
						int emptiesPerStar = empties / n;
						foreach (char c in line)
						{
							if (c == '*')
							{
								resultSpan.Slice(i * 9 + k, emptiesPerStar).Fill(placeholder);

								j++;
								k += emptiesPerStar;
							}
							else
							{
								resultSpan[i * 9 + k] = line[j];

								j++;
								k++;
							}
						}

						break;
					}
				}
			}

			result = resultSpan.ToString();
			return true;
		}
	}

	/// <summary>
	/// Parse the sukaku format string.
	/// </summary>
	/// <param name="parser">The parser.</param>
	/// <param name="compatibleFirst">
	/// Indicates whether the algorithm uses compatibility mode to check and parse sudoku grid.
	/// </param>
	/// <returns>The result.</returns>
	private static Grid OnParsingSukaku(ref FileLocalType_Utf8GridParser parser, bool compatibleFirst)
	{
		const int candidatesCount = 729;
		if (compatibleFirst)
		{
			var parsingValue = parser.ParsingValue;
			if (parsingValue.Length < candidatesCount)
			{
				return Grid.Undefined;
			}

			var result = Grid.Empty;
			for (int i = 0; i < candidatesCount; i++)
			{
				char c = parsingValue[i];
				if (c is not (>= '0' and <= '9' or '.'))
				{
					return Grid.Undefined;
				}

				if (c is '0' or '.')
				{
					result[i / 9, i % 9] = false;
				}
			}

			return result;
		}
		else
		{
			string[] matches = parser.ParsingValue.MatchAll("""\d*[\-\+]?\d+"""U8);
			if (matches is { Length: not 81 })
			{
				return Grid.Undefined;
			}

			var result = Grid.Empty;
			for (int offset = 0; offset < 81; offset++)
			{
				string s = matches[offset].Reserve(@"\d");
				if (s.Length > 9)
				{
					// More than 9 characters.
					return Grid.Undefined;
				}

				short mask = 0;
				foreach (char c in s)
				{
					mask |= (short)(1 << c - '1');
				}

				if (mask == 0)
				{
					return Grid.Undefined;
				}

				// We don't need to set the value as a given because the current parsing
				// if for sukakus, rather than normal sudokus.
				//if (IsPow2(mask))
				//{
				//	result[offset] = TrailingZeroCount(mask);
				//	result.SetStatus(offset, CellStatus.Given);
				//}

				for (int digit = 0; digit < 9; digit++)
				{
					result[offset, digit] = (mask >> digit & 1) != 0;
				}
			}

			return result;
		}
	}
}

/// <summary>
/// Indicates the factory that creates the grid formatter.
/// </summary>
internal static class FileLocalType_GridFormatterFactory
{
	/// <summary>
	/// Create a <see cref="FileLocalType_GridFormatter"/> according to the specified format.
	/// </summary>
	/// <param name="format">The format.</param>
	/// <returns>The grid formatter.</returns>
	/// <exception cref="FormatException">Throws when the format string is invalid.</exception>
	public static FileLocalType_GridFormatter Create(string? format)
		=> format switch
		{
			null or "." => new(false),
			"+" or ".+" or "+." => new(false) { WithModifiables = true },
			"0" => new(false) { Placeholder = '0' },
			":" => new(false) { WithCandidates = true },
			"!" or ".!" or "!." => new(false) { WithModifiables = true, TreatValueAsGiven = true },
			"0!" or "!0" => new(false) { Placeholder = '0', WithModifiables = true, TreatValueAsGiven = true },
			".:" => new(false) { WithCandidates = true },
			"0:" => new(false) { Placeholder = '0', WithCandidates = true },
			"0+" or "+0" => new(false) { Placeholder = '0', WithModifiables = true },
			"+:" or "+.:" or ".+:" or "#" or "#." => new(false) { WithModifiables = true, WithCandidates = true },
			"0+:" or "+0:" or "#0" => new(false) { Placeholder = '0', WithModifiables = true, WithCandidates = true },
			".!:" or "!.:" => new(false) { WithModifiables = true, TreatValueAsGiven = true },
			"0!:" or "!0:" => new(false) { Placeholder = '0', WithModifiables = true, TreatValueAsGiven = true },
			".*" or "*." => new(false) { Placeholder = '.', ShortenSusser = true },
			"0*" or "*0" => new(false) { Placeholder = '0', ShortenSusser = true },
			"@" or "@." => new(true) { SubtleGridLines = true },
			"@0" => new(true) { Placeholder = '0', SubtleGridLines = true },
			"@!" or "@.!" or "@!." => new(true) { TreatValueAsGiven = true, SubtleGridLines = true },
			"@0!" or "@!0" => new(true) { Placeholder = '0', TreatValueAsGiven = true, SubtleGridLines = true },
			"@*" or "@.*" or "@*." => new(true),
			"@0*" or "@*0" => new(true) { Placeholder = '0' },
			"@!*" or "@*!" => new(true) { TreatValueAsGiven = true },
			"@:" => new(true) { WithCandidates = true, SubtleGridLines = true },
			"@:!" or "@!:" => new(true) { WithCandidates = true, TreatValueAsGiven = true, SubtleGridLines = true },
			"@*:" or "@:*" => new(true) { WithCandidates = true },
			"@!*:" or "@*!:" or "@!:*" or "@*:!" or "@:!*" or "@:*!" => new(true) { WithCandidates = true, TreatValueAsGiven = true },
			"~" or "~0" => new(false) { Sukaku = true, Placeholder = '0' },
			"~." => new(false) { Sukaku = true },
			"@~" or "~@" => new(true) { Sukaku = true },
			"@~0" or "@0~" or "~@0" or "~0@" => new(true) { Sukaku = true, Placeholder = '0' },
			"@~." or "@.~" or "~@." or "~.@" => new(true) { Sukaku = true },
			"%" => new(true) { Excel = true },
			"^" => new(false) { OpenSudoku = true },
			_ => throw new FormatException("The specified format is invalid.")
		};
}

internal sealed class FileLocalType_MatchLengthComparer : IEqualityComparer<Match>
{
	/// <inheritdoc/>
	public bool Equals(Match? x, Match? y) => (x?.Value.Length ?? -1) == (y?.Value.Length ?? -1);

	/// <inheritdoc/>
	public int GetHashCode([DisallowNull] Match? obj) => obj?.Value.Length ?? -1;
}
