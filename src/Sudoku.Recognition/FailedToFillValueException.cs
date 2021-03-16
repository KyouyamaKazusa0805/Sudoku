#if SUDOKU_RECOGNITION

using System;
using System.Runtime.Serialization;
using Sudoku.Data;

namespace Sudoku.Recognition
{
	/// <summary>
	/// Indicates the exception that throws when the value is failed to fill into a cell.
	/// </summary>
	[Serializable]
	public sealed class FailedToFillValueException : Exception
	{
		/// <summary>
		/// Initializes a <see cref="FailedToFillValueException"/> instance
		/// with the specified cell and the digit.
		/// </summary>
		/// <param name="cell">The cell.</param>
		/// <param name="digit">The digit.</param>
		public FailedToFillValueException(int cell, int digit)
		{
			Cell = cell;
			Digit = digit;
			Data.Add(nameof(Cell), new WeakBox<int>(cell));
			Data.Add(nameof(Digit), new WeakBox<int>(digit));
		}


		/// <summary>
		/// Indicates the wrong cell.
		/// </summary>
		public int Cell { get; }

		/// <summary>
		/// Indicates the wrong digit.
		/// </summary>
		public int Digit { get; }

		/// <inheritdoc/>
		public override string Message =>
			$"The tool can't fill the cell {new Cells { Cell }.ToString()} with the digit {(Digit + 1).ToString()}.";

		/// <inheritdoc/>
		public override string HelpLink =>
			"https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3707050&doc_id=633030";


		/// <inheritdoc/>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue(nameof(Cell), Cell);
			info.AddValue(nameof(Digit), Digit);

			base.GetObjectData(info, context);
		}
	}
}

#endif