namespace Microsoft.UI.Xaml.Media.Animation;

/// <summary>
/// Provides extension methods on <see cref="PointAnimation"/>.
/// </summary>
/// <seealso cref="PointAnimation"/>
public static class PointAnimationExtensions
{
	/// <summary>
	/// Sets the properties <see cref="PointAnimation.From"/> and <see cref="PointAnimation.To"/>
	/// with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PointAnimation WithPoints(this PointAnimation @this, Point from, Point to)
	{
		(@this.From, @this.To) = (from, to);
		return @this;
	}
}
