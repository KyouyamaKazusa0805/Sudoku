namespace Microsoft.CodeAnalysis;

/// <summary>
/// Provides with extension methods on <see cref="AttributeData"/>.
/// </summary>
/// <seealso cref="AttributeData"/>
internal static class AttributeDataExtensions
{
	/// <summary>
	/// Try to get the named arguments in the specified attribute data.
	/// </summary>
	/// <typeparam name="T">The type of the named argument.</typeparam>
	/// <param name="this">The attribute data instance.</param>
	/// <param name="namedArgumentName">The named argument name.</param>
	/// <param name="defaultValue">Indicates the default value of the current named argument.</param>
	/// <returns>The found value.</returns>
	[return: NotNullIfNotNull(nameof(defaultValue))]
	public static T? GetNamedArgument<T>(this AttributeData @this, string namedArgumentName, T? defaultValue = default)
	{
		if (@this.NamedArguments is not (var namedArgs and not []))
		{
			return defaultValue;
		}

		foreach (var (name, typedConstant) in namedArgs)
		{
			if (name == namedArgumentName)
			{
				return (T?)typedConstant.Value;
			}
		}

		return defaultValue;
	}
}
