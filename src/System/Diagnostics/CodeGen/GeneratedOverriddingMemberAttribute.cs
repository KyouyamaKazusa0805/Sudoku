namespace System.Diagnostics.CodeGen;

/// <summary>
/// Represents a marker attribute to tell source generators that the member derived from type <see cref="object"/>
/// or <see cref="ValueType"/> should be automatically implemented.
/// </summary>
/// <remarks>
/// This attribute supports the following members:
/// <list type="bullet">
/// <item><see cref="object.Equals(object?)"/> and <see cref="ValueType.Equals(object?)"/></item>
/// <!--
/// <item><see cref="object.GetHashCode"/> and <see cref="ValueType.GetHashCode"/></item>
/// <item><see cref="object.ToString"/> and <see cref="ValueType.ToString"/></item>
/// -->
/// </list>
/// </remarks>
/// <seealso cref="object"/>
/// <seealso cref="ValueType"/>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class GeneratedOverriddingMemberAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="GeneratedOverriddingMemberAttribute"/> instance via the specified behavior on generating
	/// <see cref="object.Equals(object?)"/>.
	/// </summary>
	/// <param name="overriddingEqualsBehavior">The behavior.</param>
	public GeneratedOverriddingMemberAttribute(GeneratedEqualsBehavior overriddingEqualsBehavior)
		=> OverriddingEqualsBehavior = overriddingEqualsBehavior;


	/// <summary>
	/// Indicates the behavior describing source generator's generated source code on overridding <see cref="object.Equals(object?)"/>.
	/// </summary>
	public GeneratedEqualsBehavior? OverriddingEqualsBehavior { get; }
}
