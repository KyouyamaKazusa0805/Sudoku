using System;
using Sudoku.Solving.Manual;

namespace Sudoku.Solving.Annotations
{
	/// <summary>
	/// Marks on the technique searcher to describe the display level of this technique.
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
	/// </remarks>
	/// <seealso cref="StepFinder"/>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class DisplayLevelAttribute : Attribute
	{
		/// <summary>
		/// Initializes an instance with the specified level.
		/// </summary>
		/// <param name="level">The level.</param>
		public DisplayLevelAttribute(int level) => Level = level;


		/// <summary>
		/// Indicates the level to display.
		/// </summary>
		public int Level { get; }
	}
}
