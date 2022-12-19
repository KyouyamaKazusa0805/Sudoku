namespace Sudoku.Concepts;

partial struct GridRefreshingCandidatesEventArgs
{
	/// <summary>
	/// The backing field of property <see cref="GridRef"/>.
	/// </summary>
	/// <seealso cref="GridRef"/>
	private readonly ref readonly Grid _gridRef;


	/// <summary>
	/// Indicates the reference to the grid whose candidates are refreshed.
	/// </summary>
	public ref readonly Grid GridRef => ref _gridRef;
}
