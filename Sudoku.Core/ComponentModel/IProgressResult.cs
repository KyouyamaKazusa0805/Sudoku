namespace Sudoku.ComponentModel
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
		/// The globalization string.
		/// </summary>
		string GlobalizationString { get; }


		/// <include file='../../GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="__noparam"]'/>
		string ToString();
	}
}
