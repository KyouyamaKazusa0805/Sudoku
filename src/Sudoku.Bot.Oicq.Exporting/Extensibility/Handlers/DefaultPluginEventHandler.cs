#nullable enable

using static Sudoku.Bot.Oicq.CommandStringComparer;

namespace Sudoku.Bot.Oicq.Extensibility.Handlers;

/// <summary>
/// Defines a default event handler.
/// </summary>
public sealed class DefaultPluginEventHandler : IPluginEventHandler
{
	/// <summary>
	/// Indicates the routing methods.
	/// </summary>
	private static readonly (Func<string, bool>, Action<AmiableMessageEventArgs>)[] Routing =
	{
		(static msg => SplitCommand(R["CommandTest"]).Any(e => AreEqual(e, msg)), TestConnection),
		(static msg => SplitCommand(R["CommandHelp"]).Any(e => AreEqual(e, msg)), Help)
	};


	/// <inheritdoc/>
	public AmiableEventType EventType => AmiableEventType.Group;


	/// <inheritdoc/>
	public void Process(AmiableEventArgs e)
	{
		if (e is not AmiableMessageEventArgs { GroupId: var groupId, RawMessage: { } rawMessage } args)
		{
			return;
		}

		if (!IsSupportedGroup(groupId))
		{
			return;
		}

		foreach (var (predicate, routing) in Routing)
		{
			if (predicate(rawMessage))
			{
				routing(args);

				break;
			}
		}
	}


	/// <summary>
	/// To test the connection.
	/// </summary>
	/// <param name="e">The event arguments provided.</param>
	private static void TestConnection(AmiableMessageEventArgs e) => e.SendMessage(R["CommandTestReply"]!);

	/// <summary>
	/// To display the help text.
	/// </summary>
	/// <param name="e">The event arguments provided.</param>
	private static void Help(AmiableMessageEventArgs e) => e.SendMessage(Regex.Unescape(R["CommandHelpReply"]!));

	/// <summary>
	/// Determines whether the current group is supported.
	/// </summary>
	/// <param name="groupId">The group QQ number.</param>
	/// <returns>A <see cref="bool"/> value indicating the result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsSupportedGroup(long groupId)
		=> SplitCommand(R["SupportedGroups"]).Any(e => long.TryParse(e, out long v) && v == groupId);

	/// <summary>
	/// Split the command.
	/// </summary>
	/// <param name="commandStr">The full command string.</param>
	/// <param name="separator">The separator.</param>
	/// <returns>The split result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static string[] SplitCommand(string? commandStr, char separator = ',')
		=> commandStr?.Split(separator) ?? Array.Empty<string>();
}
