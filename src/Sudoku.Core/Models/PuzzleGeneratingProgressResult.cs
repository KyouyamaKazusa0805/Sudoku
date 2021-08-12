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
		public int GeneratingTrial { get; set; }

		/// <inheritdoc/>
		public readonly double Percentage => 0;

		/// <inheritdoc/>
		public CountryCode CountryCode { get; }


		/// <inheritdoc cref="object.ToString"/>
		/// <remarks><i>
		/// Before C# 10, <see langword="dynamic"/> type can be used as interpolation part
		/// in a interpolation string; after C# 10 interpolation string doesn't handle
		/// <see langword="dynamic"/> types, so we must append a type cast to <see cref="string"/>.
		/// </i></remarks>
		public override string ToString() =>
			GeneratingTrial == 1
			? TextResources.Current.GeneratingProgressSingular
			: $"{GeneratingTrial} {(string)TextResources.Current.GeneratingProgressPlural}";
	}
}
