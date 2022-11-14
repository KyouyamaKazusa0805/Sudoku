namespace Sudoku.Presentation;

partial struct ValueView
{
	partial struct OfTypeEnumerator<T> where T : ViewNode
	{
		/// <summary>
		/// The inner enumerator.
		/// </summary>
		private Enumerator _enumerator;


		/// <summary>
		/// Indicates the current node to be iterated.
		/// </summary>
		public T Current { get; private set; } = null!;


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		public bool MoveNext()
		{
			while (_enumerator.MoveNext())
			{
				if (_enumerator.Current is not T targetNode)
				{
					continue;
				}

				Current = targetNode;
				return true;
			}

			return false;
		}


		/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly OfTypeEnumerator<T> GetEnumerator() => this;
	}
}
