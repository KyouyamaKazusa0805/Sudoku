namespace Sudoku.UI.Data.Displayables;

/// <summary>
/// Provides with a displayable unit that display true candidates.
/// </summary>
/// <param name="Views"><inheritdoc/></param>
public sealed record TrueCandidatesDisplayable(ImmutableArray<View> Views) :
	IDisplayable,
	IEquatable<TrueCandidatesDisplayable>,
	IEqualityOperators<TrueCandidatesDisplayable, TrueCandidatesDisplayable>
{
	/// <summary>
	/// Initializes a <see cref="TrueCandidatesDisplayable"/> instance via the specified views.
	/// </summary>
	/// <param name="views">The views.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TrueCandidatesDisplayable(params View[] views) : this(ImmutableArray.Create(views))
	{
	}


	/// <inheritdoc/>
	/// <remarks>
	/// This property seems to be useless in this type.
	/// </remarks>
	ImmutableArray<Conclusion> IDisplayable.Conclusions => ImmutableArray<Conclusion>.Empty;


	/// <inheritdoc/>
	public bool Equals([NotNullWhen(true)] TrueCandidatesDisplayable? other)
		=> other is not null && Views == other.Views;

	/// <inheritdoc/>
	public override int GetHashCode() => Views.GetHashCode();
}
