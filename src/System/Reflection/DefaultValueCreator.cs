namespace System.Reflection;

/// <summary>
/// Represents a default value generator.
/// </summary>
public static class DefaultValueCreator
{
	/// <summary>
	/// Creates an instance that is equivalent to <see langword="default"/>(<c>T</c>), where <c>T</c> is specified
	/// as a <see cref="Type"/> instance.
	/// </summary>
	/// <param name="type">The type instance.</param>
	/// <returns>The created result.</returns>
	public static object? CreateInstance(Type type)
	{
		return type switch
		{
			{ IsPointer: true } => null,
			_ => typeof(DefaultValueCreator)
				.GetMethod($"<{nameof(CreateInstance)}>g__{nameof(get)}|0_0", BindingFlags.NonPublic | BindingFlags.Static)!
				.MakeGenericMethod(type)
				.Invoke(null, null)
		};


		static T? get<T>() => default;
	}
}
