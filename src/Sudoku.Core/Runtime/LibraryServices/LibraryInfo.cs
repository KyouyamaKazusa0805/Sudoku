namespace Sudoku.Runtime.LibraryServices;

/// <summary>
/// Represents an entry that can fetch a puzzle library file, to check for details of the file.
/// </summary>
/// <param name="filePath">Indicates the file path used.</param>
/// <remarks>
/// This type uses async enumeration. You should use <see langword="await foreach"/> statement
/// to iterate on each puzzle in this library:
/// <code><![CDATA[
/// // Suppose the puzzle library file we iterate on is stored in desktop.
/// string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
/// 
/// // Iterate on each grid.
/// string libraryFile = $@"{desktop}\library.txt";
/// await foreach (var grid in new LibraryInfo(libraryFile))
/// {
///     // Do whatever you want to do here.
/// }
/// ]]></code>
/// </remarks>
[Equals]
[GetHashCode]
[ToString]
[EqualityOperators]
[method: DebuggerStepThrough]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly partial struct LibraryInfo([PrimaryConstructorParameter, HashCodeMember, StringMember] string filePath) :
	IAsyncEnumerable<Grid>,
	IEqualityOperators<LibraryInfo, LibraryInfo, bool>,
	IEquatable<LibraryInfo>
{
	/// <summary>
	/// Indicates the "Not Exist" message.
	/// </summary>
	private const string Error_NotExist = "The file does not exist.";


	/// <summary>
	/// Indicates the number of puzzles stored in this file.
	/// </summary>
	/// <remarks>
	/// This property is run synchronously, calling <see cref="GetCountAsync"/>.
	/// <b>Always measure performance if you want to use this property.</b>
	/// </remarks>
	/// <seealso cref="GetCountAsync"/>
	public int Count => GetCountAsync().Result;


	/// <summary>
	/// Try to get the element at the specified index.
	/// </summary>
	/// <param name="index">The desired index.</param>
	/// <returns>The target <see cref="Grid"/> instance at the specified index.</returns>
	/// <remarks>
	/// This property is run synchronously, calling <see cref="GetAtAsync(int)"/>.
	/// <b>Always measure performance if you want to use this property.</b>
	/// </remarks>
	/// <seealso cref="GetAtAsync(int)"/>
	public Grid this[int index] => GetAtAsync(index).Result;


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(LibraryInfo other) => FilePath == other.FilePath;

	/// <summary>
	/// Creates the library file in local if not exist.
	/// </summary>
	/// <returns>A <see cref="bool"/> value indicating whether the creation is succeeded.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool CreateIfNotExist()
	{
		if (!File.Exists(FilePath))
		{
			File.Create(FilePath);
			return true;
		}

		return false;
	}

	/// <summary>
	/// Append a puzzle, represented as a <see cref="string"/> value,
	/// into the specified file path represented as a puzzle library.
	/// </summary>
	/// <param name="grid">The grid text code to be appended.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel the current asynchronous operation.</param>
	/// <returns>A <see cref="Task"/> instance that can be used in <see langword="await"/> expression.</returns>
	public async Task AppendPuzzleAsync(string grid, CancellationToken cancellationToken = default)
		=> await (
			Grid.TryParse(grid, out _)
				? File.AppendAllTextAsync(FilePath, grid, cancellationToken)
				: Task.FromException(new InvalidOperationException("You cannot append text that cannot be recognized as a valid sudoku grid."))
		);

	/// <inheritdoc cref="AppendPuzzleAsync(string, CancellationToken)"/>
	public async Task AppendPuzzleAsync(Grid grid, CancellationToken cancellationToken = default)
		=> await AppendPuzzleAsync(grid.ToString("#"), cancellationToken);

	/// <summary>
	/// Removes all puzzles that exactly same as the specified one from the file.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel the current asynchronous operation.</param>
	/// <returns>A <see cref="Task"/> instance that can be used in <see langword="await"/> expression.</returns>
	public async Task RemovePuzzleAsync(string grid, CancellationToken cancellationToken = default)
	{
		var tempFile = Path.GetTempFileName();
		var linesToKeep = new List<string>();
		await foreach (var line in File.ReadLinesAsync(FilePath, cancellationToken))
		{
			if (line != grid)
			{
				linesToKeep.Add(line);
			}
		}

		await File.WriteAllLinesAsync(tempFile, linesToKeep, cancellationToken);
		File.Delete(FilePath);
		File.Move(tempFile, FilePath);
	}

	/// <inheritdoc cref="RemovePuzzleAsync(string, CancellationToken)"/>
	public async Task RemovePuzzleAsync(Grid grid, CancellationToken cancellationToken = default)
		=> await RemovePuzzleAsync(grid.ToString("#"), cancellationToken);

	/// <summary>
	/// Write a puzzle into a file just created. If the file exists, it will return <see langword="false"/>.
	/// </summary>
	/// <param name="grid">The grid to be written.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel the current asynchronous operation.</param>
	/// <returns>A <see cref="Task"/> instance that can be used in <see langword="await"/> expression.</returns>
	public async Task<bool> TryWriteAsync(string grid, CancellationToken cancellationToken = default)
	{
		if (Grid.TryParse(grid, out _))
		{
			if (File.Exists(FilePath))
			{
				return false;
			}

			await File.WriteAllTextAsync(FilePath, grid, cancellationToken);
			return true;
		}

		return false;
	}

	/// <inheritdoc cref="TryWriteAsync(string, CancellationToken)"/>
	public async Task<bool> TryWriteAsync(Grid grid, CancellationToken cancellationToken = default)
		=> await TryWriteAsync(grid.ToString("#"), cancellationToken);

	/// <summary>
	/// Calculates how many puzzles in this file.
	/// </summary>
	/// <returns>A <see cref="Task{T}"/> of an <see cref="int"/> value indicating the result.</returns>
	public async Task<int> GetCountAsync()
	{
		var result = 0;
		await foreach (var _ in this)
		{
			result++;
		}

		return result;
	}

	/// <summary>
	/// Gets the <see cref="Grid"/> at the specified index.
	/// </summary>
	/// <param name="index">The desired index.</param>
	/// <returns>A <see cref="Task{T}"/> of <see cref="Grid"/> instance as the result.</returns>
	/// <exception cref="FileNotFoundException">Throws when the file doesn't exist.</exception>
	/// <exception cref="IndexOutOfRangeException">Throws when the index is out of range.</exception>
	public async Task<Grid> GetAtAsync(int index)
	{
		if (!File.Exists(FilePath))
		{
			throw new FileNotFoundException(Error_NotExist);
		}

		var i = -1;
		await foreach (var grid in this)
		{
			if (++i == index)
			{
				return grid;
			}
		}

		throw new IndexOutOfRangeException();
	}

	/// <summary>
	/// Randomly read one puzzle in the specified file, and return it.
	/// </summary>
	/// <param name="transformTypes">Indicates the available transform type that the chosen grid can be transformed.</param>
	/// <returns>A <see cref="Task{T}"/> of <see cref="Grid"/> instance as the result.</returns>
	/// <see href="http://tinyurl.com/choose-a-random-element">Choose a random element from a sequence of unknown length</see>
	public async Task<Grid> RandomReadOneAsync(TransformType transformTypes = TransformType.None)
	{
		var rng = new Random();
		var numberSeen = 0U;
		Unsafe.SkipInit(out Grid chosen);
		await foreach (var grid in new LibraryInfo(FilePath))
		{
			if ((uint)rng.Next() % ++numberSeen == 0)
			{
				chosen = grid;
			}
		}

		transformGrid(ref chosen, transformTypes);
		return chosen;


		unsafe void transformGrid(scoped ref Grid grid, TransformType transformTypes)
		{
			if (transformTypes == TransformType.None)
			{
				return;
			}

			foreach (var kind in transformTypes)
			{
				(
					kind switch
					{
						TransformType.DigitSwap => &swapDigits,
						TransformType.RowSwap => &swapRow,
						TransformType.ColumnSwap => &swapColumn,
						TransformType.BandSwap => &swapBand,
						TransformType.TowerSwap => &swapTower,
						TransformType.MirrorLeftRight => &mirrorLeftRight,
						TransformType.MirrorTopBottom => &mirrorTopBottom,
						TransformType.MirrorDiagonal => &mirrorDiagonal,
						TransformType.MirrorAntidiagonal => &mirrorAntidiagonal,
						TransformType.RotateClockwise => &rotateClockwise,
						TransformType.RotateCounterclockwise => &rotateCounterclockwise,
						_ => default(GridRandomizedSuffler)
					}
				)(rng, ref grid);
			}


			static void swapDigits(Random random, scoped ref Grid grid)
			{
				for (var i = 0; i < 10; i++)
				{
					var d1 = random.Next(0, 9);
					int d2;
					do
					{
						d2 = random.Next(0, 9);
					} while (d1 == d2);
					grid.SwapTwoDigits(d1, d2);
				}
			}

			static void swapRow(Random random, scoped ref Grid grid)
			{
				for (var i = 0; i < 10; i++)
				{
					var l1 = random.Next(9, 18);
					var l1p = l1 / 3;
					int l2;
					do
					{
						l2 = l1p * 3 + random.Next(0, 3);
					} while (l1 == l2);
					grid.SwapTwoHouses(l1, l2);
				}
			}

			static void swapColumn(Random random, scoped ref Grid grid)
			{
				for (var i = 0; i < 10; i++)
				{
					var l1 = random.Next(18, 27);
					var l1p = l1 / 3;
					int l2;
					do
					{
						l2 = l1p * 3 + random.Next(0, 3);
					} while (l1 == l2);
					grid.SwapTwoHouses(l1, l2);
				}
			}

			static void swapBand(Random random, scoped ref Grid grid)
			{
				for (var i = 0; i < 2; i++)
				{
					var c1 = random.Next(0, 3);
					int c2;
					do
					{
						c2 = random.Next(0, 3);
					} while (c1 == c2);
					grid.SwapChute(c1, c2);
				}
			}

			static void swapTower(Random random, scoped ref Grid grid)
			{
				for (var i = 0; i < 2; i++)
				{
					var c1 = random.Next(3, 6);
					int c2;
					do
					{
						c2 = random.Next(3, 6);
					} while (c1 == c2);
					grid.SwapChute(c1, c2);
				}
			}

			static void mirrorLeftRight(Random _, scoped ref Grid grid) => grid.MirrorLeftRight();

			static void mirrorTopBottom(Random _, scoped ref Grid grid) => grid.MirrorTopBottom();

			static void mirrorDiagonal(Random _, scoped ref Grid grid) => grid.MirrorDiagonal();

			static void mirrorAntidiagonal(Random _, scoped ref Grid grid) => grid.MirrorAntidiagonal();

			static void rotateClockwise(Random random, scoped ref Grid grid)
			{
				var times = random.Next(0, 4);
				for (var i = 0; i < times; i++)
				{
					grid.RotateClockwise();
				}
			}

			static void rotateCounterclockwise(Random random, scoped ref Grid grid)
			{
				var times = random.Next(0, 4);
				for (var i = 0; i < times; i++)
				{
					grid.RotateCounterclockwise();
				}
			}
		}
	}

	/// <inheritdoc/>
	public async IAsyncEnumerator<Grid> GetAsyncEnumerator(CancellationToken cancellationToken = default)
	{
		await foreach (var str in File.ReadLinesAsync(FilePath, cancellationToken))
		{
			yield return Grid.Parse(str);
		}
	}
}
