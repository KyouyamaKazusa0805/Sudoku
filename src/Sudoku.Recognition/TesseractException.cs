#if SUDOKU_RECOGNITION

using System;
using System.Runtime.Serialization;

namespace Sudoku.Recognition
{
	/// <summary>
	/// Indicates the exception that throws when the tesseract has encountered an error.
	/// </summary>
	[Serializable]
	public sealed class TesseractException : Exception
	{
		/// <summary>
		/// Initializes a <see cref="TesseractException"/> with the specified detail.
		/// </summary>
		/// <param name="detail">The detail.</param>
		public TesseractException(string detail)
		{
			Detail = detail;
			Data.Add(nameof(Detail), detail);
		}

		/// <inheritdoc/>
		private TesseractException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}


		/// <summary>
		/// Indicates the detail.
		/// </summary>
		public string? Detail { get; }

		/// <inheritdoc/>
		public override string Message => $"Tesseract has encountered an error: {Detail}.";

		/// <inheritdoc/>
		public override string HelpLink =>
			"https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3708510&doc_id=633030";


		/// <inheritdoc/>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue(nameof(Detail), Detail, typeof(string));

			base.GetObjectData(info, context);
		}
	}
}

#endif