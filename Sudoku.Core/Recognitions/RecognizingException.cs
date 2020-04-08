using System;
using System.Runtime.Serialization;

namespace Sudoku.Recognitions
{
	/// <summary>
	/// Represents an error that the OCR tools cannot work normally.
	/// </summary>
	[Serializable]
	public class RecognizingException : SudokuRuntimeException
	{
		/// <inheritdoc/>
		public RecognizingException() : base() { }

		/// <inheritdoc/>
		public RecognizingException(string message) : base(message) { }

		/// <inheritdoc/>
		public RecognizingException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <inheritdoc/>
		protected RecognizingException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
