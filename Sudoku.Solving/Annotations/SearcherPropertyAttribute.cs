using System;
using System.Runtime.CompilerServices;
using Quadruple = System.ValueTuple<bool, bool, int, Sudoku.Solving.Annotations.DisabledReason>;

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
		/// Indicates whether the property is read-only, which cannot be modified.
		/// </summary>
		public bool IsReadOnly { get; set; } = false;

		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public int Priority { get; set; }

		/// <summary>
		/// Indicates whether the current searcher has bug to fix. The default value is <see langword="false"/>.
		/// </summary>
		public DisabledReason DisabledReason { get; set; } = DisabledReason.LastResort;


		/// <include file='..\GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="isEnabled">
		/// (<see langword="out"/> parameter) Indicates whether the current searcher is enabled.
		/// </param>
		/// <param name="isReadOnly">
		/// (<see langword="out"/> parameter) Indicates whether the current searcher is read-only.
		/// </param>
		/// <param name="disabledReason">
		/// (<see langword="out"/> parameter) Indicates why the searcher is disabled.
		/// </param>
		/// <param name="priority">(<see langword="out"/> parameter) The priority of the searcher.</param>
		public void Deconstruct(
			out bool isEnabled, out bool isReadOnly, out int priority, out DisabledReason disabledReason) =>
			(isEnabled, isReadOnly, priority, disabledReason) = (IsEnabled, IsReadOnly, Priority, DisabledReason);


		/// <inheritdoc/>
		public override string ToString() => ((Quadruple)this).ToString();


		/// <summary>
		/// Converts the current object to the quadruple <see cref="Quadruple"/>.
		/// </summary>
		/// <param name="this">The current instance.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator Quadruple(SearcherPropertyAttribute @this)
		{
			var (a, b, c, d) = @this;
			return (a, b, c, d);
		}
	}
}
