namespace System.CommandLine.Annotations;

/// <summary>
/// Represents an attribute type that is applied to a property in the option type,
/// indicating the property receives the value corresponded to a certain command line argument.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class CommandAttribute : CommandLineAttributeBase
{
	/// <summary>
	/// Initializes a <see cref="CommandAttribute"/> instance via the specified short name and the full name.
	/// </summary>
	/// <param name="shortName">The short name of the command.</param>
	/// <param name="fullName">The full name of the command. The command should not contain the prefix.</param>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="shortName"/> is not a letter.
	/// </exception>
	/// <exception cref="ArgumentException">
	/// Throws when the argument <paramref name="fullName"/> doesn't start with a letter, or a hyphen.
	/// </exception>
	public CommandAttribute(char shortName, string fullName) =>
		(ShortName, FullName) = (
			shortName is >= 'A' and <= 'Z' or >= 'a' and <= 'z'
				? shortName
				: throw new ArgumentOutOfRangeException(nameof(shortName)),
			fullName is [>= 'A' and <= 'Z' or >= 'a' and <= 'z' or '-', ..]
				? fullName
				: throw new ArgumentException(
					"The argument must start with a letter or a hyphen.",
					nameof(fullName))
		);


	/// <summary>
	/// Indicates the short name.
	/// </summary>
	public char ShortName { get; }

	/// <summary>
	/// Indicates the full name.
	/// </summary>
	public string FullName { get; }
}
