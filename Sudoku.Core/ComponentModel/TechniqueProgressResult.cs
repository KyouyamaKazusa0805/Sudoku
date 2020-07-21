namespace Sudoku.ComponentModel
{
	/// <summary>
	/// Indicates a technique progress result.
	/// </summary>
	public struct TechniqueProgressResult : IProgressResult
	{
		/// <summary>
		/// Initializes an instance with the specified technique count.
		/// </summary>
		/// <param name="totalSearchers">The total number of searchers.</param>
		/// <param name="globalizationString">The globalization string.</param>
		public TechniqueProgressResult(int totalSearchers, string globalizationString) : this() =>
			(TotalSearchers, GlobalizationString) = (totalSearchers, globalizationString);


		/// <summary>
		/// Indicates the current percentage finished.
		/// </summary>
		public double Percentage => (double)CurrentIndex / TotalSearchers * 100;

		/// <summary>
		/// Indicates the current technique.
		/// </summary>
		public string CurrentTechnique { readonly get; set; }

		/// <summary>
		/// The current index.
		/// </summary>
		public int CurrentIndex { readonly get; set; }

		/// <summary>
		/// The globalization string.
		/// </summary>
		public readonly string GlobalizationString { get; }

		/// <summary>
		/// The total number of searchers.
		/// </summary>
		public readonly int TotalSearchers { get; }


		/// <include file='..\GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="__noparam"]'/>
		public override readonly string ToString() => CurrentTechnique;
	}
}
