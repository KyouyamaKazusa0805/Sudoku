namespace Sudoku.Platforms.QQ.Models;

/// <summary>
/// Defines a collection that stores a list of puzzle library data <see cref="PuzzleLibraryData"/> instances.
/// </summary>
/// <see cref="PuzzleLibraryData"/>
internal sealed partial class PuzzleLibraryCollection :
	IEnumerable<PuzzleLibraryData>,
	IReadOnlyCollection<PuzzleLibraryData>,
	IReadOnlyList<PuzzleLibraryData>,
	ISlicable<PuzzleLibraryCollection, PuzzleLibraryData>
{
	/// <summary>
	/// Indicates the group ID.
	/// </summary>
	[JsonPropertyName("groupId")]
	[JsonPropertyOrder(0)]
	public required string GroupId { get; set; }

	/// <summary>
	/// Defines a list of puzzle libraries.
	/// </summary>
	[JsonPropertyName("libraries")]
	public List<PuzzleLibraryData> PuzzleLibraries { get; set; } = new();

	/// <inheritdoc/>
	[JsonIgnore]
	public int Count => PuzzleLibraries.Count;

	/// <inheritdoc/>
	int ISlicable<PuzzleLibraryCollection, PuzzleLibraryData>.Length => Count;


	/// <inheritdoc cref="IReadOnlyList{T}.this[int]"/>
	public PuzzleLibraryData this[int index] => PuzzleLibraries[index];


	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Enumerator GetEnumerator() => new(this);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PuzzleLibraryCollection Slice(int start, int count)
		=> new() { GroupId = GroupId, PuzzleLibraries = PuzzleLibraries.Slice(start, count) };

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator<PuzzleLibraryData> IEnumerable<PuzzleLibraryData>.GetEnumerator() => PuzzleLibraries.GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator() => PuzzleLibraries.GetEnumerator();


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
