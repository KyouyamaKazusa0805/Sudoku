namespace Sudoku.Concepts.Solving;

partial struct SolvingPath
{
	/// <summary>
	/// Defines an enumerator type that can iterate an instance of type <see cref="SolvingPath"/>.
	/// </summary>
	public ref partial struct Enumerator
	{
		/// <summary>
		/// Indicates the solving path.
		/// </summary>
		private readonly SolvingPath _instance;

		/// <summary>
		/// Indicates the current index.
		/// </summary>
		private int _index = -1;


		/// <summary>
		/// Initializes an <see cref="Enumerator"/> instance via the specified instance.
		/// </summary>
		/// <param name="solvingPath">The solving path.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Enumerator(SolvingPath solvingPath) => _instance = solvingPath;


		/// <inheritdoc cref="IEnumerator{T}.Current"/>
		public readonly (Grid Grid, Step Step) Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _instance[_index];
		}


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			if (_index >= _instance.Length)
			{
				return false;
			}

			_index++;
			return true;
		}
	}
}
