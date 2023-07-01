namespace Sudoku.SourceGeneration;

/// <summary>
/// Provides with primary constructor operations.
/// </summary>
internal static class PrimaryConstructor
{
	public static string GetTargetMemberName(NamedArgs namedArgs, string parameterName, string defaultPattern)
		=> namedArgs.TryGetValueOrDefault<string>("GeneratedMemberName", out var customizedFieldName)
		&& customizedFieldName is not null
			? customizedFieldName
			: namedArgs.TryGetValueOrDefault<string>("NamingRule", out var namingRule) && namingRule is not null
				? namingRule.InternalHandle(parameterName)
				: defaultPattern.InternalHandle(parameterName);
}

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	/// <summary>
	/// Internal handle the naming rule, converting it into a valid identifier via specified parameter name.
	/// </summary>
	/// <param name="this">The naming rule.</param>
	/// <param name="parameterName">The parameter name.</param>
	/// <returns>The final identifier.</returns>
	public static string InternalHandle(this string @this, string parameterName)
		=> @this
			.Replace("<@", parameterName.ToCamelCasing())
			.Replace(">@", parameterName.ToPascalCasing())
			.Replace("@", parameterName);
}
