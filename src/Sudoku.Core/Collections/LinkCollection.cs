namespace Sudoku.Collections;

/// <summary>
/// Provides a collection that contains the chain links.
/// </summary>
public readonly ref partial struct LinkCollection
{
	/// <summary>
	/// The internal collection.
	/// </summary>
	private readonly Span<Link> _collection;


	/// <summary>
	/// Initializes an instance with the specified collection.
	/// </summary>
	/// <param name="collection">The collection.</param>
	public LinkCollection(in Span<Link> collection) : this() => _collection = collection;

	/// <summary>
	/// Initializes an instance with the specified collection.
	/// </summary>
	/// <param name="collection">The collection.</param>
	public LinkCollection(IEnumerable<Link> collection) : this() => _collection = collection.ToArray().AsSpan();


	/// <summary>
	/// Determine whether two collections are equal.
	/// </summary>
	/// <param name="other">The collection to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(in LinkCollection other) => _collection == other._collection;

	/// <inheritdoc cref="object.ToString"/>
	public override string ToString()
	{
		return _collection switch
		{
			[] => string.Empty,
			[var link] => link.ToString(),
			_ => f(_collection)
		};


		static string f(in Span<Link> collection)
		{
			var sb = new StringHandler(initialCapacity: 100);
			foreach (var (start, _, type) in collection)
			{
				sb.Append(new Candidates { start }.ToString());
				sb.Append(type.GetNotation());
			}
			sb.Append(new Candidates { collection[^1].EndCandidate }.ToString());

			// Remove redundant digit labels:
			// r1c1(1) == r1c2(1) --> r1c1 == r1c2(1).
			var list = new List<(int Pos, char Value)>();
			for (int i = 0, length = sb.Length; i < length; i++)
			{
				if (sb[i] == '(')
				{
					list.Add((i, sb[i + 1]));
					i += 2;
				}
			}

			char digit = list[^1].Value;
			for (int i = list.Count - 1; i >= 1; i--)
			{
				var (prevPos, prevValue) = list[i - 1];
				if (prevValue == digit)
				{
					sb.Remove(prevPos, 3);
				}

				digit = prevValue;
			}

			return sb.ToStringAndClear();
		}
	}
}
