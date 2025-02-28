namespace Sudoku.Concepts.Supersymmetry;

public partial struct SpaceSet
{
	/// <summary>
	/// Represents an enumerator type that can iterate on each state bit.
	/// </summary>
	/// <param name="set">The set.</param>
	public ref struct BitmapEnumerator(in SpaceSet set) : IEnumerator<bool>, IEnumerable<bool>
	{
		/// <summary>
		/// Indicates the backing set.
		/// </summary>
		private readonly ref readonly SpaceSet _set = ref set;

		/// <summary>
		/// Indicates the current index.
		/// </summary>
		private int _index = -1;


		/// <inheritdoc/>
		public readonly bool Current => _set._field[_index / 81].Contains(_index % 81);

		/// <inheritdoc/>
		readonly object IEnumerator.Current => Current;

		/// <summary>
		/// Returns a state array.
		/// </summary>
		private readonly bool[] StatesArray
		{
			get
			{
				var result = new bool[324];
				for (var i = 0; i < 324; i++)
				{
					result[i] = _set._field[i / 81].Contains(i % 81);
				}
				return result;
			}
		}


		/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
		public readonly BitmapEnumerator GetEnumerator() => this;

		/// <inheritdoc/>
		public bool MoveNext() => ++_index < 324;

		/// <inheritdoc/>
		readonly void IDisposable.Dispose() { }

		/// <inheritdoc/>
		[DoesNotReturn]
		readonly void IEnumerator.Reset() => throw new NotSupportedException();

		/// <inheritdoc/>
		readonly IEnumerator IEnumerable.GetEnumerator() => StatesArray.GetEnumerator();

		/// <inheritdoc/>
		readonly IEnumerator<bool> IEnumerable<bool>.GetEnumerator() => StatesArray.AsEnumerable().GetEnumerator();
	}
}
