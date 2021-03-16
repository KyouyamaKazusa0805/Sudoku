using System;
using System.Runtime.Serialization;
using Sudoku.Data;

namespace Sudoku
{
	/// <summary>
	/// Indicates an error that throws when a sudoku grid has multiple solutions
	/// while solving, checking or generating a puzzle.
	/// </summary>
	[Serializable]
	public sealed class MultipleSolutionsException : Exception
	{
		/// <summary>
		/// Initializes a <see cref="MultipleSolutionsException"/> with the specified invalid grid.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The invalid sudoku grid.</param>
		public MultipleSolutionsException(in SudokuGrid grid)
		{
			InvalidGrid = grid;
			Data.Add(nameof(InvalidGrid), new WeakBox<SudokuGrid>(grid));
		}

		/// <inheritdoc/>
		private MultipleSolutionsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}


		/// <inheritdoc/>
		public override string Message => $"This grid {InvalidGrid.ToString("#")} contains multiple solutions.";

		/// <inheritdoc/>
		public override string HelpLink =>
			"https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3702971&doc_id=633030";

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
