namespace System.IO;

/// <summary>
/// Extends <see cref="File"/>.
/// </summary>
/// <seealso cref="File"/>
public static class File2
{
	/// <summary>
	/// The field for invalid path characters as a file name.
	/// </summary>
	private static readonly SearchValues<char> InvalidCharacters = SearchValues.Create(@":\/?*<>|""");


	/// <summary>
	/// Determines whether the specified file name is valid.
	/// </summary>
	/// <param name="fileName">The file name to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsValidFileName(string fileName) => !fileName.AsSpan().ContainsAny(InvalidCharacters);
}
