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


	/// <inheritdoc cref="IReadOnlyList{T}.this[int]"/>
	public PuzzleLibraryData this[int index] => PuzzleLibraries[index];


	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Enumerator GetEnumerator() => new(this);


	/// <summary>
	/// Defines the internal enumerator of this type.
	/// </summary>
	public ref partial struct Enumerator
	{
		/// <summary>
		/// Initializes an <see cref="Enumerator"/> instance via the specified <see cref="PuzzleLibraryCollection"/> instance.
		/// </summary>
		/// <param name="collection">The <see cref="PuzzleLibraryCollection"/> instance.</param>
		[FileAccessOnly]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Enumerator(PuzzleLibraryCollection collection) => _enumerator = collection.PuzzleLibraries.GetEnumerator();
	}
}
