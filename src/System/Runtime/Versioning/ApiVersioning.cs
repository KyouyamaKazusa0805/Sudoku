namespace System.Runtime.Versioning;

/// <summary>
/// Try to fetch versioning information on a type or member.
/// </summary>
public static class ApiVersioning
{
	/// <summary>
	/// Try to get introduced version of this type or member.
	/// </summary>
	/// <typeparam name="TMemberInfo">The type of reflected type or member.</typeparam>
	/// <param name="typeOrMember">The type of member.</param>
	/// <returns>The target version introducing this API.</returns>
	public static Version? GetIntroducedVersion<TMemberInfo>(TMemberInfo typeOrMember) where TMemberInfo : MemberInfo
		=> typeOrMember.GetCustomAttribute<IntroducedSinceAttribute>()?.Version;

	/// <summary>
	/// Try to get deprecated version of this type or member.
	/// </summary>
	/// <typeparam name="TMemberInfo">The type of reflected type or member.</typeparam>
	/// <param name="typeOrMember">The type of member.</param>
	/// <returns>The target version making this API deprecated.</returns>
	public static Version? GetDeprecatedVersion<TMemberInfo>(TMemberInfo typeOrMember) where TMemberInfo : MemberInfo
		=> typeOrMember.GetCustomAttribute<DeprecatedSinceAttribute>()?.Version;
}
