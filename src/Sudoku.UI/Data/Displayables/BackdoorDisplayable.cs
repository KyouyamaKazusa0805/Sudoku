namespace Sudoku.UI.Data.Displayables;

/// <summary>
/// Provides with a displayable unit that display backdoors.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
public sealed record class BackdoorDisplayable(ImmutableArray<Conclusion> Conclusions, ImmutableArray<View> Views) :
	IDisplayable,
	IEquatable<BackdoorDisplayable>,
	IEqualityOperators<BackdoorDisplayable, BackdoorDisplayable>;