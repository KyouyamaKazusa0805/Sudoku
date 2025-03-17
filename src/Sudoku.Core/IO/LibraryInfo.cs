namespace Sudoku.IO;

/// <summary>
/// Represents information about a library.
/// </summary>
public sealed class LibraryInfo
{
	/// <summary>
	/// Initializes a <see cref="LibraryInfo"/> instance.
	/// </summary>
	/// <param name="name">The name.</param>
	/// <param name="description">The description.</param>
	/// <param name="author">The author.</param>
	/// <param name="tags">The tags.</param>
	[SetsRequiredMembers]
	[JsonConstructor]
	public LibraryInfo(string name, string description, string author, string[] tags)
		=> (Name, Description, Author, Tags) = (name, description, author, tags[..]);

	/// <summary>
	/// Initializes a <see cref="LibraryInfo"/> instance.
	/// </summary>
	[SetsRequiredMembers]
	internal LibraryInfo() : this(string.Empty, string.Empty, string.Empty, [])
	{
	}


	/// <summary>
	/// Indicates the name of the library.
	/// </summary>
	public required string Name { get; set; }

	/// <summary>
	/// Indicates the description to the library.
	/// </summary>
	public required string Description { get; set; }

	/// <summary>
	/// Indicates the author of the library.
	/// </summary>
	public required string Author { get; set; }

	/// <summary>
	/// Indicates the tags of the library.
	/// </summary>
	public required string[] Tags { get; set; }
}
