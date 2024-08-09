namespace System.Threading.Tasks;

/// <summary>
/// Represents an awaiter for <see cref="TimeSpan"/> instance.
/// </summary>
/// <param name="_awaiter">The base awaiter instance.</param>
/// <seealso cref="TimeSpan"/>
public readonly struct TimeSpanAwaiter(TaskAwaiter _awaiter) : INotifyCompletion
{
	/// <inheritdoc cref="TaskAwaiter.IsCompleted"/>
	public bool IsCompleted
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _awaiter.IsCompleted;
	}


	/// <inheritdoc cref="TaskAwaiter.GetResult"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void GetResult() => _awaiter.GetResult();

	/// <inheritdoc cref="TaskAwaiter.OnCompleted(Action)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void OnCompleted(Action continuation) => _awaiter.OnCompleted(continuation);
}
