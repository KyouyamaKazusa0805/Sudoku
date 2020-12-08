using System;
using Sudoku.DocComments;
using Sudoku.Solving.Manual;
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
		/// <param name="displayLabel">The display label.</param>
		public TechniqueProperties(int priority, string displayLabel)
		{
			Priority = priority;
			DisplayLabel = displayLabel;
		}


		/// <summary>
		/// Indicates whether the technique is enabled. The default value is <see langword="true"/>.
		/// </summary>
		public bool IsEnabled { get; set; } = true;

		/// <summary>
		/// Indicates whether the property is read-only, which can't be modified.
		/// </summary>
		public bool IsReadOnly { get; set; }

		/// <summary>
		/// Indicates whether the searcher is only used in analyzing a sudoku grid.
		/// If <see langword="true"/>, when in find-all-step mode, this searcher will be disabled.
		/// </summary>
		public bool OnlyEnableInAnalysis { get; set; }

		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public int Priority { get; set; }

		/// <summary>
		/// Indicates the display level of this technique.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The display level means the which level the technique is at. All higher leveled techniques
		/// won't display on the screen when the searchers at the current level have found technique
		/// instances.
		/// </para>
		/// <para>
		/// This attribute is used on <see cref="StepFinder"/>. For example, if Alternating Inference Chain (AIC)
		/// is at level 1 and Forcing Chains (FC) is at level 2, when we find any AIC technique instances,
		/// FC won't be checked at the same time in order to enhance the performance.
		/// </para>
		/// <para>
		/// This attribute is also used for grouping those the searchers, especially in Sudoku Explainer mode.
		/// </para>
		/// <para>
		/// Note that the property is different with <see cref="DisplayLabel"/>.
		/// </para>
		/// </remarks>
		/// <seealso cref="StepFinder"/>
		/// <seealso cref="DisplayLabel"/>
		public int DisplayLevel { get; set; }

		/// <summary>
		/// Indicates the display label of this technique. The program will process and handle the
		/// value to the specified technique name.
		/// </summary>
		/// <remarks>
		/// Note that the property is different with <see cref="DisplayLevel"/>.
		/// </remarks>
		/// <seealso cref="DisplayLevel"/>
		public string DisplayLabel { get; set; }

		/// <summary>
		/// Indicates whether the current searcher has bug to fix, or something else to describe why
		/// the searcher is (or should be) disabled.
		/// The default value is <see cref="DisabledReason.LastResort"/>.
		/// </summary>
		/// <seealso cref="DisabledReason.LastResort"/>
		public DisabledReason DisabledReason { get; set; } = DisabledReason.LastResort;


		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="isEnabled">
		/// (<see langword="out"/> parameter) Indicates whether the technique is enabled.
		/// </param>
		/// <param name="isReadOnly">
		/// (<see langword="out"/> parameter) Indicates whether the technique can't modify the priority.
		/// </param>
		public void Deconstruct(out bool isEnabled, out bool isReadOnly)
		{
			isEnabled = IsEnabled;
			isReadOnly = IsReadOnly;
		}

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="isEnabled">
		/// (<see langword="out"/> parameter) Indicates whether the technique is enabled.
		/// </param>
		/// <param name="isReadOnly">
		/// (<see langword="out"/> parameter) Indicates whether the technique can't modify the priority.
		/// </param>
		/// <param name="priority">
		/// (<see langword="out"/> parameter) Indicates the priority of the technique.
		/// </param>
		public void Deconstruct(out bool isEnabled, out bool isReadOnly, out int priority)
		{
			isEnabled = IsEnabled;
			isReadOnly = IsReadOnly;
			priority = Priority;
		}

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="isEnabled">
		/// (<see langword="out"/> parameter) Indicates whether the technique is enabled.
		/// </param>
		/// <param name="isReadOnly">
		/// (<see langword="out"/> parameter) Indicates whether the technique can't modify the priority.
		/// </param>
		/// <param name="priority">
		/// (<see langword="out"/> parameter) Indicates the priority of the technique.
		/// </param>
		/// <param name="disabledReason">
		/// (<see langword="out"/> parameter) Indicates why this technique is disabled.
		/// </param>
		public void Deconstruct(
			out bool isEnabled, out bool isReadOnly, out int priority, out DisabledReason disabledReason)
		{
			isEnabled = IsEnabled;
			isReadOnly = IsReadOnly;
			priority = Priority;
			disabledReason = DisabledReason;
		}

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="isEnabled">
		/// (<see langword="out"/> parameter) Indicates whether the technique is enabled.
		/// </param>
		/// <param name="isReadOnly">
		/// (<see langword="out"/> parameter) Indicates whether the technique can't modify the priority.
		/// </param>
		/// <param name="priority">
		/// (<see langword="out"/> parameter) Indicates the priority of the technique.
		/// </param>
		/// <param name="disabledReason">
		/// (<see langword="out"/> parameter) Indicates why this technique is disabled.
		/// </param>
		/// <param name="onlyEnableInAnalysis">
		/// (<see langword="out"/> parameter) Indicates whether the searcher is enabled only in traversing mode.
		/// </param>
		/// <param name="displayLevel">
		/// (<see langword="out"/> parameter) Indicates the display level.
		/// </param>
		/// <param name="displayLabel">
		/// (<see langword="out"/> parameter) Indicates the display label.
		/// </param>
		public void Deconstruct(
			out bool isEnabled, out bool isReadOnly, out int priority, out DisabledReason disabledReason,
			out bool onlyEnableInAnalysis, out int displayLevel, out string displayLabel)
		{
			isEnabled = IsEnabled;
			isReadOnly = IsReadOnly;
			priority = Priority;
			disabledReason = DisabledReason;
			onlyEnableInAnalysis = OnlyEnableInAnalysis;
			displayLevel = DisplayLevel;
			displayLabel = DisplayLabel;
		}


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
