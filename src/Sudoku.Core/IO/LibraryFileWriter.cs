namespace Sudoku.IO;

/// <summary>
/// Represents a UTF-8 format library file writer.
/// </summary>
[TypeImpl(TypeImplFlags.AsyncDisposable)]
internal sealed partial class LibraryFileWriter : IAsyncDisposable
{
	/// <summary>
	/// Indicates the flush threashold. If the maximum number of characters is reached, a flush operation will be triggered.
	/// </summary>
	private const int FlushThreshold = 10000;


	/// <summary>
	/// Indicates the writer.
	/// </summary>
	[DisposableMember]
	private readonly StreamWriter _writer;

	/// <summary>
	/// Indicates the semaphore.
	/// </summary>
	private readonly SemaphoreSlim _semaphore = new(1, 1);

	/// <summary>
	/// Indicates the buffer.
	/// </summary>
	private readonly List<string> _buffer = [];


	/// <summary>
	/// Initializes a <see cref="LibraryFileWriter"/> instance.
	/// </summary>
	/// <param name="filePath">The file path.</param>
	/// <param name="bufferSize">The buffer size.</param>
	/// <param name="exists">Indicates whether the file exists.</param>
	public LibraryFileWriter(string filePath, int bufferSize, out bool exists)
	{
		(FilePath, BufferSize) = (filePath, bufferSize);
		exists = File.Exists(filePath);
		_writer = new(
			new FileStream(
				filePath,
				FileMode.Append,
				FileAccess.Write,
				FileShare.Read,
				bufferSize,
				FileOptions.Asynchronous
			),
			Encoding.UTF8
		);
	}


	/// <summary>
	/// Indicates the file path.
	/// </summary>
	public string FilePath { get; }

	/// <summary>
	/// Indicates the buffer size.
	/// </summary>
	public int BufferSize { get; }


	/// <summary>
	/// Initializes a <see cref="LibraryFileWriter"/> instance via the specified file path;
	/// if the specified path doesn't exist, a new file will be created.
	/// </summary>
	/// <param name="filePath">The file path.</param>
	/// <param name="exists">Indicates whether the file exists.</param>
	public LibraryFileWriter(string filePath, out bool exists) : this(filePath, 4096 * 4, out exists)
	{
	}


	/// <summary>
	/// Indicates whether the flush operation will be automatically triggered.
	/// </summary>
	public bool AutoFlush { get; init; }


	/// <summary>
	/// Write a new line of values into the file.
	/// </summary>
	/// <param name="line">A line.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel the current operation.</param>
	/// <returns>A <see cref="Task"/> object that can be used as asynchronous operation.</returns>
	public async Task WriteLineAsync(string line, CancellationToken cancellationToken = default)
	{
		await _semaphore.WaitAsync(cancellationToken);

		try
		{
			_buffer.Add(line);
			if (AutoFlush || _buffer.Count >= FlushThreshold)
			{
				await FlushInternalAsync();
			}
		}
		finally
		{
			_semaphore.Release();
		}
	}

	/// <summary>
	/// Flushes the buffer and write them into file.
	/// </summary>
	/// <returns>A <see cref="Task"/> object that can be used as asynchronous operation.</returns>
	public async Task FlushAsync()
	{
		await _semaphore.WaitAsync();

		try
		{
			await FlushInternalAsync();
		}
		finally
		{
			_semaphore.Release();
		}
	}

	/// <summary>
	/// The backing logic on flushing the buffer, writting the buffer into the file.
	/// </summary>
	/// <returns>A <see cref="Task"/> object that can be used as asynchronous operation.</returns>
	private async Task FlushInternalAsync()
	{
		if (_buffer.Count == 0)
		{
			return;
		}

		await _writer.WriteAsync(string.Join("\n", _buffer));
		await _writer.FlushAsync();
		_buffer.Clear();
	}
}
