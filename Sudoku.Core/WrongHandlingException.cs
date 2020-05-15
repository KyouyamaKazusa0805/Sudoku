using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Sudoku.Data;

namespace Sudoku
{
	/// <summary>
	/// Represents an error that the puzzle has wrong handling while solving with
	/// manual logic tools.
	/// </summary>
	[Serializable, DebuggerStepThrough]
	[SuppressMessage("Usage", "CA2229:Implement serialization constructors", Justification = "<Pending>")]
	[SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "<Pending>")]
	public class WrongHandlingException : SudokuRuntimeException
	{
		/// <summary>
		/// Initializes an instance with a grid.
		/// </summary>
		/// <param name="grid">The grid.</param>
		public WrongHandlingException(IReadOnlyGrid grid) => Grid = grid;

		/// <summary>
		/// Initializes an instance with a grid and an error message.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="message">The error message.</param>
		public WrongHandlingException(IReadOnlyGrid grid, string message) : base(message) => Grid = grid;

		/// <summary>
		/// Initializes an instance with a grid, an error message and an inner exception.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="message">The error message.</param>
		/// <param name="inner">The inner exception.</param>
		public WrongHandlingException(IReadOnlyGrid grid, string message, Exception inner)
			: base(message, inner) => Grid = grid;

		/// <summary>
		/// Initializes an instance with a grid, a serialization information instance and
		/// a streaming context instance.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="info">The streaming information.</param>
		/// <param name="context">The streaming context.</param>
		protected WrongHandlingException(IReadOnlyGrid grid, SerializationInfo info, StreamingContext context)
			: base(info, context) => Grid = grid;


		/// <summary>
		/// The grid.
		/// </summary>
		public IReadOnlyGrid Grid { get; }


		/// <inheritdoc/>
		public override string Message =>
			"The specified message cannot be solved due to wrong handling in manual solver.";
	}
}
