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
	/// Indicates the internal stream.
	/// </summary>
	private readonly FileStream _stream;


	/// <summary>
	/// Initializes a <see cref="TextPointer"/> instance via the specified library.
	/// </summary>
	/// <param name="library">Indicates the libary object.</param>
	/// <exception cref="ArgumentException">Throws when the library is not initialized.</exception>
	public TextPointer(Library library)
		=> _stream = (Library = library) switch
		{
			(var p, _) { IsInitialized: true } => File.OpenRead(p),
			(var p, _) => throw new FileNotFoundException(Error_LibraryShouldBeInitialized, p)
		};


	/// <summary>
	/// Indicates the library object.
	/// </summary>
	public Library Library { get; }


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Dispose() => _stream.Dispose();

	/// <summary>
	/// Sets the pointer to the start position, 0.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetStart() => _stream.Position = 0;

	/// <summary>
	/// Sets the pointer to the end position, the length of the stream.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetEnd() => _stream.Position = _stream.Length;

	/// <summary>
	/// Try to read the next puzzle beginning with the current text pointer position.
	/// </summary>
	/// <param name="result">The result of the grid, represented as a <see cref="string"/> result.</param>
	/// <returns>A <see cref="bool"/> result indicating whether the file exists the next grid.</returns>
	public bool TryReadNextPuzzle([NotNullWhen(true)] out string? result)
	{
		while (f(out result))
		{
			if (Grid.TryParse(result, out _))
			{
				return true;
			}
		}
		result = null;
		return false;


		bool f([NotNullWhen(true)] out string? result)
		{
			if (_stream.Position == _stream.Length || _stream.Length <= 2)
			{
				result = null;
				return false;
			}

			var originalStartPosition = _stream.Position;
			switch (g(originalStartPosition))
			{
				case var position and not -1:
				{
					if (position - originalStartPosition is var delta and >= 0 and <= int.MaxValue)
					{
						_stream.Position = originalStartPosition;

						scoped var span = (stackalloc char[(int)delta]);
						for (var i = 0; i < delta; i++)
						{
							span[i] = (char)_stream.ReadByte();
						}

						result = span.ToString();
						_stream.Position += 2;
						return true;
					}

					goto default;
				}
				default:
				{
					result = null;
					return false;
				}
			}


			long g(long start)
			{
				_stream.Position = start;
				scoped var playground = (stackalloc char[2]);
				for (var i = start; _stream.Position < _stream.Length && i < _stream.Length; i++, _stream.Position--)
				{
					(playground[0], playground[1]) = ((char)_stream.ReadByte(), (char)_stream.ReadByte());
					if (playground is "\r\n")
					{
						return i;
					}
				}
				return -1;
			}
		}
	}

	/// <summary>
	/// Try to read the previous puzzle beginning with the current text pointer position.
	/// </summary>
	/// <param name="result">The result of the grid, represented as a <see cref="string"/> result.</param>
	/// <returns>A <see cref="bool"/> result indicating whether the file exists the previous grid.</returns>
	public bool TryReadPreviousPuzzle([NotNullWhen(true)] out string? result)
	{
		while (f(out result))
		{
			if (Grid.TryParse(result, out _))
			{
				return true;
			}
		}
		result = null;
		return false;


		bool f([NotNullWhen(true)] out string? result)
		{
			if (_stream.Position == 0 || _stream.Length <= 3)
			{
				result = null;
				return false;
			}

			var originalStartPosition = _stream.Position;
			switch (g(originalStartPosition))
			{
				case var position and not -1:
				{
					if (originalStartPosition - 2 - position is var delta and >= 0 and <= int.MaxValue)
					{
						_stream.Position = position;

						scoped var span = (stackalloc char[(int)delta]);
						for (var i = 0; i < delta; i++)
						{
							span[i] = (char)_stream.ReadByte();
						}

						result = span.ToString();
						_stream.Position = position;
						return true;
					}

					goto default;
				}
				default:
				{
					result = null;
					return false;
				}
			}


			long g(long start)
			{
				_stream.Position = start - 3;
				scoped var playground = (stackalloc char[2]);
				var i = start - 3;
				for (; _stream.Position >= 3 && i >= 3; i--, _stream.Position -= 3)
				{
					(playground[0], playground[1]) = ((char)_stream.ReadByte(), (char)_stream.ReadByte());
					if (playground is "\r\n")
					{
						return i + 2;
					}
				}
				return i > 0 ? 0 : -1;
			}
		}
	}

	/// <inheritdoc/>
	public async ValueTask DisposeAsync() => await _stream.DisposeAsync();
}
