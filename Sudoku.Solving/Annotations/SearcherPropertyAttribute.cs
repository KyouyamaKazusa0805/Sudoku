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
		/// Indicates the priority of this technique.
		/// </summary>
		public int Priority { get; }

		/// <summary>
		/// Indicates whether the current searcher has bug to fix. The default value is <see langword="false"/>.
		/// </summary>
		public DisabledReason DisabledReason { get; set; } = DisabledReason.LastResort;


		/// <include file='../../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="isEnabled">
		/// (<see langword="out"/> parameter) Indicates whether the current searcher is enabled.
		/// </param>
		/// <param name="disabledReason">
		/// (<see langword="out"/> parameter) Indicates why the searcher is disabled.
		/// </param>
		/// <param name="priority">(<see langword="out"/> parameter) The priority of the searcher.</param>
		public void Deconstruct(out bool isEnabled, out int priority, out DisabledReason disabledReason) =>
			(isEnabled, priority, disabledReason) = (IsEnabled, Priority, DisabledReason);
	}

	/// <summary>
	/// Indicates a reason why the searcher is disabled.
	/// </summary>
	public enum DisabledReason : byte
	{
		/// <summary>
		/// Indicates the searcher searches for last resorts, which don't need to show.
		/// </summary>
		LastResort,

		/// <summary>
		/// Indicates the searcher has bugs while searching.
		/// </summary>
		HasBugs,
	}
}
