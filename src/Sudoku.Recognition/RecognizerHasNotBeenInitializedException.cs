#if SUDOKU_RECOGNITION

using System;
using System.Runtime.Serialization;

namespace Sudoku.Recognition
{
	/// <summary>
	/// Indicates the exception that throws when the recognition tools hasn't been initialized
	/// before using a function.
	/// </summary>
	[Serializable]
	public sealed class RecognizerHasNotBeenInitializedException : Exception
	{
		/// <summary>
		/// Initializes a <see cref="RecognizerHasNotBeenInitializedException"/>.
		/// </summary>
		public RecognizerHasNotBeenInitializedException()
		{
		}

		/// <inheritdoc/>
		private RecognizerHasNotBeenInitializedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}


		/// <inheritdoc/>
		public override string Message =>
			"The recognition tools should have been initialized before using the current function.";

		/// <inheritdoc/>
		public override string HelpLink =>
			"https://sunnieshine.github.io/Sudoku/types/exceptions/Exception-RecognizerHasNotBeenInitializedException";
	}
}

#endif