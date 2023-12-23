namespace Sudoku.Compatibility.SudokuExplainer;

/// <summary>
/// Defines an attribute that is applied to a field in technique, indicating difficulty rating value defined by Sudoku Explainer.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
public sealed partial class SudokuExplainerDifficultyRatingAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="SudokuExplainerDifficultyRatingAttribute"/> via the specified difficulty rating value.
	/// </summary>
	/// <param name="difficultyRating">
	/// The difficulty rating value. Assign <see cref="double.NaN"/> if you don't know the real value.
	/// </param>
	public SudokuExplainerDifficultyRatingAttribute(double difficultyRating) => DifficultyRating = (half)difficultyRating;

	/// <summary>
	/// Initializes a <see cref="SudokuExplainerDifficultyRatingAttribute"/> via the specified difficulty rating values
	/// as a range.
	/// </summary>
	/// <param name="minDifficultyRating">
	/// The minimum difficulty rating value of the range that the specified technique can be reached.
	/// Assign <see cref="double.NaN"/> if you don't know the real value.
	/// </param>
	/// <param name="maxDifficultyRating">
	/// The maximum difficulty rating value of the range that the specified technique can be reached.
	/// Assign <see cref="double.NaN"/> if you don't know the real value.
	/// </param>
	public SudokuExplainerDifficultyRatingAttribute(double minDifficultyRating, double maxDifficultyRating)
		=> (DifficultyRating, DifficultyRatingMaximumThreshold, IsRange) = ((half)minDifficultyRating, (half)maxDifficultyRating, true);


	/// <summary>
	/// Indicates whether the specified technique is defined by advanced version of Sudoku Explainer,
	/// which is not original program, or other implementations compatible with original Sudoku Explainer's
	/// rating system.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="false"/>.
	/// </remarks>
	public bool IsAdvancedDefined { get; init; }

	/// <summary>
	/// Indicates whether the value is not accurate one to measure the technique's difficulty.
	/// If the value is <see langword="true"/>, the difficulty rating is the minimum value
	/// (in other words, threshold) of the difficulty corresponding to specified technique.
	/// </summary>
	[MemberNotNullWhen(true, nameof(DifficultyRatingMaximumThreshold))]
	public bool IsRange { get; }

	/// <summary>
	/// Indicates the difficulty rating.
	/// </summary>
	public half DifficultyRating { get; }

	/// <summary>
	/// <para>Indicates the maximum possible difficulty rating value that a technique can be reached.</para>
	/// <para>
	/// The value is <see langword="null"/> by default, but if the property <see cref="IsRange"/>
	/// is set to <see langword="true"/>, this value will not be <see langword="null"/>.
	/// </para>
	/// </summary>
	public half? DifficultyRatingMaximumThreshold { get; }


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out half min, out half? max, out bool isAdvanced)
		=> (min, max, isAdvanced) = (DifficultyRating, DifficultyRatingMaximumThreshold, IsAdvancedDefined);
}
