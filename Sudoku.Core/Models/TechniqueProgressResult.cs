using Sudoku.Extensions;
using Sudoku.Globalization;

namespace Sudoku.Models
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
		/// <param name="countryCode">The country code.</param>
		public TechniqueProgressResult(int totalSearchers, CountryCode countryCode) : this()
		{
			TotalSearchers = totalSearchers;
			CountryCode = countryCode;
		}


		/// <inheritdoc/>
		public double Percentage => (double)CurrentIndex / TotalSearchers * 100;

		/// <summary>
		/// Indicates the current technique.
		/// </summary>
		public string? CurrentTechnique { readonly get; set; }

		/// <summary>
		/// The current index.
		/// </summary>
		public int CurrentIndex { readonly get; set; }

		/// <inheritdoc/>
		public readonly CountryCode CountryCode { get; }

		/// <summary>
		/// The total number of searchers.
		/// </summary>
		public readonly int TotalSearchers { get; }


		/// <inheritdoc cref="object.ToString"/>
		public override readonly string ToString() => CurrentTechnique.NullableToString();
	}
}
