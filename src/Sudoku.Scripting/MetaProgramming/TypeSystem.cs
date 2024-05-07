namespace Sudoku.MetaProgramming;

/// <summary>
/// Represents methods relating to C# type system.
/// </summary>
public static class TypeSystem
{
	/// <summary>
	/// Try to get friendly type name of a type.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <param name="culture">Indicates the current culture.</param>
	/// <returns>The friendly name of the type.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string GetFriendlyTypeName(Type type, CultureInfo? culture = null)
		=> type == typeof(bool?)
			? ResourceDictionary.Get("Type_NullableBoolean", culture)
			: ResourceDictionary.TryGet($"Type_{type.Name}", out var resource, culture)
				? resource
				: ResourceDictionary.Get("Type_Other", culture);
}
