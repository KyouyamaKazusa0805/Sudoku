namespace Sudoku.Presentation;

partial struct ValueView
{
	partial struct OfTypeEnumerator<TViewNode>
	{
		/// <summary>
		/// The inner enumerator.
		/// </summary>
		private Enumerator _enumerator;


		/// <summary>
		/// Indicates the current node to be iterated.
		/// </summary>
		public TViewNode Current { get; private set; } = null!;


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		public bool MoveNext()
		{
			while (_enumerator.MoveNext())
			{
				if (_enumerator.Current is not TViewNode targetNode)
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
		public readonly OfTypeEnumerator<TViewNode> GetEnumerator() => this;
	}
}
