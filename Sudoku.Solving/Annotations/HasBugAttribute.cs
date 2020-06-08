using System;

namespace Sudoku.Solving.Annotations
{
	/// <summary>
	/// Indicates the <see cref="TechniqueSearcher"/> contains any bug.
	/// </summary>
	public sealed class HasBugAttribute : Attribute
	{
		/// <include file='../../GlobalDocComments.xml' path='comments/defaultConstructor'/>
		public HasBugAttribute()
		{
		}

		/// <summary>
		/// Intializes an instance with the specified information.
		/// </summary>
		/// <param name="bugInfo">The bug information.</param>
		public HasBugAttribute(string bugInfo) => BugInfo = bugInfo;


		/// <summary>
		/// Indicates the bug information.
		/// </summary>
		public string? BugInfo { get; }
	}
}
