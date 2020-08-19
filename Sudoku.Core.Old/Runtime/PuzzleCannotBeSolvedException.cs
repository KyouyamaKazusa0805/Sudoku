using System;
using System.Runtime.Serialization;

namespace Sudoku.Runtime
{
	[Serializable]
	public class PuzzleCannotBeSolvedException : Exception
	{
		public PuzzleCannotBeSolvedException()
		{ 
		}

		public PuzzleCannotBeSolvedException(string message) : base(message)
		{ 
		}

		public PuzzleCannotBeSolvedException(string message, Exception inner)
			: base(message, inner)
		{ 
		}

		protected PuzzleCannotBeSolvedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ 
		}
	}
}
