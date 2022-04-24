namespace System.CommandLine.Annotations;

/// <summary>
/// Represents a double-argument command. The command requires an argument name, following with a real vlaue,
/// such as "<c>-g grid</c>".
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class DoubleArgumentsCommandAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="DoubleArgumentsCommandAttribute"/> instance via the specified short name and the full name.
	/// </summary>
	/// <param name="shortName">The short name of the command.</param>
	/// <param name="fullName">The full name of the command. The command should not contain the prefix.</param>
	/// <param name="description">The description of the command.</param>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="shortName"/> is not a letter.
	/// </exception>
	/// <exception cref="ArgumentException">
	/// Throws when the argument <paramref name="fullName"/> doesn't start with a letter, or a hyphen.
	/// </exception>
	public DoubleArgumentsCommandAttribute(char shortName, string fullName, string description)
		=> (ShortName, FullName, Description) = (shortName, fullName, description);


	/// <summary>
	/// <para>Indicates whether the command is required.</para>
	/// <para>The default value is <see langword="false"/>.</para>
	/// </summary>
	public bool IsRequired { get; init; } = false;

	/// <summary>
	/// Indicates the short name.
	/// </summary>
	public char ShortName { get; }

	/// <summary>
	/// Indicates the full name.
	/// </summary>
	public string FullName { get; }

	/// <summary>
	/// Indicates the description of the command.
	/// </summary>
	public string Description { get; }
}
