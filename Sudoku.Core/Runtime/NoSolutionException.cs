#pragma warning disable CA2229
#pragma warning disable CA1032

using System.Runtime.Serialization;
using System;
using System.Diagnostics;
using Sudoku.Data;

namespace Sudoku.Runtime
{
	/// <summary>
	/// Represents an error that the puzzle has no solution while solving.
	/// </summary>
	[Serializable, DebuggerStepThrough]
	public class NoSolutionException : SudokuRuntimeException
	{
		/// <summary>
		/// Initializes an instance with a grid.
		/// </summary>
		/// <param name="grid">The grid.</param>
		public NoSolutionException(Grid grid) : base() => Grid = grid;

		/// <summary>
		/// Initializes an instance with a grid and an error message.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="message">The error message.</param>
		public NoSolutionException(Grid grid, string message) : base(message) => Grid = grid;

		/// <summary>
		/// Initializes an instance with a grid, an error message and an inner exception.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="message">The error message.</param>
		/// <param name="inner">The inner exception.</param>
		public NoSolutionException(Grid grid, string message, Exception inner)
			: base(message, inner) => Grid = grid;

		/// <summary>
		/// Initializes an instance with a grid, a serialization information instance and
		/// a streaming context instance.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="info">The streaming information.</param>
		/// <param name="context">The streaming context.</param>
		protected NoSolutionException(Grid grid, SerializationInfo info, StreamingContext context)
			: base(info, context) => Grid = grid;


		/// <summary>
		/// Indicates the error message.
		/// </summary>
		public override string Message => $"The grid '{Grid:#}' has no solution.";

		/// <summary>
		/// The grid.
		/// </summary>
		public Grid Grid { get; }
	}
}
