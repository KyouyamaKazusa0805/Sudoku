namespace Sudoku.UI.Data.Displayables;

/// <summary>
/// Provides with a visual unit that display true candidates.
/// </summary>
/// <param name="Views"><inheritdoc/></param>
public sealed record TrueCandidatesVisual(ImmutableArray<View> Views) :
	IVisual,
	IEquatable<TrueCandidatesVisual>,
	IEqualityOperators<TrueCandidatesVisual, TrueCandidatesVisual, bool>
{
	/// <summary>
	/// Initializes a <see cref="TrueCandidatesVisual"/> instance via the specified views.
	/// </summary>
	/// <param name="views">The views.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TrueCandidatesVisual(params View[] views) : this(ImmutableArray.Create(views))
	{
	}


	/// <inheritdoc/>
	/// <remarks>
	/// This property seems to be useless in this type.
	/// </remarks>
	ImmutableArray<Conclusion> IVisual.Conclusions => ImmutableArray<Conclusion>.Empty;


	/// <inheritdoc/>
	public bool Equals([NotNullWhen(true)] TrueCandidatesVisual? other)
		=> other is not null && Views == other.Views;

	/// <inheritdoc/>
	public override int GetHashCode() => Views.GetHashCode();
}
