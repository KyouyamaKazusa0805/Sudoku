namespace Sudoku.Solving.Logical;

partial record LogicalSolverResult
{
	/// <summary>
	/// Defines an enumerator type that allows iterating on steps of the containing type.
	/// </summary>
	public ref struct StepsEnumerator
	{
		/// <summary>
		/// The internal enumerator instance.
		/// </summary>
		private ImmutableArray<IStep>.Enumerator _enumerator;


		/// <summary>
		/// Initializes a <see cref="StepsEnumerator"/> instance via the enumerator.
		/// </summary>
		/// <param name="enumerator">The internal enumerator instance.</param>
		internal StepsEnumerator(ImmutableArray<IStep>.Enumerator enumerator) => _enumerator = enumerator;


		/// <inheritdoc cref="IEnumerator.Current"/>
		public IStep Current => _enumerator.Current;


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		public bool MoveNext() => _enumerator.MoveNext();
	}
}
