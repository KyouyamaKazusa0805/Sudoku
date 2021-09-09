namespace Microsoft.CodeAnalysis;

/// <summary>
/// Provides extension methods on <see cref="GeneratorInitializationContext"/>.
/// </summary>
/// <seealso cref="GeneratorInitializationContext"/>
internal static class GeneratorInitializationContextExtensions
{
	/// <summary>
	/// Fast invokes the method
	/// <see cref="GeneratorInitializationContext.RegisterForSyntaxNotifications(SyntaxReceiverCreator)"/>.
	/// </summary>
	/// <typeparam name="TSyntaxReceiver">The type of the syntax receiver.</typeparam>
	/// <param name="this">The current initialization context.</param>
	/// <seealso cref="GeneratorInitializationContext.RegisterForSyntaxNotifications(SyntaxReceiverCreator)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void FastRegister<TSyntaxReceiver>(this ref GeneratorInitializationContext @this)
	where TSyntaxReceiver : ISyntaxReceiver, new() =>
		@this.RegisterForSyntaxNotifications(static () => new TSyntaxReceiver());
}
