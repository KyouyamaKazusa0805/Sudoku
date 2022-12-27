namespace Sudoku.IO;

/// <summary>
/// Defines a puzzle library that stores in a file.
/// </summary>
public sealed partial class PuzzleLibrary :
	IAsyncEnumerable<Grid>,
	IEnumerable<Grid>,
	IEquatable<PuzzleLibrary>,
	IEqualityOperators<PuzzleLibrary, PuzzleLibrary, bool>
{
	/// <summary>
	/// Indicates the solver to verify the puzzle.
	/// </summary>
	private static readonly BitwiseSolver Solver = new();


	/// <summary>
	/// Initializes a <see cref="PuzzleLibrary"/> instance via the specified puzzle library file.
	/// </summary>
	/// <param name="filePath">The file path.</param>
	/// <param name="ignoreOption">Ignoring option.</param>
	/// <exception cref="ArgumentException">Throws when the specified file path is invalid or the file does not exist.</exception>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="ignoreOption"/> is not defined in enumeration type.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PuzzleLibrary(string filePath, PuzzleLibraryIgnoringOption ignoreOption = PuzzleLibraryIgnoringOption.Never)
		=> (FilePath, IgnoringOption) = (
			Uri.IsWellFormedUriString(filePath, UriKind.Absolute)
				? File.Exists(filePath)
					? filePath
					: throw new ArgumentException("Specified file does not exist.", nameof(filePath))
				: throw new ArgumentException("Specified file path is invalid.", nameof(filePath)),
			Enum.IsDefined(ignoreOption) ? ignoreOption : throw new ArgumentOutOfRangeException(nameof(ignoreOption))
		);


	/// <summary>
	/// Indicates the file path.
	/// </summary>
	public string FilePath { get; }

	/// <summary>
	/// Indicates the ignore option that will be used for ignoring on iteration of library file.
	/// </summary>
	public PuzzleLibraryIgnoringOption IgnoringOption { get; }


	[GeneratedOverriddingMember(GeneratedEqualsBehavior.AsCastAndCallingOverloading)]
	public override partial bool Equals(object? obj);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals([NotNullWhen(true)] PuzzleLibrary? other) => other is not null && FilePath == other.FilePath;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(FilePath))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(FilePath))]
	public override partial string ToString();

	/// <summary>
	/// Try to get puzzles from the target file, and parses them, returning the valid list of <see cref="Grid"/> encapsulated instances.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token that can cancel the operation.</param>
	/// <returns>
	/// A <see cref="Task{TResult}"/> instance that encapsulates the current asynchronous operation,
	/// with an <see cref="IEnumerable{T}"/> of <see cref="Grid"/> value to be returned.
	/// </returns>
	public async Task<IEnumerable<Grid>> GetPuzzlesAsync(CancellationToken cancellationToken = default)
	{
		return g(await File.ReadAllLinesAsync(FilePath, cancellationToken));


		IEnumerable<Grid> g(string[] lines)
		{
			foreach (var line in lines)
			{
				if (Grid.TryParse(line, out var grid))
				{
					switch (IgnoringOption)
					{
						case PuzzleLibraryIgnoringOption.Never:
						case PuzzleLibraryIgnoringOption.NotUnique when Solver.CheckValidity(grid.ToString()):
						{
							yield return grid;
							break;
						}
					}
				}
			}
		}
	}

	/// <inheritdoc/>
	public async IAsyncEnumerator<Grid> GetAsyncEnumerator(CancellationToken cancellationToken = default)
	{
		foreach (var line in await File.ReadAllLinesAsync(FilePath, cancellationToken))
		{
			if (Grid.TryParse(line, out var grid))
			{
				switch (IgnoringOption)
				{
					case PuzzleLibraryIgnoringOption.Never:
					case PuzzleLibraryIgnoringOption.NotUnique when Solver.CheckValidity(grid.ToString()):
					{
						yield return grid;
						break;
					}
				}
			}
		}
	}

	/// <inheritdoc/>
	public IEnumerator<Grid> GetEnumerator()
	{
		foreach (var line in File.ReadAllLines(FilePath))
		{
			if (Grid.TryParse(line, out var grid))
			{
				switch (IgnoringOption)
				{
					case PuzzleLibraryIgnoringOption.Never:
					case PuzzleLibraryIgnoringOption.NotUnique when Solver.CheckValidity(grid.ToString()):
					{
						yield return grid;
						break;
					}
				}
			}
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(PuzzleLibrary? left, PuzzleLibrary? right)
		=> (left, right) switch { (null, null) => true, (not null, not null) => left.Equals(right), _ => false };

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(PuzzleLibrary? left, PuzzleLibrary? right) => !(left == right);
}
