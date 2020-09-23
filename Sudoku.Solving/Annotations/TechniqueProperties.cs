#pragma warning disable CA1815

using Sudoku.DocComments;

namespace Sudoku.Solving.Annotations
{
	/// <summary>
	/// Indicates the properties while searching aiming to <see cref="TechniqueSearcher"/>s.
	/// </summary>
	/// <seealso cref="TechniqueSearcher"/>
	public sealed class TechniqueProperties
	{
		/// <summary>
		/// Initializes an instance with the specified priority.
		/// </summary>
		/// <param name="priority">The priority.</param>
		public TechniqueProperties(int priority) => Priority = priority;


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
		/// <param name="isEnabled">(<see langword="out"/> parameter) Indicates whether the technique is enabled.</param>
		/// <param name="isReadOnly">
		/// (<see langword="out"/> parameter) Indicates whether the technique cannot modify the priority.
		/// </param>
		/// <param name="priority">(<see langword="out"/> parameter) Indicates the priority of the technique.</param>
		/// <param name="disabledReason">(<see langword="out"/> parameter) Indicates why this technique is disabled.</param>
		public void Deconstruct(
			out bool isEnabled, out bool isReadOnly, out int priority, out DisabledReason disabledReason) =>
			(isEnabled, isReadOnly, priority, disabledReason) = (IsEnabled, IsReadOnly, Priority, DisabledReason);
	}
}
