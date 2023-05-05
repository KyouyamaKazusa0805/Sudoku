#pragma warning disable CS9113
namespace System.SourceGeneration;

/// <summary>
/// Indicates the attribute type that can be marked for a parameter, indicating the parameter is a <see langword="record"/>-like one,
/// telling the source generator that it should generate a property that references this parameter.
/// </summary>
/// <param name="memberKind">
/// Indicates the member kind to be generated. You can reference the target value via type <see cref="MemberKinds"/>.
/// </param>
/// <remarks>
/// <para>
/// Begin with C# 12, we can define primary constructors for non-<see langword="record"/> types.
/// However, the parameter defined will be expanded as a real parameter in its containing constructor.
/// The compiler won't create a binding field or property. We should define it manually. This attribute will solve this problem.
/// </para>
/// <para>
/// The original way to declare a type with primary constructor is like:
/// <code><![CDATA[
/// public readonly struct Color(byte a, byte r, byte g, byte b)
/// {
///     public byte A { get; } = a;
///     public byte R { get; } = r;
///     public byte G { get; } = g;
///     public byte B { get; } = b;
/// }
/// ]]></code>
/// </para>
/// <para>
/// Via this attribute type, we can simplify the code:
/// <code><![CDATA[
/// public readonly partial struct Color(
///     [PrimaryConstructorParameter] byte a,
///     [PrimaryConstructorParameter] byte r,
///     [PrimaryConstructorParameter] byte g,
///     [PrimaryConstructorParameter] byte b
/// );
/// ]]></code>
/// Such code is equivalent to the original one.
/// </para>
/// <para>If you want to learn more information about this attribute type, please visit the metadata of the type.</para>
/// </remarks>
[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
public sealed class PrimaryConstructorParameterAttribute(string memberKind = MemberKinds.Property) : Attribute
{
	/// <summary>
	/// Indicates the naming rule of the generated member name.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The syntax of this property can be described as two parts:
	/// <list type="number">
	/// <item>
	/// A verbatim symbol <c>@</c> ("At" symbol, may contain an extra character <c><![CDATA[<]]></c> or <c><![CDATA[>]]></c>:
	/// <c><![CDATA[<@]]></c> or <c><![CDATA[>@]]></c>)
	/// </item>
	/// <item>Characters used in a valid identifier</item>
	/// </list>
	/// </para>
	/// <para>
	/// For example, if you want to set a generated property is named like <c>xxxTargetProperty</c>
	/// where the <c>xxx</c> is the name of the generated property, you can set this property as value <c>"@TargetProperty"</c>.
	/// Then the generated property will be like:
	/// <code><![CDATA[
	/// public int ParameterTargetProperty { get; } = parameter;
	/// ]]></code>
	/// </para>
	/// <para>
	/// Also, you can set the first character as lower or upper case via symbols <c><![CDATA[<]]></c> and <c><![CDATA[>]]></c>.
	/// If this property is <c><![CDATA["_<@"]]></c>, the generated member will be like:
	/// <code><![CDATA[
	/// private int _parameter = parameter;
	/// ]]></code>
	/// where <c><![CDATA["<@"]]></c> means the generated member name will use lower case for its first character.
	/// </para>
	/// <para>
	/// By default, the naming rule is <c><![CDATA["_<@"]]></c> for fields, and <c><![CDATA[">@"]]></c> for properties.
	/// </para>
	/// </remarks>
	public string? NamingRule { get; init; }

	/// <summary>
	/// Indicates the member names becoming not <see langword="null"/>
	/// when this generated member is <see langword="true"/> or <see langword="false"/>.
	/// </summary>
	/// <remarks>
	/// The syntax of this property can be described as two parts:
	/// <list type="number">
	/// <item>Boolean value literal</item>
	/// <item>Member or parameter names</item>
	/// </list>
	/// The complete expression is <c>"(1): (2)"</c>. For example: <c>"<see langword="false"/>: property1, property2"</c>.
	/// The generated member will be:
	/// <code><![CDATA[
	/// [MemberNotNullWhen(false, nameof(Property1), nameof(Property2))]
	/// public bool Parameter { get; } = parameter;
	/// ]]></code>
	/// where <c>property1</c> and <c>property2</c> are supposed to be two parameters marked this attribute.
	/// </remarks>
	public string? MembersNotNull { get; init; }

	/// <summary>
	/// Indicates the name of the generated member.
	/// </summary>
	public string? GeneratedMemberName { get; init; }

	/// <summary>
	/// Indicates the accessibility of the generated member.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This property controls the case when you want to customize the accessibility of generated member.
	/// For example, the value is <c>"private"</c> or <c>"Private"</c>, the generated member will automatically insert the accessibility modifier
	/// into property declaration:
	/// <code><![CDATA[
	/// private int Parameter { get; } = parameter;
	/// ]]></code>
	/// </para>
	/// <para>By default, the accessibility is <see langword="private"/> for fields and <see langword="public"/> for properties.</para>
	/// </remarks>
	public string? Accessibility { get; init; }

	/// <summary>
	/// Indicates the <see langword="ref"/> kind of the generated member.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This property controls the case when you want to customize the reference kind of return value of the field or property.
	/// For example, the value is <c>"ref readonly"</c>, the generated member will automatically insert the <see langword="ref"/> modifiers
	/// into declaration:
	/// <code><![CDATA[
	/// private ref readonly int _parameter = ref parameter;
	/// ]]></code>
	/// </para>
	/// <para>
	/// By default, the <see langword="ref"/> kind should be suitable with parameter's declaration.
	/// If the parameter is defined without any <see langword="ref"/> modifiers:
	/// <list type="bullet">
	/// <item><see langword="ref"/></item>
	/// <item><see langword="ref readonly"/> (May not be supported for C# 12)</item>
	/// <item><see langword="in"/></item>
	/// <item><see langword="scoped ref"/></item>
	/// <item><see langword="scoped ref readonly"/> (May not be supported for C# 12)</item>
	/// <item><see langword="scoped in"/></item>
	/// <item><see langword="scoped in scoped"/> (May not be supported for C# 12)</item>
	/// <item><see langword="scoped ref scoped"/> (May not be supported for C# 12)</item>
	/// <item><see langword="scoped ref readonly scoped"/> (May not be supported for C# 12)</item>
	/// </list>
	/// the generated member will automatically include a <see langword="ref"/> modifier if the modifiers don't include <see langword="scoped"/>;
	/// otherwise, no <see langword="ref"/> modifiers.
	/// If you want to set <see langword="ref readonly"/>, you should manually set this property with value <c>"ref readonly"</c> value.
	/// </para>
	/// </remarks>
	public string? RefKind { get; init; }
}
