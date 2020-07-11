using System;

namespace Sudoku.Solving.Annotations
{
	/// <summary>
	/// Marks this attribute to a technique searcher to indicate the property of the settings while searching.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class SearcherPropertyAttribute : Attribute
	{
		/// <summary>
		/// Initializes an instance with the specified priority.
		/// Larger the value, later the searcher used.
		/// </summary>
		/// <param name="priority">The priority value.</param>
		public SearcherPropertyAttribute(int priority) => Priority = priority;


		/// <summary>
		/// Indicates whether the technique is enabled. The default value is <see langword="true"/>.
		/// </summary>
		public bool IsEnabled { get; set; } = true;

		/// <summary>
		/// Indicates whether the current searcher has bug to fix. The default value is <see langword="false"/>.
		/// </summary>
		public bool HasBug { get; set; } = false;

		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public int Priority { get; }


		/// <include file='../../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="isEnabled">
		/// (<see langword="out"/> parameter) Indicates whether the current searcher is enabled.
		/// </param>
		/// <param name="hasBug">
		/// (<see langword="out"/> parameter) Indicates whether the current searcher has bug while searching.
		/// </param>
		/// <param name="priority">(<see langword="out"/> parameter) The priority of the searcher.</param>
		public void Deconstruct(out bool isEnabled, out bool hasBug, out int priority) =>
			(isEnabled, hasBug, priority) = (IsEnabled, HasBug, Priority);
	}
}
