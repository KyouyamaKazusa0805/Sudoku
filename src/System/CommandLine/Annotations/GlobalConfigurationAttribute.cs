namespace System.CommandLine.Annotations;

/// <summary>
/// Defines an attribute type that provides with the global configuration.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
public sealed class GlobalConfigurationAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="GlobalConfigurationAttribute"/> instance.
	/// </summary>
	public GlobalConfigurationAttribute() : base()
	{
	}


	/// <summary>
	/// <para>
	/// Indicates the prefix of the full command name. For example, the characters <c>--</c>
	/// in the command name <c>--say-hello</c>.
	/// </para>
	/// <para>
	/// The default value is <c>"--"</c>. Generally the value can be <c>"--"</c> or <c>"/"</c>.
	/// <b>Please do not assign empty string.</b>
	/// </para>
	/// </summary>
	public string FullCommandNamePrefix { get; init; } = "--";

	/// <summary>
	/// <para>
	/// Indicates the prefix of the short command name. For example, the character <c>-</c>
	/// in the command name <c>-s</c>.
	/// </para>
	/// <para>
	/// The default value is <c>"-"</c>. Generally the value should be <c>"-"</c>.
	/// <b>Please do not assign empty string.</b>
	/// </para>
	/// </summary>
	public string ShortCommandNamePrefix { get; init; } = "-";
}
