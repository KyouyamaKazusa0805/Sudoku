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
	[SupportedOSPlatform(PlatformNames.Windows)]
	public static bool EndsWithNewLine(this StreamReader @this)
		=> @this.BaseStream.Length >= 2
		&& @this.BaseStream.Seek(-2, SeekOrigin.End) is var _
		&& (ReadOnlySpan<char>)[(char)@this.Read(), (char)@this.Read()] is "\r\n";
}
