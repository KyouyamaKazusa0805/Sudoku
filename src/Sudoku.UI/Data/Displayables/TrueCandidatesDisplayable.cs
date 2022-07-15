namespace Sudoku.UI.Data.Displayables;

/// <summary>
/// Provides with a displayable unit that display true candidates.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
public sealed record class TrueCandidatesDisplayable(ImmutableArray<Conclusion> Conclusions, ImmutableArray<View> Views) :
	IDisplayable,
	IEquatable<TrueCandidatesDisplayable>,
	IEqualityOperators<TrueCandidatesDisplayable, TrueCandidatesDisplayable>;