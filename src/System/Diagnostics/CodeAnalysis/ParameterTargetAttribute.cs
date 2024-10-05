namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents the base type of attribute target.
/// </summary>
public abstract class ParameterTargetAttribute : Attribute
{
	/// <summary>
	/// Indicates the accessibility of the generated member.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This property controls the case when you want to customize the accessibility of generated member.
	/// For example, the value is <c>"private"</c>, the generated member will automatically insert the accessibility modifier
	/// into property declaration:
	/// <code><![CDATA[
	/// private int Parameter { get; } = parameter;
	/// ]]></code>
	/// </para>
	/// <para>
	/// Also, you can append not only 1 word. For example, the combination <c>"protected internal readonly unsafe"</c> is also acceptable.
	/// </para>
	/// <para>By default, the accessibility is <see langword="private"/> for fields and <see langword="public"/> for properties.</para>
	/// </remarks>
	[DisallowNull]
	public string? Accessibility { get; init; }

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
	[DisallowNull]
	public string? NamingRule { get; init; }
}
