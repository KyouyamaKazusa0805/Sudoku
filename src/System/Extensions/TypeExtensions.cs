namespace System;

/// <summary>
/// Provides with extension methods on <see cref="Type"/>.
/// </summary>
/// <seealso cref="Type"/>
public static class TypeExtensions
{
	/// <summary>
	/// Determines whether the current type can be assigned to a variable of the specified
	/// <paramref name="targetType"/>, although it is with generic parameters.
	/// </summary>
	/// <param name="this">The current type.</param>
	/// <param name="targetType">The type to compare with the current type.</param>
	/// <returns>Returns <see langword="true"/> if the target type is matched, without generic constraints.</returns>
	public static bool IsGenericAssignableTo(this Type @this, [NotNullWhen(true)] Type? targetType)
	{
		// See: https://stackoverflow.com/questions/74616/how-to-detect-if-type-is-another-generic-type/1075059#1075059

		foreach (var it in @this.GetInterfaces())
		{
			if (it.IsGenericType && it.GetGenericTypeDefinition() == targetType)
			{
				return true;
			}
		}

		if (@this.IsGenericType && @this.GetGenericTypeDefinition() == targetType)
		{
			return true;
		}

		if (@this.BaseType is not { } baseType)
		{
			return false;
		}

		return baseType.IsGenericAssignableTo(targetType);
	}
}
