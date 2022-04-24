namespace System.CommandLine.Annotations;

/// <summary>
/// Represents an attriubute that is applied to a command type, indicating the usage of the current root command.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class UsageAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="UsageAttribute"/> instance via the specified example command and the description.
	/// </summary>
	/// <param name="example">The example command.</param>
	/// <param name="description">The description.</param>
	public UsageAttribute(string example, string? description = null)
		=> (ExampleCommand, Description) = (example, description);


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
	/// <description>Indicates the current argument can appear arbitary times.</description>
	/// </item>
	/// <item>
	/// <term>Plus sign <c>+</c></term>
	/// <description>Indicates the current argument can appear at least once.</description>
	/// </item>
	/// </list>
	/// </remarks>
	[MemberNotNullWhen(false, nameof(Description))]
	public bool IsPattern { get; init; } = false;

	/// <summary>
	/// Indicates the example command.
	/// </summary>
	public string ExampleCommand { get; }

	/// <summary>
	/// Indicates the description of the example command.
	/// </summary>
	public string? Description { get; }
}
