#pragma warning disable CA1032
#pragma warning disable CA2229

using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using Sudoku.Data;
using Sudoku.Extensions;

namespace Sudoku.Runtime
{
	/// <summary>
	/// Represents an error that the puzzle has wrong handling while solving with
	/// manual logic tools.
	/// </summary>
	[Serializable, DebuggerStepThrough]
	public class WrongHandlingException : SudokuRuntimeException
	{
		/// <summary>
		/// Initializes an instance with a grid.
		/// </summary>
		/// <param name="grid">The grid.</param>
		public WrongHandlingException(Grid grid) => Grid = grid;

		/// <summary>
		/// Initializes an instance with a grid and an error message.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="wrongInfo">The error message.</param>
		public WrongHandlingException(Grid grid, string wrongInfo) : base(wrongInfo) =>
			(Grid, WrongInfo) = (grid, wrongInfo);

		/// <summary>
		/// Initializes an instance with a grid, an error message and an inner exception.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="wrongInfo">The error message.</param>
		/// <param name="inner">The inner exception.</param>
		public WrongHandlingException(Grid grid, string wrongInfo, Exception inner) : base(wrongInfo, inner) =>
			(Grid, WrongInfo) = (grid, wrongInfo);

		/// <summary>
		/// Initializes an instance with a grid, a serialization information instance and
		/// a streaming context instance.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="info">The streaming information.</param>
		/// <param name="context">The streaming context.</param>
		protected WrongHandlingException(Grid grid, SerializationInfo info, StreamingContext context)
			: base(info, context) => Grid = grid;


		/// <summary>
		/// Indicates the wrong information.
		/// </summary>
		public string? WrongInfo { get; }

		/// <summary>
		/// The grid.
		/// </summary>
		public Grid Grid { get; }

		/// <inheritdoc/>
		public override string Message =>
			$"The specified message can't be solved due to: {WrongInfo.NullableToString("<Unknown case>")}.";
	}
}
