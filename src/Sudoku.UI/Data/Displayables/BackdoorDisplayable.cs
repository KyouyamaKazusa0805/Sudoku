namespace Sudoku.UI.Data.Displayables;

/// <summary>
/// Provides with a displayable unit that display backdoors.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
public sealed record BackdoorDisplayable(ImmutableArray<Conclusion> Conclusions) :
	IDisplayable,
	IEquatable<BackdoorDisplayable>,
	IEqualityOperators<BackdoorDisplayable, BackdoorDisplayable>
{
	/// <summary>
	/// Initializes a <see cref="BackdoorDisplayable"/> instance via the specified conclusions as possible backdoors.
	/// </summary>
	/// <param name="backdoors">The backdoors.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BackdoorDisplayable(params Conclusion[] backdoors) : this(ImmutableArray.Create(backdoors))
	{
	}


	/// <inheritdoc/>
	/// <remarks>
	/// This property seems to be useless in this type.
	/// </remarks>
	ImmutableArray<View> IDisplayable.Views => ImmutableArray<View>.Empty;


	/// <inheritdoc/>
	public bool Equals([NotNullWhen(true)] BackdoorDisplayable? other)
		=> other is not null && Conclusions == other.Conclusions;

	/// <inheritdoc/>
	public override int GetHashCode() => Conclusions.GetHashCode();
}
