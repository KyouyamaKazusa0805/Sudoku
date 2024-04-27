/// <summary>
/// Provides with extension methods for <see cref="Accessibility"/>.
/// </summary>
/// <seealso cref="Accessibility"/>
internal static class AccessibilityExtensions
{
	/// <summary>
	/// Gets the name of the accessibility enumeration field.
	/// </summary>
	/// <param name="this">The field.</param>
	/// <returns>The name of the field.</returns>
	public static string GetName(this Accessibility @this)
		=> typeof(Accessibility).GetField(@this.ToString())!.GetCustomAttribute<NameAttribute>()!.Name;
}
