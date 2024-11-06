namespace Sudoku.Analytics;

public partial class Hub
{
	public partial class TechniqueNaming
	{
		/// <summary>
		/// Represents naming rules for subsets.
		/// </summary>
		public static class Subset
		{
			/// <summary>
			/// Try to get the real name for the specified size of subset.
			/// </summary>
			/// <param name="size">The number of cells used in a subset.</param>
			/// <returns>The name of the subset.</returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static string GetSubsetName(Digit size) => SR.Get($"SubsetNamesSize{size}", CultureInfo.CurrentUICulture);
		}
	}
}
