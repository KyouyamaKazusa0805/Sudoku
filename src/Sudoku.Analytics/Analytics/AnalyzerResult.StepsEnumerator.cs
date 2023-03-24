namespace Sudoku.Analytics;

partial record AnalyzerResult
{
	/// <summary>
	/// Defines an enumerator type that allows iterating on steps of the containing type.
	/// </summary>
	public ref struct StepsEnumerator
	{
		/// <summary>
		/// The internal enumerator instance.
		/// </summary>
		private readonly Step[]? _steps;

		/// <summary>
		/// Indicates the index.
		/// </summary>
		private int _index = -1;


		/// <summary>
		/// Initializes a <see cref="StepsEnumerator"/> instance via the steps.
		/// </summary>
		/// <param name="steps">The steps.</param>
		internal StepsEnumerator(Step[]? steps) => _steps = steps;


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
