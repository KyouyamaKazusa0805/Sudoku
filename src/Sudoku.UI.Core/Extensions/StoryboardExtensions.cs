namespace Microsoft.UI.Xaml.Media.Animation;

/// <summary>
/// Provides with extension methods on <see cref="Storyboard"/>.
/// </summary>
/// <seealso cref="Storyboard"/>
public static class StoryboardExtensions
{
	/// <summary>
	/// Adds the specified element into the current collection.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Storyboard AddChildren<TTimeline>(this Storyboard @this, TTimeline timeline)
		where TTimeline : Timeline
	{
		@this.Children.Add(timeline);
		return @this;
	}
}
