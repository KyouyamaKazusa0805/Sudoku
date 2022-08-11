namespace Microsoft.UI.Xaml.Media.Animation;

/// <summary>
/// Provides with extension methods on <see cref="Timeline"/>.
/// </summary>
/// <seealso cref="Timeline"/>
public static class TimelineExtensions
{
	/// <summary>
	/// Sets the property <see cref="Timeline.Duration"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TTimeline WithDuration<TTimeline>(this TTimeline @this, double durationMilliseconds)
		where TTimeline : Timeline => @this.WithDuration(TimeSpan.FromMilliseconds(durationMilliseconds));

	/// <summary>
	/// Sets the property <see cref="Timeline.Duration"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TTimeline WithDuration<TTimeline>(this TTimeline @this, Duration duration)
		where TTimeline : Timeline
	{
		@this.Duration = duration;
		return @this;
	}

	/// <summary>
	/// Calls the method <see cref="Storyboard.SetTarget(Timeline, DependencyObject)"/>.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TTimeline WithTarget<TTimeline, TDependencyObject>(this TTimeline @this, TDependencyObject target)
		where TTimeline : Timeline
		where TDependencyObject : DependencyObject
	{
		Storyboard.SetTarget(@this, target);
		return @this;
	}

	/// <summary>
	/// Calls the method <see cref="Storyboard.SetTargetProperty(Timeline, string)"/>.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TTimeline WithTargetProperty<TTimeline>(this TTimeline @this, string propertyPath)
		where TTimeline : Timeline
	{
		Storyboard.SetTargetProperty(@this, propertyPath);
		return @this;
	}

	/// <summary>
	/// Calls the method <see cref="Storyboard.SetTargetProperty(Timeline, string)"/>
	/// if the specified <paramref name="condition"/> is <see langword="true"/>.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TTimeline WithTargetPropertyIf<TTimeline>(this TTimeline @this, bool condition, string propertyPath)
		where TTimeline : Timeline
	{
		if (condition)
		{
			Storyboard.SetTargetProperty(@this, propertyPath);
		}

		return @this;
	}
}
