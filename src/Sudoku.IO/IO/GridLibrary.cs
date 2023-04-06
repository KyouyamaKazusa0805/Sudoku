namespace Sudoku.IO;

/// <summary>
/// Defines a <see cref="Grid"/> library that stores in a file, using lines to describe puzzles.
/// </summary>
/// <param name="filePath">The file path.</param>
/// <param name="ignoreOption">Ignoring option.</param>
/// <exception cref="ArgumentException">Throws when the specified file path is invalid or the file does not exist.</exception>
/// <exception cref="ArgumentOutOfRangeException">
/// Throws when the argument <paramref name="ignoreOption"/> is not defined in enumeration type.
/// </exception>
/// <seealso cref="Grid"/>
public sealed partial class GridLibrary(string filePath, GridLibraryIgnoringOption ignoreOption = GridLibraryIgnoringOption.Never) :
	IAsyncEnumerable<Grid>,
	IEquatable<GridLibrary>,
	IEqualityOperators<GridLibrary, GridLibrary, bool>
{
	/// <summary>
	/// Indicates the solver to verify the puzzle.
	/// </summary>
	private static readonly BitwiseSolver Solver = new();


	/// <summary>
	/// Indicates the number of puzzles stored in this library.
	/// </summary>
	public int PuzzlesCount => File.ReadAllLines(FilePath).Count(static line => Grid.TryParse(line, out _));

	/// <summary>
	/// Indicates the file path.
	/// </summary>
	public string FilePath { get; } =
		File.Exists(filePath) ? filePath : throw new ArgumentException("Specified file does not exist.", nameof(filePath));

	/// <summary>
	/// Indicates the ignore option that will be used for ignoring on iteration of library file.
	/// </summary>
	public GridLibraryIgnoringOption IgnoringOption { get; } =
		Enum.IsDefined(ignoreOption) ? ignoreOption : throw new ArgumentOutOfRangeException(nameof(ignoreOption));


	[GeneratedOverridingMember(GeneratedEqualsBehavior.AsCastAndCallingOverloading)]
	public override partial bool Equals(object? obj);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals([NotNullWhen(true)] GridLibrary? other) => other is not null && FilePath == other.FilePath;

	[GeneratedOverridingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(FilePath))]
	public override partial int GetHashCode();

	[GeneratedOverridingMember(GeneratedToStringBehavior.RecordLike, nameof(FilePath))]
	public override partial string ToString();

	/// <inheritdoc/>
	public async IAsyncEnumerator<Grid> GetAsyncEnumerator(CancellationToken cancellationToken = default)
	{
		foreach (var line in await File.ReadAllLinesAsync(FilePath, cancellationToken))
		{
			if (Grid.TryParse(line, out var grid))
			{
				switch (IgnoringOption)
				{
					case GridLibraryIgnoringOption.Never:
					case GridLibraryIgnoringOption.NotUnique when Solver.CheckValidity(grid.ToString()):
					{
						yield return grid;
						break;
					}
				}
			}
		}
	}

	/// <summary>
	/// Reads the library file, and then parses puzzles into <see cref="Grid"/> instances, and finally filters
	/// <see cref="Grid"/> instances when puzzles don't pass the verification.
	/// </summary>
	/// <param name="gridFilter">The grid filter.</param>
	/// <param name="failedCallback">Indicates the failed action.</param>
	/// <param name="cancellationToken">The cancellation token that is used for cancelling the asynchronous operation.</param>
	/// <returns>An <see cref="IAsyncEnumerable{T}"/> instance that iterates on filtered <see cref="Grid"/> instances.</returns>
	public async IAsyncEnumerable<Grid> FilterAsync(
		Func<Grid, CancellationToken, bool> gridFilter,
		Action? failedCallback = null,
		[EnumeratorCancellation] CancellationToken cancellationToken = default
	)
	{
		await foreach (var grid in this)
		{
			if (gridFilter(grid, cancellationToken))
			{
				yield return grid;
				continue;
			}

			failedCallback?.Invoke();
		}
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(GridLibrary? left, GridLibrary? right)
		=> (left, right) switch { (null, null) => true, (not null, not null) => left.Equals(right), _ => false };

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(GridLibrary? left, GridLibrary? right) => !(left == right);
}
