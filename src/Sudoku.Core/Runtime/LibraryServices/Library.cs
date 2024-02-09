namespace Sudoku.Runtime.LibraryServices;

/// <summary>
/// Represents an entry that plays with a puzzle library file.
/// </summary>
/// <param name="filePath">Indicates the file path used.</param>
[StructLayout(LayoutKind.Auto)]
[Equals]
[GetHashCode]
[ToString]
[EqualityOperators]
[method: DebuggerStepThrough]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly partial struct Library([PrimaryConstructorParameter, HashCodeMember, StringMember] string filePath) :
	IAsyncEnumerable<Grid>,
	IEqualityOperators<Library, Library, bool>,
	IEquatable<Library>
{
	/// <summary>
	/// Indicates the "Not Exist" message.
	/// </summary>
	private const string Error_NotExist = "The file does not exist.";

	/// <summary>
	/// Indicates the "Multiple Same Properties Found" message.
	/// </summary>
	private const string Error_MultipleSamePropertiesFound = "Multiple same properties are found.";

	/// <summary>
	/// Indicates the "Difference Existence of Library File and Config File" message.
	/// </summary>
	private const string Error_DifferentExistenceOfConfigAndLibraryFile = "Different existence of config file and library file.";

	/// <summary>
	/// Indicates the "Related Files Should Be Initialized First" message.
	/// </summary>
	private const string Error_FileShouldBeInitializedFirst = "Related files should be initialized first.";

	/// <summary>
	/// Indicates the "Unrecognized Grid Format" message.
	/// </summary>
	private const string Error_UnrecognizedGridFormat = "You cannot append text that cannot be recognized as a valid sudoku grid.";


	/// <summary>
	/// Indicates whether the library-related files are initialized.
	/// </summary>
	public bool IsInitialized
		=> (File.Exists(ConfigFilePath), File.Exists(FilePath)) switch
		{
			(true, true) => true,
			(false, false) => false,
			_ => throw new InvalidOperationException(Error_DifferentExistenceOfConfigAndLibraryFile)
		};

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
	/// Indicates the path of configuration file. The file contains the information of the library.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Due to the design of the library APIs, a puzzle library contains two parts, separated with 2 files.
	/// One is the configuration file, and the other is the library details, only containing puzzles.
	/// </para>
	/// <para><i>
	/// Call this property will implicitly create config file if file is not found. No exception will be thrown here.
	/// </i></para>
	/// </remarks>
	public string ConfigFilePath
	{
		get
		{
			var fileName = Path.GetFileNameWithoutExtension(FilePath);
			var parentFolder = Path.GetDirectoryName(FilePath);
			var result = $@"{parentFolder}\{fileName}";

			Initialize();

			return result;
		}
	}

	/// <summary>
	/// Indicates the author of the library. Return <see langword="null"/> if no author configured.
	/// </summary>
	/// <exception cref="InvalidOperationException">Throws when the library is not initialized.</exception>
	/// <exception cref="FileNotFoundException">Throws when the config file is missing.</exception>
	[DisallowNull]
	public string? Author
	{
		get
		{
			var pattern = AuthorPattern();
			return IsInitialized
				? (
					from line in File.ReadLines(ConfigFilePath)
					let groups = pattern.Match(line).Groups
					where groups.Count == 1
					select groups[0].Value
				).FirstOrDefault()
				: throw new InvalidOperationException(Error_FileShouldBeInitializedFirst);
		}

		set
		{
			if (!File.Exists(ConfigFilePath))
			{
				throw new FileNotFoundException(Error_NotExist);
			}

			ConfigFileReplaceOrAppend(AuthorPattern().IsMatch, nameof(Author), value);
		}
	}


	/// <summary>
	/// Try to get the element at the specified index.
	/// </summary>
	/// <param name="index">The desired index.</param>
	/// <returns>The target <see cref="Grid"/> instance at the specified index.</returns>
	/// <remarks>
	/// This property is run synchronously, calling <see cref="GetAtAsync(int)"/>.
	/// <b>Always measure performance if you want to use this indexer.</b>
	/// </remarks>
	/// <seealso cref="GetAtAsync(int)"/>
	public Grid this[int index] => GetAtAsync(index).Result;


	/// <summary>
	/// Initializes the library-related files if not found. If initialized, nothing will be happened.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Initialize()
	{
		if (!IsInitialized)
		{
			File.Create(ConfigFilePath);
			File.Create(FilePath);
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Library other) => FilePath == other.FilePath;

	/// <summary>
	/// Append a puzzle, represented as a <see cref="string"/> value,
	/// into the specified file path represented as a puzzle library.
	/// </summary>
	/// <param name="grid">The grid text code to be appended.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel the current asynchronous operation.</param>
	/// <returns>A <see cref="Task"/> instance that can be used in <see langword="await"/> expression.</returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when the library is not initialized, or grid cannot be recognized.
	/// </exception>
	public async Task AppendPuzzleAsync(string grid, CancellationToken cancellationToken = default)
		=> await (
			IsInitialized
				? Grid.TryParse(grid, out _)
					? File.AppendAllTextAsync(FilePath, grid, cancellationToken)
					: throw new InvalidOperationException(Error_UnrecognizedGridFormat)
				: throw new InvalidOperationException(Error_FileShouldBeInitializedFirst)
		);

	/// <inheritdoc cref="AppendPuzzleAsync(string, CancellationToken)"/>
	public async Task AppendPuzzleAsync(Grid grid, CancellationToken cancellationToken = default)
		=> await AppendPuzzleAsync(GetSingleLineGridString(in grid), cancellationToken);

	/// <summary>
	/// Removes all puzzles that exactly same as the specified one from the file.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel the current asynchronous operation.</param>
	/// <returns>A <see cref="Task"/> instance that can be used in <see langword="await"/> expression.</returns>
	/// <exception cref="InvalidOperationException">Throw when the library file is not initialized.</exception>
	public async Task RemovePuzzleAsync(string grid, CancellationToken cancellationToken = default)
	{
		if (!IsInitialized)
		{
			throw new InvalidOperationException(Error_FileShouldBeInitializedFirst);
		}

		var linesToKeep = new List<string>();
		await foreach (var line in File.ReadLinesAsync(FilePath, cancellationToken))
		{
			if (line != grid)
			{
				linesToKeep.Add(line);
			}
		}

		var tempFile = Path.GetTempFileName();
		await File.WriteAllLinesAsync(tempFile, linesToKeep, cancellationToken);
		File.Delete(FilePath);
		File.Move(tempFile, FilePath);
	}

	/// <inheritdoc cref="RemovePuzzleAsync(string, CancellationToken)"/>
	public async Task RemovePuzzleAsync(Grid grid, CancellationToken cancellationToken = default)
		=> await RemovePuzzleAsync(GetSingleLineGridString(in grid), cancellationToken);

	/// <summary>
	/// Write a puzzle into a file just created. If the file exists, it will return <see langword="false"/>.
	/// </summary>
	/// <param name="grid">The grid to be written.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel the current asynchronous operation.</param>
	/// <returns>A <see cref="Task"/> instance that can be used in <see langword="await"/> expression.</returns>
	/// <exception cref="InvalidOperationException">Throw when the library file is not initialized.</exception>
	public async Task<bool> TryWriteAsync(string grid, CancellationToken cancellationToken = default)
	{
		if (!IsInitialized)
		{
			throw new InvalidOperationException(Error_FileShouldBeInitializedFirst);
		}

		if (Grid.TryParse(grid, out _))
		{
			await File.WriteAllTextAsync(FilePath, grid, cancellationToken);
			return true;
		}

		return false;
	}

	/// <inheritdoc cref="TryWriteAsync(string, CancellationToken)"/>
	public async Task<bool> TryWriteAsync(Grid grid, CancellationToken cancellationToken = default)
		=> await TryWriteAsync(GetSingleLineGridString(in grid), cancellationToken);

	/// <summary>
	/// Calculates how many puzzles in this file.
	/// </summary>
	/// <returns>A <see cref="Task{T}"/> of an <see cref="int"/> value indicating the result.</returns>
	/// <exception cref="InvalidOperationException">Throws when the library file is not initialized.</exception>
	public async Task<int> GetCountAsync()
	{
		if (!IsInitialized)
		{
			throw new InvalidOperationException(Error_FileShouldBeInitializedFirst);
		}

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
	/// <exception cref="InvalidOperationException">Throws when the library file is not initialized.</exception>
	/// <exception cref="IndexOutOfRangeException">Throws when the index is out of range.</exception>
	public async Task<Grid> GetAtAsync(int index)
	{
		if (!IsInitialized)
		{
			throw new InvalidOperationException(Error_FileShouldBeInitializedFirst);
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
	/// <param name="transformTypes">
	/// Indicates the available transform type that the chosen grid can be transformed.
	/// Use <see cref="TransformType"/>.<see langword="operator"/> |(<see cref="TransformType"/>, <see cref="TransformType"/>)
	/// to combine multiple flags.
	/// </param>
	/// <returns>A <see cref="Task{T}"/> of <see cref="Grid"/> instance as the result.</returns>
	/// <exception cref="InvalidOperationException">Throw when the library file is not initialized.</exception>
	/// <seealso href="http://tinyurl.com/choose-a-random-element">Choose a random element from a sequence of unknown length</seealso>
	public async Task<Grid> RandomReadOneAsync(TransformType transformTypes = TransformType.None)
	{
		if (!IsInitialized)
		{
			throw new InvalidOperationException(Error_FileShouldBeInitializedFirst);
		}

		var rng = new Random();
		var numberSeen = 0U;
		Unsafe.SkipInit(out Grid chosen);
		await foreach (var grid in new Library(FilePath))
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
		if (!File.Exists(FilePath))
		{
			yield break;
		}

		await foreach (var str in File.ReadLinesAsync(FilePath, cancellationToken))
		{
			yield return Grid.Parse(str);
		}
	}

	/// <summary>
	/// Replace or append the value into the file, using the specified match method.
	/// </summary>
	/// <param name="match">The matcher method.</param>
	/// <param name="prefix">Indicates the prefix.</param>
	/// <param name="replaceOrAppendValue">The value to replace with original value, or appened.</param>
	/// <exception cref="InvalidOperationException">Throws when multiple same properties found.</exception>
	private void ConfigFileReplaceOrAppend(Func<string, bool> match, string prefix, string replaceOrAppendValue)
	{
		var linesToKeep = new List<string>();
		var isFound = false;
		foreach (var line in File.ReadLines(ConfigFilePath))
		{
			if (!match(line))
			{
				linesToKeep.Add(line);
			}

			if (isFound)
			{
				throw new InvalidOperationException(Error_MultipleSamePropertiesFound);
			}

			a(linesToKeep, prefix, replaceOrAppendValue);
			isFound = true;
		}
		if (!isFound)
		{
			a(linesToKeep, prefix, replaceOrAppendValue);
		}

		var tempFile = Path.GetTempFileName();
		File.WriteAllLines(tempFile, linesToKeep);
		File.Delete(FilePath);
		File.Move(tempFile, FilePath);


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void a(List<string> list, string prefix, string replaceOrAppendValue) => list.Add($"{prefix}: {replaceOrAppendValue}");
	}


	/// <summary>
	/// Returns <c>grid.ToString("#")</c>.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static string GetSingleLineGridString(scoped ref readonly Grid grid) => grid.ToString("#");

	[GeneratedRegex($"""{nameof(Author)}:\s*([\s\S]+)""", RegexOptions.Compiled | RegexOptions.IgnoreCase, 5000)]
	private static partial Regex AuthorPattern();
}
