namespace Sudoku.Generating;

/// <summary>
/// Defines a puzzle generator that makes the given pattern as a hard one.
/// A <b>hard pattern</b> doesn't mean the puzzle will be hard or fiendish.
/// </summary>
public sealed class HardPatternPuzzleGenerator : IGenerator<Grid>
{
	/// <summary>
	/// Indicates the block factor.
	/// </summary>
	private static readonly int[] BlockFactor = [0, 6, 54, 60, 3, 27, 33, 57, 30];

	/// <summary>
	/// Indicates the swapping factor.
	/// </summary>
	private static readonly int[][] SwappingFactor = [[0, 1, 2], [0, 2, 1], [1, 0, 2], [1, 2, 0], [2, 0, 1], [2, 1, 0]];


	/// <summary>
	/// Indicates the shared <see cref="Random"/> instance.
	/// </summary>
	private static Random? _randomShared;


	/// <summary>
	/// Indicates the inner solver that can fast solve a sudoku puzzle, to check the validity
	/// of a puzzle being generated.
	/// </summary>
	private readonly BitwiseSolver _solver = new();


	/// <summary>
	/// Indicates the backing random.
	/// </summary>
	private static Random Rng => _randomShared ??= Random.Shared;


	/// <inheritdoc/>
	public unsafe Grid Generate(IProgress<GeneratorProgress>? progress = null, CancellationToken cancellationToken = default)
	{
		scoped var puzzleString = (stackalloc char[82]);
		scoped var solutionString = (stackalloc char[82]);
		scoped var holeCells = (stackalloc Cell[82]);
		(puzzleString[^1], solutionString[^1], holeCells[^1], var progressTimes) = ('\0', '\0', '\0', 0);

		scoped ref readonly var charRef = ref Grid.EmptyString.Ref();
		while (true)
		{
			Unsafe.CopyBlock(ref Ref.AsByteRef(ref puzzleString[0]), in Ref.AsReadOnlyByteRef(in charRef), sizeof(char) * 81);
			Unsafe.CopyBlock(ref Ref.AsByteRef(ref solutionString[0]), in Ref.AsReadOnlyByteRef(in charRef), sizeof(char) * 81);

			GenerateAnswerGrid(puzzleString, solutionString);

			holeCells.Clear();
			CreatePattern(holeCells);

			for (var trial = 0; trial < 1000; trial++)
			{
				for (var cell = 0; cell < 81; cell++)
				{
					var p = holeCells[cell];
					var temp = solutionString[p];
					solutionString[p] = '0';

					if (!_solver.CheckValidity(solutionString[0].ToPointer()))
					{
						// Reset the value.
						solutionString[p] = temp;
					}
				}

				if (_solver.CheckValidity(solutionString[0].ToPointer()) && Grid.Parse(new string(solutionString)) is var grid)
				{
					return grid;
				}

				// Report if failed to generate.
				progress?.Report(new(++progressTimes));

				cancellationToken.ThrowIfCancellationRequested();

				// Re-create pattern.
				RecreatePattern(holeCells);
			}
		}
	}

	/// <summary>
	/// Generates the answer sudoku grid via the specified puzzle and the solution variable pointer.
	/// </summary>
	/// <param name="puzzleString">The pointer that points to the puzzle.</param>
	/// <param name="solutionString">
	/// The pointer that points to the solution. The result value will be changed here.
	/// </param>
	private unsafe void GenerateAnswerGrid(scoped Span<char> puzzleString, scoped Span<char> solutionString)
	{
		do
		{
			for (var i = 0; i < 81; i++)
			{
				puzzleString[i] = '0';
			}

			var map = (CellMap)[];
			for (var i = 0; i < 16; i++)
			{
				while (true)
				{
					if (map.Add(Rng.Next(81)))
					{
						break;
					}
				}
			}

			foreach (var cell in map)
			{
				do
				{
					puzzleString[cell] = (char)(Rng.NextDigit() + '1');
				} while (CheckDuplicate(puzzleString, cell));
			}
		} while (_solver.SolveString(puzzleString[0].ToPointer(), solutionString[0].ToPointer(), 2) == 0);
	}

	/// <summary>
	/// Creates a start pattern based on a base pattern.
	/// </summary>
	/// <param name="pattern">The base pattern.</param>
	private void CreatePattern(scoped Span<Cell> pattern)
	{
		for (var (i, a, b) = (0, 54, 0); i < 9; i++)
		{
			var n = (int)(Rng.NextDouble() * 6);
			for (var j = 0; j < 3; j++)
			{
				for (var k = 0; k < 3; k++)
				{
					pattern[(k == SwappingFactor[n][j] ? ref a : ref b)++] = BlockFactor[i] + j * 9 + k;
				}
			}
		}

		RecreatePattern(pattern);
	}

	/// <summary>
	/// To re-create the pattern.
	/// </summary>
	/// <param name="pattern">The pointer that points to an array of the pattern values.</param>
	private void RecreatePattern(scoped Span<Cell> pattern)
	{
		scoped var target = (ReadOnlySpan<(int, int, int)>)[(23, 0, 1), (47, 24, -23), (53, 48, -47), (80, 54, 27)];
		for (var index = 0; index < 4; index++)
		{
			var (initial, boundary, delta) = target[index];
			for (var i = initial; i >= boundary; i--)
			{
				Ref.Swap(ref pattern[i], ref pattern[boundary + (Cell)((index == 3 ? delta : (i + delta)) * Rng.NextDouble())]);
			}
		}
	}


	/// <summary>
	/// Check whether the digit in its peer cells has duplicate ones.
	/// </summary>
	/// <param name="gridString">The pointer that points to a grid.</param>
	/// <param name="cell">The cell.</param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	private static bool CheckDuplicate(scoped Span<char> gridString, Cell cell)
	{
		var value = gridString[cell];
		foreach (var c in PeersMap[cell])
		{
			if (value != '0' && gridString[c] == value)
			{
				return true;
			}
		}

		return false;
	}
}

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	/// <summary>
	/// Cast the reference to pointer.
	/// </summary>
	/// <param name="this">The reference to be casted.</param>
	/// <returns>The pointer.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static unsafe char* ToPointer(this ref char @this) => (char*)Unsafe.AsPointer(ref @this);

	/// <summary>
	/// Gets the next digit.
	/// </summary>
	/// <param name="this">The random instance.</param>
	/// <returns>The next digit.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Digit NextDigit(this Random @this) => @this.Next(0, 9);
}
