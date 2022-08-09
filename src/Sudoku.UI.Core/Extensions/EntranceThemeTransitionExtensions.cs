namespace Microsoft.UI.Xaml.Media.Animation;

/// <summary>
/// Provides extension methods on <see cref="EntranceThemeTransition"/>.
/// </summary>
/// <seealso cref="EntranceThemeTransition"/>
public static class EntranceThemeTransitionExtensions
{
	/// <summary>
	/// Sets the property <see cref="EntranceThemeTransition.IsStaggeringEnabled"/> with the specified value.
	/// </summary>
	public static EntranceThemeTransition WithIsStaggeringEnabled(this EntranceThemeTransition @this, bool value)
	{
		@this.IsStaggeringEnabled = value;
		return @this;
	}
}
