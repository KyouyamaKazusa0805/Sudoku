namespace Sudoku.Solving.Extensions
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
		/// <param name="this">(<see langword="this"/> parameter) The instance.</param>
		/// <param name="code">The technique code to check.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		public static bool Contains(this AnalysisResult @this, TechniqueCode code)
		{
			if (@this.IsSolved)
			{
				return false;
			}

			if (@this.Steps is null)
			{
				return false;
			}

			foreach (var step in @this.Steps)
			{
				if (step.TechniqueCode == code)
				{
					return true;
				}
			}
			return false;
		}
	}
}
