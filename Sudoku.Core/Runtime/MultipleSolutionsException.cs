using System.Runtime.Serialization;
using System;
using Sudoku.Data;

namespace Sudoku.Runtime
{
	/// <summary>
	/// Represents an error that the puzzle has multiple solutions while solving.
	/// </summary>
	[Serializable]
	public class MultipleSolutionsException : SudokuRuntimeException
	{
		/// <summary>
		/// Initializes an instance with a grid.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		public MultipleSolutionsException(in SudokuGrid grid) : base() => Grid = grid;

		/// <summary>
		/// Initializes an instance with a grid and an error message.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="message">The error message.</param>
		public MultipleSolutionsException(in SudokuGrid grid, string message) : base(message) => Grid = grid;

		/// <summary>
		/// Initializes an instance with a grid, an error message and an inner exception.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="message">The error message.</param>
		/// <param name="inner">The inner exception.</param>
		public MultipleSolutionsException(in SudokuGrid grid, string message, Exception inner)
			: base(message, inner) => Grid = grid;

		/// <summary>
		/// Initializes an instance with a grid, a serialization information instance and
		/// a streaming context instance.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="info">The streaming information.</param>
		/// <param name="context">The streaming context.</param>
		protected MultipleSolutionsException(
			in SudokuGrid grid, SerializationInfo info, StreamingContext context) : base(info, context) =>
			Grid = grid;


		/// <summary>
		/// Indicates the error message.
		/// </summary>
		public override string Message => $"The grid '{Grid:#}' has multiple solutions.";

		/// <summary>
		/// The grid.
		/// </summary>
		public SudokuGrid Grid { get; }
	}
}
