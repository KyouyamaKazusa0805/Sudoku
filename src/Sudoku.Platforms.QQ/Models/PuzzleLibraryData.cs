namespace Sudoku.Platforms.QQ.Models;

/// <summary>
/// Indicates a puzzle library.
/// </summary>
internal sealed partial class PuzzleLibraryData : IEquatable<PuzzleLibraryData>, IEqualityOperators<PuzzleLibraryData, PuzzleLibraryData, bool>
{
	/// <summary>
	/// Indicates the name of the library.
	/// </summary>
	[JsonPropertyOrder(0)]
	public required string Name { get; set; }

	/// <summary>
	/// Indicates the name of the group ID.
	/// </summary>
	[JsonPropertyOrder(1)]
	public required string GroupId { get; set; }

	/// <summary>
	/// Indicates the description of the puzzle library.
	/// </summary>
	public string? Description { get; set; }

	/// <summary>
	/// Indicates the puzzles in the library.
	/// </summary>
	public required string PuzzleFilePath { get; set; }

	/// <summary>
	/// Indicates the tags of the library. For example, "Hard".
	/// </summary>
	public string[]? Tags { get; set; }

	/// <summary>
	/// Indicates how many puzzles the library has been finished.
	/// </summary>
	public int FinishedPuzzlesCount { get; set; }

	/// <summary>
	/// Indicates whether the puzzle should render candidates after the puzzle extracted.
	/// </summary>
	public bool IsAutoPencilmarking { get; set; }


	[GeneratedOverriddingMember(GeneratedEqualsBehavior.AsCastAndCallingOverloading)]
	public override partial bool Equals(object? obj);

	/// <inheritdoc/>
	/// <remarks>
	/// Please note that the equality of two <see cref="PuzzleLibraryData"/> instances only checks for their properties
	/// <see cref="Name"/> and <see cref="GroupId"/>. Other properties will not be checked even if they may be different.
	/// </remarks>
	public bool Equals([NotNullWhen(true)] PuzzleLibraryData? other) => other is not null && GroupId == other.GroupId && Name == other.Name;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(GroupId), nameof(Name))]
	public override partial int GetHashCode();


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(PuzzleLibraryData? left, PuzzleLibraryData? right)
		=> (left, right) switch { (null, null) => true, (not null, not null) => left.Equals(right), _ => false };

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(PuzzleLibraryData? left, PuzzleLibraryData? right) => !(left == right);
}
