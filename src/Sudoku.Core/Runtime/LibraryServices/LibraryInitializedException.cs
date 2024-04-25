namespace Sudoku.Runtime.LibraryServices;

/// <summary>
/// Represents an exception type that will be thrown if a library instance has already been initialized, but a user still calls
/// method <see cref="Library.Initialize"/>.
/// </summary>
/// <param name="directory"><inheritdoc cref="Library(string, string)" path="/param[@name='directory']"/></param>
/// <param name="fileId"><inheritdoc cref="Library(string, string)" path="/param[@name='fileId']"/></param>
/// <remarks><i>
/// This type is only used by Windows platform because the relied type <see cref="Library"/>
/// is marked <see cref="SupportedOSPlatformAttribute"/>, limited in Windows.
/// </i></remarks>
/// <seealso cref="Library.Initialize"/>
[SupportedOSPlatform(PlatformNames.Windows)]
public sealed class LibraryInitializedException(string directory, string fileId) : Exception
{
	/// <summary>
	/// Initializes a <see cref="LibraryInitializedException"/> instance via the specified directory and file ID.
	/// </summary>
	/// <param name="library">The library instance.</param>
	public LibraryInitializedException(Library library) : this(library._directory, library.FileId)
	{
	}


	/// <inheritdoc/>
	public override string Message => string.Format(ResourceDictionary.Get("Message_LibraryInitializedException"), directory, fileId);
}
