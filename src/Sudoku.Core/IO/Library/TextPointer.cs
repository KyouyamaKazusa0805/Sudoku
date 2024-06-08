namespace Sudoku.IO.Library;

/// <summary>
/// Represents a text pointer object that reads the detail of a <see cref="LibraryInfo"/>.
/// </summary>
/// <remarks><i>
/// This type only supports for Windows now because the relied type <see cref="LibraryInfo"/> is limited in Windows.
/// </i></remarks>
/// <seealso cref="LibraryInfo"/>
[SupportedOSPlatform(PlatformNames.Windows)]
[TypeImpl(TypeImplFlag.AllObjectMethods | TypeImplFlag.EqualityOperators)]
public sealed partial class TextPointer :
	IAdditionOperators<TextPointer, int, TextPointer>,
	IAsyncDisposable,
	IDecrementOperators<TextPointer>,
	IDisposable,
	IEnumerable<string?>,
	IEnumerator<string?>,
	IEqualityOperators<TextPointer, TextPointer, bool>,
	IEquatable<TextPointer>,
	IIncrementOperators<TextPointer>,
	IReadOnlyCollection<string?>,
	ISubtractionOperators<TextPointer, int, TextPointer>
{
	/// <summary>
	/// Indicates the max limit of a puzzle length.
	/// </summary>
	private const int MaxLimitOfPuzzleLength = 4096;


	/// <summary>
	/// Indicates the internal stream.
	/// </summary>
	private readonly FileStream _stream;


	/// <summary>
	/// Initializes a <see cref="TextPointer"/> instance via the specified library.
	/// </summary>
	/// <param name="library">Indicates the libary object.</param>
	/// <exception cref="ArgumentException">Throws when the library is not initialized.</exception>
	public TextPointer(LibraryInfo library)
		=> _stream = (Library = library) switch
		{
			(var p, _) { IsInitialized: true } => File.OpenRead(p),
			(var p, _) => throw new FileNotFoundException(ResourceDictionary.ExceptionMessage("LibraryShouldBeInitialized"), p)
		};


	/// <summary>
	/// Indicates the number of puzzles left to be iterated from the current position. The current puzzle will be included.
	/// </summary>
	[StringMember]
	public int ForwardPuzzlesCount
	{
		get
		{
			var result = 0;
			while (TryReadNextPuzzle(out _))
			{
				result++;
			}

			return result;
		}
	}

	/// <summary>
	/// Indicates the number of puzzles left to be iterated back from the current position. The current puzzle will be included.
	/// </summary>
	[StringMember]
	public int BackPuzzlesCount
	{
		get
		{
			var result = 0;
			while (TryReadPreviousPuzzle(out _))
			{
				result++;
			}

			return result;
		}
	}

	/// <summary>
	/// Indicates the currently-pointed puzzle.
	/// </summary>
	[StringMember]
	public string? Current
	{
		get
		{
			var originalStartPosition = _stream.Position;
			try
			{
				if (_stream.Position == _stream.Length || _stream.Length <= 2)
				{
					return null;
				}

				if (g(originalStartPosition) is var position and not -1
					&& position - originalStartPosition is var delta and >= 0 and <= MaxLimitOfPuzzleLength)
				{
					_stream.Position = originalStartPosition;
					var span = (stackalloc char[(int)delta]);
					for (var i = 0; i < delta; i++)
					{
						span[i] = (char)_stream.ReadByte();
					}

					return span.ToString();
				}

				return null;


				long g(long start)
				{
					_stream.Position = start;
					var playground = (stackalloc char[2]);
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
			finally
			{
				_stream.Position = originalStartPosition;
			}
		}
	}

	/// <summary>
	/// Indicates the number of puzzles stored in the file, regardless of the position of the pointer.
	/// </summary>
	/// <remarks><inheritdoc cref="LibraryInfo.Count" path="/remarks"/></remarks>
	/// <seealso cref="LibraryInfo.GetCountAsync(CancellationToken)"/>
	public int Length => Library.Count;

	/// <summary>
	/// Indicates the library object.
	/// </summary>
	[HashCodeMember]
	[StringMember]
	public LibraryInfo Library { get; }

	/// <inheritdoc/>
	object? IEnumerator.Current => Current;

	/// <inheritdoc/>
	int IReadOnlyCollection<string?>.Count => Length;

	/// <summary>
	/// Indicates the position of the pointer.
	/// </summary>
	[HashCodeMember]
	private long PositionOfPointer => _stream.Position;


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

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals([NotNullWhen(true)] TextPointer? other)
		=> other is not null && Library == other.Library && _stream.Position == other._stream.Position;

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
					if (position - originalStartPosition is var delta and >= 0 and <= MaxLimitOfPuzzleLength)
					{
						_stream.Position = originalStartPosition;

						var span = (stackalloc char[(int)delta]);
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
				var playground = (stackalloc char[2]);
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
					if (originalStartPosition - 2 - position is var delta and >= 0 and <= MaxLimitOfPuzzleLength)
					{
						_stream.Position = position;

						var span = (stackalloc char[(int)delta]);
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
				var playground = (stackalloc char[2]);
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

	/// <summary>
	/// Try to skip the number of puzzles forward, making the pointer point to the next grid after the skipped grids.
	/// </summary>
	/// <param name="count">The desired number of puzzles to be skipped. The default value is 1.</param>
	/// <returns>The number of puzzles skipped in fact.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument <paramref name="count"/> is negative.</exception>
	public int TrySkipNext(int count = 1)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(count);

		for (var i = 0; i < count; i++)
		{
			if (!TryReadNextPuzzle(out _))
			{
				return i;
			}
		}
		return count;
	}

	/// <summary>
	/// Try to skip the number of puzzles back, making the pointer point to the next grid before the skipped grids.
	/// </summary>
	/// <param name="count">The desired number of puzzles to be skipped. The default value is 1.</param>
	/// <returns>The number of puzzles skipped in fact.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument <paramref name="count"/> is negative.</exception>
	public int TrySkipPrevious(int count = 1)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(count);

		for (var i = 0; i < count; i++)
		{
			if (!TryReadPreviousPuzzle(out _))
			{
				return i;
			}
		}
		return count;
	}

	/// <summary>
	/// Fetch the number of puzzles beginning with the current pointer position.
	/// </summary>
	/// <param name="count">The desired number of puzzles.</param>
	/// <param name="result">Indicates the puzzles fetched.</param>
	/// <returns>The number of puzzles fetched.</returns>
	public int TryFetchNext(int count, out ReadOnlySpan<string> result)
	{
		TrySkipNext();

		var r = new string[count];
		var p = 0;
		for (var i = 0; i < count; i++)
		{
			if (TryReadNextPuzzle(out var grid))
			{
				r[p++] = grid;
				continue;
			}

			result = r.AsSpan()[..p];
			return p;
		}

		result = r;
		return count;
	}

	/// <summary>
	/// Fetch the number of puzzles in previous beginning with the current pointer position.
	/// </summary>
	/// <param name="count">The desired number of puzzles.</param>
	/// <param name="result">Indicates the puzzles fetched.</param>
	/// <returns>The number of puzzles fetched.</returns>
	public int TryFetchPrevious(int count, out ReadOnlySpan<string> result)
	{
		TrySkipPrevious();

		var r = new string[count];
		var p = 0;
		for (var i = 0; i < count; i++)
		{
			if (TryReadPreviousPuzzle(out var grid))
			{
				r[p++] = grid;
				continue;
			}

			var final = r.AsSpan()[..p];
			final.Reverse();
			result = final;
			return p;
		}

		result = r;
		return count;
	}

	/// <inheritdoc/>
	public async ValueTask DisposeAsync() => await _stream.DisposeAsync();

	/// <summary>
	/// Returns itself. The method is consumed by <see langword="foreach"/> loops.
	/// </summary>
	/// <returns>A <see cref="TextPointer"/> instance that can iterate on each puzzle stored in library file.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public TextPointer GetEnumerator() => this;

	/// <inheritdoc/>
	void IEnumerator.Reset() => SetStart();

	/// <inheritdoc/>
	bool IEnumerator.MoveNext() => TryReadNextPuzzle(out _);

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => this;

	/// <inheritdoc/>
	IEnumerator<string?> IEnumerable<string?>.GetEnumerator() => this;


	/// <summary>
	/// Moves the pointer to the next puzzle. If the pointer is at the end of the sequence, moves to the first element.
	/// </summary>
	/// <param name="value">The instance to be moved.</param>
	/// <returns>A reference that is same as argument <paramref name="value"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TextPointer operator ++(TextPointer value)
	{
		if (value.TryReadNextPuzzle(out _))
		{
			goto Return;
		}

		value.SetStart();

	Return:
		return value;
	}

	/// <summary>
	/// Moves the pointer to the next puzzle. If the pointer is at the end of the sequence,
	/// throw <see cref="InvalidOperationException"/>.
	/// </summary>
	/// <param name="value">The instance to be moved.</param>
	/// <returns>A reference that is same as argument <paramref name="value"/>.</returns>
	/// <exception cref="InvalidOperationException">Throws when the pointer cannot be moved.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TextPointer operator checked ++(TextPointer value)
		=> value.TryReadNextPuzzle(out _)
			? value
			: throw new InvalidOperationException(ResourceDictionary.ExceptionMessage("PointerCannotMove"));

	/// <summary>
	/// Moves the pointer to the previous puzzle. If the pointer is at the start of the sequence, moves to the last element.
	/// </summary>
	/// <param name="value">The instance to be moved.</param>
	/// <returns>A reference that is same as argument <paramref name="value"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TextPointer operator --(TextPointer value)
	{
		if (value.TryReadPreviousPuzzle(out _))
		{
			goto Return;
		}

		value.SetEnd();

	Return:
		return value;
	}

	/// <summary>
	/// Moves the pointer to the previous puzzle. If the pointer is at the start of the sequence, throw <see cref="InvalidOperationException"/>.
	/// </summary>
	/// <param name="value">The instance to be moved.</param>
	/// <returns>A reference that is same as argument <paramref name="value"/>.</returns>
	/// <exception cref="InvalidOperationException">Throws when the pointer cannot be moved.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TextPointer operator checked --(TextPointer value)
		=> value.TryReadPreviousPuzzle(out _)
			? value
			: throw new InvalidOperationException(ResourceDictionary.ExceptionMessage("PointerCannotMove"));

	/// <summary>
	/// Skips the specified number of puzzles forward.
	/// </summary>
	/// <param name="value">The value to be used.</param>
	/// <param name="count">The number of puzzles to be skipped.</param>
	/// <returns>A reference that is same as argument <paramref name="value"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TextPointer operator +(TextPointer value, int count)
	{
		_ = count > 0 ? value.TrySkipNext(count) : value.TrySkipPrevious(-count);
		return value;
	}

	/// <summary>
	/// Skips the specified number of puzzles forward. If the pointer has already moved to the last element,
	/// throw <see cref="InvalidOperationException"/>.
	/// </summary>
	/// <param name="value">The value to be used.</param>
	/// <param name="count">The number of puzzles to be skipped.</param>
	/// <returns>A reference that is same as argument <paramref name="value"/>.</returns>
	/// <exception cref="InvalidOperationException">Throws when no elements can be skipped.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TextPointer operator checked +(TextPointer value, int count)
		=> (count > 0 ? value.TrySkipNext(count) : value.TrySkipPrevious(count)) == count
			? value
			: throw new InvalidOperationException(ResourceDictionary.ExceptionMessage("PointerCannotMove"));

	/// <summary>
	/// Skips the specified number of puzzles back.
	/// </summary>
	/// <param name="value">The value to be used.</param>
	/// <param name="count">The number of puzzles to be skipped.</param>
	/// <returns>A reference that is same as argument <paramref name="value"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TextPointer operator -(TextPointer value, int count)
	{
		_ = count > 0 ? value.TrySkipPrevious(count) : value.TrySkipNext(-count);
		return value;
	}

	/// <summary>
	/// Skips the specified number of puzzles back. If the pointer has already moved to the first element,
	/// throw <see cref="InvalidOperationException"/>.
	/// </summary>
	/// <param name="value">The value to be used.</param>
	/// <param name="count">The number of puzzles to be skipped.</param>
	/// <returns>A reference that is same as argument <paramref name="value"/>.</returns>
	/// <exception cref="InvalidOperationException">Throws when no elements can be skipped.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TextPointer operator checked -(TextPointer value, int count)
		=> (count > 0 ? value.TrySkipPrevious(count) : value.TrySkipNext(count)) == count
			? value
			: throw new InvalidOperationException(ResourceDictionary.ExceptionMessage("PointerCannotMove"));
}
