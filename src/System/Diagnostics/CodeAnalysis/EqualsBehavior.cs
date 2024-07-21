namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Defines a behavior by source generator on generating <see cref="object.Equals(object?)"/> overridden methods.
/// </summary>
/// <seealso cref="object.Equals(object?)"/>
public enum EqualsBehavior
{
	/// <summary>
	/// Indicates the source generator will automatically adopt a case for an overridden unit.
	/// <list type="bullet">
	/// <item>For <see langword="ref struct"/>s, return <see langword="false"/> because there is implicitly box and unbox operation</item>
	/// <item>For <see langword="class"/>es, use <see langword="as"/> casting and call overloaded <c>Equals</c> method</item>
	/// <item>For <see langword="struct"/>s, use <see langword="is"/> casting and call overloaded <c>Equals</c> method</item>
	/// </list>
	/// </summary>
	Intelligent,

	/// <summary>
	/// Indicates the method always throws <see cref="NotSupportedException"/>.
	/// </summary>
	ThrowNotSupportedException,

	/// <summary>
	/// Indicates the method will be made <see langword="abstract"/>.
	/// </summary>
	MakeAbstract
}
