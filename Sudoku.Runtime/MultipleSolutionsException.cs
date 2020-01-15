using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Sudoku.Data.Meta;

namespace Sudoku.Runtime
{
	[Serializable]
	[SuppressMessage("Usage", "CA2229:Implement serialization constructors", Justification = "<Pending>")]
	[SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "<Pending>")]
	public class MultipleSolutionsException : Exception
	{
		private readonly Grid _grid;


		public MultipleSolutionsException(Grid grid) : base() => _grid = grid;

		public MultipleSolutionsException(Grid grid, string message)
			: base(message) => _grid = grid;

		public MultipleSolutionsException(Grid grid, string message, Exception inner)
			: base(message, inner) => _grid = grid;

		protected MultipleSolutionsException(Grid grid, SerializationInfo info, StreamingContext context)
			: base(info, context) => _grid = grid;


		public override string Message => $"The grid '{_grid:#}' has multiple solutions.";
	}
}
