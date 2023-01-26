namespace SudokuStudio.Models;

/// <summary>
/// Defines a view unit.
/// </summary>
public sealed partial class ViewUnit
{
	/// <summary>
	/// Indicates the candidates as conclusions in a single <see cref="IStep"/>.
	/// </summary>
	/// <seealso cref="IStep"/>
	public ImmutableArray<Conclusion> Conclusions { get; set; }

	/// <summary>
	/// Indicates a view of highlight elements.
	/// </summary>
	public View View { get; set; } = null!;


	[GeneratedDeconstruction]
	public partial void Deconstruct(out View view, out ImmutableArray<Conclusion> conclusions);
}
