namespace System.Threading.Tasks;

/// <summary>
/// Represents an awaiter for <see cref="TimeSpan"/> instance.
/// </summary>
/// <param name="awaiter">The base awaiter instance.</param>
/// <seealso cref="TimeSpan"/>
public readonly struct TimeSpanAwaiter(TaskAwaiter awaiter) : INotifyCompletion
{
	/// <inheritdoc cref="TaskAwaiter.IsCompleted"/>
	public bool IsCompleted
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => awaiter.IsCompleted;
	}


	/// <inheritdoc cref="TaskAwaiter.GetResult"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void GetResult() => awaiter.GetResult();

	/// <inheritdoc cref="TaskAwaiter.OnCompleted(Action)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void OnCompleted(Action continuation) => awaiter.OnCompleted(continuation);
}
