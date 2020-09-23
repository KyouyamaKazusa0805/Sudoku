using System;
using System.Runtime.CompilerServices;
using Sudoku.DocComments;

namespace Sudoku.Solving.Annotations
{
	/// <summary>
	/// Marks this attribute to a technique searcher to indicate the property of the settings while searching.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	[Obsolete("Because of they are read-only in metadata, please use the static property 'SearcherProperties' instead.", true)]
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
		/// Indicates whether the current searcher has bug to fix, or something else to describe why
		/// the searcher is (or should be) disabled.
		/// The default value is <see cref="DisabledReason.LastResort"/>.
		/// </summary>
		/// <seealso cref="DisabledReason.LastResort"/>
		public DisabledReason DisabledReason { get; set; } = DisabledReason.LastResort;


		/// <inheritdoc cref="DeconstructMethod"/>
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
		public override string ToString() => (((bool, bool, int, DisabledReason))this).ToString();


		/// <summary>
		/// Converts the current object to the quadruple <see cref="ValueTuple{T1, T2, T3, T4}"/>.
		/// </summary>
		/// <param name="this">The current instance.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator (bool, bool, int, DisabledReason)(SearcherPropertyAttribute @this)
		{
			var (a, b, c, d) = @this;
			return (a, b, c, d);
		}
	}
}
