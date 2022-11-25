namespace System.Reflection;

/// <summary>
/// Provides a way to simplify the copying using reflection on cloneable types having implemented <see cref="ICloneable{TSelf}"/>.
/// </summary>
/// <seealso cref="ICloneable{TSelf}"/>
public static class ReflectionCopying
{
	/// <summary>
	/// Defines a default clone method using reflection to copy all auto properties.
	/// </summary>
	/// <typeparam name="T">The type of the cloneable object. It must contain a parameterless constructor.</typeparam>
	/// <param name="this">The type of the instance to copy.</param>
	/// <returns>The copied result. The result has totally same value as <paramref name="this"/>.</returns>
	public static T ReflectionClone<T>(this T @this) where T : class, ICloneable<T>, new()
	{
		var instance = new T();
		foreach (var propertyInfo in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
		{
			if (propertyInfo is { CanRead: true, CanWrite: true })
			{
				var originalValue = propertyInfo.GetValue(@this);
				propertyInfo.SetValue(instance, originalValue);
			}
		}

		return instance;
	}

	/// <summary>
	/// Copies and covers the current instance via the newer instance to copy all auto properties.
	/// </summary>
	/// <param name="this">The current instance to copy.</param>
	/// <param name="new">The newer instance to copy.</param>
	public static void ReflectionCover<T>(this T @this, T @new) where T : class, ICloneable<T>, new()
	{
		foreach (var propertyInfo in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
		{
			if (propertyInfo is { CanRead: true, CanWrite: true })
			{
				var originalValue = propertyInfo.GetValue(@new);
				propertyInfo.SetValue(@this, originalValue);
			}
		}
	}
}
