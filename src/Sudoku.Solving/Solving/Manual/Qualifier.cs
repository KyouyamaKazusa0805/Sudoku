using System;
using Sudoku.CodeGenerating;

namespace Sudoku.Solving.Manual
{
	/// <summary>
	/// Indicates the qualifier of this step searcher.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	[AutoGeneratePrimaryConstructor]
	public sealed partial class QualifierAttribute : Attribute
	{
		/// <summary>
		/// Indicates the qualifier.
		/// </summary>
		public string Qualifier { get; }
	}
}
