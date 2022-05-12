namespace Sudoku.Bot.Communication;

/// <summary>
/// Defines a command that is used for the handling by a bot.
/// The type is an abstraction on bot operation.
/// </summary>
/// <param name="Name">Indicates the name of the command.</param>
/// <param name="Callback">
/// Indicates the callback function to handle the message around this commmand.
/// For example:
/// <code><![CDATA[
/// // A lambda that repeats the message asynchronously.
/// async (sender, message) =>
/// {
///     if (!string.IsNullOrWritespace(message))
///         await sender.ReplyAsync(message);
/// }
/// ]]></code>
/// </param>
/// <param name="Rule">Indicates the message matching rule.</param>
/// <param name="RequiresAdministratorPermission">
/// <para>Indicates whether the command requires administrators using it.</para>
/// <para>The default value is <see langword="false"/>.</para>
/// </param>
/// <param name="Tag">
/// <para>
/// Indicates the extra information to describes the command.
/// The property is an extra value that won't be used by the framework
/// but helpful for you to define your extra rules for the handling.
/// </para>
/// <para>The default value is <see langword="null"/>.</para>
/// </param>
public sealed record class Command(
	string Name, CommandCallback Callback, Regex? Rule = null,
	bool RequiresAdministratorPermission = false, object? Tag = null)
{
	/// <summary>
	/// Initializes a <see cref="Command"/> instance with the specified data.
	/// </summary>
	/// <param name="name">
	/// Indicates the name of the command. The name should match with the configuration on QQ bot official site.
	/// </param>
	/// <param name="callback">Indicates the callback method.</param>
	public Command(string name, CommandCallback callback) :
		this(name, callback, new($"""^{Regex.Escape(name)}\s*(?=\s|\d|\n|<@!\d+>|$)""", RegexOptions.Compiled))
	{
	}
}
