namespace Sudoku.Runtime.LibraryServices;

/// <summary>
/// Represents a text pointer object that reads the detail of a library.
/// </summary>
/// <remarks><i>
/// This type only supports for Windows now because the relied type <see cref="LibraryServices.Library"/> is limited in Windows.
/// </i></remarks>
/// <seealso cref="LibraryServices.Library"/>
[SupportedOSPlatform("windows")]
public sealed class TextPointer : IDisposable, IAsyncDisposable
{
	/// <summary>
	/// Indicates the "Library should be initialized" message.
	/// </summary>
	private const string Error_LibraryShouldBeInitialized = "The library is not initialized. It must be initialized file, ensuring the file in local exists.";

	/// <summary>
	/// Indicates the "Library file not found" message.
	/// </summary>
	private const string Error_LibraryFilePathNotExist = "Library file is not found.";


	/// <summary>
	/// Indicates the internal stream.
	/// </summary>
	private readonly FileStream _stream;

	/// <summary>
	/// Indicates the reader that easily reads the file stream.
	/// </summary>
	private readonly StreamReader _reader;


	/// <summary>
	/// Initializes a <see cref="TextPointer"/> instance via the specified library.
	/// </summary>
	/// <param name="library">Indicates the libary object.</param>
	/// <exception cref="ArgumentException">Throws when the library is not initialized.</exception>
	public TextPointer(Library library)
	{
		_stream = library switch
		{
			{ IsInitialized: true, LibraryFilePath: var filePath } => File.OpenRead(filePath),
			_ => throw new ArgumentException(
				Error_LibraryShouldBeInitialized,
				nameof(library),
				new FileNotFoundException(Error_LibraryFilePathNotExist, library.LibraryFilePath)
			)
		};
		_reader = new(_stream);
		Library = library;
	}


	/// <summary>
	/// Indicates the library object.
	/// </summary>
	public Library Library { get; }


	/// <inheritdoc/>
	public void Dispose()
	{
		_stream.Dispose();
		_reader.Dispose();
	}

	/// <summary>
	/// Try to read the next puzzle beginning with the current text pointer position.
	/// </summary>
	/// <param name="result">The result of the grid, represented as a <see cref="string"/> result.</param>
	/// <returns>A <see cref="bool"/> result indicating whether the file exists the next grid.</returns>
	public bool TryReadNextPuzzle([NotNullWhen(true)] out string? result)
	{
		if (_reader.ReadLine() is { } r && Grid.TryParse(r, out _))
		{
			result = r;
			return true;
		}

		result = null;
		return false;
	}

	/// <summary>
	/// Try to read the previous puzzle beginning with the current text pointer position.
	/// </summary>
	/// <param name="result">The result of the grid, represented as a <see cref="string"/> result.</param>
	/// <returns>A <see cref="bool"/> result indicating whether the file exists the previous grid.</returns>
	public bool TryReadPreviousPuzzle([NotNullWhen(true)] out string? result)
	{
		if (_reader.ReadPreviousLine() is { } r && Grid.TryParse(r, out _))
		{
			result = r;
			return true;
		}

		result = null;
		return false;
	}

	/// <inheritdoc/>
	public async ValueTask DisposeAsync()
	{
		await _stream.DisposeAsync();
		_reader.Dispose();
	}
}
