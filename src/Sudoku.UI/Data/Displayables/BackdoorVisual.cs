namespace Sudoku.UI.Data.Displayables;

/// <summary>
/// Provides with a visual unit that display backdoors.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
public sealed record BackdoorVisual(ImmutableArray<Conclusion> Conclusions) :
	IVisual,
	IEquatable<BackdoorVisual>,
	IEqualityOperators<BackdoorVisual, BackdoorVisual, bool>
{
	/// <summary>
	/// Initializes a <see cref="BackdoorVisual"/> instance via the specified conclusions as possible backdoors.
	/// </summary>
	/// <param name="backdoors">The backdoors.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BackdoorVisual(params Conclusion[] backdoors) : this(ImmutableArray.Create(backdoors))
	{
	}


	/// <inheritdoc/>
	/// <remarks>
	/// This property seems to be useless in this type.
	/// </remarks>
	ImmutableArray<View> IVisual.Views => ImmutableArray<View>.Empty;


	/// <inheritdoc/>
	public bool Equals([NotNullWhen(true)] BackdoorVisual? other) => other is not null && Conclusions == other.Conclusions;

	/// <inheritdoc/>
	public override int GetHashCode() => Conclusions.GetHashCode();
}
