namespace Sudoku.Analytics;

partial record AnalyzerResult
{
	/// <summary>
	/// Defines an enumerator type that allows iterating on steps of the containing type.
	/// </summary>
	/// <param name="steps">The steps.</param>
	public ref struct StepsEnumerator(Step[]? steps)
	{
		/// <summary>
		/// The internal enumerator instance.
		/// </summary>
		private readonly Step[]? _steps = steps;

		/// <summary>
		/// Indicates the index.
		/// </summary>
		private int _index = -1;


		/// <inheritdoc cref="IEnumerator.Current"/>
		public Step Current => _steps![_index];


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		public bool MoveNext()
		{
			if (_steps is null)
			{
				return false;
			}

			if (++_index < _steps.Length)
			{
				return true;
			}

			return false;
		}
	}
}
