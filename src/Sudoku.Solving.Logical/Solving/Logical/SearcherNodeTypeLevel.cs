namespace Sudoku.Solving.Logical;

internal static class SearcherNodeTypeLevel
{
	public const SearcherNodeTypes SingleDigit = SearcherNodeTypes.SoleDigit;
	public const SearcherNodeTypes Normal = SearcherNodeTypes.SoleDigit | SearcherNodeTypes.SoleCell;
	public const SearcherNodeTypes LockedCandidates = Normal | SearcherNodeTypes.LockedCandidates;
	public const SearcherNodeTypes LockedSets = LockedCandidates | SearcherNodeTypes.LockedSet;
	public const SearcherNodeTypes HiddenSets = LockedSets | SearcherNodeTypes.HiddenSet;
	public const SearcherNodeTypes UniqueRectangles = HiddenSets | SearcherNodeTypes.UniqueRectangle;
	public const SearcherNodeTypes RegularWings = UniqueRectangles | SearcherNodeTypes.XyzWing;
	public const SearcherNodeTypes Kraken = RegularWings | SearcherNodeTypes.Kraken;
}
