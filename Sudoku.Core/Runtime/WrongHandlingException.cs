using System;
using System.Runtime.Serialization;
using Sudoku.Data;

namespace Sudoku.Runtime
{
	/// <summary>
	/// Represents an error that the puzzle has wrong handling while solving with
	/// manual logic tools.
	/// </summary>
	[Serializable]
	public class WrongHandlingException : SudokuRuntimeException
	{
		/// <summary>
		/// Initializes an instance with a grid.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		public WrongHandlingException(in SudokuGrid grid) => Grid = grid;

		/// <summary>
		/// Initializes an instance with a grid and an error message.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="wrongInfo">The error message.</param>
		public WrongHandlingException(in SudokuGrid grid, string wrongInfo) : base(wrongInfo)
		{
			Grid = grid;
			WrongInfo = wrongInfo;
		}

		/// <summary>
		/// Initializes an instance with a grid, an error message and an inner exception.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="wrongInfo">The error message.</param>
		/// <param name="inner">The inner exception.</param>
		public WrongHandlingException(in SudokuGrid grid, string wrongInfo, Exception inner)
			: base(wrongInfo, inner)
		{
			Grid = grid;
			WrongInfo = wrongInfo;
		}

		/// <summary>
		/// Initializes an instance with a grid, a serialization information instance and
		/// a streaming context instance.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="info">The streaming information.</param>
		/// <param name="context">The streaming context.</param>
		protected WrongHandlingException(in SudokuGrid grid, SerializationInfo info, StreamingContext context)
			: base(info, context) => Grid = grid;


		/// <summary>
		/// Indicates the wrong information.
		/// </summary>
		public string? WrongInfo { get; }

		/// <summary>
		/// The grid.
		/// </summary>
		public SudokuGrid Grid { get; }

		/// <inheritdoc/>
		public override string Message =>
			$"The specified message can't be solved due to: {WrongInfo ?? "<Unknown case>"}.";
	}
}
