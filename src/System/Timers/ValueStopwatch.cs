// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// https://github.com/dotnet/aspnetcore/blob/a450cb69b5e4549f5515cdb057a68771f56cefd7/src/Shared/ValueStopwatch/ValueStopwatch.cs

namespace System.Timers;

/// <summary>
/// Defines a stopwatch that uses <see langword="struct"/> instead of <see langword="class"/> to optimize the performance.
/// </summary>
public readonly ref partial struct ValueStopwatch
{
	/// <summary>
	/// The read-only value that indicates the formula converting from timestamp to ticks.
	/// </summary>
	private static readonly double TimestampToTicks = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;


	/// <summary>
	/// The inner timestamp.
	/// </summary>
	private readonly long _startTimestamp;


	/// <summary>
	/// Initializes a <see cref="ValueStopwatch"/> instance.
	/// </summary>
	[FileAccessOnly]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ValueStopwatch()
	{
	}

	/// <summary>
	/// Initializes a <see cref="ValueStopwatch"/> instance via the current timestamp.
	/// </summary>
	/// <param name="startTimestamp">The timestamp value that is represented as a <see cref="long"/> value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private ValueStopwatch(long startTimestamp) => _startTimestamp = startTimestamp;


	/// <summary>
	/// Determines whether the current stopwatch is currently active.
	/// </summary>
	public bool IsActive
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _startTimestamp != 0;
	}


	[GeneratedOverriddingMember(GeneratedEqualsBehavior.RefStructDefault)]
	public override partial bool Equals(object? obj);

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.RefStructDefault)]
	public override partial int GetHashCode();

	/// <summary>
	/// Try to get the elapsed time.
	/// </summary>
	/// <returns>The elapsed time, specified as a <see cref="TimeSpan"/> instance.</returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when the current stopwatch is not active at present.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TimeSpan GetElapsedTime()
		=> IsActive
			? new((long)(TimestampToTicks * (Stopwatch.GetTimestamp() - _startTimestamp)))
			// Start timestamp can't be zero in an initialized ValueStopwatch.
			// It would have to be literally the first thing executed when the machine boots to be 0.
			// So it being 0 is a clear indication of default(ValueStopwatch).
			: throw new InvalidOperationException(
				$"""
				An uninitialized, or 'default({nameof(ValueStopwatch)})', 
				{nameof(ValueStopwatch)} cannot be used to get elapsed time.
				""".RemoveLineEndings()
			);


	/// <summary>
	/// Try to get a new <see cref="ValueStopwatch"/> that is running now.
	/// </summary>
	/// <returns>An instance of type <see cref="ValueStopwatch"/> that is running now.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ValueStopwatch StartNew() => new(Stopwatch.GetTimestamp());
}
