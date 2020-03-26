using System;

namespace Sudoku.Solving
{
	/// <summary>
	/// To mark on a technique searcher class (i.e. <see cref="TechniqueSearcher"/>,
	/// to provide additional displaying messages which are used in UI forms, such as
	/// technique priority settings form.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class TechniqueDisplayAttribute : Attribute
	{
		/// <summary>
		/// Initializes an instance with the specified displaying name.
		/// </summary>
		/// <param name="displayName">The name.</param>
		public TechniqueDisplayAttribute(string displayName) => DisplayName = displayName;


		/// <summary>
		/// Indicates the display name of this technique.
		/// </summary>
		public string DisplayName { get; }
	}
}
