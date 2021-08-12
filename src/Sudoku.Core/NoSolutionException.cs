namespace Sudoku
{
	/// <summary>
	/// Indicates an error that throws when a sudoku grid has no solution
	/// while solving, checking or generating a puzzle.
	/// </summary>
	[Serializable]
	public sealed class NoSolutionException : Exception
	{
		/// <summary>
		/// Initializes a <see cref="NoSolutionException"/> with the specified invalid grid.
		/// </summary>
		/// <param name="grid">The invalid sudoku grid.</param>
		public NoSolutionException(in SudokuGrid grid)
		{
			InvalidGrid = grid;
			Data.Add(nameof(InvalidGrid), grid);
		}

		/// <inheritdoc/>
		private NoSolutionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}


		/// <inheritdoc/>
		public override string Message => $"This grid {InvalidGrid.ToString("#")} contains no valid solution.";

		/// <inheritdoc/>
		public override string HelpLink =>
			"https://sunnieshine.github.io/Sudoku/types/exceptions/Exception-NoSolutionException";

		/// <summary>
		/// Indicates the invalid sudoku grid. This property is also stored in the property
		/// <see cref="Exception.Data"/>.
		/// </summary>
		/// <seealso cref="Exception.Data"/>
		public SudokuGrid InvalidGrid { get; }


		/// <inheritdoc/>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue(nameof(InvalidGrid), InvalidGrid.ToString("#"), typeof(string));

			base.GetObjectData(info, context);
		}
	}
}
