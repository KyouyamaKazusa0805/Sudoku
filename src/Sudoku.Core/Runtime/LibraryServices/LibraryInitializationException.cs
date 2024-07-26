namespace Sudoku.Runtime.LibraryServices;

/// <summary>
/// Represents an exception type that will be thrown if a library instance has already been initialized, but a user still calls
/// method <see cref="LibraryInfo.Initialize"/>.
/// </summary>
/// <param name="directory"><inheritdoc cref="LibraryInfo(string, string)" path="/param[@name='directory']"/></param>
/// <param name="fileId"><inheritdoc cref="LibraryInfo(string, string)" path="/param[@name='fileId']"/></param>
/// <remarks><i>
/// This type is only used by Windows platform because the relied type <see cref="LibraryInfo"/>
/// is marked <see cref="SupportedOSPlatformAttribute"/>, limited in Windows.
/// </i></remarks>
/// <seealso cref="LibraryInfo.Initialize"/>
[SupportedOSPlatform(PlatformNames.Windows)]
public sealed class LibraryInitializationException(string directory, string fileId) : Exception
{
	/// <summary>
	/// Initializes a <see cref="LibraryInitializationException"/> instance via the specified directory and file ID.
	/// </summary>
	/// <param name="library">The library instance.</param>
	public LibraryInitializationException(LibraryInfo library) : this(library._directory, library.FileId)
	{
	}


	/// <inheritdoc/>
	public override string Message => string.Format(SR.Get("Message_LibraryInitializedException"), [directory, fileId]);
}
