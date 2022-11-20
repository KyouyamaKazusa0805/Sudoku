namespace Sudoku.Communication.Qicq.Text;

/// <summary>
/// Provides with extension methods on <see cref="GroupMessageReceiver"/>.
/// </summary>
/// <seealso cref="GroupMessageReceiver"/>
public static class GroupMessageReceiverExtensions
{
	/// <summary>
	/// Sends a message using <see cref="MessageConcatenator"/> type to concatenate multiple <see cref="MessageBase"/> instances into one.
	/// </summary>
	/// <param name="this">The <see cref="GroupMessageReceiver"/> instance.</param>
	/// <param name="messageCantenator">
	/// <para>The message cantenator instance.</para>
	/// <para>
	/// Although this argument is of type <see cref="MessageConcatenator"/> instance,
	/// you can also pass an interpolated string into here - it will be automatically converted into the target value.
	/// </para>
	/// </param>
	/// <returns>The task that handles the operation.</returns>
	/// <remarks>
	/// For example, using raw string literal to combine multiple <see cref="MessageBase"/> instances; all characters as literal ones
	/// will be ignored:
	/// <code><![CDATA[
	/// await e.SendMessageAsync(
	///     $"""
	///     {new AtMessage(id)}
	///     {" "}
	///     {"You won!"}
	///     """
	/// );
	/// ]]></code>
	/// This is equivalent to
	/// <code><![CDATA[
	/// await e.SendMessageAsync(new AtMessage(id) + new PlainMessage(" ") + new PlainMessage("You won!"));
	/// ]]></code>
	/// You can also use plain cantenation:
	/// <code><![CDATA[
	/// await e.SendMessageAsync($"{new AtMessage(id)}{" "}{"You won!"}");
	/// ]]></code>
	/// Using this method can emit some calculation to optimize the performance.
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static async Task<string> SendMessageAsync(
		this GroupMessageReceiver @this,
		[InterpolatedStringHandlerArgument] MessageConcatenator messageCantenator
	) => await @this.SendMessageAsync(messageCantenator.MessageBlocks);
}
