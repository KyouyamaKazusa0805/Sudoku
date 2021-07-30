using System.Linq;
using System.Runtime.CompilerServices;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="AnalysisResult"/>.
	/// </summary>
	/// <seealso cref="AnalysisResult"/>
	public static class AnalysisResultEx
	{
		/// <summary>
		/// To check whether the specified analysis result contains the specified technique.
		/// </summary>
		/// <param name="this">The instance.</param>
		/// <param name="code">The technique code to check.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Contains(this AnalysisResult @this, Technique code) => @this switch
		{
			{ IsSolved: true } or { Steps: null } => false,
			_ => @this.Steps.Any(step => step.TechniqueCode == code)
		};
	}
}
