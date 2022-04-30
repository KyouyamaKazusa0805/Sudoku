namespace Sudoku.Bot.Oicq.Exporting.PluginEventHandlers;

/// <summary>
/// Defines a test event handler.
/// </summary>
public sealed class TestPluginEventHandler : IPluginEventHandler
{
	/// <inheritdoc/>
	public AmiableEventType EventType => AmiableEventType.Group;


	/// <inheritdoc/>
	public void Process(AmiableEventArgs e)
	{
		if (
			e is not AmiableMessageEventArgs
			{
				UserId: 747507738L,
				GroupId: 663888612L or 924849321L,
				RawMessage: { } rawMessage
			} args
			|| rawMessage.ToLower() != "#debug"
		)
		{
			return;
		}

		args.SendMessage("Test successful.");
	}
}
