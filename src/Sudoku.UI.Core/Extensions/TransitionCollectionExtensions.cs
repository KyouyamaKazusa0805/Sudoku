namespace Microsoft.UI.Xaml.Media.Animation;

/// <summary>
/// Provides with extension methods on <see cref="TransitionCollection"/>.
/// </summary>
/// <seealso cref="TransitionCollection"/>
public static class TransitionCollectionExtensions
{
	/// <summary>
	/// Adds an element into the collection.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TransitionCollection Append<TTransition>(this TransitionCollection @this)
		where TTransition : Transition, new() => @this.Append(new TTransition());

	/// <summary>
	/// Adds an element into the collection.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TransitionCollection Append(this TransitionCollection @this, Transition transition)
	{
		@this.Add(transition);
		return @this;
	}
}
