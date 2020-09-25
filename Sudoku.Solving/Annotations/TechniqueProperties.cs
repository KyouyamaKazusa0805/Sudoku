#pragma warning disable CA1815

using System;
using Sudoku.DocComments;
using static System.Reflection.BindingFlags;

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


		/// <summary>
		/// Get the specified properties using reflection.
		/// </summary>
		/// <typeparam name="TTechniqueSearcher">The type of the searcher.</typeparam>
		/// <returns>
		/// The properties instance. If the searcher is <see langword="abstract"/> type
		/// or not <see cref="TechniqueSearcher"/> at all,
		/// the return value will be <see langword="null"/>.
		/// </returns>
		public static TechniqueProperties? GetPropertiesFrom<TTechniqueSearcher>()
			where TTechniqueSearcher : TechniqueSearcher => GetPropertiesFrom(typeof(TTechniqueSearcher));

		/// <summary>
		/// Get the specified properties using reflection.
		/// </summary>
		/// <param name="searcher">The searcher.</param>
		/// <returns>
		/// The properties instance. If the searcher is <see langword="abstract"/> type,
		/// the return value will be <see langword="null"/>.
		/// </returns>
		public static TechniqueProperties? GetPropertiesFrom(TechniqueSearcher searcher) =>
			searcher.GetType() is not { IsAbstract: false } type
				? null
				: type.GetProperty("Properties", Public | Static)?.GetValue(null) as TechniqueProperties;

		/// <summary>
		/// Get the specified properties using reflection.
		/// </summary>
		/// <param name="type">The type of the specified searcher.</param>
		/// <returns>
		/// The properties instance. If the searcher is <see langword="abstract"/> type
		/// or not <see cref="TechniqueSearcher"/> at all,
		/// the return value will be <see langword="null"/>.
		/// </returns>
		public static TechniqueProperties? GetPropertiesFrom(Type type) =>
			!type.IsSubclassOf(typeof(TechniqueSearcher)) || type.IsAbstract
				? null
				: type.GetProperty("Properties", Public | Static)?.GetValue(null) as TechniqueProperties;
	}
}
