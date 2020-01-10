using System;
using System.Runtime.Serialization;

namespace Sudoku.Runtime
{
	[Serializable]
	public class MultipleSolutionsException : Exception
	{
		public MultipleSolutionsException() : base()
		{
		}

		public MultipleSolutionsException(string message) : base(message)
		{
		}

		public MultipleSolutionsException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected MultipleSolutionsException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}


		public override string Message => "The specified grid has multiple solutions.";
	}
}
