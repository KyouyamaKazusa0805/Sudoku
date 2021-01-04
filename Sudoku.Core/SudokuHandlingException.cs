using System;
using Sudoku.Data;
using Sudoku.DocComments;

namespace Sudoku
{
	/// <summary>
	/// Proivdes an exception that should thrown when the operation is error to handle or something is wrong.
	/// </summary>
	public class SudokuHandlingException : Exception
	{
		/// <inheritdoc cref="DefaultConstructor"/>
		public SudokuHandlingException()
		{
		}


		/// <summary>
		/// Initializes an instance with the error code.
		/// </summary>
		/// <param name="errorCode">The error code.</param>
		public SudokuHandlingException(int errorCode) => ErrorCode = errorCode;


		/// <summary>
		/// Indicates the error code.
		/// </summary>
		public int ErrorCode { get; }

		/// <inheritdoc/>
		public override string Message =>
			ErrorCode switch
			{
				301 => "The recognizer has not initialized.",
				303 => "Tesseract is wrong: can't recognize any cell image.",
				501 => "Parent node can't be found.",
				_ => string.Empty
			};
	}


	/// <summary>
	/// Proivdes an exception that should thrown when the operation is error to handle or something is wrong.
	/// </summary>
	public class SudokuHandlingException<T> : SudokuHandlingException
	{
		/// <inheritdoc cref="DefaultConstructor"/>
		public SudokuHandlingException() : base()
		{
		}

		/// <summary>
		/// Initializes an instance with the error code and the argument.
		/// </summary>
		/// <param name="errorCode">The error code.</param>
		/// <param name="arg">The arg 1.</param>
		public SudokuHandlingException(int errorCode, T arg) : base(errorCode) => Arg1 = arg;


		/// <summary>
		/// Indicates the argument 1.
		/// </summary>
		public T? Arg1 { get; }

		/// <inheritdoc/>
		public override string Message =>
			ErrorCode switch
			{
				101 when Arg1 is SudokuGrid g => $"The specified grid {g:#} contains multiple solutions.",
				102 when Arg1 is SudokuGrid g => $"The specified grid {g:#} contains no valid solution.",
				202 when Arg1 is SudokuGrid g => $"The specified grid {g:#} is invalid.",
				_ => string.Empty
			};
	}


	/// <summary>
	/// Proivdes an exception that should thrown when the operation is error to handle or something is wrong.
	/// </summary>
	public sealed class SudokuHandlingException<T1, T2> : SudokuHandlingException<T1>
	{
		/// <inheritdoc cref="DefaultConstructor"/>
		public SudokuHandlingException() : base()
		{
		}

		/// <summary>
		/// Initializes an instance with the error code and the argument.
		/// </summary>
		/// <param name="errorCode">The error code.</param>
		/// <param name="arg1">The arg 1.</param>
		/// <param name="arg2">The arg 2.</param>
		public SudokuHandlingException(int errorCode, T1 arg1, T2 arg2) : base(errorCode, arg1) => Arg2 = arg2;


		/// <summary>
		/// Indicates the argument 2.
		/// </summary>
		public T2? Arg2 { get; }

		/// <inheritdoc/>
		public override string Message =>
			ErrorCode switch
			{
				201 when Arg1 is SudokuGrid g && Arg2 is string info =>
					$"The specified grid {g:#} can't go on to be solved due to the step: {info} is wrong.",
				302 when Arg1 is int cell && Arg2 is int digit =>
					$"Recognizer error: can't fill the cell {new Cells { cell }} with the digit {digit + 1}.",
				401 when Arg1 is string assembly && Arg2 is string path =>
					$"The assembly {assembly} can't be loaded. " +
					$"The large possibility of the problem raised is that the required files don't exist. " +
					$"Please check the existence of the resource dictionary file (path {path}).",
				_ => string.Empty
			};
	}
}
