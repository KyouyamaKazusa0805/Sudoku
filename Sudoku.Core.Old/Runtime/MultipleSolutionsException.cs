using System;
using System.Runtime.Serialization;

namespace Sudoku.Runtime
{
	[Serializable]
	public class MultipleSolutionsException : Exception
	{
		public MultipleSolutionsException()
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
	}
}
