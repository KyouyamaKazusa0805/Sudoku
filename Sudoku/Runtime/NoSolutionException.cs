using System;
using System.Runtime.Serialization;

namespace Sudoku.Runtime
{
	[Serializable]
	public class NoSolutionException : Exception
	{
		public NoSolutionException() : base()
		{
		}

		public NoSolutionException(string message) : base(message)
		{
		}

		public NoSolutionException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected NoSolutionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}


		public override string Message => "The specified grid has no solution.";
	}
}
