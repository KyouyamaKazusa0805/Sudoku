// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// https://github.com/dotnet/aspnetcore/blob/a450cb69b5e4549f5515cdb057a68771f56cefd7/src/Shared/ValueStopwatch/ValueStopwatch.cs

namespace System.Timers;

/// <summary>
/// Defines a stopwatch that uses <see langword="struct"/> instead of <see langword="class"/> to optimize the performance.
/// </summary>
/// <param name="startTimestamp">The timestamp value that is represented as a <see cref="long"/> value.</param>
[Equals]
[GetHashCode]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly ref partial struct ValueStopwatch([Data(DataMemberKinds.Field)] long startTimestamp)
{
	/// <summary>
	/// The error information describing the type is uninitialized.
	/// </summary>
	private const string ErrorInfo_TypeIsUninitialized =
		$"An uninitialized, or 'default({nameof(ValueStopwatch)})', {nameof(ValueStopwatch)} cannot be used for getting elapsed time.";


	/// <summary>
	/// The read-only value that indicates the formula converting from timestamp to ticks.
	/// </summary>
	private static readonly double TimestampToTicks = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;


	/// <summary>
	/// Try to get the elapsed time.
	/// </summary>
	/// <returns>The elapsed time, specified as a <see cref="TimeSpan"/> instance.</returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when the current stopwatch is not active at present.
	/// </exception>
	public TimeSpan ElapsedTime
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _startTimestamp != 0
			? new((long)(TimestampToTicks * (Stopwatch.GetTimestamp() - _startTimestamp)))
			: throw new InvalidOperationException(ErrorInfo_TypeIsUninitialized);
	}


	/// <summary>
	/// Indicates a new instance. Use this property to start a new stopwatch instead of expressions like
	/// <see langword="new"/> <see cref="ValueStopwatch"/>() and <see langword="default"/>(<see cref="ValueStopwatch"/>).
	/// </summary>
	public static ValueStopwatch NewInstance
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new(Stopwatch.GetTimestamp());
	}
}
