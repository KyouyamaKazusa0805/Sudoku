using System;
using System.Runtime.Serialization;

namespace Sudoku.Runtime
{
	public class NoSolutionException : Exception
	{
		public NoSolutionException()
		{
		}

		public NoSolutionException(string message) : base(message)
		{
		}

		public NoSolutionException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected NoSolutionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
