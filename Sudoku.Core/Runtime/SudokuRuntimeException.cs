using System;
using System.Runtime.Serialization;

namespace Sudoku.Runtime
{
	/// <summary>
	/// Represents an error that the puzzle has no solution while solving.
	/// </summary>
	[Serializable]
	public class SudokuRuntimeException : Exception
	{
		/// <inheritdoc/>
		public SudokuRuntimeException()
		{
		}

		/// <inheritdoc/>
		public SudokuRuntimeException(string message) : base(message)
		{
		}

		/// <inheritdoc/>
		public SudokuRuntimeException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <inheritdoc/>
		protected SudokuRuntimeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		/// <inheritdoc/>
		public override string Message => base.Message;
	}
}
