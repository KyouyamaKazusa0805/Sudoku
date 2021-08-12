namespace Sudoku.Models
{
	/// <summary>
	/// Encapsulates and provides with a progress result instance.
	/// </summary>
	public interface IProgressResult
	{
		/// <summary>
		/// Indicates the current percentage.
		/// </summary>
		double Percentage { get; }

		/// <summary>
		/// The country code.
		/// </summary>
		CountryCode CountryCode { get; }


		/// <inheritdoc cref="object.ToString"/>
		string ToString();
	}
}
