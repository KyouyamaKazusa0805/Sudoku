using System;
using System.Runtime.Serialization;
using Sudoku.Data;

namespace Sudoku
{
	/// <summary>
	/// Indicates an error that throws when a sudoku grid is in an invalid state in other scenarios
	/// that <see cref="MultipleSolutionsException"/> or <see cref="NoSolutionException"/> can't cover.
	/// </summary>
	[Serializable]
	public sealed class InvalidPuzzleException : Exception
	{
		/// <summary>
		/// Initializes a <see cref="InvalidPuzzleException"/> with the specified invalid grid,
		/// and the reason why throws this exception instance.
		/// </summary>
		/// <param name="grid">The invalid sudoku grid.</param>
		/// <param name="reason">The reason why the exception throws.</param>
		public InvalidPuzzleException(in SudokuGrid grid, string reason)
		{
			InvalidGrid = grid;
			Reason = reason;
			Data.Add(nameof(InvalidGrid), grid);
			Data.Add(nameof(Reason), reason);
		}

		/// <inheritdoc/>
		private InvalidPuzzleException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}


		/// <summary>
		/// Indicates the reason why the exception throws.
		/// </summary>
		public string? Reason { get; }

		/// <inheritdoc/>
		public override string Message =>
			$"This grid {InvalidGrid.ToString("#")} is invalid{(Reason is null ? "." : $" because {Reason}.")}";

		/// <inheritdoc/>
		public override string HelpLink =>
			"https://sunnieshine.github.io/Sudoku/types/exceptions/Exception-InvalidPuzzleException";

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
			info.AddValue(nameof(Reason), Reason, typeof(string));

			base.GetObjectData(info, context);
		}
	}
}
