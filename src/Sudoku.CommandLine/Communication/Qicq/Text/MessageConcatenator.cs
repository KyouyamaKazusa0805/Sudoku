namespace Sudoku.Communication.Qicq.Text;

/// <summary>
/// Defines a message concatenator instance.
/// </summary>
[InterpolatedStringHandler]
public sealed class MessageConcatenator
{
	/// <summary>
	/// Initializes a <see cref="MessageConcatenator"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
	public MessageConcatenator(int _, int __)
	{
	}


	/// <summary>
	/// The message blocks.
	/// </summary>
	public MessageChain MessageBlocks { get; private set; } = new();


	/// <summary><b><i>
	/// Unused method. Keep this method empty.
	/// </i></b></summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AppendLiteral(string _)
	{
	}

	/// <summary>
	/// Appends a new text.
	/// </summary>
	/// <param name="textMessage">The plain text message.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AppendFormatted(string textMessage) => MessageBlocks += textMessage;

	/// <summary>
	/// Appends a new formatted message.
	/// </summary>
	/// <typeparam name="T">The type of the message.</typeparam>
	/// <param name="message">The message instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AppendFormatted<T>(T message) where T : MessageBase => MessageBlocks += message;
}
