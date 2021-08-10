using System;
using System.Diagnostics.CodeAnalysis;
using System.Extensions;
using System.Reflection;
using Sudoku.CodeGenerating;

namespace Sudoku.Solving.Manual
{
	/// <summary>
	/// Indicates the properties while searching aiming to <see cref="StepSearcher"/>s.
	/// </summary>
	/// <param name="Priority">Indicates the priority of this technique.</param>
	/// <param name="DisplayLabel">
	/// <para>
	/// Indicates the display label of this technique. The program will process and handle the
	/// value to the specified technique name.
	/// </para>
	/// <para>Note that the property is different with <see cref="DisplayLevel"/>.</para>
	/// </param>
	/// <seealso cref="StepSearcher"/>
	[AutoDeconstruct(nameof(IsEnabled), nameof(IsReadOnly))]
	[AutoDeconstruct(nameof(IsEnabled), nameof(IsReadOnly), nameof(Priority))]
	[AutoDeconstruct(nameof(IsEnabled), nameof(IsReadOnly), nameof(Priority), nameof(DisabledReason))]
	[AutoDeconstruct(nameof(IsEnabled), nameof(IsReadOnly), nameof(Priority), nameof(DisabledReason), nameof(OnlyEnableInAnalysis), nameof(DisplayLevel), nameof(DisplayLabel))]
	[Obsolete("Please use type '" + nameof(SearchingOptions) + "' instead.", false)]
	public sealed partial record TechniqueProperties(int Priority, string DisplayLabel)
	{
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
		/// Indicates the display level of this technique.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The display level means the which level the technique is at. All higher leveled techniques
		/// won't display on the screen when the searchers at the current level have found technique
		/// instances.
		/// </para>
		/// <para>
		/// This attribute is used on <see cref="AllStepSearcher"/>. For example, if Alternating Inference Chain (AIC)
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
		/// <seealso cref="AllStepSearcher"/>
		/// <seealso cref="DisplayLabel"/>
		public int DisplayLevel { get; set; }

		/// <summary>
		/// Indicates whether the current searcher has bug to fix, or something else to describe why
		/// the searcher is (or should be) disabled.
		/// The default value is <see cref="DisabledReason.None"/>.
		/// </summary>
		/// <seealso cref="DisabledReason.None"/>
		public DisabledReason DisabledReason { get; set; }


		/// <summary>
		/// Get the specified properties using reflection.
		/// </summary>
		/// <param name="searcher">The searcher.</param>
		/// <returns>
		/// The properties instance. If the searcher is <see langword="abstract"/> type,
		/// the return value will be <see langword="null"/>.
		/// </returns>
		public static TechniqueProperties? FromSearcher(StepSearcher searcher)
		{
			if (searcher.GetType() is not { IsAbstract: false } type)
			{
				return null;
			}

			var propInfo = type.GetProperty("Properties", BindingFlags.Public | BindingFlags.Static);
			return propInfo?.GetValue(null) as TechniqueProperties;
		}

		/// <summary>
		/// Get the specified properties using reflection.
		/// </summary>
		/// <param name="type">The type of the specified searcher.</param>
		/// <returns>
		/// The properties instance. If the searcher is <see langword="abstract"/> type
		/// or not <see cref="StepSearcher"/> at all,
		/// the return value will be <see langword="null"/>.
		/// </returns>
		public static TechniqueProperties? FromType(
			[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] Type type)
		{
			if (!type.IsSubclassOf<StepSearcher>() || type.IsAbstract)
			{
				return null;
			}

			var propInfo = type.GetProperty("Properties", BindingFlags.Public | BindingFlags.Static);
			return propInfo?.GetValue(null) as TechniqueProperties;
		}
	}
}
