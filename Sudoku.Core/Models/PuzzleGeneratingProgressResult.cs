using Sudoku.Globalization;
using Sudoku.Resources;

namespace Sudoku.Models
{
	/// <summary>
	/// Indicates a puzzle generating progress result.
	/// </summary>
	public struct PuzzleGeneratingProgressResult : IProgressResult
	{
		/// <summary>
		/// Initializes an instance with the specified country code.
		/// </summary>
		/// <param name="countryCode">The country code.</param>
		public PuzzleGeneratingProgressResult(CountryCode countryCode) : this() => CountryCode = countryCode;

		/// <summary>
		/// Initializes an instance with the specified trial times and country code.
		/// </summary>
		/// <param name="generatingTrial">The number of the trial times.</param>
		/// <param name="countryCode">The country code.</param>
		public PuzzleGeneratingProgressResult(int generatingTrial, CountryCode countryCode) : this()
		{
			GeneratingTrial = generatingTrial;
			CountryCode = countryCode;
		}


		/// <summary>
		/// Indicates how many trials of the specified generator generates.
		/// </summary>
		public int GeneratingTrial { readonly get; set; }

		/// <inheritdoc/>
		public readonly double Percentage => 0;

		/// <inheritdoc/>
		public readonly CountryCode CountryCode { get; }


		/// <inheritdoc cref="object.ToString"/>
		public override string ToString() =>
			GeneratingTrial == 1
			? TextResources.Current.GeneratingProgressSingular
			: $"{GeneratingTrial.ToString()} {TextResources.Current.GeneratingProgressPlural}";
	}
}
