namespace Sudoku.IO;

/// <summary>
/// Represents a UTF-8 format library file reader.
/// </summary>
/// <param name="filePath">The file path.</param>
/// <param name="bufferSize">The buffer size.</param>
[TypeImpl(TypeImplFlags.AsyncDisposable)]
public sealed partial class LibraryFileReader([Property] string filePath, [Property] int bufferSize) : IAsyncDisposable
{
	/// <summary>
	/// Indicates the reader stream.
	/// </summary>
	[DisposableMember]
	private readonly FileStream _readerStream = new(
		filePath,
		FileMode.Open,
		FileAccess.Read,
		FileShare.Read,
		bufferSize,
		FileOptions.Asynchronous | FileOptions.SequentialScan
	);


	/// <summary>
	/// Initializes a <see cref="LibraryFileReader"/> instance via the specified file path.
	/// </summary>
	/// <param name="path">The file path.</param>
	public LibraryFileReader(string path) : this(path, 4096 * 4)
	{
	}


	/// <summary>
	/// Asynchronously read for the specified lines.
	/// </summary>
	/// <param name="startLine">Indicates the start line index.</param>
	/// <param name="endLine">Indicates the end line index.</param>
	/// <param name="cancellationToken">Indicates the cancellation token.</param>
	/// <returns>An asynchronous enumerator instance.</returns>
	public async IAsyncEnumerable<string> ReadLinesRangeAsync(
		int startLine,
		int endLine,
		[EnumeratorCancellation] CancellationToken cancellationToken = default
	)
	{
		var buffer = new byte[BufferSize];
		var remainingBytes = new List<byte>(256);
		var currentLineNumber = 0;
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();

			// Read the block asynchronously.
			var bytesRead = await _readerStream.ReadAsync(buffer, cancellationToken);
			if (bytesRead == 0)
			{
				break;
			}

			// To parse the lines.
			var (lines, newRemaining) = parseLines(buffer.AsMemory(0, bytesRead), remainingBytes);

			// Hanlde for the range of valid line number.
			foreach (var line in lines)
			{
				currentLineNumber++;
				if (currentLineNumber > endLine)
				{
					// Unexpected end-of-line.
					yield break;
				}

				if (currentLineNumber > startLine)
				{
					yield return line;
				}
			}

			remainingBytes = newRemaining;
			if (currentLineNumber >= endLine)
			{
				break;
			}
		}

		// Handle for the last.
		if (remainingBytes.Count > 0 && currentLineNumber < endLine)
		{
			currentLineNumber++;
			if (currentLineNumber > startLine)
			{
				yield return Encoding.UTF8.GetString(remainingBytes.AsSpan());
			}
		}


		static (List<string> Lines, List<byte> Remaining) parseLines(ReadOnlyMemory<byte> data, List<byte> previousRemaining)
		{
			var lines = new List<string>();
			var currentLine = new List<byte>(previousRemaining);
			var span = data.Span;
			for (var i = 0; i < span.Length; i++)
			{
				var b = span[i];
				if (b == (byte)'\n')
				{
					if (currentLine.Count > 0 && currentLine[^1] == (byte)'\r')
					{
						currentLine.RemoveAt(currentLine.Count - 1);
					}

					lines.Add(Encoding.UTF8.GetString(currentLine.AsSpan()));
					currentLine.Clear();
				}
				else
				{
					currentLine.Add(b);
				}
			}
			return (lines, currentLine);
		}
	}

	/// <summary>
	/// Loads the puzzles line by line, and enumerate them without loading them together.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token that can cancel the current operation.</param>
	/// <returns>An enumerator object that allows iterating values asynchronously.</returns>
	public IAsyncEnumerable<string> ReadLinesAsync(CancellationToken cancellationToken = default)
		=> ReadLinesRangeAsync(1, int.MaxValue, cancellationToken: cancellationToken);
}
