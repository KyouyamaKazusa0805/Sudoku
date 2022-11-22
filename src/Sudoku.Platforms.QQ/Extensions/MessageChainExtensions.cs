namespace Mirai.Net.Data.Messages;

/// <summary>
/// Provides with extension methods on <see cref="MessageChain"/>.
/// </summary>
/// <seealso cref="MessageChain"/>
internal static class MessageChainExtensions
{
	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Deconstruct(this MessageChain @this, out string? plainMessage, out string? trimmedPlainMessage, out string[] messageSegments)
	{
		plainMessage = @this.GetPlainMessage();
		(trimmedPlainMessage, messageSegments) = (plainMessage?.Trim(), @this.GetSeparatedPlainMessage().ToArray());
	}
}
