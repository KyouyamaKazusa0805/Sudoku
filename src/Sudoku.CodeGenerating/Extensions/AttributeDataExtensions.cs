namespace Microsoft.CodeAnalysis;

/// <summary>
/// Provides extension methods on <see cref="AttributeData"/>.
/// </summary>
/// <seealso cref="AttributeData"/>
internal static class AttributeDataExtensions
{
	/// <summary>
	/// Try to get the named argument using the specified name.
	/// </summary>
	/// <param name="this">The attribute data instance.</param>
	/// <param name="namedArg">The named argument name.</param>
	/// <param name="result">The result got.</param>
	/// <returns>A <see cref="bool"/> result indicating whether the operation is successful.</returns>
	public static bool TryGetNamedArgument(this AttributeData @this, string namedArg, out TypedConstant result)
	{
		if (@this is not { NamedArguments: { Length: not 0 } namedArgs })
		{
			goto ReturnDefault;
		}

#if NETSTANDARD2_1_OR_GREATER
		foreach (var (argName, argValue) in namedArgs)
		{
#else
		foreach (var kvp in namedArgs)
		{
			string argName = kvp.Key;
			var argValue = kvp.Value;
#endif

			if (argName == namedArg)
			{
				result = argValue;
				return true;
			}
		}

	ReturnDefault:
		result = default;
		return false;
	}
}
