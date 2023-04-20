namespace System.CommandLine.Annotations;

/// <summary>
/// Represents an attribute that is applied to a command type, indicating the usage of the current root command.
/// </summary>
/// <param name="example">Indicates the example command.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed partial class UsageAttribute([PrimaryConstructorParameter] string example) : Attribute
{
	/// <summary>
	/// <para>
	/// Indicates whether the example command is fact, which means whether the current example command
	/// contains any wildcards or fuzzy argument patterns.
	/// </para>
	/// <para>The default value is <see langword="false"/>.</para>
	/// </summary>
	/// <remarks>
	/// Fuzzy argument patterns:
	/// <list type="table">
	/// <listheader>
	/// <term>Patterns</term>
	/// <description>Description</description>
	/// </listheader>
	/// <item>
	/// <term><c><![CDATA[<name>]]></c></term>
	/// <description>
	/// Indicates the arguments that is passed whose main idea is surrounded with the name "<c>name</c>".
	/// </description>
	/// </item>
	/// <item>
	/// <term><c>{a|b|c|...}</c></term>
	/// <description>
	/// Indicates you should choose a value from the given values inside the curly brace.
	/// </description>
	/// </item>
	/// <item>
	/// <term><c>[-argName value]</c></term>
	/// <description>
	/// Indicates the current value is optional, which means the current argument "<c>argName</c>"
	/// has a default value even if you don't assign to it.
	/// </description>
	/// </item>
	/// </list>
	/// The wildcards:
	/// <list type="table">
	/// <listheader>
	/// <term>Wildcard</term>
	/// <description>Description</description>
	/// </listheader>
	/// <item>
	/// <term>Question mark <c>?</c></term>
	/// <description>Indicates the current argument can only appear no more than once.</description>
	/// </item>
	/// <item>
	/// <term>Star <c>*</c></term>
	/// <description>Indicates the current argument can appear arbitrary times.</description>
	/// </item>
	/// <item>
	/// <term>Plus sign <c>+</c></term>
	/// <description>Indicates the current argument can appear at least once.</description>
	/// </item>
	/// </list>
	/// </remarks>
	public bool IsPattern { get; init; } = false;

	/// <summary>
	/// Indicates the description of the example command.
	/// </summary>
	/// <remarks><b>
	/// This property can be <see langword="null"/>. However, both properties <see cref="Description"/> and <see cref="DescriptionResourceKey"/>
	/// cannot be <see langword="null"/>.
	/// </b></remarks>
	/// <seealso cref="Description"/>
	/// <seealso cref="DescriptionResourceKey"/>
	public string? Description { get; init; }

	/// <summary>
	/// Indicates the description key of the example command stored in resource dictionary.
	/// </summary>
	/// <remarks>
	/// <inheritdoc cref="Description" path="/remarks"/>
	/// </remarks>
	/// <seealso cref="Description"/>
	/// <seealso cref="DescriptionResourceKey"/>
	public string? DescriptionResourceKey { get; init; }
}
