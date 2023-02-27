namespace Sudoku.Platforms.QQ.Models;

/// <summary>
/// Defines a collection that stores a list of puzzle library data <see cref="PuzzleLibraryData"/> instances.
/// </summary>
/// <see cref="PuzzleLibraryData"/>
internal sealed partial class PuzzleLibraryCollection
{
	/// <summary>
	/// Indicates the group ID.
	/// </summary>
	[JsonPropertyOrder(0)]
	public required string GroupId { get; set; }

	/// <summary>
	/// Defines a list of puzzle libraries.
	/// </summary>
	public List<PuzzleLibraryData> PuzzleLibraries { get; set; } = new();

	/// <inheritdoc cref="IReadOnlyCollection{T}.Count"/>
	[JsonIgnore]
	public int Count => PuzzleLibraries.Count;


	/// <summary>
	/// Gets the target puzzle library at the specified index.
	/// </summary>
	/// <param name="index">The desired index.</param>
	/// <returns>The library at the specified index.</returns>
	public PuzzleLibraryData this[int index] => PuzzleLibraries[index];


	/// <summary>
	/// Determines whether the current collection contains the specified puzzle library.
	/// </summary>
	/// <param name="puzzleLibrary">The puzzle library to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public bool Contains(PuzzleLibraryData puzzleLibrary)
	{
		foreach (var element in PuzzleLibraries)
		{
			if (element == puzzleLibrary)
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Determines whether the current collection contains the specified element satisfying the specified condition.
	/// </summary>
	/// <param name="predicate">The condition.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public bool Exists(Predicate<PuzzleLibraryData> predicate)
	{
		foreach (var element in PuzzleLibraries)
		{
			if (predicate(element))
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Gets the index of the element that is equal to the specified library.
	/// </summary>
	/// <param name="puzzleLibrary">The puzzle library to be checked.</param>
	/// <returns>An <see cref="int"/> value indicating the index of the first satisfied element; -1 if none found.</returns>
	public int IndexOf(PuzzleLibraryData puzzleLibrary)
	{
		for (var i = 0; i < PuzzleLibraries.Count; i++)
		{
			if (PuzzleLibraries[i] == puzzleLibrary)
			{
				return i;
			}
		}

		return -1;
	}

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Enumerator GetEnumerator() => new(this);

	/// <summary>
	/// <inheritdoc cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, TResult})" path="/summary"/>
	/// </summary>
	/// <typeparam name="T">
	/// <inheritdoc cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, TResult})" path="/typeparam[@name='TResult']"/>
	/// </typeparam>
	/// <param name="selector">
	/// <inheritdoc cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, TResult})" path="/param[@name='selector']"/>
	/// </param>
	/// <returns>
	/// <inheritdoc cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, TResult})" path="/returns"/>
	/// </returns>
	public IEnumerable<T> Select<T>(Func<PuzzleLibraryData, T> selector)
	{
		foreach (var element in PuzzleLibraries)
		{
			yield return selector(element);
		}
	}
}
