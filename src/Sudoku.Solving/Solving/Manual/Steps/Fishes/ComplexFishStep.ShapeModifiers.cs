namespace Sudoku.Solving.Manual.Steps;

partial record ComplexFishStep
{
	/// <summary>
	/// Indicates the shape modifiers.
	/// </summary>
	[Flags]
	private enum ShapeModifiers
	{
		/// <summary>
		/// Indicates the basic fish.
		/// </summary>
		Basic = 1,

		/// <summary>
		/// Indicates the franken fish.
		/// </summary>
		Franken = 2,

		/// <summary>
		/// Indicates the mutant fish.
		/// </summary>
		Mutant = 4
	}
}
