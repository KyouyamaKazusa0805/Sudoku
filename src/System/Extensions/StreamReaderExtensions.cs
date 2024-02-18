// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// https://source.dot.net/#System.Private.CoreLib/src/libraries/System.Private.CoreLib/src/System/IO/StreamReader.cs,f5c6f6ec6c1c6eb9

namespace System.IO;

/// <summary>
/// Provides with extension methods on <see cref="StreamReader"/>.
/// </summary>
/// <seealso cref="StreamReader"/>
public static class StreamReaderExtensions
{
	/// <summary>
	/// Determines whether a file ends with new line character.
	/// </summary>
	/// <param name="this">The <see cref="StreamReader"/> instance.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	/// <remarks><i>
	/// This method only supports for Windows now. For other OS platforms, this method cannot determine the end line characters
	/// because I have already forgotten them...
	/// </i></remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[SupportedOSPlatform("windows")]
	public static bool EndsWithNewLine(this StreamReader @this)
		=> @this.BaseStream.Length >= 2
		&& @this.BaseStream.Seek(-2, SeekOrigin.End) is var _
		&& (ReadOnlySpan<char>)[(char)@this.Read(), (char)@this.Read()] is "\r\n";

	/// <summary>
	/// Reads the previous line of the stream.
	/// </summary>
	/// <param name="this">The <see cref="StreamReader"/> instance.</param>
	/// <returns>The previous line.</returns>
	/// <seealso href="https://source.dot.net/#System.Private.CoreLib/src/libraries/System.Private.CoreLib/src/System/IO/StreamReader.cs,f5c6f6ec6c1c6eb9">
	/// Source for method <see cref="StreamReader.ReadLine"/>
	/// </seealso>
	public static string? ReadPreviousLine(this StreamReader @this)
	{
		// TODO: Copied from source. Code will be modified later.

		ThrowIfDisposed(@this);
		CheckAsyncTaskInProgress(@this);

		if (CharPos(@this) == CharLen(@this))
		{
			if (ReadBuffer(@this) == 0)
			{
				return null;
			}
		}

		var sb = new StringBuilder();
		do
		{
			// Look for '\r' or '\n'.
			var charBufferSpan = (ReadOnlySpan<char>)CharBuffer(@this).AsSpan(CharPos(@this), CharLen(@this) - CharPos(@this));
			var idxOfNewline = charBufferSpan.IndexOfAny('\r', '\n');
			if (idxOfNewline >= 0)
			{
				var retVal = sb.Length == 0
					? new string(charBufferSpan[..idxOfNewline])
					: string.Concat(sb.ToString().AsSpan(), charBufferSpan[..idxOfNewline]);
				var matchedChar = charBufferSpan[idxOfNewline];
				CharPos(@this) += idxOfNewline + 1;

				// If we found '\r', consume any immediately following '\n'.
				if (matchedChar == '\r')
				{
					if (CharPos(@this) < CharLen(@this) || @this.ReadBufferPrevious() > 0)
					{
						if (CharBuffer(@this)[CharPos(@this)] == '\n')
						{
							CharPos(@this)++;
						}
					}
				}

				return retVal;
			}

			// We didn't find '\r' or '\n'. Add it to the StringBuilder and loop until we reach a newline or EOF.
			sb.Append(charBufferSpan);
		} while (@this.ReadBufferPrevious() > 0);

		return sb.ToString();
	}

	private static int ReadBufferPrevious(this StreamReader @this)
	{
		CharLen(@this) = 0;
		CharPos(@this) = 0;

		if (!CheckPreamble(@this))
		{
			ByteLen(@this) = 0;
		}

		var eofReached = false;
		do
		{
			if (CheckPreamble(@this))
			{
				var len = Stream(@this).Read(ByteBuffer(@this), BytePos(@this), ByteBuffer(@this).Length - BytePos(@this));
				if (len == 0)
				{
					eofReached = true;
					break;
				}

				ByteLen(@this) += len;
			}
			else
			{
				ByteLen(@this) = Stream(@this).Read(ByteBuffer(@this), 0, ByteBuffer(@this).Length);
				if (ByteLen(@this) == 0)
				{
					eofReached = true;
					break;
				}
			}

			// _isBlocked == whether we read fewer bytes than we asked for.
			// Note we must check it here because CompressBuffer or DetectEncoding will change byteLen.
			IsBlocked(@this) = ByteLen(@this) < ByteBuffer(@this).Length;

			// Check for preamble before detect encoding. This is not to override the
			// user supplied Encoding for the one we implicitly detect. The user could
			// customize the encoding which we will loose, such as ThrowOnError on UTF8
			if (IsPreamble(@this))
			{
				continue;
			}

			// If we're supposed to detect the encoding and haven't done so yet, do it.
			// Note this may need to be called more than once.
			if (DetectEncoding_Field(@this) && ByteLen(@this) >= 2)
			{
				DetectEncoding(@this);
			}

			CharLen(@this) = Decoder(@this).GetChars(ByteBuffer(@this), 0, ByteLen(@this), CharBuffer(@this), 0, flush: false);
		} while (CharLen(@this) == 0);

		if (eofReached)
		{
			// EOF has been reached - perform final flush.
			// We need to reset BytePos(@this) and ByteLen(@this) just in case we hadn't finished processing the preamble before we reached EOF.
			CharLen(@this) = Decoder(@this).GetChars(ByteBuffer(@this), 0, ByteLen(@this), CharBuffer(@this), 0, flush: true);
			BytePos(@this) = 0;
			ByteLen(@this) = 0;
		}

		return CharLen(@this);
	}

	[UnsafeAccessor(UnsafeAccessorKind.Method, Name = nameof(ThrowIfDisposed))]
	private static extern void ThrowIfDisposed(StreamReader @this);

	[UnsafeAccessor(UnsafeAccessorKind.Method, Name = nameof(CheckAsyncTaskInProgress))]
	private static extern void CheckAsyncTaskInProgress(StreamReader @this);

	[UnsafeAccessor(UnsafeAccessorKind.Method, Name = nameof(DetectEncoding))]
	private static extern void DetectEncoding(StreamReader @this);

	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_checkPreamble")]
	private static extern ref bool CheckPreamble(StreamReader @this);

	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_isBlocked")]
	private static extern ref bool IsBlocked(StreamReader @this);

	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_detectEncoding")]
	private static extern ref bool DetectEncoding_Field(StreamReader @this);

	[UnsafeAccessor(UnsafeAccessorKind.Method, Name = nameof(IsPreamble))]
	private static extern bool IsPreamble(StreamReader @this);

	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_byteBuffer")]
	private static extern ref byte[] ByteBuffer(StreamReader @this);

	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_charBuffer")]
	private static extern ref char[] CharBuffer(StreamReader @this);

	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_charPos")]
	private static extern ref int CharPos(StreamReader @this);

	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_bytePos")]
	private static extern ref int BytePos(StreamReader @this);

	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_charLen")]
	private static extern ref int CharLen(StreamReader @this);

	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_byteLen")]
	private static extern ref int ByteLen(StreamReader @this);

	[UnsafeAccessor(UnsafeAccessorKind.Method, Name = nameof(ReadBuffer))]
	private static extern int ReadBuffer(StreamReader @this);

	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_encoding")]
	private static extern ref Encoding Encoding(StreamReader @this);

	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_decoder")]
	private static extern ref Decoder Decoder(StreamReader @this);

	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_stream")]
	private static extern ref Stream Stream(StreamReader @this);
}
