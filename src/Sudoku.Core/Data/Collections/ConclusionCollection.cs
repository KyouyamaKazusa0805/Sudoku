namespace Sudoku.Data.Collections;

/// <summary>
/// Provides a collection that contains the conclusions.
/// </summary>
[AutoEquality(nameof(_collection))]
public readonly ref partial struct ConclusionCollection
{
	/// <summary>
	/// The internal collection.
	/// </summary>
	private readonly ReadOnlySpan<Conclusion> _collection;


	/// <summary>
	/// Initializes an instance with the specified collection.
	/// </summary>
	/// <param name="collection">The collection.</param>
	public ConclusionCollection(in ReadOnlySpan<Conclusion> collection) : this() => _collection = collection;

	/// <summary>
	/// Initializes an instance with the specified collection. 
	/// </summary>
	/// <param name="collection">The collection.</param>
	public ConclusionCollection(in ImmutableArray<Conclusion> collection) : this() =>
		_collection = collection.ToArray();

	/// <summary>
	/// Initializes an instance with the specified collection.
	/// </summary>
	/// <param name="collection">The collection.</param>
	public ConclusionCollection(IEnumerable<Conclusion> collection) : this() =>
		_collection = collection.ToArray().AsSpan();


	/// <summary>
	/// Indicates all cells used in this conclusions list.
	/// </summary>
	public Cells Cells => new(from conclusion in _collection select conclusion.Cell);

	/// <summary>
	/// Indicates all digits used in this conclusions list, represented as a mask.
	/// </summary>
	public short Digits
	{
		get
		{
			short result = 0;
			foreach (var conclusion in _collection)
			{
				result |= (short)(1 << conclusion.Digit);
			}

			return result;
		}
	}


	/// <inheritdoc cref="object.ToString"/>
	public override string ToString() => ToString(true, ", ");

	/// <summary>
	/// Converts the current instance to <see cref="string"/> with the specified separator.
	/// </summary>
	/// <param name="shouldSort">Indicates whether the specified collection should be sorted first.</param>
	/// <param name="separator">The separator.</param>
	/// <returns>The string result.</returns>
	public string ToString(bool shouldSort, string separator)
	{
		return _collection.Length switch
		{
			0 => string.Empty,
			1 => _collection[0].ToString(),
			_ => internalToString(_collection)
		};

		unsafe string internalToString(in ReadOnlySpan<Conclusion> collection)
		{
			var conclusions = collection.ToArray();
			var sb = new ValueStringBuilder(stackalloc char[50]);
			if (shouldSort)
			{
				conclusions.Sort(&cmp);

				var selection =
					from conclusion in conclusions
					orderby conclusion.Digit
					group conclusion by conclusion.ConclusionType;
				bool hasOnlyOneType = selection.HasOnlyOneElement();
				foreach (var typeGroup in selection)
				{
					string op = typeGroup.Key == ConclusionType.Assignment ? " = " : " <> ";
					foreach (var digitGroup in
						from conclusion in typeGroup
						group conclusion by conclusion.Digit)
					{
						sb.Append(new Cells(from conclusion in digitGroup select conclusion.Cell));
						sb.Append(op);
						sb.Append(digitGroup.Key + 1);
						sb.Append(separator);
					}

					sb.RemoveFromEnd(separator.Length);
					if (!hasOnlyOneType)
					{
						sb.Append(separator);
					}
				}

				if (!hasOnlyOneType)
				{
					sb.RemoveFromEnd(separator.Length);
				}
			}
			else
			{
				sb.AppendRange(conclusions, static c => c.ToString(), separator);
			}

			return sb.ToStringAndClear();
		}


		static int cmp(in Conclusion left, in Conclusion right) => left.CompareTo(right);
	}
}
